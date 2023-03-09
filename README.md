# **Snowplow**

**A client utility for deterministic snowflake testing**

## **How to install**

You must use a PAT with `package:read` in combination with your username in order to authenticate towards your GitHub Packages Nuget feed. \
Secondly you add a `nuget.config` file adding your github organization as a source for Nuget packages.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="github" value="https://nuget.pkg.github.com/<YOUR_ORGANIZATION_NAME>/index.json"/>
  </packageSources>
</configuration>
```

Adding this in combination with searching for packages from either **Visual Studio** or **Rider** will prompt your for username/password regardless of the package being public.
The reason for this is that you're accessing the GitHub organisations feed in order to access the package; this endpoint is private.

