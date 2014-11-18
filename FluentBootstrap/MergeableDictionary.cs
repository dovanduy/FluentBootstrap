﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBootstrap
{
    // This class wraps an IDictionary<string, string> and allows merging key/value pairs
    internal class MergeableDictionary
    {
        public IDictionary<string, string> Dictionary { get; private set; }

        public MergeableDictionary()
        {
            Dictionary = new Dictionary<string, string>();
        }

        public void Merge(object values, bool replaceExisting = true)
        {
            if (values == null)
            {
                return;
            }
            AnonymousObjectToHtmlAttributes(values, replaceExisting);
        }

        public void Merge<TKey, TValue>(IDictionary<TKey, TValue> dictionary, bool replaceExisting = true)
        {
            if (dictionary == null)
            {
                return;
            }
            foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                string key = Convert.ToString(kvp.Key, CultureInfo.InvariantCulture);
                string value = Convert.ToString(kvp.Value, CultureInfo.InvariantCulture);
                Merge(key, value, replaceExisting);
            }
        }

        public void Merge(string key, string value, bool replaceExisting = true)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }
            if (value == null && replaceExisting && Dictionary.ContainsKey(key))
            {
                Dictionary.Remove(key);
            }
            else if (value != null && (replaceExisting || !Dictionary.ContainsKey(key)))
            {
                Dictionary[key] = value;
            }
        }
        
        // This returns string.Empty if not found
        public string GetValue(string key)
        {
            string value;
            if (Dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            return string.Empty;
        }

        private void AnonymousObjectToHtmlAttributes(object htmlAttributes, bool replaceExisting)
        {
            if (htmlAttributes != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(htmlAttributes))
                {
                    Merge(property.Name.Replace('\u005F', '-'), Convert.ToString(property.GetValue(htmlAttributes), CultureInfo.InvariantCulture));
                }
            }
        }
    }
}
