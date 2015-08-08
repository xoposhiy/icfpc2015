using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Lib
{
	[TestFixture]
	public class SubmissionHelpers_Test
	{
		[Test, Explicit]
		public void GenerateSolveScript()
		{
			var problremArgs = string.Join(" ", Directory.GetFiles(@"problems", "problem*.json")
			                          .Select(f => string.Format("-f {0}", f)));
			var solveAllCmd = string.Format(@"@echo off
cd bin
play_icfp2015.exe {0}", problremArgs);
			Console.Out.WriteLine(solveAllCmd);
			File.WriteAllText("..\\..\\..\\solve-all.cmd", solveAllCmd);
		}
	}
}