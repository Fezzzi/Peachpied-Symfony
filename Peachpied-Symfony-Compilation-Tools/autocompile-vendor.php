<?php

define("NUGET_REPO", "..". DIRECTORY_SEPARATOR ."Peachpied-Symfony-Nuget-Repository");
define("OUT_DIR", ".". DIRECTORY_SEPARATOR ."Autocompiled-Nuget-Repository");
define("PROJ_TYPE", "msbuildproj");
define("DEBUG", true);

$libraries = getLibrariesToPack();
foreach ($libraries as $key => $library) {
    packLibrary($library);
}
// Final Cleanup
if (!DEBUG) {
	removeDir("bin");
	removeDir("obj");
}


/**
 * Recursive directory removal function
 *
 * @param string $dir
 */
function removeDir(string $dir) {
	$it = new RecursiveDirectoryIterator($dir, RecursiveDirectoryIterator::SKIP_DOTS);
	$files = new RecursiveIteratorIterator($it, RecursiveIteratorIterator::CHILD_FIRST);
	foreach($files as $file) {
		if ($file->isDir()) {
			rmdir($file->getRealPath());
		} else {
			unlink($file->getRealPath());
		}
	}

	rmdir($dir);
}

/**
 * Scans vendor directory for libraries
 *
 * @return array
 */
function getLibrariesToPack() : array {
    $libraries = [];
    $dirs = array_diff(scandir(".". DIRECTORY_SEPARATOR ."vendor"), array('..', '.'));
    foreach ($dirs as $dirkey => $dirname) {
        if ($dirname !== "composer") {
            $libs = array_diff(scandir("." . DIRECTORY_SEPARATOR . "vendor" . DIRECTORY_SEPARATOR . $dirname),
                array('..', '.'));

            foreach ($libs as $libKey => $libname) {
                $libraries[] = $dirname . DIRECTORY_SEPARATOR . $libname;
            }
        }
    }
    return $libraries;
}

/**
 * Handles packing of given library into Nuget
 *
 * @param string $library
 */
function packLibrary(string $library) {
    // Library name is denoted by our filesystem
    $nugetName = getNugetName($library, DIRECTORY_SEPARATOR);
    if (!nugetExists($nugetName)) {
        echo ("Resolving dependencies for library $library...\n");
        $dependencies = resolveDependencies(".". DIRECTORY_SEPARATOR ."vendor" . DIRECTORY_SEPARATOR . $library);
        if ($dependencies !== null) {
            echo("Generating nuget $nugetName.nupkg\n");
            if (createNuget($nugetName, $dependencies)) {
                echo ("Nuget for library $library has been created successfully!\n");
            } else {
                echo ("Creation of nuget for $library has failed!\n");
            }
        }
    } else {
        echo ("Nuget $nugetName already exists, skipping...\n");
    }
}

/**
 * Resolves given library's dependencies for correct packageReferences setup
 *
 * @param string $library
 * @return array|null
 */
function resolveDependencies(string $library) : ?array {
    $composers = array_filter(scandir($library), function($el){return $el === "composer.json";});
    if (sizeof($composers) == 0) {
        echo ("No composer file found, proceeding...\n");
        return [];
    } else {
        $content = json_decode(file_get_contents($library . DIRECTORY_SEPARATOR . "composer.json"));
        $dependencies = $content->require;
        $packageReferences = [];

        // We will deliberately skip version as composer installs latest version available
        foreach ($dependencies as $dependency => $version) {
            // By enforcing [prefix].[library] names, we skip PHP requirements and other non-symfony components
            if (preg_match("/.+\/.+/", $dependency)) {
                if (!packDependency($dependency)) {
                    return null;
                } else {
                    $packageReferences[] = $dependency;
                }
            }
        }
        return $packageReferences;
    }
}

/**
 * handles packing of given dependency library into Nuget
 *
 * @param string $dependency
 * @return bool
 */
function packDependency(string $dependency) : bool {
    echo ("Resolving dependency $dependency...\n");
    // Libraries have names delimited with slash
    $nugetname = getNugetName($dependency, "/");
    if (!nugetExists($nugetname)) {
        global $libraries;
        $pathName = str_replace("/", DIRECTORY_SEPARATOR, $dependency);

        if (array_search($pathName, $libraries) === false) {
            echo ("Please supply $dependency and retry the process again.\n");
            return false;
        }

        echo ("Resolving dependencies for library $dependency...\n");
        $dependencies = resolveDependencies(".". DIRECTORY_SEPARATOR ."vendor" . DIRECTORY_SEPARATOR . $pathName);
        echo ("Generating nuget $nugetname.nupkg\n");
        if (createNuget($nugetname, $dependencies)) {
            echo ("Nuget for library $dependency has been created successfully!\n");
            return true;
        }
        echo ("Creation of nuget for $dependency has failed!\n");
        return false;
    }

    echo ("Nuget for library $dependency already exists.\n");
    return true;
}

/**
 * Create dotnet compiler build config file with given dependencies and compile it
 *
 * @param string $nugetname
 * @param array $dependencies
 * @return bool
 */
function createNuget(string $nugetname, array $dependencies) : bool {
    $extension = PROJ_TYPE;
    $msbuild = "<Project Sdk=\"Peachpie.NET.Sdk/\">

            <!-- Sets package-specific properties based on name and path -->
            <Import Project=\"..\Props\loadPropsConfig.props\" />

            <!-- Configurates build properties such as packageId, AssemblyName -->
            <Import Project=\"..\Props\buildPropsConfig.props\" />

            <PropertyGroup>
                <PackageOutputPath>.\Auto-Generated-Nuget-Repository\</PackageOutputPath>
                <AdditionalExcludes />
            </PropertyGroup>

            <!-- Adds compile and content ItemGroup Node.
                 Adds to Content's Exclude attr additional excludes  -->
            <Import Project=\"..\Props\compileContentConfig.props\" />\n";

    $msbuild .= "\n\t\t<ItemGroup>";
    foreach($dependencies as $key => $dependency) {
        $name = getNugetName($dependency, "/");
        $msbuild .= "\n\t\t\t<PackageReference Include=\"". $name ."\" Version=\"1.0.0\" />\n";
    }
    $msbuild .= "\n\t\t</ItemGroup>";
    $msbuild .="\n</Project>";

    file_put_contents ($nugetname . "." . $extension, $msbuild);
    system("dotnet build \"". $nugetname . "." . $extension . "\"", $res);
    if (!DEBUG) {
        unlink($nugetname . "." . $extension);
    }
    return $res === 0;
}

/**
 * Tests whether is given nuget registered in global or local nuget repository regardless its version
 *
 * @param string $name
 * @return bool
 */
function nugetExists(string $name) : bool {
    return (fileExists(NUGET_REPO . DIRECTORY_SEPARATOR, $name))
        || (fileExists(OUT_DIR . DIRECTORY_SEPARATOR, $name));
}

/**
 * Tests presence of file between nugets stripped of their versions and extensions
 *
 * @param string $dir
 * @param string $file
 * @return bool
 */
function fileExists(string $dir, string $file) : bool {
    $files = array_map(function($el){
        return preg_replace("/\.\d+\.\d+\.\d+\.nupkg/", "", $el);
        },
        scandir($dir)
    );
    return array_search($file, $files) !== false;
}

/**
 * Converts given library name to Nuget name
 *
 * @param string $library
 * @param string $separator
 * @return string
 */
function getNugetName(string $library, string $separator) : string {
    return implode(".", array_map(
        function($el){
            return ucfirst($el);
        },
        explode($separator, $library))
    );
}
