<?php

namespace AppBundle\Controller;

use AppBundle\Entity\Sport;
use AppBundle\Form\Type\SportType;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Route;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Serializer\Serializer;
use AppBundle\Api\SportApiModel;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Method;
use Symfony\Component\HttpKernel\Exception\BadRequestHttpException;

class SportsController extends HelperController
{
    /**
     * @Route("/sport/{id}", name="sport_get")
     * @Method("GET")
     */
    public function getSportAction(Sport $sport)
    {
        $apiModel = $this->createSportApiModel($sport);
        $apiModel = array();

        return $this->createApiResponse($apiModel);
    }

    /**
     * @Route("/sport/{id}", name="sport_delete")
     * @Method("POST")
     */
    public function deleteSportAction(Sport $sport)
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');
        $em = $this->getDoctrine()->getManager();

        $achvs1 = $this->getDoctrine()->getRepository('AppBundle:AchvSportReqs')
            ->findBy(array('sportid' => $sport->getId()));
        $achvs2 = $this->getDoctrine()->getRepository('AppBundle:AchvChalSpReqs')
            ->findBy(array('sportid' => $sport->getId()));
        foreach([$achvs1, $achvs2] as $achvs)
            foreach($achvs as $achv){
                $this->removeAchievement($achv->getAchvId());
                $em->remove($achv);
                $em->remove($this->getDoctrine()->getRepository('AppBundle:Achievement')
                    ->findBy(array('id' => $achv->getAchvId()))[0]);
            }

        $sportLogs = $this->getDoctrine()->getRepository('AppBundle:SportLog')
            ->findBy(array('sportid' => $sport->getId()));
        foreach($sportLogs as $sportLog)
            $sportLog->setSportid(null);

        $em->remove($sport);
        $em->flush();

        return new Response(null, 204);
    }

    /**
     * @Route("/sport", name="sport_new", options={"expose" = true})
     * @Method("POST")
     */
    public function newSportAction(Request $request)
    {
        $data = json_decode($request->getContent(), true);
        $data = $this->deInject($data);

        if ($data === null) {
            throw new BadRequestHttpException('Invalid JSON');
        }

        $form = $this->createForm(SportType::class, null, [
            'csrf_protection' => false,
        ]);

        $form->submit($data);
        dump($form);
        if (!$form->isValid()) {
            $errors = $this->getErrorsFromForm($form);
            return $this->createApiResponse([
                'errors' => $errors
            ], 400);
        }

        /** @var Sport $sport */
        $sport = $form->getData();
        $em = $this->getDoctrine()->getManager();
        $em->persist($sport);
        $em->flush();

        $apiModel = $this->createSportApiModel($sport);

        $response = new Response(null, 204);
        //setting the Location header... it's a best-practice
        $response->headers->set(
            'Location',
            $this->generateUrl('sport_get', ['id' => $sport->getId()])
        );

        return $response;
    }

    /**
     * @Route("/sports", name="sports", options={"expose" = true})
     */
    public function sportsAction()
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');
        $data = $this->getDoctrine()->getRepository('AppBundle:Sport')
            ->findBy(array(),array('id' => 'DESC'));
        $logs = $this->getDoctrine()->getRepository('AppBundle:SportLog')
            ->findBy(array('user_id' => $this->getUser()->getId()));

        $spLogs = [];
        foreach($logs as $log){
            if(isset($spLogs[$log->getSportId()]))
                ++$spLogs[$log->getSportId()];
            else
                $spLogs[$log->getSportId()] = 1;
        }
        $images = array();
        foreach ($data as $key => $dat) {
            $images[$key] = stream_get_contents($dat->getImage());
            $data[$key]->setImage($images[$key]);
        }
        return $this->render('chalSp_list.html.twig', array(
            'data' => $data,
            'dataName' => "Sport",
            'logs' => $spLogs,
            'points' => false,
        ));
    }


    /**
     * Returns an array of sports information
     *
     * @return array
     */
    public function getSportsNames(){
        $sportsDetails = $this->getDoctrine()->getRepository('AppBundle:Sport')->getSports();
        $sportsNames = array();
        foreach ($sportsDetails as $sport) {

            $sportsNames[] = array(
                'name' => $sport['name'],
            );
        }

        return $sportsNames;
    }
}