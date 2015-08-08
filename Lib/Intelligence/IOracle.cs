using Lib.Finder;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Intelligence
{
    public interface IOracle
    {
        IEnumerable<OracleSuggestion> GetSuggestions(Map map);
    }
}
