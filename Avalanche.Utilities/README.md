<b>Avalanche.Utilities</b> contains utilities for various coding facets, 
[[git]](https://github.com/tagcode/Avalanche.Utilities/Avalanche.Utilities/), 
[[www]](https://avalanche.fi/Avalanche.Core/Avalanche.Utilities/docs/), 
[[licensing]](https://avalanche.fi/Avalanche.Core/license/index.html).

Add package reference to .csproj.
```xml
<PropertyGroup>
    <RestoreAdditionalProjectSources>https://avalanche.fi/Avalanche.Core/nupkg/index.json</RestoreAdditionalProjectSources>
</PropertyGroup>
<ItemGroup>
    <PackageReference Include="Avalanche.Utilities"/>
</ItemGroup>
```