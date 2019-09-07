<?php

namespace App\Controller;

use Symfony\Component\HttpFoundation\Response;
use Twig\Environment;
use \Twig\Loader\FilesystemLoader;

class GeneralController
{
    public function indexAction()
    {
		$html = file_get_contents('index.html');
		return new Response($html);
    }
	
	public function twigPageAction()
	{
        $loader = new FilesystemLoader("../templates/");
        $twig = new Environment($loader);

        $str = $twig->render('twig-page.html.twig', ['the' => 'variables', 'go' => 'here']);
        return new Response($str);
	}
	
	public function razorInTwigPageAction()
	{
		$loader = new FilesystemLoader("../templates/");
        $twig = new Environment($loader);

        $str = $twig->render('razor-in-twig-page.html.twig', ['the' => 'variables', 'go' => 'here']);
        return new Response($str);
	}
}
