<?php

define("PEACHPIED_SYMFONY_NUGET_REPOSITORY", "..". DIRECTORY_SEPARATOR ."Peachpied-Symfony-Nuget-Repository");
define("OUTPUT_DIRECTORY", ".". DIRECTORY_SEPARATOR ."Auto-Generated-Nuget-Repository");

$dirs = scandir(".". DIRECTORY_SEPARATOR ."vendor");
foreach ($dirs as $dirkey => $dirname) {
    $libs = scandir(".". DIRECTORY_SEPARATOR ."vendor" . DIRECTORY_SEPARATOR . $dirname);

    if ($dirname !== "composer") {
        foreach ($libs as $libKey => $libname) {
            echo ($dirname . $libname . "\n");
            $nugetname = getNugetName($dirname, $libname);
            if (!nugetExists($nugetname)) {
                //createNuget($nugetname);
            } else {
                echo ("Component " . $nugetname . " already compiled\n");
            }
            //removeLib($dirname, $libname);
        }
    }
    //removeLib($dirname);
}

function removeLib($dirname, $libname = null) {
    $dir = $dirname . ($libname != null ? DIRECTORY_SEPARATOR . $libname : "");
    $it = new RecursiveDirectoryIterator($dir, RecursiveDirectoryIterator::SKIP_DOTS);
    $files = new RecursiveIteratorIterator($it, RecursiveIteratorIterator::CHILD_FIRST);
    foreach($files as $file) {
        if ($file->isDir()){
            rmdir($file->getRealPath());
        } else {
            unlink($file->getRealPath());
        }
    }
    rmdir($dir);
}

function createNuget($nugetname) {
    $dependencies = json_decode(file_get_contents("composer.json"));
    $msbuild = `<Project Sdk=\"Peachpie.NET.Sdk/1.0.0-appv2527\">

              <!-- Sets package-specific properties based on name and path -->
              <Import Project=\"..\props\loadPropsConfig.props\" />

              <!-- Configurates build properties such as packageId, AssemblyName -->
              <Import Project=\"..\props\buildPropsConfig.props\" />

              <PropertyGroup>
                <AdditionalExcludes />
              </PropertyGroup>

              <!-- Adds compile and content ItemGroup Node.
                  Adds to Content's Exclude attr additional excludes  -->
              <Import Project=\"..\props\compileContentConfig.props\" />

            </Project>`;

    file_put_contents ($nugetname . ".msbuildproj", $msbuild);
}

function nugetExists($name) {
    return (file_exists(PEACHPIED_SYMFONY_NUGET_REPOSITORY . DIRECTORY_SEPARATOR . $name))
        || (file_exists(OUTPUT_DIRECTORY . DIRECTORY_SEPARATOR . $name));
}

function getNugetName($dirname, $libname) {
    return ucfirst($dirname) . "." . ucfirst($libname);
}
