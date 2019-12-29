using GraphQL;
using Microsoft.AspNetCore.Mvc;
using GR.Calendar.NetCore.Api.GraphQL.Models;
using GR.Calendar.NetCore.Api.GraphQL.Schemas.Contracts;
using System;
using System.Threading.Tasks;

namespace GR.Calendar.NetCore.Api.GraphQL.Controllers
{
    [Route("api/[controller]")]
    public class CalendarGraphQLController : Controller
    {
        #region Injectable
        /// <summary>
        /// Document executor
        /// </summary>
        private readonly IDocumentExecuter _documentExecuter;

        /// <summary>
        /// Schema
        /// </summary>
        private readonly ICalendarSchema _schema;
        #endregion


        public CalendarGraphQLController(IDocumentExecuter documentExecuter, ICalendarSchema schema)
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