<?php

use \App\Kernel;
use \Symfony\Component\Config\FileLocator;
use \Symfony\Component\Routing\Loader\YamlFileLoader;
use \Symfony\Component\Routing\Router;

generateCache($argv[1] ?? 'dev', isset($argv[2]) && $argv[2] == true);

/**
 * Performs Kernel and Routing cache generating
 *
 * @param string $environment
 * @param bool $debug
 */
function generateCache(string $environment, bool $debug) {
	try {
        require (implode(DIRECTORY_SEPARATOR, ["vendor", "autoload.php"]));

        // Symfony project's Kernel cache generating
        $kernel = new Kernel($environment, (bool)$debug);
        $kernel->boot();

        // Manual router cache generating
        $loader = new YamlFileLoader(new FileLocator("config" . DIRECTORY_SEPARATOR));
        $cacheDir = implode(DIRECTORY_SEPARATOR, ["var", "cache", $environment]);
        $router = new Router($loader, "routes.yaml", array(
            "matcher_cache_class" => "srcApp_Kernel" . ucfirst(strtolower($environment)) . "ContainerUrlMatcher",
            "cache_dir" => $cacheDir
        ));
        $router->getMatcher();

        echo "Cache directory generated.\n";
    } catch(\Exception $e) {
	    echo "Cache directory creation failed!\n";
    }
}
