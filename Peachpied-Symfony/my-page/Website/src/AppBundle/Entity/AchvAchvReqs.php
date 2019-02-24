<?php

namespace AppBundle\Entity;

use Doctrine\ORM\Mapping as ORM;
use Symfony\Component\Validator\Constraints as Assert;

/**
 * @ORM\Table(name="achvachvreqs")
 * @ORM\Entity()
 */
class AchvAchvReqs
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
     * @ORM\ManyToOne(targetEntity="Achievement")
     * @ORM\Id @ORM\JoinColumn(name="achvReqId", referencedColumnName="id")
     */
    protected $achvreqid;

    public function __construct($achvId, $achvReqId){
        $this->achvid = $achvId;
        $this->achvreqid = $achvReqId;
    }

    /**
     * @param int $achvId
     */
    public function setAchvId($achvId)
    {
        $this->achvid = $achvId;
    }

    /**
     * @param int $achvReqId
     */
    public function setAchvReqId($achvReqId)
    {
        $this->achvreqid = $achvReqId;
    }

    /**
     * @return int
     */
    public function getAchvId()
    {
        return $this->achvid;
    }

    /**
     * @return int
     */
    public function getAchvReqId()
    {
        return $this->achvreqid;
    }

    public function getFirst(){
        return $this->achvreqid;
    }

    public function getSecond(){
        return null;
    }
}