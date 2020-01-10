using GR.UI.Menu.Abstractions.Models;

namespace GR.UI.Menu.Abstractions.ViewModels
{
    public class MenuViewModel : MenuItem
    {
        public MenuViewModel[] Children { get; set; }
    }
}
