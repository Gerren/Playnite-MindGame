d:\Playnite\Toolbox.exe verify Installer "e:\Zaloha\Projects\Playnite\MindGame\MindGame\installer.yaml"
d:\Playnite\Toolbox.exe verify Addon "e:\Zaloha\Projects\Playnite\MindGame\MindGame\Gerren_MindGame.yaml"
pause
copy /Y "e:\Zaloha\Projects\Playnite\MindGame\MindGame\extension.yaml" "E:\Zaloha\Projects\Playnite\MindGame\MindGame\bin\Debug\extension.yaml"
d:\Playnite\Toolbox.exe pack "E:\Zaloha\Projects\Playnite\MindGame\MindGame\bin\Debug" "E:\Zaloha\Projects\Playnite"

explorer "E:\Zaloha\Projects\Playnite"
explorer "https://github.com/Gerren/Playnite-MindGame/upload/master"
