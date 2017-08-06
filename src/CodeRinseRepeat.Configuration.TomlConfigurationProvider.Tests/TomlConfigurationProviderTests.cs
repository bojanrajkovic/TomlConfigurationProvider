using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace CodeRinseRepeat.Configuration.TomlConfigurationProvider.Tests
{
    public class TomlConfigurationProviderTests
    {
        IConfiguration configuration;
        readonly ITestOutputHelper output;

        public TomlConfigurationProviderTests(ITestOutputHelper output)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddTomlFile("sample.toml");
            configuration = configurationBuilder.Build();
            this.output = output;
        }

        [Fact]
        public void Can_read_basic_table() =>
            AssertKey("table:key", "value");

        [Fact]
        public void Can_read_sub_table() =>
            AssertKey("table:subtable:key", "another value");

        [Theory]
        [InlineData("table:inline:name:first", "Tom")]
        [InlineData("table:inline:name:last", "Preston-Werner")]
        [InlineData("table:inline:point:x", "1")]
        [InlineData("table:inline:point:y", "2")]
        public void Can_read_inline_table(string key, string value) =>
            AssertKey(key, value);

        [Theory]
        [InlineData("string:basic:basic", "I'm a string. \"You can quote me\". Name\tJos\u00E9\nLocation\tSF.")]
        public void Can_read_basic_strings(string key, string value) =>
            AssertKey(key, value);

        [Theory]
        [InlineData("string:multiline:key1", "One\nTwo")]
        [InlineData("string:multiline:key3", "One\r\nTwo")]
        // This case seems to be a bug in Nett where it interprets the triply
        // quoted string as if it was non-escaping.
        [InlineData("string:multiline:key2", "One\\nTwo")]
        [InlineData("string:multiline:continued:key1", "The quick brown fox jumps over the lazy dog.")]
        [InlineData("string:multiline:continued:key2", "The quick brown fox jumps over the lazy dog.")]
        [InlineData("string:multiline:continued:key3", "The quick brown fox jumps over the lazy dog.")]
        public void Can_read_multiline_strings(string key, string value) =>
            AssertKey(key, value);

        [Theory]
        [InlineData("string:literal:winpath", "C:\\Users\\nodejs\\templates")]
        [InlineData("string:literal:winpath2", "\\\\ServerX\\admin$\\system32\\")]
        [InlineData("string:literal:quoted", "Tom \"Dubs\" Preston-Werner")]
        [InlineData("string:literal:regex", "<\\i\\c*\\s*>")]
        public void Can_read_literal_strings(string key, string value) =>
            AssertKey(key, value);

        [Theory]
        [InlineData("string:literal:multiline:regex2", "I [dw]on't need \\d{2} apples")]
        // This case seems to expose a bug in Nett--the first newline isn't trimmed.
        [InlineData("string:literal:multiline:lines", "The first newline is\r\ntrimmed in raw strings.\r\n   All other whitespace\r\n   is preserved.\r\n")]
        public void Can_read_multiline_literal_strings(string key, string value) =>
            AssertKey(key, value);

        [Theory]
        [InlineData("integer:key1", "99")]
        [InlineData("integer:key2", "42")]
        [InlineData("integer:key3", "0")]
        [InlineData("integer:key4", "-17")]
        public void Can_read_integers(string key, string value) =>
            AssertKey(key, value);

        [Theory]
        [InlineData("integer:underscores:key1", "1000")]
        [InlineData("integer:underscores:key2", "5349221")]
        [InlineData("integer:underscores:key3", "12345")]
        public void Can_read_integers_with_underscores(string key, string value) =>
            AssertKey(key, value);

        [Theory]
        [InlineData("float:fractional:key1", "1")]
        [InlineData("float:fractional:key2", "3.1415")]
        [InlineData("float:fractional:key3", "-0.01")]
        public void Can_read_floats(string key, string value) =>
            AssertKey(key, value);

        [Theory]
        [InlineData("float:exponent:key1", "5E+22")]
        [InlineData("float:exponent:key2", "1000000")]
        [InlineData("float:exponent:key3", "-0.02")]
        public void Can_read_exponent_floats(string key, string value) =>
            AssertKey(key, value);

        [Fact]
        public void Can_read_mixed_floats() =>
            AssertKey("float:both:key", "6.626E-34");

        // TODO: Investigate why this is getting rounded.
        [Fact]
        public void Can_read_floats_with_underscores() =>
            AssertKey("float:underscores:key1", "9224617.44599123");

        [Fact(Skip = "This is a bignum, Nett doesn't parse those.")]
        public void Can_read_exponent_floats_with_underscores() =>
            AssertKey("float:underscores:key2", "1e1000");

        [Theory]
        [InlineData("boolean:True", "True")]
        [InlineData("boolean:False", "False")]
        public void Can_read_booleans(string key, string value) =>
            AssertKey(key, value);

        [Theory]
        [InlineData("datetime:key1", "05/27/1979 07:32:00 +00:00")]
        [InlineData("datetime:key2", "05/27/1979 00:32:00 -07:00")]
        [InlineData("datetime:key3", "05/27/1979 00:32:00 -07:00")]
        public void Can_read_datetimes(string key, string value) =>
            AssertKey(key, value);

        [Theory]
        [InlineData("array:key1", new[] { "1", "2", "3" })]
        [InlineData("array:key2", new[] { "red", "yellow", "green" })]
        [InlineData("array:key5", new[] { "1", "2", "3" })]
        [InlineData("array:key6", new[] { "1", "2" })]
        public void Can_read_arrays(string key, string[] values) =>
            AssertArrayKey(key, values);

        [Theory]
        [MemberData(nameof(NestedArrayData))]
        public void Can_read_nested_arrays(string key, string[][] values) =>
            AssertNestedArrayKey(key, values);

        [Theory]
        [InlineData("products:0:name", "Hammer")]
        [InlineData("products:0:sku", "738594937")]
        [InlineData("products:2:name", "Nail")]
        [InlineData("products:2:sku", "284758393")]
        [InlineData("products:2:color", "gray")]
        public void Can_read_array_of_tables(string key, string value) =>
            AssertKey(key, value);

        [Theory]
        [InlineData("fruit:0:name", "apple")]
        [InlineData("fruit:0:physical:color", "red")]
        [InlineData("fruit:0:physical:shape", "round")]
        [InlineData("fruit:0:variety:0:name", "red delicious")]
        [InlineData("fruit:0:variety:1:name", "granny smith")]
        [InlineData("fruit:1:name", "banana")]
        [InlineData("fruit:1:variety:0:name", "plantain")]
        public void Can_read_nested_arrays_of_tables(string key, string value) =>
            AssertKey(key, value);

        [Fact]
        public void Can_read_timespans() =>
            AssertKey("timespan:key1", "01:02:03");

        void AssertNestedArrayKey(string key, string[][] values) =>
            values.ForEach((i, v) => AssertArrayKey($"{key}:{i}", v));

        void AssertArrayKey(string key, string[] values) => 
            values.ForEach((i, v) => AssertKey($"{key}:{i}", v));

        void AssertKey(string key, string value) => Assert.Equal(value, configuration[key]);

        public static TheoryData<string, string[][]> NestedArrayData = new TheoryData<string, string[][]> {
            { "array:key3", new[] { new[] { "1", "2" }, new[] { "3", "4", "5" } } },
            { "array:key4", new[] { new[] { "1", "2" }, new[] { "a", "b", "c" } } }
        };
    }

    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IList<T> collection, Action<int, T> callback)
        {
            for (var i = 0; i < collection.Count; i++)
                callback(i, collection[i]);
        }
    }
}
