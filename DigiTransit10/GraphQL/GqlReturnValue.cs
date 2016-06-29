using System.Collections.Generic;
using System.Linq;

namespace DigiTransit10.GraphQL
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
            Descendants = descendants.ToList();
        }
    }
}