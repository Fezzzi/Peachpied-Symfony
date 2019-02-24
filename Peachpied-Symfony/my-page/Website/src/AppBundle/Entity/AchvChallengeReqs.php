<?php

namespace AppBundle\Entity;

use Doctrine\ORM\Mapping as ORM;
use Symfony\Component\Validator\Constraints as Assert;

/**
 * @ORM\Table(name="achvchallengereqs")
 * @ORM\Entity()
 */
class AchvChallengeReqs
{
    /**
     * @var integer
     *
     * @ORM\Column(type="integer")
     * @ORM\ManyToOne(targetEntity="Achievement")
     * @ORM\Id @ORM\JoinColumn(name="achvId", referencedColumnName="id")
     */
    protected $achvid;

    /**
     * @var integer
     *
     * @ORM\Column(type="integer")
     * @ORM\ManyToOne(targetEntity="Challenge")
     * @ORM\Id @ORM\JoinColumn(name="challengeId", referencedColumnName="id")
     */
    protected $challengeid;

    /**
     * @var integer
     *
     * @Assert\GreaterThan(value=0, message="You can certainly give more!")
     * @Assert\LessThan(value=100, message="You may be too generous!")
     * @ORM\Column(name="count", type="integer")
     */
    protected $count;

    public function __construct($achvId, $chalId, $count){
        $this->achvid = $achvId;
        $this->challengeid = $chalId;
        $this->count = $count;
    }

    /**
     * @return int
     */
    public function getChallengeid()
    {
        return $this->challengeid;
    }

    /**
     * @param int $challengeId
     */
    public function setChallengeid($challengeId)
    {
        $this->challengeid = $challengeId;
    }

    /**
     * @return int
     */
    public function getCount()
    {
        return $this->count;

    }
    /**
     * @param int $count
     */
    public function setCount($count)
    {
        $this->count = $count;
    }

    /**
     * @param int $achvId
     */
    public function setAchvid($achvId)
    {
        $this->achvid = $achvId;
    }

    /**
     * @return int
     */
    public function getAchvid()
    {
        return $this->achvid;
    }

    public function getFirst(){
        return $this->challengeid;
    }

    public function getSecond(){
        return $this->count;
    }

}