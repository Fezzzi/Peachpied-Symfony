<?php

namespace AppBundle\Entity;

use Doctrine\ORM\Mapping as ORM;
use Symfony\Component\Validator\Constraints as Assert;

/**
 * @ORM\Table(name="achvchalspreqs")
 * @ORM\Entity()
 */
class AchvChalSpReqs
{
    /**
     * @var integer
     *
     * @ORM\Column(type="integer")
     * @ORM\ManyToOne(targetEntity="Achievement")
     * @ORM\Id  @ORM\JoinColumn(name="achvId", referencedColumnName="id")
     */
    protected $achvid;

    /**
     * @var integer
     *
     * @ORM\Column(type="integer")
     * @ORM\ManyToOne(targetEntity="Challenge")
     * @ORM\Id  @ORM\JoinColumn(name="$challengeId", referencedColumnName="id")
     */
    protected $challengeid;

    /**
     * @var integer
     *
     * @ORM\Column(type="integer")
     * @ORM\ManyToOne(targetEntity="Sport")
     * @ORM\Id  @ORM\JoinColumn(name="$sportId", referencedColumnName="id")
     */
    protected $sportid;

    public function __construct($achvId, $chalId, $spId){
        $this->achvid = $achvId;
        $this->challengeid = $chalId;
        $this->sportid = $spId;
    }

    /**
     * @return int
     */
    public function getAchvId()
    {
        return $this->achvid;
    }

    /**
     * @param int $achvId
     */
    public function setAchvId($achvId)
    {
        $this->achvid = $achvId;
    }

    /**
     * @return int
     */
    public function getChallengeId()
    {
        return $this->challengeid;
    }

    /**
     * @param int $challengeId
     */
    public function setChallengeId($challengeId)
    {
        $this->challengeid = $challengeId;
    }

    /**
     * @return int
     */
    public function getSportId()
    {
        return $this->sportid;
    }

    /**
     * @param int $sportId
     */
    public function setSportId($sportId)
    {
        $this->sportid = $sportId;
    }

    public function getFirst(){
        return $this->challengeid;
    }

    public function getSecond(){
        return $this->sportid;
    }
}