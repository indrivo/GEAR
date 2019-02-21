using System.Collections.Generic;
using ST.Entities.ViewModels.BaseModel;

namespace ST.Entities.Controls.Builders
{
    public static class BaseModelBuilder
    {
        /// <summary>
        ///     Create base model
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static List<BaseModelVieModel> CreateBaseModel(string entityType)
        {
            var baseModel = new List<BaseModelVieModel>
            {
                new BaseModelVieModel
                {
                    Name = "Id",
                    DataType = "uniqueidentifier",
                    AllowNull = false
                },
                new BaseModelVieModel
                {
                    Name = "Author",
                    DataType = "nvarchar",
                    AllowNull = true,
                    MaxLenght = 50
                },
                new BaseModelVieModel
                {
                    Name = "Created",
                    DataType = "datetime",
                    AllowNull = true
                },
                new BaseModelVieModel
                {
                    Name = "ModifiedBy",
                    DataType = "nvarchar",
                    AllowNull = true,
                    MaxLenght = 50
                },
                new BaseModelVieModel
                {
                    Name = "Changed",
                    DataType = "datetime",
                    AllowNull = true
                },
                new BaseModelVieModel
                {
                    Name = "IsDeleted",
                    DataType = "bit",
                    AllowNull = true
                },
                new BaseModelVieModel
                {
                    Name = "TenantId",
                    DataType = "uniqueidentifier",
                    AllowNull = true
                }
            };
            if (entityType == "Profile")
                baseModel.Add(new BaseModelVieModel
                {
                    Name = "UserId",
                    DataType = "uniqueidentifier",
                    AllowNull = false
                });

            return baseModel;
        }
    }
}