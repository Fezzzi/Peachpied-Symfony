Remove-Item -r ~/.nuget/packages/symfony.*
Remove-Item -r ~/.nuget/packages/composer
Remove-Item -r ~/.nuget/packages/psr.*
Remove-Item -r ~/.nuget/packages/twig.twig

cd empty-page/Symfony.Skeleton

$libraries = 
	"Symfony.Process.msbuildproj",
	"Symfony.Var-exporter.msbuildproj",
	"Symfony.Http-foundation.msbuildproj",
	"Symfony.Polyfill-mbstring.msbuildproj",
	"Symfony.Polyfill-intl-icu.msbuildproj",
	"Symfony.Dotenv.msbuildproj",
	"Composer.msbuildproj",
	"Psr.Cache.msbuildproj",
	"Symfony.Config.msbuildproj",
	"Psr.Container.msbuildproj",
	"Psr.Log.msbuildproj",
	"Psr.Simple-cache.msbuildproj",
	"Symfony.Templating.msbuildproj",
	"Symfony.Inflector.msbuildproj",
	"Symfony.Finder.msbuildproj",
	"Symfony.Contracts.msbuildproj",
	"Symfony.Property-access.msbuildproj",
	"Twig.Twig.msbuildproj",
	"Symfony.Filesystem.msbuildproj",
	"Symfony.Dependency-injection.msbuildproj",
	"Symfony.Routing.msbuildproj",
	"Symfony.Event-dispatcher.msbuildproj",
	"Symfony.Debug.msbuildproj",
	"Symfony.Http-kernel.msbuildproj",
	"Symfony.Console.msbuildproj",
	"Symfony.Options-resolver.msbuildproj",
	"Symfony.Yaml.msbuildproj",
	"Symfony.Cache.msbuildproj",
	"Symfony.Translation.msbuildproj",
	"Symfony.Validator.msbuildproj",
	"Symfony.Form.msbuildproj",
	"Symfony.Framework-bundle.msbuildproj",
	"Symfony.Twig-bridge.msbuildproj",
	"Symfony.Twig-bundle.msbuildproj",
	"Symfony.Skeleton.msbuildproj";

foreach ($library in $libraries){
	dotnet build $library;
}
