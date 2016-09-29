#include "StdAfx.h"
#include "FirmwareItem.h"

FirmwareItem::FirmwareItem(void)
{
	this->FloatData = 0.0;
	this->BinaryData = false;
	this->UIntData = 0;
	this->IntData = 0;
	this->StringData = L"";
	this->ValueAlign = L"";
	this->OptionCount = 0;

	Options = gcnew cli::array<FirmwareOption^>(MAX_OPTIONS);
}

FirmwareItem^ FirmwareItem::Duplicate()
{
	FirmwareItem^ newItem = gcnew FirmwareItem;
	
	// Copy information relevant to list item.  Note that child nodes are NOT copied by this function
	newItem->Checked = this->Checked;
	newItem->Name = this->Name;
	newItem->Text = this->Text;
	newItem->ToolTipText = this->ToolTipText;

	// Now copy all the other information specific to firmware items
	newItem->BaseText = this->BaseText;
	newItem->Description = this->Description;
	newItem->Address = this->Address;
	newItem->Bits = this->Bits;
	newItem->Start = this->Start;
	newItem->DataType = this->DataType;
	newItem->ValueAlign = this->ValueAlign;
	newItem->Min = this->Min;
	newItem->Max = this->Max;
	newItem->ScaleFactor = this->ScaleFactor;
	newItem->FloatData = this->FloatData;
	newItem->BinaryData = this->BinaryData;
	newItem->UIntData = this->UIntData;
	newItem->IntData = this->IntData;
	newItem->StringData = this->StringData;
	newItem->ParentIndex = this->ParentIndex;
	Array::Copy(this->Options, newItem->Options, this->Options->Length);
	newItem->OptionCount = this->OptionCount;
	newItem->OptionSelection = this->OptionSelection;

	return newItem;
}
