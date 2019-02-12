using System;
using System.Collections.Generic;
using System.Linq;
using ST.BaseBusinessRepository;
using ST.Entities.Data;
using ST.Entities.Models.Forms;
using ST.Entities.Services.Abstraction;
using ST.Entities.ViewModels.Form;

namespace ST.Entities.Services
{
    public class FormService : IFormService
    {
        private readonly IBaseBusinessRepository<EntitiesDbContext> _repository;
        private readonly EntitiesDbContext _context;

        public FormService(IBaseBusinessRepository<EntitiesDbContext> repository, EntitiesDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultModel DeleteForm(Guid id)
        {
            var form = _repository.GetSingle<Form>(id);
            var result = _context.Forms.Remove(form);
            _context.SaveChanges();
            return new ResultModel
            {
                IsSuccess = result.IsKeySet
            };
        }


        /// <inheritdoc />
        /// <summary>
        /// Get type of form
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        public FormType GetTypeByFormId(Guid formId)
        {
            var form = _repository.GetById<Form, Form>(formId);
            if (!form.IsSuccess) return null;
            if (form.Result.TypeId == Guid.Empty) return null;
            var formType = _repository.GetById<FormType, FormType>(form.Result.TypeId);
            return formType.IsSuccess ? formType.Result : null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get form by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultModel<FormViewModel> GetFormById(Guid id)
        {
            var response = _repository.GetById<Form, Form>(id);

            if (response.IsSuccess)
            {
                var form = new FormViewModel
                {
                    Id = response.Result.Id
                };
                var fields = new Dictionary<Guid, FieldViewModel>();
                var columns = new Dictionary<Guid, ColumnViewModel>();
                var stages = new Dictionary<Guid, StageViewModel>();
                var rows = new Dictionary<Guid, RowViewModel>();

                var _stages = _context.Stages.Where(x => x.FormId == form.Id).ToList();
                foreach (var stage in _stages)
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

                var _rows = _context.Rows.Where(x => x.FormId == form.Id).ToList();
                foreach (var row in _rows)
                {
                    var c = new List<Guid>(_context
                        .RowColumns
                        .Where(x => x.RowId == row.Id).Select(x => x.ColumnId));

                    var attr = _context.Attrs.FirstOrDefault(x => x.Id == row.AttrsId);
                    var config = _context.Configs.FirstOrDefault(x => x.Id == row.ConfigId);
                    rows.Add(row.Id, new RowViewModel
                    {
                        Id = row.Id,
                        Config = new ConfigViewModel
                        {
                            Fieldset = config.Fieldset,
                            Label = config.Label,
                            InputGroup = config.InputGroup,
                            Legend = config.Legend,
                            Width = config.Width
                        },
                        Columns = c,
                        Attrs = new AttrsViewModel
                        {
                            Type = attr?.Type,
                            ClassName = attr?.ClassName,
                            Required = attr.Required,
                            Value = attr?.Value
                        }
                    });
                }

                var _columns = _context.Columns.Where(x => x.FormId == form.Id).ToList();
                foreach (var column in _columns)
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
                            Fieldset = config.Fieldset,
                            Label = config.Label,
                            InputGroup = config.InputGroup,
                            Legend = config.Legend,
                            Width = config.Width
                        },
                        Id = column.Id,
                        Fields = f
                    });
                }

                var _fields = _context.Fields.Where(x => x.FormId == form.Id).ToList();

                foreach (var field in _fields)
                {
                    var config = _context.Configs.FirstOrDefault(x => x.Id == field.ConfigId);
                    var attr = _context.Attrs.FirstOrDefault(x => x.Id == field.AttrsId);
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

                    fields.Add(field.Id, new FieldViewModel
                    {
                        Id = field.Id,
                        TableFeldId = field.TableFieldId,
                        Config = new ConfigViewModel
                        {
                            Fieldset = config.Fieldset,
                            Label = config.Label,
                            InputGroup = config.InputGroup,
                            Legend = config.Legend,
                            Width = config.Width,
                            DisabledAttrs = configDisabled,
                            HideLabel = field.Config.HideLabel,
                            Editable = field.Config.Editable
                        },
                        Attrs = new AttrsViewModel
                        {
                            ClassName = attr.ClassName ?? "",
                            Required = attr.Required,
                            Type = attr.Type,
                            Value = field.Attrs.Value,
                            TableFieldId = field.TableFieldId.ToString(),
                            Tag = (field.Tag == "h1")
                                ? new List<AttrTagViewModel>
                                {
                                    new AttrTagViewModel
                                    {
                                        Label = "H1",
                                        Value = "h1"
                                    },
                                    new AttrTagViewModel
                                    {
                                        Label = "H2",
                                        Value = "h2"
                                    },
                                    new AttrTagViewModel
                                    {
                                        Label = "H3",
                                        Value = "h3"
                                    },
                                    new AttrTagViewModel
                                    {
                                        Label = "H4",
                                        Value = "h4"
                                    }
                                }
                                : null
                        },
                        Content = field.Content,
                        FMap = field.FMap,
                        Tag = field.Tag,
                        Meta = new MetaViewModel
                        {
                            Group = meta.Group,
                            Icon = meta.Icon,
                            Id = meta.Icon
                        },
                        Options = (field.Attrs.Type == "radio" ||
                                   field.Tag == "select" ||
                                   field.Tag == "button")
                            ? ops
                            : null,
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
                    IsSuccess = response.IsSuccess,
                    Result = form
                };
                return result;
            }
            else
            {
                var result = new ResultModel<FormViewModel>
                {
                    IsSuccess = response.IsSuccess
                };
                return result;
            }
        }

        /// <inheritdoc cref="description" />
        /// <summary>
        /// Create form
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tableId"></param>
        /// <param name="formTypeId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResultModel<Guid> CreateForm(FormCreateDetailsViewModel model, string userId)
        {
            var contor = 0;
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
                var stages = new List<Stage>();
                var rows = new List<Row>();
                var columns = new List<Column>();
                var fields = new List<Field>();
                var stageRows = new List<StageRows>();
                var rowColumns = new List<RowColumn>();
                var columnFields = new List<ColumnField>();

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
                                Attrs = new Attrs
                                {
                                    ClassName = row.Value.Attrs.ClassName,
                                    Required = row.Value.Attrs.Required,
                                    Type = row.Value.Attrs.Type
                                },
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
                                                disabled.AddRange(field.Value.Config.DisabledAttrs.Select(dis => new DisabledAttr { Name = dis }));
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

                                            fields.Add(new Field
                                            {
                                                Order = contor++,
                                                TableFieldId = field.Value.TableFeldId,
                                                Attrs = (field.Value.Attrs != null)
                                                    ? new Attrs
                                                    {
                                                        ClassName = field.Value.Attrs.ClassName,
                                                        Required = field.Value.Attrs.Required,
                                                        Type = field.Value.Attrs.Type,
                                                        Value = field.Value.Attrs.Value
                                                    }
                                                    : new Attrs
                                                    {
                                                        ClassName = string.Empty
                                                    },
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
                                                Order = fields[fields.Count - 1].Order
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
                    form.ModifiedBy = userId;
                }
                else
                {
                    form.Author = userId;
                    form.Created = DateTime.Now;
                    form.Changed = DateTime.Now;
                }

                form.Fields = fields;
                form.Rows = rows;
                form.Settings = settings;
                form.Columns = columns;
                form.Stages = stages;
                //Set formTypeId
                form.TypeId = model.FormTypeId;
                _context.Forms.Add(form);
                _context.StageRows.AddRange(stageRows);
                _context.RowColumns.AddRange(rowColumns);
                _context.ColumnFields.AddRange(columnFields);
                _context.SaveChanges();
                var result = new ResultModel<Guid>
                {
                    IsSuccess = true
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
                            Message = ex.ToString()
                        }
                    }
                };
                return result;
            }
        }
    }
}