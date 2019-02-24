<?php

namespace AppBundle\Form\Type;

use AppBundle\Entity\SportLog;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\Extension\Core\Type\NumberType;
use Symfony\Component\Form\Extension\Core\Type\DateTimeType;
use Symfony\Component\Form\Extension\Core\Type\FileType;
use Symfony\Component\Form\Extension\Core\Type\TextType;
use Symfony\Component\Form\Extension\Core\Type\IntegerType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;

class SportLogType extends AbstractType
{
    public function buildForm(FormBuilderInterface $builder, array $options)
    {
        $builder
            ->add('challengeId', TextType::class)
            ->add('challenge', TextType::class)
            ->add('sportId', TextType::class)
            ->add('sport', TextType::class)
            ->add('user_id', IntegerType::class)
            ->add('image', FileType::class)
            ->add('points', NumberType::class)
            ->add('date', DateTimeType::class)
        ;
    }

    public function configureOptions(OptionsResolver $resolver)
    {
        $resolver->setDefaults(array(
            'data_class' => SportLog::class,
        ));

        $resolver
            ->setRequired('entity_manager');
    }
} 