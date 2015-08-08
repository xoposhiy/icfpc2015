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
        public OracleSuggestion(UnitPosition position, Directions lockingDirection)
        {
            Position = position;
            LockingDirection = lockingDirection;
        }
        
    }
}
