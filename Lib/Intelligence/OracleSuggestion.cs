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
        public readonly UnitState State;
        public readonly Directions LockingDirection;
        public OracleSuggestion(UnitState state, Directions lockingDirection)
        {
            State = state;
            LockingDirection = lockingDirection;
        }
        
    }
}
