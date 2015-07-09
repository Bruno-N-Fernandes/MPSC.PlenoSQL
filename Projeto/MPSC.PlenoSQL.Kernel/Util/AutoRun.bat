@Echo Off
cls
Set dir=C:\Script
Set cfg=PlenoSqlConfig.txt
Set file="%dir%\%cfg%"

If Exist %file% Del /F /Q %file%
Echo -Dir="%dir%">>%file%
Echo -Rdb="IBM DB2">>%file%
Echo -Srv=MtzSrva2>>%file%
Echo -Usr=UsrBen>>%file%
Echo -Pwd=@poiuy>>%file%
Echo -Bco=eSim>>%file%
Echo -Cmd=AllSqlInDir>>%file%
Echo -Brk=;>>%file%

..\..\Bin\PlenoSQL.exe -Dir="%dir%" -Cfg="%cfg%"

TimeOut 10

If Exist %file% Del /F /Q %file%
