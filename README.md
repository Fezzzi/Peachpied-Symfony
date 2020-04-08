## Peachpied Symfony

Symfony 4.2.3 framework nugetized and compiled to C# using peachpie, the PHP compiler (https://www.peachpie.io/).

#### Requirements:

 - PHP 5.3.2+ with `openssl` extension
 - Nuget Package Manager 5.0+

#### Directory structure:
 - **[Docs](/Docs)**
   - The project is both Bachelor thesis and Semestral project - documentations for each can be found there.
   - The Semestral Project docs might be little obsolete as the project continued to develop after meeting the requiremnets of a semestral project.
 - **[Examples](/Examples)**
   - Contains two projects demonstrating possible usage.
   - **Twig-Razor-Only** - pure .NET Core application using both Razor and Twig template languages without including any Symfony project.
   - **Twig-Razor-Full-Page** - .NET Core application using Symfony, displaying interoperability of Razor and Twig within both environments.
 - **[Libs](/Libs)**
   - Contains a set of `.dll`s that are used during Symfony components building.
   - These will be hopefully embedded into SDK in the future to avoid the necessity for this kind of providing.
   - In case of issues, delete content of this folder and publish the **/Tools/BuildGen** project as described [here](/Tools). 
 - **[Nuget-Repository](/Nuget-Repository)**
   - Contains nuget packages for Symfony components and other provided tools.
 - **[Peachpied.Symfony](/Peachpied.Symfony)**
   - Dev directory with Peachpied.Symfony.AspNetCore project, former Symfony 4.2.3 components, Peachpied.Symfony.Skeleton project and an empty page using Symfony.
 - **[Props](/Props)**
   - Collective build settings for packages' `.msbuildproj` configurations.
 - **[Tools](/Tools)**
   - Helper tools that automatize processes of compilation of both Symfony components and resulting .NET Core apps are located here.

#### Files:
 - **Nuget.config**
   - File defining **Nuget-Repository** as local nuget repository for provided tools and nugets.
   - Hopefully, nugets will be distributed in the more standard way in the future.
 - **global.json**
   - File globally controlling used Peachpie SDK version.
 - **libsConfig.json**
   - File for per-component configuration of Symfony components' compilation.
   - In addition to default includes and excludes specified in the **Props/compilationConfig.props**, one can define additional ones/overrides for each version of the Symfony component using this file.
   - Aside from that, Symfony components occasionally containt circular non-dev dependencies that the application can't resolve automatically. These needs to be resolved manually using the `ignoredDependencies` key.
   - In case of repetitive values for different versions of componnets, it is possible to use the `default` key instead of the version one. In case the version key of compiled package can't be found, app searches for the `default` key and applies config from there => both keys can't be used at the same time for one component.
