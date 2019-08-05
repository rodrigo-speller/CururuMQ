// Copyright 2019 Rodrigo Speller. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.
// See LICENSE.txt in the solution directory for license information.

using System;
using System.Text;

namespace CururuMQ.Networking.Amqp091
{
    internal class AmqpDefinitions
    {
        public static readonly Encoding UTF8 = Encoding.UTF8;
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static readonly IIntegerBitConverter IntegerConverter = EndiannessBitConverter.BigEndian.IntegerConverter;
        public static readonly IFloatBitConverter FloatConverter = EndiannessBitConverter.BigEndian.FloatConverter;
    }
}
