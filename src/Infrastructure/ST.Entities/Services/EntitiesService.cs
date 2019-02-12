using ST.BaseBusinessRepository;
using ST.Entities.Data;

namespace ST.Entities.Services
{
    public abstract class EntitiesService 
    {
        private readonly IBaseBusinessRepository<EntitiesDbContext> _repository;

        public EntitiesService()
        {
        }

        public EntitiesService(IBaseBusinessRepository<EntitiesDbContext> repository)
        {
            _repository = repository;
        }

        //public ResultModel<bool> DeleteById(TableModel table, string connectionString, Guid parameterId)
        //{
        //    var returnModel = new ResultModel<bool>
        //    {
        //        IsSuccess = false,
        //        Result = false
        //    };
        //    if (table != null)
        //        try
        //        {
        //            var viewModel = ViewModelBuilder.Create(table,true);
        //            var sqlQuerry = EntityQuerryBuilder.DeleteByIdQuerry(viewModel.TableName, parameterId);
        //            if (!string.IsNullOrEmpty(sqlQuerry))
        //            {
        //                using (var connection = new SqlConnection(connectionString))
        //                {
        //                    var command = new SqlCommand(sqlQuerry, connection);
        //                    connection.Open();
        //                    command.ExecuteNonQuery();
        //                    connection.Close();
        //                }
        //                returnModel.Result = true;
        //                returnModel.IsSuccess = true;
        //                return returnModel;
        //            }
        //            // Empty querry
        //            return returnModel;
        //        }
        //        catch (Exception)
        //        {
        //            // Error
        //            return returnModel;
        //        }
        //    return returnModel;
        //}

        //public ResultModel<bool> DeleteByParams(TableModel table, string connectionString,
        //    string parameterName, string parameter)
        //{
        //    var returnModel = new ResultModel<bool>
        //    {
        //        IsSuccess = false,
        //        Result = false
        //    };
        //    if (table != null)
        //        try
        //        {
        //            var viewModel = ViewModelBuilder.Create(table,true);
        //            var sqlQuerry = EntityQuerryBuilder.DeleteByParamQuerry(viewModel.TableName, parameterName, parameter);
        //            if (!string.IsNullOrEmpty(sqlQuerry))
        //            {
        //                using (var connection = new SqlConnection(connectionString))
        //                {
        //                    var command = new SqlCommand(sqlQuerry, connection);
        //                    connection.Open();
        //                    command.ExecuteNonQuery();
        //                    connection.Close();
        //                }
        //                returnModel.Result = true;
        //                returnModel.IsSuccess = true;
        //                return returnModel;
        //            }
        //            // Empty querry
        //            return returnModel;
        //        }
        //        catch (Exception)
        //        {
        //            // Error
        //            return returnModel;
        //        }
        //    return returnModel;
        //}

    }
}