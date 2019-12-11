using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using GR.Calendar.Abstractions.Models;
using GR.Identity.Abstractions;

namespace GR.Calendar.NetCore.Api.GraphQL.Models.GraphQLTypes
{
    public class EventMemberType : ObjectGraphType<EventMember>
    {
        public EventMemberType(IUserManager<GearUser> userManager)
        {
            Field(x => x.UserId, type: typeof(IdGraphType));
            Field<UserType>("user", resolve: context => userManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(context.Source.UserId)));
        }
    }
}
