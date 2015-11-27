
This is a MRDS 2008 R3 Microsoft GPS sample, modified slightly to work in R4 Beta and with HI-406BT (Bluetooth Sirf III) device.

To run:

	cd bin
	dsshost /p:50000 /m:"..\Projects\MicrosoftGps\Config\MicrosoftGps.user.manifest.xml"

See more info at:
	http://msdn.microsoft.com/en-us/library/bb483029.aspx
	http://data.xlfqnet.cn/u/UUYOPoMJLfJ2@WXDmKMOBK.html

Pairing code for HI-406BT - 0000  (creates COM6 on my computer; baud rate seems to not matter, I am using 9600. It asks for pairing code every time - very inconvenient)

C:\Microsoft Robotics Dev Studio 4\store\microsoftgps.config.xml will be created on successful connection and used later on startup.

Complete log is available at http://localhost:50000/console/output  (check "Level")

=================
The HI-406BT modifications were:
1. changing the C:\Microsoft Robotics Dev Studio 4\projects\MicrosoftGps\MicrosoftGps.cs : 400 to accept package length 13, and line 492 to accept package length 10.
2. for the History to be accumulated, GpRmc packets should be used from HI-406BT as GpGll are not available. See _HI-406BT_sample.txt for data types received. Here is the code around line 310:

                if (_state.MicrosoftGpsConfig.CaptureHistory
                    && _state.GpGsa != null && _state.GpGsa.IsValid
                    && (_state.GpGll != null && _state.GpGll.IsValid || _state.GpRmc != null && _state.GpRmc.IsValid)
                    && _state.GpGga != null && _state.GpGga.IsValid)
                {
                    double Latitude, Longitude;
                    DateTime LastUpdate;

                    if (_state.GpGll != null && _state.GpGll.IsValid)
                    {
                        Latitude = _state.GpGll.Latitude;
                        Longitude = _state.GpGll.Longitude;
                        LastUpdate = _state.GpGll.LastUpdate;
                    }
                    else
                    {
                        Latitude = _state.GpRmc.Latitude;
                        Longitude = _state.GpRmc.Longitude;
                        LastUpdate = _state.GpRmc.LastUpdate;
                    }

                    EarthCoordinates ec = new EarthCoordinates(Latitude, Longitude, _state.GpGga.AltitudeMeters, LastUpdate, _state.GpGsa.HorizontalDilutionOfPrecision, _state.GpGsa.VerticalDilutionOfPrecision);
                    _state.History.Add(ec);
                    if (_state.History.Count % 100 == 0)
                        SaveState(_state);
                }

Conversion from Virtual Earth 3 to Bing VE 6.3:   http://msdn.microsoft.com/en-us/library/cc161073.aspx

<script type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.3"></script>
- and other modifications to MicrosoftGpsMap.user.xslt (and MicrosoftGpsMap.xslt just in case)


