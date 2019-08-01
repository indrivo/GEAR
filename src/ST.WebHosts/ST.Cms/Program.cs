using ST.Application;

namespace ST.Cms
{
	public static class Program
	{
		/// <summary>
		/// Main
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			CoreApp.Run<Startup>(args);
		}
	}
}