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
			var problremArgs = string.Join(" ", Directory.GetFiles(@"problems", "problem*.json").Select(f => $"-f {f}"));
			var powerWordsArgs = string.Join(" ", Phrases.all.Select(p => $"-p \"{p}\""));
			var solveAllCmd = $@"@echo off
cd bin
play_icfp2015.exe {powerWordsArgs} {problremArgs}";
			Console.Out.WriteLine(solveAllCmd);
			File.WriteAllText("..\\..\\..\\solve-all.cmd", solveAllCmd);
		}
	}
}