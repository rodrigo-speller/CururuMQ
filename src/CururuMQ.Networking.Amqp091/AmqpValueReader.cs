// Copyright 2019 Rodrigo Speller. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.
// See LICENSE.txt in the solution directory for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace CururuMQ.Networking.Amqp091
{
    public class AmqpValueReader : IDisposable
    {
        private byte[] stringBuffer = null;
        
        public AmqpValueReader(Stream input, bool leaveOpen)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            BaseReader = new BinaryReader(input, AmqpDefinitions.UTF8, leaveOpen);
        }
        
        protected BinaryReader BaseReader { get; }

        public void Dispose()
            => BaseReader.Dispose();

        // USE SEALED READS TO REUSE BaseReader's OPERATIONS
        // This avoids side effects by calling overridden methods
        // by inherited readers.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected double SealedReadDouble()
            => AmqpDefinitions.FloatConverter.ToDouble(BaseReader.ReadBytes(8));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected short SealedReadInt16()
            => AmqpDefinitions.IntegerConverter.ToInt16(BaseReader.ReadBytes(2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int SealedReadInt32()
            => AmqpDefinitions.IntegerConverter.ToInt32(BaseReader.ReadBytes(4));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected long SealedReadInt64()
            => AmqpDefinitions.IntegerConverter.ToInt64(BaseReader.ReadBytes(8));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected float SealedReadSingle()
            => AmqpDefinitions.FloatConverter.ToSingle(BaseReader.ReadBytes(4));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ushort SealedReadUInt16()
            => AmqpDefinitions.IntegerConverter.ToUInt16(BaseReader.ReadBytes(2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected uint SealedReadUInt32()
            => AmqpDefinitions.IntegerConverter.ToUInt32(BaseReader.ReadBytes(4));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ulong SealedReadUInt64()
            => AmqpDefinitions.IntegerConverter.ToUInt64(BaseReader.ReadBytes(8));

        public virtual IEnumerable ReadArray()
        {
            var length = SealedReadInt32();

            if (length == 0)
                return Array.Empty<object>();
            
            var data = BaseReader.ReadBytes(length);
            var array = AmqpValueParser.ParseArray(data, 0, length);
            return array;
        }

        public virtual bool ReadBoolean()
            => BaseReader.ReadBoolean();

        public virtual byte ReadByte()
            => BaseReader.ReadByte();

        public virtual byte[] ReadBytes()
        {
            var lengthData = SealedReadUInt32();

            int length = checked((int)lengthData);

            return BaseReader.ReadBytes(length);
        }
        
        public virtual decimal ReadDecimal()
        {
            var scale = BaseReader.ReadByte();
            var value = SealedReadInt32();
            var signal = value < 0;

            value = Math.Abs(value);

            return new decimal(value, 0, 0, signal, scale);
        }

        public virtual double ReadDouble()
            => SealedReadDouble();

        public virtual short ReadInt16()
            => SealedReadInt16();

        public virtual int ReadInt32()
            => SealedReadInt32();

        public virtual long ReadInt64()
            => SealedReadInt64();

        public virtual sbyte ReadSByte()
            => BaseReader.ReadSByte();

        public virtual float ReadSingle()
            => SealedReadSingle();

        public virtual string ReadString()
        {
            var dataLength = BaseReader.ReadByte();

            if (dataLength == 0)
                return String.Empty;

            if (stringBuffer == null)
                stringBuffer = new byte[255];
            
            int bytesRead;
            for (var currentPos = 0; currentPos < dataLength; currentPos += bytesRead)
            {
                int remainLength = dataLength - currentPos;
                bytesRead = BaseReader.Read(stringBuffer, currentPos, remainLength);
                if (bytesRead == 0)
                {
                    // Hack: throw EOF
                    stringBuffer[currentPos] = BaseReader.ReadByte();
                    bytesRead = 1;
                }
            }
            
            return AmqpDefinitions.UTF8.GetString(stringBuffer, 0, dataLength);
        }

        public virtual IReadOnlyDictionary<string, object> ReadTable()
        {
            int length = checked((int)SealedReadUInt32());

            if (length == 0)
                return new Dictionary<string, object>();

            var data = BaseReader.ReadBytes(length);
            var array = AmqpValueParser.ParseTable(data, 0, (int)length);
            return array;
        }

        public virtual DateTime ReadTime()
        {
            var posixTimestamp = SealedReadUInt64();
            return AmqpDefinitions.UnixEpoch.AddSeconds(posixTimestamp);
        }

        public virtual ushort ReadUInt16()
            => SealedReadUInt16();

        public virtual uint ReadUInt32()
            => SealedReadUInt32();

        public virtual ulong ReadUInt64()
            => SealedReadUInt64();

        public virtual object ReadFieldValue()
        {
            var signature = BaseReader.ReadByte();
            
            switch ((char)signature)
            {
                case 't': // boolean: 0 = FALSE, else TRUE
                    return ReadBoolean();
                case 'b': // short-short-int
                    return ReadSByte();
                case 'B': // short-short-uint
                    return ReadByte();
                case 'U': // short-int
                    return ReadInt16();
                case 'u': // short-uint
                    return ReadUInt16();
                case 'I': // long-int
                    return ReadInt32();
                case 'i': // long-uint
                    return ReadUInt32();
                case 'L': // long-long-int
                    return ReadInt64();
                case 'l': // long-long-uint
                    return ReadUInt64();
                case 'f': //float: IEEE-754 
                    return ReadSingle();
                case 'd': // double: rfc1832 XDR double
                    return ReadDouble();
                case 'D': // decimal-value: scale long-int
                    return ReadDecimal();
                case 's': // short-string: OCTET *string-char: length + content
                    return ReadString();
                case 'S': // long-string: long-uint *OCTET: length + content
                    return ReadBytes();
                case 'A': // field-array: long-int *field-value: array of values
                    return ReadArray();
                case 'T': // timestamp: long-long-uint: 64-bit POSIX 
                    return ReadTime();
                case 'F': // field-table: long-uint *(field-name field-value)
                    return ReadTable();
                case 'V': // no field
                    return null;

                default:
                    throw new InvalidOperationException($"Unknown field signature: 0x{signature:X2}");
            }
        }
    }
}
