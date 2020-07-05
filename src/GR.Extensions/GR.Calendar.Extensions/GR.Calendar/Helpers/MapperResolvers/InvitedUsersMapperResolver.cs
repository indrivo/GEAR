using System.Collections.Generic;
using AutoMapper;
using GR.Calendar.Abstractions.Models;
using GR.Calendar.Abstractions.Models.ViewModels;
using GR.Identity.Abstractions;

namespace GR.Calendar.Helpers.MapperResolvers
{
    public class InvitedUsersMapperResolver : IValueResolver<CalendarEvent, GetEventViewModel, ICollection<CalendarUserViewModel>>
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

        public InvitedUsersMapperResolver(IUserManager<GearUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public ICollection<CalendarUserViewModel> Resolve(CalendarEvent source, GetEventViewModel destination, ICollection<CalendarUserViewModel> destMember,
            ResolutionContext context)
        {
            var data = new List<CalendarUserViewModel>();
            foreach (var member in source.EventMembers)
            {
                var user = _userManager.FindUserByIdAsync(member.UserId).GetAwaiter().GetResult();
                if (!user.IsSuccess) continue;
                var mapped = _mapper.Map<CalendarUserViewModel>(user.Result);
                mapped.Acceptance = member.Acceptance;
                data.Add(mapped);
            }

            return data;
        }
    }
}
