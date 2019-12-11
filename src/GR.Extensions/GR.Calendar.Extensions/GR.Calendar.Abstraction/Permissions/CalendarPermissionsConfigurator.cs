using System.Threading.Tasks;
using GR.Identity.Permissions.Abstractions.Configurators;

namespace GR.Calendar.Abstractions.Permissions
{
    public class CalendarPermissionsConfigurator : DefaultPermissionsConfigurator<CalendarPermissionsConstants>
    {
        public override Task SeedAsync()
        {
            OnPermissionsSeedComplete += (sender, args) =>
            {

            };
            return Task.CompletedTask;
        }
    }
}
