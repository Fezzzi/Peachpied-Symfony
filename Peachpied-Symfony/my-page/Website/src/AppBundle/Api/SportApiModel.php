<?php

namespace AppBundle\Api;

class SportApiModel
{
    public $name;

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