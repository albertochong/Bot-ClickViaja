using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using ClickViajaBot.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using ClickViajaBot.Dialogs;
using System.Text;

namespace ClickViajaBot.Services
{
    public class AmadeuService
    {
        private static readonly string client_id                    = ConfigurationManager.AppSettings["AmadeusClient_id"].ToString();
        private static readonly string client_secret                = ConfigurationManager.AppSettings["AmadeusClient_secret"].ToString();
        private static readonly string urlServiceToken              = ConfigurationManager.AppSettings["AmadeusUrlServiceToken"].ToString();
        private static readonly string urlServiceAirport            = ConfigurationManager.AppSettings["AmadeusUrlServiceAirport"].ToString();
        private static readonly string urlServiceSearchFlights      = ConfigurationManager.AppSettings["AmadeusUrlServiceSearchFligths"].ToString();
        private static readonly string urlServiceSearchCheapestDate = ConfigurationManager.AppSettings["AmadeusUrlServiceSearchCheapestDate"].ToString();
        private static readonly int Amadeus_Num_voo_Mostrar         = int.Parse(ConfigurationManager.AppSettings["Amadeus_Num_voo_Mostrar"].ToString());

        ///// <summary>
        ///// Método que invoca o servico da amadeus para pesquisa de voos
        ///// </summary>
        ///// <param name="pesquisaVoo"></param>
        ///// <returns></returns>
        public static async Task<RootObjectDataSearchFligths> PesquisaVoo(PesquisaVooDialog pesquisaVoo, string token)
        {
            RootObjectDataSearchFligths searchResults = null;

            ///if token is not null or string not empty
            if (token != "")
            {
                
                //call web api to search:

                StringBuilder search = new StringBuilder();
                search.Append(urlServiceSearchFlights).Append("?origin=" + pesquisaVoo.OriginCity).Append("&destination=" + pesquisaVoo.DestinationCity)
                      .Append("&departureDate=" + pesquisaVoo.DepartDate);

                if (pesquisaVoo.ReturnDate != "")
                    search.Append("&returnDate=" + pesquisaVoo.ReturnDate);

                if (pesquisaVoo.NumeroAdultosRequired != 0)
                    search.Append("&adults=" + (int)pesquisaVoo.NumeroAdultosRequired);

                //if (pesquisaVoo.NumeroCriancaRequired != 0)
                //    search.Append("&children=" + pesquisaVoo.NumeroCriancaRequired);

                //if (pesquisaVoo.NumeroBebeRequired != 0)
                //    search.Append("&infants=" + pesquisaVoo.NumeroBebeRequired);

                //if (pesquisaVoo.NumeroIdosoRequired != 0)
                //    search.Append("&seniors=" + pesquisaVoo.NumeroIdosoRequired);

                //travelClass

                //nonStop

                search.Append("&max=" + Amadeus_Num_voo_Mostrar);

                using (var client = new WebClient())
                {
                    client.Headers.Add("Content-Type:application/json");
                    client.Headers.Add("Accept:application/vnd.amadeus+json");
                    client.Headers.Add("Authorization", "Bearer " + token);
                    var result = client.DownloadString(search.ToString());

                    searchResults = JsonConvert.DeserializeObject<RootObjectDataSearchFligths>(result);
                }
            }

            return searchResults;
        }

        public static async Task<DataSearchCheapFligths> PesquisaDataVooBarato(string originCity, string destinationCity, string token, bool vooDirecto)
        {
            DataSearchCheapFligths searchResults = null;

            if (token != "")
            {
                StringBuilder search = new StringBuilder();
                search.Append(urlServiceSearchCheapestDate).Append("?origin=" + originCity).Append("&destination=" + destinationCity);

                search.Append("&max=" + Amadeus_Num_voo_Mostrar);

                using (var client = new WebClient())
                {
                    client.Headers.Add("Content-Type:application/json");
                    client.Headers.Add("Accept:application/vnd.amadeus+json");
                    client.Headers.Add("Authorization", "Bearer " + token);
                    var result = client.DownloadString(search.ToString());

                    searchResults = JsonConvert.DeserializeObject<DataSearchCheapFligths>(result);
                }
            }

            return searchResults;
        }

        /// <summary>
        /// Get Aiport origin and depart iata code
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetAirportIataCodeAsync(string cityName, string token)
        {
            string airportIataCode = String.Empty;
 
            ///if token is not null or string not empty
            if (token != "")
            {
                //call web api to get airpot iata code:

                using (var client = new WebClient())
                {
                    client.Headers.Add("Content-Type:application/json");
                    client.Headers.Add("Accept:application/json");
                    client.Headers.Add("Authorization", "Bearer " + token);
                    var result = client.DownloadString(urlServiceAirport + "?subType=AIRPORT,CITY&keyword=" + cityName + "&page[limit]=1&view=LIGHT");

                    var data = JsonConvert.DeserializeObject<RootObject>(result).data;

                    if (data.Count != 0)
                    {
                        airportIataCode = data[0].iataCode;
                    }
                 }
            }

            return airportIataCode;
        }

        /// <summary>
        /// Get amadeus Token to search data
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetAmadeusAcessToken()
        {
            var dict = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", client_id },
                { "client_secret", client_secret }
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.PostAsync(urlServiceToken, new FormUrlEncodedContent(dict)).Result;
                return JsonConvert.DeserializeObject<AmadeusAuthorization>(response.Content.ReadAsStringAsync().Result).access_token;
            }
        }
    }
}