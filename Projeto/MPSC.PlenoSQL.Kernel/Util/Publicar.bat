@Echo Off

if Not Exist "\\svdatfs01\Sistemas\bNogueira\PlenoSQL\" MkDir "\\svdatfs01\Sistemas\bNogueira\PlenoSQL\"

xCopy /Y "D:\Prj\MP\MPSC.PlenoSQL\Projeto\Bin\*.dll" "\\svdatfs01\Sistemas\bNogueira\PlenoSQL\*.*"
xCopy /Y "D:\Prj\MP\MPSC.PlenoSQL\Projeto\Bin\PlenoSQL.exe" "\\svdatfs01\Sistemas\bNogueira\PlenoSQL\*.*"
