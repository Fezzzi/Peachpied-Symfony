<?php

namespace App\Controller;

use Symfony\Component\HttpFoundation\Response;

class GeneralController
{
    public function indexAction()
    {
        return new Response(
            '<html><body><h1>Homepage</h1></body></html>'
        );
    }
	
	public function twigPageAction()
	{
		return new Response(
            '<html><body><h1>twigPage</h1></body></html>'
        );
	}
	
	public function razorInTwigPageAction()
	{
		return new Response(
            '<html><body><h1>razorInTwigPage</h1></body></html>'
        );
	}
}
