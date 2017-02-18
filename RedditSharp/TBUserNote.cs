﻿#pragma warning disable 1591
using System;

namespace RedditSharp
{
    public class TBUserNote
    {
        public int NoteTypeIndex { get; set; }
        public string NoteType { get; set; }
        public string SubName { get; set; }
        public string Submitter { get; set; }
        public int SubmitterIndex { get; set; }
        public string Message { get; set; }
        public string AppliesToUsername { get; set; }
        public string Url { get; set; }
        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                _timestamp = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }
    }
}
#pragma warning restore 1591
