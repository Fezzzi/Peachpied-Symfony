<?php

generateCache("..". DIRECTORY_SEPARATOR ."Peachpied-Symfony-Twig-Razor". DIRECTORY_SEPARATOR ."twig-razor-page". DIRECTORY_SEPARATOR ."src");

function generateCache($kernelPath, $environment = "dev", $debug = false) {
	require_once($kernelPath . DIRECTORY_SEPARATOR . "Kernel.php");
	
	$kernel = new Kernel($environment, (bool) $debug);
	$kernel->boot();
	
	if ($kernel->booted) {
		echo "Cache generated successfully";
		return false;
	} else {
		echo "Cache generation failed!";
		return true;
	}
}