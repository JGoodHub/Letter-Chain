using System;
using System.Collections;
using UnityEngine;

namespace GoodHub.Core
{
    /// <summary>
    /// A class to manage cross-scene data handling
    /// </summary>
    public class Properties
    {
        /// <summary>
        /// Main store for data
        /// </summary>
        private static Hashtable properties;

        /// <summary>
        /// Get the property at the provided key
        /// </summary>
        /// <param name="key">The key to search</param>
        /// <returns>The value at the keys position or null if no value found</returns>
        public static object GetProperty(string key)
        {
            if (properties.ContainsKey(key))
            {
                return properties[key];
            }

            return null;
        }

        /// <summary>
        /// Try get the property at the provided key and output the value found
        /// </summary>
        /// <typeparam name="T">The expected type of the value</typeparam>
        /// <param name="key">The key to search</param>
        /// <param name="outputValue">The value to assign the output to</param>
        /// <returns>Was the value found</returns>
        public static bool TryGetProperty<T>(string key, out T outputValue)
        {
            if (properties.ContainsKey(key))
            {
                try
                {
                    outputValue = (T)properties[key];
                    return true;
                }
                catch (InvalidCastException e)
                {
                    Debug.LogError($"Value of property at key: {key} is not of type: {typeof(T)}");
                }
            }

            outputValue = default(T);
            return false;
        }

        /// <summary>
        /// Set or update the provided key value pair
        /// </summary>
        /// <param name="key">The key to store the value under</param>
        /// <param name="value">The value to store</param>
        public static void SetProperty(string key, object value)
        {
            SetProperties(new Hashtable() { { key, value } });
        }

        /// <summary>
        /// Set or update all provided key value pairs
        /// </summary>
        /// <param name="newProps">A hashtable containing the key value pairs to set or update</param>
        public static void SetProperties(Hashtable newProps)
        {
            if (newProps != null)
            {
                foreach (string key in newProps.Keys)
                {
                    if (properties.ContainsKey(key))
                    {
                        properties[key] = newProps[key];
                    }
                    else
                    {
                        properties.Add(key, newProps[key]);
                    }
                }
            }
        }

        /// <summary>
        /// Clear all properties
        /// </summary>
        public static void ClearAllProperties()
        {
            properties.Clear();
        }

        /// <summary>
        /// Clear the property at the provided key
        /// </summary>
        /// <param name="key">The key to clear</param>
        /// <returns>Whether the properties contain the provided key</returns>
        public static bool ClearProperty(string key)
        {
            if (properties.ContainsKey(key))
            {
                properties.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clear all property for each of the provided keys
        /// </summary>
        /// <param name="keys">An array of keys to clear</param>
        /// <returns>Whether the properties contained all provided keys</returns>
        public static bool ClearProperties(string[] keys)
        {
            bool allCleared = true;
            for (int i = 0; i < keys.Length; i++)
            {
                if (ClearProperty(keys[i]) == false)
                {
                    allCleared = false;
                }
            }

            return allCleared;
        }


        /// <summary>
        /// Checks if the properties table contains a key
        /// </summary>
        /// <param name="key">Key to check for</param>
        /// <returns>True if found</returns>
        public static bool ContainsKey(string key)
        {
            return properties.ContainsKey(key);
        }
    }

}
