using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.ArenaImpl;
using Lib.Finder;
using Lib.Models;
using NUnit.Framework.Constraints;

namespace Lib.Intelligence
{
    public class AzuraOracle : IOracle
    {
        public override string ToString()
        {
            return "Azura";
        }

        public IEnumerable<OracleSuggestion> GetSuggestions(Map map)
        {
            var allUnitPositions = OracleServices.GetAllUnitPositions(map);
            
            return OracleServices.GetAllFinalPositions(map).OrderByDescending(z => z.Position.Point.Y); 
        }
    }
}
