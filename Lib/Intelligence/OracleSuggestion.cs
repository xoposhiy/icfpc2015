using Lib.Finder;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Intelligence
{
    public class OracleSuggestion
    {
        public readonly UnitPosition Position;
        public readonly Directions LockingDirection;

        public readonly double Metrics;
        public OracleSuggestion(UnitPosition position, Directions lockingDirection, double metrics = 0)
        {
            Position = position;
            LockingDirection = lockingDirection;
            Metrics = metrics;
        }
        
    }
}
