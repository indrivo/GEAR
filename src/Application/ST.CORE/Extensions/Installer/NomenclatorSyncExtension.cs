using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ST.Entities.Extensions;
using ST.Entities.Models.Actions;
using ST.Entities.Models.KPI;
using ST.Entities.Models.Nomenclator;
using ST.Entities.Models.Requirement;
using ST.Entities.Models.Standart;
using ST.Entities.Services.Abstraction;

namespace ST.CORE.Extensions.Installer
{
	public class NomenclatorSyncExtension
	{

		public static List<Nomenclator> n = new List<Nomenclator>()
		{
			new Nomenclator()
			{
				Name = "NomenclatorType",
				Description="test",
				MachineName="NomType"
			},
		};

	
		public static async Task SyncNomenclatorItems(IDynamicEntityDataService service)
		{
			var vocabularyList = GetBaseNomenclators();
			var kpiCategoryItems = GetKpiCategoryItems();
			var measurementsCategoryItems = GetMeasurementsCategoryItems();
			var periodsCategoryItems = GetPeriodsCategoryItems();
			var fulfillmentCategoryItems = GetFulfillmentCategoryItems();
			var goalCategoryItems = GetGoalsCategoryItems();
			var goalRequirementsCategoryItems = GetGoalRequirementsCategoryItems();
			var interestedPartyTypeCategoryItems = GetInterestedPartyTypeCategoriesItems();
			var interestedPartyCategoryItems = GetInterestedPartyCategoriesItems();
			


			await service.AddRange(vocabularyList);
			await service.AddRange(kpiCategoryItems);
			await service.AddRange(measurementsCategoryItems);
			await service.AddRange(periodsCategoryItems);
			await service.AddRange(fulfillmentCategoryItems);
			await service.AddRange(goalCategoryItems);
			await service.AddRange(goalRequirementsCategoryItems);
			await service.AddRange(interestedPartyTypeCategoryItems);
			await service.AddRange(interestedPartyCategoryItems);
		

			await service.AddRange(GetKpiItems());
			await service.AddRange(GetPartyRequirement());
			await service.AddRange(GetGoalPartyRequirement());
			await service.AddRange(GetStandarts());			
			await service.AddRange(GetStandardCategories());
			await service.AddRange(GetCategoryRequirements());
		

			var exists = await service.Any<NomenclatorItem>();
			if (exists) return;
		
		}

		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<Nomenclator> GetBaseNomenclators()
			=> JsonParser.ReadArrayDataFromJsonFile<List<Nomenclator>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/Vocabulary/BaseVocabulary/VocabularyList.json"));

		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<NomKPICategory> GetKpiCategoryItems()
			=> JsonParser.ReadArrayDataFromJsonFile<List<NomKPICategory>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/KpiCategory.json"));

		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<NomMeasurement> GetMeasurementsCategoryItems()
			=> JsonParser.ReadArrayDataFromJsonFile<List<NomMeasurement>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/MeasurementCategory.json"));

		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<NomPeriod> GetPeriodsCategoryItems()
			=> JsonParser.ReadArrayDataFromJsonFile<List<NomPeriod>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/PeriodCategory.json"));
		
		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<NomFulfillment> GetFulfillmentCategoryItems()
			=> JsonParser.ReadArrayDataFromJsonFile<List<NomFulfillment>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/FulfillmentCategory.json"));


		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<NomGoal> GetGoalsCategoryItems()
			=> JsonParser.ReadArrayDataFromJsonFile<List<NomGoal>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/ObjectiveCategory.json"));

		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<NomRequirement> GetGoalRequirementsCategoryItems()
			=> JsonParser.ReadArrayDataFromJsonFile<List<NomRequirement>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/NomRequirement.json"));

		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<NomInterestedPartyType> GetInterestedPartyTypeCategoriesItems()
			=> JsonParser.ReadArrayDataFromJsonFile<List<NomInterestedPartyType>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/InterestedPartyCategory.json"));


		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<NomInterestedParty> GetInterestedPartyCategoriesItems()
			=> JsonParser.ReadArrayDataFromJsonFile<List<NomInterestedParty>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/InterestedParty.json"));

		/*********Entities seed**************/

		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<KPI> GetKpiItems()
			=> JsonParser.ReadArrayDataFromJsonFile<List<KPI>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/DynamicEntities/KPI.json"));


		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<PartyRequirement> GetPartyRequirement()
			=> JsonParser.ReadArrayDataFromJsonFile<List<PartyRequirement>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/DynamicEntities/PartyRequirement.json"));

		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<GoalPartyRequirement> GetGoalPartyRequirement()
			=> JsonParser.ReadArrayDataFromJsonFile<List<GoalPartyRequirement>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/DynamicEntities/GoalPartyRequirement.json"));

		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<Standards> GetStandarts()
			=> JsonParser.ReadArrayDataFromJsonFile<List<Standards>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/DynamicEntities/Standards.json"));

		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<StandardCategories> GetStandardCategories()
			=> JsonParser.ReadArrayDataFromJsonFile<List<StandardCategories>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/DynamicEntities/StandardCategories.json"));

		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<CategoryRequirements> GetCategoryRequirements()
			=> JsonParser.ReadArrayDataFromJsonFile<List<CategoryRequirements>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/DynamicEntities/CategoryRequirements.json"));


	}
}
