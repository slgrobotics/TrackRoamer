#pragma once

using namespace System;

ref class FirmwareRegister
{
public:
	FirmwareRegister(void);

	property UInt32 Contents
	{
		UInt32 get()
		{
			return _contents;
		}

		void set( UInt32 Contents )
		{
			this->_contents = Contents;
		}
	}

	property String^ Name
	{
		String^ get()
		{
			return _name;
		}

		void set( String^ Name )
		{
			this->_name = Name;
		}
	}

	property String^ Description
	{
		String^ get()
		{
			return _description;
		}

		void set( String^ Description )
		{
			this->_description = Description;
		}
	}

	property bool UserModified
	{
		bool get()
		{
			return _user_modified;
		}

		void set( bool userModified )
		{
			this->_user_modified = userModified;
		}
	}

	property bool SensorModified
	{
		bool get()
		{
			return _sensor_modified;
		}

		void set( bool sensorModified )
		{
			this->_sensor_modified = sensorModified;
		}
	}

private:

	UInt32 _contents;
	String^ _name;
	String^ _description;

	bool _user_modified;
	bool _sensor_modified;
	
};
