using AutoMapper;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.ViewModels.UserViewModels;
using GR.Orders.Abstractions.Models;
using GR.Orders.Abstractions.ViewModels.OrderViewModels;

namespace GR.Orders.Abstractions.Helpers.MapperResolvers
{
    public class UserOrderMapperResolver : IValueResolver<Order, GetOrdersViewModel, UserInfoViewModel>
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

        public UserOrderMapperResolver(IUserManager<GearUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public UserInfoViewModel Resolve(Order source, GetOrdersViewModel destination, UserInfoViewModel destMember,
            ResolutionContext context)
        {
            var userReq = _userManager.FindUserByIdAsync(source.UserId).GetAwaiter().GetResult();
            return !userReq.IsSuccess ? new UserInfoViewModel() : _mapper.Map<UserInfoViewModel>(userReq.Result);
        }
    }
}
