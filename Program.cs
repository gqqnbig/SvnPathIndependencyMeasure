using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SvnPathIndependencyMeasure
{
	class Program
	{
		static void Main(string[] args)
		{
			var targetFolder = args[args.Length - 1];
			var svnPath = GetSvnPath(targetFolder);
			
			var stream = GetFolderLog(args, targetFolder);

			int selfContainedCommits = 0;
			int dependentCommits = 0;
			var reportProgressInterval = GetOptionalNamedOptionArgument(args, "--progress", "-p", 10);

			DateTime startRunning = DateTime.Now;
			int total;


			var excludePattern = GetNamedOptionArgument(args, "-e", "--exclude");
			Regex excludeReg = null;
			if (excludePattern != null)
				excludeReg = new Regex(excludePattern);

			using (XmlReader reader = XmlReader.Create(stream))
			{

				int reportCount = -1;
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element && reader.Name == "logentry")
					{
						var element = XElement.ReadFrom(reader);

						var paths = from path in element.XPathSelectElements("paths/path")
									select path.Value;

						if (excludeReg != null)
							paths = paths.Where(p => excludeReg.IsMatch(p) == false);

						if (paths.All(p => p.StartsWith(svnPath)))
							selfContainedCommits++;
						else
							dependentCommits++;

						if (reportProgressInterval.HasValue && (int)(DateTime.Now - startRunning).TotalSeconds > reportProgressInterval.Value * reportCount)
						{
							reportCount++;
							total = selfContainedCommits + dependentCommits;
							Console.WriteLine($"Self-contained Commits: {selfContainedCommits}; Dependent Commits: {dependentCommits}; Ratio: {selfContainedCommits / (double)total:f2} ...");
						}
					}

				}
			}

			total = selfContainedCommits + dependentCommits;
			Console.WriteLine($"Self-contained Commits: {selfContainedCommits}; Dependent Commits: {dependentCommits}; Ratio: {selfContainedCommits / (double)total:f2}");
			if (reportProgressInterval.HasValue)
				Console.WriteLine("done");

		}

		private static Stream GetFolderLog(string[] args, string targetFolder)
		{
			var startInfo = new ProcessStartInfo
			{
				RedirectStandardOutput = true,
				WorkingDirectory = targetFolder,
				FileName = "svn",
				Arguments = "log -v --xml",
				UseShellExecute = false
			};

			var arg = GetNamedOptionArgument(args, "--range", "-r");
			if (arg != null)
			{
				string today = DateTime.Now.ToString("yyyy-MM-dd");
				string startDate = DateTime.Now.AddYears(-Convert.ToInt32(arg)).ToString("yyyy-MM-dd");

				startInfo.Arguments += $" -r{{{startDate}}}:{{{today}}}";
			}
			arg = GetNamedOptionArgument(args, "--limit", "-l");
			if (arg != null)
				startInfo.Arguments += $" -l {arg}";

			var process = Process.Start(startInfo);
			return process.StandardOutput.BaseStream;
		}

		private static string GetNamedOptionArgument(string[] args, string option, string shortOption = null)
		{
			int p = Array.IndexOf(args, option);
			if (p == -1)
			{
				if (shortOption != null)
					return GetNamedOptionArgument(args, shortOption);
				else
					return null;
			}
			else
			{
				if (p + 1 == args.Length)
					throw new ArgumentException($"Argument of option {option} is missing.");
				else
					return args[p + 1];
			}
		}

		private static T? GetOptionalNamedOptionArgument<T>(string[] args, string option, T defaultValue) where T : struct
		{
			int p = Array.IndexOf(args, option);
			if (p == -1)
			{
				return null;
			}
			else
			{
				if (p + 1 == args.Length)
					return defaultValue;
				else if (args[p + 1].StartsWith("-"))
					return defaultValue;
				else
					return (T)Convert.ChangeType(args[p + 1], typeof(T));
			}
		}

		private static T? GetOptionalNamedOptionArgument<T>(string[] args, string option, string shortOption, T defaultValue) where T : struct
		{
			int p = Array.IndexOf(args, option);
			if (p == -1)
			{
				if (shortOption != null)
					return GetOptionalNamedOptionArgument(args, shortOption, defaultValue);
				else
					return null;
			}
			else
			{
				if (p + 1 == args.Length)
					return defaultValue;
				else if (args[p + 1].StartsWith("-") && args[p + 1].StartsWith("--") == false)
					return defaultValue;
				else
					return (T)Convert.ChangeType(args[p + 1], typeof(T));
			}
		}


		private static bool HasBooleanOption(string[] args, string option, string shortOption = null)
		{
			return args.Contains(option) || (shortOption != null && args.Contains(shortOption));
		}

		private static string GetSvnPath(string targetFolder)
		{
			var startInfo = new ProcessStartInfo
			{
				RedirectStandardOutput = true,
				WorkingDirectory = targetFolder,
				FileName = "svn",
				Arguments = "info",
				UseShellExecute = false
			};


			var p = Process.Start(startInfo);
			var output = p.StandardOutput.ReadToEnd();
			Regex reg = new Regex(@"(?<=Relative URL: \^).+");
			var m = reg.Match(output);
			return m.Value.Trim();
		}
	}
}
