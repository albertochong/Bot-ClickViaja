using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickViajaBot.Model
{
    public class Amadeus
    {
        public double x
        {
            get; set;
        }
    }

    #region Amadeus Token

    public class AmadeusAuthorization
    {
        public string type
        {
            get; set;
        }
        public string username
        {
            get; set;
        }
        public string application_name
        {
            get; set;
        }
        public string client_id
        {
            get; set;
        }
        public string token_type
        {
            get; set;
        }
        public string access_token
        {
            get; set;
        }
        public int expires_in
        {
            get; set;
        }
        public string state
        {
            get; set;
        }
        public string scope
        {
            get; set;
        }
    }

    #endregion

    #region Amadeus get iata airport Code

    public class Links
    {
        public string self
        {
            get; set;
        }
        public string next
        {
            get; set;
        }
        public string last
        {
            get; set;
        }
    }

    public class Meta
    {
        public int count
        {
            get; set;
        }
        public Links links
        {
            get; set;
        }
    }

    public class Self
    {
        public string href
        {
            get; set;
        }
        public List<string> methods
        {
            get; set;
        }
    }

    public class Address
    {
        public string cityName
        {
            get; set;
        }
        public string countryName
        {
            get; set;
        }
    }

    public class Data
    {
        public string type
        {
            get; set;
        }
        public string subType
        {
            get; set;
        }
        public string name
        {
            get; set;
        }
        public string detailedName
        {
            get; set;
        }
        public string id
        {
            get; set;
        }
        public Self self
        {
            get; set;
        }
        public string iataCode
        {
            get; set;
        }
        public Address address
        {
            get; set;
        }
    }

    public class RootObject
    {
        public Meta meta
        {
            get; set;
        }
        public List<Data> data
        {
            get; set;
        }
    }

    #endregion

    #region amadeus search fligths

    public class Departure
    {
        public string iataCode
        {
            get; set;
        }
        public string terminal
        {
            get; set;
        }
        public DateTime at
        {
            get; set;
        }
    }

    public class Arrival
    {
        public string iataCode
        {
            get; set;
        }
        public string terminal
        {
            get; set;
        }
        public DateTime at
        {
            get; set;
        }
    }

    public class Aircraft
    {
        public string code
        {
            get; set;
        }
    }

    public class Operating
    {
        public string carrierCode
        {
            get; set;
        }
        public string number
        {
            get; set;
        }
    }

    public class FlightSegment
    {
        public Departure departure
        {
            get; set;
        }
        public Arrival arrival
        {
            get; set;
        }
        public string carrierCode
        {
            get; set;
        }
        public string number
        {
            get; set;
        }
        public Aircraft aircraft
        {
            get; set;
        }
        public Operating operating
        {
            get; set;
        }
        public string duration
        {
            get; set;
        }
    }

    public class PricingDetailPerAdult
    {
        public string travelClass
        {
            get; set;
        }
        public string fareClass
        {
            get; set;
        }
        public int availability
        {
            get; set;
        }
        public string fareBasis
        {
            get; set;
        }
    }

    public class Segment
    {
        public FlightSegment flightSegment
        {
            get; set;
        }
        public PricingDetailPerAdult pricingDetailPerAdult
        {
            get; set;
        }
    }

    public class Service
    {
        public List<Segment> segments
        {
            get; set;
        }
    }

    public class Price
    {
        public string total
        {
            get; set;
        }
        public string totalTaxes
        {
            get; set;
        }
    }

    public class PricePerAdult
    {
        public string total
        {
            get; set;
        }
        public string totalTaxes
        {
            get; set;
        }
    }

    public class OfferItem
    {
        public List<Service> services
        {
            get; set;
        }
        public Price price
        {
            get; set;
        }
        public PricePerAdult pricePerAdult
        {
            get; set;
        }
    }

    public class DataSearchFligths
    {
        public string type
        {
            get; set;
        }
        public string id
        {
            get; set;
        }
        public List<OfferItem> offerItems
        {
            get; set;
        }
    }

    //public class Carriers
    //{
    //    public string SN
    //    {
    //        get; set;
    //    }
    //    public string UA
    //    {
    //        get; set;
    //    }
    //}

    public class Currencies
    {
        public string EUR
        {
            get; set;
        }
    }

    //public class Aircraft2
    //{
    //    public string __invalid_name__319
    //    {
    //        get; set;
    //    }
    //    public string __invalid_name__333
    //    {
    //        get; set;
    //    }
    //    public string __invalid_name__777
    //    {
    //        get; set;
    //    }
    //}

    //public class EWR
    //{
    //    public string subType
    //    {
    //        get; set;
    //    }
    //    public string detailedName
    //    {
    //        get; set;
    //    }
    //}

    //public class MAD
    //{
    //    public string subType
    //    {
    //        get; set;
    //    }
    //    public string detailedName
    //    {
    //        get; set;
    //    }
    //}

    //public class BRU
    //{
    //    public string subType
    //    {
    //        get; set;
    //    }
    //    public string detailedName
    //    {
    //        get; set;
    //    }
    //}

    //public class JFK
    //{
    //    public string subType
    //    {
    //        get; set;
    //    }
    //    public string detailedName
    //    {
    //        get; set;
    //    }
    //}

    //public class Locations
    //{
    //    public EWR EWR
    //    {
    //        get; set;
    //    }
    //    public MAD MAD
    //    {
    //        get; set;
    //    }
    //    public BRU BRU
    //    {
    //        get; set;
    //    }
    //    public JFK JFK
    //    {
    //        get; set;
    //    }
    //}

    public class Dictionaries
    {
        //public Carriers carriers
        //{
        //    get; set;
        //}
        public Currencies currencies
        {
            get; set;
        }
        //public Aircraft2 aircraft
        //{
        //    get; set;
        //}
        //public Locations locations
        //{
        //    get; set;
        //}
    }
  
    public class Defaults
    {
        public bool nonStop
        {
            get; set;
        }
    }

    public class RootObjectDataSearchFligths
    {
        public List<DataSearchFligths> data
        {
            get; set;
        }
        public Dictionaries dictionaries
        {
            get; set;
        }
    }

    #endregion

    #region amadeus search Cheaptest fligths

    public class DataSearchCheapFligths
    {
        public string type
        {
            get; set;
        }
        public string origin
        {
            get; set;
        }
        public string destination
        {
            get; set;
        }
        public DateTime departureDate
        {
            get; set;
        }
        public DateTime returnDate
        {
            get; set;
        }
        public PriceFligth price
        {
            get; set;
        }
    }

    public class PriceFligth
    {
        public string total
        {
            get; set;
        }
        
    }


    #endregion
}