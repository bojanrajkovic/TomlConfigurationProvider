﻿using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace CodeRinseRepeat.Configuration.TomlConfigurationProvider
{
    [PublicAPI]
    public sealed class TomlConfigurationSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            FileProvider = FileProvider ?? builder.GetFileProvider();
            return new TomlConfigurationProvider(this);
        }
    }
}
