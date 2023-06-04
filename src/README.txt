#
# this is C:\Projects\Robotics\src\README.txt 
#         https://github.com/slgrobotics/TrackRoamer/blob/master/src/README.txt
#

see https://github.com/slgrobotics/TrackRoamer (code good as of 2023-06-04)

=== Prerequisites:

"C:\Microsoft Robotics Dev Studio 4"   - installation folder for MRDS (use custom install)

Create folder "C:\Microsoft Robotics Dev Studio 4\Config" - files are copied there on build

Create folder C:\temp  - some logs go there.

You may need Visual Studio C# 2019 to compile Proximity Module / USRF test suite code (WPF based) - if you are using PIC4550 board.

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

C:\Projects\Robotics\src\Hardware\RoboteQ\RoboteqControllerTest\RoboteqControllerTest.sln  - opens with Visual Studio 2019

Rebuild; run - should open a window; second tab - AX2850. Select available port.

Keep in mind that good initial settings for AX2850 firmware are here - C:\Projects\Robotics\src\TrackRoamer\RoboteQ-Profile.rpf  (use RoboRun to import)



=== Compiling MRDS code:

Open C:\Projects\Robotics\src\TrackRoamer\TrackRoamerBot.sln

Rebuild.


All dlls go to "C:\Microsoft Robotics Dev Studio 2008 R3\bin\" folder.

Copy run.bat there. Invoke DSS Command prompt, use "cd bin" and "run" to start the robot.


---------------------------------------------------------------------------------------------

=== Bits and pieces:

http://localhost:50000/directory    - Service directory, open in MS Edge (saved in favorites)

parameters:
Very important: PowerScale - there's a slider in "PID Controllers" tab of Trackroamer GUI window.

C:\Projects\Robotics\src\TrackRoamer\TrackRoamerBehaviors\DriveBehaviorServiceBase.cs : 90

Example COM ports assignement (June 2023):

"Arduino Uno"                       COM11  - Monkey Head
"Arduino USB Serial Light Adapter"  COM 7 - 
"Profilic USB-To-Serial Com Port"   COM 10 - UM6 IMU/AHRS
"USB Serial Port"                   COM 12 - "Brick", Wheels, RoboteQ AX2850 motor controller


---------------------------------------------------------------------------------------------








