# CodeRinseRepeat.Configuration.TomlConfigurationProvider

[![Build status](https://teamcity.coderinserepeat.com/app/rest/builds/buildType:(id:TomlConfigurationProvider_Build)/statusIcon)](https://teamcity.coderinserepeat.com/viewType.html?buildTypeId=TomlConfigurationProvider_Build&guest=1)
[![NuGet](https://img.shields.io/nuget/v/CodeRinseRepeat.Configuration.TomlConfigurationProvider.svg?style=flat)](https://nuget.org/packages/CodeRinseRepeat.Configuration.TomlConfigurationProvider)

A configuration provider for Microsoft.Extensions.Configuration that
reads [TOML][toml] files. Targets .NET Standard 2.0 in order to
consume [Nett][nett]. Includes tests that load the TOML sample from the TOML
repository and ensures that the tables/keys/nesting are converted to the proper
configuration keys.

[toml]: https://github.com/toml-lang/toml
[nett]: https://github.com/paiden/Nett
