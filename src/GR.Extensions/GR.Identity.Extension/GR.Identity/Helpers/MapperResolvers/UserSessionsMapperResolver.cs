using AutoMapper;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.ViewModels.UserViewModels;
using GR.Notifications.Abstractions;

namespace GR.Identity.Helpers.MapperResolvers
{
    public class UserSessionsMapperResolver : IValueResolver<GearUser, UserListItemViewModel, int>
    {
        #region Injectable

        private readonly ICommunicationHub _communicationHub;

        #endregion

        public UserSessionsMapperResolver(ICommunicationHub communicationHub)
        {
            _communicationHub = communicationHub;
        }

        public int Resolve(GearUser source, UserListItemViewModel destination, int destMember, ResolutionContext context)
        {
            var sessions = _communicationHub.GetSessionsCountByUserId(source.Id);
            return sessions;
        }
    }
}
