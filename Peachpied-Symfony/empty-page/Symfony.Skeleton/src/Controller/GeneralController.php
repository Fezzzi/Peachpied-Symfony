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
        //return $this->redirectToRoute('home');
    }

//    /**
//     * @Route("/home", name="home", options={"expose"=true})
//     */
//    public function homeAction(Request $request)
//    {
//        return new Response(
//            '<html><body><h1>Homepage</h1></body></html>'
//        );
//    }
}
