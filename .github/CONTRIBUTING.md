# **Contributing**

Please add tests for any feature you want added.

## **CI-setup**

Beware that GitHub does not permit one workflow to trigger another, as per [this](). \
To mediate this the [release.yaml](./workflows/release.yaml) is run of the permissions configured for a [personal access token (PAT)](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token). \
The time to live (TTL) of these are 12 months -> should it stop working the PAT must be renewed.

Then you must add the following snippet to your package `.csproj`-file:

```xml
<PropertyGroup>
    <PackageId>NuGet.Workflow</PackageId>
    <RepositoryType>git</RepositoryType>
    <RepositoryUrl>https://github.com/**YOUR_ORG**/**YOUR_REPO**</RepositoryUrl>
</PropertyGroup>
```

Lastly you must alter the [package.yaml](./workflows/package.yaml) `push` step to reference your package name:

```yaml
    - name: Push
      run: dotnet nuget push <YOUR_PACKAGE_NAME>.${VERSION}.nupkg --source https://nuget.pkg.github.com/<YOUR_ORG>/index.json --api-key ${GITHUB_TOKEN}
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

## **CI-sauce**

- https://acraven.medium.com/a-nuget-package-workflow-using-github-actions-7da8c6557863