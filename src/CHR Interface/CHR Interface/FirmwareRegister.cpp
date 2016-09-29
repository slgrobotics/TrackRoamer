#include "StdAfx.h"
#include "FirmwareRegister.h"

FirmwareRegister::FirmwareRegister(void)
{
	_contents = 0;
	_user_modified = false;
}
