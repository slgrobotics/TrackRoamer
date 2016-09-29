#pragma once

ref class HexEntry
{
public:
	HexEntry(void);

	property Byte RecordLength
	{
		Byte get()
		{
			return _RecordLength;
		}

		void set( Byte RecordLength )
		{
			this->_RecordLength = RecordLength;
		}
	}

	property UInt32 Address
	{
		UInt32 get()
		{
			return _Address;
		}

		void set( UInt32 Address )
		{
			this->_Address = Address;
		}
	}

	property Byte RecordType
	{
		Byte get()
		{
			return _RecordType;
		}

		void set( Byte RecordType )
		{
			this->_RecordType = RecordType;
		}
	}

	property Byte Checksum
	{
		Byte get()
		{
			return _Checksum;
		}

		void set( Byte Checksum )
		{
			this->_Checksum = Checksum;
		}
	}

	cli::array<Byte,1>^ data;

private:

	Byte _RecordLength;
	UInt32 _Address;
	Byte _RecordType;
	Byte _Checksum;
};
