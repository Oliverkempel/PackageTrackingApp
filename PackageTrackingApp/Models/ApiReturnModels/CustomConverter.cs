using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Globalization;

namespace PackageTrackingApp.Models.ApiReturnModels
{
    public class CustomConverter
    {
        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }

    }

    public static class Serialize
    {
        public static string ToJson(this object self) => JsonConvert.SerializeObject(self, CustomConverter.Converter.Settings);
    }
}

