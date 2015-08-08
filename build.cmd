@echo off
pushd %~dp0

if exist bin (
	rd bin /Q /S
)
for /D %%d in (*) DO (
	pushd "%%d"
	if exist bin (
		rd bin /Q /S
	)
	popd
)

set sln=icfpc2015.sln

nuget restore %sln%

rem set dotNetBasePath=%windir%\Microsoft.NET\Framework
rem if exist %dotNetBasePath%64 set dotNetBasePath=%dotNetBasePath%64
rem for /R %dotNetBasePath% %%i in (*msbuild.exe) do set msbuild=%%i

set msbuild="%PROGRAMFILES(X86)%\MSBuild\14.0\Bin\MSBuild.exe"
%msbuild% /nr:false /v:minimal /fl /p:TargetFrameworkVersion=v4.6 /t:Rebuild /p:Configuration=Release %sln%

popd