namespace Trippit.Models.ApiModels
{
    /// <summary>
    /// A basic static class that contains properties for defining all the member names the server uses in GQL queries
    /// so we can avoid needing to deal with raw strings when constructing a <see cref="GqlQuery"/> .
    /// </summary>
    public static class ApiGqlMembers
    {
        internal static string effectiveEndDate => nameof(effectiveEndDate);

        internal static string effectiveStartDate => nameof(effectiveStartDate);

        internal static string alertUrl => nameof(alertUrl);

        internal static string language => nameof(language);

        internal static string text => nameof(text);

        internal static string alertDescriptionTextTranslations => nameof(alertDescriptionTextTranslations);

        internal static string alertHeaderTextTranslations => nameof(alertHeaderTextTranslations);

        internal static string alertHeaderText => nameof(alertHeaderText);

        public static string points => nameof(points);
        public static string length => nameof(length);
        public static string legGeometry => nameof(legGeometry);
        public static string stoptimes => nameof(stoptimes);
        public static string intermediatePlaces => nameof(intermediatePlaces);
        public static string stoptimesForServiceDate => nameof(stoptimesForServiceDate);
        public static string id => nameof(id);
        public static string plan => nameof(plan);
        public static string from => nameof(from);
        public static string lat => nameof(lat);
        public static string lon => nameof(lon);
        public static string to => nameof(to);
        public static string numItineraries => nameof(numItineraries);
        public static string time => nameof(time);
        public static string date => nameof(date);
        public static string arriveBy => nameof(arriveBy);
        public static string itineraries => "itineraries";
        public static string legs => "legs";
        public static string startTime => "startTime";
        public static string endTime => "endTime";
        public static string mode => "mode";
        public static string modes => "modes";
        public static string duration => "duration";
        public static string realtime => "realtime";
        public static string realTime => nameof(realTime); //yes, the API uses both "realtime" and "realTime". ffs...
        public static string distance => "distance";
        public static string transitLeg => "transitLeg";
        public static string stops => "stops";
        public static string gtfsId => "gtfsId";
        public static string name => "name";
        public static string code => "code";
        public static string route => nameof(route);
        public static string shortName => nameof(shortName);
        public static string routes => "routes";
        public static string type => "type";
        public static string fares => "fares";
        public static string currency => "currency";
        public static string cents => "cents";
        public static string intermediateStops => nameof(intermediateStops);
        public static string stoptimesWithoutPatterns => nameof(stoptimesWithoutPatterns);
        public static string scheduledArrival => nameof(scheduledArrival);
        public static string numberOfDepartures => nameof(numberOfDepartures);
        public static string longName => nameof(longName);
        public static string patterns => nameof(patterns);
        public static string geometry => nameof(geometry);
        public static string stopsByBbox => nameof(stopsByBbox);
        public static string minLat => nameof(minLat);
        public static string maxLat => nameof(maxLat);
        public static string minLon => nameof(minLon);
        public static string maxLon => nameof(maxLon);
        public static string stopsByRadius => nameof(stopsByRadius);
        public static string radius => nameof(radius);
        public static string edges => nameof(edges);
        public static string node => nameof(node);
        public static string stop => nameof(stop);
        public static string pattern => nameof(pattern);
        public static string realtimeState => nameof(realtimeState);
        public static string scheduledDeparture => nameof(scheduledDeparture);
        public static string realtimeArrival => nameof(realtimeArrival);
        public static string realtimeDeparture => nameof(realtimeDeparture);
        public static string stopHeadsign => nameof(stopHeadsign);
        public static string ids => nameof(ids);
        public static string walkSpeed => nameof(walkSpeed);
        public static string walkReluctance => nameof(walkReluctance);
        public static string alerts => nameof(alerts);
    }
}
