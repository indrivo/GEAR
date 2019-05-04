using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ST.Core;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.Entities.Extensions;

namespace ST.Configuration.Seed
{
    public static class NomenclatureManager
    {
        /// <summary>
        /// Seed nomenclatures
        /// </summary>
        /// <returns></returns>
        public static async Task SyncNomenclaturesAsync()
        {
            var service = IoC.Resolve<IDynamicService>();
            var nomenclatureTable = service.Table(Settings.Tables.Nomenclature);
            var req = await nomenclatureTable.Any();
            if (!req.Result) await nomenclatureTable.AddRange(GetFrom(nomenclatureTable.Type, "Installation/SystemEntitySeed/Vocabulary/BaseVocabulary/VocabularyList.json"));

            var cpiCategory = service.Table(Settings.Tables.NomKpiCategory);
            await cpiCategory.AddRange(GetFrom(cpiCategory.Type, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/KpiCategory.json"));

            var nomMeasurement = service.Table(Settings.Tables.NomMeasurement);
            await nomMeasurement.AddRange(GetFrom(nomMeasurement.Type, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/MeasurementCategory.json"));


            var periodCategory = service.Table(Settings.Tables.NomPeriod);
            await periodCategory.AddRange(GetFrom(periodCategory.Type, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/PeriodCategory.json"));


            var fullFilament = service.Table(Settings.Tables.NomFulfillment);
            await fullFilament.AddRange(GetFrom(fullFilament.Type, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/FulfillmentCategory.json"));

            var nomGoal = service.Table(Settings.Tables.NomGoal);
            await nomGoal.AddRange(GetFrom(nomGoal.Type, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/ObjectiveCategory.json"));

            var nomRequirement = service.Table(Settings.Tables.NomRequirement);
            await nomRequirement.AddRange(GetFrom(nomRequirement.Type, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/NomRequirement.json"));

            var nomInterestedPartTime = service.Table(Settings.Tables.NomInterestedPartyType);
            await nomInterestedPartTime.AddRange(GetFrom(nomInterestedPartTime.Type, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/InterestedPartyCategory.json"));

            var interestedPartyCat = service.Table(Settings.Tables.NomInterestedParty);
            await interestedPartyCat.AddRange(GetFrom(interestedPartyCat.Type, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/InterestedParty.json"));

            var nomCpi = service.Table(Settings.Tables.Kpi);
            await nomCpi.AddRange(GetFrom(nomCpi.Type, "Installation/SystemEntitySeed/DynamicEntities/KPI.json"));

            var partyRequirement = service.Table(Settings.Tables.PartyRequirement);
            await partyRequirement.AddRange(GetFrom(partyRequirement.Type, "Installation/SystemEntitySeed/DynamicEntities/PartyRequirement.json"));

            var goalPartyReq = service.Table(Settings.Tables.GoalPartyRequirement);
            await goalPartyReq.AddRange(GetFrom(goalPartyReq.Type, "Installation/SystemEntitySeed/DynamicEntities/GoalPartyRequirement.json"));

            var standards = service.Table(Settings.Tables.Standards);
            await standards.AddRange(GetFrom(standards.Type, "Installation/SystemEntitySeed/DynamicEntities/Standards.json"));

            var standardCategory = service.Table(Settings.Tables.StandardCategories);
            await standardCategory.AddRange(GetFrom(standardCategory.Type, "Installation/SystemEntitySeed/DynamicEntities/StandardCategories.json"));

            var categoryRequirements = service.Table(Settings.Tables.CategoryRequirements);
            await categoryRequirements.AddRange(GetFrom(categoryRequirements.Type, "Installation/SystemEntitySeed/DynamicEntities/CategoryRequirements.json"));
        }

        /// <summary>
        /// Get data from path
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static IEnumerable<dynamic> GetFrom(Type entity, string filePath)
        {
            return JsonParser.ReadDataListFromJsonWithTypeParameter(Path.Combine(AppContext.BaseDirectory, filePath), entity);
        }
    }
}
