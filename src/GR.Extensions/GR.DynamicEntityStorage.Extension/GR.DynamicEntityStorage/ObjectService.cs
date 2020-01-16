using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using GR.DynamicEntityStorage.Abstractions.Helpers;
using GR.DynamicEntityStorage.Exceptions;
using GR.Entities.Controls.Builders;
using GR.Entities.Data;
using GR.Core;
using GR.Core.Helpers;
using GR.DynamicEntityStorage.Abstractions;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;
using GR.Entities.Abstractions.Models.Tables;
using Microsoft.Extensions.Caching.Memory;

namespace GR.DynamicEntityStorage
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
        /// <returns></returns>
        public async Task<DynamicObject> ResolveAsync(EntitiesDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
          
            Arg.NotNull(context, nameof(EntitiesDbContext));
            Arg.NotNull(httpContextAccessor, nameof(IHttpContextAccessor));
            var entity = _assemblyName.Name;
            var key = $"entity_{entity}";
            
            var table = await context.Table.FirstOrDefaultAsync(x => x.Name.Equals(entity));

            if (table == null) throw new DynamicTableOperationException($"Table {entity} not found in database!");
            var schema = table.EntityType;
            var stored = TypeManager.TryGet(entity, schema);

            if (stored.IsSuccess)
            {
                return new DynamicObject
                {
                    Type = stored.Result,
                    Service = IoC.Resolve<IDynamicService>()
                };
            }

            var model = new EntityViewModel
            {
                TableName = entity,
                TableSchema = schema,
                Fields = new List<EntityFieldsViewModel>()
            };

            var proprieties = BaseModel.GetPropsName().ToList();

            model = await ViewModelBuilderFactory.ResolveAsync(context, model);

            var dynamicClass = CreateClass();
            CreateConstructor(dynamicClass);

            //Create base props
            foreach (var field in model.Fields)
            {
                if (proprieties.Contains(field.ColumnName)) continue;

                var fieldType = DynamicTypeMapper.GetTypeFromString(field.Type);
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
                    var refType = await Task.Run(async () =>
                        await new ObjectService(entityRef.Value).ResolveAsync(context, httpContextAccessor));

                    CreateProperty(dynamicClass, $"{field.ColumnName}Reference", refType.Type);
                }
            }

            var type = dynamicClass.CreateTypeInfo();

            TypeManager.Register(schema, entity, type);

            return new DynamicObject
            {
                Type = type,
                Service = IoC.Resolve<IDynamicService>()
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

            var rq = TypeManager.TryGet(_assemblyName.Name, string.Empty);
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
                        rq.Result.GetProperty(prop.Name)?.SetValue(resultObject, value);
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
                , typeof(BaseModel));
            return typeBuilder;
        }

        /// <summary>
        /// Create constructor
        /// </summary>
        /// <param name="typeBuilder"></param>
        private static void CreateConstructor(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName |
                                                 MethodAttributes.RTSpecialName);
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

                var propertyBuilder =
                    typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
                var getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType,
                    Type.EmptyTypes);
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
    }
}
