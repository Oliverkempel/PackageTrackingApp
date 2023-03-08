namespace PackageTrackingApp.Models.ShipmentSubModels
{
    public class ShipmentInfo
    {
        public int weight { get; set; }

        public string service { get; set; }

        public string courrier { get; set; }

        public Person consignee { get; set; }

        public Person consignor { get; set; }
    }
}
