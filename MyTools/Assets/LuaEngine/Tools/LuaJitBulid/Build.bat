@echo off 
setlocal enabledelayedexpansion
set workPath=%1
set savePath=%2
cd %~dp0
for /r %workPath% %%s in (*.lua) do (
    for /f %%a in ("%%s") do (
        echo building %%s
        luajit.exe -b %%s %savePath%\%%~nxa
    )
)
echo 编译完成！保存路径：%savePath%
pause