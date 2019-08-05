<?php

namespace App\Controller;

use Symfony\Component\HttpFoundation\Response;
use Twig\Environment;
use \Twig\Loader\FilesystemLoader;
use Symfony\Component\HttpFoundation\BinaryFileResponse;

class GeneralController
{
    public function indexAction()
    {
		$file = 'index.html';
		$response = new BinaryFileResponse($file);
		return $response;
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
