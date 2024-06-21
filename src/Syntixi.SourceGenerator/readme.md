# Syntixi Attributes

Syntixi SourceGenerator is to enable the embedding of the Syncfusion license key into your code. This is just the package contains the Source Generator. There is a marker attribute in `Syntixi.Attributes` package that triggers the generation.

## Getting Started

To get started with Syntixi, you'll first need to add the library to your project.


### Installation

If you're using NuGet Package Manager, you can install Syntixi Sourcegenerator by running the following command in your package manager console:

```
Install-Package Syntixi.SourceGenerator

```

This will install the source generator. In order to make use of it, you'll need to add the Syntixi.Attributes package as well. Or you can just install the `Syntixi` package and let it set up the sub packages.

```

## Usage

To use Syntixi Attributes, you'll need to add the `EmbedSyncfusionLicenseKey` attribute to your class. This will embed `SYNCFUSION_LICENSE_KEY` into your code. This is an example using WPF.

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

You then need to add the license key to your CI/CD pipeline or your local environment variables. If you are working in a local environment you may need to restart the IDE or codespace in order for the change to be picked up.

## Further reading

Have a look at https://docs.dpvreony.com/projects/syntixi/

## Contributing or Issues

Contributions and feedback to Syntixi are welcome! Take a look at https://github.com/dpvreony/syntixi/

## License

Syntixi Attributes is released under the MIT License. See the LICENSE file in the package for more information.

