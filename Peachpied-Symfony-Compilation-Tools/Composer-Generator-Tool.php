<?php

define("DEBUG", true);

/**
 * Handles generating of content of vendor/composer folder.
 * Nuget generating requires PHP ^5.5.12 with json ^1.3.6
 *
 * Class ComposerGenerator
 */
class ComposerGenerator {
    private $projectBuild;
    private $projectLock;
    private $packages;
    private $vendorDir;
    private $installedFile;

    /**
     * ComposerGenerator constructor.
     * @param string $vendorDir
     * @param string|null $projectBuild
     * @param string|null $projectLock
     */
    public function __construct(string $vendorDir, string $projectBuild = null, string $projectLock = null) {
        $this->projectBuild = $projectBuild;
        if ($projectBuild !== null) {
            $dir = dirname($this->projectBuild);
            if (!is_dir($dir)) {
                throw new \UnexpectedValueException($dir . ' does not exist.');
            }
            if (!file_exists($projectBuild)) {
                throw new \UnexpectedValueException($projectBuild.' does not exist.');
            }

            $this->packages = array();
            $this->installedFile = $vendorDir.'/composer/installed.json';
            $dir = dirname($this->installedFile);
            if (!is_dir($dir)) {
                if (file_exists($dir)) {
                    throw new \UnexpectedValueException($dir.' exists and is not a directory.');
                }
                if (!@mkdir($dir, 0777, true)) {
                    throw new \UnexpectedValueException($dir.' does not exist and could not be created.');
                }
            }
        }

        $this->vendorDir = $vendorDir;

        $this->projectLock = $projectLock;
        if ($projectLock != null) {
            $dir = dirname($this->projectLock);
            if (!is_dir($dir)) {
                throw new \UnexpectedValueException($dir . ' does not exist.');
            }
            if (!file_exists($projectLock)) {
                throw new \UnexpectedValueException($projectLock.' does not exist.');
            }
        }
    }

    /**
     * Generates Composer folder in vendor from project's composer.lock file
     */
    public function generateFromLock() {
        echo("Calling composer install...\n");
        system("php composer.phar install -d \"" . dirname($this->projectLock). "\"");
        $dirs = scandir($this->vendorDir);
        if ($dirs) {
            echo("Composer directory generated.\n");

            $dirs = array_diff($dirs, [".", "..", "composer", "autoload.php", ".gitignore"]);
            if (!DEBUG) {
				foreach ($dirs as $key => $dir) {
					$this->removeDir($this->vendorDir . DIRECTORY_SEPARATOR . $dir);
				}
			}
        } else {
            echo("Composer directory was not generated.\n");
        }
    }

    /**
     * Recursive directory removal function
     *
     * @param string $dir
     */
    private function removeDir(string $dir) {
        $it = new RecursiveDirectoryIterator($dir);
        $files = new RecursiveIteratorIterator($it, RecursiveIteratorIterator::CHILD_FIRST);
        foreach($files as $file) {
            if ($file !== ".." && $file !== ".") {
                if ($file->isDir()) {
                    rmdir($file->getRealPath());
                } else {
                    unlink($file->getRealPath());
                }
            }
        }

        rmdir($dir);
    }

    /**
     * Generates Composer folder in vendor from Nuget repository and project build file
     */
    public function generateFromNugets() {
        $this->getInstalledpackages();
        $this->generateInstalledJSON();
    }

    /**
     * Uses dependency cache file to get list of all packages the project is dependant on
     */
    private function getInstalledPackages() {
        $regex = "/<PackageReference Include=\"(.*)\" Version=\"(.*)\" \/>/";
        preg_match_all($regex, file_get_contents($this->projectBuild), $matches, PREG_SET_ORDER);
        $basePackages = array_map(function($el) {
            return sizeof($el) === 3 ? $el[1] : false;
        }, $matches);

        $dependencyCache = file_get_contents("../Peachpied-Symfony-Nuget-Repository/dependencies.json");
        if ($dependencyCache !== false) {
            $dependencyCache = json_decode($dependencyCache);
            foreach ($basePackages as $key => $package) {
                $this->packages[str_replace(".", "/", $package)] = 1;
                $this->RecursiveDependencies($dependencyCache, $dependencyCache->$package);
            }
        }
    }

    /**
     * Resolve dependencies recursively
     *
     * @param stdClass $dependencyCache
     * @param stdClass $dependencies
     */
    private function RecursiveDependencies(stdClass $dependencyCache, stdClass $dependencies){
        foreach ($dependencies as $key => $dependency) {
            $depLib = str_replace(".", "/", $dependency->name);
            if (!isset($this->packages[$depLib])) {
                $this->packages[$depLib] = 1;
                $this->RecursiveDependencies($dependencyCache, $dependencyCache->{$dependency->name});
            }
        }
    }

    /**
     * Uses referenced packages to creates installed.json file
     */
    private function generateInstalledJSON() {
        $data = array();
        $dumper = new ArrayDumper();

        foreach ($this->packages as $package) {
            $data[] = $dumper->dump($package);
        }

        usort($data, function ($a, $b) {
            return strcmp($a['name'], $b['name']);
        });

        $json = json_encode($data, 448);
        file_put_contents($this->installedFile, $json. "\n");
    }
}