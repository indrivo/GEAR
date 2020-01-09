using System.Collections.Generic;
using System.Linq;
using GR.ECommerce.Razor.ViewModels.ProductsGalleryViewModels;

namespace GR.ECommerce.Razor.Models
{
    public class JsonFiles
    {
        public ViewDataUploadFilesResult[] Files;

        public string TempFolder { get; set; }
        public JsonFiles(List<ViewDataUploadFilesResult> filesList)
        {
            Files = new ViewDataUploadFilesResult[filesList.Count];
            for (var i = 0; i < filesList.Count; i++)
            {
                Files[i] = filesList.ElementAt(i);
            }
        }
    }
}
