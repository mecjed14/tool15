if (dotnet tool list --global | Select-String changelogtool -quiet){
    Write-Host
    Write-Host "clt is already installed as global tool" -ForegroundColor Red
    Write-Host "To uninstall run following command: " -ForegroundColor Red
    Write-Host "dotnet tool uninstall --global changelogtool" -ForegroundColor DarkGray
    Write-Host
    exit -1
}

dotnet pack .\source\Buhler.IoT.Environment.ChangeLogTool.sln

$toolName = "Buhler.IoT.Environment.ChangeLogTool"

if (dotnet tool list -g | Select-String $toolName -quiet){
    dotnet tool uninstall $toolName -g
}

dotnet tool install -g $toolName --add-source .\source\bin\Debug\
