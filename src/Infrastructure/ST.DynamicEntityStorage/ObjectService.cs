using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ST.DynamicEntityStorage.Abstractions.Helpers;
using ST.DynamicEntityStorage.Exceptions;
using ST.DynamicEntityStorage.Services;
using ST.Entities.Controls.Builders;
using ST.Entities.Data;
using ST.Entities.ViewModels.DynamicEntities;
using ST.Shared;

namespace ST.DynamicEntityStorage
{
    public class ObjectService
    {
        /// <summary>
        /// Assembly name
        /// </summary>
        private readonly AssemblyName _assemblyName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="className"></param>
        public ObjectService(string className)
        {
            _assemblyName = new AssemblyName(className);
        }

        /// <summary>
        /// Create object from table configuration
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="includeFieldReferences"></param>
        /// <returns></returns>
        public async Task<DynamicObject> ResolveAsync(EntitiesDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            var entity = _assemblyName.Name;
            if (entity == "Control")
            {

            }
            var table = await context.Table.FirstOrDefaultAsync(x => x.Name.Equals(entity));
            if (table == null) throw new DynamicTableOperationException($"Table {entity} not found in database!");
            var schema = table.EntityType;
            var stored = TypeManager.TryGet(entity, schema);

            if (stored.IsSuccess)
            {
                return new DynamicObject
                {
                    Type = stored.Result,
                    Service = new DynamicService<EntitiesDbContext>(context, httpContextAccessor)
                };
            }

            var model = new EntityViewModel
            {
                TableName = entity,
                TableSchema = schema,
                Fields = new List<EntityFieldsViewModel>()
            };

            var proprieties = typeof(ExtendedModel).GetProperties().Select(x => x.Name).ToList();

            model = await ViewModelBuilder.ResolveAsync(context, model);

            var dynamicClass = CreateClass();
            CreateConstructor(dynamicClass);

            //Create base props
            foreach (var field in model.Fields)
            {
                if (proprieties.Contains(field.ColumnName)) continue;

                var fieldType = GetTypeFromString(field.Type);
                CreateProperty(dynamicClass, field.ColumnName, fieldType);
            }

            //Create references
            foreach (var field in model.Fields)
            {
                if (field.Configurations == null) continue;
                if (!field.Configurations.Any()) continue;
                var entityRef = field.Configurations.FirstOrDefault(x => x.Name == "ForeingTable");
                var entityRefSchema = field.Configurations.FirstOrDefault(x => x.Name == "ForeingSchemaTable");
                if (entityRef == null || entityRefSchema == null) continue;

                if (entityRef.Value == _assemblyName.Name)
                {
                    CreateProperty(dynamicClass, $"{field.ColumnName}Reference", dynamicClass);
                }
                else
                {
                    var refType = await Task.Run(async () => await new ObjectService(entityRef.Value).ResolveAsync(context, httpContextAccessor));

                    CreateProperty(dynamicClass, $"{field.ColumnName}Reference", refType.Type);
                }
            }

            var type = dynamicClass.CreateType();

            TypeManager.Register(schema, entity, type);

            return new DynamicObject
            {
                Type = type,
                Service = new DynamicService<EntitiesDbContext>(context, httpContextAccessor)
            };
        }

        /// <summary>
        /// Object to custom object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object ParseObject<TObject>(TObject obj)
        {
            var props = obj.GetType().GetProperties();

            var rq = TypeManager.TryGet(_assemblyName.Name, "");
            if (!rq.IsSuccess) return obj;
            var resultObject = Activator.CreateInstance(rq.Result);
            foreach (var prop in resultObject.GetType().GetProperties())
            {
                foreach (var data in props)
                {
                    if (!prop.Name.Equals(data.Name)) continue;
                    try
                    {
                        var value = data.GetValue(obj, null);
                        rq.Result.GetProperty(prop.Name).SetValue(resultObject, value);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return resultObject;
        }
        /// <summary>
        /// Implement from object to dictionary
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetDictionary<TEntity>(TEntity model)
        {
            var dictionary = new Dictionary<string, object>();
            try
            {
                dictionary = model.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(model, null));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return dictionary;
        }
        /// <summary>
        /// Create a class
        /// </summary>
        /// <returns></returns>
        private TypeBuilder CreateClass()
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            var typeBuilder = moduleBuilder.DefineType(_assemblyName.FullName
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , typeof(ExtendedModel));
            return typeBuilder;
        }
        /// <summary>
        /// Create constructor
        /// </summary>
        /// <param name="typeBuilder"></param>
        private static void CreateConstructor(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }
        /// <summary>
        /// Create new property
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            try
            {
                var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

                var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
                var getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
                var getIl = getPropMthdBldr.GetILGenerator();

                getIl.Emit(OpCodes.Ldarg_0);
                getIl.Emit(OpCodes.Ldfld, fieldBuilder);
                getIl.Emit(OpCodes.Ret);

                var setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    null, new[] { propertyType });

                var setIl = setPropMthdBldr.GetILGenerator();
                var modifyProperty = setIl.DefineLabel();
                var exitSet = setIl.DefineLabel();

                setIl.MarkLabel(modifyProperty);
                setIl.Emit(OpCodes.Ldarg_0);
                setIl.Emit(OpCodes.Ldarg_1);
                setIl.Emit(OpCodes.Stfld, fieldBuilder);

                setIl.Emit(OpCodes.Nop);
                setIl.MarkLabel(exitSet);
                setIl.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getPropMthdBldr);
                propertyBuilder.SetSetMethod(setPropMthdBldr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// Get type from string definition
        /// </summary>
        /// <param name="stringType"></param>
        /// <returns></returns>
        private static Type GetTypeFromString(string stringType)
        {
            var type = Type.GetType(string.Empty);
            switch (stringType)
            {
                case "nvarchar":
                    {
                        type = typeof(string);
                    }
                    break;
                case "int":
                case "int32":
                    {
                        type = typeof(int);
                    }
                    break;
                case "char":
                    {
                        type = typeof(char);
                    }
                    break;
                case "bool":
                    {
                        type = typeof(bool);
                    }
                    break;
                case "uniqueidentifier":
                    {
                        type = typeof(Guid);
                    }
                    break;
                case "bigint":
                    {
                        type = typeof(long);
                    }
                    break;
                case "date":
                case "datetime":
                    {
                        type = typeof(DateTime);
                    }
                    break;
                case "decimal":
                    {
                        type = typeof(double);
                    }
                    break;
            }
            return type;
        }
    }
}
