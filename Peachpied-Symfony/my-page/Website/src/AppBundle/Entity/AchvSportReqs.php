<?php

namespace AppBundle\Entity;

use Doctrine\ORM\Mapping as ORM;
use Symfony\Component\Validator\Constraints as Assert;

/**
 * @ORM\Table(name="achvsportreqs")
 * @ORM\Entity()
 */
class AchvSportReqs
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
     * @ORM\ManyToOne(targetEntity="Sport")
     * @ORM\Id @ORM\JoinColumn(name="sportId", referencedColumnName="id")
     */
    protected $sportid;

    /**
     * @var integer
     *
     *
     * @ORM\Column(name="count", type="integer")
     */
    protected $count;

    public function __construct($achvId, $spId, $count){
        $this->achvid = $achvId;
        $this->sportid = $spId;
        $this->count = $count;
    }

    /**
     * @return int
     */
    public function getAchvid()
    {
        return $this->achvid;
    }

    /**
     * @param int $achvid
     */
    public function setAchvId($achvid)
    {
        $this->achvid = $achvid;
    }

    /**
     * @return int
     */
    public function getSportid()
    {
        return $this->sportid;
    }

    /**
     * @param int $sportId
     */
    public function setSportId($sportId)
    {
        $this->sportId = $sportId;
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

    public function getFirst(){
        return $this->sportid;
    }

    public function getSecond(){
        return $this->count;
    }
}