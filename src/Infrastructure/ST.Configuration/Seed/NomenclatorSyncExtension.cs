using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Extensions;
using ST.Entities.Extensions;

namespace ST.Configuration.Seed
{
	public class NomenclatureSyncExtension
	{
		public static async Task SyncNomenclatureItems(IDynamicService service)
		{
			var nomenclatureTable = service.Table(Settings.Tables.Nomenclature);
			var req = await nomenclatureTable.Any();
			if (!req.Result) await nomenclatureTable.AddRange(GetFrom(nomenclatureTable.Object, "Installation/SystemEntitySeed/Vocabulary/BaseVocabulary/VocabularyList.json"));

			var cpiCategory = service.Table(Settings.Tables.NomKpiCategory);
			await cpiCategory.AddRange(GetFrom(cpiCategory.Object, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/KpiCategory.json"));

			var nomMeasurement = service.Table(Settings.Tables.NomMeasurement);
			await nomMeasurement.AddRange(GetFrom(nomMeasurement.Object, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/MeasurementCategory.json"));


			var periodCategory = service.Table(Settings.Tables.NomPeriod);
			await periodCategory.AddRange(GetFrom(periodCategory.Object, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/PeriodCategory.json"));


			var fullFilament = service.Table(Settings.Tables.NomFulfillment);
			await fullFilament.AddRange(GetFrom(fullFilament.Object, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/FulfillmentCategory.json"));

			var nomGoal = service.Table(Settings.Tables.NomGoal);
			await nomGoal.AddRange(GetFrom(nomGoal.Object, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/ObjectiveCategory.json"));

			var nomRequirement = service.Table(Settings.Tables.NomRequirement);
			await nomRequirement.AddRange(GetFrom(nomRequirement.Object, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/NomRequirement.json"));

			var nomInterestedPartTime = service.Table(Settings.Tables.NomInterestedPartyType);
			await nomInterestedPartTime.AddRange(GetFrom(nomInterestedPartTime.Object, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/InterestedPartyCategory.json"));

			var interestedPartyCat = service.Table(Settings.Tables.NomInterestedParty);
			await interestedPartyCat.AddRange(GetFrom(interestedPartyCat.Object, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/InterestedParty.json"));

			var nomCpi = service.Table(Settings.Tables.Kpi);
			await nomCpi.AddRange(GetFrom(nomCpi.Object, "Installation/SystemEntitySeed/DynamicEntities/KPI.json"));

			var partyRequirement = service.Table(Settings.Tables.PartyRequirement);
			await partyRequirement.AddRange(GetFrom(partyRequirement.Object, "Installation/SystemEntitySeed/DynamicEntities/PartyRequirement.json"));

			var goalPartyReq = service.Table(Settings.Tables.GoalPartyRequirement);
			await goalPartyReq.AddRange(GetFrom(goalPartyReq.Object, "Installation/SystemEntitySeed/DynamicEntities/GoalPartyRequirement.json"));

			var standards = service.Table(Settings.Tables.Standards);
			await standards.AddRange(GetFrom(standards.Object, "Installation/SystemEntitySeed/DynamicEntities/Standards.json"));

			var standardCategory = service.Table(Settings.Tables.StandardCategories);
			await standardCategory.AddRange(GetFrom(standardCategory.Object, "Installation/SystemEntitySeed/DynamicEntities/StandardCategories.json"));

			var categoryRequirements = service.Table(Settings.Tables.CategoryRequirements);
			await categoryRequirements.AddRange(GetFrom(categoryRequirements.Object, "Installation/SystemEntitySeed/DynamicEntities/CategoryRequirements.json"));
		}

		private static IEnumerable<dynamic> GetFrom(dynamic entity, string filePath)
		{
			return JsonParser.ReadDataListFromJsonWithTypeParameter(Path.Combine(AppContext.BaseDirectory, filePath), entity);
		}
	}
}
