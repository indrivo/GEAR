using GR.WebApplication;

namespace GR.Cms
{
	public static class Program
	{
		/// <summary>
		/// Main
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args) => GearWebApplication.Run<Startup>(args);
	}
}