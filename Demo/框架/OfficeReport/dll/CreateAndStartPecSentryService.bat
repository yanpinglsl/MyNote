@ echo off

%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit cd /d "%~dp0"


@echo add  temporary variable
set mypath=%~dp0
set path=%mypath%;%path%

@echo install service
call nssm install PecSentryService %~dp0PecSentry.exe

@echo start service
call nssm start PecSentryService confirm

exit