using System;
using System.Collections.Generic;
using System.Text;
using GR.Cache.Abstractions;
using GR.Core.Helpers;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;

namespace GR.Localization.DataBaseProvider
{
    public class DbLocalizationService : ILocalizationService
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly ILocalizationContext _context;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        #endregion


        public DbLocalizationService(ILocalizationContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public virtual void EditKey(EditLocalizationViewModel model)
        {
            throw new NotImplementedException();
        }

        public virtual void AddOrUpdateKey(string key, IDictionary<string, string> localizedStrings)
        {
            throw new NotImplementedException();
        }

        public virtual ResultModel AddLanguage(AddLanguageViewModel model)
        {
            throw new NotImplementedException();
        }

        public virtual ResultModel ChangeStatusOfLanguage(LanguageCreateViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
