<?php

namespace AppBundle\Controller;

use Sensio\Bundle\FrameworkExtraBundle\Configuration\Route;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Method;

class GroupsController extends HelperController
{
    /**
     * @Route("/groups", name="groups", options={"expose" = true})
     */
    public function groupsAction()
    {
        return $this->render('groups_list.html.twig', array(
        ));
    }
}