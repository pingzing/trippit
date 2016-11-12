using DigiTransit10.Models.ApiModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Models
{
    public class TripLeg
    {
        public bool IsStart { get; set; }
        public string StartPlaceName { get; set; }
        public DateTime StartTime { get; set; }
        public BasicGeoposition StartCoords { get; set; }

        public bool IsEnd { get; set; }
        public string EndPlaceName { get; set; }
        public DateTime EndTime { get; set; }
        public BasicGeoposition EndCoords { get; set; }

        public ApiMode Mode { get; set; }
        public string ShortName { get; set; }
        public float DistanceMeters { get; set; }

        public List<TransitStop> IntermediateStops { get; set; }
        public string LegGeometryString { get; set; }


        [JsonIgnore]
        public Guid TemporaryId { get; set; }

        /// <summary>
        /// Mostly for JSON.NET's benefit.
        /// </summary>
        public TripLeg() { }

        public TripLeg(ApiLeg apiLeg, bool isStart, bool isEnd, string startPlaceName, string endPlaceName)
        {
            IsStart = isStart;
            StartPlaceName = startPlaceName;
            StartTime = DateTimeOffset.FromUnixTimeMilliseconds(apiLeg.StartTime.Value).UtcDateTime;
            StartCoords = apiLeg.From.Coords;

            IsEnd = isEnd;
            EndPlaceName = endPlaceName;
            EndTime = DateTimeOffset.FromUnixTimeMilliseconds(apiLeg.EndTime.Value).UtcDateTime;
            EndCoords = apiLeg.To.Coords;

            Mode = apiLeg.Mode.Value;
            DistanceMeters = apiLeg.Distance.Value;
            if (Mode == ApiMode.Subway)
            {
                ShortName = "M";
            }
            else
            {
                ShortName = apiLeg.Route?.ShortName;
            }

            IntermediateStops = apiLeg.IntermediateStops
                .Select(x => new TransitStop
                {
                    Coords = new BasicGeoposition { Altitude = 0.0, Latitude = x.Lat, Longitude = x.Lon },
                    Name = x.Name
                }).ToList();

            LegGeometryString = apiLeg.LegGeometry.Points;
        }

        public IMapPoi StartPlaceToPoi()
        {
            return new BasicMapPoi
            {
                Coords = StartCoords,
                Name = StartPlaceName
            };
        }

        public IMapPoi EndPlaceToPoi()
        {
            return new BasicMapPoi
            {
                Coords = EndCoords,
                Name = EndPlaceName
            };
        }
    }
}
