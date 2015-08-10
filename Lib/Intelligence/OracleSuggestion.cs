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
        public readonly Map LockedFinalMap;

        public readonly double Metrics;
        public OracleSuggestion(UnitPosition position, Directions lockingDirection, Map lockedFinalMap, double metrics = 0)
        {
            Position = position;
            LockingDirection = lockingDirection;
            this.LockedFinalMap = lockedFinalMap;
            Metrics = metrics;
        }

        public OracleSuggestion WithMetric(double value)
        {
            return new OracleSuggestion(Position, LockingDirection, LockedFinalMap, value);
        }

        public override string ToString()
        {
            return $"{Position}, Metrics: {Metrics}";
        }
    }
}
