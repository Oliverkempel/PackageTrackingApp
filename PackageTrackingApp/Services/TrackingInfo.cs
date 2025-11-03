using PackageTrackingApp.Models;
using Newtonsoft.Json;
using RestSharp;
using PackageTrackingApp.Models.ApiReturnModels;
using PackageTrackingApp.Models.ShipmentSubModels;

namespace PackageTrackingApp.Services
{
    //interface til alle metoderne i klassen.
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
            //initialisere liste af Shipments
            List<Shipment> shipmentsList = new List<Shipment>();

            //looper igennem alle postnordMailInfos i allMailinfos fra parametrerne i metoden
            foreach(var postnordMailInfo in allMailInfo.postNordMailInfos)
            {
                //initialisere ny postnordreturn til opbevaring af data fra api kald.
                PostnordReturnWrapper.PostNordReturn shipmentData = new PostnordReturnWrapper.PostNordReturn();

                // henter postnord tracking info via metoden med den nuværende iteration i loopets trackingnummer
                shipmentData = getPostnordTrackingInfo(postnordMailInfo.trackingNumber);

                // tjekker om lengden ad shipments er større end eller lig en
                if(shipmentData.TrackingInformationResponse.Shipments.Length >= 1)
                {
                    // initialisere ny liste af SmipmentEvents
                    List<ShipmentEvent> Events = new List<ShipmentEvent>();

                    // Looper igennem events i sporingsinformationerne
                    foreach (var ev in shipmentData.TrackingInformationResponse.Shipments.First().Items.First().Events)
                    {
                        //tilføjer nyt shipmentevent til eventlisten
                        Events.Add(new ShipmentEvent
                        {
                            //tilføjer informationen omrking eventet
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

                    //tjekker om vægten af smipmenttet er null
                    if (shipmentData.TrackingInformationResponse.Shipments.First().TotalWeight == null)
                    {
                        //tilføjer shipment til listen
                        shipmentsList.Add(new Shipment
                        {
                            //tilføjer informationen til shipment
                            currentStatus = shipmentData.TrackingInformationResponse.Shipments.First().Status,
                            //events bliver tilføjet til shipment
                            events = Events,
                            //informationer vedrørende shipment bliver tilføjet
                            info = new ShipmentInfo
                            {
                                //weight sættes til strengen none da der ingen vægt er angivet
                                weight = "none",
                                //tracking nummer sættes
                                trackingNumber = shipmentData.TrackingInformationResponse.Shipments.First().ShipmentId,
                                //fragtfirma sættes
                                courrier = postnordMailInfo.Courier,
                                //servicen sættes
                                service = shipmentData.TrackingInformationResponse.Shipments.First().Service.Name,
                                //Afsender sættes
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
                                //modtager sættes
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
                    //Hvis vægt ikke er null
                    else
                    {
                        //tilføjer shipment til listen
                        shipmentsList.Add(new Shipment
                        {
                            //tilføjer informationen til shipment
                            currentStatus = shipmentData.TrackingInformationResponse.Shipments.First().Status,
                            //events bliver tilføjet til shipment
                            events = Events,
                            //informationer vedrørende shipment bliver tilføjet
                            info = new ShipmentInfo
                            {
                                //weight sættes til strengen none da der ingen vægt er angivet
                                weight = shipmentData.TrackingInformationResponse.Shipments.First().TotalWeight.Value,
                                //tracking nummer sættes
                                trackingNumber = shipmentData.TrackingInformationResponse.Shipments.First().ShipmentId,
                                //fragtfirma sættes
                                courrier = postnordMailInfo.Courier,
                                //serice sættes
                                service = shipmentData.TrackingInformationResponse.Shipments.First().Service.Name,
                                //Afsender sættes
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
                                //Modtager sættes
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

                
            }

            ////lopper igennem alle glsMailInfo i allMailInfos.GlsMailInfo
            //foreach(var glsMailInfo in allMailInfo.glsMailInfos)
            //{
            //    //opretter instans af Glsreturn til at indeholde data sendt fra api
            //    GlsReturnContainer.GlsReturn shipmentData = new GlsReturnContainer.GlsReturn();

            //    // henter tracking information på nuværende iteration af loopet
            //    shipmentData = getGlsTrackingInfo(glsMailInfo.trackingNumber);

            //    //tjekker at status af shipment returneret af api ikke er "ERRORNOTFOUND404"
            //    if (shipmentData.TuStatus.FirstOrDefault().TuNo != "ERRORNOTFOUND404")
            //    {
            //        //Intitalizere instans af ShipmentEvent
            //        List<ShipmentEvent> Events = new List<ShipmentEvent>();

            //        //looper igennem events i shipmentData
            //        foreach (var ev in shipmentData.TuStatus.First().History)
            //        {
            //            //tilføjer en shipmentevent til events listen
            //            Events.Add(new ShipmentEvent
            //            {
            //                //data tildeles
            //                dateTime = ev.Date.UtcDateTime,
            //                description = ev.EvtDscr,
            //                location = new Address
            //                {
            //                    city = ev.Address.City,
            //                    country = ev.Address.CountryName,
            //                    //de informationer som ikke findes i deres tracking info bliver tildelt som not provided
            //                    zipCode = "zip Not provided",
            //                },
            //                status = "status Not Provided",
            //            });
            //        }

            //        //tilføjer ny instans af Shipment til shipmentsList listen
            //        shipmentsList.Add(new Shipment
            //        {
            //            //data tildeles
            //            currentStatus = shipmentData.TuStatus.First().ProgressBar.StatusInfo,
            //            events = Events,
            //            info = new ShipmentInfo
            //            {
            //                weight = shipmentData.TuStatus.First().Infos.Where(x => x.Type == "WEIGHT").First().Value,
            //                trackingNumber = shipmentData.TuStatus.First().TuNo,
            //                courrier = glsMailInfo.Courier,
            //                service = shipmentData.TuStatus.First().Infos.Where(x => x.Type == "SERVICES").First().Value,
            //                consignor = new Person
            //                {
            //                    // data som ikke findes i fratfirmaets sporingdata tildeles strengen med not provided
            //                    name = "Name Not provided",
            //                    address = new Address
            //                    {
            //                        city = "City not provided",
            //                        country = "Country not provided",
            //                        zipCode = "Zipcode not provided",

            //                    },
            //                },
            //                consignee = new Person
            //                {
            //                    name = "No name (You)",
            //                    address = new Address
            //                    {
            //                        city = "City not provided",
            //                        country = "Country not provided",
            //                        zipCode = "Zipcode not provided",

            //                    },
            //                }

            //            }

            //        });
            //    } 

            //}

            //returnere shipmentsList
            return shipmentsList;
        }


        public PostnordReturnWrapper.PostNordReturn getPostnordTrackingInfo(string trackingNumber)
        {
            //opretter options til https kald, med url som parameter
            RestClientOptions options = new RestClientOptions("https://api2.postnord.com")
            {
                //sætter timeout til -1, så der ikke er nogen timeout
                MaxTimeout = -1,
            };
            //opretter ny restclient med options som parameter
            RestClient client = new RestClient(options);
            // initialisere ny instans ad restrequest og giver endpoint og get metode som parametre
            RestRequest request = new RestRequest("/rest/shipment/v5/trackandtrace/findByIdentifier.json", Method.Get);

            // tilføjer parametrer til https kald, heriblandt apikey, tracking id og locale
            request.AddQueryParameter("apikey", "bfe34e5cb08e6c7de83a82ef1c2e29aa");
            request.AddQueryParameter("id", trackingNumber);
            request.AddQueryParameter("locale", "da");

            // initialisere ny instans af response og køre kaldet med client.Get(request)
            RestResponse response = client.Get(request);

            //initialisere ny instans af PostNordReturn til opbevaring af data fra api kald
            PostnordReturnWrapper.PostNordReturn data = new PostnordReturnWrapper.PostNordReturn();
            //konvertere json svar fra api til PostNordReturn type og gemmer i data variablen.
            data = JsonConvert.DeserializeObject<PostnordReturnWrapper.PostNordReturn>(response.Content);

            //returnere data
            return data;
        }

        public GlsReturnContainer.GlsReturn getGlsTrackingInfo(string trackingNumber)
        {
            //opretter options til https kald, med url som parameter
            var options = new RestClientOptions("https://gls-group.com")
            {
                //sætter timeout til -1, så der ikke er nogen timeout
                MaxTimeout = -1,
            };

            //opretter ny restclient med options som parameter
            var client = new RestClient(options);
            // initialisere ny instans ad restrequest og giver endpoint og get metode som parametre
            var request = new RestRequest("/app/service/open/rest/DK/da/rstt001", Method.Get);

            // tilføjer trackingnumber parameter til kald
            request.AddQueryParameter("match", trackingNumber);

            // initialisere ny instans af response og køre kaldet med client.Get(request)
            RestResponse response = client.Get(request);

            //initialisere ny instans af GlsReturn til opbevaring af data fra api kald
            GlsReturnContainer.GlsReturn data = new GlsReturnContainer.GlsReturn();

            //konvertere json svar fra api til GlsReturn type og gemmer i data variablen.
            data = JsonConvert.DeserializeObject<GlsReturnContainer.GlsReturn>(response.Content);

            //tjekker om http kaldet returnere med statuskode 404 not found
            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                //opretter ny instans af TuStatus arrayet
                GlsReturnContainer.TuStatus[] notFoundCatch = new GlsReturnContainer.TuStatus[1];
                //giver plads 0 i arrayet ny instans af TuStaus 
                notFoundCatch[0] = new GlsReturnContainer.TuStatus 
                { 
                    // sætter TuNo lig med "ERRONOTFOUND404"
                    TuNo = "ERRORNOTFOUND404"
                };
                //returnere GlsReturn med tustaus lig notFoundCatch
                return new GlsReturnContainer.GlsReturn
                {
                    TuStatus = notFoundCatch,
                };
            } else
            {
                //Returnere data
                return data;
            }
        }

    }
}
