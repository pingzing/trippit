using System.Collections.Generic;
using System.Text;

namespace DigiTransit10.GraphQL
{
    public class GqlParameterArray : List<GqlTuple>
    {
        public string Key { get; set; }

        public GqlParameterArray(IEnumerable<GqlTuple> entries)
        {
            this.AddRange(entries);
        }

        public GqlParameterArray(params GqlTuple[] entries)
        {            
            this.AddRange(entries);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach(var entry in this)
            {
                bool isLast = this.IndexOf(entry) == this.Count - 1;
                sb.Append(entry.ToString());
                if (!isLast) sb.Append(", ");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}