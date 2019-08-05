// Copyright 2019 Rodrigo Speller. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.
// See LICENSE.txt in the solution directory for license information.

using System;

namespace CururuMQ.Networking.Amqp091
{
    internal class PreserveOrderBitConverter : IIntegerBitConverter, IFloatBitConverter
    {
        public static readonly PreserveOrderBitConverter Instance = new PreserveOrderBitConverter();

        private PreserveOrderBitConverter() { }

        public byte[] GetBytes(short value)
            => BitConverter.GetBytes(value);

        public byte[] GetBytes(int value)
            => BitConverter.GetBytes(value);

        public byte[] GetBytes(long value)
            => BitConverter.GetBytes(value);

        public byte[] GetBytes(ushort value)
            => BitConverter.GetBytes(value);

        public byte[] GetBytes(uint value)
            => BitConverter.GetBytes(value);

        public byte[] GetBytes(ulong value)
            => BitConverter.GetBytes(value);

        public byte[] GetBytes(double value)
            => BitConverter.GetBytes(value);

        public byte[] GetBytes(float value)
            => BitConverter.GetBytes(value);

        public double ToDouble(byte[] value)
            => BitConverter.ToDouble(value, 0);

        public short ToInt16(byte[] value)
            => BitConverter.ToInt16(value, 0);

        public int ToInt32(byte[] value)
            => BitConverter.ToInt32(value, 0);

        public long ToInt64(byte[] value)
            => BitConverter.ToInt64(value, 0);

        public float ToSingle(byte[] value)
            => BitConverter.ToSingle(value, 0);

        public ushort ToUInt16(byte[] value)
            => BitConverter.ToUInt16(value, 0);

        public uint ToUInt32(byte[] value)
            => BitConverter.ToUInt32(value, 0);

        public ulong ToUInt64(byte[] value)
            => BitConverter.ToUInt64(value, 0);
    }
}
