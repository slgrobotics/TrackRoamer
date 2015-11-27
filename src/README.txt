
=== Prerequisites:

"C:\Microsoft Robotics Dev Studio 2008 R3"   - installation folder for MRDS (use custom install)

Create folder "C:\Microsoft Robotics Dev Studio 2008 R3\Config" - files are copied there on build

Create folder C:\temp  - some logs go there.

You may need Visual Studio C# Express 2010 to compile Proximity Module / USRF test suite code (WPF based) - if you are using PIC4550 board.

Get real familiar with MRDS Manifest Editor - it can read manifests as well as create and run them. No manual editing!

DebugView is your other best friend.


---------------------------------------------------------------------------------------------

=== Installation:

Unpack the zip file.

The src folder should end up as C:\Projects\Robotics\src

Create a key for signing assemblies:

C:\Projects\Robotics\src\TrackRoamer\make-keys.bat


=== Compiling test suite:

Start with 

C:\Projects\Robotics\src\Hardware\RoboteQ\RoboteqControllerTest\RoboteqControllerTest.sln  - opens with Visual Studio 2008

Rebuild; run - should open a window; second tab - AX2850. Select available port.

Keep in mind that good initial settings for AX2850 firmware are here - C:\Projects\Robotics\src\TrackRoamer\RoboteQ-Profile.rpf  (use RoboRun to import)



=== Compiling MRDS code:

Open C:\Projects\Robotics\src\TrackRoamer\TrackRoamerBot.sln

Rebuild.


All dlls go to "C:\Microsoft Robotics Dev Studio 2008 R3\bin\" folder.

Copy run.bat there. Invoke DSS Command prompt, use "cd bin" and "run" to start the robot.


---------------------------------------------------------------------------------------------

=== Bits and pieces:






