<?php

namespace AppBundle\Controller;

use AppBundle\Entity\User;
use AppBundle\Entity\Message;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Route;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Method;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpKernel\Exception\BadRequestHttpException;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Security\Core\Exception\AuthenticationException;
use Symfony\Component\Security\Core\Security;
use FOS\UserBundle\Event\FilterUserResponseEvent;
use FOS\UserBundle\Event\FormEvent;
use FOS\UserBundle\Event\GetResponseUserEvent;
use FOS\UserBundle\Form\Factory\FactoryInterface;
use FOS\UserBundle\FOSUserEvents;
use FOS\UserBundle\Model\UserManagerInterface;
use Symfony\Component\EventDispatcher\EventDispatcherInterface;
use Symfony\Component\HttpFoundation\RedirectResponse;

class ProfileController extends HelperController
{
    /**
     * @Route("/profile/delete", name="profile_delete", options={"expose" = true})
     * @Method("POST")
     */
    public function profileDeleteAction()
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');

        $em = $this->getDoctrine()->getManager();
        $achvLogs = $em->getRepository('AppBundle:AchvLog')->findBy(array("user_id" => $this->getUser()->getId()));
        foreach($achvLogs as $achvLog){
            $em -> remove($achvLog);
        }
        $em->flush();

        $sportLogs = $em->getRepository('AppBundle:SportLog')->findBy(array("user_id" => $this->getUser()->getId()));
        foreach($sportLogs as $sportLog){
            $em -> remove($sportLog);
        }
        $em->flush();

        $messages = $em->getRepository('AppBundle:Message')->findBy(array("user_id" => $this->getUser()->getId()));
        foreach ($messages as $message)
            $message->setUserId(null);

        $em->flush();

        $userManager = $this->get('fos_user.user_manager');
        $user = $this->getUser();
        $userManager->deleteUser($user);

        return new Response(null, 204);
    }

    /**
     * @Route("/profile/deleteImg", name="profile_delete_img", options={"expose" = true})
     * @Method("POST")
     */
    public function profileDeleteImg(Request $request)
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');

        $data = json_decode($request->getContent(), true);
        if($data[1] != $this->getUser()->getId())
            throw new BadRequestHttpException('Invalid Action');

        $em = $this->getDoctrine()->getManager();
        $sportLog = $em->getRepository('AppBundle:SportLog')->findBy(array("id" => $data[0]));
        $sportLog[0]->setImage(null);
        $em->flush();
        return new Response(null, 204);
    }

    /**
     * @Route("/profile/edit", name="profile_edit", options={"expose" = true})
     * @Method("POST")
     */
    public function profileChangeAction(Request $request)
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');

        $data = json_decode($request->getContent(), true);
        $this->deInject($data);

        if ($data === null) {
            throw new BadRequestHttpException('Invalid JSON');
        }

        $em = $this->getDoctrine()->getManager();
        $user = $em->getRepository('AppBundle:User')->findBy(array("id" => $this->getUser()->getId()));
        if($data[1] == "Username"){
            if($this->unique('username', $data[0]))
                $user[0]->setUsername($data[0]);
            else
                return new Response(null, 404);
        }
        else if($data[1] == "Email"){
            if($this->unique('email', $data[0]))
                $user[0]->setEmail($data[0]);
            else
                return new Response(null, 404);
        }
        else if($data[1] == "Name 1"){
            $user[0]->setTeamMember($data[0]);
        }
        else if($data[1] == "Avatar"){
            $user[0]->setAvatar($data[0]);
        }
        else if($data[0] == "Gender"){
            $user[0]->setTMemberGender(($data[1])[0]);
        }
        else
            throw new BadRequestHttpException('Invalid JSON');

        $em->persist($user[0]);
        $em->flush();
        return new Response(null, 204);
    }

    private function unique($element, $value){
        $result = $this->getDoctrine()->getRepository('AppBundle:User')->findBy(array($element => $value));
        if($result == [])
            return true;
        else
            return false;
    }

    /**
     * @Route("/profile", name="profile")
     */
    public function profileAction()
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');
        return $this->getProfileView($this->getUser()->getId(), true);
    }

    /**
     * @Route("/profile/{id}", name="profile_visit", options={"expose" = true})
     */
    public function profileVisit(User $user)
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');
        return $this->getProfileView( $user->getId(), false);
    }

    private function getProfileView($id, $edit){
        $user = $this->getDoctrine()->getRepository('AppBundle:User')->findBy(array("id" => $id));
        $sportLogs = $this->getDoctrine()->getRepository('AppBundle:SportLog')
            ->findBy(array("user_id" => $id));
        $allChallenges = $this->getDoctrine()->getRepository('AppBundle:Challenge')->findAll();
        $allSports = $this->getDoctrine()->getRepository('AppBundle:Sport')->findAll();
        $completedChallenges = [];
        $completedSports = [];
        $challenges =[];
        $achievementTypes = ['bronze' => 0, 'silver' => 0, 'gold' => 0];
        $points = 0;
        $maxChallenge = ['name' => '-', 'count' => 0];
        $maxSport = ['name' => '-', 'count' => 0];
        $userImages = [];
        foreach($sportLogs as $sportLog){
            $img = $sportLog->getImage();
            if($img !== null) {
                array_push($userImages, [stream_get_contents($img), $sportLog->getId()]);
            }

            if($sportLog->getChallengeId() !== null){
                $challenges[$sportLog->getChallengeId()] = [
                    'name' => $sportLog->getChallenge(),
                    'date' => $sportLog->getDate()];

                if (!isset($completedChallenges[$sportLog->getChallengeId()])) {
                    $completedChallenges[$sportLog->getChallengeId()] = 1;
                } else {
                    ++$completedChallenges[$sportLog->getChallengeId()];
                }

                if($maxChallenge['count'] < $completedChallenges[$sportLog->getChallengeId()]){
                    $maxChallenge['count'] = $completedChallenges[$sportLog->getChallengeId()];
                    $maxChallenge['name'] = $sportLog->getChallenge();
                }
            };

            if($sportLog->getChallengeId() !== null){
                if (!isset($completedSports[$sportLog->getSportId()])) {
                    $completedSports[$sportLog->getSportId()] = 1;
                } else {
                    ++$completedSports[$sportLog->getSportId()];
                }

                if($maxSport['count'] < $completedSports[$sportLog->getSportId()]){
                    $maxSport['count'] = $completedSports[$sportLog->getSportId()];
                    $maxSport['name'] = $sportLog->getSport();
                }
            };

            $points += $sportLog->getPoints();
        }

        $achvData = $this->getAchvUpdateData($sportLogs, $id);

        foreach($achvData['userdata']['achvs'] as $achievement){
            ++$achievementTypes[$achievement['type']];
        }



        $user[0]->setAvatar($user[0]->getAvatar());

        return $this->render('profile.html.twig',array(
            'user_info' => $user[0],
            'user_imgs' => $userImages,
            'achv_data' => $achvData,
            'challenges' => $challenges,
            'completed_challenges' => count($completedChallenges),
            'completed_sports' => count($completedSports),
            'all_challenges' => count($allChallenges),
            'all_sports' => count($allSports),
            'max_challenge' => $maxChallenge['name'],
            'max_sport' => $maxSport['name'],
            'achv_count' => count($achvData['userdata']['achvs']),
            'achv_types' => $achievementTypes,
            'total_sport_logs' => count($sportLogs),
            'points' => $points,
            'allow_edit' => $edit
        ));
    }
}