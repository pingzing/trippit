using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.GraphQL
{
    public class GqlQuery
    {
        public string MethodName { get; set; }
        public List<GqlParameter> Parameters { get; set; }
        public List<GqlReturnValue> ReturnValues { get; set; }

        public GqlQuery(string name)
        {
            MethodName = name;
        }

        public GqlQuery WithParameters(params GqlParameter[] parameters)
        {
            if (Parameters == null)
            {
                Parameters = new List<GqlParameter>(parameters);
            }
            else
            {
                Parameters.AddRange(parameters);
            }
            return this;
        }

        public GqlQuery WithReturnValues(params GqlReturnValue[] returnValues)
        {
            if(ReturnValues == null)
            {
                ReturnValues = new List<GqlReturnValue>(returnValues);
            }
            else
            {
                ReturnValues.AddRange(returnValues);
            }
            return this;
        }

        public string ParseToJsonString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"query\": \"{ ");
            sb.Append($"{MethodName}("); //method name, opening paren
            if (Parameters != null)
            {
                foreach (var param in Parameters)
                {
                    bool isLast = Parameters.IndexOf(param) == Parameters.Count - 1;
                    sb.Append($"{param.Name}: {param.Value}");
                    if (!isLast) sb.Append(", ");
                }
            }
            sb.Append(")"); //closing method paren
            if (ReturnValues != null)
            {
                foreach (var retVal in ReturnValues)
                {
                    sb.Append("{");
                    sb = ParseReturnValueRecursively(sb, retVal);
                    sb.Append("}");
                }
            }
            sb.Append("}\"}\""); //close the bracket enclosing the method, and the opening query bracket
            return sb.ToString();
        }

        private StringBuilder ParseReturnValueRecursively(StringBuilder sb, GqlReturnValue retVal)
        {
            sb.Append($"{retVal.Name} ");
            if(retVal.Descendants != null)
            {
                sb.Append("{");
                foreach (var innerRetVal in retVal.Descendants)
                {                    
                    ParseReturnValueRecursively(sb, innerRetVal);
                }
            }
            if(retVal.Descendants?.Count > 0) sb.Append("}");
            return sb;
        }
    }
}
