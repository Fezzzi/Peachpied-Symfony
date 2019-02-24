<?php

namespace AppBundle\Api;

class ChallengeApiModel
{
    public $name;

    public $points;

    public $description;

    public $image;

    private $links = [];

    public function addLink($ref, $url)
    {
        $this->links[$ref] = $url;
    }

    public function getLinks()
    {
        return $this->links;
    }
}