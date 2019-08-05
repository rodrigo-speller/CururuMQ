// Copyright 2019 Rodrigo Speller. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.
// See LICENSE.txt in the solution directory for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace CururuMQ.Networking.Amqp091.Test
{
    public class Reading_values_using_AmqpValueReader
    {
        [Theory]
        [InlineData(new byte[] { (byte)'t', 0 })]
        public void Field_value_boolean_false(byte[] data)
        {
            var stream = new MemoryStream(data);
            var reader = new AmqpValueReader(stream, false);

            var value = reader.ReadFieldValue();

            Assert.IsType<bool>(value);

            Assert.Equal(false, value);
        }

        [Theory]
        [InlineData(new byte[] { (byte)'t', 1 })]
        [InlineData(new byte[] { (byte)'t', 2 })]
        [InlineData(new byte[] { (byte)'t', 127 })]
        [InlineData(new byte[] { (byte)'t', 128 })]
        [InlineData(new byte[] { (byte)'t', 255 })]
        public void Field_value_boolean_true(byte[] data)
        {
            var stream = new MemoryStream(data);
            var reader = new AmqpValueReader(stream, false);

            var value = reader.ReadFieldValue();

            Assert.IsType<bool>(value);

            Assert.Equal(true, value);
        }
        
        [Theory]
        [InlineData((float)1 / 2, new object[] { 'f', 0x3F, 0x00, 0x00, 0x00 })]
        [InlineData((float)1 / 3, new object[] { 'f', 0x3E, 0xAA, 0xAA, 0xAB })]
        [InlineData((float)1 / 4, new object[] { 'f', 0x3E, 0x80, 0x00, 0x00 })]
        [InlineData((float)1 / 5, new object[] { 'f', 0x3E, 0x4C, 0xCC, 0xCD })]
        [InlineData((float)1 / 65537, new object[] { 'f', 0x37, 0x7F, 0xFF, 0x00 })]
        [InlineData((float)-1 / 2, new object[] { 'f', 0xBF, 0x00, 0x00, 0x00 })]
        [InlineData((float)-1 / 3, new object[] { 'f', 0xBE, 0xAA, 0xAA, 0xAB })]
        [InlineData((float)-1 / 4, new object[] { 'f', 0xBE, 0x80, 0x00, 0x00 })]
        [InlineData((float)-1 / 5, new object[] { 'f', 0xBE, 0x4C, 0xCC, 0xCD })]
        [InlineData((float)-1 / 65537, new object[] { 'f', 0xB7, 0x7F, 0xFF, 0x00 })]
        public void Field_value_single(float expected, object[] data)
        {
            var bytes = data
                .Select(x => Convert.ToByte(x))
                .ToArray()
                ;

            var stream = new MemoryStream(bytes);
            var reader = new AmqpValueReader(stream, false);

            var value = reader.ReadFieldValue();

            Assert.IsType<float>(value);

            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData((double)1 / 2, new object[] { 'd', 0x3F, 0xE0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData((double)1 / 3, new object[] { 'd', 0x3F, 0xD5, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55 })]
        [InlineData((double)1 / 4, new object[] { 'd', 0x3F, 0xD0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData((double)1 / 5, new object[] { 'd', 0x3F, 0xC9, 0x99, 0x99, 0x99, 0x99, 0x99, 0x9A })]
        [InlineData((double)1 / 65537, new object[] { 'd', 0x3E, 0xEF, 0xFF, 0xE0, 0x00, 0x1F, 0xFF, 0xE0 })]
        [InlineData((double)-1 / 2, new object[] { 'd', 0xBF, 0xE0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData((double)-1 / 3, new object[] { 'd', 0xBF, 0xD5, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55 })]
        [InlineData((double)-1 / 4, new object[] { 'd', 0xBF, 0xD0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData((double)-1 / 5, new object[] { 'd', 0xBF, 0xC9, 0x99, 0x99, 0x99, 0x99, 0x99, 0x9A })]
        [InlineData((double)-1 / 65537, new object[] { 'd', 0xBE, 0xEF, 0xFF, 0xE0, 0x00, 0x1F, 0xFF, 0xE0 })]
        public void Field_value_double(double expected, object[] data)
        {
            var bytes = data
                .Select(x => Convert.ToByte(x))
                .ToArray()
                ;

            var stream = new MemoryStream(bytes);
            var reader = new AmqpValueReader(stream, false);

            var value = reader.ReadFieldValue();

            Assert.IsType<double>(value);

            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("", new object[] { 's', 0 })]
        [InlineData("1", new object[] { 's', 1, '1' })]
        [InlineData("12", new object[] { 's', 2, '1', '2' })]
        [InlineData("123", new object[] { 's', 3, '1', '2', '3' })]
        [InlineData("1234", new object[] { 's', 4, '1', '2', '3', '4' })]
        public void Field_value_string(string expected, object[] data)
        {
            var bytes = data
                .Select(x => Convert.ToByte(x))
                .ToArray()
                ;

            var stream = new MemoryStream(bytes);
            var reader = new AmqpValueReader(stream, false);

            Assert.Equal(expected, reader.ReadFieldValue());
        }

        [Theory]
        [InlineData("ش", new object[] { 's', 2, 0xD8, 0xB4 })]
        [InlineData("ص", new object[] { 's', 2, 0xD8, 0xB5 })]
        [InlineData("ꩠ", new object[] { 's', 3, 0xEA, 0xA9, 0xA0 })]
        [InlineData("😄", new object[] { 's', 4, 0xF0, 0x9F, 0x98, 0x84 })]
        [InlineData("شصꩠ😄", new object[] { 's', 11, 0xD8, 0xB4, 0xD8, 0xB5, 0xEA, 0xA9, 0xA0, 0xF0, 0x9F, 0x98, 0x84 })]
        public void Field_value_string_with_multibyte_chars(string expected, object[] data)
        {
            var bytes = data
                .Select(x => Convert.ToByte(x))
                .ToArray()
                ;

            var stream = new MemoryStream(bytes);
            var reader = new AmqpValueReader(stream, false);

            Assert.Equal(expected, reader.ReadFieldValue());
        }

        [Theory]
        [InlineData(new int[] { 1970, 01, 01, 00, 00, 00 }, new object[] { 'T', 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(new int[] { 1970, 01, 01, 00, 00, 01 }, new object[] { 'T', 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 })]
        [InlineData(new int[] { 9999, 12, 31, 23, 59, 59 }, new object[] { 'T', 0x00, 0x00, 0x00, 0x3a, 0xff, 0xf4, 0x41, 0x7F })]
        public void Field_value_time(int[] expected, object[] data)
        {
            var bytes = data
                .Select(x => Convert.ToByte(x))
                .ToArray()
                ;

            var stream = new MemoryStream(bytes);
            var reader = new AmqpValueReader(stream, false);

            var value = reader.ReadFieldValue();

            Assert.IsType<DateTime>(value);

            var time = (DateTime)value;

            var expectedTime = new DateTime(
                expected[0],
                expected[1],
                expected[2],
                expected[3],
                expected[4],
                expected[5],
                DateTimeKind.Utc
            );

            Assert.Equal(expectedTime, time);
            Assert.Equal(DateTimeKind.Utc, time.Kind);
        }

        [Fact]
        public void Field_value_array()
        {
            var expectedDouble = (double)-1 / 65537;
            var expectedString = "شصꩠ😄";
            var expectedTime = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            var expected = new object[]
            {
                expectedDouble,
                expectedString,
                expectedTime
            };

            var data = new object[] {
                'A', 0x00, 0x00, 0x00, 31,
                    'd', 0xBE, 0xEF, 0xFF, 0xE0, 0x00, 0x1F, 0xFF, 0xE0,
                    's', 11, 0xD8, 0xB4, 0xD8, 0xB5, 0xEA, 0xA9, 0xA0, 0xF0, 0x9F, 0x98, 0x84,
                    'T', 0x00, 0x00, 0x00, 0x3a, 0xff, 0xf4, 0x41, 0x7F
            };

            var bytes = data
                .Select(x => Convert.ToByte(x))
                .ToArray()
                ;

            var stream = new MemoryStream(bytes);
            var reader = new AmqpValueReader(stream, false);

            var value = reader.ReadFieldValue();

            Assert.Equal(expected, value);
        }

        [Fact]
        public void Field_value_nested_array()
        {
            var expectedDouble = (double)-1 / 65537;
            var expectedString = "شصꩠ😄";
            var expectedTime = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            var expected = new object[]
            {
                expectedDouble,
                expectedString,
                expectedTime,
                new object[]
                {
                    expectedDouble,
                    expectedString,
                    expectedTime,
                    new object[]
                    {
                        expectedDouble,
                        expectedString,
                        expectedTime,
                    }
                }
            };

            var data = new object[] {
                'A', 0x00, 0x00, 0x00, 31 + 36 + 36,
                    'd', 0xBE, 0xEF, 0xFF, 0xE0, 0x00, 0x1F, 0xFF, 0xE0,
                    's', 11, 0xD8, 0xB4, 0xD8, 0xB5, 0xEA, 0xA9, 0xA0, 0xF0, 0x9F, 0x98, 0x84,
                    'T', 0x00, 0x00, 0x00, 0x3a, 0xff, 0xf4, 0x41, 0x7F,
                    'A', 0x00, 0x00, 0x00, 31 + 36,
                        'd', 0xBE, 0xEF, 0xFF, 0xE0, 0x00, 0x1F, 0xFF, 0xE0,
                        's', 11, 0xD8, 0xB4, 0xD8, 0xB5, 0xEA, 0xA9, 0xA0, 0xF0, 0x9F, 0x98, 0x84,
                        'T', 0x00, 0x00, 0x00, 0x3a, 0xff, 0xf4, 0x41, 0x7F,
                        'A', 0x00, 0x00, 0x00, 31,
                            'd', 0xBE, 0xEF, 0xFF, 0xE0, 0x00, 0x1F, 0xFF, 0xE0,
                            's', 11, 0xD8, 0xB4, 0xD8, 0xB5, 0xEA, 0xA9, 0xA0, 0xF0, 0x9F, 0x98, 0x84,
                            'T', 0x00, 0x00, 0x00, 0x3a, 0xff, 0xf4, 0x41, 0x7F
            };

            var bytes = data
                .Select(x => Convert.ToByte(x))
                .ToArray()
                ;

            var stream = new MemoryStream(bytes);
            var reader = new AmqpValueReader(stream, false);

            var value = reader.ReadFieldValue();
            Assert.Equal(expected, value);
        }

        [Fact]
        public void Field_value_table()
        {
            var expectedDouble = (double)-1 / 65537;
            var expectedString = "شصꩠ😄";
            var expectedTime = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            var expected = new Dictionary<string, object>()
            {
                { "double", expectedDouble },
                { "string", expectedString },
                { "time", expectedTime }
            };

            var data = new object[] {
                'F', 0x00, 0x00, 0x00, 50,
                    6, 'd', 'o', 'u', 'b', 'l', 'e',
                    'd', 0xBE, 0xEF, 0xFF, 0xE0, 0x00, 0x1F, 0xFF, 0xE0,
                    6, 's', 't', 'r', 'i', 'n', 'g',
                    's', 11, 0xD8, 0xB4, 0xD8, 0xB5, 0xEA, 0xA9, 0xA0, 0xF0, 0x9F, 0x98, 0x84,
                    4, 't', 'i', 'm', 'e',
                    'T', 0x00, 0x00, 0x00, 0x3a, 0xff, 0xf4, 0x41, 0x7F
            };

            var bytes = data
                .Select(x => Convert.ToByte(x))
                .ToArray()
                ;

            var stream = new MemoryStream(bytes);
            var reader = new AmqpValueReader(stream, false);

            var value = reader.ReadFieldValue();

            Assert.Equal(expected, value);
        }

        [Fact]
        public void Field_value_nested_table()
        {
            var expectedDouble = (double)-1 / 65537;
            var expectedString = "شصꩠ😄";
            var expectedTime = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            var expected = new Dictionary<string, object>()
            {
                { "double", expectedDouble },
                { "string", expectedString },
                { "time", expectedTime },
                { "nested",  new Dictionary<string, object>()
                    {
                        { "double", expectedDouble },
                        { "string", expectedString },
                        { "time", expectedTime }
                    }
                }
            };

            var data = new object[] {
                'F', 0x00, 0x00, 0x00, 112,
                    6, 'd', 'o', 'u', 'b', 'l', 'e',
                    'd', 0xBE, 0xEF, 0xFF, 0xE0, 0x00, 0x1F, 0xFF, 0xE0,
                    6, 's', 't', 'r', 'i', 'n', 'g',
                    's', 11, 0xD8, 0xB4, 0xD8, 0xB5, 0xEA, 0xA9, 0xA0, 0xF0, 0x9F, 0x98, 0x84,
                    4, 't', 'i', 'm', 'e',
                    'T', 0x00, 0x00, 0x00, 0x3a, 0xff, 0xf4, 0x41, 0x7F,
                    6, 'n', 'e', 's', 't', 'e', 'd',
                    'F', 0x00, 0x00, 0x00, 50,
                        6, 'd', 'o', 'u', 'b', 'l', 'e',
                        'd', 0xBE, 0xEF, 0xFF, 0xE0, 0x00, 0x1F, 0xFF, 0xE0,
                        6, 's', 't', 'r', 'i', 'n', 'g',
                        's', 11, 0xD8, 0xB4, 0xD8, 0xB5, 0xEA, 0xA9, 0xA0, 0xF0, 0x9F, 0x98, 0x84,
                        4, 't', 'i', 'm', 'e',
                        'T', 0x00, 0x00, 0x00, 0x3a, 0xff, 0xf4, 0x41, 0x7F
            };

            var bytes = data
                .Select(x => Convert.ToByte(x))
                .ToArray()
                ;

            var stream = new MemoryStream(bytes);
            var reader = new AmqpValueReader(stream, false);

            var value = reader.ReadFieldValue();

            Assert.Equal(expected, value);
        }
    }
}
