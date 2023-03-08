using PackageTrackingApp.Models;
using Newtonsoft.Json;
using RestSharp;
using PackageTrackingApp.Models.ApiReturnModels;

namespace PackageTrackingApp.Services
{

    public interface ITrackingInfo
    {
        PostnordReturnWrapper.PostNordReturn getPostnordTrackingInfo(string trackingNumber);
        void getGlsTrackingInfo(string trackingNumber);
    }
    public class TrackingInfo : ITrackingInfo
    {
        public void getTrackingInfoAllCourriers()
        {

        }


        public PostnordReturnWrapper.PostNordReturn getPostnordTrackingInfo(string trackingNumber)
        {
            var options = new RestClientOptions("https://api2.postnord.com")
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("/rest/shipment/v5/trackandtrace/findByIdentifier.json", Method.Get);
            request.AddQueryParameter("apikey", "aa5bab080e542f8f20d09e27a48321e0");
            request.AddQueryParameter("id", trackingNumber);
            request.AddQueryParameter("locale", "da");

            RestResponse response = client.Get(request);

            PostnordReturnWrapper.PostNordReturn data = new PostnordReturnWrapper.PostNordReturn();
            data = JsonConvert.DeserializeObject<PostnordReturnWrapper.PostNordReturn>(response.Content);

            return data;
        }

        public void getGlsTrackingInfo(string trackingNumber)
        {

        }

    }
}
