using System;

namespace TransTrack.Common.Models
{
    public class ProcessingSummary
    {
        public string FileName { get; set; }
        public int TotalRecords { get; set; }
        public string FileUrl { get; set; }
        public DateTime DateProcessed { get; set; }
        public bool IsValid { get; set; }
        public string ErrorReason { get; set; }
    }
}
