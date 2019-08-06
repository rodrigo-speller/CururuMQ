// Copyright 2019 Rodrigo Speller. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.
// See LICENSE.txt in the solution directory for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CururuMQ.Networking.Amqp091
{
    internal sealed class InMemoryAmqpValueReader : AmqpValueReader
    {
        private readonly byte[] buffer;
        private readonly int bufferIndex;
        
        public InMemoryAmqpValueReader(byte[] buffer, int index, int count)
            : base(new MemoryStream(buffer, index, count, false), true)
        {
            this.buffer = buffer;
            bufferIndex = index;
        }

        public bool IsEndOfStream {
            get {
                var stream = BaseReader.BaseStream;
                return stream.Position == stream.Length;
            }
        }
        
        public override IEnumerable ReadArray()
        {
            var length = SealedReadInt32();
            
            if (length == 0)
                return Array.Empty<object>();

            var stream = BaseReader.BaseStream;
            var currentIndex = bufferIndex + (int)stream.Position;

            var array = AmqpValueParser.ParseArray(buffer, currentIndex, length);
            stream.Seek(length, SeekOrigin.Current);

            return array;
        }

        public override IReadOnlyDictionary<string, object> ReadTable()
        {
            var length = checked((int)SealedReadUInt32());

            if (length == 0)
                return new Dictionary<string, object>();

            var stream = BaseReader.BaseStream;
            var currentIndex = bufferIndex + (int)stream.Position;

            var table = AmqpValueParser.ParseTable(buffer, currentIndex, length);
            stream.Seek(length, SeekOrigin.Current);
            
            return table;
        }
    }
}
