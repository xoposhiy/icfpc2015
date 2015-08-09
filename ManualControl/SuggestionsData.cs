using Lib.Intelligence;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualControl
{
    public class SuggestionsModel
    {
        public List<OracleSuggestion> Suggestions
        {
            get; private set;
        }

        public List<Point> OraclePresense = new List<Point>();

        public int Position
        {
            get; private set;
        }

        public Unit Unit
        {
            get; private set;
        }

        public void Load(IEnumerable<OracleSuggestion> suggestions, Unit unit)
        {
            Position = 0;
            Unit = unit;
            Suggestions = suggestions.ToList();
            OraclePresense = Suggestions
                .Take(200)
                .Select(z => new PositionedUnit(unit, z.Position))
                .SelectMany(z => z.Members)
                .Distinct()
                .ToList();

            OnUpdated();
        }

        public void Clear()
        {
            Suggestions = null;
            Unit = null;
            OraclePresense = new List<Point>();

            OnUpdated();
        }

        public bool Next()
        {
            Position++;
            OnUpdated();
            return Position < Suggestions.Count;
        }


        public OracleSuggestion GetCurrentSuggestion()
        {
            if (Suggestions == null) return null;
            if (Position >= Suggestions.Count) return null;
            return Suggestions[Position];
        }

        public event Action Updated;

        void OnUpdated()
        {
            if (Updated != null) Updated();
        }
    }
}
