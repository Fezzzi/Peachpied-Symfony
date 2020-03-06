using System;
using System.IO;
using System.Collections.Generic;
using Jokedst.GetOpt;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using System.Linq;
using System.Diagnostics;

namespace PSBuildGen {

    /// <summary>
    /// Simple generator of build files for Symfony components
    /// </summary>
    class PSBuildGen {
        private static string configOpt = getDefaultConfigPath();
        private static string projectOpt = String.Empty;
        private static string propsOpt = getDefaultPropsPath();
        private static bool buildOpt = false;

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
                ConfigPath = new TaskItem(configOpt),
                ProjPath = new TaskItem(projectOpt)
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
                foreach (string component in components) {
                    generateBuildFile(component);
                }

                Console.WriteLine("Generating build script...");
                generateBuildScript(components);

                if (buildOpt) {
                    Console.WriteLine("Building components...");
                    runPowershellBuild();
                } else {
                    Console.WriteLine("done!");
                }
            } else {
                Console.WriteLine("Cache generating failed, aborting! Please check your input and try again.");
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
            Process.Start(startInfo);
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
            file.WriteLine("echo \"Building Nuget packages...\";");
            file.WriteLine("foreach ($library in $libraries) {");
            file.WriteLine("\t$counter = $counter + 1;");
            file.WriteLine("\tdotnet build $library;");
            file.WriteLine("\techo \"Done building $counter of $($libraries.count) libraries...\"");
            file.WriteLine("}");
            file.WriteLine("echo \"Building finished!\";");
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
