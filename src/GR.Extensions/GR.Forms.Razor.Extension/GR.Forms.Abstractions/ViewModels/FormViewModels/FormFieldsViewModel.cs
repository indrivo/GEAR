using GR.Entities.Abstractions.Models.Tables;
using GR.Forms.Abstractions.Models.FormModels;

namespace GR.Forms.Abstractions.ViewModels.FormViewModels
{
    public sealed class FormFieldsViewModel : Form
    {
        public TableModel Table { get; set; }
    }
}
