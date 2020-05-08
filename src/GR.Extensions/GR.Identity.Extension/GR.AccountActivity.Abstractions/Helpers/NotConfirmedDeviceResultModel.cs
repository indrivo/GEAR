using System.Collections.Generic;
using GR.Core.Helpers;

namespace GR.AccountActivity.Abstractions.Helpers
{
    public class NotConfirmedDeviceResultModel<T> : ResultModel<T>
    {
        public override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel("G403", "The device is not confirmed, an email has been sent for confirmation")
        };
    }

    public class NotConfirmedDeviceResultModel : ResultModel
    {
        public override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel("G403", "The device is not confirmed, an email has been sent for confirmation")
        };
    }
}
