<?php

namespace AppBundle\Repository;

use Doctrine\ORM\EntityRepository;

/**
 * sportLogRepository
 *
 * This class was generated by the Doctrine ORM. Add your own custom
 * repository methods below.
 */
class SportLogRepository extends EntityRepository
{
    public function getLeaderboardDetails()
    {
        return $this->createQueryBuilder('rl')
            ->select('rl.user_id as user_id, SUM(rl.points) as points, COUNT(rl.challenge) as challenges')
            ->groupBy('rl.user_id')
            ->orderBy('points', 'DESC')
            ->getQuery()
            ->execute();
    }
}
