# Dovetail Attributes

Dovetail Attributes is to enable the embedding of the Syncfusion license key into your code. This is just the package that contains the attributes. The actual generation implementation is in the Dovetail.SourceGenerator package.

## Getting Started

To get started with Dovetail Attributes, you'll first need to add the library to your project.


### Installation

If you're using NuGet Package Manager, you can install Dovetail Attributes by running the following command in your package manager console:

```
Install-Package Dovetail

```

This will add the attributes package and the source generator package as development dependencies.

## Usage

To use Dovetail Attributes, you'll need to add the `EmbedSyncfusionLicenseKey` attribute to your class. This will embed `SYNCFUSION_LICENSE_KEY` into your code. This is an example using WPF.

```cs
    /// <summary>
    /// WPF Application entry point.
    /// </summary>
    [Dovetail.Attributes.EmbedSyncfusionLicenseKey]
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

## Contributing or Issues

Contributions and feedback to Dovetail are welcome! Take a look at https://github.com/dpvreony/dovetail/

## License

Dovetail Attributes is released under the MIT License. See the LICENSE file in the package for more information.
