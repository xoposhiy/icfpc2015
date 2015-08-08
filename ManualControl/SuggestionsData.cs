using Lib.Intelligence;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualControl
{
    public class SuggestionsModel
    {
        public List<OracleSuggestion> Suggestions = new List<OracleSuggestion>();
        public int Position;
        public Unit Unit;

        public OracleSuggestion GetCurrentSuggestion()
        {
            if (Suggestions == null) return null;
            if (Position >= Suggestions.Count) return null;
            return Suggestions[Position];
        }

    }
}
