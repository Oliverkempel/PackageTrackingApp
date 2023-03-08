using PackageTrackingApp.Models;
using Newtonsoft.Json;
using RestSharp;
using PackageTrackingApp.Models.ApiReturnModels;
using PackageTrackingApp.Models.ShipmentSubModels;

namespace PackageTrackingApp.Services
{

    public interface ITrackingInfo
    {
        List<Shipment> getTrackingInfoAllCourriers(AllMailInfo allMailInfo);
        PostnordReturnWrapper.PostNordReturn getPostnordTrackingInfo(string trackingNumber);
        GlsReturnContainer.GlsReturn getGlsTrackingInfo(string trackingNumber);
    }
    public class TrackingInfo : ITrackingInfo
    {
        public List<Shipment> getTrackingInfoAllCourriers(AllMailInfo allMailInfo)
        {
            List<Shipment> shipmentsList = new List<Shipment>();

            foreach(var postnordMailInfo in allMailInfo.postNordMailInfos)
            {
                PostnordReturnWrapper.PostNordReturn shipmentData = new PostnordReturnWrapper.PostNordReturn();

                shipmentData = getPostnordTrackingInfo(postnordMailInfo.trackingNumber);

                if(shipmentData.TrackingInformationResponse.Shipments.Length >= 1)
                {
                    List<ShipmentEvent> Events = new List<ShipmentEvent>();


                    foreach (var ev in shipmentData.TrackingInformationResponse.Shipments.First().Items.First().Events)
                    {
                        Events.Add(new ShipmentEvent
                        {
                            dateTime = ev.EventTime.UtcDateTime,
                            description = ev.EventDescription,
                            location = new Address
                            {
                                city = ev.Location.City,
                                country = ev.Location.Country,
                                zipCode = ev.Location.Postcode,
                            },
                            status = ev.Status,
                        });
                    }

                    shipmentsList.Add(new Shipment
                    {
                        currentStatus = shipmentData.TrackingInformationResponse.Shipments.First().Status,
                        events = Events,
                        info = new ShipmentInfo
                        {
                            weight = shipmentData.TrackingInformationResponse.Shipments.First().TotalWeight.Value,
                            trackingNumber = shipmentData.TrackingInformationResponse.Shipments.First().ShipmentId,
                            courrier = postnordMailInfo.Courier,
                            service = shipmentData.TrackingInformationResponse.Shipments.First().Service.Name,
                            consignor = new Person
                            {
                                name = shipmentData.TrackingInformationResponse.Shipments.First().Consignor.Name,
                                address = new Address
                                {
                                    city = shipmentData.TrackingInformationResponse.Shipments.First().Consignor.Address.City,
                                    country = shipmentData.TrackingInformationResponse.Shipments.First().Consignor.Address.Country,
                                    zipCode = shipmentData.TrackingInformationResponse.Shipments.First().Consignor.Address.PostCode.ToString(),

                                },
                            },
                            consignee = new Person
                            {
                                name = "No name (You)",
                                address = new Address
                                {
                                    city = shipmentData.TrackingInformationResponse.Shipments.First().Consignee.Address.City,
                                    country = shipmentData.TrackingInformationResponse.Shipments.First().Consignee.Address.Country,
                                    zipCode = shipmentData.TrackingInformationResponse.Shipments.First().Consignee.Address.PostCode.ToString(),

                                },
                            }

                        }

                    });
                }

                
            }

            foreach(var glsMailInfo in allMailInfo.glsMailInfos)
            {
                GlsReturnContainer.GlsReturn shipmentData = new GlsReturnContainer.GlsReturn();

                shipmentData = getGlsTrackingInfo(glsMailInfo.trackingNumber);

                if(shipmentData.TuStatus.FirstOrDefault().TuNo != "ERRORNOTFOUND404")
                {
                    List<ShipmentEvent> Events = new List<ShipmentEvent>();

                    foreach (var ev in shipmentData.TuStatus.First().History)
                    {
                        Events.Add(new ShipmentEvent
                        {
                            dateTime = ev.Date.UtcDateTime,
                            description = ev.EvtDscr,
                            location = new Address
                            {
                                city = ev.Address.City,
                                country = ev.Address.CountryName,
                                zipCode = "zip Not provided",
                            },
                            status = "status Not Provided",
                        });
                    }

                    shipmentsList.Add(new Shipment
                    {
                        currentStatus = shipmentData.TuStatus.First().ProgressBar.StatusInfo,
                        events = Events,
                        info = new ShipmentInfo
                        {
                            weight = shipmentData.TuStatus.First().Infos.Where(x => x.Type == "WEIGHT").First().Value,
                            trackingNumber = shipmentData.TuStatus.First().TuNo,
                            courrier = glsMailInfo.Courier,
                            service = shipmentData.TuStatus.First().Infos.Where(x => x.Type == "SERVICES").First().Value,
                            consignor = new Person
                            {
                                name = "Name Not provided",
                                address = new Address
                                {
                                    city = "City not provided",
                                    country = "Country not provided",
                                    zipCode = "Zipcode not provided",

                                },
                            },
                            consignee = new Person
                            {
                                name = "No name (You)",
                                address = new Address
                                {
                                    city = "City not provided",
                                    country = "Country not provided",
                                    zipCode = "Zipcode not provided",

                                },
                            }

                        }

                    });
                } 

            }

            return shipmentsList;
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

        public GlsReturnContainer.GlsReturn getGlsTrackingInfo(string trackingNumber)
        {
            var options = new RestClientOptions("https://gls-group.com")
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("/app/service/open/rest/DK/da/rstt001", Method.Get);
            request.AddQueryParameter("match", trackingNumber);

            RestResponse response = client.Get(request);

            GlsReturnContainer.GlsReturn data = new GlsReturnContainer.GlsReturn();
            data = JsonConvert.DeserializeObject<GlsReturnContainer.GlsReturn>(response.Content);

            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                GlsReturnContainer.TuStatus[] test = new GlsReturnContainer.TuStatus[1];
                test[0] = new GlsReturnContainer.TuStatus 
                { 
                    TuNo = "ERRORNOTFOUND404"
                };

                return new GlsReturnContainer.GlsReturn
                {
                    TuStatus = test,
                };
            } else
            {
                return data;
            }
        }

    }
}
