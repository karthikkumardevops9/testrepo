using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class LinkScript
    {
        public string ScriptName { get; set; }
        public double ScriptSequence { get; set; }
        public string Command { get; set; }
        public string ArgOne { get; set; }
        public string ArgTwo { get; set; }
        public string ArgThree { get; set; }
        public string Result { get; set; }
        public string Comment { get; set; }
    }
}
