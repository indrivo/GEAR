namespace GR.Entities.Abstractions.ViewModels.BaseModel
{
   public  class BaseModelVieModel
    {
	    /// <summary>
	    /// Max length for string types
	    /// </summary>
	    public int MaxLenght { get; set; }

	    /// <summary>
	    /// Name
	    /// </summary>
	    public string Name { get; set; }

	    /// <summary>
	    /// AllowNull
	    /// </summary>
	    public bool AllowNull { get; set; }

	    /// <summary>
	    /// DataType of field
	    /// </summary>
	    public string DataType { get; set; }
	}
}
