using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projekt_RESTfulWebAPI.Models;

namespace Projekt_RESTfulWebAPI.DTO
{
    namespace V1
    {
        public class GeoMessageDTO
        {
            public string Message { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
        }
    }

    namespace V2
    {
        public class GetGeoMessageDTO
        {
            public GetMessageDTO Message { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
        }

        public class GetMessageDTO
        {
            public string Title { get; set; }
            public string Body { get; set; }
            public string Author { get; set; }
        }

        public class AddGeoMessageDTO
        {
            public AddMessageDTO Message { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
        }

        public class AddMessageDTO
        {
            public string Title { get; set; }
            public string Body { get; set; }
        }
    }
}
