#include "StdAfx.h"
#include "HexEntry.h"

HexEntry::HexEntry(void)
{
	data = gcnew cli::array<Byte,1>(255);
}
