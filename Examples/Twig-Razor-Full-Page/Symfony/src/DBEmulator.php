<?php

namespace App;

define(
    "LOREM_IPSUM",
    "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore 
    et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip 
    ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu 
    fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt 
    mollit anim id est laborum."
);

class DBEmulator {

    /**
     * Emulates data fetching from db
     *
     * @return array
     */
    public static function getRooms(){
        return array(
            [   
                "name" => "1 bed room", 
                "price" => "19€", 
                "desc" => DBEmulator::getDesc(),
                "services" => ["wifi", "minibar"], 
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => DBEmulator::getImage()
            ], [
                "name" => "1 bed room", 
                "price" => "24€", 
                "desc" => DBEmulator::getDesc(90),
                "services" => ["wifi", "minibar"], 
				"occupancy" => DBEmulator::getOccupancy(),
				"img" => DBEmulator::getImage()
            ], [
                "name" => "2 beds room", "price" => "39€",
                "desc" => DBEmulator::getDesc(170),
                "services" => ["wifi", "minibar", "parking"],
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => DBEmulator::getImage()
            ], [
                "name" => "2 beds room", "price" => "45€",
                "desc" => DBEmulator::getDesc(55),
                "services" => ["wifi", "minibar", "parking"],
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => DBEmulator::getImage()
            ], [
                "name" => "3 beds room", "price" => "52€",
                "desc" => DBEmulator::getDesc(83),
                "services" => ["wifi", "minibar", "parking", "pool"],
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => DBEmulator::getImage()
            ], [
                "name" => "4 beds room",
                "price" => "59€",
                "desc" => DBEmulator::getDesc(136),
                "services" => ["wifi", "minibar", "parking", "pool"],
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => DBEmulator::getImage()
            ], [
                "name" => "5 beds apartment",
                "price" => "79€",
                "desc" => DBEmulator::getDesc(205),
                "services" => ["wifi", "minibar", "parking", "pool"],
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => DBEmulator::getImage()
            ], [
                "name" => "6 beds apartment",
                "price" => "99€",
                "desc" => DBEmulator::getDesc(158),
                "services" => ["wifi", "minibar", "parking", "pool", "hottub"],
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => DBEmulator::getImage()
            ]
        ); 
    }

	private static function getImage() {
		$imgs = array(
			"https://cdn.pixabay.com/photo/2017/03/22/17/39/kitchen-2165756__340.jpg",
			"https://cdn.pixabay.com/photo/2017/09/09/18/25/living-room-2732939__340.jpg",
			"https://cdn.pixabay.com/photo/2015/10/20/18/57/furniture-998265__340.jpg",
			"https://cdn.pixabay.com/photo/2017/03/28/12/17/chairs-2181994__340.jpg",
			"https://cdn.pixabay.com/photo/2016/11/21/16/21/bed-1846251__340.jpg",
			"https://cdn.pixabay.com/photo/2015/05/15/14/22/conference-room-768441__340.jpg",
			"https://cdn.pixabay.com/photo/2016/11/29/08/42/desk-1868498__340.jpg",
			"https://cdn.pixabay.com/photo/2016/11/21/12/59/chair-1845270__340.jpg",
			"https://cdn.pixabay.com/photo/2019/03/30/21/03/space-4092053__340.jpg",
			"https://cdn.pixabay.com/photo/2017/03/25/23/32/kitchen-2174593__340.jpg",
			"https://cdn.pixabay.com/photo/2016/09/19/17/20/home-1680800__340.jpg",
			"https://cdn.pixabay.com/photo/2016/11/19/17/25/furniture-1840463__340.jpg",
			"https://cdn.pixabay.com/photo/2014/12/06/12/29/chair-558951__340.jpg",
			"https://cdn.pixabay.com/photo/2015/12/05/23/45/sofa-1078931__340.jpg",
			"https://cdn.pixabay.com/photo/2016/06/05/22/23/home-1438326__340.jpg",
			"https://cdn.pixabay.com/photo/2015/03/12/14/13/living-room-670237__340.jpg",
			"https://cdn.pixabay.com/photo/2017/12/27/14/42/furniture-3042835__340.jpg",
			"https://cdn.pixabay.com/photo/2017/03/28/12/07/chairs-2181923__340.jpg",
			"https://cdn.pixabay.com/photo/2017/10/04/14/50/staging-2816464__340.jpg",
			"https://cdn.pixabay.com/photo/2014/09/23/20/19/castle-458058__340.jpg",
			"https://cdn.pixabay.com/photo/2017/09/15/12/10/mockup-2752023__340.jpg",
			"https://cdn.pixabay.com/photo/2017/03/24/14/22/pin-up-girl-2171312_1280.jpg"
		);
		$index = rand(0, count($imgs));
		return $imgs[$index];
	}

	/**
	 * Returns array of random dates in current and next maonth
	 *
	 * @return array
	 */
	private static function getOccupancy() {
		$count = rand(5,20);
		$date = array(
			"year" => date("Y"),
			"month" => date("m")
		);
		$max_inc = (int)(31 / ($count / 2));
		$incs = array(
			"odd" => 0, 
			"even" => 0
		);
		$occupancy = [];
		
		for ($i = 0; $i < $count; ++$i) {
			if ($i % 2) {
				$occupancy[$date['year'] . "-" . ((int)$date['month'] + 1) . "-" . ((int)$incs["odd"] + rand(1, $max_inc))] = 1;
				$incs["odd"] += $max_inc;
			} else {
				$occupancy[$date['year'] . "-" . $date['month'] . "-" . ((int)$incs["even"] + rand(1, $max_inc))] = 1;
				$incs["even"] += $max_inc;
			}
		}

		return $occupancy;
	}

    /**
     * Returns substring of Lorem Ipsum
     *
     * @param int $length
     * @return bool|string
     */
    private static function getDesc(int $length = 50) {
        if ($length >= strlen(LOREM_IPSUM))
            return LOREM_IPSUM;

        return substr(LOREM_IPSUM, 0, $length) . "...";
    }
}