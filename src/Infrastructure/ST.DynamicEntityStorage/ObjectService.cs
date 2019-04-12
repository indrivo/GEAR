using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Http;
using ST.Audit.Models;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Exceptions;
using ST.DynamicEntityStorage.Services;
using ST.Entities.Controls.Builders;
using ST.Entities.Data;
using ST.Entities.ViewModels.DynamicEntities;

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
        public DynamicObject Resolve(EntitiesDbContext context, IHttpContextAccessor httpContextAccessor, bool includeFieldReferences = true)
        {

            var entity = _assemblyName.Name;
            var table = context.Table.FirstOrDefault(x => x.Name.Equals(entity));
            if (table == null) throw new DynamicTableOperationException($"Table {entity} not found in database!");
            var schema = table.EntityType;
            var stored = TypeManager.TryGet(entity, schema);

            if (stored.IsSuccess)
            {
                return new DynamicObject
                {
                    Object = stored.Result,
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

            model = ViewModelBuilder.Resolve(context, model);

            var dynamicClass = CreateClass();
            CreateConstructor(dynamicClass);

            foreach (var field in model.Fields)
            {
                if (proprieties.Contains(field.ColumnName)) continue;

                var entityRef = field.Configurations.FirstOrDefault(x => x.Name == "ForeingTable");
                var entityRefSchema = field.Configurations.FirstOrDefault(x => x.Name == "ForeingSchemaTable");
                if (entityRef != null && entityRefSchema != null)
                {
                    if (includeFieldReferences)
                    {
                        var refType = new ObjectService(entityRef.Value).Resolve(context, httpContextAccessor, false);
                        CreateProperty(dynamicClass, $"{field.ColumnName}Reference", refType.Object.GetType());
                    }
                }
                var fieldType = GetTypeFromString(field.Type);
                CreateProperty(dynamicClass, field.ColumnName, fieldType);
            }

            var type = dynamicClass.CreateType();

            TypeManager.Register(schema, entity, type);

            var obj = Activator.CreateInstance(type);

            return new DynamicObject
            {
                Object = obj,
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
            var proprieties = typeof(ExtendedModel).GetProperties().Select(x => x.Name).ToList();
            var dynamicClass = CreateClass();
            CreateConstructor(dynamicClass);
            var props = obj.GetType().GetProperties();

            foreach (var prop in props)
            {
                if (!proprieties.Contains(prop.Name))
                {
                    CreateProperty(dynamicClass, prop.Name, prop.PropertyType);
                }
            }

            var type = dynamicClass.CreateType();
            var resultObject = Activator.CreateInstance(type);
            foreach (var prop in resultObject.GetType().GetProperties())
            {
                foreach (var data in props)
                {
                    if (!prop.Name.Equals(data.Name)) continue;
                    try
                    {
                        var value = data.GetValue(obj, null);
                        type.GetProperty(prop.Name).SetValue(resultObject, value);
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

    public class DynamicObject
    {
        public IDynamicService Service { get; set; }
        public object Object { get; set; }
    }
}
