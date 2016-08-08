using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.ExtensionMethods
{
    public static class DictionaryExtensions
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> thisDict, TKey key, TValue value)
        {
            if(thisDict.ContainsKey(key))
            {
                thisDict[key] = value;
            }
            else
            {
                thisDict.Add(key, value);
            }
        }
    }
}
