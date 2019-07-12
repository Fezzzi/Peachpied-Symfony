<?php

use Twig\Environment;
use Twig\Error\LoaderError;
use Twig\Error\RuntimeError;
use Twig\Extension\SandboxExtension;
use Twig\Markup;
use Twig\Sandbox\SecurityError;
use Twig\Sandbox\SecurityNotAllowedTagError;
use Twig\Sandbox\SecurityNotAllowedFilterError;
use Twig\Sandbox\SecurityNotAllowedFunctionError;
use Twig\Source;
use Twig\Template;

/* @Twig/Exception/error.html.twig */
class __TwigTemplate_bf8b10bdab2e08991b32e3305d370c70f1fd433b73f38ca8327e26af43e70ddf extends \Twig\Template
{
    private $source;
    private $macros = [];

    public function __construct(Environment $env)
    {
        parent::__construct($env);

        $this->source = $this->getSourceContext();

        $this->parent = false;

        $this->blocks = [
        ];
    }

    protected function doDisplay(array $context, array $blocks = [])
    {
        $macros = $this->macros;
        $__internal_319393461309892924ff6e74d6d6e64287df64b63545b994e100d4ab223aed02 = $this->extensions["Symfony\\Bridge\\Twig\\Extension\\ProfilerExtension"];
        $__internal_319393461309892924ff6e74d6d6e64287df64b63545b994e100d4ab223aed02->enter($__internal_319393461309892924ff6e74d6d6e64287df64b63545b994e100d4ab223aed02_prof = new \Twig\Profiler\Profile($this->getTemplateName(), "template", "@Twig/Exception/error.html.twig"));

        // line 1
        echo "<!DOCTYPE html>
<html>
    <head>
        <meta charset=\"";
        // line 4
        echo twig_escape_filter($this->env, $this->env->getCharset(), "html", null, true);
        echo "\" />
        <title>An Error Occurred: ";
        // line 5
        echo twig_escape_filter($this->env, (isset($context["status_text"]) || array_key_exists("status_text", $context) ? $context["status_text"] : (function () { throw new RuntimeError('Variable "status_text" does not exist.', 5, $this->source); })()), "html", null, true);
        echo "</title>
        <style>
            body { background-color: #fff; color: #222; font: 16px/1.5 -apple-system, BlinkMacSystemFont, \"Segoe UI\", Roboto, \"Helvetica Neue\", Arial, sans-serif; margin: 0; }
            .container { margin: 30px; max-width: 600px; }
            h1 { color: #dc3545; font-size: 24px; }
            h2 { font-size: 18px; }
        </style>
    </head>
    <body>
        <div class=\"container\">
            <h1>Oops! An Error Occurred</h1>
            <h2>The server returned a \"";
        // line 16
        echo twig_escape_filter($this->env, (isset($context["status_code"]) || array_key_exists("status_code", $context) ? $context["status_code"] : (function () { throw new RuntimeError('Variable "status_code" does not exist.', 16, $this->source); })()), "html", null, true);
        echo " ";
        echo twig_escape_filter($this->env, (isset($context["status_text"]) || array_key_exists("status_text", $context) ? $context["status_text"] : (function () { throw new RuntimeError('Variable "status_text" does not exist.', 16, $this->source); })()), "html", null, true);
        echo "\".</h2>

            <p>
                Something is broken. Please let us know what you were doing when this error occurred.
                We will fix it as soon as possible. Sorry for any inconvenience caused.
            </p>
        </div>
    </body>
</html>
";
        
        $__internal_319393461309892924ff6e74d6d6e64287df64b63545b994e100d4ab223aed02->leave($__internal_319393461309892924ff6e74d6d6e64287df64b63545b994e100d4ab223aed02_prof);

    }

    public function getTemplateName()
    {
        return "@Twig/Exception/error.html.twig";
    }

    public function isTraitable()
    {
        return false;
    }

    public function getDebugInfo()
    {
        return array (  63 => 16,  49 => 5,  45 => 4,  40 => 1,);
    }

    public function getSourceContext()
    {
        return new Source("<!DOCTYPE html>
<html>
    <head>
        <meta charset=\"{{ _charset }}\" />
        <title>An Error Occurred: {{ status_text }}</title>
        <style>
            body { background-color: #fff; color: #222; font: 16px/1.5 -apple-system, BlinkMacSystemFont, \"Segoe UI\", Roboto, \"Helvetica Neue\", Arial, sans-serif; margin: 0; }
            .container { margin: 30px; max-width: 600px; }
            h1 { color: #dc3545; font-size: 24px; }
            h2 { font-size: 18px; }
        </style>
    </head>
    <body>
        <div class=\"container\">
            <h1>Oops! An Error Occurred</h1>
            <h2>The server returned a \"{{ status_code }} {{ status_text }}\".</h2>

            <p>
                Something is broken. Please let us know what you were doing when this error occurred.
                We will fix it as soon as possible. Sorry for any inconvenience caused.
            </p>
        </div>
    </body>
</html>
", "@Twig/Exception/error.html.twig", "C:\\Users\\Fezzi\\Documents\\MATFYZ\\BAKALARKA\\Peachpied_Symfony\\Peachpied-Symfony-Twig-Razor\\Razor-Twig-Interoperabiliy\\vendor\\symfony\\twig-bundle\\Resources\\views\\Exception\\error.html.twig");
    }
}
