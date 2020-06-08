@ECHO OFF
	IF %1.==. GOTO NoParms

:Run
	ECHO Refreshing:  %1

	ECHO Deleting Repo %1
	IF EXIST c:\work\%1\ (
		DEL /f/s/q c:\work\%1 > NUL
		RMDIR /s/q c:\work\%1
	)


	ECHO Cloning Repo %1
	IF NOT EXIST c:\work\%1\ (
		MKDIR c:\work\%1
		git clone --branch master --recursive <ChangeMe>/%1.git C:\work\%1
	) ELSE (
		ECHO Abort: Delete Failed
		GOTO TheEnd
	)

	ECHO Linking Packages
	MKLINK /J c:\work\%1\packages c:\work\packages
	GOTO TheEnd

:NoParms
	ECHO Aprt: Repo not defined
	ECHO Usage: RefreshRepo.bat [MyRepoName]
	GOTO TheEnd

:TheEnd