namespace Trippit.GraphQL
{
    public class GqlParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public GqlParameter(string key, object value)
        {
            Name = key;
            Value = value;
        }
    }
}