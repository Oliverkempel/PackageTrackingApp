using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PackageTrackingApp.Models.ApiReturnModels
{
    public class PostnordReturnWrapper
    {
        public partial class PostNordReturn : CustomConverter
        {
            [JsonProperty("TrackingInformationResponse")]
            public TrackingInformationResponse TrackingInformationResponse { get; set; }
        }

        public partial class TrackingInformationResponse
        {
            [JsonProperty("shipments")]
            public Shipment[] Shipments { get; set; }
        }

        public partial class Shipment
        {
            [JsonProperty("shipmentId")]
            public string ShipmentId { get; set; }

            [JsonProperty("assessedNumberOfItems")]
            public long AssessedNumberOfItems { get; set; }

            [JsonProperty("deliveryDate")]
            public DateTimeOffset DeliveryDate { get; set; }

            [JsonProperty("flexChangePossible")]
            public bool FlexChangePossible { get; set; }

            [JsonProperty("service")]
            public Service Service { get; set; }

            [JsonProperty("consignor")]
            public Consignor Consignor { get; set; }

            [JsonProperty("consignee")]
            public Consignee Consignee { get; set; }

            [JsonProperty("statusText")]
            public StatusText StatusText { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("totalWeight")]
            public AssessedWeight? TotalWeight { get; set; }

            [JsonProperty("assessedWeight")]
            public AssessedWeight? AssessedWeight { get; set; }

            [JsonProperty("items")]
            public Item[] Items { get; set; }

            [JsonProperty("additionalServices")]
            public AdditionalService[] AdditionalServices { get; set; }

            [JsonProperty("splitStatuses")]
            public object[] SplitStatuses { get; set; }

            [JsonProperty("shipmentReferences")]
            public Reference[] ShipmentReferences { get; set; }
        }

        public partial class AdditionalService
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("groupCode")]
            public string GroupCode { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public partial class AssessedWeight
        {
            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("unit")]
            public string Unit { get; set; }
        }

        public partial class Consignee
        {
            [JsonProperty("address")]
            public Address Address { get; set; }
        }

        public partial class Address
        {
            [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
            public string City { get; set; }

            [JsonProperty("countryCode")]
            public string CountryCode { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("postCode")]
            public string PostCode { get; set; }
        }

        public partial class Consignor
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("address")]
            public Address Address { get; set; }
        }

        public partial class Item
        {
            [JsonProperty("itemId")]
            public string ItemId { get; set; }

            [JsonProperty("dropOffDate")]
            public DateTimeOffset DropOffDate { get; set; }

            [JsonProperty("deliveryDate")]
            public DateTimeOffset DeliveryDate { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("statusText")]
            public StatusText StatusText { get; set; }

            [JsonProperty("statedMeasurement")]
            public StatedMeasurement StatedMeasurement { get; set; }

            [JsonProperty("assessedMeasurement")]
            public AssessedMeasurement AssessedMeasurement { get; set; }

            [JsonProperty("events")]
            public Event[] Events { get; set; }

            [JsonProperty("references")]
            public Reference[] References { get; set; }
        }

        public partial class AssessedMeasurement
        {
            [JsonProperty("weight")]
            public AssessedWeight Weight { get; set; }
        }

        public partial class Event
        {
            [JsonProperty("eventTime")]
            public DateTimeOffset EventTime { get; set; }

            [JsonProperty("eventCode")]
            public string EventCode { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("eventDescription")]
            public string EventDescription { get; set; }

            [JsonProperty("location")]
            public Location Location { get; set; }
        }

        public partial class Location
        {
            [JsonProperty("displayName", NullValueHandling = NullValueHandling.Ignore)]
            public string DisplayName { get; set; }

            [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
            public string Name { get; set; }

            [JsonProperty("locationId", NullValueHandling = NullValueHandling.Ignore)]
            public string LocationId { get; set; }

            [JsonProperty("countryCode", NullValueHandling = NullValueHandling.Ignore)]
            public string CountryCode { get; set; }

            [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
            public string Country { get; set; }

            [JsonProperty("locationType", NullValueHandling = NullValueHandling.Ignore)]
            public string LocationType { get; set; }

            [JsonProperty("postcode", NullValueHandling = NullValueHandling.Ignore)]
            public string Postcode { get; set; }

            [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
            public string City { get; set; }
        }

        public partial class Reference
        {
            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public partial class StatedMeasurement
        {
            [JsonProperty("weight")]
            public AssessedWeight Weight { get; set; }

            [JsonProperty("length")]
            public AssessedWeight Length { get; set; }

            [JsonProperty("height")]
            public AssessedWeight Height { get; set; }

            [JsonProperty("width")]
            public AssessedWeight Width { get; set; }
        }

        public partial class StatusText
        {
            [JsonProperty("header")]
            public string Header { get; set; }

            [JsonProperty("body")]
            public string Body { get; set; }
        }

        public partial class Service
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public partial class PostNordReturn
        {
            public static PostNordReturn FromJson(string json) => JsonConvert.DeserializeObject<PostNordReturn>(json, CustomConverter.Converter.Settings);
        }

        internal class ParseStringConverter : JsonConverter
        {
            public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                long l;
                if (Int64.TryParse(value, out l))
                {
                    return l;
                }
                throw new Exception("Cannot unmarshal type long");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (long)untypedValue;
                serializer.Serialize(writer, value.ToString());
                return;
            }

            public static readonly ParseStringConverter Singleton = new ParseStringConverter();
        }

    }
}
