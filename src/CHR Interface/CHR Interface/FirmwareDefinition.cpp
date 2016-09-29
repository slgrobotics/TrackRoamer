using namespace System::Xml;
using namespace System::Collections;
using namespace System::Math;

#include "StdAfx.h"
#include "FirmwareDefinition.h"

FirmwareDefinition::FirmwareDefinition(void)
{
	DataItems = gcnew array<FirmwareItem^>(MAX_DATA_ITEMS);
	ConfigItems = gcnew array<FirmwareItem^>(MAX_CONFIG_ITEMS);
	Registers = gcnew array<FirmwareRegister^>(MAX_REGISTERS);
	Commands = gcnew array<FirmwareCommand^>(MAX_COMMANDS);

	ConfigItemCount = 0;
	DataItemCount = 0;
	CurrentRegisterAddress = 0;
}

int FirmwareDefinition::XML_parse( System::String^ file )
{
	// Load XML File
	XmlDocument^ xmlDoc = gcnew XmlDocument;

	try {
		xmlDoc->Load( file );
	}
	catch( System::Exception^ e ) {
		throw( e );
	}

	// Read firmware name, version, description, etc.
	XmlNodeList^ firmwareHeader = xmlDoc->GetElementsByTagName("CHR_FIRMWARE");
	int headerCount = firmwareHeader->Count;

	if( headerCount != 1 )
	{
		return 0;
	}

	try {
		Description = firmwareHeader->Item(0)->Attributes->GetNamedItem("description")->Value;
		Name = firmwareHeader->Item(0)->Attributes->GetNamedItem("name")->Value;
		ID = firmwareHeader->Item(0)->Attributes->GetNamedItem("id")->Value;
	}
	catch( System::Exception^ e ) {
		throw( e );
	}

	XmlNode^ root_node = xmlDoc->DocumentElement;
	
	ExtractXMLRecursive( root_node );

	return 1;
}

void FirmwareDefinition::ExtractXMLRecursive( XmlNode^ current_node )
{
	if( current_node->Name == L"#comment" )
	{
		return;
	}

	// Determine the name of the parent of this node - will help determine what to do with it
	System::String^ parent_name = current_node->ParentNode->Name;
	System::String^ current_name = current_node->Name;

	// If the next child node contains only text, then (given the formatting requirements of the XML file) this node contains
	// data.  Knowledge of the parent name, the current node name, and the value of the current node is sufficient to extract all
	// relevant information.  If the SetData function doesn't recognize the parent name or the current_name, it throws an exception.
	XmlNode^ child =current_node->FirstChild;
	if( (child != nullptr) && (child->Name == L"#text") )
	{
		SetData( parent_name, current_name, current_node->FirstChild->Value );

		return;
	}

	// If code reaches this point, then the current node contains mainly formatting information.  All names, descriptions, addresses, etc.
	// are handled by the preceding code.  The other nodes define groups of information, etc.  If the name of the current node is "ConfigGroup,"
	// then it signals the start of a new configuration group, and all subsequent Config items should be added to the new group (until a new "ConfigGroup"
	// node is encountered).  To ensure that config items are added to the correct group, a pointer is used to indicate how many groups have been
	// read by the XML file.  This counter is incremented every time a new "ConfigGroup" tag is encountered.  The counter is used when a config item is
	// found, to indicate which group to place it in.
	// The methodology is similar for data groups, and (at a lower level) config items and data items themselves.
	IEnumerator^ ienum = current_node->GetEnumerator();

	XmlNode^ child_node;

	while( ienum->MoveNext() )
	{
		child_node = dynamic_cast<XmlNode^>(ienum->Current);
		
		if( child_node->Name == L"ConfigGroup" )
		{
			ConfigItems[ConfigItemCount] = gcnew FirmwareItem();
			ConfigParentIndex = ConfigItemCount;
			ConfigItems[ConfigItemCount]->SetParentIndex( ConfigParentIndex );

			// Add information for tree view
			ConfigItems[ConfigItemCount]->Name = Convert::ToString(ConfigItemCount);

			ConfigItemCount++;

			if( ConfigItemCount >= MAX_CONFIG_ITEMS )
			{
				throw gcnew System::Exception(L"Number of configuration groups exceeded the maximum allowed.");
			}
		}
		else if( child_node->Name == L"DataGroup" )
		{
			DataItems[DataItemCount] = gcnew FirmwareItem();
			DataParentIndex = DataItemCount;
			DataItems[DataItemCount]->SetParentIndex( DataParentIndex );

			// Add information for tree view
			DataItems[DataItemCount]->Name = Convert::ToString(DataItemCount);

			DataItemCount++;

			if( DataItemCount >= MAX_DATA_ITEMS )
			{
				throw gcnew System::Exception(L"Number of data groups exceeded the maximum allowed.");
			}
		}
		else if( child_node->Name == L"Register" )
		{
			String^ address_string = child_node->Attributes->GetNamedItem("address")->Value;
			Int32 address = Int32::Parse( address_string, System::Globalization::CultureInfo::InvariantCulture );
			if( address < 0 || address >= MAX_REGISTERS )
			{
				throw gcnew Exception("Invalid register address specified in XML file.");
			}

			this->CurrentRegisterAddress = address;
			Registers[CurrentRegisterAddress] = gcnew FirmwareRegister;
		}
		else if( child_node->Name == L"Command" )
		{
			Commands[CommandCount] = gcnew FirmwareCommand();

			CommandCount++;

			if( CommandCount >= MAX_DATA_ITEMS )
			{
				throw gcnew System::Exception(L"Number of commands exceeded the maximum allowed.");
			}
		}
		else if( child_node->Name == L"DataItem" )
		{
			if( current_node->Name != L"DataGroup" )
			{
				throw gcnew System::Exception("Error: <DataItem> tag parent must be <DataGroup>");
			}

			// Increment item count so that data tags are applied to the right item
			DataItems[DataItemCount] = gcnew FirmwareItem();
			DataItems[DataItemCount]->SetParentIndex( DataParentIndex );
			DataItems[DataItemCount]->SetValueAlign( L"right" );

			// Add information for tree view
			DataItems[DataItemCount]->Name = Convert::ToString(DataItemCount);
			DataItems[DataParentIndex]->Nodes->Add(DataItems[DataItemCount]);

			DataItemCount++;
		}
		else if( child_node->Name == L"ConfigItem" )
		{
			if( current_node->Name != L"ConfigGroup" )
			{
				throw gcnew System::Exception("Error: <ConfigItem> tag parent must be <ConfigGroup>");
			}

			// Increment item count so that data tags are applied to the right item
			ConfigItems[ConfigItemCount] = gcnew FirmwareItem();
			ConfigItems[ConfigItemCount]->SetParentIndex( ConfigParentIndex );
			ConfigItems[ConfigItemCount]->SetValueAlign( L"left" );
			
			// Add information for tree view
			ConfigItems[ConfigItemCount]->Name = Convert::ToString(ConfigItemCount);
			ConfigItems[ConfigParentIndex]->Nodes->Add(ConfigItems[ConfigItemCount]);

			ConfigItemCount++;
		}
		else if( child_node->Name == L"Option" )
		{
			if( current_node->Name != L"ConfigItem" )
			{
				throw gcnew System::Exception("Error: <Option> tag parent must be <ConfigItem>");
			}

			ConfigItems[ConfigItemCount-1]->AddOption();
		}

		// Process children of this node
		ExtractXMLRecursive(child_node);

	}
}

void FirmwareDefinition::SetData( System::String^ parent_name, System::String^ name, System::String^ value )
{
	FirmwareItem^ current_item;

	// Identify the parent of this item.  If the parent is "DataItem," then apply the data to the current
	// data item.  If config item, apply to current config item
	if( parent_name == L"DataItem" || parent_name == L"DataGroup")
	{
		current_item = DataItems[DataItemCount-1];
	}
	else if( parent_name == L"ConfigItem" || parent_name == L"ConfigGroup")
	{
		current_item = ConfigItems[ConfigItemCount-1];		
	}
	else if( parent_name == L"Register" )
	{
		if( name == L"Name" )
		{
			Registers[CurrentRegisterAddress]->Name = value;
		}
		else if( name == L"Description" )
		{
			Registers[CurrentRegisterAddress]->Description = value;
		}

		return;
	}
	else if( parent_name == L"Option" )
	{
		current_item = ConfigItems[ConfigItemCount-1];
		UInt32 OptionIndex = current_item->GetOptionCount() - 1;

		FirmwareOption^ option = current_item->GetFirmwareOption( OptionIndex );

		if( name == L"Name" )
		{
			option->Name = value;
		}
		else if( name == L"Value" )
		{
			try
			{
				option->Value = UInt32::Parse(value, System::Globalization::CultureInfo::InvariantCulture);
			}
			catch( System::Exception^ /*e*/ )
			{
				throw gcnew System::Exception(L"Value '" + value + L"' for option with name '" + option->Name + L"' is invalid." );
			}
		}
		else if( name == L"Description" )
		{
			option->Description = value;
		}

		return;
	}
	else if( parent_name == L"Command" )
	{
		FirmwareCommand^ current_command = Commands[CommandCount-1];

		if( name == L"Name" )
		{
			current_command->Name = value;
		}
		else if( name == L"Description" )
		{
			current_command->Description = value;
		}
		else if( name == L"Address" )
		{
			try
			{
				current_command->Address = UInt32::Parse( value, System::Globalization::CultureInfo::InvariantCulture );
			}
			catch( System::Exception^ /*e*/ )
			{
				throw gcnew System::Exception(L"Address '" + value + L"' for command with name '" + current_command->Name + L"' is invalid." );
			}
		}

		return;
	}
	else
	{
		throw gcnew System::Exception("Unrecognized text item found in XML file: " + parent_name);
	}

	if( name == L"Name" )
	{
		current_item->Text = value;
		current_item->SetBaseText( value );
	}
	else if( name == L"Description" )
	{
		current_item->SetDescription( value );
		current_item->ToolTipText = value;
	}
	else if( name == L"Address" )
	{
		try
		{
			UInt32 address = UInt32::Parse( value, System::Globalization::CultureInfo::InvariantCulture );
			current_item->SetAddress( address );
		}
		catch( System::Exception^ /*e*/ )
		{
			throw gcnew System::Exception(L"Invalid address '" + value + L"' found while parsing item with name '" + current_item->Name + L"'.");
		}
		
	}
	else if( name == L"Bits" )
	{
		try
		{
			UInt32 bits = UInt32::Parse( value, System::Globalization::CultureInfo::InvariantCulture );
			current_item->SetBits( bits );
		}
		catch( System::Exception^ /*e*/ )
		{
			throw gcnew System::Exception(L"Invalid bit count '" + value + L"' found while parsing item with name '" + current_item->Name + L"'.");
		}
	}
	else if( name == L"Start" )
	{
		try
		{
			UInt32 start = UInt32::Parse( value, System::Globalization::CultureInfo::InvariantCulture );
			current_item->SetStart( start );
		}
		catch( System::Exception^ /*e*/ )
		{
			throw gcnew System::Exception(L"Invalid start bit '" + value + L"' found while parsing item with name '" + current_item->Name + L"'.");
		}
	}
	else if( name == L"DataType" )
	{
		current_item->SetDataType( value );
	}
	else if( name == L"Min" )
	{
		try
		{
			float min = float::Parse( value, System::Globalization::CultureInfo::InvariantCulture );
			current_item->SetMin( min );
		}
		catch( System::Exception^ /*e*/ )
		{
			throw gcnew System::Exception(L"Invalid min value '" + value + L"' found while parsing item with name '" + current_item->Name + L"'.");
		}
	}
	else if( name == L"Max" )
	{
		try
		{
			float max = float::Parse( value, System::Globalization::CultureInfo::InvariantCulture );
			current_item->SetMax( max );
		}
		catch( System::Exception^ /*e*/ )
		{
			throw gcnew System::Exception(L"Invalid max value '" + value + L"' found while parsing item with name '" + current_item->Name + L"'.");
		}

	}
	else if( name == L"ScaleFactor" )
	{
		try
		{
			float ScaleFactor = float::Parse( value, System::Globalization::CultureInfo::InvariantCulture );
			current_item->SetScaleFactor( ScaleFactor );
		}
		catch( System::Exception^ /*e*/ )
		{
			throw gcnew System::Exception(L"Invalid scale factor '" + value + L"' found while parsing item with name '" + current_item->Name + L"'.");
		}

	}
	else
	{
		throw gcnew Exception("Error: Unknown tag type detected in XML file.");
	}
}

void FirmwareDefinition::UpdateRegistersFromItems( )
{
	for( UInt32 i = 0; i < ConfigItemCount; i++ )
	{
		UpdateRegisterFromItem( i, CONFIG_ITEM );
	}
/*
	for( UInt32 i = 0; i < DataItemCount; i++ )
	{
		UpdateRegisterFromItem( i, DATA_ITEM );
	}
	*/
}

void FirmwareDefinition::UpdateItemsFromRegisters( int update_type )
{

	if( update_type == CONFIG_UPDATE || update_type == CONFIG_AND_DATA_UPDATE )
	{
		for( UInt32 i = 0; i < ConfigItemCount; i++ )
		{
			UpdateItemFromRegister( i, CONFIG_ITEM );
		}
	}
	
	if( update_type == DATA_UPDATE || update_type == CONFIG_AND_DATA_UPDATE )
	{
		for( UInt32 i = 0; i < DataItemCount; i++ )
		{
			UpdateItemFromRegister( i, DATA_ITEM );
		}
	}
}

void FirmwareDefinition::UpdateItemFromRegister( int item_index, int item_type )
{
	FirmwareItem^ item;

	// Select item based on given item type and item index
	if( item_type == DATA_ITEM )
	{
		item = DataItems[item_index];
	}
	else
	{
		item = ConfigItems[item_index];
	}

	// Check to make sure this isn't a heading.  If it is, return - no data association with this item
	if( item->GetParentIndex() == item_index )
	{
		return;
	}

	// Extract register address, data type, bit length and start address from item
	System::UInt32 Address = item->GetAddress();
	System::UInt32 Bits = item->GetBits();
	System::UInt32 Start = item->GetStart();
	System::String^ DataType = item->GetDataType();

	if( DataType == L"float" )
	{
		fConvert itof;
		itof.int32_val = Registers[Address]->Contents;
		item->SetFloatData( itof.float_val );
	}
	else if( DataType == L"int32" )
	{
		item->SetIntData( (Int32)Registers[Address]->Contents );
	}
	else if( DataType == L"uint32" )
	{
		item->SetUIntData( (UInt32)Registers[Address]->Contents );
	}
	else
	{
		// Extract the data from the register using specifications in the FirmwareItem object
		int mask = (int)System::Math::Pow(2.0,(double)item->GetBits()) - 1;
		int register_value = (Registers[Address]->Contents >> item->GetStart()) & (mask);

		if( DataType == L"binary" || DataType == L"en/dis" )
		{
			if( register_value == 1 )
			{
				item->SetBinaryData( true );
			}
			else
			{
				item->SetBinaryData( false );
			}
		}
		else if( DataType == L"option" )
		{
			item->SetOptionSelection( (UInt32)register_value );
		}
		else if( DataType == L"int16" )
		{
			// Sign-extend the bits of the signed value
			Int32 data = ((Int32)register_value << 16) >> 16;
			item->SetIntData( data );
		}
		else if( DataType == L"uint16" )
		{
			item->SetUIntData( (unsigned int)register_value );
		}
	}
	
	if( item_type != DATA_ITEM )
		item->SetTitle();
}

void FirmwareDefinition::UpdateRegisterFromItem( int item_index, int item_type )
{
	FirmwareItem^ item;

	// Select item based on given item type and item index
	if( item_type == DATA_ITEM )
	{
		item = DataItems[item_index];
	}
	else
	{
		item = ConfigItems[item_index];
	}

	// Check to make sure this isn't a heading.  If it is, return - no data association with this item
	if( item->GetParentIndex() == item_index )
	{
		return;
	}

	// Extract register address, data type, bit length and start address from item
	System::UInt32 Address = item->GetAddress();
	System::UInt32 Bits = item->GetBits();
	System::UInt32 Start = item->GetStart();
	System::UInt32 SelectedOption = item->GetOptionSelection();
	System::String^ DataType = item->GetDataType();

	if( DataType == L"float" )
	{
		fConvert itof;
		itof.float_val = item->GetFloatData();
		Registers[Address]->Contents = (UInt32)itof.int32_val;
	}
	else if( DataType == L"int32" )
	{
		Registers[Address]->Contents = (UInt32)item->GetIntData();
	}
	else if( DataType == L"uint32" )
	{
		Registers[Address]->Contents = (UInt32)item->GetUIntData();
	}
	else
	{
		// Format data according to item definition and add to register
		int mask = (int)System::Math::Pow(2.0,(double)item->GetBits()) - 1;
		UInt32 item_data;

		if( DataType == L"int16" )
		{
			item_data = (UInt32)((item->GetIntData() & mask) << item->GetStart());
		}
		else if( DataType == L"uint16" )
		{
			item_data = (UInt32)((item->GetUIntData() & mask) << item->GetStart());
		}
		else if( DataType == L"en/dis" || DataType == L"binary" )
		{
			if( item->GetBinaryData() )
			{
				item_data = 1;
			}
			else
			{
				item_data = 0;
			}

			item_data = (item_data & mask) << item->GetStart();
		}
		else if( DataType == L"option" )
		{
			item_data = SelectedOption << item->GetStart();
		}

		// First, clear all the bits that are being modified
		Registers[Address]->Contents |= (mask << item->GetStart());
		Registers[Address]->Contents ^= (mask << item->GetStart());

		// Now set the data in question
		Registers[Address]->Contents |= item_data;
	}

	Registers[Address]->UserModified = true;
}


void FirmwareDefinition::SetRegisterContents( unsigned char address, UInt32 data )
{
	this->Registers[address]->Contents = data;
}

String^ FirmwareDefinition::GetCommandName( UInt32 address )
{
	for( UInt32 i = 0; i < this->CommandCount; i++ )
	{
		if( Commands[i]->Address == address )
		{
			return Commands[i]->Name;
		}
	}

	return L"";
}

UInt32 FirmwareDefinition::GetCommandAddress( String^ text )
{
	for( UInt32 i = 0; i < this->CommandCount; i++ )
	{
		if( Commands[i]->Name == text )
		{
			return Commands[i]->Address;
		}
	}

	return 0;
}

void FirmwareDefinition::MarkRegisterAsUserModified( unsigned char address, bool modified )
{
	Registers[address]->UserModified = modified;
}
