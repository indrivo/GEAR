using System.Diagnostics;
using GR.WebApplication;
using GR.WebApplication.Models;

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
			var options = new GearApplicationArgs
			{
				UseKestrel = !Debugger.IsAttached
			};

			GearWebApplication.Run<Startup>(args, options);
		}
	}
}