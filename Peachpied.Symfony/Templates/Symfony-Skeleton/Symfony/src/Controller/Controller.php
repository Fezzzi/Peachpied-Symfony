<?php

namespace App\Controller;

use Symfony\Component\HttpFoundation\Response;

class Controller
{
    public function indexAction()
    {
	    return new Response(
            '<html>
                <head>
                    <link rel="stylesheet" href="Styles.css" type="text/css">
                    
                </head>
                <body>
                    <h1>Symfony Empty Page</h1>
                </body>
            </html>'
        );
    }
}
