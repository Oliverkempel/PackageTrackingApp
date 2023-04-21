using PackageTrackingApp.Models.ShipmentSubModels;

namespace PackageTrackingApp.Models
{
    public class Shipment
    {
        public string currentStatus { get; set; }

        public List<ShipmentEvent> events { get; set; }

        public ShipmentInfo? info { get; set; }


    }
}
