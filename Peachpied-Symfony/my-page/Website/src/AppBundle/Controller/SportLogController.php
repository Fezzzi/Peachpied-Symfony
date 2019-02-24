<?php

namespace AppBundle\Controller;

use AppBundle\Api\SportLogApiModel;
use AppBundle\Entity\SportLog;
use Psr\Log\LoggerInterface;
use AppBundle\Form\Type\SportLogType;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Route;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Method;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\HttpKernel\Exception\BadRequestHttpException;
use Symfony\Component\HttpKernel\Exception\NotFoundHttpException;

class SportLogController extends HelperController
{
    /**
     * @Route("/sportlog/{id}", name="sport_log_get")
     * @Method("GET")
     */
    public function getSportLogAction(SportLog $sportLog)
    {
        $apiModel = $this->createSportLogApiModel($sportLog);
        $apiModel = array();

        return $this->createApiResponse($apiModel);
    }

    /**
     * @Route("/sportlog/{id}", name="sport_log_delete", options={"expose" = true})
     * @Method("POST")
     */
    public function deleteSportLogAction(SportLog $sportLog)
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');
        $em = $this->getDoctrine()->getManager();
        $em->remove($sportLog);
        $em->flush();

        return new Response(null, 204);
    }

    /**
     * @Route("/sportlog", name="sport_log_new", options={"expose" = true})
     * @Method("POST")
     */
    public function newSportLogAction(Request $request)
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');
        $data = json_decode($request->getContent(), true);
        if ($data === null) {
            throw new BadRequestHttpException('Invalid JSON');
        }

        $data['user_id'] = $this->getUser()->getId();
        $data = $this->deInject($data);

        $form = $this->createForm(SportLogType::class, null, [
            'csrf_protection' => false,
            'entity_manager' => $this->getDoctrine()->getManager()
        ]);
        $form->submit($data);
        if (!$form->isValid()) {
            $errors = $this->getErrorsFromForm($form);
            return $this->createApiResponse([
                'errors' => $errors
            ], 400);
        }

        /** @var SportLog $sportLog */
        $sportLog = $form->getData();
        $em = $this->getDoctrine()->getManager();
        $em->persist($sportLog);
        $em->flush();
        $response = new Response(null, 204);
        // setting the Location header... it's a best-practice
        $response->headers->set(
            'Location',
            $this->generateUrl('sport_log_get', ['id' => $sportLog->getId()])
        );

        return $response;
    }

    /**
     * @Route("/sportlogs", name="sport_logs", options={"expose"=true})
     */
    public function indexAction(Request $request)
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');

        $form = $this->createForm(SportLogType::class, null, array(
            'entity_manager' => $this->getDoctrine()->getManager()
        ));
        $form->handleRequest($request);

        if ($form->isValid()) {
            $em = $this->getDoctrine()->getManager();

            $sportLog = $form->getData();
            $sportLog->setUser($this->getUser());

            $em->persist($sportLog);
            $em->flush();

            $this->addFlash('notice', 'sport logged!');

            return $this->redirectToRoute('sport_logs');
        }
        $chal_choices = $this->getDoctrine()->getRepository('AppBundle:Challenge')->findAll();
        $sport_choices = $this->getDoctrine()->getRepository('AppBundle:Sport')->findAll();
        $sportLogs = $this->getDoctrine()->getRepository('AppBundle:SportLog')->findBy(array("user_id" => $this->getUser()->getId()),array('id' => 'DESC'));
        $uncompleted_challenges = [];
        $uncompleted_sports = [];
        $challenge_choices = [];
        foreach ($sport_choices as $sport){
            $uncompleted_sports[$sport->getId()] = $sport -> getMultiplier();
        }
        foreach($chal_choices as $challenge) {
            $uncompleted_challenges[$challenge->getId()] = $challenge->getPoints();
            $challenge_choices[$challenge->getId()] = $challenge;
        }


        $images = array();
        foreach ($sportLogs as $key => $dat) {
            $img = $dat->getImage();
            if($img == null)
                $sportLogs[$key]->setImage(null);
            else {
                $images[$key] = stream_get_contents($dat->getImage());
                $sportLogs[$key]->setImage($images[$key]);
            }
        }

        return $this->render('sportLogs/sportLog.html.twig', array(
            'form' => $form->createView(),
            'challenge_choices' => $challenge_choices,
            'sport_choices' => $sport_choices,
            'uncompleted_challenges' => $uncompleted_challenges,
            'uncompleted_sports' => $uncompleted_sports,
            'sport_logs' => $sportLogs,
            'achvs_update_data' => $this->getAchvUpdateData($sportLogs, $this->getUser()->getId()),
            'leaderboard' => $this->getLeaders(),
        ));
    }

    /**
     * Returns an array of leader information
     *
     * @return array
     */
    private function getLeaders()
    {
        $leaderboardDetails = $this->getDoctrine()->getRepository('AppBundle:SportLog')
            ->getLeaderboardDetails()
        ;

        $users = $this->getDoctrine()->getRepository('AppBundle:User')->findAll();
        $usersMap = [];
        foreach($users as $user){
            $usersMap[$user->getId()] = $user->getUsername();
        }

        $leaderboard = array();
        foreach ($leaderboardDetails as $details) {
            if(isset($details['user_id'], $usersMap)) {

                $achvLogs = $this->getDoctrine()->getRepository('AppBundle:AchvLog')->findBy(array("user_id" => $details['user_id']));
                $achievementTypes = ['gold' => 0, 'silver' => 0, 'bronze' => 0];
                foreach ($achvLogs as $achievement) {
                    ++$achievementTypes[$achievement->getType()];
                }

                $leaderboard[] = array(
                    'username' => $usersMap[$details['user_id']],
                    'points' => $details['points'],
                    'challenges' => $details['challenges'],
                    'achv_types' => $achievementTypes,
                    'id' => $details['user_id']
                );

                $usersMap[$details['user_id']] = null;
            }
        }

        foreach ($usersMap as $key=>$user){
            if($user !== null) {
                $achvLogs = $this->getDoctrine()->getRepository('AppBundle:AchvLog')->findBy(array("user_id" => $key));
                $achievementTypes = ['gold' => 0, 'silver' => 0, 'bronze' => 0];
                foreach ($achvLogs as $achievement) {
                    ++$achievementTypes[$achievement->getType()];
                }

                $leaderboard[] = array(
                    'username' => $user,
                    'points' => '0',
                    'challenges' => '0',
                    'achv_types' => $achievementTypes,
                    'id' => $key
                );
            }
        }

        return $leaderboard;
    }
}
