<?php

namespace AppBundle\Api;

class AchievementApiModel
{
    public $name;

    public $type;

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