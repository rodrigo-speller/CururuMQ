// Copyright 2019 Rodrigo Speller. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.
// See LICENSE.txt in the solution directory for license information.

namespace CururuMQ.Networking.Amqp091
{
    public interface IIntegerBitConverter
    {
        byte[] GetBytes(short value);
        byte[] GetBytes(int value);
        byte[] GetBytes(long value);
        byte[] GetBytes(ushort value);
        byte[] GetBytes(uint value);
        byte[] GetBytes(ulong value);

        short ToInt16(byte[] value);
        int ToInt32(byte[] value);
        long ToInt64(byte[] value);
        ushort ToUInt16(byte[] value);
        uint ToUInt32(byte[] value);
        ulong ToUInt64(byte[] value);
    }
}
