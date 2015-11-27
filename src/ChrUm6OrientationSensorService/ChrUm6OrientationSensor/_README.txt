
ChrUm6OrientationSensor

place it in    C:\Microsoft Robotics Dev Studio 4\projects\TrackRoamer\ChrUm6OrientationSensor  (this is the folder where this _README.txt should reside)

This is a Microsoft Robotics Studio Service.
It runs under DSS Host and connects to a CH Robotics UM6 Orientation Sensor (a.k.a. AHRS)
The UM6 device must be preconfigured to broadcast at least Quaternions (registers 100, 101).
You can configure UM6 using "CHR Serial Interface" software.

Downloads - http://www.chrobotics.com/index.php?main_page=page&id=3

		• CHR Serial Interface v2.1.0 - Windows Installer
			Use UM6 Serial Interface to configure the UM6, plot and log sensor data, and calibrate the magnetometer.
			
		• CHR Serial Interface Source Code - http://sourceforge.net/projects/chrinterface/
			The Serial Interface source code is available from Sourceforge and can be checked out using SVN.
			Or download tarball: see Code->SVN Browse->Download GNU tarball
			
The ChrUm6OrientationSensor can be started with the following command on DSS prompt:

	C:\Microsoft Robotics Dev Studio 4\bin\dsshost.exe /p:50000 /t:50001 /m:"C:\Microsoft Robotics Dev Studio 4\projects\TrackRoamer\ChrUm6OrientationSensor\ChrUm6OrientationSensor.manifest.xml"


