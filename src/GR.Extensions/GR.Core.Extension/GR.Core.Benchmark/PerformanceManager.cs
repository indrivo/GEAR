using System;
using System.Collections.Generic;
using System.Diagnostics;
using GR.Core.Benchmark.Models.Performance;
using GR.Core.Extensions;

namespace GR.Core.Benchmark
{
	public static class PerformanceServer
	{
		public static string GetRamAvailable()
		{
			const string strCommand = "systeminfo |find \"Available Physical Memory\"";
			var ramAvailable = CmdExecute(strCommand);
			if (string.IsNullOrEmpty(ramAvailable)) return ramAvailable;
			try
			{
				ramAvailable = ramAvailable.Split(":")[1].Trim();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return ramAvailable;
		}

		/// <summary>
		/// Get processor count
		/// </summary>
		/// <returns></returns>
		public static int GetProcessorCount()
		{
			return Environment.ProcessorCount;
		}

		/// <summary>
		/// Get OS version
		/// </summary>
		/// <returns></returns>
		public static OperatingSystem GetOsVersion()
		{
			return Environment.OSVersion;
		}

		/// <summary>
		/// Get cpu info
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<CpuInfo> GetCpuInfo()
		{
			var result = new List<CpuInfo>();
			try
			{
				var props = typeof(CpuInfo).GetProperties();
				var obj = new CpuInfo();
				foreach (var prop in props)
				{
					var res = CmdExecute($"wmic cpu get {prop.Name.ToLower()}");
					if (string.IsNullOrEmpty(res)) continue;
					var values = res.Split("\n");
					typeof(CpuInfo).GetProperty(prop.Name)?.SetValue(obj, values[1].Trim());
				}
				result.Add(obj);
			}
			catch
			{
				// ignored
			}

			return result;
		}

		/// <summary>
		/// Get cpu percentage
		/// </summary>
		/// <returns></returns>
		public static double GetCpuUtilization()
		{
			const string strCommand = "wmic cpu get loadpercentage";
			var res = CmdExecute(strCommand);

			if (string.IsNullOrEmpty(res)) return 0;
			try
			{
				var percentage = Convert.ToDouble(res.Split("\n")[1]);
				return percentage;
			}
			catch
			{
				return 0;
			}
		}

		public static string GetAllRamInformation()
		{
			const string strCommand = "wmic MEMORYCHIP get BankLabel, DeviceLocator, MemoryType, TypeDetail, Capacity, Speed";
			var ram = CmdExecute(strCommand);
			return ram;
		}

		/// <summary>
		/// Cmd execute
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public static string CmdExecute(string command)
		{
			try
			{
				var process = new Process()
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = "cmd.exe",
						Arguments = $"/c {command}",
						RedirectStandardOutput = true,
						UseShellExecute = false,
						CreateNoWindow = true
					}
				};
				process.Start();
				var result = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				return result;
			}
			catch
			{
				// ignored
			}

			return null;
		}
	}
}
