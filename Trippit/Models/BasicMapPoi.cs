﻿using System;
using Windows.Devices.Geolocation;

namespace Trippit.Models
{
    public class BasicMapPoi : IMapPoi
    {
        public BasicGeoposition Coords { get; set; }
        public string Name { get; set; }

        public Guid Id{ get; set; }
    }
}
