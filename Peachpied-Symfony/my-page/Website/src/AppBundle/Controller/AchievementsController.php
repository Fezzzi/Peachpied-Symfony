<?php

namespace AppBundle\Controller;

use AppBundle\Entity\Achievement;
use AppBundle\Entity\AchvAchvReqs;
use AppBundle\Entity\AchvChallengeReqs;
use AppBundle\Entity\AchvChalSpReqs;
use AppBundle\Entity\AchvSportReqs;
use AppBundle\Entity\Challenge;
use AppBundle\Entity\Sport;
use AppBundle\Entity\AchvLog;
use AppBundle\Form\Type\AchievementType;
use AppBundle\Form\Type\AchvLogType;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Route;
use Symfony\Component\Config\Definition\Exception\Exception;
use Symfony\Component\Debug\Exception\ContextErrorException;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Method;
use Symfony\Component\HttpKernel\Exception\BadRequestHttpException;

class AchievementsController extends HelperController
{
    /**
     * @Route("/achvlog/{id}", name="achievementLog_get")
     * @Method("GET")
     */
    public function getAchvLogAction(AchvLog $achvLog)
    {
        $apiModel = array();

        return $this->createApiResponse($apiModel);
    }

    /**
     * @Route("/achvlog", name="achievementLog_new", options={"expose" = true})
     * @Method("POST")
     */
    public function newAchvLogAction(Request $request)
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');

        $data = json_decode($request->getContent(), true);
        $data['user_id'] = $this->getUser()->getId();
        $achievement = $this->getDoctrine()->getRepository('AppBundle:Achievement')->findBy(array('id' => $data['achv_id']))[0];
        $data['name'] = $achievement->getName();
        $data['type'] = $achievement->getType();

        $data = $this->deInject($data);

        if ($data === null) {
            throw new BadRequestHttpException('Invalid JSON');
        }

        $form = $this->createForm(AchvLogType::class, null, [
            'csrf_protection' => false,
        ]);

        $form->submit($data);

        if (!$form->isValid()) {
            $errors = $this->getErrorsFromForm($form);
            return $this->createApiResponse([
                'errors' => $errors
            ], 400);
        }

        /** @var AchvLog $achvLog */
        $achvLog = $form->getData();
        $em = $this->getDoctrine()->getManager();
        $em->persist($achvLog);
        $em->flush();

        $response = new Response(null, 204);
        $response->headers->set(
            'Location',
            $this->generateUrl('achievementLog_get', ['id' => $achvLog->getId()])
        );

        return $response;
    }

    /**
     * @Route("/achievement/{name}", name="achievement_get")
     * @Method("GET")
     */
    public function getAchievementAction(Achievement $achievement)
    {
        $apiModel = array();

        return $this->createApiResponse($apiModel);
    }

    /**
     * @Route("/achievement/{id}", name="achievement_delete")
     * @Method("POST")
     */
    public function deleteAchievementAction(Achievement $achievement)
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');
        $em = $this->getDoctrine()->getManager();

        $this->removeAchievement($achievement->getId());
        $em->remove($achievement);
        $em->flush();

        return new Response(null, 204);
    }

    /**
     * @Route("/achievement", name="achievement_new", options={"expose" = true})
     * @Method("POST")
     */
    public function newAchievementAction(Request $request)
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');

        $data = json_decode($request->getContent(), true);

        if ($data === null) {
            throw new BadRequestHttpException('Invalid JSON');
        }

        $form = $this->createForm(AchievementType::class, null, [
            'csrf_protection' => false,
        ]);

        $form->submit($data);

        if (!$form->isValid()) {
            $errors = $this->getErrorsFromForm($form);
            return $this->createApiResponse([
                'errors' => $errors
            ], 400);
        }

        $em = $this->getDoctrine()->getManager();
        try {
            $em->getConnection()->beginTransaction();

            /** @var Achievement $achievement */
            $achievement = $form->getData();
            $em->persist($achievement);
            $em->flush();
            foreach ($data['reqChallenges'] as $chalReq)
                $em->persist(new AchvChallengeReqs($achievement->getId(), $chalReq[0], $chalReq[1]));
            foreach ($data['reqSports'] as $spReq)
                $em->persist(new AchvSportReqs($achievement->getId(), $spReq[0], $spReq[1]));
            foreach ($data['reqChalsp'] as $chalSpReq)
                $em->persist(new AchvChalSpReqs($achievement->getId(), $chalSpReq[0], $chalSpReq[1]));
            foreach ($data['reqAchievements'] as $achvReq)
                $em->persist(new AchvAchvReqs($achievement->getId(), $achvReq));

            $em->flush();
            $em->getConnection()->commit();
        }
        catch (ContextErrorException $e){
            $em->getConnection()->rollBack();
            throw new BadRequestHttpException();
        }

        $response = new Response(null, 204);
        // setting the Location header... it's a best-practice
        $response->headers->set(
            'Location',
            $this->generateUrl('achievement_get', ['name' => $achievement->getName()])
        );

        return $response;
    }

    /**
     * @Route("/achievements", name="achievements", options={"expose" = true})
     */
    public function achievementsAction()
    {

        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');
        $allAchvsRaw = $this->getDoctrine()->getRepository('AppBundle:Achievement')
            ->findBy(array(),array('id' => 'DESC'));
        $allChalsRaw = $this->getDoctrine()->getRepository('AppBundle:Challenge')->findAll();
        $allSportsRaw = $this->getDoctrine()->getRepository('AppBundle:Sport')->findAll();
        $sportLogs = $this->getDoctrine()->getRepository('AppBundle:SportLog')
            ->findBy(array("user_id" => $this->getUser()->getId()),array('id' => 'DESC'));

        $achvsNames = [];
        $chalsNames = [];
        $sportsNames = [];

        foreach ($allAchvsRaw as $achievement){
            $achvsNames[$achievement->getId()] = $achievement->getName();
        }
        foreach ($allChalsRaw as $challenge){
            $chalsNames[$challenge->getId()] = $challenge->getName();
        }
        foreach ($allSportsRaw as $sport){
               $sportsNames[$sport->getId()] = $sport->getName();
        }

        $achvsData = $this->getAchvUpdateData($sportLogs, $this->getUser()->getId());
        return $this->render('achievements/achievements_list.html.twig', array(
            'all_achvs' => $allAchvsRaw,
            'achvs_names' => $achvsNames,
            'chals_names' => $chalsNames,
            'sps_names' => $sportsNames,
            'achvs_req_data' => $achvsData,
            'challenge_choices' => $allChalsRaw,
            'sport_choices' => $allSportsRaw,
            'achievement_choices' => $allAchvsRaw
        ));
    }
}