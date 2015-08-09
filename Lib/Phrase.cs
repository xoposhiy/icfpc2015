using System;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using ApprovalTests;
using Lib.Models;
using NUnit.Framework;

namespace Lib
{
    public class Phrase
    {
        public static implicit operator Phrase(string original)
        {
            return new Phrase(original);
        }

        public Phrase(string original)
        {
            Original = original;
            Dirs = Original.ToDirections().ToArray();
            Canonical = Dirs.ToPhrase();
            Len = original.Length;
            shift = new SizeF(Dirs.Count(d => d == Directions.E) - Dirs.Count(d => d == Directions.W), 0);
            var se = Dirs.Count(d => d == Directions.SE);
            var sw = Dirs.Count(d => d == Directions.SW);
            shift += new SizeF((se - sw)/2f, (se + sw)*sqrt32);
            cw = (Dirs.Count(d => d == Directions.CW) - Dirs.Count(d => d == Directions.CCW) + 60000) % 6;
        }

        public PositionedUnit Move(PositionedUnit pos)
        {
            var newPos = (pos.Position.Point.ToGeometry() + shift).ToMap();
            return pos.WithNewPosition(new UnitPosition(newPos, (pos.Position.Angle + cw) % pos.Unit.Period));
        }

        public readonly string Original;
        public readonly string Canonical;
        public readonly int Len;
        public readonly SizeF shift;
        public readonly int cw;
        public readonly Directions[] Dirs;
        private static readonly float sqrt32 = (float)(Math.Sin(Math.PI / 3));
    }

    [TestFixture]
    public class PhraseTest
    {
        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Move()
        {
            Approvals.VerifyAll(Phrases.Words.Select(w => w.shift + " " + w.Original), "phrase");
        }
    }
}