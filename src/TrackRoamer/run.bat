REM dsshost /manifest:"..\Config\TrackRoamer.TrackRoamerDriveInSquares.manifest.xml" /p:50000 /t:50001

REM dsshost /manifest:"..\samples\Config\DriveInSquare.P3DXSim.manifest.xml" /p:50000 /t:50001

REM dsshost /manifest:"..\Config\TrackRoamer.TrackRoamerLaserOnly.manifest.xml" /p:50000 /t:50001

REM dsshost32 /manifest:"..\Config\TrackRoamer.TrackRoamerSpeechRecognizerOnly.manifest.xml" /p:50000 /t:50001

REM dsshost32 /manifest:"..\samples\Config\MicArraySpeechRecognizerGui.user.manifest.xml" /p:50000 /t:50001

// warning: in MRDS 4 speech recognizer requires 32-bit DSS host

dsshost32.exe /manifest:"..\Config\TrackRoamer.TrackRoamerBot.manifest.xml" /p:50000 /t:50001

REM dsshost /manifest:"..\Config\TrackRoamer.TrackRoamerBrickProximityBoard.manifest.xml" /p:50000 /t:50001

REM dsshost /manifest:"..\Config\TrackRoamer.TrackRoamerUsrf.manifest.xml" /p:50000 /t:50001

