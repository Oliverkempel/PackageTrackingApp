using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PackageTrackingApp.Models.ApiReturnModels
{
    public class GlsReturnContainer : CustomConverter
    {
        public partial class GlsReturn
        {
            [JsonProperty("tuStatus")]
            public TuStatus[] TuStatus { get; set; }
        }

        public partial class TuStatus
        {
            [JsonProperty("emailNotificationCard")]
            public bool EmailNotificationCard { get; set; }

            [JsonProperty("references")]
            public Info[] References { get; set; }

            [JsonProperty("tuNo")]
            public string TuNo { get; set; }

            [JsonProperty("history")]
            public History[] History { get; set; }

            [JsonProperty("signature")]
            public Signature Signature { get; set; }

            [JsonProperty("progressBar")]
            public ProgressBar ProgressBar { get; set; }

            [JsonProperty("owners")]
            public Owner[] Owners { get; set; }

            [JsonProperty("infos")]
            public Info[] Infos { get; set; }

            [JsonProperty("deliveryOwnerCode")]
            public string DeliveryOwnerCode { get; set; }

            [JsonProperty("arrivalTime")]
            public ArrivalTime ArrivalTime { get; set; }

            [JsonProperty("changeDeliveryPossible")]
            public bool ChangeDeliveryPossible { get; set; }
        }

        public partial class ArrivalTime
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public partial class History
        {
            [JsonProperty("time")]
            public DateTimeOffset Time { get; set; }

            [JsonProperty("date")]
            public DateTimeOffset Date { get; set; }

            [JsonProperty("address")]
            public Address Address { get; set; }

            [JsonProperty("evtDscr")]
            public string EvtDscr { get; set; }
        }

        public partial class Address
        {
            [JsonProperty("countryName")]
            public string CountryName { get; set; }

            [JsonProperty("countryCode")]
            public string CountryCode { get; set; }

            [JsonProperty("city")]
            public string City { get; set; }
        }

        public partial class Info
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public partial class Owner
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }
        }

        public partial class ProgressBar
        {
            [JsonProperty("level")]
            public long Level { get; set; }

            [JsonProperty("retourFlag")]
            public bool RetourFlag { get; set; }

            [JsonProperty("statusBar")]
            public StatusBar[] StatusBar { get; set; }

            [JsonProperty("statusText")]
            public string StatusText { get; set; }

            [JsonProperty("statusInfo")]
            public string StatusInfo { get; set; }

            [JsonProperty("evtNos")]
            public string[] EvtNos { get; set; }

            [JsonProperty("colourIndex")]
            public long ColourIndex { get; set; }
        }

        public partial class StatusBar
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("statusText")]
            public string StatusText { get; set; }

            [JsonProperty("imageStatus")]
            public string ImageStatus { get; set; }

            [JsonProperty("imageText")]
            public string ImageText { get; set; }
        }

        public partial class Signature
        {
            [JsonProperty("validate")]
            public bool Validate { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            [JsonConverter(typeof(ParseStringConverter))]
            public bool Value { get; set; }
        }

        public partial class GlsReturn
        {
            public static GlsReturn FromJson(string json) => JsonConvert.DeserializeObject<GlsReturn>(json, CustomConverter.Converter.Settings);
        }

        internal class ParseStringConverter : JsonConverter
        {

            public override bool CanConvert(Type t) => t == typeof(bool) || t == typeof(bool?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                bool b;
                if (Boolean.TryParse(value, out b))
                {
                    return b;
                }
                throw new Exception("Cannot unmarshal type bool");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (bool)untypedValue;
                var boolString = value ? "true" : "false";
                serializer.Serialize(writer, boolString);
                return;
            }

            public static readonly ParseStringConverter Singleton = new ParseStringConverter();
        }

    }
}
