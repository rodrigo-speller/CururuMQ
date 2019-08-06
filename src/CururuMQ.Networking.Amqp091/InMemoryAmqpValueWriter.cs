// Copyright 2019 Rodrigo Speller. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.
// See LICENSE.txt in the solution directory for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CururuMQ.Networking.Amqp091
{
    internal sealed class InMemoryAmqpValueWriter : AmqpValueWriter
    {
        private readonly MemoryStream stream;

        public InMemoryAmqpValueWriter(MemoryStream stream, bool leaveOpen)
            : base(stream, leaveOpen)
        {
            this.stream = stream;
        }

        public override void WriteArray(IEnumerable value)
        {
            var stream = this.stream;

            // mark begin
            var startPosition = stream.Position;
            stream.Seek(4, SeekOrigin.Current);

            // write data
            foreach (var item in value)
                WriteFieldValue(item);

            // compute length
            var endPosition = stream.Position;
            var length = checked((int)(endPosition - startPosition - 4));

            // write length
            stream.Position = startPosition;
            SealedWriteInt32(length);
            stream.Position = endPosition;
        }

        public override void WriteTable(IEnumerable<KeyValuePair<string, object>> value)
        {
            var stream = this.stream;
            
            // mark begin
            var startPosition = stream.Position;
            stream.Seek(4, SeekOrigin.Current);

            // write data
            var keys = new HashSet<string>();
            foreach (var item in value)
            {
                var key = item.Key;
                if (key == null)
                    throw new ArgumentException($"The table has a null field name.", nameof(value));

                if (!keys.Add(key))
                    throw new ArgumentException($"The table has a duplicated field name.", nameof(value));

                WriteString(key);
                WriteFieldValue(item.Value);
            }

            // compute length
            var endPosition = stream.Position;
            var length = checked((uint)(endPosition - startPosition - 4));

            // write length
            stream.Position = startPosition;
            SealedWriteUInt32(length);
            stream.Position = endPosition;
        }
    }
}
