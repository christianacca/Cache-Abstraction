@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

cd src

REM Package restore
call %nuget% restore CcAcca.CacheAbstraction.sln -OutputDirectory %cd%\..\packages -NonInteractive

REM Build
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild CcAcca.CacheAbstraction.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

REM Test
%nuget% install NUnit.Runners -Version 2.6.3 -OutputDirectory packages
packages\NUnit.Runners.2.6.3\tools\nunit-console.exe /config:%config% /framework:net-4.5 ..\bin\%config%\CcAcca.CacheAbstraction.Test\CcAcca.CacheAbstraction.Test.dll

REM Package
mkdir ..\dist
%nuget% pack CcAcca.CacheAbstraction\CcAcca.CacheAbstraction.csproj -OutputDirectory ..\dist -Symbols -Prop Configuration=%config%
%nuget% pack CcAcca.CacheAbstraction.WebApi\CcAcca.CacheAbstraction.WebApi.csproj -OutputDirectory ..\dist -Symbols -Prop Configuration=%config%
%nuget% pack CcAcca.CacheAbstraction.Distributed\CcAcca.CacheAbstraction.Distributed.csproj -OutputDirectory ..\dist -Symbols -Prop Configuration=%config%