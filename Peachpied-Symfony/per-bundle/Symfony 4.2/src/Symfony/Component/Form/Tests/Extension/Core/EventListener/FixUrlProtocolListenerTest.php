<?php

/*
 * This file is part of the Symfony package.
 *
 * (c) Fabien Potencier <fabien@symfony.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Symfony\Component\Form\Tests\Extension\Core\EventListener;

use PHPUnit\Framework\TestCase;
use Symfony\Component\Form\Extension\Core\EventListener\FixUrlProtocolListener;
use Symfony\Component\Form\FormEvent;

class FixUrlProtocolListenerTest extends TestCase
{
    public function testFixHttpUrl()
    {
        $data = 'www.symfony.com';
        $form = $this->getMockBuilder('Symfony\Component\Form\Test\FormInterface')->getMock();
        $event = new FormEvent($form, $data);

        $filter = new FixUrlProtocolListener('http');
        $filter->onSubmit($event);

        $this->assertEquals('http://www.symfony.com', $event->getData());
    }

    public function testSkipKnownUrl()
    {
        $data = 'http://www.symfony.com';
        $form = $this->getMockBuilder('Symfony\Component\Form\Test\FormInterface')->getMock();
        $event = new FormEvent($form, $data);

        $filter = new FixUrlProtocolListener('http');
        $filter->onSubmit($event);

        $this->assertEquals('http://www.symfony.com', $event->getData());
    }

    public function provideUrlsWithSupportedProtocols()
    {
        return [
            ['ftp://www.symfony.com'],
            ['chrome-extension://foo'],
            ['h323://foo'],
            ['iris.beep://foo'],
            ['foo+bar://foo'],
        ];
    }

    /**
     * @dataProvider provideUrlsWithSupportedProtocols
     */
    public function testSkipOtherProtocol($url)
    {
        $form = $this->getMockBuilder('Symfony\Component\Form\Test\FormInterface')->getMock();
        $event = new FormEvent($form, $url);

        $filter = new FixUrlProtocolListener('http');
        $filter->onSubmit($event);

        $this->assertEquals($url, $event->getData());
    }
}
