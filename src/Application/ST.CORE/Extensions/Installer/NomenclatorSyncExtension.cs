using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ST.DynamicEntityStorage.Abstractions;
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


		public static NomType activ = new NomType
		{
			Id = Guid.NewGuid(),
			Name = "Activ",
			NomenclatorId = n.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};

		public static NomType pasiv = new NomType
		{
			Id = Guid.NewGuid(),
			Name = "Pasiv",
			NomenclatorId = n.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomType neutru = new NomType
		{
			Id = Guid.NewGuid(),
			Name = "Neutru",
			NomenclatorId = n.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomType activ1 = new NomType
		{
			Id = Guid.NewGuid(),
			Name = "Activ1",
			NomenclatorId = n.FirstOrDefault().Id,
			ParentId = activ.Id,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomType activ2 = new NomType
		{
			Id = Guid.NewGuid(),
			Name = "Activ2",
			NomenclatorId = n.FirstOrDefault().Id,
			ParentId = activ.Id,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomType activ3 = new NomType
		{
			Id = Guid.NewGuid(),
			Name = "Activ3",
			NomenclatorId = n.FirstOrDefault().Id,
			ParentId = activ.Id,
			Author = "System",
			Created = DateTime.Now
		};

		#region KPI category Nomenclator
		public static List<Nomenclator> kPICategory = new List<Nomenclator>()
		{
			new Nomenclator()
			{
				Name = "KPI Category",
				Description="",
				MachineName="NomKPICategory"

			},
		};
		public static NomKPICategory objectiveKPI = new NomKPICategory
		{
			Id = Guid.NewGuid(),
			Name = "Objective KPI",
			NomenclatorId = kPICategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomKPICategory processKPI = new NomKPICategory
		{
			Id = Guid.NewGuid(),
			Name = "Process KPI",
			NomenclatorId = kPICategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomKPICategory managementKPI = new NomKPICategory
		{
			Id = Guid.NewGuid(),
			Name = "Management KPI",
			NomenclatorId = kPICategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomKPICategory controlsKPI = new NomKPICategory
		{
			Id = Guid.NewGuid(),
			Name = "Controls KPI",
			NomenclatorId = kPICategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		#endregion

		#region Measurements items Nomenclator
		public static List<Nomenclator> measurementsItemsCategory = new List<Nomenclator>()
		{
			new Nomenclator()
			{
				Name = "Measurements Items Category",
				Description="",
				MachineName="NomMeasurement"
			},
		};

		public static NomMeasurement procent = new NomMeasurement
		{
			Id = Guid.NewGuid(),
			Name = "Procent",
			NomenclatorId = measurementsItemsCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomMeasurement number = new NomMeasurement
		{
			Id = Guid.NewGuid(),
			Name = "Number",
			NomenclatorId = measurementsItemsCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomMeasurement boolean = new NomMeasurement
		{
			Id = Guid.NewGuid(),
			Name = "Boolean",
			NomenclatorId = measurementsItemsCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};

		#endregion

		#region Calculation period Nomenclator
		public static List<Nomenclator> periodsCategory = new List<Nomenclator>()
		{
			new Nomenclator()
			{
				Name = "Calculation period Category",
				Description="",
				MachineName="NomPeriod"
			},
		};

		public static NomPeriod monthly = new NomPeriod
		{
			Id = Guid.NewGuid(),
			Name = "Monthly",
			NomenclatorId = periodsCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomPeriod quarterly = new NomPeriod
		{
			Id = Guid.NewGuid(),
			Name = "Quarterly",
			NomenclatorId = periodsCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomPeriod biannual = new NomPeriod
		{
			Id = Guid.NewGuid(),
			Name = "Biannual",
			NomenclatorId = periodsCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomPeriod annual = new NomPeriod
		{
			Id = Guid.NewGuid(),
			Name = "Annual",
			NomenclatorId = periodsCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};

		#endregion

		#region Criterion of fulfillment Nomenclator
		public static List<Nomenclator> fulfillmentCategory = new List<Nomenclator>()
		{
			new Nomenclator()
			{
				Name = "Criterion of fulfillment Category",
				Description="",
				MachineName="NomFulfillment"
			},
		};

		public static NomFulfillment yes = new NomFulfillment
		{
			Id = Guid.NewGuid(),
			Name = "The real value is Yes",
			NomenclatorId = fulfillmentCategory.FirstOrDefault().Id,
			ParentId = null,
			DependencyId = boolean.Id,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomFulfillment no = new NomFulfillment
		{
			Id = Guid.NewGuid(),
			Name = "The real value is No",
			NomenclatorId = fulfillmentCategory.FirstOrDefault().Id,
			ParentId = null,
			DependencyId = boolean.Id,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomFulfillment greater = new NomFulfillment
		{
			Id = Guid.NewGuid(),
			Name = "Actual value greater than or equal to the target value",
			NomenclatorId = fulfillmentCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomFulfillment less = new NomFulfillment
		{
			Id = Guid.NewGuid(),
			Name = "The actual value is less than or equal to the target value",
			NomenclatorId = fulfillmentCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};

		#endregion

		//#region Objective Nomenclator
		//public static List<Nomenclator> objectiveCategory = new List<Nomenclator>()
		//{
		//	new Nomenclator()
		//	{   Id=Guid.Parse("8bc32838-03dd-43c9-8a07-1a044f6f6f25"),
		//		Name = "Objective",
		//		Description=""
		//	},
		//};

		//public static NomenclatorItem o1 = new NomenclatorItem
		//{
		//	Id = Guid.Parse("bd9c32b7-10a7-45d5-b9aa-cb16f2bf67a5"),
		//	Name = "O1 Asigurarea continuitatii operatiunilor",
		//	NomenclatorId = objectiveCategory.FirstOrDefault().Id,
		//	ParentId = null,
		//	DependencyId = null,
		//	Author = "System",
		//	Created = DateTime.Now
		//};
		//public static NomenclatorItem o11 = new NomenclatorItem
		//{
		//	Id = Guid.Parse("d43eefaf-3a76-4c25-ba37-1cfb97c6d1c9"),
		//	Name = "O1.1 Existenta unui BCP",
		//	NomenclatorId = objectiveCategory.FirstOrDefault().Id,
		//	ParentId = o1.Id,
		//	DependencyId = null,
		//	Author = "System",
		//	Created = DateTime.Now
		//};
		//public static NomenclatorItem o12 = new NomenclatorItem
		//{
		//	Id = Guid.Parse("af3dbba9-7346-4545-b62e-b3cae89da81f"),
		//	Name = "O1.2 Testarea anuala a BCP-ului",
		//	NomenclatorId = objectiveCategory.FirstOrDefault().Id,
		//	ParentId = o1.Id,
		//	DependencyId = null,
		//	Author = "System",
		//	Created = DateTime.Now
		//};
		//#endregion

		//#region requirement Nomenclator
		//public static List<Nomenclator> requirementCategory = new List<Nomenclator>()
		//{
		//	new Nomenclator()
		//	{   Id=Guid.Parse("04ae72fa-325b-40be-9bcc-a6e849eca060"),
		//		Name = "Requirements",
		//		Description=""
		//	},
		//};

		//public static NomenclatorItem r1 = new NomenclatorItem
		//{
		//	Id = Guid.Parse("7ece6749-4b83-4da7-8ed5-1795d8c85dc6"),
		//	Name = "Sa existe continuitatea activitatii",
		//	NomenclatorId = requirementCategory.FirstOrDefault().Id,
		//	ParentId = null,
		//	DependencyId = null,
		//	Author = "System",
		//	Created = DateTime.Now
		//};
		//public static NomenclatorItem r2 = new NomenclatorItem
		//{
		//	Id = Guid.Parse("d43eefaf-3a76-4c25-ba37-1cfb97c6d1c9"),
		//	Name = "Sa fie tranzactiile securizate",
		//	NomenclatorId = requirementCategory.FirstOrDefault().Id,
		//	ParentId = null,
		//	DependencyId = null,
		//	Author = "System",
		//	Created = DateTime.Now
		//};
		//public static NomenclatorItem r3 = new NomenclatorItem
		//{
		//	Id = Guid.Parse("36d76dd4-910e-4632-b647-eb3ddd0a7555"),
		//	Name = "Sa se acorde suport pentru investigarea incidentelor",
		//	NomenclatorId = requirementCategory.FirstOrDefault().Id,
		//	ParentId = null,
		//	DependencyId = null,
		//	Author = "System",
		//	Created = DateTime.Now
		//};
		//#endregion

		//#region Category Interested party Nomenclator
		//public static List<Nomenclator> categoryInterestedParty = new List<Nomenclator>()
		//{
		//	new Nomenclator()
		//	{   Id=Guid.Parse("1b9f9bd9-989e-4c96-9c5d-c0e2fc01bfb4"),
		//		Name = "Category interested party",
		//		Description=""
		//	},
		//};
		//public static NomenclatorItem intern = new NomenclatorItem
		//{
		//	Id = Guid.Parse("57196dfa-df32-4e3e-8d41-dad04db1a681"),
		//	Name = "Internal",
		//	NomenclatorId = categoryInterestedParty.FirstOrDefault().Id,
		//	ParentId = null,
		//	DependencyId = null,
		//	Author = "System",
		//	Created = DateTime.Now
		//};
		//public static NomenclatorItem external = new NomenclatorItem
		//{
		//	Id = Guid.Parse("bdf33400-a677-4723-bc41-c613573ee586"),
		//	Name = "External",
		//	NomenclatorId = categoryInterestedParty.FirstOrDefault().Id,
		//	ParentId = null,
		//	DependencyId = null,
		//	Author = "System",
		//	Created = DateTime.Now
		//};

		//#endregion

		//#region  Interested party Nomenclator
		//public static List<Nomenclator> interestedParty = new List<Nomenclator>()
		//{
		//	new Nomenclator()
		//	{   Id=Guid.Parse("244d70a8-fb9a-4c74-be9a-ffce4400bb53"),
		//		Name = "Interested party",
		//		Description=""
		//	},
		//};
		//public static NomenclatorItem ip1 = new NomenclatorItem
		//{
		//	Id = Guid.Parse("8de1111d-4fe7-48c8-a57e-5830b77043c6"),
		//	Name = "Actionari",
		//	NomenclatorId = interestedParty.FirstOrDefault().Id,
		//	ParentId = null,
		//	DependencyId = null,
		//	Author = "System",
		//	RefId=intern.Id,
		//	Created = DateTime.Now
		//};
		//public static NomenclatorItem ip2 = new NomenclatorItem
		//{
		//	Id = Guid.Parse("0f606382-3597-405d-879a-f985829eead1"),
		//	Name = "Autoritati publice",
		//	NomenclatorId = interestedParty.FirstOrDefault().Id,
		//	ParentId = null,
		//	DependencyId = null,
		//	Author = "System",
		//	RefId=external.Id,
		//	Created = DateTime.Now
		//};

		//#endregion
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

		#endregion

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


		public static List<NomType> nomenclatorItems = new List<NomType>();
		public static List<NomType> childItems = new List<NomType>();
		public static List<NomKPICategory> categoryItems = new List<NomKPICategory>();
		public static List<NomMeasurement> measurementsItems = new List<NomMeasurement>();
		public static List<NomPeriod> periodsItems = new List<NomPeriod>();
		public static List<NomFulfillment> fulfillmentItems = new List<NomFulfillment>();
		public static List<NomenclatorItem> objectiveItems = new List<NomenclatorItem>();
		public static List<NomenclatorItem> requirementItems = new List<NomenclatorItem>();
		public static List<NomenclatorItem> categoryInterestedPartyItems = new List<NomenclatorItem>();
		public static List<NomenclatorItem> interestedPartyItems = new List<NomenclatorItem>();
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
		/// <param name="dataService"></param>
		public static async Task SyncNomenclatorItems(IDynamicService dataService)
		{
			nomenclatorItems.Add(activ);
			nomenclatorItems.Add(pasiv);
			nomenclatorItems.Add(neutru);
			childItems.Add(activ1);
			childItems.Add(activ2);
			childItems.Add(activ3);
			categoryItems.Add(objectiveKPI);
			categoryItems.Add(processKPI);
			categoryItems.Add(managementKPI);
			categoryItems.Add(controlsKPI);
			measurementsItems.Add(procent);
			measurementsItems.Add(number);
			measurementsItems.Add(boolean);
			periodsItems.Add(monthly);
			periodsItems.Add(quarterly);
			periodsItems.Add(biannual);
			periodsItems.Add(annual);
			fulfillmentItems.Add(yes);
			fulfillmentItems.Add(no);
			fulfillmentItems.Add(less);
			fulfillmentItems.Add(greater);
			//objectiveItems.Add(o11);
			//objectiveItems.Add(o12);
			//requirementItems.Add(r1);
			//requirementItems.Add(r2);
			//requirementItems.Add(r3);
			//categoryInterestedPartyItems.Add(intern);
			//categoryInterestedPartyItems.Add(external);
			//interestedPartyItems.Add(ip1);
			//interestedPartyItems.Add(ip2);
			



			var nomenclatorExists = await dataService.Any<Nomenclator>();
			if (nomenclatorExists) return;
			await dataService.AddRange(n);
			await dataService.AddRange(kPICategory);
			await dataService.AddRange(measurementsItemsCategory);
			await dataService.AddRange(periodsCategory);
			await dataService.AddRange(fulfillmentCategory);
			//await dataService.AddRange(objectiveCategory);
			//await dataService.AddRange(requirementCategory);
			//await dataService.AddRange(categoryInterestedParty);
			//await dataService.AddRange(interestedParty);


			//var existsNomKPICategoryItems = await dataService.Any<NomKPICategory>();

			if (!await dataService.Any<NomType>())
			{
				await dataService.AddRange(nomenclatorItems);
				await dataService.AddRange(childItems);
			}
			if (!await dataService.Any<NomKPICategory>())
			{
				await dataService.AddRange(categoryItems);
			}
			if (!await dataService.Any<NomMeasurement>())
			{
				await dataService.AddRange(measurementsItems);
			}
			if (!await dataService.Any<Requirement>())
			{
				await dataService.AddSystem(organization);
				await dataService.AddSystem(organization41);
				await dataService.AddSystem(organization42);
				await dataService.AddSystem(organization42a);
				await dataService.AddSystem(organization42b);

			}
			if (!await dataService.Any<RequirementAction>())
			{
				await dataService.AddSystem(action1);
			}

			var exists = await dataService.Any<NomenclatorItem>();
			if (exists) return;
		
			
			await dataService.AddRange(periodsItems);
			await dataService.AddRange(fulfillmentItems);
			await dataService.AddRange(objectiveItems);
			await dataService.AddRange(requirementItems);
			await dataService.AddRange(categoryInterestedPartyItems);
			await dataService.AddRange(interestedPartyItems);

			var test = periodsCategory.FirstOrDefault().Id;
			if (!await dataService.Any<KPI>())
			{
				var kpiEx = new KPI
				{
					Name = "Base KPI",
					Description = "Seeded",
					CategoryId = categoryItems.FirstOrDefault().Id,
					CalculationMethod = "Manual",
					PeriodId = periodsItems.FirstOrDefault().Id,
					MeasurementUnitId = measurementsItems.FirstOrDefault().Id,
					ProcentGoal = "35% si mai mult",
					BoolGoal = true,
					IntGoal = 890,
					FulfillmentCriterionId = fulfillmentItems.FirstOrDefault().Id,
					Status = true
				

				};
				await dataService.AddSystem(kpiEx);
			}
			//if (!await dataService.Any<Requirement>())
			//{
			//	await dataService.AddSystem(
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
			//		var res = await dataService.AddSystem(item);
			//		//if (!res.IsSuccess) continue;
			//		//foreach (var i in item.SubItems)
			//		//{
			//		//	var obj = i.Adapt<NomenclatorItem>();
			//		//	obj.ParentMenuItemId = res.Result;
			//		//	var r = await dataService.AddSystem(obj);
			//		//	if (!r.IsSuccess || i.SubItems == null) continue;
			//		//	foreach (var j in i.SubItems)
			//		//	{
			//		//		var ob = j.Adapt<NomenclatorItem>();
			//		//		ob.ParentMenuItemId = r.Result;
			//		//		var r1 = await dataService.AddSystem(ob);
			//		//		if (!r1.IsSuccess || j.SubItems == null) continue;
			//		//		foreach (var m in j.SubItems)
			//		//		{
			//		//			var ob1 = m.Adapt<NomenclatorItem>();
			//		//			ob1.ParentMenuItemId = r1.Result;
			//		//			await dataService.AddSystem(ob1);
			//		//		}
			//		//	}
			//		//}
			//	}
			//}
		}

		/// <summary>
		/// Read menus
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<NomenclatorViewModel> GetGetNomenclators()
			=> JsonParser.ReadArrayDataFromJsonFile<List<NomenclatorViewModel>>(Path.Combine(AppContext.BaseDirectory, "nomenclator.json"));
	}
}
