
Improved MRDS Simple Dashboard (version 2.0)

-----------------------------------------------------------------------

Introduction
================

     Microsoft Robotics Developers Studio ships with a great utility known 
as the Simple Dashboard.  This service allows you to control a robot with a 
joystick or mouse and displays the output of a Laser Range Finder in a nice 
“cylinder” view.  It can be used with physical hardware or the simulator.  
Additionally, it can log messages and control a jointed arm.  This service 
makes some improvements to the standard dashboard.  These improvements 
include: a top view of the LRF, throttle control of the joystick, and 
keyboard control. 


Installing
================


     Extract the zip in the Robotics Developers Studio root.  It will place the
files at: RoboticsDevelopersStudioRoot/packages/BA/SimpleDashboard.  Then run
RoboticsDevelopersStudioRoot/packages/MigrateUser.cmd to customize the project 
files to your user and RDS version.  Then compile.

     This dashboard will replace the original one.  If you want to go back to the
original dashboard, simply compile the original dashboard located at:
RoboticsDevelopersStudioRoot/samples\UX\SimpleDashboard


Usage
================

     Once all the services have started, press the connect button in the upper 
right hand corner of the dashboard.  This will query all the services running 
for any drive platforms, laser range finders, and jointed arms.  The services 
found will show up in the text box on the right.  

     To drive a robot, double click the motor base service.  This will enable all 
movement methods.  To drive the robot with a joystick, hold down the pointer 
finger trigger button and move the joystick.  Note that you may need to select 
the joystick from the input device drop down box in the upper left.  To drive the 
robot with the mouse, click and drag the ball with the crosshairs in the upper 
left of the dashboard.  To drive the robot with the keyboard arrow keys, make sure 
the text box with the list of services is in focus.  You may need to click it once 
with the mouse.  If the robot drives too fast or slow, you can adjust the throttle 
in the upper left of the dashboard.  

     To view the laser range finder, double click a laser service in the text box.  
This will turn on both the “cylinder” and top views.  The red in both the top and 
cylinder views indicates the width of the robot projected forward.  Green dots in 
the cylinder view also show this.  The robot width is set to 400 mm which is about 
the width of the Pioneer robot.  To change the width to match your own robot, 
modify the robotWidth variable in DriveControl.cs.  


Version History
================

Version 2.0
 - Version upgrade
 - Change file location to be in 'packages' directory

Version 1.5
 - Removed joystick singularity
 - Added keyboard control
 - Red laser hits indicate robot width in top view
 - Green dots in cylinder view indicate robot width
 - Default throttle set to 75%
 - Changed laser max distance to 8000 to accommodate laser changes in 1.5
 - Added entity prefixes (from standard 1.5 version)

Version 1.0
 - Improved joystick control (trigonometric transform)
 - Added throttle control
 - Set localhost as default machine name
 - Removed “drive” and “stop” buttons
 - Robot width shown in LRF cylinder view
 - Top view of LRF added

-----------------------------------------------------------------------
Ben Axelrod 2009
http://www.benaxelrod.com


