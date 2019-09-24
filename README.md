## Peachpied Symfony

Symfony 4.2.3 framework compiled to c# using peachpie PHP compiler (https://www.peachpie.io/).

The aplication has selected structure:

 - **Additional-Tools/**
   - Contains a set of tools and script that make process of Nuget recompilation or application start-up easier.
   - These will be further integrated into build and start-up processes.

 - **Peachpied-Symfony-Nuget-Repository/**
   - Contains generated Nuget packages that correspond to Symfony components.

 - **Peachpied-Symfony-Twig-Razor/**
   - Contains minimal project (without actual Symfony 4.2.3 framework components) that Twig <-> Razor template engines interoperability is developed on.

 - **Peachpied-Symfony/**
   - Contains empty page build with Symfony 4.2.3 framework that all the NuGet compilation is performed on.

 - **Twig-Razor-Only/**
   - Contains minimal ASP.NET Core page illustrating the usage of Twig Razor interoperibility features without referencing the whole SYmfony project.

 - **Props/**
   - Collective build settings for package's `.msbuildproj` configurations.

 - **Docs/**
   - PDF versions of Semestral Project documentation and WIP Bachelor Thesis.

 - **global.json**
   - File controlling globally used Peachpie SDK version.
