<?php

namespace App\Controller;

use Symfony\Component\HttpFoundation\Response;
use Twig\Environment;
use \Twig\Loader\FilesystemLoader;
use App\DBEmulator;

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

        $rooms = DBEmulator::getRooms();
        $str = $twig->render('twig-page.html.twig', ['rooms' => $rooms]);
        return new Response($str);
	}
	
	public function razorInTwigPageAction()
	{
		$loader = new FilesystemLoader("../templates/");
        $twig = new Environment($loader);

        $rooms = DBEmulator::getRooms();
        $str = $twig->render('razor-in-twig-page.html.twig', ['rooms' => $rooms]);
        return new Response($str);
	}
}
