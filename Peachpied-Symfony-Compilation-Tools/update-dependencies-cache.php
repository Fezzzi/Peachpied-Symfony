<?php

/**
 * This script refreshes dependency cache file in Nuget repository
 */

define("NUGET_REPO", "..". DIRECTORY_SEPARATOR ."Peachpied-Symfony-Nuget-Repository");
define("DEPENDENCIES_FILE", NUGET_REPO . DIRECTORY_SEPARATOR . "dependencies.json");
define("DEBUG", false);

$nugets = array();
foreach(array_diff(scandir(NUGET_REPO), [".", "..", "dependencies.json"]) as $key => $nuget) {
    $libName = array_filter(preg_split("/\./", $nuget), function($el){
        return $el !== "nupkg" && !preg_match("/^\d+$/", $el);
    });

    if (sizeof($libName > 0)) {
        $libName = implode(".", $libName);
        $nugets[$libName] = getNugetDependencies($nuget, $libName);
    }
}
updateNugetDependencies($nugets);

if (!DEBUG) {
    removeDir("temp");
}

echo "Dependencies updated!";


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
 * Unzips Nuget package to temp directory and parses out dependencies from .nuspec file
 *
 * @param string $nuget
 * @param string $libName
 * @return array
 */
function getNugetDependencies(string $nuget, string $libName) : array {
    $zip = new ZipArchive();
    $res = $zip->open(NUGET_REPO . DIRECTORY_SEPARATOR . $nuget);
    if ($res === true) {
        $zip->extractTo("temp" . DIRECTORY_SEPARATOR . $libName);
        $zip->close();

        $specFile = file_get_contents("temp" . DIRECTORY_SEPARATOR . $libName . DIRECTORY_SEPARATOR . $libName . ".nuspec");
        $regexp = "/<dependency id=\"(.*)\" version=\"(.*)\" exclude.* \/>/";
        preg_match_all($regexp, $specFile, $matches, PREG_SET_ORDER);
        $dependencies = array_map(function($el) {
            return sizeof($el) === 3 && $el[1] !== "Peachpie.App" ? [
                "name" => $el[1],
                "version" => $el[2]
            ] : false;
        }, $matches);

        return array_diff($dependencies, [false]);
    }

    return [];
}

/**
 * Encodes dependencies array as json and stores it in dependencies file
 *
 * @param array $nugets
 */
function updateNugetDependencies(array $nugets) {
    $data = json_encode($nugets, 448);
    file_put_contents(DEPENDENCIES_FILE, $data);
}