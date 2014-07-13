mkdir ..\dist\CcAcca.CacheAbstraction\lib\net40
xcopy ..\bin\Release\CcAcca.CacheAbstraction\CcAcca.CacheAbstraction.dll ..\dist\CcAcca.CacheAbstraction\lib\net40 /y
xcopy ..\bin\Release\CcAcca.CacheAbstraction\CcAcca.CacheAbstraction.pdb ..\dist\CcAcca.CacheAbstraction\lib\net40 /y
xcopy ..\bin\Release\CcAcca.CacheAbstraction\CcAcca.CacheAbstraction.XML ..\dist\CcAcca.CacheAbstraction\lib\net40 /y
xcopy  CcAcca.CacheAbstraction\* ..\dist\CcAcca.CacheAbstraction /s /i
NuGet.exe pack ..\dist\CcAcca.CacheAbstraction\CcAcca.CacheAbstraction.nuspec -OutputDirectory ..\dist
rmdir /s /q ..\dist\CcAcca.CacheAbstraction