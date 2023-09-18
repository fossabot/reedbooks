﻿using ReedBooks.Core;
using ReedBooks.Models.Assessment;
using System;
using System.Collections.Generic;

namespace ReedBooks.Models.Diary
{
    public class Diary : ObservableObject
    {
        public DateTime BeginReadingAt { get; set; }
        public DateTime EndReadingAt { get; set; }
        public EmotionalAssessment EmotionalAssessment { get; set; }
        public BookAssessment BookAssessment { get; set; }
        public string PlotBriefRetelling { get; set; }
        public List<Quote> Quotes { get; set; }
    }
}
