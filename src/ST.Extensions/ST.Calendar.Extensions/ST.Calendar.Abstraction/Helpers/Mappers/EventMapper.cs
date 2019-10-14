using System;
using System.Collections.Generic;
using System.Linq;
using Mapster;
using ST.Calendar.Abstractions.Enums;
using ST.Calendar.Abstractions.Models;
using ST.Calendar.Abstractions.Models.ViewModels;
using ST.Core.Helpers;
using ST.Identity.Abstractions;

namespace ST.Calendar.Abstractions.Helpers.Mappers
{
    public static class EventMapper
    {
        /// <summary>
        /// User manager
        /// </summary>
        private static Lazy<IUserManager<ApplicationUser>> UserManager => new Lazy<IUserManager<ApplicationUser>>(IoC.Resolve<IUserManager<ApplicationUser>>);

        /// <summary>
        /// Map events
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<GetEventViewModel> Map(IList<CalendarEvent> source)
        {
            Arg.NotNull(source, nameof(Map));
            var mapped = source?.Select(Map);
            return mapped;
        }

        /// <summary>
        /// Map
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ResultModel<GetEventViewModel> Map(ResultModel<CalendarEvent> source)
        {
            if (source == null) return null;

            return new ResultModel<GetEventViewModel>
            {
                IsSuccess = source.IsSuccess,
                Errors = source.Errors,
                Result = Map(source.Result)
            };
        }

        /// <summary>
        /// Map
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static GetEventViewModel Map(CalendarEvent source)
        {
            if (source == null) return null;
            var obj = source.Adapt<GetEventViewModel>();
            if (source.Organizer != Guid.Empty)
            {
                obj.OrganizerInfo = new CalendarUserViewModel(GetUserById(source.Organizer))
                {
                    Acceptance = EventAcceptance.Accept
                };
            }

            if (source.EventMembers.Any())
            {
                obj.InvitedUsers = source.EventMembers.Select(x => new CalendarUserViewModel(GetUserById(x.UserId))
                {
                    Acceptance = x.Acceptance
                }).ToList();
            }

            return obj;
        }


        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static ApplicationUser GetUserById(Guid? id)
        {
            var user = UserManager.Value.UserManager.FindByIdAsync(id.ToString().ToLowerInvariant()).GetAwaiter().GetResult();
            return user;
        }


        /// <summary>
        /// Map
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ResultModel<IEnumerable<GetEventViewModel>> Map(ResultModel<IEnumerable<CalendarEvent>> source)
        {
            var response = new ResultModel<IEnumerable<GetEventViewModel>>();
            if (!source.IsSuccess)
            {
                response.Errors = source.Errors;

                return response;
            }

            response.Result = Map(source.Result?.ToList());
            response.IsSuccess = true;
            return response;
        }

        /// <summary>
        /// Map with helpers
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ResultModel<GetEventWithHelpersViewModel> MapWithHelpers(ResultModel<IEnumerable<CalendarEvent>> source)
        {
            var response = new ResultModel<GetEventWithHelpersViewModel>();
            if (!source.IsSuccess)
            {
                response.Errors = source.Errors;

                return response;
            }

            response.Result = new GetEventWithHelpersViewModel
            {
                Events = Map(source.Result?.ToList())
            };
            response.IsSuccess = true;
            return response;
        }

        /// <summary>
        /// Map
        /// </summary>
        /// <param name="initialSource"></param>
        /// <param name="targetSource"></param>
        /// <returns></returns>
        public static CalendarEvent Map(CalendarEvent initialSource, UpdateEventViewModel targetSource)
        {
            if (targetSource == null) return initialSource;
            initialSource.Title = targetSource.Title;
            initialSource.Details = targetSource.Details;
            initialSource.StartDate = targetSource.StartDate;
            initialSource.EndDate = targetSource.EndDate;
            initialSource.Location = targetSource.Location;
            initialSource.Priority = targetSource.Priority;
            initialSource.MinutesToRemind = targetSource.MinutesToRemind;
            return initialSource;
        }
    }
}
