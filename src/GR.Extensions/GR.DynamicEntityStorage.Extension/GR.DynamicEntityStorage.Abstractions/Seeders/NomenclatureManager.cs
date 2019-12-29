using System.Threading.Tasks;

namespace GR.DynamicEntityStorage.Abstractions.Seeders
{
    public static class NomenclatureManager
    {
        /// <summary>
        /// Seed nomenclatures
        /// </summary>
        /// <returns></returns>
        public static Task SyncNomenclaturesAsync()
        {
            //TODO: For seed custom nomenclatures in dynamic entities uncomment code
            //var service = IoC.Resolve<IDynamicService>();
            //var nomenclatureTable = service.Table(Settings.Tables.CustomTable);
            //var req = await nomenclatureTable.Any();
            //if (!req.Result) await nomenclatureTable.AddRange(DynamicJsonReader.GetEntityDataFromJsonFile(nomenclatureTable.Type, "filepath/name.json"));
            return Task.CompletedTask;
        }
    }
}
