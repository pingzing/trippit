using DigiTransit10.GraphQL;

namespace DigiTransit10.Models.ApiModels
{
    /// <summary>
    /// A basic static class that contains properties for defining all the member names the server uses in GQL queries
    /// so we can avoid needing to deal with raw strings when constructing a <see cref="GqlQuery"/> .
    /// </summary>
    public static class ApiGqlMembers
    {
        public static string points => nameof(points);
        public static string length => nameof(length);
        public static string legGeometry => nameof(legGeometry);
        public static string stoptimes => nameof(stoptimes);
        public static string intermediatePlaces => nameof(intermediatePlaces);
        public static string stoptimesForServiceDate => nameof(stoptimesForServiceDate);
        public static string id => "id";
        public static string plan => "plan";
        public static string from => "from";
        public static string lat => "lat";
        public static string lon => "lon";
        public static string to => "to";
        public static string numItineraries => "numItineraries";
        public static string time => "time";
        public static string date => "date";
        public static string arriveBy => "arriveBy";
        public static string itineraries => "itineraries";
        public static string legs => "legs";
        public static string startTime => "startTime";
        public static string endTime => "endTime";
        public static string mode => "mode";
        public static string modes => "modes";
        public static string duration => "duration";
        public static string realTime => "realTime";
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
    }
}
