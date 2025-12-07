namespace TransTrack.Common.Models
{
    public class ShipmentRecord
    {
        public string ShipmentId { get; set; }
        public string Origin { get; set; }        // For North
        public string Region { get; set; }        // For South
        public string Destination { get; set; }
        public string Date { get; set; }
        public string Weight { get; set; }        // For North
        public string LoadType { get; set; }      // For South
    }
}
