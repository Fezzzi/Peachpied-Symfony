<?php

namespace AppBundle\Api;

class SportLogApiModel
{
    public $id;

    public $user;

    public $sport;

    public $challenge;

    public $points;

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