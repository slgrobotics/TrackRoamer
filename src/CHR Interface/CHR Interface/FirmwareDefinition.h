#pragma once

using namespace System;
using namespace System::Xml;
using namespace System::Collections;
using namespace System::Windows::Forms;

#define		MAX_DATA_ITEMS			500
#define		MAX_CONFIG_ITEMS		500
#define		MAX_REGISTERS			256
#define		MAX_COMMANDS			85

#define		CONFIG_ITEM				0
#define		DATA_ITEM				1

// Definitions of constant register address (addresses that must remain the same on all firmware versions)
#define		UM6_GET_FW_VERSION			170

// Definitions of some error packets - these are also required to remain constant through all firmware revisions
#define		UM6_BAD_CHECKSUM			253
#define		UM6_UNKNOWN_ADDRESS			254
#define		UM6_INVALID_BATCH_SIZE		255

#define		CONFIG_REGISTER_START_ADDRESS	0
#define		DATA_REGISTER_START_ADDRESS		85
#define		COMMAND_START_ADDRESS			170

#define		CONFIG_UPDATE					0
#define		DATA_UPDATE						1
#define		CONFIG_AND_DATA_UPDATE			2

// Structure for packaging floating point values as integers and vice-versa
typedef union __fconvert 
{
	 int int32_val;
	 unsigned int uint32_val;
	 float float_val;
} fConvert;

ref class FirmwareDefinition
{
public:
	FirmwareDefinition(void);

	int XML_parse( System::String^ file );

	System::String^ GetDescription( void ) { return Description; };
	System::String^ GetName( void ) { return Name; };
	System::String^ GetID( void ) { return ID; };

	System::UInt32 GetDataItemCount( void ) { return DataItemCount; };
	System::UInt32 GetConfigItemCount( void ) { return ConfigItemCount; };

	FirmwareItem^ GetConfigItem( int index ) { return ConfigItems[index]; };
	FirmwareItem^ GetDataItem( int index ) { return DataItems[index]; };

	FirmwareCommand^ GetCommand( int index )  { return Commands[index]; };
	UInt32 GetCommandCount() { return CommandCount; };

	cli::array<FirmwareItem^>^ GetConfigItems( void ) { return ConfigItems; };
	cli::array<FirmwareItem^>^ GetDataItems( void ) { return DataItems; };

	cli::array<FirmwareCommand^>^ GetCommands( void ) { return Commands; };

	String^ GetCommandName( UInt32 address );
	UInt32 GetCommandAddress( String^ text );

	void UpdateRegistersFromItems( void );
	void UpdateItemsFromRegisters( int update_type );
	void UpdateItemFromRegister( int item_index, int item_type );
	void UpdateRegisterFromItem( int item_index, int item_type );

	void SetRegisterContents( unsigned char address, UInt32 data );
	void MarkRegisterAsUserModified( unsigned char address, bool modified );
	
	FirmwareRegister^ GetRegister( int index ) { return Registers[index]; };

private:
	void ExtractXMLRecursive( XmlNode^ current_node );
	void SetData( System::String^ parent_name, System::String^ current_name, System::String^ value );

	// Variables used for initialization
	System::UInt32 DataParentIndex;
	System::UInt32 ConfigParentIndex;

	int CurrentRegisterAddress;

	// Firmware description
	System::String^ Description;
	System::String^ Name;
	System::String^ ID;

	System::UInt32 DataItemCount;
	System::UInt32 ConfigItemCount;
	System::UInt32 CommandCount;

	cli::array<FirmwareItem^>^ DataItems;
	cli::array<FirmwareItem^>^ ConfigItems;

	// Array of firmware registers on the sensor
	cli::array<FirmwareRegister^>^ Registers;

	// Array of commands that can be sent to the sensor
	cli::array<FirmwareCommand^>^ Commands;

};
