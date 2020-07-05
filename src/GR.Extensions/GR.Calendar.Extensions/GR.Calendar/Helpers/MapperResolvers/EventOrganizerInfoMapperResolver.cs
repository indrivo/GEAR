using AutoMapper;
using GR.Calendar.Abstractions.Enums;
using GR.Calendar.Abstractions.Models;
using GR.Calendar.Abstractions.Models.ViewModels;
using GR.Identity.Abstractions;

namespace GR.Calendar.Helpers.MapperResolvers
{
    public class EventOrganizerInfoMapperResolver : IValueResolver<CalendarEvent, GetEventViewModel, CalendarUserViewModel>
    {
        #region Injectable

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;
        #endregion

        public EventOrganizerInfoMapperResolver(IUserManager<GearUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }


        public CalendarUserViewModel Resolve(CalendarEvent source, GetEventViewModel destination, CalendarUserViewModel destMember,
            ResolutionContext context)
        {
            var user = _userManager.FindUserByIdAsync(source.Organizer).GetAwaiter().GetResult();
            if (!user.IsSuccess) return null;
            var mapped = _mapper.Map<CalendarUserViewModel>(user.Result);
            mapped.Acceptance = EventAcceptance.Accept;
            return mapped;
        }
    }
}
