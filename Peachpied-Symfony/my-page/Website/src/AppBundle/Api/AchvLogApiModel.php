<?php

namespace AppBundle\Api;

class AchvLogApiModel
{
    public $name;

    public $type;

    public $user_id;

    public $date;

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