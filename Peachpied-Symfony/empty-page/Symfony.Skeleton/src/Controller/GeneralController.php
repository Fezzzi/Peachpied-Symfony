<?php

namespace App\Controller;

use Symfony\Component\HttpFoundation\Response;
use Twig\Environment;
use \Twig\Loader\FilesystemLoader;

class GeneralController
{
    public function indexAction()
    {
	$loader = new FilesystemLoader("templates/");
        $twig = new Environment($loader);

        $str = $twig->render('base.html.twig', ['the' => 'variables', 'go' => 'here']);
        return new Response($str);

        return new Response(
            '<html>
                <head>
                    <link rel="stylesheet" href="GeneralStyles.css" type="text/css">
                    
                </head>
                <body>
                    <h1>RAZOR and TWIG interoperability</h1>
                    <h3>minimal exmaple</h3>
                    <div class="tile-row">
                        <a href="/razor-page" class="tile t1"><div><h2>Razor Page</h2></div></a>
                        <a href="/twig-page" class="tile t2"><div><h2>Twig Page</h2></div></a>
                    </div>
                    <div class="tile-row">
                        <a href="/twig-in-razor-page" class="tile t3"><div><h2>Razor Page</h2><h5>with embedded Twig</h5></div></a>
                        <a href="/razor-in-twig-page" class="tile t4"><div><h2>Twig Page</h2><h5>with embedded Razor</h5></div></a>
                    </div>
                </body>
            </html>'
        );
    }
	
	public function twigPageAction()
	{
        $loader = new FilesystemLoader("./templates/");
        $twig = new Environment($loader);

        $str = $twig->render('base.html.twig', ['the' => 'variables', 'go' => 'here']);
        return new Response($str);
	}
	
	public function razorInTwigPageAction()
	{
		return new Response(
            '<html><body><h1>razorInTwigPage</h1></body></html>'
        );
	}
}
