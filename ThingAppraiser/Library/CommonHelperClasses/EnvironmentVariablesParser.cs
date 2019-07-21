﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser
{
    /// <summary>
    /// Provides access to read environment variable values. Service reads only specified 
    /// environment variable and parse it.
    /// </summary>
    public static class EnvironmentVariablesParser
    {
        /// <summary>
        /// Stores parsed values from environment variable.
        /// </summary>
        private static readonly Dictionary<string, string> _values;

        /// <summary>
        /// Name of the environment variable to read.
        /// </summary>
        private static string EnvironmentVariableName { get; } = "ThingAppraiser";

        /// <summary>
        /// Specifies the target where the variable is located.
        /// </summary>
        private static EnvironmentVariableTarget DefaultVariableTarget { get; } =
            EnvironmentVariableTarget.User;


        /// <summary>
        /// Reads environment variable and initializes key-values dictionary.
        /// </summary>
        static EnvironmentVariablesParser()
        {
            string environmentVariableValue = Environment.GetEnvironmentVariable(
                EnvironmentVariableName, DefaultVariableTarget
            );
            _values = ParseEnvironmentVariableValue(environmentVariableValue);
        }

        /// <summary>
        /// Gets value from dictionary with parsed values.
        /// </summary>
        /// <param name="variableName">Variable name to get value.</param>
        /// <returns>Raw value of specified key.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="variableName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="variableName" /> isn't contained in the dictionary -or-
        /// <paramref name="variableName" /> presents empty string.
        /// </exception>
        public static string GetValue(string variableName)
        {
            variableName.ThrowIfNullOrEmpty(nameof(variableName));

            if (!_values.TryGetValue(variableName, out string value))
            {
                throw new ArgumentException(
                    "Specified argument didn't find in environment variable values.",
                    nameof(variableName)
                );
            }
            return value;
        }

        /// <summary>
        /// Gets value from dictionary and converts it to specified type.
        /// </summary>
        /// <typeparam name="T">Type to convert.</typeparam>
        /// <param name="variableName">Variable name to get value.</param>
        /// <returns>Converted value of specified key.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="variableName" /> isn't contained in the dictionary -or-
        /// <paramref name="variableName" /> is <c>null</c> or presents empty.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// This conversion is not supported. -or- value is <c>null</c> and conversionType is a
        /// value type. -or- value does not implement the System.IConvertible interface.
        /// </exception>
        /// <exception cref="FormatException">
        /// Value is not in a format recognized by conversionType.
        /// </exception>
        /// <exception cref="OverflowException">
        /// Value represents a number that is out of the range of conversionType.
        /// </exception>
        /// <exception cref="ArgumentNullException">ConversionType is <c>null</c>.</exception>
        public static T GetValue<T>(string variableName)
            where T : IConvertible
        {
            string stringValue = GetValue(variableName);
            return (T) Convert.ChangeType(stringValue, typeof(T));
        }

        /// <summary>
        /// Tries to get value from dictionary and converts it to specified type or return default
        /// value.
        /// </summary>
        /// <typeparam name="T">Type to convert.</typeparam>
        /// <param name="variableName">Variable name to get value.</param>
        /// <returns>Converted value of specified key.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="variableName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="variableName" /> presents empty string.
        /// </exception>
        public static T GetValueOrDefault<T>(string variableName, T defaultValue = default)
            where T : IConvertible
        {
            variableName.ThrowIfNullOrEmpty(nameof(variableName));

            try
            {
                string stringValue = GetValue(variableName);
                return (T) Convert.ChangeType(stringValue, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Parses environment variable value and converts it to dictionary of key-value pairs.
        /// </summary>
        /// <param name="environmentVariableValue">Raw value of environment variable.</param>
        /// <returns>Dictionary with parsed values.</returns>
        private static Dictionary<string, string> ParseEnvironmentVariableValue(
            string environmentVariableValue)
        {
            if (string.IsNullOrEmpty(environmentVariableValue))
            {
                return new Dictionary<string, string>();
            }

            string[] keyValuePairsRaw = environmentVariableValue.Split(';');

            Dictionary<string, string> result = keyValuePairsRaw
                .Select(kv => ProcessKeyValuePair(kv))
                .ToDictionary(kv => kv.key, kv => kv.value);

            return result;
        }

        /// <summary>
        /// Processes single key-value pair of string.
        /// </summary>
        /// <param name="rawKeyValuePair">String value of pair.</param>
        /// <returns>Parsed key and value packed in a tuple.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="paramName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="rawKeyValuePair" /> presents empty string.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="rawKeyValuePair" /> has invalid format. -or-
        /// <paramref name="rawKeyValuePair" /> has empty value among keys. -or-
        /// <paramref name="rawKeyValuePair" /> has empty value among values.
        /// </exception>
        private static (string key, string value) ProcessKeyValuePair(string rawKeyValuePair)
        {
            rawKeyValuePair.ThrowIfNullOrEmpty(nameof(rawKeyValuePair));

            string[] keyValuePair = rawKeyValuePair.Split('=');
            if (keyValuePair.Length != 2)
            {
                throw new InvalidOperationException(
                    $"Environment variable has invalid format: {rawKeyValuePair}"
                );
            }

            if (string.IsNullOrWhiteSpace(keyValuePair[0]))
            {
                throw new InvalidOperationException(
                    "Environment variable has empty value among keys: " +
                    $"{keyValuePair[0]}"
                );
            }
            if (string.IsNullOrWhiteSpace(keyValuePair[1]))
            {
                throw new InvalidOperationException(
                    "Environment variable has empty value among values: " +
                    $"{keyValuePair[1]}"
                );
            }

            return (key: keyValuePair[0], value: keyValuePair[1]);
        }
    }
}
