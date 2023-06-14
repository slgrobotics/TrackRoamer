REM
REM removing all intermediate and distribution files:
REM
REM

rmdir /q /s LibSystem\obj
rmdir /q /s LibSystem\bin

rmdir /q /s LibRoboteqController\obj
rmdir /q /s LibRoboteqController\bin

rmdir /q /s LibBehavior\obj
rmdir /q /s LibBehavior\bin

rmdir /q /s LibMapping\obj
rmdir /q /s LibMapping\bin

rmdir /q /s LibGuiWpf\obj
rmdir /q /s LibGuiWpf\bin

rmdir /q /s LibLvrGenericHid\obj
rmdir /q /s LibLvrGenericHid\bin

rmdir /q /s LibPicSensors\obj
rmdir /q /s LibPicSensors\bin

rmdir /q /s TrackRoamerBot\obj
rmdir /q /s TrackRoamerBot\bin
rmdir /q /s TrackRoamerBot\Proxy

rmdir /q /s TrackRoamerServices\obj
rmdir /q /s TrackRoamerServices\bin
rmdir /q /s TrackRoamerServices\Proxy

rmdir /q /s TrackRoamerDrive\obj
rmdir /q /s TrackRoamerDrive\bin
rmdir /q /s TrackRoamerDrive\Proxy

rmdir /q /s TrackRoamerBehaviors\obj
rmdir /q /s TrackRoamerBehaviors\bin
rmdir /q /s TrackRoamerBehaviors\Proxy

rmdir /q /s TrackRoamerBrickPower\obj
rmdir /q /s TrackRoamerBrickPower\bin
rmdir /q /s TrackRoamerBrickPower\Proxy

rmdir /q /s TrackRoamerBrickProximityBoard\obj
rmdir /q /s TrackRoamerBrickProximityBoard\bin
rmdir /q /s TrackRoamerBrickProximityBoard\Proxy

rmdir /q /s TrackRoamerDashboard\obj
rmdir /q /s TrackRoamerDashboard\bin
rmdir /q /s TrackRoamerDashboard\Proxy

rmdir /q /s BASimpleDashboard\obj
rmdir /q /s BASimpleDashboard\bin
rmdir /q /s BASimpleDashboard\Proxy

rmdir /q /s TrackRoamerExplorer\obj
rmdir /q /s TrackRoamerExplorer\bin
rmdir /q /s TrackRoamerExplorer\Proxy

rmdir /q /s TrackRoamerFollower\obj
rmdir /q /s TrackRoamerFollower\bin
rmdir /q /s TrackRoamerFollower\Proxy

rmdir /q /s TrackRoamerUsrf\obj
rmdir /q /s TrackRoamerUsrf\bin
rmdir /q /s TrackRoamerUsrf\Proxy

rmdir /q /s TrackroamerRP2011AbstractionLayer\obj
rmdir /q /s TrackroamerRP2011AbstractionLayer\bin
rmdir /q /s TrackroamerRP2011AbstractionLayer\Proxy

del /f "..\..\bin\TrackRoamer.Robotics.Hardware.*"
del /f "..\..\bin\TrackRoamer.Robotics.Services.*"
del /f "..\..\bin\TrackRoamer.Robotics.Utility.*"
del /f "..\..\bin\TrackRoamer.Robotics.LibMapping.*"
del /f "..\..\bin\TrackRoamer.Robotics.LibBehavior.*"
del /f "..\..\bin\TrackRoamer.Robotics.LibGuiWpf.*"

REM del /f "..\..\Config\TrackRoamer*.*"

REM keep in mind: all log* and trace.txt files are in ..\..\bin\ folder

pause

