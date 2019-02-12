using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ST.CORE.Models.InstallerModels
{
    public class DbSetupModel
    {
	    [Required]
		public string DbName { get; set; }

	    [Required]
		public string DbAdrress { get; set; }

	    [Required]
		public string UserName { get; set; }
	//    [Required, DataType(DataType.Password)]
		[Required]
		public string UserPassword{ get; set; }

	    public List<LanguageSetup> Languages { get; set; }
	}

	public class LanguageSetup : AppSettingsModel.Language
	{
		public bool Selected { get; set; }
	}
}
