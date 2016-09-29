#include "StdAfx.h"

STM32Programmer::STM32Programmer(void)
{
	// Create RX buffer
	RXBuffer = gcnew cli::array<unsigned char,1>(STM32_BUFFER_SIZE);
	RXBufPtr = 0;

	HexEntries = gcnew cli::array<HexEntry^,1>(MAX_HEX_ENTRIES);
	HexEntryCount = 0;

	this->FirmwareSize = 0;
	this->HexEntryCount = 0;
}


int STM32Programmer::TransmitBytes( cli::array<unsigned char,1>^ data_array, int count )
{
	try
	{
		// Transmit data
		this->Write(data_array,0,count);
	}
	catch( Exception^ /*e*/ )
	{
		return 0;
	}

	return count;
}

int STM32Programmer::ReadBytes( cli::array<unsigned char,1>^ data_array, int count )
{
	int BytesReturned;
	int TotalBytesRead = 0;

	try
	{
		while( TotalBytesRead < count )
		{
			BytesReturned = this->Read( data_array, TotalBytesRead, (count - TotalBytesRead) );
			TotalBytesRead += BytesReturned;
		}
	}
	catch( Exception^ /*e*/ )
	{
		return 0;
	}

	return TotalBytesRead;
}

int STM32Programmer::OpenPort( String^ port_name, int baud_rate )
{
	// Copy settings to serial port
	this->BaudRate = baud_rate;
	this->Parity = System::IO::Ports::Parity::Even;
	this->PortName = port_name;
	this->StopBits = System::IO::Ports::StopBits::One;

	this->ReadTimeout = 2000;

	// Attemp to connect
	try
	{
		this->Open();
	}
	catch( Exception^ /*e*/ )
	{
		return STM32_ERROR_UNABLE_TO_OPEN_PORT;
	}

	return STM32_SUCCESS;
}

/***************************************************************************************************
int STM32Programmer::ProgramDevice( String^ hexFilePath )

Loads the specified firmware file into memory.  This function loads each hex record from the hex file,
and consolidates records from adjacent data hex records.

This function should be called before 

***************************************************************************************************/
int STM32Programmer::LoadFirmware( String^ hexFilePath )
{
	this->FirmwareSize = 0;
	this->HexEntryCount = 0;

	// OPEN THE FIRMWARE FILE
	try
	{
		// Open the firmware file
		hexFile = gcnew StreamReader( hexFilePath );
	}
	catch( Exception^ /*e*/ )
	{
		return STM32_ERROR_HEXFILE_OPEN_FAILED;
	}

	// START READING DATA FROM THE .HEX FILE AND WRITING IT TO THE DEVICE
	String^ hexEntry = hexFile->ReadLine();
	UInt32 high_address = 0x00000000;

	while( hexEntry != nullptr )
	{
		HexEntry^ current_entry = gcnew HexEntry;

		// Extract hex record data from entry
		current_entry->RecordLength = Convert::ToByte(hexEntry->Substring(1,2), 16);
		current_entry->Address = Convert::ToUInt16(hexEntry->Substring(3,4), 16);
		current_entry->RecordType = Convert::ToByte(hexEntry->Substring(7,2), 16);
		
		for( Byte index = 0; index < current_entry->RecordLength; index++ )
		{
			current_entry->data[index] = Convert::ToByte(hexEntry->Substring(9+index*2, 2), 16);
		}
		
		current_entry->Checksum = Convert::ToByte(hexEntry->Substring(9+current_entry->RecordLength*2,2), 16);

		// Compute full address
		current_entry->Address |= high_address;

		// Attempt to consolidate this record if possible
		if( HexEntryCount > 0 )
		{
			HexEntry^ previous_entry = HexEntries[HexEntryCount-1];
			
			// Previous record must be a data record, its data must be exactly adjacent to the current record's data,
			// and the length of the data in the previous record must not be too long.
			if( (previous_entry->RecordType == 0x00) && (current_entry->RecordType == 0x00) &&
				((previous_entry->Address + previous_entry->RecordLength) == current_entry->Address) &&
				((previous_entry->RecordLength + current_entry->RecordLength) < MAX_RECORD_LENGTH) )
			{
				// Add the data in the current record to the data in the preceding record.  
				for( int index = 0; index < current_entry->RecordLength; index++ )
				{
					previous_entry->data[previous_entry->RecordLength + index] = current_entry->data[index];
				}

				previous_entry->RecordLength += current_entry->RecordLength;
			}
			else
			{
				// Just add this entry to the array - the data could not be consolidated, either because the
				// addresses were not adjacent to each-other, because the previous entry was not a data record,
				// or because the previous record already has the maximum allowed data.
				HexEntries[HexEntryCount] = current_entry;
				HexEntryCount++;
			}
		}
		else
		{
			// Just add this entry to the array - this is the first entry
			HexEntries[HexEntryCount] = current_entry;
			HexEntryCount++;
		}

		// If this is an Extended Linear Address Record, record the data in the record as the high bytes for subsequent addresses
		if( current_entry->RecordType == 0x04 )
		{
			high_address = (UInt32)(current_entry->data[0] << 24) | (current_entry->data[1] << 16);
		}

		if( current_entry->RecordType == 0x00 )
		{
			this->FirmwareSize += current_entry->RecordLength;
		}

		hexEntry = hexFile->ReadLine();
	}

	this->hexFile->Close();

	OnStatusChange( L"Firmware Loaded - Ready to Program." );

	return STM32_SUCCESS;		 
}

/***************************************************************************************************
int STM32Programmer::ProgramDevice( )

This function call handles everything needed to program the firmware using the STM32 bootloader.
It is assumed that the serial port is already open and properly configured, and that the firmware
file has already been loaded into memory by calling LoadFirmware();

The order of operations is:

1. Send synchronization byte to the bootloader and wait for ACK byte (Connect() function call)
2. Erase FLASH memory pages
3. Program addresses corresponding to hex records loaded by LoadFirmware()

***************************************************************************************************/
int STM32Programmer::ProgramDevice( )
{
	int returnval;

	OnProgressUpdate( 0 );

	OnStatusChange( L"Connecting to bootloader" );
	// CONNECT WITH THE BOOTLOADER
	returnval = Connect();
	
	if( returnval != STM32_SUCCESS )
	{
		OnStatusChange( L"Failed to connect to bootloader" );
		return returnval;
	}

	OnStatusChange( L"Erasing FLASH data" );
	// PERFORM A GLOBAL ERASE
	returnval = GlobalErase();

	if( returnval != STM32_SUCCESS )
	{
		OnStatusChange( L"Failed to clear FLASH data" );
		return Cleanup(returnval);
	}


	OnStatusChange( L"Writing program to FLASH and verifying" );
	// WRITE EACH HEX DATA RECORD TO THE DEVICE
	int bytes_written = 0;
	double progress = 0;
	double last_reported_progress = 0;
	cli::array<unsigned char,1>^ rx_data = gcnew cli::array<unsigned char,1>(255);


	for( int index = 0; index < HexEntryCount; index++ )
	{
		if( HexEntries[index]->RecordType == 0x00 )
		{
			// Write Bytes
			returnval = WriteMemory( HexEntries[index]->Address, HexEntries[index]->RecordLength, HexEntries[index]->data );

			if( returnval != STM32_SUCCESS )
			{
				return Cleanup(returnval);
			}

			// Verify that write operation was succesfull
			returnval = ReadMemory( HexEntries[index]->Address, HexEntries[index]->RecordLength, rx_data );

			if( returnval != STM32_SUCCESS )
			{
				return Cleanup(returnval);
			}

			// Check to make sure the data matches
			for( int i = 0; i < HexEntries[index]->RecordLength; i++ )
			{
				if( HexEntries[index]->data[i] != rx_data[i] )
				{
					return Cleanup(STM32_ERROR_READBACK_MISMATCH);
				}
			}

			bytes_written += HexEntries[index]->RecordLength;
			progress = ((double)bytes_written/(double)this->FirmwareSize)*100;

			if( (progress - last_reported_progress) >= 1 )
			{
				last_reported_progress = progress;
				OnProgressUpdate( (int)Math::Round(progress) );
			}
		}
	}

	OnProgressUpdate( 100 );
	OnStatusChange( L"Programming and Verification Complete" );

	return Cleanup(STM32_SUCCESS);
}

/********************************************************************************************
int STM32Programmer::Cleanup( int returnval, StreamReader^ hexFile )

Before ProgramDevice returns, it calls Cleanup to close files, etc.

********************************************************************************************/
int STM32Programmer::Cleanup( int returnval )
{
	return returnval;
}

/********************************************************************************************
int STM32Programmer::GlobalErase( )

Performs a global erase of all FLASH memory on the device.

********************************************************************************************/
int STM32Programmer::GlobalErase( )
{
	int data_count = 0;
	cli::array<unsigned char,1>^ data_array = gcnew cli::array<unsigned char,1>(2);

	// Send FLASH erase command
	data_array[0] = 0x43;
	data_array[1] = 0xBC;
	data_count = TransmitBytes( data_array, 2 );

	if( data_count != 2 )
	{
		return STM32_ERROR_SERIAL_CONNECTION_FAILED;
	}

	// Wait for ACK reception
	data_count = ReadBytes( data_array, 1 );

	if( data_count != 1 )
	{
		return STM32_ERROR_COMMUNICATION_LOST;
	}

	if( data_array[0] != 0x79 )
	{
		return STM32_ERROR_NACK_RECEIVED;
	}

	// Send bytes indicating a global erase
	data_array[0] = 0xFF;
	data_array[1] = 0x00;

	data_count = TransmitBytes( data_array, 2 );

	if( data_count != 2 )
	{
		return STM32_ERROR_SERIAL_CONNECTION_FAILED;
	}

	// Wait for ACK reception
	data_count = ReadBytes( data_array, 1 );

	if( data_count != 1 )
	{
		return STM32_ERROR_COMMUNICATION_LOST;
	}

	if( data_array[0] != 0x79 )
	{
		return STM32_ERROR_NACK_RECEIVED;
	}

	
	return STM32_SUCCESS;
}

/********************************************************************************************
int STM32Programmer::Connect()

Attempts to establish a connection with the STM32 bootloader.  
Returns STM32_SUCCESS if it succeeds.

If unable to write data to the serial port, returns STM32_ERROR_SERIAL_CONNECTION_FAILED
If able to write data to the serial port, but receives to response, returns
STM32_ERROR_DEVICE_UNAVAILABLE.

This function activates the auto baud-rate detection circuitry on the STM32 bootloader.

********************************************************************************************/
int STM32Programmer::Connect()
{
	int data_count = 0;
	cli::array<unsigned char,1>^ data_array = gcnew cli::array<unsigned char,1>(1);

	data_array[0] = 0x7F;
	data_count = TransmitBytes( data_array, 1 );

	if( data_count != 1 )
	{
		return STM32_ERROR_SERIAL_CONNECTION_FAILED;
	}

	data_count = ReadBytes( data_array, 1 );

	if( data_count != 1 )
	{
		return STM32_ERROR_DEVICE_UNAVAILABLE;
	}

	if( data_array[0] == 0x79 )
	{
		return STM32_SUCCESS;
	}
	else
	{
		return STM32_ERROR_DEVICE_UNAVAILABLE;
	}
}

/********************************************************************************************
int STM32Programmer::WriteMemory( UInt32 address, Byte bytes_to_write, cli::array<Byte,1>^ data )

Writes "bytes_to_write" bytes of data contained in array "data" to FLASH memory, starting at
address "address."

********************************************************************************************/
int STM32Programmer::WriteMemory( UInt32 address, Byte bytes_to_write, cli::array<Byte,1>^ FLASH_data )
{
	int data_count = 0;
	cli::array<unsigned char,1>^ data_array = gcnew cli::array<unsigned char,1>(300);

	// Send WRITE command
	data_array[0] = 0x31;
	data_array[1] = 0xCE;
	data_count = TransmitBytes( data_array, 2 );

	if( data_count != 2 )
	{
		return STM32_ERROR_SERIAL_CONNECTION_FAILED;
	}

	// Wait for ACK reception
	data_count = ReadBytes( data_array, 1 );

	if( data_count != 1 )
	{
		return STM32_ERROR_COMMUNICATION_LOST;
	}

	if( data_array[0] != 0x79 )
	{
		return STM32_ERROR_NACK_RECEIVED;
	}

	// Send the start address for the write and a checksum
	data_array[0] = (Byte)((address >> 24) & 0x0FF);
	data_array[1] = (Byte)((address >> 16) & 0x0FF);
	data_array[2] = (Byte)((address >> 8) & 0x0FF);
	data_array[3] = (Byte)(address & 0x0FF);

	data_array[4] = data_array[0] ^ data_array[1] ^ data_array[2] ^ data_array[3];

	data_count = TransmitBytes( data_array, 5 );

	if( data_count != 5 )
	{
		return STM32_ERROR_SERIAL_CONNECTION_FAILED;
	}

	// Wait for ACK reception
	data_count = ReadBytes( data_array, 1 );

	if( data_count != 1 )
	{
		return STM32_ERROR_COMMUNICATION_LOST;
	}

	if( data_array[0] != 0x79 )
	{
		return STM32_ERROR_NACK_RECEIVED;
	}

	// Send the number of bytes to be written - 1, the data, and another checksum
	data_array[0] = bytes_to_write - 1;
	Byte checksum = data_array[0];
	for( Byte i = 0; i < bytes_to_write; i++ )
	{
		data_array[i+1] = FLASH_data[i];
		checksum = checksum ^ FLASH_data[i];
	}
	data_array[bytes_to_write+1] = checksum;

	data_count = TransmitBytes( data_array, bytes_to_write+2 );

	if( data_count != (bytes_to_write+2) )
	{
		return STM32_ERROR_SERIAL_CONNECTION_FAILED;
	}

	// Wait for ACK reception
	data_count = ReadBytes( data_array, 1 );

	if( data_count != 1 )
	{
		return STM32_ERROR_COMMUNICATION_LOST;
	}

	if( data_array[0] != 0x79 )
	{
		return STM32_ERROR_NACK_RECEIVED;
	}

	// FINISHED!
	return STM32_SUCCESS;
}


int STM32Programmer::ReadMemory( UInt32 address, Byte bytes_to_read, cli::array<unsigned char,1>^ rx_data )
{
	int data_count = 0;
	cli::array<unsigned char,1>^ data_array = gcnew cli::array<unsigned char,1>(10);

	// Send READ command
	data_array[0] = 0x11;
	data_array[1] = 0xEE;
	data_count = TransmitBytes( data_array, 2 );

	if( data_count != 2 )
	{
		return STM32_ERROR_SERIAL_CONNECTION_FAILED;
	}

	// Wait for ACK reception
	data_count = ReadBytes( data_array, 1 );

	if( data_count != 1 )
	{
		return STM32_ERROR_COMMUNICATION_LOST;
	}

	if( data_array[0] != 0x79 )
	{
		return STM32_ERROR_NACK_RECEIVED;
	}

	// Send the start address for the read and a checksum
	data_array[0] = (Byte)((address >> 24) & 0x0FF);
	data_array[1] = (Byte)((address >> 16) & 0x0FF);
	data_array[2] = (Byte)((address >> 8) & 0x0FF);
	data_array[3] = (Byte)(address & 0x0FF);

	data_array[4] = data_array[0] ^ data_array[1] ^ data_array[2] ^ data_array[3];

	data_count = TransmitBytes( data_array, 5 );

	if( data_count != 5 )
	{
		return STM32_ERROR_SERIAL_CONNECTION_FAILED;
	}

	// Wait for ACK reception
	data_count = ReadBytes( data_array, 1 );

	if( data_count != 1 )
	{
		return STM32_ERROR_COMMUNICATION_LOST;
	}

	if( data_array[0] != 0x79 )
	{
		return STM32_ERROR_NACK_RECEIVED;
	}

	// Send the number of bytes to be read - 1 and another checksum
	data_array[0] = bytes_to_read - 1;
	data_array[1] = ~data_array[0];
	
	data_count = TransmitBytes( data_array, 2 );

	if( data_count != 2 )
	{
		return STM32_ERROR_SERIAL_CONNECTION_FAILED;
	}

	// Wait for ACK reception
	data_count = ReadBytes( data_array, 1 );

	if( data_count != 1 )
	{
		return STM32_ERROR_COMMUNICATION_LOST;
	}

	if( data_array[0] != 0x79 )
	{
		return STM32_ERROR_NACK_RECEIVED;
	}

	// Now read in the data
	data_count = ReadBytes( rx_data, bytes_to_read );

	if( data_count != bytes_to_read )
	{
		return STM32_ERROR_COMMUNICATION_LOST;
	}

	return STM32_SUCCESS;
}


int STM32Programmer::EraseMemory( int page_count, cli::array<unsigned char,1>^ pages )
{
	return 0;
}

int STM32Programmer::Get( cli::array<unsigned char,1>^ data )
{
	return 0;
}

int STM32Programmer::GetID( cli::array<unsigned char,1>^ data )
{
	return 0;
}