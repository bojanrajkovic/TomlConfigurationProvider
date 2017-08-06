# CodeRinseRepeat.Configuration.TomlConfigurationProvider

A configuration provider for Microsoft.Extensions.Configuration that
reads [TOML][toml] files. Targets .NET Standard 2.0 in order to
consume [Nett][nett]. Includes tests that load the TOML sample from the TOML
repository and ensures that the tables/keys/nesting are converted to the proper
configuration keys.

[toml]: https://github.com/toml-lang/toml
[nett]: https://github.com/paiden/Nett
