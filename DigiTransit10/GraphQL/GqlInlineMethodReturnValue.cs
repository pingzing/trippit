using System.Collections.Generic;

namespace DigiTransit10.GraphQL
{
    public class GqlInlineMethodReturnValue : GqlReturnValue
    {
        public List<GqlParameter> MethodParameters { get; private set; }

        public GqlInlineMethodReturnValue(string name) : base(name) { }

        public GqlInlineMethodReturnValue(string name, params GqlReturnValue[] descendants) : base(name, descendants) { }

        public GqlInlineMethodReturnValue(string name, IEnumerable<GqlParameter> parameters,
            params GqlReturnValue[] descendants) : base(name, descendants)
        {
            MethodParameters = new List<GqlParameter>(parameters);
        }
    }
}
