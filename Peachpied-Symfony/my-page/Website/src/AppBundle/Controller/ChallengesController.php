<?php

namespace AppBundle\Controller;

use AppBundle\Entity\Challenge;
use AppBundle\Form\Type\ChallengeType;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Route;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Serializer\Serializer;
use AppBundle\Api\ChallengeApiModel;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Method;

class ChallengesController extends HelperController
{
    /**
     * @Route("/challenge/{id}", name="challenge_get")
     * @Method("GET")
     */
    public function getChallengeAction(Challenge $challenge)
    {
        $apiModel = $this->createChallengeApiModel($challenge);
        $apiModel = array();

        return $this->createApiResponse($apiModel);
    }

    /**
     * @Route("/challenge/{id}", name="challenge_delete")
     * @Method("POST")
     */
    public function deleteChallengeAction(Challenge $challenge)
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');
        $em = $this->getDoctrine()->getManager();

        $achvs1 = $this->getDoctrine()->getRepository('AppBundle:AchvChallengeReqs')->findBy(array('challengeid' => $challenge->getId()));
        $achvs2 = $this->getDoctrine()->getRepository('AppBundle:AchvChalSpReqs')->findBy(array('challengeid' => $challenge->getId()));
        foreach([$achvs1, $achvs2] as $achvs)
            foreach($achvs as $achv){
                $this->removeAchievement($achv->getAchvId());
                $em->remove($achv);
                $em->remove($this->getDoctrine()->getRepository('AppBundle:Achievement')->findBy(array('id' => $achv->getAchvId()))[0]);
            }

        $sportLogs = $this->getDoctrine()->getRepository('AppBundle:SportLog')->findBy(array('challengeid' => $challenge->getId()));
        foreach($sportLogs as $sportLog)
            $sportLog->setChallengeid(null);

        $em->remove($challenge);
        $em->flush();

        return new Response(null, 204);
    }

    /**
     * @Route("/challenge", name="challenge_new", options={"expose" = true})
     * @Method("POST")
     */
    public function newChallengeAction(Request $request)
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');

        $data = json_decode($request->getContent(), true);
        $data = $this->deInject($data);

        if ($data === null) {
            throw new BadRequestHttpException('Invalid JSON');
        }

        $form = $this->createForm(ChallengeType::class, null, [
            'csrf_protection' => false,
        ]);

        $form->submit($data);

        if (!$form->isValid()) {
            $errors = $this->getErrorsFromForm($form);
            return $this->createApiResponse([
                'errors' => $errors,
                'form' => $form->getData(),
                '$data' => $data
            ], 400);
        }

        /** @var Challenge $challenge */
        $challenge = $form->getData();
        $em = $this->getDoctrine()->getManager();
        $em->persist($challenge);
        $em->flush();

        $apiModel = $this->createChallengeApiModel($challenge);

        $response = new Response(null, 204);
        // setting the Location header... it's a best-practice
        $response->headers->set(
            'Location',
            $this->generateUrl('challenge_get', ['id' => $challenge->getId()])
        );

        return $response;
    }

    /**
     * @Route("/challenges", name="challenges", options={"expose" = true})
     */
    public function challengesAction()
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');
        $data = $this->getDoctrine()->getRepository('AppBundle:Challenge')
            ->findBy(array(),array('id' => 'DESC'));
        $logs = $this->getDoctrine()->getRepository('AppBundle:SportLog')
            ->findBy(array('user_id' => $this->getUser()->getId()));

        $spLogs = [];
        foreach($logs as $log){
            if(isset($spLogs[$log->getChallengeId()]))
                ++$spLogs[$log->getChallengeId()];
            else
                $spLogs[$log->getChallengeId()] = 1;
        }

        $images = array();
        foreach ($data as $key => $dat) {
            $images[$key] = stream_get_contents($dat->getImage());
            $data[$key]->setImage($images[$key]);
        }

        return $this->render('chalSp_list.html.twig', array(
            'data' => $data,
            'dataName' => "Challenge",
            'logs' => $spLogs,
            'points' => true,
        ));
    }
}