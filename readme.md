[![Build status](https://ci.appveyor.com/api/projects/status/4e1h882fflq86o1f?svg=true)](https://ci.appveyor.com/project/igi/chr2cre)

---

# chr2cre

A small webapi to convert an Infinity Engine CHR file to an Infinity Engine CRE file

## Usage

Visit https://chr2cre.iimods.com/index
Upload an Infinity Enginer CHR file
Submit the form
The converted CRE file is automatically downloaded, or an error message is displayed


## Download

No explicit releases are available, though you can clone this repository and build the project.


## Technologies

chr2cre is written in C# Net Core 3 and is hosted as Azure functions.


## Compiling

To clone and run this application, you'll need [Git](https://git-scm.com) and [.NET](https://dotnet.microsoft.com/) installed on your computer. From your command line:

```
# Clone this repository
$ git clone https://github.com/btigi/chr2cre

# Go into the repository
$ cd chr2cre

# Build  the app
$ dotnet build
```


## License

chr2cre is licensed under [CC BY-NC 4.0](https://creativecommons.org/licenses/by-nc/4.0/)
chr2cre uses [base64js](https://github.com/beatgammit/base64-js), licenced under MIT - see licence.md for more information.
