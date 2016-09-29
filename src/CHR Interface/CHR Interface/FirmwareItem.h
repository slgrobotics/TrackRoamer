#pragma once

using namespace System;
using namespace System::Windows::Forms;

#define		MAX_OPTIONS		16

ref class FirmwareItem : public System::Windows::Forms::TreeNode
{
public:
	FirmwareItem(void);

	System::String^ GetDescription( void ) { return Description; };
	void SetDescription( System::String^ Description ) { this->Description = Description; };

	System::UInt32 GetAddress( void ) { return Address; };
	void SetAddress( System::UInt32 Address ) { this->Address = Address; };

	System::UInt32 GetBits( void ) { return Bits; };
	void SetBits( System::UInt32 Bits ) { this->Bits = Bits; };

	System::UInt32 GetStart( void ) { return Start; };
	void SetStart( System::UInt32 Start ) { this->Start = Start; };

	System::String^ GetDataType( void ) { return DataType; };
	void SetDataType( System::String^ DataType ) { this->DataType = DataType; };

	System::String^ GetBaseText( void ) { return BaseText; };
	void SetBaseText( System::String^ BaseText ) { this->BaseText = BaseText; };

	System::String^ GetValueAlign( void ) { return ValueAlign; };
	void SetValueAlign( System::String^ ValueAlign ) { this->ValueAlign = ValueAlign; };

	float GetMin( void ) { return Min; };
	void SetMin( float Min ) { this->Min = Min; };

	float GetMax( void ) { return Max; };
	void SetMax( float Max ) { this->Max = Max; };

	float GetScaleFactor( void ) { return ScaleFactor; };
	void SetScaleFactor( float ScaleFactor ) { this->ScaleFactor = ScaleFactor; };

	System::UInt32 GetParentIndex( void ) { return ParentIndex; };
	void SetParentIndex( System::UInt32 ParentIndex ) { this->ParentIndex = ParentIndex; };

	bool GetBinaryData( void ) { return BinaryData; };
	void SetBinaryData( bool BinaryData ) { this->BinaryData = BinaryData; };

	float GetFloatData( void ) { return FloatData; };
	void SetFloatData( float FloatData ) { this->FloatData = FloatData; };

	Int32 GetIntData( void ) { return IntData; };
	void SetIntData( Int32 IntData ) { this->IntData = IntData; };

	UInt32 GetUIntData( void ) { return UIntData; };
	void SetUIntData( UInt32 UIntData ) { this->UIntData = UIntData; };

	UInt32 GetOptionCount( void ) { return OptionCount; };
	void SetOptionCount( UInt32 OptionCount ) { this->OptionCount = OptionCount; };

	UInt32 GetOptionSelection( void ) { return OptionSelection; };
	void SetOptionSelection( UInt32 OptionSelection ) { this->OptionSelection = OptionSelection; };

	FirmwareOption^ GetFirmwareOption( UInt32 index ) { return Options[index]; };
	void AddOption( void ) { Options[OptionCount] = gcnew FirmwareOption(); OptionCount++; };

	FirmwareItem^ Duplicate( void );

	void SetTitle( )
	{
		String^ text;

		if( Int32::Parse(this->Name) == this->ParentIndex )
		{
			text = this->GetBaseText();
			return;
		}

		if( this->ValueAlign == L"left" )
		{
			text = L"(" + this->GetStringData() + L") " + this->GetBaseText();
		}
		else
		{
			text = this->GetBaseText() + L" (" + this->GetStringData() + L")";
		}

		if( this->Text != text )
			this->Text = text;
	}

	String^ GetStringData( void )
	{
		if( DataType == L"float" )
		{
			return FloatData.ToString();
		}
		else if( DataType == L"binary" || DataType == L"en/dis" )
		{
			if( BinaryData )
			{
				return L"1";
			}
			else
			{
				return L"0";
			}
		}
		else if( DataType == L"int16" || DataType == L"int32" )
		{
			return IntData.ToString();
		}
		else if( DataType == L"uint16" || DataType == L"uint32" )
		{
			return UIntData.ToString();
		}
		else if( DataType == L"option" )
		{
			return OptionSelection.ToString();
		}
		else
		{
			return L"";
		}
}

private:
	// Name and Text attributes are in the parent class
//	System::String^ Name;
	System::String^ BaseText;
	System::String^ Description;
	System::UInt32 Address;
	System::UInt32 Bits;
	System::UInt32 Start;
	System::String^ DataType;
	System::String^ ValueAlign;
	float Min;
	float Max;
	float ScaleFactor;

	float FloatData;
	bool BinaryData;
	UInt32 UIntData;
	Int32 IntData;
	String^ StringData;

	System::UInt32 ParentIndex;

	cli::array<FirmwareOption^>^ Options;
	UInt32 OptionCount;
	UInt32 OptionSelection;
};
