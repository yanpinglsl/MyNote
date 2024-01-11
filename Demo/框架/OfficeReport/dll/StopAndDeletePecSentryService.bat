@ echo off
%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit cd /d "%~dp0"

@echo  add  temporary variable
set mypath=%~dp0
set path=%mypath%;%path%

@echo  stop service
call nssm stop PecSentryService confirm

@echo  remove service
call nssm remove PecSentryService confirm

exit