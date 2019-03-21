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







		#region Requirements
		public static Requirement organization = new Requirement
		{
			Id = Guid.Parse("2dc1dee7-c8dd-49a1-9fe9-a94b6010c344"),
			Name = " 4  Organization",
			Created = DateTime.Now,
			ParentId = null,
			Treegrid = 1,
			ParentTreegrid=null
		
		};

		public static Requirement organization41 = new Requirement
		{
			Id = Guid.Parse("840829f7-c78b-48dc-8997-a6cd12ecefa4"),
			Name = "4.1 Understanding the organization and its context",
			Created = DateTime.Now,
			ParentId = organization.Id,
			Treegrid = 2,
			ParentTreegrid = 1
		};
		public static Requirement organization42 = new Requirement
		{
			Id = Guid.Parse("ebf188bc-4865-46c2-ad39-747c8899386d"),
			Name = "4.2 Understanding the needs and expectations of interested parties",
			Created = DateTime.Now,
			ParentId = organization.Id,
			Treegrid = 3,
			ParentTreegrid = 1
		};
		public static Requirement organization42a = new Requirement
		{
			Id = Guid.Parse("ca74a4cc-441c-4676-be70-db0fedc49267"),
			Name = "4.2.a Interested parties that are relevant to the information security management system",
			Created = DateTime.Now,
			ParentId = organization42.Id,
			Treegrid = 4,
			ParentTreegrid = 3
		};
		public static Requirement organization42b = new Requirement
		{
			Id = Guid.Parse("11ce8d72-db89-4863-be20-5b631bbd74f5"),
			Name = "4.2.b the requirements of these interested parties relevant to information security.",
			Created = DateTime.Now,
			ParentId = organization42.Id,
			Treegrid = 5,
			ParentTreegrid = 3
		};
		//5
		public static Requirement leadership = new Requirement
		{
			Id = Guid.Parse("eaf03b23-9850-4f6e-8f6a-85c2c4e9ae91"),
			Name = " 5  Leadership",
			Created = DateTime.Now,
			ParentId = null,
			Treegrid = 7,
			ParentTreegrid = null

		};

		public static Requirement leadership51 = new Requirement
		{
			Id = Guid.Parse("04a00264-83d6-45f7-8add-201ae8a9906a"),
			Name = " 5.1 Leadership and commitment",
			Created = DateTime.Now,
			ParentId = leadership.Id,
			Treegrid = 8,
			ParentTreegrid = 7
		};
		public static Requirement leadership51a = new Requirement
		{
			Id = Guid.Parse("ae6083de-35fa-412f-b7fe-197d0c2cc9f3"),
			Name = "5.1.a ensuring the information security policy and the information security objectives are established and are compatible with the strategic direction of the organization;",
			Created = DateTime.Now,
			ParentId = organization.Id,
			Treegrid = 9,
			ParentTreegrid = 8
		};
		public static Requirement leadership51b = new Requirement
		{
			Id = Guid.Parse("997d7ea1-3e74-46d0-ae9c-5a3e11b2d3a4"),
			Name = "5.1.b ensuring the integration of the information security management system requirements into the organization’s processes;",
			Created = DateTime.Now,
			ParentId = organization.Id,
			Treegrid = 10,
			ParentTreegrid = 8
		};
		public static Requirement leadership51c = new Requirement
		{
			Id = Guid.Parse("3d9fb7b6-2d1e-4b3d-9e9e-f55d7c829777"),
			Name = "5.1.c ensuring that the resources needed for the information security management system are available;",
			Created = DateTime.Now,
			ParentId = organization.Id,
			Treegrid = 11,
			ParentTreegrid = 8
		};
		public static Requirement leadership52 = new Requirement
		{
			Id = Guid.Parse("62644b14-67ee-4ad1-966b-e2044386f77e"),
			Name = "5.2 Policy",
			Created = DateTime.Now,
			ParentId = organization.Id,
			Treegrid = 12,
			ParentTreegrid = 7
		};
		public static Requirement leadership52a = new Requirement
		{
			Id = Guid.Parse("d1628fba-b81d-4ef9-96c0-085c401fcb84"),
			Name = "5.2.a is appropriate to the purpose of the organization;",
			Created = DateTime.Now,
			ParentId = organization.Id,
			Treegrid = 13,
			ParentTreegrid = 12
		};
		#endregion
		public static Requirement leadership52b = new Requirement
		{
			Id = Guid.Parse("417b7e69-c918-4d51-9b3e-e509137c58fb"),
			Name = "5.2.b includes information security objectives (see 6.2) or provides the framework for setting information security objectives;",
			Created = DateTime.Now,
			ParentId = organization.Id,
			Treegrid = 14,
			ParentTreegrid = 12
		};

		#region Actions
		public static RequirementAction action1 = new RequirementAction
		{
			Id = Guid.Parse("2595f241-80e3-471f-8558-836594bc278a"),
			Name = "4.2.a-1) To determine interested parties that are relevant to the ISMS",
			Created = DateTime.Now,
			RequirementId = organization42a.Id,
			Treegrid = 6,
			DeadLine=DateTime.Now.AddDays(10)		

		};

		

		#endregion


	    public static List<Requirement> requirementsList = new List<Requirement>();

		//#region KPIRegion

		//public static KPI baseKPI = new KPI
		//{
		//	Name = "Base KPI",
		//	Description = "Seeded",
		//	CategoryId = CategoryItems.FirstOrDefault().Id,
		//	CalculationMethod = "Manual"

		//};
		//#endregion


		/// <summary>
		/// Sync default menus
		/// </summary>
		/// <param name="service"></param>
		public static async Task SyncNomenclatorItems(IDynamicEntityDataService service)
		{
			var vocabularyList = GetBaseNomenclators();
			var kpiCategoryItems = GetKpiCategoryItems();
			var measurementsCategoryItems = GetMeasurementsCategoryItems();
			var periodsCategoryItems = GetPeriodsCategoryItems();
			var fulfillmentCategoryItems = GetFulfillmentCategoryItems();
			var objectiveCategoryItems = GetObjectiveCategoryItems();
			var requirementsCategoryItems = GetRequirementsCategoryItems();
			var interestedPartyTypeCategoryItems = GetInterestedPartyTypeCategoriesItems();
			var interestedPartyCategoryItems = GetInterestedPartyCategoriesItems();


			await service.AddRange(vocabularyList);
			await service.AddRange(kpiCategoryItems);
			await service.AddRange(measurementsCategoryItems);
			await service.AddRange(periodsCategoryItems);
			await service.AddRange(fulfillmentCategoryItems);
			await service.AddRange(objectiveCategoryItems);
			await service.AddRange(requirementsCategoryItems);
			await service.AddRange(interestedPartyTypeCategoryItems);
			await service.AddRange(interestedPartyCategoryItems);



			//var existsNomKPICategoryItems = await service.Any<NomKPICategory>();

			//if (!await service.Any<NomType>())
			//{
			//	await service.AddRange(nomenclatorItems);
			//	await service.AddRange(childItems);
			//}
			//if (!await service.Any<NomKPICategory>())
			//{
			//	await service.AddRange(categoryItems);
			//}
			//if (!await service.Any<NomMeasurement>())
			//{
			//	await service.AddRange(measurementsItems);
			//}
			if (!await service.Any<Requirement>())
			{
				await service.AddSystem(organization);
				await service.AddSystem(organization41);
				await service.AddSystem(organization42);
				await service.AddSystem(organization42a);
				await service.AddSystem(organization42b);

				//await service.AddSystem(leadership);
				//await service.AddSystem(leadership51);
				//await service.AddSystem(leadership51a);
				//await service.AddSystem(leadership51b);
				//await service.AddSystem(leadership51c);
				//await service.AddSystem(leadership52);
				//await service.AddSystem(leadership52a);
				//await service.AddSystem(leadership52b);
					

			}
			if (!await service.Any<RequirementAction>())
			{
				await service.AddSystem(action1);
			}

			var exists = await service.Any<NomenclatorItem>();
			if (exists) return;
		

			//var test = periodsCategory.FirstOrDefault().Id;
			//if (!await service.Any<KPI>())
			//{
			//	var kpiEx = new KPI
			//	{
			//		Name = "Base KPI",
			//		Description = "Seeded",
			//		CategoryId = categoryItems.FirstOrDefault().Id,
			//		CalculationMethod = "Manual",
			//		PeriodId = periodsItems.FirstOrDefault().Id,
			//		MeasurementUnitId = measurementsItems.FirstOrDefault().Id,
			//		ProcentGoal = "35% si mai mult",
			//		BoolGoal = true,
			//		IntGoal = 890,
			//		FulfillmentCriterionId = fulfillmentItems.FirstOrDefault().Id,
			//		Status = true
				

			//	};
			//	await service.AddSystem(kpiEx);
			//}
			//if (!await service.Any<Requirement>())
			//{
			//	await service.AddSystem(
			//		new Requirement
			//		{
			//			InterestedPartId = ip1.Id,
			//			RequirementId = r1.Id,
			//			Comments = "System seeded request"
			//		});
			//}
			//if (rq.All(x => x.IsSuccess))
			//{
			//	foreach (var item in GetGetNomenclators())
			//	{
			//		var res = await service.AddSystem(item);
			//		//if (!res.IsSuccess) continue;
			//		//foreach (var i in item.SubItems)
			//		//{
			//		//	var obj = i.Adapt<NomenclatorItem>();
			//		//	obj.ParentMenuItemId = res.Result;
			//		//	var r = await service.AddSystem(obj);
			//		//	if (!r.IsSuccess || i.SubItems == null) continue;
			//		//	foreach (var j in i.SubItems)
			//		//	{
			//		//		var ob = j.Adapt<NomenclatorItem>();
			//		//		ob.ParentMenuItemId = r.Result;
			//		//		var r1 = await service.AddSystem(ob);
			//		//		if (!r1.IsSuccess || j.SubItems == null) continue;
			//		//		foreach (var m in j.SubItems)
			//		//		{
			//		//			var ob1 = m.Adapt<NomenclatorItem>();
			//		//			ob1.ParentMenuItemId = r1.Result;
			//		//			await service.AddSystem(ob1);
			//		//		}
			//		//	}
			//		//}
			//	}
			//}
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
		private static IEnumerable<NomObjective> GetObjectiveCategoryItems()
			=> JsonParser.ReadArrayDataFromJsonFile<List<NomObjective>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/ObjectiveCategory.json"));

		/// <summary>
		/// Read Nomenclators
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<NomRequirement> GetRequirementsCategoryItems()
			=> JsonParser.ReadArrayDataFromJsonFile<List<NomRequirement>>(Path.Combine(AppContext.BaseDirectory, "Installation/SystemEntitySeed/Vocabulary/VocabularyItems/RequirementCategory.json"));

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


	}
}
