@echo Off

set config=%1
if "%config%" == "" (
   set config=Release
)

set version=0.1.0
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)

mkdir Build.AspNet
mkdir Build.Hosting
dotnet build PrometheusCore.sln -c %config%
dotnet pack PrometheusCore.AspNet\PrometheusCore.AspNet.csproj /p:PackageVersion=%version% -o .\..\Build.AspNet
dotnet pack PrometheusCore.Hosting\PrometheusCore.Hosting.csproj /p:PackageVersion=%version% -o .\..\Build.Hosting
