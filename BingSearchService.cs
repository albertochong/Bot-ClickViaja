using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
//using video = Microsoft.Azure.CognitiveServices.Search.VideoSearch;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models;
using Microsoft.Azure.CognitiveServices.Search.VideoSearch;

namespace ClickViajaBot.Services
{
    public static class BingSearchService
    {
        //const string accessKey = "988c2071beae433e8d422f8a52a89e45";
        //const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/images/search";

        //struct SearchResult
        //{
        //    public String jsonResult;
        //    public Dictionary<String, String> relevantHeaders;
        // }


        //static SearchResult BingWebSearch(string searchQuery)
        //{
        //    // Construct the URI of the search request
        //    var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(searchQuery);

        //    // Perform the Web request and get the response
        //    WebRequest request = HttpWebRequest.Create(uriQuery);
        //    request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
        //    string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

        //    // Create result object for return
        //    var searchResult = new SearchResult()
        //    {
        //        jsonResult = json,
        //        relevantHeaders = new Dictionary<String, String>()
        //    };

        //    // Extract Bing HTTP headers
        //    foreach (String header in response.Headers)
        //    {
        //        if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
        //            searchResult.relevantHeaders[header] = response.Headers[header];
        //    }

        //    return searchResult;
        //}

        //static string JsonPrettyPrint(string json)
        //{
        //    if (string.IsNullOrEmpty(json))
        //        return string.Empty;

        //    json = json.Replace(Environment.NewLine, "").Replace("\t", "");

        //    StringBuilder sb = new StringBuilder();
        //    bool quote = false;
        //    bool ignore = false;
        //    char last = ' ';
        //    int offset = 0;
        //    int indentLength = 2;

        //    foreach (char ch in json)
        //    {
        //        switch (ch)
        //        {
        //            case '"':
        //                if (!ignore)
        //                    quote = !quote;
        //                break;
        //            case '\\':
        //                if (quote && last != '\\')
        //                    ignore = true;
        //                break;
        //        }

        //        if (quote)
        //        {
        //            sb.Append(ch);
        //            if (last == '\\' && ignore)
        //                ignore = false;
        //        }
        //        else
        //        {
        //            switch (ch)
        //            {
        //                case '{':
        //                case '[':
        //                    sb.Append(ch);
        //                    sb.Append(Environment.NewLine);
        //                    sb.Append(new string(' ', ++offset * indentLength));
        //                    break;
        //                case '}':
        //                case ']':
        //                    sb.Append(Environment.NewLine);
        //                    sb.Append(new string(' ', --offset * indentLength));
        //                    sb.Append(ch);
        //                    break;
        //                case ',':
        //                    sb.Append(ch);
        //                    sb.Append(Environment.NewLine);
        //                    sb.Append(new string(' ', offset * indentLength));
        //                    break;
        //                case ':':
        //                    sb.Append(ch);
        //                    sb.Append(' ');
        //                    break;
        //                default:
        //                    if (quote || ch != ' ')
        //                        sb.Append(ch);
        //                    break;
        //            }
        //        }
        //        last = ch;
        //    }

        //    return sb.ToString().Trim();
        //}

        /// <summary>
        /// Método que pesquisa imagens do hotel pedido com cognitiveservices do azure 
        /// </summary>
        /// <param name="queryHotel"></param>
        /// <param name="canal"></param>
        /// <returns></returns>
        public static List<string> ImageSearch(string queryHotel, string canal)
        {
            var client           = new ImageSearchAPI(new Microsoft.Azure.CognitiveServices.Search.ImageSearch.ApiKeyServiceClientCredentials(ConfigurationManager.AppSettings["BingSearchKey"].ToString()));
            List<string> urlList = null;
            int numm_imagess     = int.Parse(ConfigurationManager.AppSettings["Numimages"].ToString());

            try
            {
                var imageResults = client.Images.SearchAsync(query: queryHotel).Result;

                urlList = new List<string>();

                if (canal == "skype")
                {
                    numm_imagess = 10;
                }

                if (imageResults == null)
                {
                    //nao encontrou nada
                    urlList.Add("");
                }
                else
                {
                    // Image results
                    if (imageResults.Value.Count > 0)
                    {
                         //var firstImageResult = imageResults.Value.First();

                        //Console.WriteLine($"Image result count: {imageResults.Value.Count}");
                        //Console.WriteLine($"First image insights token: {firstImageResult.ImageInsightsToken}");
                        // Console.WriteLine($"First image thumbnail url: {firstImageResult.ThumbnailUrl}");
                        //Console.WriteLine($"First image content url: {firstImageResult.ContentUrl}");
                        
                        for (int i = 0; i < numm_imagess; i++)
                        {
                            urlList.Add(imageResults.Value.ElementAt(i).ContentUrl);
                        }
                    }
                    else
                    {
                        urlList.Add("");
                    }

                    //Console.WriteLine($"Image result total estimated matches: {imageResults.TotalEstimatedMatches}");
                    //Console.WriteLine($"Image result next offset: {imageResults.NextOffset}");

                    //// Pivot suggestions
                    //if (imageResults.PivotSuggestions.Count > 0)
                    //{
                    //    var firstPivot = imageResults.PivotSuggestions.First();

                    //    Console.WriteLine($"Pivot suggestion count: {imageResults.PivotSuggestions.Count}");
                    //    Console.WriteLine($"First pivot: {firstPivot.Pivot}");

                    //    if (firstPivot.Suggestions.Count > 0)
                    //    {
                    //        var firstSuggestion = firstPivot.Suggestions.First();

                    //        Console.WriteLine($"Suggestion count: {firstPivot.Suggestions.Count}");
                    //        Console.WriteLine($"First suggestion text: {firstSuggestion.Text}");
                    //        Console.WriteLine($"First suggestion web search url: {firstSuggestion.WebSearchUrl}");
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine("Couldn't find suggestions!");
                    //    }
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Couldn't find pivot suggestions!");
                    //}

                    //// Query expansions
                    //if (imageResults.QueryExpansions.Count > 0)
                    //{
                    //    var firstQueryExpansion = imageResults.QueryExpansions.First();

                    //    Console.WriteLine($"Query expansion count: {imageResults.QueryExpansions.Count}");
                    //    Console.WriteLine($"First query expansion text: {firstQueryExpansion.Text}");
                    //    Console.WriteLine($"First query expansion search link: {firstQueryExpansion.SearchLink}");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Couldn't find query expansions!");
                    //}
                }

               
            }

            catch (Exception ex)
            {
                Console.WriteLine("Encountered exception. " + ex.Message);
            }

            return urlList;
        }

        /// <summary>
        /// Método que pesquisa videos do hotel pedido com cognitiveservices do azure 
        /// </summary>
        /// <param name="queryHotel"></param>
        /// <param name="canal"></param>
        /// <returns></returns>
        public static List<string> VideoSearch(string queryHotel, string canal)
        {
            var client           = new VideoSearchAPI(new Microsoft.Azure.CognitiveServices.Search.VideoSearch.ApiKeyServiceClientCredentials(ConfigurationManager.AppSettings["BingSearchKey"].ToString()));
            List<string> urlList = null;
            int num_videos       = int.Parse(ConfigurationManager.AppSettings["NumVideos"].ToString());

            try
            {
                var imageResults = client.Videos.SearchAsync(query: queryHotel).Result;
               
                urlList = new List<string>();

                if (imageResults == null)
                {
                    //nao encontrou nada
                    urlList.Add("");
                }
                else
                {
                    // video results
                    if (imageResults.Value.Count > 0)
                    {
                        for (int i = 0; i < num_videos; i++)
                        {
                            
                            urlList.Add(imageResults.Value.ElementAt(i).ContentUrl);
                        }
                    }
                    else
                        urlList.Add("");
                    

                    //Console.WriteLine($"Image result total estimated matches: {imageResults.TotalEstimatedMatches}");
                    //Console.WriteLine($"Image result next offset: {imageResults.NextOffset}");

                    //// Pivot suggestions
                    //if (imageResults.PivotSuggestions.Count > 0)
                    //{
                    //    var firstPivot = imageResults.PivotSuggestions.First();

                    //    Console.WriteLine($"Pivot suggestion count: {imageResults.PivotSuggestions.Count}");
                    //    Console.WriteLine($"First pivot: {firstPivot.Pivot}");

                    //    if (firstPivot.Suggestions.Count > 0)
                    //    {
                    //        var firstSuggestion = firstPivot.Suggestions.First();

                    //        Console.WriteLine($"Suggestion count: {firstPivot.Suggestions.Count}");
                    //        Console.WriteLine($"First suggestion text: {firstSuggestion.Text}");
                    //        Console.WriteLine($"First suggestion web search url: {firstSuggestion.WebSearchUrl}");
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine("Couldn't find suggestions!");
                    //    }
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Couldn't find pivot suggestions!");
                    //}

                    //// Query expansions
                    //if (imageResults.QueryExpansions.Count > 0)
                    //{
                    //    var firstQueryExpansion = imageResults.QueryExpansions.First();

                    //    Console.WriteLine($"Query expansion count: {imageResults.QueryExpansions.Count}");
                    //    Console.WriteLine($"First query expansion text: {firstQueryExpansion.Text}");
                    //    Console.WriteLine($"First query expansion search link: {firstQueryExpansion.SearchLink}");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Couldn't find query expansions!");
                    //}
                }


            }

            catch (Exception ex)
            {
                Console.WriteLine("Encountered exception. " + ex.Message);
            }

            return urlList;
        }

         
    }

}