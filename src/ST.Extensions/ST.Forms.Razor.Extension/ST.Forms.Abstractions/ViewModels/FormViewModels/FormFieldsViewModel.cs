using ST.Entities.Abstractions.Models.Tables;
using ST.Forms.Abstractions.Models.FormModels;

namespace ST.Forms.Abstractions.ViewModels.FormViewModels
{
    public sealed class FormFieldsViewModel : Form
    {
        public TableModel Table { get; set; }
    }
}
