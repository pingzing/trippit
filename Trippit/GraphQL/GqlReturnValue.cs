using System.Collections.Generic;

namespace Trippit.GraphQL
{
    public class GqlReturnValue
    {
        public string Name { get; set; }
        public List<GqlReturnValue> Descendants { get; set; }

        public GqlReturnValue(string name)
        {
            Name = name;
        }

        public GqlReturnValue(string name, params GqlReturnValue[] descendants)
        {
            Name = name;
            Descendants = new List<GqlReturnValue>(descendants);
        }
    }
}