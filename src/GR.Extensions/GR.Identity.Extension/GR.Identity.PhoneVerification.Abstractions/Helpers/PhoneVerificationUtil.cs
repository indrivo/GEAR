using System;
using GR.Core.Helpers;
using PhoneNumbers;

namespace GR.Identity.PhoneVerification.Abstractions.Helpers
{
    public static class PhoneVerificationUtil
    {
        private static PhoneNumberUtil _phoneUtil;

        public static PhoneNumberUtil PhoneHelper => _phoneUtil ?? (_phoneUtil = PhoneNumberUtil.GetInstance());

        public static ResultModel<PhoneNumber> Parse(string phoneRaw)
        {
            var response = new ResultModel<PhoneNumber>();
            try
            {
                // Parse the number to check into a PhoneNumber object.
                var phoneNumber = PhoneHelper.Parse(phoneRaw, null);
                response.Result = phoneNumber;
                response.IsSuccess = true;
            }
            catch (NumberParseException e)
            {
                response.AddError(e.ErrorType.ToString(), e.Message);
            }
            catch (Exception e)
            {
                response.AddError(e.Message);
            }

            return response;
        }

        public static ResultModel<PhoneNumber> Parse(string phoneRaw, string region)
        {
            var response = new ResultModel<PhoneNumber>();
            try
            {
                // Parse the number to check into a PhoneNumber object.
                var phoneNumber = PhoneHelper.Parse(phoneRaw, region);
                response.Result = phoneNumber;
                response.IsSuccess = true;
            }
            catch (NumberParseException e)
            {
                response.AddError(e.ErrorType.ToString(), e.Message);
            }
            catch (Exception e)
            {
                response.AddError(e.Message);
            }

            return response;
        }
    }
}
