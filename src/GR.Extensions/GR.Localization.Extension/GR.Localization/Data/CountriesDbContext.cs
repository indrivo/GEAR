using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GR.Audit.Contexts;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Models.Countries;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GR.Localization.Data
{
    public class CountriesDbContext : TrackerDbContext, ICountryContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Localization";

        public CountriesDbContext(DbContextOptions<CountriesDbContext> options) : base(options)
        {
        }

        #region Entities

        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<StateOrProvince> StateOrProvinces { get; set; }
        public virtual DbSet<District> Districts { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);

            builder.Entity<Country>().HasKey(k => k.Id);
            builder.Entity<StateOrProvince>().Property(x => x.Id);


            //seed countries
            var countries = GetCountriesFromJsonFile();
            foreach (var country in countries)
            {
                var cities = country.StatesOrProvinces;
                country.StatesOrProvinces = null;
                builder.Entity<Country>().HasData(country);
                builder.Entity<StateOrProvince>().HasData(cities);
            }
        }

        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Get countries
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Country> GetCountriesFromJsonFile()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Configuration/countries.json");
            using (var reader = new FileStream(path, FileMode.Open))
            {
                if (!reader.CanRead) throw new FileNotFoundException("countries.json not found for seed countries");
                using (var fileReader = new StreamReader(reader))
                {
                    var fileContent = fileReader.ReadToEnd();
                    if (string.IsNullOrEmpty(fileContent))
                    {
                        return new List<Country>();
                    }

                    var parse = JsonConvert.DeserializeObject<IEnumerable<Country>>(fileContent);
                    return parse;
                }
            }
        }
    }
}
