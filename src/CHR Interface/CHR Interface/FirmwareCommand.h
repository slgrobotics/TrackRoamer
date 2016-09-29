#pragma once

ref class FirmwareCommand
{
public:
	FirmwareCommand(void);

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

	property UInt32 Address
	{
		UInt32 get()
		{
			return _address;
		}

		void set( UInt32 Address )
		{
			this->_address = Address;
		}
	}

private:
	String^ _name;
	String^ _description;

	UInt32 _address;

};
