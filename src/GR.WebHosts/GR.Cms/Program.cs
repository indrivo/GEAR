using GR.Application;

namespace GR.Cms
{
	public static class Program
	{
		/// <summary>
		/// Main
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			GearApplication.Run<Startup>(args);
		}
	}
}