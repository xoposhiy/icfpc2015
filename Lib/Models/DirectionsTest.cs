using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;

namespace Lib.Models
{
    [TestFixture]
    public class DirectionsTest
    {
        [Test, UseReporter(typeof(DiffReporter))]
        public void Test()
        {
            var directions = Phrases.all.Select(w => w + " → " + string.Join(" ", w.ToDirections().Select(d => d.ToString())));
            Console.WriteLine(directions);
            Approvals.VerifyAll(directions, "d");
        }
    }
}