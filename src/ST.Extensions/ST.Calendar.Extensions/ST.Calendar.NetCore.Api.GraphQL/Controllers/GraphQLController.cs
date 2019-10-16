using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using ST.Calendar.NetCore.Api.GraphQL.Models;
using System;
using System.Threading.Tasks;

namespace ST.Calendar.NetCore.Api.GraphQL.Controllers
{
    [Route("graphql")]
    public class GraphQLController : Controller
    {
        #region Injectable
        /// <summary>
        /// Document executor
        /// </summary>
        private readonly IDocumentExecuter _documentExecuter;

        /// <summary>
        /// Schema
        /// </summary>
        private readonly ISchema _schema;
        #endregion


        public GraphQLController(IDocumentExecuter documentExecuter, ISchema schema)
        {
            _documentExecuter = documentExecuter;
            _schema = schema;
        }

        /// <summary>
        /// Execute query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]GraphQLQuery query)
        {
            if (query == null) { throw new ArgumentNullException(nameof(query)); }

            var executionOptions = new ExecutionOptions { Schema = _schema, Query = query.Query };

            try
            {
                var result = await _documentExecuter.ExecuteAsync(executionOptions).ConfigureAwait(false);

                if (result.Errors?.Count > 0)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}