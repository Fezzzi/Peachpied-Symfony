<?php

namespace AppBundle\Controller;

use AppBundle\Entity\User;
use AppBundle\Entity\Message;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Route;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Method;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpKernel\Exception\BadRequestHttpException;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Security\Core\Exception\AuthenticationException;
use Symfony\Component\Security\Core\Security;
use FOS\UserBundle\Event\FilterUserResponseEvent;
use FOS\UserBundle\Event\FormEvent;
use FOS\UserBundle\Event\GetResponseUserEvent;
use FOS\UserBundle\Form\Factory\FactoryInterface;
use FOS\UserBundle\FOSUserEvents;
use FOS\UserBundle\Model\UserManagerInterface;
use Symfony\Component\EventDispatcher\EventDispatcherInterface;
use Symfony\Component\HttpFoundation\RedirectResponse;

class GeneralController extends HelperController
{
    /**
     * @Route("/", name="homepage")
     */
    public function indexAction()
    {
        return $this->redirectToRoute('home');
    }

    /**
     * @Route("/home", name="home", options={"expose"=true})
     */
    public function homeAction(Request $request)
    {
        $regErrors = $request->query->get('errors');
        if($regErrors == null){
            $regErrors = [];
        }

        $form = $this->getForm($request);
        foreach($form->children as $child) {
            if($child->vars['name'] == 'plainPassword'){
                $children = $child->vars['form']->children;
                $children['first']->vars['label_attr']['class'] = 'sr-only';
                $children['first']->vars['attr']['placeholder'] = 'password';
                $children['second']->vars['label_attr']['class'] = 'sr-only';
                $children['second']->vars['attr']['placeholder'] = 'repeat password';
            }
            else {
                $child->vars['label_attr']['class'] = 'sr-only';
                $child->vars['attr']['placeholder'] = $child->vars['name'];
            }
        }
        $form->vars['attr']['placeholder'] = $form->vars['name'];

        $data = $this->getData($request);

        return $this->render('homepage.html.twig', array(
                'error' => $data['error'],
                'regErrors' => $regErrors,
                'csrf_token' => $data['csrf_token'],
                'last_username' => $data['last_username'],
                'form' => $form)
        );
    }

    private function getForm(Request $request)
    {
        /** @var $formFactory FactoryInterface */
        $formFactory = $this->get('fos_user.registration.form.factory');
        /** @var $userManager UserManagerInterface */
        $userManager = $this->get('fos_user.user_manager');
        /** @var $dispatcher EventDispatcherInterface */
        $dispatcher = $this->get('event_dispatcher');

        $user = $userManager->createUser();
        $user->setEnabled(true);

        $event = new GetResponseUserEvent($user, $request);
        $dispatcher->dispatch(FOSUserEvents::REGISTRATION_INITIALIZE, $event);

        if (null !== $event->getResponse()) {
            return $event->getResponse();
        }

        $form = $formFactory->createForm();
        $form->setData($user);

        $form->handleRequest($request);

        if ($form->isSubmitted()) {
            if ($form->isValid()) {
                $event = new FormEvent($form, $request);
                $dispatcher->dispatch(FOSUserEvents::REGISTRATION_SUCCESS, $event);

                $userManager->updateUser($user);

                if (null === $response = $event->getResponse()) {
                    $url = $this->generateUrl('fos_user_registration_confirmed');
                    $response = new RedirectResponse($url);
                }

                $dispatcher->dispatch(FOSUserEvents::REGISTRATION_COMPLETED, new FilterUserResponseEvent($user, $request, $response));

                return $response;
            }

            $event = new FormEvent($form, $request);
            $dispatcher->dispatch(FOSUserEvents::REGISTRATION_FAILURE, $event);

            if (null !== $response = $event->getResponse()) {
                return $response;
            }
        }

        return $form->createView();
    }

    private function getData(Request $request)
    {
        /** @var $session \Symfony\Component\HttpFoundation\Session\Session */
        $session = $request->getSession();

        $authErrorKey = Security::AUTHENTICATION_ERROR;
        $lastUsernameKey = Security::LAST_USERNAME;

        // get the error if any (works with forward and redirect -- see below)
        if ($request->attributes->has($authErrorKey)) {
            $error = $request->attributes->get($authErrorKey);
        } elseif (null !== $session && $session->has($authErrorKey)) {
            $error = $session->get($authErrorKey);
            $session->remove($authErrorKey);
        } else {
            $error = null;
        }

        if (!$error instanceof AuthenticationException) {
            $error = null; // The value does not come from the security component.
        }

        // last username entered by the user
        $lastUsername = (null === $session) ? '' : $session->get($lastUsernameKey);

        $csrfToken = $this->has('security.csrf.token_manager')
            ? $this->get('security.csrf.token_manager')->getToken('authenticate')->getValue()
            : null;

        $data = array(
            'last_username' => $lastUsername,
            'error' => $error,
            'csrf_token' => $csrfToken,
        );

        return $data;
    }

    /**
     * @Route("/message", name="message")
     */
    public function messageAction()
    {
        $this->denyAccessUnlessGranted('IS_AUTHENTICATED_REMEMBERED');
        return $this->render('message.html.twig',array());
    }

    /**
     * @Route("/message/log", name="log_message", options={"expose" = true})
     * @Method("POST")
     */
    public function logMessage(Request $request)
    {
        $data = json_decode($request->getContent());

        /** @var Message $message */
        $message = new Message();
        $message->setUserId($this->getUser()->getId());
        $message->setMessage($data);
        $em = $this->getDoctrine()->getManager();
        $em->persist($message);
        $em->flush();

        return new Response(null, 204);
    }
}
