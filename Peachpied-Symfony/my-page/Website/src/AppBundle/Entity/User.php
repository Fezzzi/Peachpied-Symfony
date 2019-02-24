<?php

namespace AppBundle\Entity;

use FOS\UserBundle\Model\User as BaseUser;
use Doctrine\ORM\Mapping as ORM;
use Symfony\Component\Validator\Constraints as Assert;

/**
 * @ORM\Entity
 * @ORM\Table(name="users")
 */
class User extends BaseUser
{
    /**
     * @ORM\Id
     * @ORM\Column(type="integer")
     * @ORM\GeneratedValue(strategy="AUTO")
     */
    protected $id;

    /**
     * @Assert\NotBlank
     * @ORM\Column(name="teamMember", type="string", length=10)
     */
    protected $teamMember;

    /**
     * @var string
     *
     * @ORM\Column(name="tMemberGender", type="string", length=15)
     */
    protected $tMemberGender;

    /**
     * @return mixed
     */
    public function getTMemberGender()
    {
        return $this->tMemberGender;
    }

    /**
     * @param mixed $tMemberGender
     */
    public function setTMemberGender($tMemberGender)
    {
        $this->tMemberGender = $tMemberGender;
    }

    /**
     * @ORM\Column(name="avatar", type="blob")
     */
    protected $avatar;

    /**
     * @return mixed
     */
    public function getAvatar()
    {
        if($this->avatar === "" || $this->avatar === null)
            return "/assets/images/defaultAvatar.png";
        if(is_string($this->avatar))
            return $this->avatar;
        return stream_get_contents($this->avatar);
    }

    /**
     * @param mixed $avatar
     */
    public function setAvatar($avatar)
    {
        $this->avatar = $avatar;
    }

    /**
     * @return mixed
     */
    public function getTeamMember()
    {
        return $this->teamMember;
    }

    /**
     * @param mixed $teamMember
     */
    public function setTeamMember($teamMember)
    {
        $this->teamMember = $teamMember;
    }

    /**
     * @return mixed
     */
    public function getId()
    {
        return $this->id;
    }

}