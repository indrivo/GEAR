using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Entities.Abstractions;
using GR.Entities.Abstractions.Constants;
using GR.Entities.Abstractions.Models.Tables;
using GR.Forms.Abstractions;
using GR.Forms.Abstractions.Models.FormModels;
using GR.Forms.Abstractions.ViewModels.FormViewModels;
using GR.Forms.Data;
using Settings = GR.Forms.Abstractions.Models.FormModels.Settings;

namespace GR.Forms
{
    public class FormService<TContext> : IFormService where TContext : FormDbContext, IFormContext
    {
        private readonly TContext _context;

        private readonly IEntityContext _entityContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityContext"></param>
        public FormService(TContext context, IEntityContext entityContext)
        {
            _context = context;
            _entityContext = entityContext;
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ResultModel DeleteForm(Guid id)
        {
            var form = _context.Forms.FirstOrDefault(x => x.Id == id);
            if (form == null)
                return new ResultModel
                {
                    Errors = new List<IErrorModel>
                    {
                        new ErrorModel("", "Form not found")
                    }
                };
            _context.Forms.Remove(form);
            try
            {
                _context.SaveChanges();
                return new ResultModel
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ResultModel
                {
                    Errors = new List<IErrorModel>
                    {
                        new ErrorModel(string.Empty, e.ToString())
                    }
                };
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// Get type of form
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        public virtual FormType GetTypeByFormId(Guid formId)
        {
            var form = _context.Forms.Include(x => x.Type).FirstOrDefault(x => x.Id == formId);
            if (form == null) return null;
            return form.TypeId == Guid.Empty ? null : form.Type;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get form by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ResultModel<FormViewModel> GetFormById(Guid id)
        {
            var response = _context.Forms
                .Include(x => x.Settings)
                .Include(x => x.Type)
                .Include(x => x.Rows)
                .ThenInclude(x => x.Attrs)
                .Include(x => x.Columns)
                .ThenInclude(x => x.Config)
                .Include(x => x.Fields)
                .ThenInclude(x => x.Attrs)
                .Include(x => x.Fields)
                .ThenInclude(x => x.Attrs)
                .Include(x => x.Stages)
                .FirstOrDefault(x => x.Id == id);

            if (response == null)
                return new ResultModel<FormViewModel>
                {
                    Errors = new List<IErrorModel>
                    {
                        new ErrorModel(string.Empty, "Form not found")
                    }
                };

            var form = new FormViewModel
            {
                Id = response.Id
            };
            var fields = new Dictionary<Guid, FieldViewModel>();
            var columns = new Dictionary<Guid, ColumnViewModel>();
            var stages = new Dictionary<Guid, StageViewModel>();
            var rows = new Dictionary<Guid, RowViewModel>();

            var formStages = response.Stages.ToList();
            foreach (var stage in formStages)
            {
                var r = new List<Guid>(_context.StageRows
                    .Where(x => x.StageId == stage.Id)
                    .Select(x => x.RowId));

                stages.Add(stage.Id, new StageViewModel
                {
                    Id = stage.Id,
                    Settings = new SettingsViewModel(),
                    Rows = r
                });
            }

            var formRows = response.Rows.ToList();
            foreach (var row in formRows)
            {
                var c = new List<Guid>(_context
                    .RowColumns
                    .Where(x => x.RowId == row.Id).Select(x => x.ColumnId));

                var rowAttrs = _context.Attrs
                    .Where(x => x.RowId == row.Id)
                    .ToDictionary(x => x.Key, v => v.Value ?? "");

                var config = _context.Configs.FirstOrDefault(x => x.Id == row.ConfigId);
                rows.Add(row.Id, new RowViewModel
                {
                    Id = row.Id,
                    Config = new ConfigViewModel
                    {
                        Fieldset = config?.Fieldset ?? false,
                        Label = config?.Label,
                        InputGroup = config?.InputGroup ?? false,
                        Legend = config?.Legend,
                        Width = config?.Width
                    },
                    Columns = c,
                    Attrs = rowAttrs
                });
            }

            var formColumns = response.Columns.ToList();
            foreach (var column in formColumns)
            {
                var f = new List<Guid>(_context.ColumnFields
                    .Where(x => x.ColumnId == column.Id)
                    .OrderBy(x => x.Order)
                    .Select(x => x.FieldId));

                var config = _context.Configs.FirstOrDefault(x => x.Id == column.ConfigId);
                columns.Add(column.Id, new ColumnViewModel
                {
                    ClassName = column.ClassName,
                    Config = new ConfigViewModel
                    {
                        Fieldset = config?.Fieldset ?? false,
                        Label = config?.Label,
                        InputGroup = config?.InputGroup ?? false,
                        Legend = config?.Legend,
                        Width = config?.Width
                    },
                    Id = column.Id,
                    Fields = f
                });
            }

            var formFields = response.Fields.ToList();

            foreach (var field in formFields)
            {
                var config = _context.Configs.FirstOrDefault(x => x.Id == field.ConfigId);
                var fieldAttrs = _context.Attrs
                    .Where(x => x.FieldId == field.Id)
                    .ToDictionary(x => x.Key, v => v.Value ?? "");


                var meta = _context.Meta.FirstOrDefault(x => x.Id == field.MetaId);
                var disabledAttr = _context.DisabledAttrs.Where(x => x.ConfigId == config.Id).ToList();
                var configDisabled = new List<string>();
                var options = _context.Options.Where(x => x.FieldId == field.Id).ToList();
                var ops = new List<OptionsViewModel>();
                foreach (var dis in disabledAttr)
                {
                    configDisabled.Add(dis.Name);
                }

                foreach (var f in options)
                {
                    ops.Add(new OptionsViewModel
                    {
                        Label = f.Label,
                        Type = new List<AttrTagViewModel>
                        {
                            new AttrTagViewModel
                            {
                                Label = f.TypeLabel,
                                Value = f.Value,
                                Selected = true
                            }
                        },
                        Value = f.Value,
                        Selected = f.Selected
                    });
                }

                var type = field.Attrs?.FirstOrDefault(x => x.Key == "Type")?.Value;

                fields.Add(field.Id, new FieldViewModel
                {
                    Id = field.Id,
                    Config = new ConfigViewModel
                    {
                        Fieldset = config?.Fieldset ?? false,
                        Label = config?.Label,
                        InputGroup = config?.InputGroup ?? false,
                        Legend = config?.Legend,
                        Width = config?.Width,
                        DisabledAttrs = configDisabled,
                        HideLabel = field.Config.HideLabel,
                        Editable = field.Config.Editable
                    },
                    Attrs = fieldAttrs,
                    Content = field.Content,
                    FMap = field.FMap,
                    Tag = field.Tag,
                    Meta = new MetaViewModel
                    {
                        Group = meta?.Group,
                        Icon = meta?.Icon,
                        Id = meta?.Icon
                    },
                    Options = (type == "radio" || field.Tag == "select" || field.Tag == "button") ? ops : null,
                    Order = field.Order
                });
            }

            form.Columns = columns;
            form.Fields = fields.OrderBy(x => x.Value.Order)
                .ToDictionary(x => x.Key, x => x.Value);
            form.Rows = rows;
            form.Stages = stages;
            form.Settings = new SettingsViewModel();

            var result = new ResultModel<FormViewModel>
            {
                IsSuccess = true,
                Result = form
            };
            return result;
        }

        /// <inheritdoc>
        ///     <cref>description</cref>
        /// </inheritdoc>
        /// <summary>
        /// Create form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual ResultModel<Guid> CreateForm(FormCreateDetailsViewModel model)
        {
            var order = 0;
            try
            {
                var form = new Form
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    TableId = model.TableId,
                    PostUrl = model.PostUrl,
                    RedirectUrl = model.RedirectUrl
                };

                var settings = new Settings();
                var stages = new Collection<Stage>();
                var rows = new Collection<Row>();
                var columns = new Collection<Column>();
                var fields = new Collection<Field>();
                var stageRows = new Collection<StageRows>();
                var rowColumns = new Collection<RowColumn>();
                var columnFields = new Collection<ColumnField>();

                foreach (var stage in model.Model.Stages)
                {
                    stages.Add(new Stage
                    {
                        Settings = new Settings()
                    });

                    if (!stage.Value.Rows.Any()) continue;
                    foreach (var r in stage.Value.Rows)
                    {
                        foreach (var row in model.Model.Rows)
                        {
                            if (r != row.Value.Id) continue;
                            rows.Add(new Row
                            {
                                Attrs = row.Value.Attrs.Select(x => new Attrs
                                {
                                    Key = x.Key,
                                    Value = x.Value.ToString()
                                }).ToList(),
                                Config = new Config
                                {
                                    Width = row.Value.Config.Width,
                                    Fieldset = row.Value.Config.Fieldset,
                                    InputGroup = row.Value.Config.InputGroup,
                                    Label = row.Value.Config.Label,
                                    Legend = row.Value.Config.Legend
                                }
                            });
                            stageRows.Add(new StageRows
                            {
                                RowId = rows[rows.Count - 1].Id,
                                StageId = stages[stages.Count - 1].Id
                            });
                            if (!row.Value.Columns.Any()) continue;
                            foreach (var c in row.Value.Columns)
                            {
                                foreach (var column in model.Model.Columns)
                                {
                                    if (c != column.Value.Id) continue;
                                    columns.Add(new Column
                                    {
                                        ClassName = column.Value.ClassName,
                                        Config = new Config
                                        {
                                            Width = column.Value.Config.Width,
                                            Fieldset = column.Value.Config.Fieldset,
                                            InputGroup = column.Value.Config.InputGroup,
                                            Label = column.Value.Config.Label,
                                            Legend = column.Value.Config.Legend
                                        }
                                    });
                                    rowColumns.Add(new RowColumn
                                    {
                                        ColumnId = columns[columns.Count - 1].Id,
                                        RowId = rows[rows.Count - 1].Id
                                    });

                                    if (!column.Value.Fields.Any()) continue;
                                    foreach (var f in column.Value.Fields)
                                    {
                                        foreach (var field in model.Model.Fields)
                                        {
                                            if (f != field.Value.Id) continue;
                                            var disabled = new List<DisabledAttr>();
                                            if (field.Value.Config.DisabledAttrs != null)
                                            {
                                                disabled.AddRange(
                                                    field.Value.Config.DisabledAttrs.Select(dis =>
                                                        new DisabledAttr {Name = dis}));
                                            }

                                            var opt = new List<Option>();
                                            if (field.Value.Options != null)
                                            {
                                                foreach (var e in field.Value.Options)
                                                {
                                                    var o = new Option();

                                                    if (e.Type != null && e.Type.Any())
                                                    {
                                                        foreach (var d in e.Type)
                                                        {
                                                            if (!d.Selected) continue;
                                                            o.TypeLabel = d.Label;
                                                            o.TypeValue = d.Value;
                                                        }
                                                    }

                                                    if (e.ClassName != null &&
                                                        e.ClassName.Any())
                                                    {
                                                        foreach (var d in e.ClassName)
                                                        {
                                                            if (!d.Selected) continue;
                                                            o.TypeLabel = d.Label;
                                                            o.TypeValue = d.Value;
                                                        }
                                                    }

                                                    o.Label = e.Label;
                                                    o.Selected = e.Selected;
                                                    o.Value = e.Value;
                                                    opt.Add(o);
                                                }
                                            }

                                            //add new Field
                                            fields.Add(new Field
                                            {
                                                Order = order++,
                                                TableFieldId = field.Value.Attrs
                                                    ?.FirstOrDefault(x => x.Key == "tableFieldId").Value?.TryToGuid(),
                                                Attrs = field.Value.Attrs?.Select(x => new Attrs
                                                {
                                                    Key = x.Key,
                                                    Value = x.Value?.ToString()
                                                }).ToList(),
                                                Config = (field.Value.Config != null)
                                                    ? new Config
                                                    {
                                                        Fieldset = field.Value.Config.Fieldset,
                                                        InputGroup =
                                                            field.Value.Config.InputGroup,
                                                        Label = field.Value.Config.Label,
                                                        Legend = field.Value.Config.Legend,
                                                        Width = field.Value.Config.Width,
                                                        DisabledAttrs =
                                                            (disabled.Count > 0)
                                                                ? disabled
                                                                : null,
                                                        HideLabel =
                                                            field.Value.Config.HideLabel,
                                                        Editable = field.Value.Config.Editable
                                                    }
                                                    : null,
                                                FMap = field.Value.FMap,
                                                Meta = (field.Value.Meta != null)
                                                    ? new Meta
                                                    {
                                                        Group = field.Value.Meta.Group,
                                                        Icon = field.Value.Meta.Icon
                                                    }
                                                    : null,
                                                Tag = field.Value.Tag,
                                                Content = field.Value.Content,
                                                Options = opt
                                            });
                                            columnFields.Add(new ColumnField
                                            {
                                                ColumnId = columns[columns.Count - 1].Id,
                                                FieldId = fields[fields.Count - 1].Id,
                                                Order = fields.ElementAtOrDefault(fields.Count - 1).Order
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (model.EditMode)
                {
                    form.Author = model.Author;
                    form.Created = model.Created;
                    form.Changed = DateTime.Now;
                    form.ModifiedBy = model.ModifiedBy;
                }
                else
                {
                    form.Author = model.Author;
                    form.Created = DateTime.Now;
                    form.Changed = DateTime.Now;
                }

                form.Fields = fields;
                form.Rows = rows;
                form.Settings = settings;
                form.Columns = columns;
                form.Stages = stages;
                form.TypeId = model.FormTypeId;
                _context.Forms.Add(form);
                _context.StageRows.AddRange(stageRows);
                _context.RowColumns.AddRange(rowColumns);
                _context.ColumnFields.AddRange(columnFields);
                _context.SaveChanges();
                var result = new ResultModel<Guid>
                {
                    IsSuccess = true,
                    Result = form.Id
                };
                return result;
            }
            catch (Exception ex)
            {
                var result = new ResultModel<Guid>
                {
                    IsSuccess = false,
                    Errors = new List<IErrorModel>
                    {
                        new ErrorModel
                        {
                            Key = string.Empty,
                            Message = "Fail to create form, something was wrong!"
                        },
                        new ErrorModel
                        {
                            Key = string.Empty,
                            Message = ex.ToString()
                        }
                    }
                };
                return result;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Get entity fields
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public virtual JsonResult GetEntityFields(Guid tableId)
        {
            var fields = _entityContext.Table
                .Include(x => x.TableFields)
                .FirstOrDefault(x => !x.IsDeleted && x.Id == tableId)?.TableFields
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.DataType
                })
                .ToList();

            return new JsonResult(fields);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entitySchema"></param>
        /// <returns></returns>
        public virtual JsonResult GetEntityReferenceFields(string entityName, string entitySchema)
        {
            if (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(entitySchema))
                return new JsonResult(default(Collection<TableModelField>));
            var table = _entityContext.Table.Include(x => x.TableFields)
                .FirstOrDefault(x => x.Name == entityName && x.EntityType == entitySchema);
            if (table == null) return new JsonResult(default(Collection<TableModelField>));
            return new JsonResult(table.TableFields.Select(x => new
            {
                x.DataType,
                x.Id,
                x.Name
            }));
        }

        /// <inheritdoc />
        /// <summary>
        /// Get reference fields
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityFieldId"></param>
        /// <returns></returns>
        public virtual JsonResult GetReferenceFields(Guid? entityId, Guid? entityFieldId)
        {
            if (entityFieldId == null || entityId == null) return new JsonResult(default(Collection<TableModelField>));
            var field = _entityContext.TableFields
                .Include(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .FirstOrDefault(x => x.Id == entityFieldId && x.TableId == entityId);

            if (field == null) return new JsonResult(default(Collection<TableModelField>));
            var refEntity = field.TableFieldConfigValues.FirstOrDefault(x => x.TableFieldConfig.Code == "3000");
            if (refEntity == null) return new JsonResult(default(Collection<TableModelField>));
            var refEntitySchema = field.TableFieldConfigValues.FirstOrDefault(x => x.TableFieldConfig.Code == "9999");
            var table = _entityContext
                .Table
                .Include(x => x.TableFields)
                .FirstOrDefault(x => x.Name == refEntity.Value && x.EntityType == refEntitySchema.Value);
            if (table == null) return new JsonResult(default(Collection<TableModelField>));
            return new JsonResult(table.TableFields.Select(x => new
            {
                x.DataType,
                x.Id,
                x.Name
            }));
        }


        /// <inheritdoc />
        /// <summary>
        /// Get table fields
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public virtual JsonResult GetTableFields(Guid tableId)
        {
            var fields = _entityContext.TableFields
                .Include(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .Where(x => x.TableId == tableId);

            return new JsonResult(new ResultModel
            {
                IsSuccess = true,
                Result = fields.ToList()
            }, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        /// <inheritdoc />
        /// <summary>
        /// Generate form from entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="name"></param>
        /// <param name="redirectUrl"></param>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public virtual async Task<FormCreateDetailsViewModel> GenerateFormByEntity(Guid entityId, string name,
            string redirectUrl, string headerName)
        {
            if (entityId == Guid.Empty) return default;
            var entity = await _entityContext.Table.Include(x => x.TableFields)
                .FirstOrDefaultAsync(x => x.Id == entityId);
            if (entity == null) return default;
            var formType = _context.FormTypes.FirstOrDefault();
            if (formType == null) return default;
            var stageId = Guid.NewGuid();
            var rowId = Guid.NewGuid();
            var colId = Guid.Empty;
            var order = 0;
            var headId = Guid.NewGuid();
            var fields = new Dictionary<Guid, FieldViewModel>
            {
                {
                    headId,
                    new FieldViewModel
                    {
                        Id = headId,
                        Attrs = new Dictionary<string, string> {{"className", ""}},
                        Order = order++,
                        Config = new ConfigViewModel {Editable = true, HideLabel = true, Label = "Header"},
                        Content = headerName,
                        Meta = new MetaViewModel {Id = "header", Group = "html", Icon = "header"},
                        Tag = "h1",
                        Options = new List<OptionsViewModel>()
                    }
                }
            };

            foreach (var field in entity.TableFields)
            {
                var fieldConfig = new FormControl();
                switch (field.DataType)
                {
                    case TableFieldDataType.Guid:
                        fieldConfig = FormeoControls.Select;
                        break;
                    case TableFieldDataType.Boolean:
                        fieldConfig = FormeoControls.CheckBox;
                        break;
                    case TableFieldDataType.BigInt:
                    case TableFieldDataType.Decimal:
                    case TableFieldDataType.Int:
                        fieldConfig = FormeoControls.Number;
                        break;
                    case TableFieldDataType.String:
                        fieldConfig = FormeoControls.Text;
                        break;
                    case TableFieldDataType.Date:
                    case TableFieldDataType.DateTime:
                        fieldConfig = FormeoControls.Date;
                        break;
                }

                var key = Guid.NewGuid();
                fields.Add(key, new FieldViewModel
                {
                    Id = key,
                    Attrs = new Dictionary<string, string>
                    {
                        {
                            "className", string.Empty
                        },
                        {
                            "tableFieldId", field.Id.ToString()
                        },
                        {
                            "required", (!field.AllowNull).ToString()
                        }
                    },
                    FMap = "attrs.value",
                    Order = order++,
                    Config = new ConfigViewModel
                    {
                        Editable = true,
                        Label = field.DisplayName,
                        DisabledAttrs = new List<string> {"type"}
                    },
                    Meta = new MetaViewModel
                    {
                        Id = fieldConfig.Id,
                        Group = fieldConfig.Group,
                        Icon = fieldConfig.Icon
                    },
                    Tag = fieldConfig.Tag
                });

                if (field.DataType != TableFieldDataType.Guid)
                {
                    fields[key].Attrs.Add("type", fieldConfig.Type);
                }
                else
                {
                    fields[key].Options = new List<OptionsViewModel>();
                    fields[key].Attrs.Add("fieldReference", string.Empty);
                }
            }

            var buttonId = Guid.NewGuid();
            fields.Add(buttonId, new FieldViewModel
            {
                Attrs = new Dictionary<string, string>
                {
                    {
                        "translate", "save"
                    }
                },
                Id = buttonId,
                Order = order,
                Config = new ConfigViewModel
                {
                    HideLabel = true,
                    Label = "Button",
                    DisabledAttrs = new List<string> {"type"}
                },
                Meta = new MetaViewModel
                {
                    Id = "button",
                    Group = "common",
                    Icon = "button"
                },
                Options = new List<OptionsViewModel>
                {
                    new OptionsViewModel
                    {
                        ClassName = new List<AttrTagViewModel>
                        {
                            new AttrTagViewModel
                            {
                                Value = "error",
                                Label = "Danger",
                                Selected = false,
                            },
                            new AttrTagViewModel
                            {
                                Value = "success",
                                Label = "Success",
                                Selected = false,
                            },
                            new AttrTagViewModel
                            {
                                Value = "default",
                                Label = "Default",
                                Selected = false,
                            },
                            new AttrTagViewModel
                            {
                                Value = "primary",
                                Label = "Primary",
                                Selected = false,
                            }
                        },
                        Label = "Save",
                        Type = new List<AttrTagViewModel>
                        {
                            new AttrTagViewModel
                            {
                                Label = "submit",
                                Selected = true,
                                Value = "submit"
                            },
                            new AttrTagViewModel
                            {
                                Label = "Button",
                                Selected = false,
                                Value = "button"
                            },
                            new AttrTagViewModel
                            {
                                Label = "Reset",
                                Selected = false,
                                Value = "reset"
                            }
                        }
                    }
                },
                Tag = "button"
            });


            var form = new FormCreateDetailsViewModel
            {
                Name = name,
                Description = "Generated on scaffold",
                FormTypeId = formType.Id,
                RedirectUrl = redirectUrl,
                TableId = entityId,
                Model = new FormViewModel
                {
                    Stages = new Dictionary<Guid, StageViewModel>
                    {
                        {
                            stageId, new StageViewModel
                            {
                                Id = stageId,
                                Rows = new List<Guid> {rowId}
                            }
                        }
                    },
                    Rows = new Dictionary<Guid, RowViewModel>
                    {
                        {
                            rowId, new RowViewModel
                            {
                                Id = rowId,
                                Columns = new List<Guid> {colId},
                                Attrs = new Dictionary<string, string> {{"className", "f-row"}},
                                Config = new ConfigViewModel(),
                            }
                        }
                    },
                    Columns = new Dictionary<Guid, ColumnViewModel>
                    {
                        {
                            colId, new ColumnViewModel
                            {
                                Fields = fields.Select(x => x.Key),
                                Id = colId,
                                Config = new ConfigViewModel
                                {
                                    Width = "100%"
                                }
                            }
                        }
                    },
                    Fields = fields
                }
            };

            return form;
        }


        public virtual ResultModel<IDictionary<string, string>> GetValuesForEditForm(Form form,
            IDictionary<string, object> objDict)
        {
            var result = new ResultModel<IDictionary<string, string>>();

            if (form == null)
            {
                result.Errors = new List<IErrorModel>
                {
                    new ErrorModel(string.Empty, "Form not found")
                };
                return result;
            }

            var fields = new Dictionary<string, string>();

            var baseFields = typeof(BaseModel).GetProperties();
            var table = _entityContext.Table.Include(x => x.TableFields)
                .FirstOrDefault(x => x.Id == form.TableId);
            if (table == null)
            {
                result.Errors.Add(new ErrorModel("ArgumentNull", "Form doesn't have a reference to table"));
                return result;
            }

            foreach (var field in form.Fields)
            {
                var tableField = table.TableFields.FirstOrDefault(x => x.Id == field.TableFieldId);
                if (tableField == null) continue;
                if (!objDict.ContainsKey(tableField.Name)) continue;
                fields.Add(field.Id.ToString(), objDict[tableField.Name]?.ToString());
            }

            foreach (var baseField in baseFields)
            {
                if (objDict.ContainsKey(baseField.Name))
                {
                    fields.Add(baseField.Name, objDict[baseField.Name]?.ToString());
                }
            }

            result.IsSuccess = true;
            result.Result = fields;
            return result;
        }
    }
}