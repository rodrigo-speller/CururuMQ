// Copyright 2019 Rodrigo Speller. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.
// See LICENSE.txt in the solution directory for license information.

namespace CururuMQ.Networking.Amqp091
{
    public interface IFloatBitConverter
    {
        byte[] GetBytes(double value);
        byte[] GetBytes(float value);

        double ToDouble(byte[] value);
        float ToSingle(byte[] value);
    }
}
