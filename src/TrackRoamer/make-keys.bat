REM On a command prompt create a key for signing assemblies:

mkdir ..\Keys
"..\..\tools\sn.exe" -k "..\Keys\TrackRoamer.snk"

REM sn.exe is also part of .NET SDK 
REM (see http://msdn.microsoft.com/en-us/library/6f05ezxy.aspx for explanation)

REM Copy that file to the following locations:

copy ..\Keys\TrackRoamer.snk LibBehavior\.
copy ..\Keys\TrackRoamer.snk LibGuiWpf\.
copy ..\Keys\TrackRoamer.snk LibLvrGenericHid\.
copy ..\Keys\TrackRoamer.snk LibMapping\.
copy ..\Keys\TrackRoamer.snk LibPicSensors\.
copy ..\Keys\TrackRoamer.snk LibRoboteqController\.
copy ..\Keys\TrackRoamer.snk LibSystem\.
copy ..\Keys\TrackRoamer.snk TrackRoamerBehavior\.
copy ..\Keys\TrackRoamer.snk TrackRoamerBehaviors\.
copy ..\Keys\TrackRoamer.snk TrackRoamerBot\.
copy ..\Keys\TrackRoamer.snk TrackRoamerBrickProximityBoard\.
copy ..\Keys\TrackRoamer.snk TrackRoamerDashboard\.
copy ..\Keys\TrackRoamer.snk TrackRoamerDrive\.
copy ..\Keys\TrackRoamer.snk TrackRoamerExplorer\.
copy ..\Keys\TrackRoamer.snk TrackRoamerFollower\.
copy ..\Keys\TrackRoamer.snk TrackRoamerServices\.
copy ..\Keys\TrackRoamer.snk TrackRoamerUsrf\.
copy ..\Keys\TrackRoamer.snk "TrackRoamerUsrf - Serial\."

pause
