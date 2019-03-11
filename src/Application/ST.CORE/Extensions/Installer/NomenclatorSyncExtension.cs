using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ST.Entities.Extensions;
using ST.Entities.Models.KPI;
using ST.Entities.Models.Nomenclator;
using ST.Entities.Models.Request;
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
				Description="test"
			},
		};


		public static NomenclatorItem activ = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Activ",
			NomenclatorId = n.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};

		public static NomenclatorItem pasiv = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Pasiv",
			NomenclatorId = n.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem neutru = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Neutru",
			NomenclatorId = n.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem activ1 = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Activ1",
			NomenclatorId = n.FirstOrDefault().Id,
			ParentId = activ.Id,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem activ2 = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Activ2",
			NomenclatorId = n.FirstOrDefault().Id,
			ParentId = activ.Id,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem activ3 = new NomenclatorItem
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
				Description=""
			},
		};
		public static NomenclatorItem objectiveKPI = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Objective KPI",
			NomenclatorId = kPICategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem processKPI = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Process KPI",
			NomenclatorId = kPICategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem managementKPI = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Management KPI",
			NomenclatorId = kPICategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem controlsKPI = new NomenclatorItem
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
				Description=""
			},
		};

		public static NomenclatorItem procent = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Procent",
			NomenclatorId = measurementsItemsCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem number = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Number",
			NomenclatorId = measurementsItemsCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem boolean = new NomenclatorItem
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
				Description=""
			},
		};

		public static NomenclatorItem monthly = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Monthly",
			NomenclatorId = periodsCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem quarterly = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Quarterly",
			NomenclatorId = periodsCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem biannual = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Biannual",
			NomenclatorId = periodsCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem annual = new NomenclatorItem
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
				Description=""
			},
		};

		public static NomenclatorItem yes = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "The real value is Yes",
			NomenclatorId = fulfillmentCategory.FirstOrDefault().Id,
			ParentId = null,
			DependencyId = boolean.Id,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem no = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "The real value is No",
			NomenclatorId = fulfillmentCategory.FirstOrDefault().Id,
			ParentId = null,
			DependencyId = boolean.Id,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem greater = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "Actual value greater than or equal to the target value",
			NomenclatorId = fulfillmentCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem less = new NomenclatorItem
		{
			Id = Guid.NewGuid(),
			Name = "The actual value is less than or equal to the target value",
			NomenclatorId = fulfillmentCategory.FirstOrDefault().Id,
			ParentId = null,
			Author = "System",
			Created = DateTime.Now
		};

		#endregion

		#region Objective Nomenclator
		public static List<Nomenclator> objectiveCategory = new List<Nomenclator>()
		{
			new Nomenclator()
			{   Id=Guid.Parse("8bc32838-03dd-43c9-8a07-1a044f6f6f25"),
				Name = "Objective",
				Description=""
			},
		};

		public static NomenclatorItem o1 = new NomenclatorItem
		{
			Id = Guid.Parse("bd9c32b7-10a7-45d5-b9aa-cb16f2bf67a5"),
			Name = "O1 Asigurarea continuitatii operatiunilor",
			NomenclatorId = objectiveCategory.FirstOrDefault().Id,
			ParentId = null,
			DependencyId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem o11 = new NomenclatorItem
		{
			Id = Guid.Parse("d43eefaf-3a76-4c25-ba37-1cfb97c6d1c9"),
			Name = "O1.1 Existenta unui BCP",
			NomenclatorId = objectiveCategory.FirstOrDefault().Id,
			ParentId = o1.Id,
			DependencyId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem o12 = new NomenclatorItem
		{
			Id = Guid.Parse("af3dbba9-7346-4545-b62e-b3cae89da81f"),
			Name = "O1.2 Testarea anuala a BCP-ului",
			NomenclatorId = objectiveCategory.FirstOrDefault().Id,
			ParentId = o1.Id,
			DependencyId = null,
			Author = "System",
			Created = DateTime.Now
		};
		#endregion

		#region requirement Nomenclator
		public static List<Nomenclator> requirementCategory = new List<Nomenclator>()
		{
			new Nomenclator()
			{   Id=Guid.Parse("04ae72fa-325b-40be-9bcc-a6e849eca060"),
				Name = "Requirements",
				Description=""
			},
		};

		public static NomenclatorItem r1 = new NomenclatorItem
		{
			Id = Guid.Parse("7ece6749-4b83-4da7-8ed5-1795d8c85dc6"),
			Name = "Sa existe continuitatea activitatii",
			NomenclatorId = requirementCategory.FirstOrDefault().Id,
			ParentId = null,
			DependencyId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem r2 = new NomenclatorItem
		{
			Id = Guid.Parse("d43eefaf-3a76-4c25-ba37-1cfb97c6d1c9"),
			Name = "Sa fie tranzactiile securizate",
			NomenclatorId = requirementCategory.FirstOrDefault().Id,
			ParentId = null,
			DependencyId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem r3 = new NomenclatorItem
		{
			Id = Guid.Parse("36d76dd4-910e-4632-b647-eb3ddd0a7555"),
			Name = "Sa se acorde suport pentru investigarea incidentelor",
			NomenclatorId = requirementCategory.FirstOrDefault().Id,
			ParentId = null,
			DependencyId = null,
			Author = "System",
			Created = DateTime.Now
		};
		#endregion

		#region Category Interested party Nomenclator
		public static List<Nomenclator> categoryInterestedParty = new List<Nomenclator>()
		{
			new Nomenclator()
			{   Id=Guid.Parse("1b9f9bd9-989e-4c96-9c5d-c0e2fc01bfb4"),
				Name = "Category interested party",
				Description=""
			},
		};
		public static NomenclatorItem intern = new NomenclatorItem
		{
			Id = Guid.Parse("57196dfa-df32-4e3e-8d41-dad04db1a681"),
			Name = "Internal",
			NomenclatorId = categoryInterestedParty.FirstOrDefault().Id,
			ParentId = null,
			DependencyId = null,
			Author = "System",
			Created = DateTime.Now
		};
		public static NomenclatorItem external = new NomenclatorItem
		{
			Id = Guid.Parse("bdf33400-a677-4723-bc41-c613573ee586"),
			Name = "External",
			NomenclatorId = categoryInterestedParty.FirstOrDefault().Id,
			ParentId = null,
			DependencyId = null,
			Author = "System",
			Created = DateTime.Now
		};

		#endregion

		#region  Interested party Nomenclator
		public static List<Nomenclator> interestedParty = new List<Nomenclator>()
		{
			new Nomenclator()
			{   Id=Guid.Parse("244d70a8-fb9a-4c74-be9a-ffce4400bb53"),
				Name = "Interested party",
				Description=""
			},
		};
		public static NomenclatorItem ip1 = new NomenclatorItem
		{
			Id = Guid.Parse("8de1111d-4fe7-48c8-a57e-5830b77043c6"),
			Name = "Actionari",
			NomenclatorId = interestedParty.FirstOrDefault().Id,
			ParentId = null,
			DependencyId = null,
			Author = "System",
			RefId=intern.Id,
			Created = DateTime.Now
		};
		public static NomenclatorItem ip2 = new NomenclatorItem
		{
			Id = Guid.Parse("0f606382-3597-405d-879a-f985829eead1"),
			Name = "Autoritati publice",
			NomenclatorId = interestedParty.FirstOrDefault().Id,
			ParentId = null,
			DependencyId = null,
			Author = "System",
			RefId=external.Id,
			Created = DateTime.Now
		};

		#endregion

		public static List<NomenclatorItem> nomenclatorItems = new List<NomenclatorItem>();
		public static List<NomenclatorItem> childItems = new List<NomenclatorItem>();
		public static List<NomenclatorItem> categoryItems = new List<NomenclatorItem>();
		public static List<NomenclatorItem> measurementsItems = new List<NomenclatorItem>();
		public static List<NomenclatorItem> periodsItems = new List<NomenclatorItem>();
		public static List<NomenclatorItem> fulfillmentItems = new List<NomenclatorItem>();
		public static List<NomenclatorItem> objectiveItems = new List<NomenclatorItem>();
		public static List<NomenclatorItem> requirementItems = new List<NomenclatorItem>();
		public static List<NomenclatorItem> categoryInterestedPartyItems = new List<NomenclatorItem>();
		public static List<NomenclatorItem> interestedPartyItems = new List<NomenclatorItem>();

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
			objectiveItems.Add(o11);
			objectiveItems.Add(o12);
			requirementItems.Add(r1);
			requirementItems.Add(r2);
			requirementItems.Add(r3);
			categoryInterestedPartyItems.Add(intern);
			categoryInterestedPartyItems.Add(external);
			interestedPartyItems.Add(ip1);
			interestedPartyItems.Add(ip2);



			var nomenclatorExists = await service.Any<Nomenclator>();
			if (nomenclatorExists) return;
			await service.AddRange(n);
			await service.AddRange(kPICategory);
			await service.AddRange(measurementsItemsCategory);
			await service.AddRange(periodsCategory);
			await service.AddRange(fulfillmentCategory);
			await service.AddRange(objectiveCategory);
			await service.AddRange(requirementCategory);
			await service.AddRange(categoryInterestedParty);
			await service.AddRange(interestedParty);

			var exists = await service.Any<NomenclatorItem>();
			if (exists) return;
			await service.AddRange(nomenclatorItems);
			await service.AddRange(childItems);
			await service.AddRange(categoryItems);
			await service.AddRange(measurementsItems);
			await service.AddRange(periodsItems);
			await service.AddRange(fulfillmentItems);
			await service.AddRange(objectiveItems);
			await service.AddRange(requirementItems);
			await service.AddRange(categoryInterestedPartyItems);
			await service.AddRange(interestedPartyItems);

			var test = periodsCategory.FirstOrDefault().Id;
			if (!await service.Any<KPI>())
			{
				await service.AddSystem(
					new KPI
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

					});
			}
			if (!await service.Any<Request>())
			{
				await service.AddSystem(
					new Request
					{
						InterestedPartId=ip1.Id,
						RequestId=r1.Id,
						Comments="System seeded request"
					});
			}
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
		/// Read menus
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<NomenclatorViewModel> GetGetNomenclators()
			=> JsonParser.ReadArrayDataFromJsonFile<List<NomenclatorViewModel>>(Path.Combine(AppContext.BaseDirectory, "nomenclator.json"));
	}
}
