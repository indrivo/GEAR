namespace ST.Calendar.NetCore.Api.GraphQL.Models
{
    public class GraphQLQuery
    {
        public virtual string OperationName { get; set; }
        public virtual string NamedQuery { get; set; }
        public virtual string Query { get; set; }
        public virtual string Variables { get; set; }
    }
}
