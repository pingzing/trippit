using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.GraphQL
{
    public class GqlTuple : List<KeyValuePair<string, object>>
    {
        public void Add(string key, object value)
        {
            Add(new KeyValuePair<string, object>(key, value));
        }

        public GqlTuple(params KeyValuePair<string, object>[] items)
        {
            this.AddRange(items);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach(var keyPair in this)
            {
                bool isLast = this.IndexOf(keyPair) == this.Count - 1;

                sb.Append($"{keyPair.Key}: ");
                sb.Append(ParserHelpers.ParseValue(keyPair.Value));
                if(!isLast) sb.Append(", ");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
