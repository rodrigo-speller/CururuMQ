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
    public class AmqpValueWriter : IDisposable
    {
        /// <summary>
        /// <see cref="WriteString(string)"/> requires at least 255 bytes.
        /// </summary>
        private const int InitialBufferSize = 255;
        private MemoryStream buffer;

        public AmqpValueWriter(Stream output, bool leaveOpen)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            BaseWriter = new BinaryWriter(output, AmqpDefinitions.UTF8, leaveOpen);
        }
        
        protected BinaryWriter BaseWriter { get; }

        protected MemoryStream Buffer
        {
            get
            {
                if (buffer == null)
                    buffer = new MemoryStream(InitialBufferSize);

                return buffer;
            }
        }
        
        public void Dispose()
        {
            BaseWriter.Dispose();
            buffer?.Dispose();
        }

        protected void BufferedWrite(Action<AmqpValueWriter> action)
        {
            var buffer = Buffer;
            using (var writer = new InMemoryAmqpValueWriter(buffer, true))
            {
                buffer.SetLength(0);

                action(writer);

                buffer.Position = 0;
                buffer.WriteTo(BaseWriter.BaseStream);
            }
        }

        // USE SEALED WRITES TO REUSE BaseWrite's OPERATIONS
        // This avoids side effects by calling overridden methods
        // by inherited writers.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SealedWriteDouble(double value)
            => BaseWriter.Write(AmqpDefinitions.FloatConverter.GetBytes(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SealedWriteInt16(short value)
            => BaseWriter.Write(AmqpDefinitions.IntegerConverter.GetBytes(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SealedWriteInt32(int value)
            => BaseWriter.Write(AmqpDefinitions.IntegerConverter.GetBytes(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SealedWriteInt64(long value)
            => BaseWriter.Write(AmqpDefinitions.IntegerConverter.GetBytes(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SealedWriteSingle(float value)
            => BaseWriter.Write(AmqpDefinitions.FloatConverter.GetBytes(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SealedWriteUInt16(ushort value)
            => BaseWriter.Write(AmqpDefinitions.IntegerConverter.GetBytes(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SealedWriteUInt32(uint value)
            => BaseWriter.Write(AmqpDefinitions.IntegerConverter.GetBytes(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SealedWriteUInt64(ulong value)
            => BaseWriter.Write(AmqpDefinitions.IntegerConverter.GetBytes(value));

        public virtual void WriteArray(IEnumerable value)
            => BufferedWrite((writer) => writer.WriteArray(value));

        public virtual void WriteBoolean(bool value)
            => BaseWriter.Write(value);

        public virtual void WriteByte(byte value)
            => BaseWriter.Write(value);

        public virtual void WriteBytes(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var length = checked((uint)value.LongLength);
            SealedWriteUInt32(length);
            BaseWriter.Write(value);
        }

        public virtual void WriteDecimal(decimal value)
        {
            var parts = Decimal.GetBits(value);

            if (parts[0] < 0 || parts[1] != 0 || parts[2] != 0)
                throw new ArgumentException($"The decimal value '{value}' is out of AMQP bounds.", nameof(value));

            var signal = parts[3] < 0;
            byte scale = (byte)((parts[3] >> 16) & 0x7F);
            var part = parts[0];

            if (signal)
                part = unchecked(part | -0x80000000);

            BaseWriter.Write(scale);
            SealedWriteInt32(part);
        }

        public virtual void WriteDouble(double value)
            => SealedWriteDouble(value);

        public virtual void WriteInt16(short value)
            => SealedWriteInt16(value);

        public virtual void WriteInt32(int value)
            => SealedWriteInt32(value);

        public virtual void WriteInt64(long value)
            => SealedWriteInt64(value);

        public virtual void WriteSByte(sbyte value)
            => BaseWriter.Write(value);

        public virtual void WriteSingle(float value)
            => SealedWriteSingle(value);

        public virtual void WriteString(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length > 255)
                throw new ArgumentException($"The string length is out of AMQP bounds.", nameof(value));

            var buffer = Buffer.GetBuffer();

            int length;
            try
            {
                length = AmqpDefinitions.UTF8.GetBytes(value, 0, value.Length, buffer, 0);
            }
            catch (ArgumentException)
            {
                length = int.MaxValue;
            }
            
            if (length > 255)
                throw new ArgumentException($"The data length of the string is out of AMQP bounds.", nameof(value));

            BaseWriter.Write((byte)length);
            BaseWriter.Write(buffer, 0, length);
        }

        public virtual void WriteTable(IEnumerable<KeyValuePair<string, object>> value)
            => BufferedWrite((writer) => writer.WriteTable(value));

        public virtual void WriteTime(DateTime value)
        {
            var ticks = value.ToUniversalTime().Ticks - AmqpDefinitions.UnixEpoch.Ticks;

            if (ticks < 0)
                throw new ArgumentException($"The time value '{value}' is out of AMQP bounds.", nameof(value));

            var posixTimestamp = (ulong)(ticks / TimeSpan.TicksPerSecond);

            SealedWriteUInt64(posixTimestamp);
        }

        public virtual void WriteUInt16(ushort value)
            => SealedWriteUInt16(value);

        public virtual void WriteUInt32(uint value)
            => SealedWriteUInt32(value);

        public virtual void WriteUInt64(ulong value)
            => SealedWriteUInt64(value);

        public virtual void WriteFieldValue(bool value)
        {
            BaseWriter.Write((byte)'t');
            WriteBoolean(value);
        }

        public virtual void WriteFieldValue(sbyte value)
        {
            BaseWriter.Write((byte)'b');
            WriteSByte(value);
        }

        public virtual void WriteFieldValue(byte value)
        {
            BaseWriter.Write((byte)'B');
            WriteByte(value);
        }

        public virtual void WriteFieldValue(short value)
        {
            BaseWriter.Write((byte)'U');
            WriteInt16(value);
        }

        public virtual void WriteFieldValue(ushort value)
        {
            BaseWriter.Write((byte)'u');
            WriteUInt16(value);
        }

        public virtual void WriteFieldValue(int value)
        {
            BaseWriter.Write((byte)'I');
            WriteInt32(value);
        }

        public virtual void WriteFieldValue(uint value)
        {
            BaseWriter.Write((byte)'i');
            WriteUInt32(value);
        }

        public virtual void WriteFieldValue(long value)
        {
            BaseWriter.Write((byte)'L');
            WriteInt64(value);
        }

        public virtual void WriteFieldValue(ulong value)
        {
            BaseWriter.Write((byte)'l');
            WriteUInt64(value);
        }

        public virtual void WriteFieldValue(float value)
        {
            BaseWriter.Write((byte)'f');
            WriteSingle(value);
        }

        public virtual void WriteFieldValue(double value)
        {
            BaseWriter.Write((byte)'d');
            WriteDouble(value);
        }

        public virtual void WriteFieldValue(decimal value)
        {
            BaseWriter.Write((byte)'D');
            WriteDecimal(value);
        }

        public virtual void WriteFieldValue(string value)
        {
            BaseWriter.Write((byte)'s');
            WriteString(value);
        }

        public virtual void WriteFieldValue(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            BaseWriter.Write((byte)'S');
            WriteBytes(value);
        }

        public virtual void WriteFieldValue(DateTime value)
        {
            if (value.ToUniversalTime() < AmqpDefinitions.UnixEpoch)
                throw new ArgumentException($"The time value '{value}' is out of AMQP bounds.", nameof(value));

            BaseWriter.Write((byte)'T');
            WriteTime(value);
        }

        public virtual void WriteFieldValue(IEnumerable<KeyValuePair<string, object>> value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            BaseWriter.Write((byte)'F');
            WriteTable(value);
        }

        public virtual void WriteFieldValue(IEnumerable value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            BaseWriter.Write((byte)'A');
            WriteArray(value);
        }

        public virtual void WriteNullField()
            => BaseWriter.Write((byte)'V');

        public virtual void WriteFieldValue(object value)
        {
            switch (value)
            {
                case bool typedValue: // boolean: 0 = FALSE, else TRUE
                    WriteFieldValue(typedValue);
                    break;
                case sbyte typedValue: // short-short-int
                    WriteFieldValue(typedValue);
                    break;
                case byte typedValue: // short-short-uint
                    WriteFieldValue(typedValue);
                    break;
                case short typedValue: // short-int
                    WriteFieldValue(typedValue);
                    break;
                case ushort typedValue: // short-uint
                    WriteFieldValue(typedValue);
                    break;
                case int typedValue: // long-int
                    WriteFieldValue(typedValue);
                    break;
                case uint typedValue: // long-uint
                    WriteFieldValue(typedValue);
                    break;
                case long typedValue: // long-long-int
                    WriteFieldValue(typedValue);
                    break;
                case ulong typedValue: // long-long-uint
                    WriteFieldValue(typedValue);
                    break;
                case float typedValue: //float: IEEE-754 
                    WriteFieldValue(typedValue);
                    break;
                case double typedValue: // double: rfc1832 XDR double
                    WriteFieldValue(typedValue);
                    break;
                case decimal typedValue: // decimal-value: scale long-int
                    WriteFieldValue(typedValue);
                    break;
                case string typedValue: // short-string: OCTET *string-char: length + content
                    WriteFieldValue(typedValue);
                    break;
                case byte[] typedValue: // long-string: long-uint *OCTET: length + content
                    WriteFieldValue(typedValue);
                    break;
                case DateTime typedValue: // timestamp: long-long-uint: 64-bit POSIX 
                    WriteFieldValue(typedValue);
                    break;
                case IEnumerable<KeyValuePair<string, object>> typedValue: // field-table: long-uint *(field-name field-value)
                    WriteFieldValue(typedValue);
                    break;
                case IEnumerable typedValue: // field-array: long-int *field-value: array of values
                    WriteFieldValue(typedValue);
                    break;
                case null: // no field
                    WriteNullField();
                    break;

                default:
                    throw new ArgumentException($"The value type '{value.GetType()}' is not supported.", nameof(value));
            }
        }
    }
}
