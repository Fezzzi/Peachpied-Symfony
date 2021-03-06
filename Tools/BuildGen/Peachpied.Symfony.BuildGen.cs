﻿using System;
using System.IO;
using System.Collections.Generic;
using Jokedst.GetOpt;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using System.Linq;
using System.Diagnostics;
using System.Json;
using JsonFormatter;

namespace PSBuildGen {

    /// <summary>
    /// Simple generator of build files for Symfony components
    /// </summary>
    class PSBuildGen {
        private static string configOpt = getDefaultConfigPath();
        private static string projectOpt = String.Empty;
        private static string propsOpt = getDefaultPropsPath();
        private static bool buildOpt = false;
        private static bool debugOpt = false;

        static void Main(string[] args) {
            GetOpt opts = configOpts();
            opts.ParseOptions(args);

            if (projectOpt.Equals(String.Empty)) {
                opts.ShowUsage();
            } else {
                generate();
            }
        }

        /// <summary>
        /// Handles generating of .msbuild files and build script
        /// </summary>
        private static void generate() {
            WarmLibsCache wlc = new WarmLibsCache() {
                ConfigPath = configOpt,
                ProjPath = projectOpt
            };

            Console.WriteLine("Generating cache...");
            if (wlc.Execute()) {
                List<string> components = new List<string>(wlc.OrderedComponents.Select(
                    component => String.Join('.', component.Split("/").Select(
                        s => Char.ToUpper(s[0]) + s.Substring(1)
                    ))
                ));

                Console.WriteLine("Generating build files...");
                generateDirectoryBuildProps();
                foreach(string component in components) {
                    generateTargetsFile(component);
                    generateBuildFile(component);
                }

                Console.WriteLine("Generating lock fragments...");
                foreach(JsonObject package in wlc.LockPackages) {
                    generatePackageLockPart(package, projectOpt, false);
                }
                foreach (JsonObject package in wlc.LockPackagesDev) {
                    generatePackageLockPart(package, projectOpt, true);
                }

                Console.WriteLine("Generating build script...");
                generateBuildScript(components);

                if (buildOpt) {
                    Console.WriteLine("Building components...");
                    runPowershellBuild();
                    if (!debugOpt) {
                        cleanGeneratedFiles(components);
                    }
                }
                Console.WriteLine("done!");
            } else {
                Console.WriteLine("Cache generating failed, aborting! Please check your input and try again.");
            }
        }

        /// <summary>
        /// Removes generated files
        /// </summary>
        private static void cleanGeneratedFiles(List<string> components) {
            if (File.Exists(Path.Combine(".", "buildScript.ps1"))) {
                File.Delete(Path.Combine(".", "buildScript.ps1"));
            }
            if (File.Exists(Path.Combine(projectOpt, "Directory.Build.props"))) {
                File.Delete(Path.Combine(projectOpt, "Directory.Build.props"));
            }
            foreach (string component in components) {
                if (File.Exists(Path.Combine(projectOpt, component + ".targets"))) {
                    File.Delete(Path.Combine(projectOpt, component + ".targets"));
                }
                if (File.Exists(Path.Combine(projectOpt, component + ".msbuildproj"))) {
                    File.Delete(Path.Combine(projectOpt, component + ".msbuildproj"));
                }
                if (File.Exists(Path.Combine(projectOpt, component + ".lockFragment.json"))) {
                    File.Delete(Path.Combine(projectOpt, component + ".lockFragment.json"));
                }
				if (File.Exists(Path.Combine(projectOpt, component + ".lockFragment-dev.json"))) {
                    File.Delete(Path.Combine(projectOpt, component + ".lockFragment-dev.json"));
                }
            }
        }

        /// <summary>
        /// Executes generated powershell build script
        /// </summary>
        private static void runPowershellBuild() {
            var startInfo = new ProcessStartInfo() {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy unrestricted -file \"buildScript.ps1\"",
                UseShellExecute = false
            };
            Process.Start(startInfo).WaitForExit();
        }

        /// <summary>
        /// Generate build script for Symfony components
        /// </summary>
        private static void generateBuildScript(List<string> components) {
            string fileName = Path.Combine(".", "buildScript.ps1");
            StreamWriter file = new StreamWriter(fileName, false);

            file.Write($"cd \'{projectOpt}\';\n\n");
            file.WriteLine("$libraries =");
            bool first = true;
            foreach (string component in components) {
                if (!first) {
                    file.WriteLine(",");
                } else {
                    first = false;
                }
                file.Write($"\t\"{component}.msbuildproj\"");
            }
            file.Write(";\n\n");
            file.WriteLine("foreach ($library in $libraries) {");
            file.WriteLine("\t$counter = $counter + 1;");
            file.WriteLine("\tdotnet build $library;");
            file.WriteLine("\techo \"Done building $counter of $($libraries.count) libraries...\"");
            file.WriteLine("}");
            file.Flush();
            file.Close();
        }

        /// <summary>
        /// Saves minified package's part of composer.lock file into standalone file
        /// </summary>
        private static void generatePackageLockPart(JsonObject package, string projPath, bool isDev) {
            string packageName = String.Join(".", package["name"].ToString().Replace("\"", "").Split('/').Select(
                el => Char.ToUpper(el[0]) + el.Substring(1)
            ));
            string filePath = Path.Combine(projPath, $"{packageName}.lockFragment{(isDev ? "-dev" : "")}.json");
            if (!File.Exists(filePath)) {
                using (StreamWriter sw = new StreamWriter(filePath)) {
                    sw.Write(package.ToString());
                }
            }
        }

        /// <summary>
        /// Generate targets file with task that manually copies content files from nuget to project
        /// Also sets up item with paths to components' lockFragments
        /// </summary>
        private static void generateTargetsFile(string component) {
            string target = $"Copy_{component.Replace(".", "_")}_Vendor";
            string toolsDir = "$([System.IO.Path]::GetFullPath($(MSBuildThisFileDirectory)))../tools/any/any";
            string assetsDir = "$([System.IO.Path]::GetFullPath($(MSBuildThisFileDirectory)))../contentFiles/any/netcoreapp2.0";
            string assetsPathSufix = component.Replace('.', '/').ToLower();
            string fileContent =
$@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" InitialTargets=""{target}"">
    
    <ItemGroup>
        <LockFragments Include=""{toolsDir}/{component}.lockFragment.json"" Condition=""Exists('{toolsDir}/{component}.lockFragment.json')"" />
        <LockFragments Include=""{toolsDir}/{component}.lockFragment-dev.json"" Condition=""Exists('{toolsDir}/{component}.lockFragment-dev.json')"" />
    </ItemGroup>

    <Target Name=""{target}"" Condition=""$(RestoreVendor)==true AND !Exists('$(MSBuildProjectDirectory)/vendor/{assetsPathSufix}') AND Exists('{assetsDir}/vendor')"">
        <Message Text=""Copying {component} assets..."" Importance=""high"" />
        <ItemGroup>
            <{target}_Files Include=""{assetsDir}/vendor/**/*.*"" />
        </ItemGroup>
        <Copy SourceFiles=""@({target}_Files)"" DestinationFolder=""$(MSBuildProjectDirectory)/vendor/%(RecursiveDir)""/>
    </Target>

</Project>";

            string fileName = Path.Combine(projectOpt, component + ".targets");
            StreamWriter file = new StreamWriter(fileName, false);
            file.Write(fileContent);
            file.Flush();
            file.Close();
        }

        /// <summary>
        /// Generate default build file for Symfony component
        /// </summary>
        private static void generateBuildFile(string component) {
            string fileContent = 
$@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Peachpie.NET.Sdk/"">
            
    <!-- Configurates automatization targets -->
    <Import Project=""{Path.Combine(propsOpt, "automatizationConfig.targets")}"" />

    <!-- Configurates build properties -->
    <Import Project=""{Path.Combine(propsOpt, "propertiesConfig.props")}"" />

    <!-- Fills compile and content items -->   
    <Import Project=""{Path.Combine(propsOpt, "compilationConfig.props")}"" />

    <!-- Adds task that manually copies content files from nuget to project -->
    <ItemGroup>
        <None Include="".\$(MSBuildProjectName).targets"" Pack=""true"" PackagePath=""build"" />
        <None Include="".\$(MSBuildProjectName).targets"" Pack=""true"" PackagePath=""buildTransitive"" />
    </ItemGroup>

</Project>";

            string fileName = Path.Combine(projectOpt, component + ".msbuildproj");
            StreamWriter file = new StreamWriter(fileName, false);
            file.Write(fileContent);
            file.Flush();
            file.Close();
        }

        /// <summary>
        /// Generate file that defines properties in an early stage of compilation
        /// </summary>
        private static void generateDirectoryBuildProps() {
            string fileContent =
$@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
   
    <PropertyGroup>
        <ProjectNameLocation>$([MSBuild]::EnsureTrailingSlash('$([System.String]::Copy('$(MSBuildProjectName)').Replace('.','\'))'))</ProjectNameLocation>
        <BaseOutputPath>.\bin\$(Configuration)\$(ProjectNameLocation)</BaseOutputPath>
        <BaseIntermediateOutputPath>.\obj\$(Configuration)\$(ProjectNameLocation)</BaseIntermediateOutputPath>
    </PropertyGroup>      

</Project>";

            string fileName = Path.Combine(projectOpt, "Directory.Build.props");
            StreamWriter file = new StreamWriter(fileName, false);
            file.Write(fileContent);
            file.Flush();
            file.Close();
        }

        /// <summary>
        /// Configures tool's command line options
        /// </summary>
        private static GetOpt configOpts() {
            return new GetOpt("Simple generator of build files for Symfony components.",
                new[] {
                    new CommandLineOption('c', "config", "Config file directory",
                        ParameterType.String, c => configOpt = (string)c),
                    new CommandLineOption('p', "project", "Project root directory",
                        ParameterType.String, p => projectOpt = (string)p),
                    new CommandLineOption('r', "props", "Props directory",
                        ParameterType.String, r => propsOpt = (string)r),
                    new CommandLineOption('b', "build", "Automatically run generated build script",
                        ParameterType.None, o => buildOpt = true),
                    new CommandLineOption('d', "debug", "Skip generated files cleanup",
                        ParameterType.None, o => debugOpt = true),
                });
        }

        private static string getDefaultConfigPath() {
            return new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName;
        }

        private static string getDefaultPropsPath() {
            return Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName, "Props");
        }
    }
}
