for /D %%d in (*) DO (
	pushd "%%d"
	if exist bin (
		rd bin /Q /S
	)
	if exist obj (
		rd obj /Q /S
	)
	popd
)

set targetZip=hack-the-loop.zip

erase %targetZip%

7za.exe a -ir@filesToSubmit.txt %targetZip%
