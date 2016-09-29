CH Robotics Serial Interface
Description and Revision History

Description
-------------------------------------------------
The CH Robotics Serial Interface software provides a generic interface for plotting data and reading/writing configuration settings on CH Robotics sensors.
The serial interface is designed to be platform agnostic, so that any device conforming to the correct communication protocol can be used with the software.
Currently, only the UM6 from CH Robotics supports the correct communication protocol.

Revision History
-------------------------------------------------
- v1.0

Initial release.  Supports reading and modifying configuration registers, plotting data from data registers, basic magnetometer calibration, and data logging.

- v1.1

Updated to provide more instructive information in the event of an .XML parsing error.  Also updated the installer to automatically overwrite old versions on an install.

- v1.2

Fixed a bug that caused the XML parsing code to fail when running in countries where a comma is expected instead of a decimal for fractional numbers.

- v2.0.0

Added support for UM6 firmware revision UM62A (adds support for gyro temperature compensation and SPI communication).

- v2.1.0

Added code that allows CH Robotics sensors to be reprogrammed from within the Serial Interface software.  The new programming functionality is available in the "Serial Settings" tab.