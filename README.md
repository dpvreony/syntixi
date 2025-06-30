# Syntixi

## Mission Statement

* To provide a mechanism to embed Syncfusion license keys into applications as part of the build process.

## Introduction

Syntixi is a Roslyn Source Generator that picks up the license key in a way that:
* allows the key to not be stored in source control
* allows the embedding without the direct modification of the source code, again preventing the risk of the key accidentally ending up in source control.
  * Syncfusion has a couple of batch scripts that can be embedded into your csproj but these do alter the source code and do it a slightly blind fashion.

## Getting Started

You will need a Syncfusion license key

### 1. Create an application (such as WPF)
### 2. Add a nuget package reference to "Syntixi.Attributes" and "Syntixi.SourceGenerator" in your project

```xml
    <PackageReference Include="Syntixi.Attributes" Version="1.0.18" />
    <PackageReference Include="Syntixi.SourceGenerator" Version="1.0.18">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
```

This will add an attributes package and the source generator package. NOTE: if you don't include the build assets for the SourceGenerator it will not make the props file available that allows Roslyn to access the SyncFusion environment variable \ secret.

### 3. Add the following initialisation for the Syncfusion license manager

```cs
   Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SYNCFUSION_LICENSE_KEY);
```

### 4. Mark the class hosting the logic as partial and add the attribute to the class

```cs
    /// <summary>
    /// WPF Application entry point.
    /// </summary>
    [Syntixi.Attributes.EmbedSyncfusionLicenseKey]
    public partial class App : Application
    {
      // YOUR CODE HERE
    }
```

### 5. Add the license key to a CI secret or your local environment variables.

???

## longer examples

### Full example that will fail if the license key isn't present

The source generator is written to set a Diagnostic Error if the attribute is included but the license key isn't present in the environment.

```cs
    /// <summary>
    /// WPF Application entry point.
    /// </summary>
    [Syntixi.Attributes.EmbedSyncfusionLicenseKey]
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SYNCFUSION_LICENSE_KEY);
        }
    }
```

### Full example that will not include the Syncfusion license manager if they license key isn't present.

The source generator has a .props file includes the compiler directive `SYNTIXI_SYNCFUSION_LICENSE_KEY`.

```cs
    /// <summary>
    /// WPF Application entry point.
    /// </summary>
#if SYNTIXI_SYNCFUSION_LICENSE_KEY
    [Syntixi.Attributes.EmbedSyncfusionLicenseKey]
#endif
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
#if SYNTIXI_SYNCFUSION_LICENSE_KEY
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SYNCFUSION_LICENSE_KEY);
#endif
        }
    }
```

## Viewing the documentation

The documentation can be found at https://docs.dpvreony.com/projects/syntixi/

## Contributing to the code

See the [contribution guidelines](CONTRIBUTING.md).
