<?php

namespace AppBundle\Form\Type;

use Symfony\Component\Form\Extension\Core\Type\CheckboxType;
use Symfony\Component\Form\Extension\Core\Type\ChoiceType;
use Symfony\Component\Form\Extension\Core\Type\HiddenType;
use Symfony\Component\Form\Extension\Core\Type\TextType;
use Symfony\Component\Form\FormBuilderInterface;
use FOS\UserBundle\Form\Type\RegistrationFormType;

class RegistrationType extends RegistrationFormType
{
    public function buildForm(FormBuilderInterface $builder, array $options)
    {
        parent::buildForm($builder, $options);

        $builder
            ->add('teamMember', TextType::class, array(
                'attr' => array('maxlength' => '10')
            ))
            ->add('tMemberGender', ChoiceType::class, array(
                'choices' => array('fa fa-venus', 'fa fa-mars', 'fa fa-genderless'),
                'attr' => ['style' => 'display: none'],
                'multiple' => false,
                'expanded' => true,
            ));
    }
} 