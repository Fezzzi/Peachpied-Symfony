<?php

namespace AppBundle\Entity;

use Doctrine\ORM\Mapping as ORM;
use Symfony\Component\Validator\Constraints as Assert;

/**
 * @ORM\Table(name="challenges")
 * @ORM\Entity
 */
class Challenge
{
    /**
     * @var integer
     *
     * @ORM\Id
     * @ORM\Column(type="integer")
     * @ORM\GeneratedValue(strategy="AUTO")
     */
    protected $id;

    /**
     * @var string
     *
     * @Assert\NotBlank(message="What's the name of the challenge?")
     * @ORM\Column(name="name", type="string", length=15)
     */
    protected $name;

    /**
     * @Assert\NotBlank(message="Let's describe the challenge a bit!")
     * @ORM\Column(name="description", type="text")
     */
    protected $description;

    /**
     * @var integer
     *
     * @Assert\NotBlank(message="How many points is it worth?")
     * @Assert\GreaterThan(value=0, message="You can certainly give more!")
     * @Assert\LessThan(value=100, message="You may be too generous!")
     * @ORM\Column(name="points", type="integer")
     */
    protected $points;

    /**
     * @ORM\Column(name="image", type="blob")
     */
    protected $image;

    /**
     * @return mixed
     */
    public function getName()
    {
        return $this->name;
    }

    /**
     * @param mixed $name
     */
    public function setName($name)
    {
        $this->name = $name;
    }

    /**
     * @return mixed
     */
    public function getDescription()
    {
        return $this->description;
    }

    /**
     * @param mixed $description
     */
    public function setDescription($description)
    {
        $this->description = $description;
    }

    /**
     * @return mixed
     */
    public function getPoints()
    {
        return $this->points;
    }

    /**
     * @param mixed $points
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
     * @param mixed $points
     */
    public function setImage($image)
    {
        $this->image = $image;
    }

    /**
     * @return int
     */
    public function getId()
    {
        return $this->id;
    }

}