using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using ST.Calendar.Abstractions.Models;
using ST.Identity.Abstractions;

namespace ST.Calendar.NetCore.Api.GraphQL.Models.GraphQLTypes
{
    public class EventMemberType : ObjectGraphType<EventMember>
    {
        public EventMemberType(IUserManager<ApplicationUser> userManager)
        {
            Field(x => x.UserId, type: typeof(IdGraphType));
            Field<UserType>("user", resolve: context => userManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(context.Source.UserId)));
        }
    }
}
