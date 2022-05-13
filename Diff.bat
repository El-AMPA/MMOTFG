@echo off

setlocal EnableDelayedExpansion

::VERIFY errors 2>nul
::SETLOCAL ENABLEEXTENSIONS
::IF ERRORLEVEL 1 echo Unable to enable extensions

::verify>NUL
::call :EXTTEST 2>NUL
::if not ErrorLevel 1 goto EXTERROR

::echo:Command extensions v2 or later are available.

dir /a| findstr Test[0-9]*$ > numCarpetas.txt

for /f "tokens=3" %%f in ('find /c /v "" numCarpetas.txt') do ( set count=%%f ) 

echo !count!

SET /A index = 1

echo !index!

SET /A numFilas = 1

:while

if %index% LEQ %count% (

    echo index: !index!

    for /f "tokens=3" %%f in ('find /c /v "" .\\Test%index%\\expectedOutput.txt') do ( set textSize1=%%f ) 

    for /f "tokens=3" %%f in ('find /c /v "" .\\Test%index%\\Output.txt') do ( set textSize2=%%f )

    echo Test!index!/expectedOutput.txt
    echo a: !textSize1!

    echo Test!index!/Output.txt
    echo b: !textSize2!

    if !textSize1! NEQ !textSize2! ( goto :failedSize )

    echo Archivos con el mismo tamanio

    fc .\\Test%index%\\expectedOutput.txt .\\Test%index%\\Output.txt > diffResult.txt

    for /f "tokens=3" %%f in ('find /c /v "FC: no se han encontrado diferencias" diffResult.txt') do ( set diffText=%%f ) 

    if !diffText! EQU !numFilas! ( goto :failedDiff )

    echo Test !index! completado con exito

    SET /A index = %index% + 1

    echo index: !index!

    echo "------------------"

    goto :while
)

PAUSE

:::EXTTEST
::if CmdExtVersion 2 exit /b 1
::goto :EOF


:::EXTERROR
::echo:This script requires command extensions v2 or later!>&2
::verify error 2>NUL
:: this should be the last line of the file, so that script can exit when command extensions are not available

exit 0

:failedDiff
    echo Los archivos no tienen el mismo contenido
    exit 1

:failedSize
    echo El tamanio de los archivos es distinto
    exit 1