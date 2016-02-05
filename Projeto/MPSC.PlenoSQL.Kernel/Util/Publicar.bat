@Echo Off

if Not Exist "\\10.21.25.59\transf\MTZ - SISTEMAS\bNogueira\PlenoSQL\" MkDir "\\10.21.25.59\transf\MTZ - SISTEMAS\bNogueira\PlenoSQL\"

xCopy /Y "D:\Projeto\MP\MPSC.PlenoSQL\Projeto\Bin\*.dll" "\\10.21.25.59\transf\MTZ - SISTEMAS\bNogueira\PlenoSQL\*.*"
xCopy /Y "D:\Projeto\MP\MPSC.PlenoSQL\Projeto\Bin\PlenoSQL.exe" "\\10.21.25.59\transf\MTZ - SISTEMAS\bNogueira\PlenoSQL\*.*"
