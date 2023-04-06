@ECHO OFF
cd /D "%~dp0"
set "_path=%~dp0%"

cd /D "%_path%.."
set "_path=%CD%" 

set "_sourcePath=%_path%\Projects\Znode.Engine.WebStore\Views\Themes"
set "_destinationPath=%_path%\Projects\Znode.Engine.Admin\Themes"

::echo %_sourcePath%
::echo %_destinationPath%

xcopy /e/v/z/y/i "%_sourcePath%" "%_destinationPath%"
::Pause
exit