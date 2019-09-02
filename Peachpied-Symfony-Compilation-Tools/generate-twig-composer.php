<?php

require_once("composer-generator.php");

$ct = new ComposerGenerator('C:\Users\Fezzi\Documents\MATFYZ\BAKALARKA\Peachpied_Symfony\Peachpied-Symfony-Twig-Razor\twig-razor-page\vendor', null, 'C:\Users\Fezzi\Documents\MATFYZ\BAKALARKA\Peachpied_Symfony\Peachpied-Symfony-Twig-Razor\twig-razor-page\composer.lock');
$ct->generateFromLock();
