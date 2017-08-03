using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trippit.GraphQL
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

        public string ParseToJsonString(DateParsingStrategy dateHandling = DateParsingStrategy.HyphenSeparators)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"query\": \"{ ");
            sb.Append($"{MethodName}"); //method name
            if (Parameters != null)
            {
                sb.Append("("); //opening paren
                foreach (var param in Parameters)
                {
                    bool isLast = Parameters.IndexOf(param) == Parameters.Count - 1;
                    sb.Append($"{param.Name}: {ParserHelpers.ParseValue(param.Value, dateHandling)}");
                    if (!isLast) sb.Append(", ");
                }
                sb.Append(")"); //closing method paren
            }            
            if (ReturnValues != null)
            {
                foreach (var retVal in ReturnValues)
                {
                    if(ReturnValues.First() == retVal) sb.Append("{");
                    sb = ParseReturnValueRecursively(sb, retVal, dateHandling);
                    if(ReturnValues.Last() == retVal) sb.Append("}");
                }
            }
            sb.Append("}\"}\""); //close the bracket enclosing the method, and the opening query bracket
            return sb.ToString();
        }        

        private StringBuilder ParseReturnValueRecursively(StringBuilder sb, GqlReturnValue retVal, DateParsingStrategy dateHandling = DateParsingStrategy.HyphenSeparators)
        {
            sb.Append($"{retVal.Name} ");

            var inlineMethod = retVal as GqlInlineMethodReturnValue;
            if (inlineMethod != null && inlineMethod.MethodParameters?.Count > 0)
            {
                sb.Append("(");
                int paramCount = inlineMethod.MethodParameters.Count;
                foreach (var param in inlineMethod.MethodParameters)
                {
                    bool isLast = inlineMethod.MethodParameters.IndexOf(param) == paramCount - 1;
                    sb.Append($"{param.Name}: {ParserHelpers.ParseValue(param.Value, dateHandling)}");
                    if (!isLast) sb.Append(",");
                }
                sb.Append(")");
            }

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

    public enum DateParsingStrategy
    {
        HyphenSeparators,
        NoSeparators
    }
}
