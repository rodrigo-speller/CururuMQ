// Copyright 2019 Rodrigo Speller. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.
// See LICENSE.txt in the solution directory for license information.

using System.Collections.Generic;
using System.Linq;

namespace CururuMQ.Networking.Amqp091
{
    internal static class AmqpValueParser
    {
        public static object[] ParseArray(byte[] buffer, int index, int count)
        {
            return ParseArrayItems(buffer, index, count)
                .ToArray();
        }

        private static IEnumerable<object> ParseArrayItems(byte[] buffer, int index, int count)
        {
            using (var reader = new InMemoryAmqpValueReader(buffer, index, count))
            {
                while (!reader.IsEndOfStream)
                    yield return reader.ReadFieldValue();
            }
        }

        public static Dictionary<string, object> ParseTable(byte[] buffer, int index, int count)
        {
            return ParseTableItems(buffer, index, count)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private static IEnumerable<(string Key, object Value)> ParseTableItems(byte[] buffer, int index, int count)
        {
            using (var reader = new InMemoryAmqpValueReader(buffer, index, count))
            {
                while (!reader.IsEndOfStream)
                    yield return (reader.ReadString(), reader.ReadFieldValue());
            }
        }
    }
}
