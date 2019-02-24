<?php

namespace AppBundle\Entity;

use Doctrine\ORM\Mapping as ORM;
use Symfony\Component\Serializer\Annotation as Serializer;
use Symfony\Component\Validator\Constraints as Assert;
use Doctrine\DBAL\Types\DateTimeType;
use Doctrine\DBAL\Types\ConversionException;

/**
 * SportLog
 *
 * @ORM\Table(name="sport_log")
 * @ORM\Entity(repositoryClass="AppBundle\Repository\SportLogRepository")
 */
class SportLog
{
    /**
     * @var integer
     *
     * @Serializer\Groups({"Default"})
     * @ORM\Column(name="id", type="integer")
     * @ORM\Id
     * @ORM\GeneratedValue(strategy="AUTO")
     */
    private $id;

    /**
     * @ORM\Column(type="integer")
     * @ORM\ManyToOne(targetEntity="Sport")
     * @ORM\JoinColumn(name="sportId", referencedColumnName="id")
     */
    private $sportid;

    /**
     * @ORM\Column(name="sportName", type="string", length=15)
     * @Assert\NotBlank(message="How did you make it?")
     */
    private $sport;

    /**
     * @return mixed
     */
    public function getSportid()
    {
        return $this->sportid;
    }

    /**
     * @param mixed $sportid
     */
    public function setSportid($sportid)
    {
        $this->sportid = $sportid;
    }

    /**
     * @return mixed
     */
    public function getChallengeid()
    {
        return $this->challengeid;
    }

    /**
     * @param mixed $challengeid
     */
    public function setChallengeid($challengeid)
    {
        $this->challengeid = $challengeid;
    }

    /**
     * @ORM\Column(type="integer")
     * @ORM\ManyToOne(targetEntity="Challenge")
     * @ORM\JoinColumn(name="challengeId", referencedColumnName="id")
     */
    private $challengeid;

    /**
     * @ORM\Column(name="challengeName", type="string", length=15)
     * @Assert\NotBlank(message="What challenge?")
     */
    private $challenge;

    /**
     *
     * @Serializer\Groups({"Default"})
     * @ORM\Column(name="points", type="decimal", precision=1)
     */
    private $points;

    /**
     * The user who completed this challenge
     *
     * @var User
     *
     * @ORM\Column(type="integer")
     * @ORM\ManyToOne(targetEntity="User")
     * @ORM\JoinColumn(name="user_id", referencedColumnName="id", nullable=false)
     * @Assert\NotBlank
     */
    private $user_id;

    /**
     * @Serializer\Groups({"Default"})
     * @ORM\Column(name="date", type="datetime")
     */
    private $date;

    /**
     * @return mixed
     */
    public function getDate()
    {
        return $this->date;
    }

    /**
     * @param mixed $date
     */
    public function setDate($date)
    {
        $this->date = new \DateTime("now");
    }

    /**
     * @return User
     */
    public function getUserId()
    {
        return $this->user_id;
    }

    /**
     * @param User $user_id
     */
    public function setUserId($user_id)
    {
        $this->user_id = $user_id;
    }

    /**
     *
     * @ORM\Column(name="image", type="blob")
     */
    private $image;

    /**
     * @return integer
     */
    public function getId()
    {
        return $this->id;
    }

    /**
     *
     * @param string
     * @return SportLog
     */
    public function setSport($sport)
    {
        $this->sport = $sport;

        return $this;
    }

    /**
     * @return string
     */
    public function getSport()
    {
        return $this->sport;
    }

    /**
     *
     * @param string
     * @return SportLog
     */
    public function setChallenge($challenge)
    {
        $this->challenge = $challenge;

        return $this;
    }

    /**
     * @return string
     */
    public function getChallenge()
    {
        return $this->challenge;
    }

    /**
     * Get points
     *
     * @return float
     */
    public function getPoints()
    {
        return $this->points;
    }

    /**
     * @param float $points
     */
    public function setPoints($points)
    {
        $this->points = $points;
    }

    /**
     * @return mixed
     */
    public function getImage()
    {
        return $this->image;
    }

    /**
     * @param mixed $image
     */
    public function setImage($image)
    {
        $this->image = $image;
    }

}
