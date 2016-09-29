#pragma once

using namespace System;

ref class FirmwareOption
{
public:
	FirmwareOption(void);

	property String^ Name
	{
		String^ get()
		{
			return _Name;
		}

		void set( String^ Name )
		{
			_Name = Name;
		}
	}

	property String^ Description
	{
		String^ get()
		{
			return _Description;
		}

		void set( String^ Description )
		{
			_Description = Description;
		}
	}

	property UInt32 Value
	{
		UInt32 get()
		{
			return _Value;
		}

		void set( UInt32 Value )
		{
			_Value = Value;
		}
	}

private:
	String^ _Name;
	String^ _Description;
	UInt32 _Value;
};
