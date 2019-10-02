<?php

namespace App\Controller;

use Symfony\Component\HttpFoundation\Response;
use Twig\Environment;
use \Twig\Loader\FilesystemLoader;

class GeneralController
{
    public function indexAction()
    {
	    return new Response(
            '<html>
                <head>
                    <link rel="stylesheet" href="GeneralStyles.css" type="text/css">
                    
                </head>
                <body>
                    <h1>Symfony Empty Page</h1>
                </body>
            </html>'
        );
    }
}