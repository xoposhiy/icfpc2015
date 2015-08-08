@echo off
pushd %~dp0

for /D %%d in (*) DO (
	pushd "%%d"
	if exist bin (
		rd bin /Q /S
	)
	popd
)

pushd play_icfp2015

rem set dotNetBasePath=%windir%\Microsoft.NET\Framework
rem if exist %dotNetBasePath%64 set dotNetBasePath=%dotNetBasePath%64
rem for /R %dotNetBasePath% %%i in (*msbuild.exe) do set msbuild=%%i

set msbuild="%PROGRAMFILES(X86)%\MSBuild\14.0\Bin\MSBuild.exe"
%msbuild% /nr:false /v:minimal /fl /p:TargetFrameworkVersion=v4.6 /t:Rebuild /p:Configuration=Release play_icfp2015.csproj

popd