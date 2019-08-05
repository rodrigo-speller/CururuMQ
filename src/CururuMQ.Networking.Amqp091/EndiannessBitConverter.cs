// Copyright 2019 Rodrigo Speller. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.
// See LICENSE.txt in the solution directory for license information.

using System;

namespace CururuMQ.Networking.Amqp091
{
    public partial class EndiannessBitConverter
    {
        public static readonly EndiannessBitConverter BigEndian;
        public static readonly EndiannessBitConverter LittleEndian;

        static EndiannessBitConverter()
        {
            if (BitConverter.IsLittleEndian)
            {

                BigEndian = new EndiannessBitConverter(
                    ReverseOrderBitConverter.Instance,
                    ReverseOrderBitConverter.Instance
                );

                LittleEndian = new EndiannessBitConverter(
                    PreserveOrderBitConverter.Instance,
                    PreserveOrderBitConverter.Instance
                );
            }
            else
            {
                BigEndian = new EndiannessBitConverter(
                    PreserveOrderBitConverter.Instance,
                    PreserveOrderBitConverter.Instance
                );

                LittleEndian = new EndiannessBitConverter(
                    ReverseOrderBitConverter.Instance,
                    ReverseOrderBitConverter.Instance
                );
            }
        }

        public EndiannessBitConverter(IIntegerBitConverter integerConverter, IFloatBitConverter floatConverter)
        {
            IntegerConverter = integerConverter
                ?? throw new ArgumentNullException(nameof(integerConverter));

            FloatConverter = floatConverter
                ?? throw new ArgumentNullException(nameof(floatConverter));
        }

        public IIntegerBitConverter IntegerConverter { get; }
        public IFloatBitConverter FloatConverter { get; }
    }
}
