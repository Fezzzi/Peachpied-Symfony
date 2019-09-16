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
                "img" => "http://www.minecraftinformation.com/images/how-to-make-a-bed.png"
            ], [
                "name" => "1 bed room", 
                "price" => "24€", 
                "desc" => DBEmulator::getDesc(90),
                "services" => ["wifi", "minibar"], 
				"occupancy" => DBEmulator::getOccupancy(),
				"img" => "https://i.ytimg.com/vi/eDYK2vhoBPQ/maxresdefault.jpg"
            ], [
                "name" => "2 beds room", "price" => "39€",
                "desc" => DBEmulator::getDesc(170),
                "services" => ["wifi", "minibar", "parking"],
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => "https://minecraftfurniture.net/wp-content/uploads/2018/07/minecraft-bed-with-backboard.jpg"
            ], [
                "name" => "2 beds room", "price" => "45€",
                "desc" => DBEmulator::getDesc(55),
                "services" => ["wifi", "minibar", "parking"],
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => "https://i.pinimg.com/originals/9e/eb/66/9eeb664f3176882143a759a2112604e8.png"
            ], [
                "name" => "3 beds room", "price" => "52€",
                "desc" => DBEmulator::getDesc(83),
                "services" => ["wifi", "minibar", "parking", "pool"],
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => "https://i.pinimg.com/originals/ac/e8/de/ace8dec14b7ed194559c7afe421a8916.jpg"
            ], [
                "name" => "4 beds room",
                "price" => "59€",
                "desc" => DBEmulator::getDesc(136),
                "services" => ["wifi", "minibar", "parking", "pool"],
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => "https://feedback.minecraft.net/hc/user_images/5sd4HyqqBOaOukmgA_xyrw.png"
            ], [
                "name" => "5 beds apartment",
                "price" => "79€",
                "desc" => DBEmulator::getDesc(205),
                "services" => ["wifi", "minibar", "parking", "pool"],
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => ""
            ], [
                "name" => "6 beds apartment",
                "price" => "99€",
                "desc" => DBEmulator::getDesc(158),
                "services" => ["wifi", "minibar", "parking", "pool", "hottub"],
				"occupancy" => DBEmulator::getOccupancy(),
                "img" => "https://gamerheadquarters.com/minecraft/images/villagerbunkbeds.jpg"
            ]
        ); 
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