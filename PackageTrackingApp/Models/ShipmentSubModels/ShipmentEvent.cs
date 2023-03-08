namespace PackageTrackingApp.Models.ShipmentSubModels
{
    public class ShipmentEvent
    {
        public string description { get; set; }

        public DateTime dateTime { get; set; }

        public Location location { get; set; }

        public string status { get; set; }
    }
}
