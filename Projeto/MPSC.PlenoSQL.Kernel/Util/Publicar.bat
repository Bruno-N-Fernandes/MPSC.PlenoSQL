@Echo Off

Set Destino=\\svdatfs01\Sistemas\bNogueira\PlenoSQL

if Not Exist "%Destino%\" MkDir "%Destino%\"

xCopy /Y "..\..\Bin\*.dll" "%Destino%\*.*"
xCopy /Y "..\..\Bin\PlenoSQL.exe" "%Destino%\*.*"
