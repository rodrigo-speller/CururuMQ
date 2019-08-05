// Copyright 2019 Rodrigo Speller. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.
// See LICENSE.txt in the solution directory for license information.

using System;

namespace CururuMQ.Networking.Amqp091
{
    internal class ReverseOrderBitConverter : IIntegerBitConverter, IFloatBitConverter
    {
        public static readonly ReverseOrderBitConverter Instance = new ReverseOrderBitConverter();

        private ReverseOrderBitConverter() { }

        private static byte[] Reverse(byte[] value)
        {
            Array.Reverse(value);
            return value;
        }

        public byte[] GetBytes(short value)
            => Reverse(BitConverter.GetBytes(value));

        public byte[] GetBytes(int value)
            => Reverse(BitConverter.GetBytes(value));

        public byte[] GetBytes(long value)
            => Reverse(BitConverter.GetBytes(value));

        public byte[] GetBytes(ushort value)
            => Reverse(BitConverter.GetBytes(value));

        public byte[] GetBytes(uint value)
            => Reverse(BitConverter.GetBytes(value));

        public byte[] GetBytes(ulong value)
            => Reverse(BitConverter.GetBytes(value));

        public byte[] GetBytes(double value)
            => Reverse(BitConverter.GetBytes(value));

        public byte[] GetBytes(float value)
            => Reverse(BitConverter.GetBytes(value));

        public double ToDouble(byte[] value)
            => BitConverter.ToDouble(Reverse(value), 0);

        public short ToInt16(byte[] value)
            => BitConverter.ToInt16(Reverse(value), 0);

        public int ToInt32(byte[] value)
            => BitConverter.ToInt32(Reverse(value), 0);

        public long ToInt64(byte[] value)
            => BitConverter.ToInt64(Reverse(value), 0);

        public float ToSingle(byte[] value)
            => BitConverter.ToSingle(Reverse(value), 0);

        public ushort ToUInt16(byte[] value)
            => BitConverter.ToUInt16(Reverse(value), 0);

        public uint ToUInt32(byte[] value)
            => BitConverter.ToUInt32(Reverse(value), 0);

        public ulong ToUInt64(byte[] value)
            => BitConverter.ToUInt64(Reverse(value), 0);
    }
}
