using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace CodeRinseRepeat.Configuration.TomlConfigurationProvider
{
    public sealed class TomlConfigurationProvider : FileConfigurationProvider
    {
        public TomlConfigurationProvider(TomlConfigurationSource source) : base(source)
        {
        }

        public override void Load(Stream stream)
        {
            var parser = new TomlConfigurationFileParser();
            Data = parser.Parse(stream);
        }
    }
}
