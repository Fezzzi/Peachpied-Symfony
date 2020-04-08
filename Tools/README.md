### Peachpied Symfony - Tools

#### BuildGen
This tool automatizes Symfony components compilation. Output of this project is `.exe` utility that generates `.msbuildproj` and other necessary  files 
for all Symfony components found in the target directory. In addition to, the utility also outputs a `buildScript.ps1` that can be run in order to execute `dotnet build` on
each produced `msbuildproj` in required order.

To print man page of `Peachpied.Symfony.BuildGen.exe` simply invoke the utility with `-h` or `--help` parameter. The only required parameter is a project root directory. The
rest of parameters default to certain values.

To build and ready this tool properly, it is necessary to run
```
dotnet publish -r {Runtime} // e.g. win10-x64
```

### Component Tools
This set of tools automatizes certain tasks during a Symfony component's compilation. These are:
 - version retrieval -> GetLibVersion
 - package references setup -> GetLibReferences
 - compile itemGroup optimalization -> FixLibExcludes
 
To avoid the need to repeatedly parse `composer.lock` file, **WarmLibsCache** task first creates `obj\libsCache.json` file that contains only the necessary information for the
provided tasks in much more parsable form.

It is not necessary to build this project as it is deployed automatically during publish of **BuildGen**.

### Project Tools
This set of tools provide tasks needed to smoothly consume nugetized Symfony components from .NET Core application using Peachpied Symfony. Tasks mentioned provide:
 - `comoser.lock` file restoring -> RestoreComposerLock
 - autoload files generating -> GenerateSymfonyAutoload
 - cache files generating -> GenerateSymfonyCache

It is possible to deploy an application without these tools but doing so would require a lot of manual work so it is recommended to include a `PackageReference` to produced nuget from
Symfony project of .NET Core applications. Tasks are delivered to the project via `.targets` file included inside the nuget therefore the task does not require any additional setup.

This project can be build by running 
```
dotnet build
```
