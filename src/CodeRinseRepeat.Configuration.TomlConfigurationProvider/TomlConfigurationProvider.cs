using System.IO;

using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace CodeRinseRepeat.Configuration.TomlConfigurationProvider
{
    [PublicAPI]
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
