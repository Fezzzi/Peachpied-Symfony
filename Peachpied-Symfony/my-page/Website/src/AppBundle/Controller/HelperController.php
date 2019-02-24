<?php

namespace AppBundle\Controller;

use AppBundle\Api\SportLogApiModel;
use AppBundle\Api\SportApiModel;
use AppBundle\Api\ChallengeApiModel;
use AppBundle\Api\AchievementApiModel;
use AppBundle\Api\AchvLogApiModel;
use AppBundle\Entity\AchvLog;
use AppBundle\Entity\SportLog;
use AppBundle\Entity\Sport;
use AppBundle\Entity\Challenge;
use AppBundle\Entity\Achievement;
use Symfony\Bundle\FrameworkBundle\Controller\Controller;
use Symfony\Component\Form\FormInterface;
use Symfony\Component\HttpFoundation\JsonResponse;

class HelperController extends Controller
{
    /**
     * @param mixed $data Usually an object you want to serialize
     * @param int $statusCode
     * @return JsonResponse
     */
    protected function createApiResponse($data, $statusCode = 200)
    {
        $json = $this->get('serializer')
            ->serialize($data, 'json');

        return new JsonResponse($json, $statusCode, [], true);
    }

    /**
     * Returns an associative array of validation errors
     *
     * {
     *     'firstName': 'This value is required',
     *     'subForm': {
     *         'someField': 'Invalid value'
     *     }
     * }
     *
     * @param FormInterface $form
     * @return array|string
     */
    protected function getErrorsFromForm(FormInterface $form)
    {
        foreach ($form->getErrors() as $error) {
            // only supporting 1 error per field
            // and not supporting a "field" with errors, that has more
            // fields with errors below it
            return $error->getMessage();
        }

        $errors = array();
        foreach ($form->all() as $childForm) {
            if ($childForm instanceof FormInterface) {
                if ($childError = $this->getErrorsFromForm($childForm)) {
                    $errors[$childForm->getName()] = $childError;
                }
            }
        }

        return $errors;
    }

    /**
     * Turns a SportLog into a SportLogApiModel for the API.
     *
     * This could be moved into a service if it needed to be
     * re-used elsewhere.
     *
     * @param SportLog $sportLog
     * @return SportLogApiModel
     */
    protected function createSportLogApiModel(SportLog $sportLog)
    {
        $model = new SportLogApiModel();
        $model->id = $sportLog->getId();
        $model->sport = $sportLog->getSport();
        $model->challenge = $sportLog->getChallenge();
        $model->points = $sportLog->getPoints();

        $selfUrl = $this->generateUrl(
            'sport_log_get',
            ['id' => $sportLog->getId()]
        );
        $model->addLink('_self', $selfUrl);

        return $model;
    }

    /**
     * Turns a Sport into a SportApiModel for the API.
     *
     * This could be moved into a service if it needed to be
     * re-used elsewhere.
     *
     * @param Sport $sport
     * @return SportApiModel
     */
    protected function createSportApiModel(Sport $sport)
    {
        $model = new SportApiModel();
        $model->name = $sport->getName();
        $model->description = $sport->getDescription();
        $model->image = $sport->getImage();

        $selfUrl = $this->generateUrl(
            'sport_get',
            ['id' => $sport->getId()]
        );
        $model->addLink('_self', $selfUrl);

        return $model;
    }

    /**
     * Turns a Challenge into a ChallengeApiModel for the API.
     *
     * This could be moved into a service if it needed to be
     * re-used elsewhere.
     *
     * @param Challenge $challenge
     * @return ChallengeApiModel
     */
    protected function createChallengeApiModel(Challenge $challenge)
    {
        $model = new ChallengeApiModel();
        $model->name = $challenge->getName();
        $model->description = $challenge->getDescription();
        $model->points = $challenge->getPoints();
        $model->image = $challenge->getImage();

        $selfUrl = $this->generateUrl(
            'challenge_get',
            ['id' => $challenge->getId()]
        );
        $model->addLink('_self', $selfUrl);

        return $model;
    }

    /**
     * Turns a Challenge into a ChallengeApiModel for the API.
     *
     * This could be moved into a service if it needed to be
     * re-used elsewhere.
     *
     * @param Achievement $achievement
     * @return AchievementApiModel
     */
    protected function createAchievementApiModel(Achievement $achievement)
    {
        $model = new AchievementApiModel();
        $model->name = $achievement->getName();
        $model->type = $achievement->getType();
        $model->reqAchievements = $achievement->getReqAchievements();
        $model->reqChallenges = $achievement->getReqChallenges();
        $model->reqChalsp = $achievement->getReqChalsp();
        $model->reqSports = $achievement->getReqSports();

        $selfUrl = $this->generateUrl(
            'achievement_get',
            ['id' => $achievement->getId()]
        );
        $model->addLink('_self', $selfUrl);

        return $model;
    }

    /**
     * @param AchvLog $achvLog
     * @return AchievementApiModel
     */
    protected function createAchvLogApiModel(AchvLog $achvLog)
    {
        $model = new AchvLogApiModel();
        $model->user_id = $achvLog->getUserId();
        $model->achv_id = $achvLog->getAchvId();
        $model->date = $achvLog->getDate();

        $selfUrl = $this->generateUrl(
            'achievementLog_get',
            ['id' => $achvLog->getId()]
        );
        $model->addLink('_self', $selfUrl);

        return $model;
    }

    /**
     * @return SportLogApiModel[]
     */
    protected function findAllUsersSportLogModels()
    {
        $sportLogs = $this->getDoctrine()->getRepository('AppBundle:SportLog')
            ->findBy(array('user' => $this->getUser()))
        ;

        $models = [];
        foreach ($sportLogs as $sportLog) {
            $models[] = $this->createSportLogApiModel($sportLog);
        }

        return $models;
    }

    /**
     * @return SportApiModel[]
     */
    protected function findAllSportsModels()
    {
        $sports = $this->getDoctrine()->getRepository('AppBundle:Sport')
            ->findAll()
        ;

        $models = [];
        foreach ($sports as $sport) {
            $models[] = $this->createSportApiModel($sport);
        }
        return $models;
    }

    /**
     * @return ChallengeApiModel[]
     */
    protected function findAllChallengesModels()
    {
        $challenges = $this->getDoctrine()->getRepository('AppBundle:Challenge')
            ->findAll()
        ;

        $models = [];
        foreach ($challenges as $challenge) {
            $models[] = $this->createChallengeApiModel($challenge);
        }
        return $models;
    }

    /**
     * @return AchievementApiModel[]
     */
    protected function findAllAchievementsModels()
    {
        $achievements = $this->getDoctrine()->getRepository('AppBundle:Achievement')
            ->findAll()
        ;

        $models = [];
        foreach ($achievements as $achievement) {
            $models[] = $this->createAchievementApiModel($achievement);
        }
        return $models;
    }

    /**
     * Returns an array of user's Sport Log information
     *
     * @return array
     */
    protected function getAchvUpdateData($sportLog, $userId)
    {
        $chals = [];
        $sps = [];
        $chalsps = [];
        $nullChalIndex = 0;
        $nullSpIndex = 0;
        foreach ($sportLog as $sportLog) {
            if($sportLog->getChallengeid() !== null)
                $idchal = $sportLog->getChallengeid();
            else {
                $idchal = "nullChal" . $nullChalIndex;
                ++$nullChalIndex;
            }
            if($sportLog->getSportId() !== null)
                $idsp = $sportLog->getSportId();
            else {
                $idsp = "nullChal" . $nullSpIndex;
                ++$nullSpIndex;
            }
            if(isset($chals[$idchal]))
                $chals[$idchal] = $chals[$idchal] + 1;
            else
                $chals[$idchal] = 1;

            if(isset($sps[$idsp]))
                $sps[$idsp] = $sps[$idsp] + 1;
            else
                $sps[$idsp] = 1;

            if(isset($chalsps[$idchal]))
                $chalsps[$idchal][$idsp] = null;
            else
                $chalsps[$idchal] = array($idsp => null);
        }

        $achvsAll = $this->getDoctrine()->getRepository('AppBundle:Achievement')->findAll();
        $lockedAchvs = [];
        foreach ($achvsAll as $achievement) {
            $lockedAchvs[$achievement->getId()] = array(
                'type' => $achievement->getType(),
                'name' => $achievement->getName());

            $lockedAchvs[$achievement->getId()]['achvs'] = $this
                ->reshapeUpdateData($achievement->getId(), 'AchvAchvReqs');
            $lockedAchvs[$achievement->getId()]['chalsps'] = $this
                ->reshapeUpdateData($achievement->getId(), 'AchvChalSpReqs');
            $lockedAchvs[$achievement->getId()]['sps'] = $this
                ->reshapeUpdateData($achievement->getId(), 'AchvSportReqs');
            $lockedAchvs[$achievement->getId()]['chals'] = $this
                ->reshapeUpdateData($achievement->getId(), 'AchvChallengeReqs');
        }

        $achvs = [];
        $nullIndex = 0;
        $achvsLog = $this->getDoctrine()->getRepository('AppBundle:AchvLog')
            ->findBy(array("user_id" => $userId));
        foreach ($achvsLog as $achievement){
            if(isset($lockedAchvs[$achievement->getAchvId()]))
                unset($lockedAchvs[$achievement->getAchvId()]);
            if($achievement->getAchvId() !== null)
                $index = $achievement->getAchvId();
            else {
                $index = "null" . $nullIndex;
                ++$nullIndex;
            }

            $achvs[$index] = [
                "name" => $achievement->getName(),
                "type" => $achievement->getType(),
                "date" => $achievement->getDate()];
        }
        return array('userdata' => array(
            'chals' => $chals, 'sps' => $sps,
            'chalsps' => $chalsps, 'achvs' => $achvs),
            'lockedachvs' => $lockedAchvs);
    }

    private function reshapeUpdateData($key, $appBundle){
        $reqs = $this->getDoctrine()->getRepository('AppBundle:' . $appBundle)
            ->findBy(array('achvid' => $key));
        $arr = [];
        foreach ($reqs as $reqObj){
            $arr[$reqObj->getFirst()] = $reqObj->getSecond();
        }
        return $arr;
    }

    public function removeAchievement($id){
        $em = $this->getDoctrine()->getManager();
        $logs = $this->getDoctrine()->getRepository('AppBundle:AchvLog')->findBy(array('achv_id' => $id));
        foreach($logs as $log)
            $log->setAchvId(null);

        $reqs1 = $this->getDoctrine()->getRepository('AppBundle:AchvAchvReqs')
            ->findBy(array('achvid' => $id));
        $reqs2 = $this->getDoctrine()->getRepository('AppBundle:AchvChallengeReqs')
            ->findBy(array('achvid' => $id));
        $reqs3 = $this->getDoctrine()->getRepository('AppBundle:AchvChalSpReqs')
            ->findBy(array('achvid' => $id));
        $reqs4 = $this->getDoctrine()->getRepository('AppBundle:AchvSportReqs')
            ->findBy(array('achvid' => $id));
        foreach([$reqs1,$reqs2,$reqs3,$reqs4] as $reqs)
            foreach ($reqs as $req){
                $em->remove($req);
            }

        $reqs0 = $this->getDoctrine()->getRepository('AppBundle:AchvAchvReqs')->findBy(array('achvreqid' => $id));
        foreach($reqs0 as $req){
            $this->removeAchievement($req->getAchvId());
            $em->remove($this->getDoctrine()->getRepository('AppBundle:Achievement')->findBy(array('id' => $req->getAchvId()))[0]);
            $em->remove($req);
        }
    }

    public function deInject($data){
        if(isset($data[0]) and $data[0] == "Gender")
            foreach ($data[1] as $key=>$value)
                $data[1][$key] = strip_tags($value);
        else
            foreach ($data as $key=>$value)
                $data[$key] = strip_tags($value);

        return $data;
    }
}