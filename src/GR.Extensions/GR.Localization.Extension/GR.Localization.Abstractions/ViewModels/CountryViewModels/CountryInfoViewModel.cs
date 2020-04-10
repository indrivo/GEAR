using System.Collections.Generic;

namespace GR.Localization.Abstractions.ViewModels.CountryViewModels
{
    public class CountryInfoViewModel
    {
        public string Alpha2Code { get; set; }
        public string Alpha3Code { get; set; }
        public IEnumerable<string> AltSpellings { get; set; }
        public long Area { get; set; }
        public IEnumerable<string> Borders { get; set; }
        public IEnumerable<string> CallingCodes { get; set; }
        public string Capital { get; set; }
        public IEnumerable<CountryCurrencyViewModel> Currencies { get; set; }
        public string Demonym { get; set; }
        public string Flag { get; set; }
        public double? Gini { get; set; }
        public IEnumerable<CountryLanguagesViewModel> Languages { get; set; }
        public IEnumerable<double> Latlng { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string NumericCode { get; set; }
        public long Population { get; set; }
        public string Region { get; set; }
        public IEnumerable<RegionalBlockViewModel> RegionalBlocs { get; set; }
        public string Subregion { get; set; }
        public IEnumerable<string> Timezones { get; set; }
        public IEnumerable<string> TopLevelDomain { get; set; }
        public Dictionary<string, string> Translations { get; set; }
        public string Cioc { get; set; }
    }

    public class RegionalBlockViewModel
    {
        public string Acronym { get; set; }
        public string Name { get; set; }
    }

    public class CountryCurrencyViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
    }

    public class CountryLanguagesViewModel
    {
        public string Iso639_1 { get; set; }
        public string Iso639_2 { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
    }
}
