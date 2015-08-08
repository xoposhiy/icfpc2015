﻿using Lib.Finder;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManualControl
{
    public class HistoryItem
    {
        public readonly Map Map;
        public readonly string Token;
        public readonly char Char;
        public HistoryItem(Map map, string token, char c)
        {
            Map = map;
            Token = token;
            Char = c;
        }

    }


    public class MainModel
    {
        public History History;

        bool playing;
        public event Action PlayingChanged;

        Timer timer;

        public bool Playing { get; private set; }

        public MainModel()
        {
            timer = new Timer();
            timer.Interval = 10;
            timer.Tick += (s, a) => { History.Forward(); if (History.Ended) Pause(); };
        }

        public void Play()
        {
            timer.Start();
            Playing = true;
            if (PlayingChanged != null) PlayingChanged();
        }

        public void Pause()
        {
            timer.Stop();
            Playing = false;
            if (PlayingChanged != null) PlayingChanged();
        }


        public PositionedUnit Suggestion { get; set; }

    }

    public class History
    {
        List<HistoryItem> allHistory;
        public int CurrentPosition { get; private set; }
        public Map CurrentMap {  get { return allHistory[CurrentPosition].Map; } }

        public IEnumerable<HistoryItem> Items { get { return allHistory; } }

        public bool Ended { get { return CurrentPosition == allHistory.Count - 1; } }

        public void Append(string str, string token)
        {
            if (allHistory.Count > CurrentPosition + 1) allHistory.RemoveRange(CurrentPosition + 1, allHistory.Count - CurrentPosition - 1);
            foreach (var e in str)
            {
                AppendInternal(token, Finder.CharToDirection(e),e);
            }
            if (Updated != null) Updated();
        }

        public void Append(string token, Directions direction, char c)
        {
            if (allHistory.Count > CurrentPosition + 1) allHistory.RemoveRange(CurrentPosition + 1, allHistory.Count - CurrentPosition - 1);
            AppendInternal(token, direction, c);
            if (Updated != null) Updated();
        }

        void AppendInternal(string token, Directions direction, char c)
        {
            var map = allHistory[allHistory.Count-1].Map;
            map = map.Move(direction);
            allHistory.Add(new HistoryItem(map, token, c));
        }



        public event Action Updated;

        public void Forward()
        {
            if (CurrentPosition < allHistory.Count-1) CurrentPosition++;
            if (Updated != null) Updated();
        }

        public void Backward()
        {
            if (CurrentPosition>0) CurrentPosition--;
            if (Updated != null) Updated();
        }

        public History(Map map)
        {
            allHistory = new List<HistoryItem>();
            allHistory.Add(new HistoryItem(map, "", 'x'));
        }
    }
}
