
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v2.0.50727
.module extern winusb.dll
.module extern setupapi.dll
.module extern kernel32.dll
.module extern cfgmgr32.dll
.module extern user32.dll
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly UsbWrapper
{
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 1D 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. 
                                                                                                  50 6F 6C 6F 6C 75 20 32 30 30 39 2D 32 30 31 31   // Pololu 2009-2011
                                                                                                  00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 2F 48 61 6E 64 6C 65 73 20 6C 6F 77 2D 6C   // ../Handles low-l
                                                                                                    65 76 65 6C 20 55 53 42 20 63 6F 6D 6D 75 6E 69   // evel USB communi
                                                                                                    63 61 74 69 6F 6E 20 69 6E 20 57 69 6E 64 6F 77   // cation in Window
                                                                                                    73 2E 00 00 )                                     // s...
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 06 50 6F 6C 6F 6C 75 00 00 )                // ...Pololu..
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 0A 55 73 62 57 72 61 70 70 65 72 00 00 )    // ...UsbWrapper..
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 0A 55 73 62 57 72 61 70 70 65 72 00 00 )    // ...UsbWrapper..
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 64 65 31 63 39 65 30 62 2D 33 35 64 61   // ..$de1c9e0b-35da
                                                                                                  2D 34 63 38 30 2D 62 37 66 61 2D 35 32 63 63 30   // -4c80-b7fa-52cc0
                                                                                                  33 39 36 35 38 37 66 00 00 )                      // 396587f..
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 05 31 2E 35 2E 30 00 00 )                   // ...1.5.0..

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 1:5:0:0
}
.module UsbWrapper.dll
// MVID: {637D5FC0-F0C7-4BF9-8746-AFF56B2110F1}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00330000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit Pololu.UsbWrapper.AsynchronousInTransfer
       extends [mscorlib]System.Object
{
  .field private class Pololu.UsbWrapper.UsbDevice device
  .field private uint8 endpoint
  .field private uint32 size
  .method assembly hidebysig specialname rtspecialname 
          instance void  .ctor(class Pololu.UsbWrapper.UsbDevice device,
                               uint8 endpoint,
                               uint32 size) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      class Pololu.UsbWrapper.UsbDevice Pololu.UsbWrapper.AsynchronousInTransfer::device
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      uint8 Pololu.UsbWrapper.AsynchronousInTransfer::endpoint
    IL_0014:  ldarg.0
    IL_0015:  ldarg.3
    IL_0016:  stfld      uint32 Pololu.UsbWrapper.AsynchronousInTransfer::size
    IL_001b:  ret
  } // end of method AsynchronousInTransfer::.ctor

  .method public hidebysig instance void 
          start() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method AsynchronousInTransfer::start

  .method public hidebysig specialname instance valuetype Pololu.UsbWrapper.TransferStatus 
          get_status() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method AsynchronousInTransfer::get_status

  .method public hidebysig specialname instance uint32 
          get_lengthTransferred() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method AsynchronousInTransfer::get_lengthTransferred

  .method public hidebysig specialname instance uint8[] 
          get_buffer() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method AsynchronousInTransfer::get_buffer

  .property instance valuetype Pololu.UsbWrapper.TransferStatus
          status()
  {
    .get instance valuetype Pololu.UsbWrapper.TransferStatus Pololu.UsbWrapper.AsynchronousInTransfer::get_status()
  } // end of property AsynchronousInTransfer::status
  .property instance uint32 lengthTransferred()
  {
    .get instance uint32 Pololu.UsbWrapper.AsynchronousInTransfer::get_lengthTransferred()
  } // end of property AsynchronousInTransfer::lengthTransferred
  .property instance uint8[] buffer()
  {
    .get instance uint8[] Pololu.UsbWrapper.AsynchronousInTransfer::get_buffer()
  } // end of property AsynchronousInTransfer::buffer
} // end of class Pololu.UsbWrapper.AsynchronousInTransfer

.class public auto ansi sealed Pololu.UsbWrapper.TransferStatus
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype Pololu.UsbWrapper.TransferStatus Pending = int32(0x00000000)
  .field public static literal valuetype Pololu.UsbWrapper.TransferStatus Completed = int32(0x00000001)
  .field public static literal valuetype Pololu.UsbWrapper.TransferStatus Error = int32(0x00000002)
  .field public static literal valuetype Pololu.UsbWrapper.TransferStatus TimedOut = int32(0x00000003)
  .field public static literal valuetype Pololu.UsbWrapper.TransferStatus Cancelled = int32(0x00000004)
  .field public static literal valuetype Pololu.UsbWrapper.TransferStatus Stall = int32(0x00000005)
  .field public static literal valuetype Pololu.UsbWrapper.TransferStatus NoDevice = int32(0x00000006)
  .field public static literal valuetype Pololu.UsbWrapper.TransferStatus Overflow = int32(0x00000007)
} // end of class Pololu.UsbWrapper.TransferStatus

.class public auto ansi beforefieldinit Pololu.UsbWrapper.DeviceListItem
       extends [mscorlib]System.Object
{
  .field assembly initonly int32 deviceInstance
  .field public initonly valuetype [mscorlib]System.Guid guid
  .field private string privateText
  .field public initonly string serialNumber
  .field public initonly uint16 productId
  .method public hidebysig specialname instance string 
          get_text() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string Pololu.UsbWrapper.DeviceListItem::privateText
    IL_0006:  ret
  } // end of method DeviceListItem::get_text

  .method public hidebysig specialname instance void 
          set_text(string 'value') cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      string Pololu.UsbWrapper.DeviceListItem::privateText
    IL_0007:  ret
  } // end of method DeviceListItem::set_text

  .method assembly hidebysig specialname rtspecialname 
          instance void  .ctor(int32 deviceInstance,
                               valuetype [mscorlib]System.Guid guid,
                               string text,
                               string serialNumber,
                               uint16 productId) cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 Pololu.UsbWrapper.DeviceListItem::deviceInstance
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      valuetype [mscorlib]System.Guid Pololu.UsbWrapper.DeviceListItem::guid
    IL_0014:  ldarg.0
    IL_0015:  ldarg.3
    IL_0016:  stfld      string Pololu.UsbWrapper.DeviceListItem::privateText
    IL_001b:  ldarg.0
    IL_001c:  ldarg.s    serialNumber
    IL_001e:  stfld      string Pololu.UsbWrapper.DeviceListItem::serialNumber
    IL_0023:  ldarg.0
    IL_0024:  ldarg.s    productId
    IL_0026:  stfld      uint16 Pololu.UsbWrapper.DeviceListItem::productId
    IL_002b:  ret
  } // end of method DeviceListItem::.ctor

  .method public hidebysig static class Pololu.UsbWrapper.DeviceListItem 
          CreateDummyItem(string text) cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  13
    IL_0000:  ldc.i4.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  ldc.i4.0
    IL_0005:  ldc.i4.0
    IL_0006:  ldc.i4.0
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.0
    IL_000b:  ldc.i4.0
    IL_000c:  newobj     instance void [mscorlib]System.Guid::.ctor(int32,
                                                                    int16,
                                                                    int16,
                                                                    uint8,
                                                                    uint8,
                                                                    uint8,
                                                                    uint8,
                                                                    uint8,
                                                                    uint8,
                                                                    uint8,
                                                                    uint8)
    IL_0011:  ldarg.0
    IL_0012:  ldstr      ""
    IL_0017:  ldc.i4.0
    IL_0018:  newobj     instance void Pololu.UsbWrapper.DeviceListItem::.ctor(int32,
                                                                               valuetype [mscorlib]System.Guid,
                                                                               string,
                                                                               string,
                                                                               uint16)
    IL_001d:  ret
  } // end of method DeviceListItem::CreateDummyItem

  .method public hidebysig instance bool 
          isSameDeviceAs(class Pololu.UsbWrapper.DeviceListItem deviceListItem) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Pololu.UsbWrapper.DeviceListItem::deviceInstance
    IL_0006:  ldarg.1
    IL_0007:  ldfld      int32 Pololu.UsbWrapper.DeviceListItem::deviceInstance
    IL_000c:  ceq
    IL_000e:  ret
  } // end of method DeviceListItem::isSameDeviceAs

  .property instance string text()
  {
    .get instance string Pololu.UsbWrapper.DeviceListItem::get_text()
    .set instance void Pololu.UsbWrapper.DeviceListItem::set_text(string)
  } // end of property DeviceListItem::text
} // end of class Pololu.UsbWrapper.DeviceListItem

.class private abstract auto ansi beforefieldinit Pololu.WinusbHelper.WinUsbDevice
       extends [mscorlib]System.Object
{
  .class sequential ansi sealed nested private beforefieldinit OVERLAPPED
         extends [mscorlib]System.ValueType
  {
    .field assembly native int Internal
    .field assembly native int InternalHigh
    .field assembly int32 Offset
    .field assembly int32 OffsetHigh
    .field assembly native int hEvent
  } // end of class OVERLAPPED

  .class sequential ansi sealed nested family beforefieldinit WINUSB_SETUP_PACKET
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 0
    .field assembly uint8 RequestType
    .field assembly uint8 Request
    .field assembly uint16 Value
    .field assembly uint16 Index
    .field assembly uint16 Length
  } // end of class WINUSB_SETUP_PACKET

  .field private valuetype Pololu.WinusbHelper.WinUsbDeviceHandles handles
  .method famorassem hidebysig specialname rtspecialname 
          instance void  .ctor(valuetype Pololu.WinusbHelper.WinUsbDeviceHandles handles) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldflda     valuetype Pololu.WinusbHelper.WinUsbDeviceHandles Pololu.WinusbHelper.WinUsbDevice::handles
    IL_000c:  ldarga.s   handles
    IL_000e:  ldfld      native int Pololu.WinusbHelper.WinUsbDeviceHandles::winusbHandle
    IL_0013:  stfld      native int Pololu.WinusbHelper.WinUsbDeviceHandles::winusbHandle
    IL_0018:  ldarg.0
    IL_0019:  ldflda     valuetype Pololu.WinusbHelper.WinUsbDeviceHandles Pololu.WinusbHelper.WinUsbDevice::handles
    IL_001e:  ldarga.s   handles
    IL_0020:  ldfld      class [mscorlib]Microsoft.Win32.SafeHandles.SafeFileHandle Pololu.WinusbHelper.WinUsbDeviceHandles::deviceHandle
    IL_0025:  stfld      class [mscorlib]Microsoft.Win32.SafeHandles.SafeFileHandle Pololu.WinusbHelper.WinUsbDeviceHandles::deviceHandle
    IL_002a:  ldarg.0
    IL_002b:  ldflda     valuetype Pololu.WinusbHelper.WinUsbDeviceHandles Pololu.WinusbHelper.WinUsbDevice::handles
    IL_0030:  ldarga.s   handles
    IL_0032:  ldfld      int32 Pololu.WinusbHelper.WinUsbDeviceHandles::deviceInstance
    IL_0037:  stfld      int32 Pololu.WinusbHelper.WinUsbDeviceHandles::deviceInstance
    IL_003c:  ret
  } // end of method WinUsbDevice::.ctor

  .method assembly hidebysig specialname 
          instance int32  get_deviceInstance() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldflda     valuetype Pololu.WinusbHelper.WinUsbDeviceHandles Pololu.WinusbHelper.WinUsbDevice::handles
    IL_0006:  ldfld      int32 Pololu.WinusbHelper.WinUsbDeviceHandles::deviceInstance
    IL_000b:  ret
  } // end of method WinUsbDevice::get_deviceInstance

  .method assembly hidebysig instance void 
          disconnect() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      valuetype Pololu.WinusbHelper.WinUsbDeviceHandles Pololu.WinusbHelper.WinUsbDevice::handles
    IL_0006:  call       void Pololu.WinusbHelper.Winusb::disconnect(valuetype Pololu.WinusbHelper.WinUsbDeviceHandles)
    IL_000b:  ret
  } // end of method WinUsbDevice::disconnect

  .method family hidebysig instance void 
          controlTransfer(valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET setupPacket) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  3
    .locals init (uint32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.0
    IL_0003:  conv.u
    IL_0004:  call       instance uint32 Pololu.WinusbHelper.WinUsbDevice::controlTransfer(valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET,
                                                                                           void*)
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  brfalse.s  IL_0028

    IL_000d:  ldstr      "The control transfer was expected to have no data "
    + "stage, but "
    IL_0012:  ldloc.0
    IL_0013:  box        [mscorlib]System.UInt32
    IL_0018:  ldstr      " bytes were transferred."
    IL_001d:  call       string [mscorlib]System.String::Concat(object,
                                                                object,
                                                                object)
    IL_0022:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0027:  throw

    IL_0028:  ret
  } // end of method WinUsbDevice::controlTransfer

  .method assembly hidebysig instance uint16 
          getProductID() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Pololu.WinusbHelper.WinUsbDevice::get_deviceInstance()
    IL_0006:  call       uint16 Pololu.WinusbHelper.Winusb::getProductID(int32)
    IL_000b:  ret
  } // end of method WinUsbDevice::getProductID

  .method assembly hidebysig instance uint16 
          getVendorID() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Pololu.WinusbHelper.WinUsbDevice::get_deviceInstance()
    IL_0006:  call       uint16 Pololu.WinusbHelper.Winusb::getVendorID(int32)
    IL_000b:  ret
  } // end of method WinUsbDevice::getVendorID

  .method family hidebysig instance uint32 
          controlTransfer(valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET setupPacket,
                          uint8[] buffer) cil managed
  {
    // Code size       136 (0x88)
    .maxstack  3
    .locals init (uint8& pinned V_0,
             uint32 V_1,
             object[] V_2)
    IL_0000:  ldarga.s   setupPacket
    IL_0002:  ldfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Length
    IL_0007:  brfalse.s  IL_0072

    IL_0009:  ldarg.2
    IL_000a:  brtrue.s   IL_001c

    IL_000c:  ldstr      "The setupPacket length field is non-zero, but no b"
    + "uffer was provided."
    IL_0011:  ldstr      "buffer"
    IL_0016:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string,
                                                                                 string)
    IL_001b:  throw

    IL_001c:  ldarg.2
    IL_001d:  ldlen
    IL_001e:  conv.i4
    IL_001f:  ldarga.s   setupPacket
    IL_0021:  ldfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Length
    IL_0026:  bge.s      IL_0072

    IL_0028:  ldc.i4.5
    IL_0029:  newarr     [mscorlib]System.Object
    IL_002e:  stloc.2
    IL_002f:  ldloc.2
    IL_0030:  ldc.i4.0
    IL_0031:  ldstr      "The setupPacket length field is "
    IL_0036:  stelem.ref
    IL_0037:  ldloc.2
    IL_0038:  ldc.i4.1
    IL_0039:  ldarga.s   setupPacket
    IL_003b:  ldfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Length
    IL_0040:  box        [mscorlib]System.UInt16
    IL_0045:  stelem.ref
    IL_0046:  ldloc.2
    IL_0047:  ldc.i4.2
    IL_0048:  ldstr      ", but the buffer provided is only "
    IL_004d:  stelem.ref
    IL_004e:  ldloc.2
    IL_004f:  ldc.i4.3
    IL_0050:  ldarg.2
    IL_0051:  ldlen
    IL_0052:  conv.i4
    IL_0053:  box        [mscorlib]System.Int32
    IL_0058:  stelem.ref
    IL_0059:  ldloc.2
    IL_005a:  ldc.i4.4
    IL_005b:  ldstr      " bytes."
    IL_0060:  stelem.ref
    IL_0061:  ldloc.2
    IL_0062:  call       string [mscorlib]System.String::Concat(object[])
    IL_0067:  ldstr      "buffer"
    IL_006c:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string,
                                                                                 string)
    IL_0071:  throw

    IL_0072:  ldarg.2
    IL_0073:  ldc.i4.0
    IL_0074:  ldelema    [mscorlib]System.Byte
    IL_0079:  stloc.0
    IL_007a:  ldarg.0
    IL_007b:  ldarg.1
    IL_007c:  ldloc.0
    IL_007d:  conv.i
    IL_007e:  call       instance uint32 Pololu.WinusbHelper.WinUsbDevice::controlTransfer(valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET,
                                                                                           void*)
    IL_0083:  stloc.1
    IL_0084:  leave.s    IL_0086

    IL_0086:  ldloc.1
    IL_0087:  ret
  } // end of method WinUsbDevice::controlTransfer

  .method family hidebysig instance uint32 
          controlTransfer(valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET setupPacket,
                          void* buffer) cil managed
  {
    // Code size       49 (0x31)
    .maxstack  6
    .locals init (uint32 V_0,
             bool V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldflda     valuetype Pololu.WinusbHelper.WinUsbDeviceHandles Pololu.WinusbHelper.WinUsbDevice::handles
    IL_0008:  ldfld      native int Pololu.WinusbHelper.WinUsbDeviceHandles::winusbHandle
    IL_000d:  ldarg.1
    IL_000e:  ldarg.2
    IL_000f:  ldarga.s   setupPacket
    IL_0011:  ldfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Length
    IL_0016:  ldloca.s   V_0
    IL_0018:  conv.u
    IL_0019:  ldc.i4.0
    IL_001a:  conv.i
    IL_001b:  call       bool Pololu.WinusbHelper.WinUsbDevice::WinUsb_ControlTransfer(native int,
                                                                                       valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET,
                                                                                       void*,
                                                                                       uint32,
                                                                                       uint32*,
                                                                                       valuetype Pololu.WinusbHelper.WinUsbDevice/OVERLAPPED*)
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  brtrue.s   IL_002f

    IL_0024:  ldstr      "Control transfer failed."
    IL_0029:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_002e:  throw

    IL_002f:  ldloc.0
    IL_0030:  ret
  } // end of method WinUsbDevice::controlTransfer

  .method private hidebysig static pinvokeimpl("winusb.dll" lasterr winapi) 
          bool  WinUsb_ControlTransfer(native int InterfaceHandle,
                                       valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET SetupPacket,
                                       void* Buffer,
                                       uint32 BufferLength,
                                       uint32* LengthTransferred,
                                       valuetype Pololu.WinusbHelper.WinUsbDevice/OVERLAPPED* Overlapped) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("winusb.dll" lasterr winapi) 
          bool  WinUsb_Free(native int InterfaceHandle) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("winusb.dll" lasterr winapi) 
          bool  WinUsb_ReadPipe(native int InterfaceHandle,
                                uint8 PipeID,
                                uint8& Buffer,
                                uint32 BufferLength,
                                uint32& LengthTransferred,
                                native int Overlapped) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("winusb.dll" lasterr winapi) 
          bool  WinUsb_SetPipePolicy(native int InterfaceHandle,
                                     uint8 PipeID,
                                     uint32 PolicyType,
                                     uint32 ValueLength,
                                     uint8& Value) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("winusb.dll" lasterr winapi) 
          bool  WinUsb_WritePipe(native int InterfaceHandle,
                                 uint8 PipeID,
                                 uint8& Buffer,
                                 uint32 BufferLength,
                                 uint32& LengthTransferred,
                                 native int Overlapped) cil managed preservesig
  {
  }
  .property instance int32 deviceInstance()
  {
    .get instance int32 Pololu.WinusbHelper.WinUsbDevice::get_deviceInstance()
  } // end of property WinUsbDevice::deviceInstance
} // end of class Pololu.WinusbHelper.WinUsbDevice

.class public abstract auto ansi sealed beforefieldinit Pololu.UsbWrapper.Usb
       extends [mscorlib]System.Object
{
  .field public static uint32 WM_DEVICECHANGE
  .method public hidebysig static class [mscorlib]System.Collections.Generic.IList`1<string> 
          getPortNames(string deviceInstanceIdPrefix) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [mscorlib]System.Collections.Generic.IList`1<string> Pololu.WinusbHelper.Winusb::getPortNames(string)
    IL_0006:  ret
  } // end of method Usb::getPortNames

  .method public hidebysig specialname static 
          bool  get_supportsNotify() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  ret
  } // end of method Usb::get_supportsNotify

  .method public hidebysig static native int 
          notificationRegister(valuetype [mscorlib]System.Guid guid,
                               native int handle) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       native int Pololu.WinusbHelper.Winusb::notificationRegister(valuetype [mscorlib]System.Guid,
                                                                                     native int)
    IL_0007:  ret
  } // end of method Usb::notificationRegister

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  ldc.i4     0x219
    IL_0005:  stsfld     uint32 Pololu.UsbWrapper.Usb::WM_DEVICECHANGE
    IL_000a:  ret
  } // end of method Usb::.cctor

  .property bool supportsNotify()
  {
    .get bool Pololu.UsbWrapper.Usb::get_supportsNotify()
  } // end of property Usb::supportsNotify
} // end of class Pololu.UsbWrapper.Usb

.class public abstract auto ansi beforefieldinit Pololu.UsbWrapper.UsbDevice
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .class auto ansi nested private beforefieldinit MyWinUsbDevice
         extends Pololu.WinusbHelper.WinUsbDevice
  {
    .field assembly initonly string serialNumber
    .method assembly hidebysig specialname rtspecialname 
            instance void  .ctor(valuetype Pololu.WinusbHelper.WinUsbDeviceHandles handles,
                                 string serialNumber) cil managed
    {
      // Code size       39 (0x27)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       instance void Pololu.WinusbHelper.WinUsbDevice::.ctor(valuetype Pololu.WinusbHelper.WinUsbDeviceHandles)
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      string Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice::serialNumber
      IL_000e:  ldarg.0
      IL_000f:  call       instance uint16 Pololu.WinusbHelper.WinUsbDevice::getVendorID()
      IL_0014:  ldc.i4     0x1ffb
      IL_0019:  beq.s      IL_0026

      IL_001b:  ldstr      "Error code 0x9876."
      IL_0020:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_0025:  throw

      IL_0026:  ret
    } // end of method MyWinUsbDevice::.ctor

    .method assembly hidebysig instance void 
            controlTransfer(uint8 RequestType,
                            uint8 Request,
                            uint16 Value,
                            uint16 Index) cil managed
    {
      // Code size       57 (0x39)
      .maxstack  2
      .locals init (valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET
      IL_0008:  ldloca.s   V_0
      IL_000a:  ldarg.1
      IL_000b:  stfld      uint8 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::RequestType
      IL_0010:  ldloca.s   V_0
      IL_0012:  ldarg.2
      IL_0013:  stfld      uint8 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Request
      IL_0018:  ldloca.s   V_0
      IL_001a:  ldarg.3
      IL_001b:  stfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Value
      IL_0020:  ldloca.s   V_0
      IL_0022:  ldarg.s    Index
      IL_0024:  stfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Index
      IL_0029:  ldloca.s   V_0
      IL_002b:  ldc.i4.0
      IL_002c:  stfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Length
      IL_0031:  ldarg.0
      IL_0032:  ldloc.0
      IL_0033:  call       instance void Pololu.WinusbHelper.WinUsbDevice::controlTransfer(valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET)
      IL_0038:  ret
    } // end of method MyWinUsbDevice::controlTransfer

    .method assembly hidebysig instance uint32 
            controlTransfer(uint8 RequestType,
                            uint8 Request,
                            uint16 Value,
                            uint16 Index,
                            uint8[] data) cil managed
    {
      // Code size       63 (0x3f)
      .maxstack  3
      .locals init (valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET
      IL_0008:  ldloca.s   V_0
      IL_000a:  ldarg.1
      IL_000b:  stfld      uint8 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::RequestType
      IL_0010:  ldloca.s   V_0
      IL_0012:  ldarg.2
      IL_0013:  stfld      uint8 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Request
      IL_0018:  ldloca.s   V_0
      IL_001a:  ldarg.3
      IL_001b:  stfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Value
      IL_0020:  ldloca.s   V_0
      IL_0022:  ldarg.s    Index
      IL_0024:  stfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Index
      IL_0029:  ldloca.s   V_0
      IL_002b:  ldarg.s    data
      IL_002d:  ldlen
      IL_002e:  conv.i4
      IL_002f:  conv.u2
      IL_0030:  stfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Length
      IL_0035:  ldarg.0
      IL_0036:  ldloc.0
      IL_0037:  ldarg.s    data
      IL_0039:  call       instance uint32 Pololu.WinusbHelper.WinUsbDevice::controlTransfer(valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET,
                                                                                             uint8[])
      IL_003e:  ret
    } // end of method MyWinUsbDevice::controlTransfer

    .method assembly hidebysig instance uint32 
            controlTransfer(uint8 RequestType,
                            uint8 Request,
                            uint16 Value,
                            uint16 Index,
                            void* data,
                            uint16 Length) cil managed
    {
      // Code size       60 (0x3c)
      .maxstack  3
      .locals init (valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET
      IL_0008:  ldloca.s   V_0
      IL_000a:  ldarg.1
      IL_000b:  stfld      uint8 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::RequestType
      IL_0010:  ldloca.s   V_0
      IL_0012:  ldarg.2
      IL_0013:  stfld      uint8 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Request
      IL_0018:  ldloca.s   V_0
      IL_001a:  ldarg.3
      IL_001b:  stfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Value
      IL_0020:  ldloca.s   V_0
      IL_0022:  ldarg.s    Index
      IL_0024:  stfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Index
      IL_0029:  ldloca.s   V_0
      IL_002b:  ldarg.s    Length
      IL_002d:  stfld      uint16 Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET::Length
      IL_0032:  ldarg.0
      IL_0033:  ldloc.0
      IL_0034:  ldarg.s    data
      IL_0036:  call       instance uint32 Pololu.WinusbHelper.WinUsbDevice::controlTransfer(valuetype Pololu.WinusbHelper.WinUsbDevice/WINUSB_SETUP_PACKET,
                                                                                             void*)
      IL_003b:  ret
    } // end of method MyWinUsbDevice::controlTransfer

  } // end of class MyWinUsbDevice

  .field private class Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice device
  .method public hidebysig instance string 
          getSerialNumber() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice Pololu.UsbWrapper.UsbDevice::device
    IL_0006:  ldfld      string Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice::serialNumber
    IL_000b:  ret
  } // end of method UsbDevice::getSerialNumber

  .method family hidebysig instance uint16 
          getProductID() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice Pololu.UsbWrapper.UsbDevice::device
    IL_0006:  callvirt   instance uint16 Pololu.WinusbHelper.WinUsbDevice::getProductID()
    IL_000b:  ret
  } // end of method UsbDevice::getProductID

  .method family hidebysig instance void 
          controlTransfer(uint8 RequestType,
                          uint8 Request,
                          uint16 Value,
                          uint16 Index) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice Pololu.UsbWrapper.UsbDevice::device
    IL_0006:  ldarg.1
    IL_0007:  ldarg.2
    IL_0008:  ldarg.3
    IL_0009:  ldarg.s    Index
    IL_000b:  callvirt   instance void Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice::controlTransfer(uint8,
                                                                                                   uint8,
                                                                                                   uint16,
                                                                                                   uint16)
    IL_0010:  ret
  } // end of method UsbDevice::controlTransfer

  .method family hidebysig instance uint32 
          controlTransfer(uint8 RequestType,
                          uint8 Request,
                          uint16 Value,
                          uint16 Index,
                          uint8[] data) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice Pololu.UsbWrapper.UsbDevice::device
    IL_0006:  ldarg.1
    IL_0007:  ldarg.2
    IL_0008:  ldarg.3
    IL_0009:  ldarg.s    Index
    IL_000b:  ldarg.s    data
    IL_000d:  callvirt   instance uint32 Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice::controlTransfer(uint8,
                                                                                                     uint8,
                                                                                                     uint16,
                                                                                                     uint16,
                                                                                                     uint8[])
    IL_0012:  ret
  } // end of method UsbDevice::controlTransfer

  .method family hidebysig instance uint32 
          controlTransfer(uint8 RequestType,
                          uint8 Request,
                          uint16 Value,
                          uint16 Index,
                          void* data,
                          uint16 Length) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice Pololu.UsbWrapper.UsbDevice::device
    IL_0006:  ldarg.1
    IL_0007:  ldarg.2
    IL_0008:  ldarg.3
    IL_0009:  ldarg.s    Index
    IL_000b:  ldarg.s    data
    IL_000d:  ldarg.s    Length
    IL_000f:  callvirt   instance uint32 Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice::controlTransfer(uint8,
                                                                                                     uint8,
                                                                                                     uint16,
                                                                                                     uint16,
                                                                                                     void*,
                                                                                                     uint16)
    IL_0014:  ret
  } // end of method UsbDevice::controlTransfer

  .method assembly hidebysig specialname 
          instance int32  get_deviceInstance() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice Pololu.UsbWrapper.UsbDevice::device
    IL_0006:  callvirt   instance int32 Pololu.WinusbHelper.WinUsbDevice::get_deviceInstance()
    IL_000b:  ret
  } // end of method UsbDevice::get_deviceInstance

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor(class Pololu.UsbWrapper.DeviceListItem deviceListItem) cil managed
  {
    // Code size       91 (0x5b)
    .maxstack  4
    .locals init (valuetype Pololu.WinusbHelper.WinUsbDeviceHandles V_0,
             class [mscorlib]System.Exception V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    .try
    {
      IL_0006:  ldarg.1
      IL_0007:  ldfld      valuetype [mscorlib]System.Guid Pololu.UsbWrapper.DeviceListItem::guid
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 Pololu.UsbWrapper.DeviceListItem::deviceInstance
      IL_0012:  call       valuetype Pololu.WinusbHelper.WinUsbDeviceHandles Pololu.WinusbHelper.Winusb::connect(valuetype [mscorlib]System.Guid,
                                                                                                                 int32)
      IL_0017:  stloc.0
      IL_0018:  leave.s    IL_0048

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_001a:  stloc.1
      IL_001b:  ldloc.1
      IL_001c:  isinst     [System]System.ComponentModel.Win32Exception
      IL_0021:  brfalse.s  IL_003c

      IL_0023:  ldloc.1
      IL_0024:  castclass  [System]System.ComponentModel.Win32Exception
      IL_0029:  callvirt   instance int32 [System]System.ComponentModel.Win32Exception::get_NativeErrorCode()
      IL_002e:  ldc.i4.5
      IL_002f:  bne.un.s   IL_003c

      IL_0031:  ldstr      "Access was denied when trying to connect to the de"
      + "vice.  Try closing all programs using the device."
      IL_0036:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_003b:  throw

      IL_003c:  ldstr      "There was an error connecting to the device."
      IL_0041:  ldloc.1
      IL_0042:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0047:  throw

    }  // end handler
    IL_0048:  ldarg.0
    IL_0049:  ldloc.0
    IL_004a:  ldarg.1
    IL_004b:  ldfld      string Pololu.UsbWrapper.DeviceListItem::serialNumber
    IL_0050:  newobj     instance void Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice::.ctor(valuetype Pololu.WinusbHelper.WinUsbDeviceHandles,
                                                                                         string)
    IL_0055:  stfld      class Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice Pololu.UsbWrapper.UsbDevice::device
    IL_005a:  ret
  } // end of method UsbDevice::.ctor

  .method public hidebysig instance void 
          disconnect() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Pololu.UsbWrapper.UsbDevice/MyWinUsbDevice Pololu.UsbWrapper.UsbDevice::device
    IL_0006:  callvirt   instance void Pololu.WinusbHelper.WinUsbDevice::disconnect()
    IL_000b:  ret
  } // end of method UsbDevice::disconnect

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Pololu.UsbWrapper.UsbDevice::disconnect()
    IL_0006:  ret
  } // end of method UsbDevice::Dispose

  .method family hidebysig static class [mscorlib]System.Collections.Generic.List`1<class Pololu.UsbWrapper.DeviceListItem> 
          getDeviceList(valuetype [mscorlib]System.Guid deviceInterfaceGuid) cil managed
  {
    // Code size       109 (0x6d)
    .maxstack  6
    .locals init (native int V_0,
             uint32 V_1,
             class [mscorlib]System.Collections.Generic.List`1<class Pololu.UsbWrapper.DeviceListItem> V_2,
             uint8 V_3,
             int32 V_4,
             string V_5,
             uint16 V_6,
             class Pololu.UsbWrapper.DeviceListItem V_7,
             class [mscorlib]System.Collections.Generic.List`1<class Pololu.UsbWrapper.DeviceListItem> V_8)
    IL_0000:  ldarg.0
    IL_0001:  call       native int Pololu.WinusbHelper.Winusb::listCreate(valuetype [mscorlib]System.Guid)
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  ldloc.0
      IL_0008:  call       uint32 Pololu.WinusbHelper.Winusb::listSize(native int)
      IL_000d:  stloc.1
      IL_000e:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.UsbWrapper.DeviceListItem>::.ctor()
      IL_0013:  stloc.2
      IL_0014:  ldc.i4.0
      IL_0015:  stloc.3
      IL_0016:  br.s       IL_005a

      IL_0018:  ldloc.0
      IL_0019:  ldloc.3
      IL_001a:  call       int32 Pololu.WinusbHelper.Winusb::listGetDeviceInstance(native int,
                                                                                   uint32)
      IL_001f:  stloc.s    V_4
      IL_0021:  ldloc.s    V_4
      IL_0023:  call       string Pololu.WinusbHelper.Winusb::getSerialNumber(int32)
      IL_0028:  stloc.s    V_5
      IL_002a:  ldloc.s    V_4
      IL_002c:  call       uint16 Pololu.WinusbHelper.Winusb::getProductID(int32)
      IL_0031:  stloc.s    V_6
      IL_0033:  ldloc.s    V_4
      IL_0035:  ldarg.0
      IL_0036:  ldstr      "#"
      IL_003b:  ldloc.s    V_5
      IL_003d:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0042:  ldloc.s    V_5
      IL_0044:  ldloc.s    V_6
      IL_0046:  newobj     instance void Pololu.UsbWrapper.DeviceListItem::.ctor(int32,
                                                                                 valuetype [mscorlib]System.Guid,
                                                                                 string,
                                                                                 string,
                                                                                 uint16)
      IL_004b:  stloc.s    V_7
      IL_004d:  ldloc.2
      IL_004e:  ldloc.s    V_7
      IL_0050:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.UsbWrapper.DeviceListItem>::Add(!0)
      IL_0055:  ldloc.3
      IL_0056:  ldc.i4.1
      IL_0057:  add
      IL_0058:  conv.u1
      IL_0059:  stloc.3
      IL_005a:  ldloc.3
      IL_005b:  ldloc.1
      IL_005c:  blt.un.s   IL_0018

      IL_005e:  ldloc.2
      IL_005f:  stloc.s    V_8
      IL_0061:  leave.s    IL_006a

    }  // end .try
    finally
    {
      IL_0063:  ldloc.0
      IL_0064:  call       void Pololu.WinusbHelper.Winusb::listDestroy(native int)
      IL_0069:  endfinally
    }  // end handler
    IL_006a:  ldloc.s    V_8
    IL_006c:  ret
  } // end of method UsbDevice::getDeviceList

  .method family hidebysig static class [mscorlib]System.Collections.Generic.List`1<class Pololu.UsbWrapper.DeviceListItem> 
          getDeviceList(uint32 vendorId,
                        uint16[] productIds) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method UsbDevice::getDeviceList

  .method public hidebysig instance bool 
          isSameDeviceAs(class Pololu.UsbWrapper.DeviceListItem item) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Pololu.UsbWrapper.UsbDevice::get_deviceInstance()
    IL_0006:  ldarg.1
    IL_0007:  ldfld      int32 Pololu.UsbWrapper.DeviceListItem::deviceInstance
    IL_000c:  ceq
    IL_000e:  ret
  } // end of method UsbDevice::isSameDeviceAs

  .property instance int32 deviceInstance()
  {
    .get instance int32 Pololu.UsbWrapper.UsbDevice::get_deviceInstance()
  } // end of property UsbDevice::deviceInstance
} // end of class Pololu.UsbWrapper.UsbDevice

.class private sequential ansi sealed beforefieldinit Pololu.WinusbHelper.WinUsbDeviceHandles
       extends [mscorlib]System.ValueType
{
  .field assembly native int winusbHandle
  .field assembly class [mscorlib]Microsoft.Win32.SafeHandles.SafeFileHandle deviceHandle
  .field assembly int32 deviceInstance
} // end of class Pololu.WinusbHelper.WinUsbDeviceHandles

.class private abstract auto ansi sealed beforefieldinit Pololu.WinusbHelper.Winusb
       extends [mscorlib]System.Object
{
  .class sequential ansi sealed nested private beforefieldinit DEV_BROADCAST_DEVICEINTERFACE
         extends [mscorlib]System.ValueType
  {
    .field assembly uint32 dbcc_size
    .field assembly uint32 dbcc_devicetype
    .field assembly uint32 dbcc_reserved
    .field assembly valuetype [mscorlib]System.Guid dbcc_classguid
    .field assembly int16 dbcc_name
  } // end of class DEV_BROADCAST_DEVICEINTERFACE

  .class sequential ansi sealed nested private beforefieldinit SP_DEVINFO_DATA
         extends [mscorlib]System.ValueType
  {
    .field assembly uint32 cbSize
    .field assembly valuetype [mscorlib]System.Guid ClassGuid
    .field assembly int32 DevInst
    .field assembly native int Reserved
  } // end of class SP_DEVINFO_DATA

  .class sequential ansi sealed nested private beforefieldinit SP_DEVICE_INTERFACE_DATA
         extends [mscorlib]System.ValueType
  {
    .field assembly uint32 cbSize
    .field assembly valuetype [mscorlib]System.Guid InterfaceClassGuid
    .field assembly uint32 Flags
    .field assembly native int Reserved
  } // end of class SP_DEVICE_INTERFACE_DATA

  .class sequential ansi sealed nested private beforefieldinit SP_DEVICE_INTERFACE_DETAIL_DATA
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 0
    .field assembly uint32 cbSize
    .field assembly uint16 character
  } // end of class SP_DEVICE_INTERFACE_DETAIL_DATA

  .field private static literal uint32 SPDRP_DEVICEDESC = uint32(0x00000000)
  .field static assembly literal uint32 WM_DEVICECHANGE = uint32(0x00000219)
  .field private static literal uint32 DEVICE_NOTIFY_WINDOW_HANDLE = uint32(0x00000000)
  .field private static literal uint32 DBT_DEVTYP_DEVICEINTERFACE = uint32(0x00000005)
  .field private static literal uint16 FILE_ATTRIBUTE_NORMAL = uint16(0x0080)
  .field private static literal uint32 FILE_FLAG_OVERLAPPED = uint32(0x40000000)
  .field private static literal uint16 FILE_SHARE_READ = uint16(0x0001)
  .field private static literal uint16 FILE_SHARE_WRITE = uint16(0x0002)
  .field private static literal uint32 GENERIC_READ = uint32(0x80000000)
  .field private static literal uint32 GENERIC_WRITE = uint32(0x40000000)
  .field private static literal uint16 OPEN_EXISTING = uint16(0x0003)
  .field private static literal uint32 CR_SUCCESS = uint32(0x00000000)
  .field private static literal uint16 DIGCF_PRESENT = uint16(0x0002)
  .field private static literal uint16 DIGCF_DEVICEINTERFACE = uint16(0x0010)
  .field private static literal uint32 ERROR_INSUFFICIENT_BUFFER = uint32(0x0000007A)
  .field private static literal uint32 ERROR_NO_MORE_ITEMS = uint32(0x00000103)
  .field private static literal uint32 PIPE_TRANSFER_TIMEOUT = uint32(0x00000003)
  .field private static valuetype [mscorlib]System.Guid GUID_DEVCLASS_PORTS
  .method assembly hidebysig static valuetype Pololu.WinusbHelper.WinUsbDeviceHandles 
          connect(valuetype [mscorlib]System.Guid deviceInterfaceGuid) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  3
    .locals init (native int V_0,
             valuetype Pololu.WinusbHelper.WinUsbDeviceHandles V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       native int Pololu.WinusbHelper.Winusb::listCreate(valuetype [mscorlib]System.Guid)
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.0
      IL_0009:  ldarg.0
      IL_000a:  call       valuetype Pololu.WinusbHelper.WinUsbDeviceHandles Pololu.WinusbHelper.Winusb::listConnect(native int,
                                                                                                                     uint8,
                                                                                                                     valuetype [mscorlib]System.Guid)
      IL_000f:  stloc.1
      IL_0010:  leave.s    IL_0019

    }  // end .try
    finally
    {
      IL_0012:  ldloc.0
      IL_0013:  call       void Pololu.WinusbHelper.Winusb::listDestroy(native int)
      IL_0018:  endfinally
    }  // end handler
    IL_0019:  ldloc.1
    IL_001a:  ret
  } // end of method Winusb::connect

  .method assembly hidebysig static native int 
          listCreate(valuetype [mscorlib]System.Guid deviceInterfaceGuid) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  4
    .locals init (native int V_0)
    IL_0000:  ldarga.s   deviceInterfaceGuid
    IL_0002:  ldnull
    IL_0003:  ldsfld     native int [mscorlib]System.IntPtr::Zero
    IL_0008:  ldc.i4.s   18
    IL_000a:  call       native int Pololu.WinusbHelper.Winusb::SetupDiGetClassDevs(valuetype [mscorlib]System.Guid&,
                                                                                    string,
                                                                                    native int,
                                                                                    uint32)
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.m1
    IL_0012:  newobj     instance void [mscorlib]System.IntPtr::.ctor(int32)
    IL_0017:  call       bool [mscorlib]System.IntPtr::op_Equality(native int,
                                                                   native int)
    IL_001c:  brfalse.s  IL_0029

    IL_001e:  ldstr      "Unable to create a list of connected devices."
    IL_0023:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_0028:  throw

    IL_0029:  ldloc.0
    IL_002a:  ret
  } // end of method Winusb::listCreate

  .method assembly hidebysig static uint32 
          listSize(native int listHandle) cil managed
  {
    // Code size       103 (0x67)
    .maxstack  3
    .locals init (uint32 V_0,
             valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA V_1,
             bool V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_005d

    IL_0006:  ldloca.s   V_1
    IL_0008:  initobj    Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA
    IL_000e:  ldloca.s   V_1
    IL_0010:  ldloc.1
    IL_0011:  box        Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA
    IL_0016:  call       int32 [mscorlib]System.Runtime.InteropServices.Marshal::SizeOf(object)
    IL_001b:  stfld      uint32 Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA::cbSize
    IL_0020:  ldarg.0
    IL_0021:  ldloc.0
    IL_0022:  ldloca.s   V_1
    IL_0024:  call       bool Pololu.WinusbHelper.Winusb::SetupDiEnumDeviceInfo(native int,
                                                                                uint32,
                                                                                valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA&)
    IL_0029:  stloc.2
    IL_002a:  ldloc.2
    IL_002b:  brtrue.s   IL_0059

    IL_002d:  call       int32 [mscorlib]System.Runtime.InteropServices.Marshal::GetLastWin32Error()
    IL_0032:  conv.i8
    IL_0033:  ldc.i4     0x103
    IL_0038:  conv.i8
    IL_0039:  beq.s      IL_0057

    IL_003b:  ldstr      "Unable to access device number "
    IL_0040:  ldloca.s   V_0
    IL_0042:  call       instance string [mscorlib]System.UInt32::ToString()
    IL_0047:  ldstr      "."
    IL_004c:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0051:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_0056:  throw

    IL_0057:  ldloc.0
    IL_0058:  ret

    IL_0059:  ldloc.0
    IL_005a:  ldc.i4.1
    IL_005b:  add
    IL_005c:  stloc.0
    IL_005d:  ldloc.0
    IL_005e:  ldc.i4     0xff
    IL_0063:  blt.un.s   IL_0006

    IL_0065:  ldloc.0
    IL_0066:  ret
  } // end of method Winusb::listSize

  .method assembly hidebysig static string 
          listGetName(native int deviceListHandle,
                      uint8 index) cil managed
  {
    // Code size       82 (0x52)
    .maxstack  7
    .locals init (valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA V_0,
             uint8[] V_1,
             uint8& pinned V_2,
             bool V_3,
             string V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA Pololu.WinusbHelper.Winusb::listGetDevinfoData(native int,
                                                                                                                             uint32)
    IL_0007:  stloc.0
    IL_0008:  ldc.i4     0x100
    IL_000d:  newarr     [mscorlib]System.Byte
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  ldc.i4.0
    IL_0015:  ldelema    [mscorlib]System.Byte
    IL_001a:  stloc.2
    IL_001b:  ldarg.0
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldc.i4.0
    IL_001f:  ldsfld     native int [mscorlib]System.IntPtr::Zero
    IL_0024:  ldloc.2
    IL_0025:  conv.i
    IL_0026:  ldc.i4     0x100
    IL_002b:  ldsfld     native int [mscorlib]System.IntPtr::Zero
    IL_0030:  call       bool Pololu.WinusbHelper.Winusb::SetupDiGetDeviceRegistryProperty(native int,
                                                                                           valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA&,
                                                                                           uint32,
                                                                                           native int,
                                                                                           uint8*,
                                                                                           uint32,
                                                                                           native int)
    IL_0035:  stloc.3
    IL_0036:  ldloc.3
    IL_0037:  brtrue.s   IL_0044

    IL_0039:  ldstr      "There was an error getting the name."
    IL_003e:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_0043:  throw

    IL_0044:  ldloc.2
    IL_0045:  conv.i
    IL_0046:  newobj     instance void [mscorlib]System.String::.ctor(char*)
    IL_004b:  stloc.s    V_4
    IL_004d:  leave.s    IL_004f

    IL_004f:  ldloc.s    V_4
    IL_0051:  ret
  } // end of method Winusb::listGetName

  .method assembly hidebysig static string 
          listGetSerialNumber(native int listHandle,
                              uint32 index) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       int32 Pololu.WinusbHelper.Winusb::listGetDeviceInstance(native int,
                                                                                 uint32)
    IL_0007:  call       string Pololu.WinusbHelper.Winusb::getSerialNumber(int32)
    IL_000c:  ret
  } // end of method Winusb::listGetSerialNumber

  .method private hidebysig static valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA 
          listGetDevinfoData(native int listHandle,
                             uint32 index) cil managed
  {
    // Code size       69 (0x45)
    .maxstack  3
    .locals init (valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA V_0,
             bool V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA
    IL_0008:  ldloca.s   V_0
    IL_000a:  ldloc.0
    IL_000b:  box        Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA
    IL_0010:  call       int32 [mscorlib]System.Runtime.InteropServices.Marshal::SizeOf(object)
    IL_0015:  stfld      uint32 Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA::cbSize
    IL_001a:  ldarg.0
    IL_001b:  ldarg.1
    IL_001c:  ldloca.s   V_0
    IL_001e:  call       bool Pololu.WinusbHelper.Winusb::SetupDiEnumDeviceInfo(native int,
                                                                                uint32,
                                                                                valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA&)
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  brtrue.s   IL_0043

    IL_0027:  ldstr      "Unable to access device info data for device number "
    IL_002c:  ldarga.s   index
    IL_002e:  call       instance string [mscorlib]System.UInt32::ToString()
    IL_0033:  ldstr      "."
    IL_0038:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_003d:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_0042:  throw

    IL_0043:  ldloc.0
    IL_0044:  ret
  } // end of method Winusb::listGetDevinfoData

  .method assembly hidebysig static int32 
          listGetDeviceInstance(native int listHandle,
                                uint32 index) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA Pololu.WinusbHelper.Winusb::listGetDevinfoData(native int,
                                                                                                                             uint32)
    IL_0007:  ldfld      int32 Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA::DevInst
    IL_000c:  ret
  } // end of method Winusb::listGetDeviceInstance

  .method assembly hidebysig static string 
          getSerialNumber(int32 deviceInstance) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  2
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       string Pololu.WinusbHelper.Winusb::getDeviceInstanceId(int32)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldstr      "&MI_"
    IL_000d:  callvirt   instance bool [mscorlib]System.String::Contains(string)
    IL_0012:  brfalse.s  IL_0022

    IL_0014:  ldarg.0
    IL_0015:  call       int32 Pololu.WinusbHelper.Winusb::getParentDeviceInstance(int32)
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  call       string Pololu.WinusbHelper.Winusb::getDeviceInstanceId(int32)
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  ldc.i4.s   22
    IL_0025:  callvirt   instance string [mscorlib]System.String::Substring(int32)
    IL_002a:  ret
  } // end of method Winusb::getSerialNumber

  .method assembly hidebysig static uint16 
          getProductID(int32 deviceInstance) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  3
    .locals init (string V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       string Pololu.WinusbHelper.Winusb::getDeviceInstanceId(int32)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s   17
    IL_000a:  ldc.i4.4
    IL_000b:  callvirt   instance string [mscorlib]System.String::Substring(int32,
                                                                            int32)
    IL_0010:  ldc.i4     0x203
    IL_0015:  call       uint16 [mscorlib]System.UInt16::Parse(string,
                                                               valuetype [mscorlib]System.Globalization.NumberStyles)
    IL_001a:  ret
  } // end of method Winusb::getProductID

  .method assembly hidebysig static uint16 
          listGetProductId(native int deviceListHandle,
                           uint32 index) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       int32 Pololu.WinusbHelper.Winusb::listGetDeviceInstance(native int,
                                                                                 uint32)
    IL_0007:  call       uint16 Pololu.WinusbHelper.Winusb::getProductID(int32)
    IL_000c:  ret
  } // end of method Winusb::listGetProductId

  .method assembly hidebysig static uint16 
          getVendorID(int32 deviceInstance) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  3
    .locals init (string V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       string Pololu.WinusbHelper.Winusb::getDeviceInstanceId(int32)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.8
    IL_0009:  ldc.i4.4
    IL_000a:  callvirt   instance string [mscorlib]System.String::Substring(int32,
                                                                            int32)
    IL_000f:  ldc.i4     0x203
    IL_0014:  call       uint16 [mscorlib]System.UInt16::Parse(string,
                                                               valuetype [mscorlib]System.Globalization.NumberStyles)
    IL_0019:  ret
  } // end of method Winusb::getVendorID

  .method private hidebysig static string 
          getDeviceInstanceId(int32 deviceInstance) cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  4
    .locals init (uint32 V_0,
             native int V_1,
             string V_2)
    IL_0000:  ldc.i4     0xc8
    IL_0005:  call       native int [mscorlib]System.Runtime.InteropServices.Marshal::AllocHGlobal(int32)
    IL_000a:  stloc.1
    .try
    {
      IL_000b:  ldloc.1
      IL_000c:  ldsfld     native int [mscorlib]System.IntPtr::Zero
      IL_0011:  call       bool [mscorlib]System.IntPtr::op_Equality(native int,
                                                                     native int)
      IL_0016:  brfalse.s  IL_0023

      IL_0018:  ldstr      "Unable to allocate memory for the parent device's "
      + "device instance ID string."
      IL_001d:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
      IL_0022:  throw

      IL_0023:  ldarg.0
      IL_0024:  ldloc.1
      IL_0025:  ldc.i4.s   100
      IL_0027:  ldc.i4.0
      IL_0028:  call       uint32 Pololu.WinusbHelper.Winusb::CM_Get_Device_IDW(int32,
                                                                                native int,
                                                                                uint32,
                                                                                uint32)
      IL_002d:  stloc.0
      IL_002e:  ldloc.0
      IL_002f:  brfalse.s  IL_003d

      IL_0031:  ldloc.0
      IL_0032:  ldstr      "Unable to get the device instance id."
      IL_0037:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(int32,
                                                                                             string)
      IL_003c:  throw

      IL_003d:  ldloc.1
      IL_003e:  call       string [mscorlib]System.Runtime.InteropServices.Marshal::PtrToStringUni(native int)
      IL_0043:  stloc.2
      IL_0044:  leave.s    IL_004d

    }  // end .try
    finally
    {
      IL_0046:  ldloc.1
      IL_0047:  call       void [mscorlib]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
      IL_004c:  endfinally
    }  // end handler
    IL_004d:  ldloc.2
    IL_004e:  ret
  } // end of method Winusb::getDeviceInstanceId

  .method private hidebysig static int32 
          getParentDeviceInstance(int32 deviceInstance) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  3
    .locals init (uint32 V_0,
             int32 V_1)
    IL_0000:  ldloca.s   V_1
    IL_0002:  ldarg.0
    IL_0003:  ldc.i4.0
    IL_0004:  call       uint32 Pololu.WinusbHelper.Winusb::CM_Get_Parent(int32&,
                                                                          int32,
                                                                          uint32)
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  brfalse.s  IL_0019

    IL_000d:  ldloc.0
    IL_000e:  ldstr      "Unable to get the parent of the device."
    IL_0013:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(int32,
                                                                                           string)
    IL_0018:  throw

    IL_0019:  ldloc.1
    IL_001a:  ret
  } // end of method Winusb::getParentDeviceInstance

  .method assembly hidebysig static void 
          listDestroy(native int listHandle) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       int32 Pololu.WinusbHelper.Winusb::SetupDiDestroyDeviceInfoList(native int)
    IL_0006:  pop
    IL_0007:  ret
  } // end of method Winusb::listDestroy

  .method assembly hidebysig static valuetype Pololu.WinusbHelper.WinUsbDeviceHandles 
          connect(valuetype [mscorlib]System.Guid deviceInterfaceGuid,
                  int32 deviceInstance) cil managed
  {
    // Code size       82 (0x52)
    .maxstack  3
    .locals init (native int V_0,
             uint32 V_1,
             uint8 V_2,
             valuetype Pololu.WinusbHelper.WinUsbDeviceHandles V_3)
    IL_0000:  ldarg.0
    IL_0001:  call       native int Pololu.WinusbHelper.Winusb::listCreate(valuetype [mscorlib]System.Guid)
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  ldloc.0
      IL_0008:  call       uint32 Pololu.WinusbHelper.Winusb::listSize(native int)
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  brtrue.s   IL_001c

      IL_0011:  ldstr      "No devices were detected."
      IL_0016:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_001b:  throw

      IL_001c:  ldc.i4.0
      IL_001d:  stloc.2
      IL_001e:  br.s       IL_003a

      IL_0020:  ldarg.1
      IL_0021:  ldloc.0
      IL_0022:  ldloc.2
      IL_0023:  call       int32 Pololu.WinusbHelper.Winusb::listGetDeviceInstance(native int,
                                                                                   uint32)
      IL_0028:  bne.un.s   IL_0035

      IL_002a:  ldloc.0
      IL_002b:  ldloc.2
      IL_002c:  ldarg.0
      IL_002d:  call       valuetype Pololu.WinusbHelper.WinUsbDeviceHandles Pololu.WinusbHelper.Winusb::listConnect(native int,
                                                                                                                     uint8,
                                                                                                                     valuetype [mscorlib]System.Guid)
      IL_0032:  stloc.3
      IL_0033:  leave.s    IL_0050

      IL_0035:  ldloc.2
      IL_0036:  ldc.i4.1
      IL_0037:  add
      IL_0038:  conv.u1
      IL_0039:  stloc.2
      IL_003a:  ldloc.2
      IL_003b:  ldloc.1
      IL_003c:  blt.un.s   IL_0020

      IL_003e:  ldstr      "None of the connected devices have the device inst"
      + "ance id expected."
      IL_0043:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_0048:  throw

    }  // end .try
    finally
    {
      IL_0049:  ldloc.0
      IL_004a:  call       void Pololu.WinusbHelper.Winusb::listDestroy(native int)
      IL_004f:  endfinally
    }  // end handler
    IL_0050:  ldloc.3
    IL_0051:  ret
  } // end of method Winusb::connect

  .method assembly hidebysig static valuetype Pololu.WinusbHelper.WinUsbDeviceHandles 
          listConnect(native int listHandle,
                      uint8 index,
                      valuetype [mscorlib]System.Guid deviceInterfaceGuid) cil managed
  {
    // Code size       426 (0x1aa)
    .maxstack  8
    .locals init (valuetype Pololu.WinusbHelper.WinUsbDeviceHandles V_0,
             valuetype Pololu.WinusbHelper.Winusb/SP_DEVICE_INTERFACE_DATA V_1,
             bool V_2,
             uint32 V_3,
             native int V_4,
             bool V_5,
             native int V_6,
             string V_7,
             int32 V_8,
             uint32 V_9,
             int32 V_10)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    Pololu.WinusbHelper.WinUsbDeviceHandles
    IL_0008:  ldloca.s   V_0
    IL_000a:  ldarg.0
    IL_000b:  ldarg.1
    IL_000c:  call       int32 Pololu.WinusbHelper.Winusb::listGetDeviceInstance(native int,
                                                                                 uint32)
    IL_0011:  stfld      int32 Pololu.WinusbHelper.WinUsbDeviceHandles::deviceInstance
    IL_0016:  ldloca.s   V_1
    IL_0018:  initobj    Pololu.WinusbHelper.Winusb/SP_DEVICE_INTERFACE_DATA
    IL_001e:  ldloca.s   V_1
    IL_0020:  ldloc.1
    IL_0021:  box        Pololu.WinusbHelper.Winusb/SP_DEVICE_INTERFACE_DATA
    IL_0026:  call       int32 [mscorlib]System.Runtime.InteropServices.Marshal::SizeOf(object)
    IL_002b:  stfld      uint32 Pololu.WinusbHelper.Winusb/SP_DEVICE_INTERFACE_DATA::cbSize
    IL_0030:  ldarg.0
    IL_0031:  ldsfld     native int [mscorlib]System.IntPtr::Zero
    IL_0036:  ldarga.s   deviceInterfaceGuid
    IL_0038:  ldarg.1
    IL_0039:  ldloca.s   V_1
    IL_003b:  call       bool Pololu.WinusbHelper.Winusb::SetupDiEnumDeviceInterfaces(native int,
                                                                                      native int,
                                                                                      valuetype [mscorlib]System.Guid&,
                                                                                      uint32,
                                                                                      valuetype Pololu.WinusbHelper.Winusb/SP_DEVICE_INTERFACE_DATA&)
    IL_0040:  stloc.2
    IL_0041:  ldloc.2
    IL_0042:  brtrue.s   IL_004f

    IL_0044:  ldstr      "Unable to get the device interface data."
    IL_0049:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_004e:  throw

    IL_004f:  ldc.i4.0
    IL_0050:  stloc.3
    IL_0051:  ldarg.0
    IL_0052:  ldloca.s   V_1
    IL_0054:  ldsfld     native int [mscorlib]System.IntPtr::Zero
    IL_0059:  ldc.i4.0
    IL_005a:  ldloca.s   V_3
    IL_005c:  ldsfld     native int [mscorlib]System.IntPtr::Zero
    IL_0061:  call       bool Pololu.WinusbHelper.Winusb::SetupDiGetDeviceInterfaceDetail(native int,
                                                                                          valuetype Pololu.WinusbHelper.Winusb/SP_DEVICE_INTERFACE_DATA&,
                                                                                          native int,
                                                                                          uint32,
                                                                                          uint32&,
                                                                                          native int)
    IL_0066:  stloc.2
    IL_0067:  ldloc.2
    IL_0068:  brtrue.s   IL_0080

    IL_006a:  call       int32 [mscorlib]System.Runtime.InteropServices.Marshal::GetLastWin32Error()
    IL_006f:  conv.i8
    IL_0070:  ldc.i4.s   122
    IL_0072:  conv.i8
    IL_0073:  beq.s      IL_0080

    IL_0075:  ldstr      "Unable to get the size of the device interface det"
    + "ail."
    IL_007a:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_007f:  throw

    IL_0080:  ldloc.3
    IL_0081:  call       native int [mscorlib]System.Runtime.InteropServices.Marshal::AllocHGlobal(int32)
    IL_0086:  stloc.s    V_4
    IL_0088:  sizeof     [mscorlib]System.IntPtr
    IL_008e:  ldc.i4.8
    IL_008f:  ceq
    IL_0091:  stloc.s    V_5
    IL_0093:  ldloc.s    V_4
    IL_0095:  ldloc.s    V_5
    IL_0097:  brtrue.s   IL_009c

    IL_0099:  ldc.i4.6
    IL_009a:  br.s       IL_009d

    IL_009c:  ldc.i4.8
    IL_009d:  call       void [mscorlib]System.Runtime.InteropServices.Marshal::WriteInt32(native int,
                                                                                           int32)
    IL_00a2:  ldarg.0
    IL_00a3:  ldloca.s   V_1
    IL_00a5:  ldloc.s    V_4
    IL_00a7:  ldloc.3
    IL_00a8:  ldloca.s   V_3
    IL_00aa:  ldsfld     native int [mscorlib]System.IntPtr::Zero
    IL_00af:  call       bool Pololu.WinusbHelper.Winusb::SetupDiGetDeviceInterfaceDetail(native int,
                                                                                          valuetype Pololu.WinusbHelper.Winusb/SP_DEVICE_INTERFACE_DATA&,
                                                                                          native int,
                                                                                          uint32,
                                                                                          uint32&,
                                                                                          native int)
    IL_00b4:  stloc.2
    IL_00b5:  ldloc.2
    IL_00b6:  brtrue.s   IL_00ca

    IL_00b8:  ldloc.s    V_4
    IL_00ba:  call       void [mscorlib]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
    IL_00bf:  ldstr      "Unable to get the device interface detail."
    IL_00c4:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_00c9:  throw

    IL_00ca:  ldloca.s   V_6
    IL_00cc:  ldloca.s   V_4
    IL_00ce:  call       instance int32 [mscorlib]System.IntPtr::ToInt32()
    IL_00d3:  ldc.i4.4
    IL_00d4:  add
    IL_00d5:  call       instance void [mscorlib]System.IntPtr::.ctor(int32)
    IL_00da:  ldloc.s    V_6
    IL_00dc:  call       string [mscorlib]System.Runtime.InteropServices.Marshal::PtrToStringAuto(native int)
    IL_00e1:  stloc.s    V_7
    IL_00e3:  ldloca.s   V_0
    IL_00e5:  ldloc.s    V_7
    IL_00e7:  ldc.i4     0xc0000000
    IL_00ec:  ldc.i4.3
    IL_00ed:  ldsfld     native int [mscorlib]System.IntPtr::Zero
    IL_00f2:  ldc.i4.3
    IL_00f3:  ldc.i4     0x40000000
    IL_00f8:  ldsfld     native int [mscorlib]System.IntPtr::Zero
    IL_00fd:  call       class [mscorlib]Microsoft.Win32.SafeHandles.SafeFileHandle Pololu.WinusbHelper.Winusb::CreateFile(string,
                                                                                                                           uint32,
                                                                                                                           uint32,
                                                                                                                           native int,
                                                                                                                           uint32,
                                                                                                                           uint32,
                                                                                                                           native int)
    IL_0102:  stfld      class [mscorlib]Microsoft.Win32.SafeHandles.SafeFileHandle Pololu.WinusbHelper.WinUsbDeviceHandles::deviceHandle
    IL_0107:  ldloc.s    V_4
    IL_0109:  call       void [mscorlib]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
    IL_010e:  ldloca.s   V_0
    IL_0110:  ldfld      class [mscorlib]Microsoft.Win32.SafeHandles.SafeFileHandle Pololu.WinusbHelper.WinUsbDeviceHandles::deviceHandle
    IL_0115:  callvirt   instance bool [mscorlib]System.Runtime.InteropServices.SafeHandle::get_IsInvalid()
    IL_011a:  brfalse.s  IL_013a

    IL_011c:  call       int32 [mscorlib]System.Runtime.InteropServices.Marshal::GetLastWin32Error()
    IL_0121:  ldc.i4.5
    IL_0122:  bne.un.s   IL_012f

    IL_0124:  ldstr      "Access denied when trying to open device.  Try clo"
    + "sing all other programs that are using the device."
    IL_0129:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_012e:  throw

    IL_012f:  ldstr      "Unabled to create a handle for the device."
    IL_0134:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_0139:  throw

    IL_013a:  ldloca.s   V_0
    IL_013c:  ldfld      class [mscorlib]Microsoft.Win32.SafeHandles.SafeFileHandle Pololu.WinusbHelper.WinUsbDeviceHandles::deviceHandle
    IL_0141:  ldloca.s   V_0
    IL_0143:  ldflda     native int Pololu.WinusbHelper.WinUsbDeviceHandles::winusbHandle
    IL_0148:  call       bool Pololu.WinusbHelper.Winusb::WinUsb_Initialize(class [mscorlib]Microsoft.Win32.SafeHandles.SafeFileHandle,
                                                                            native int&)
    IL_014d:  stloc.2
    IL_014e:  ldloc.2
    IL_014f:  brtrue.s   IL_0171

    IL_0151:  call       int32 [mscorlib]System.Runtime.InteropServices.Marshal::GetLastWin32Error()
    IL_0156:  stloc.s    V_8
    IL_0158:  ldloca.s   V_0
    IL_015a:  ldfld      class [mscorlib]Microsoft.Win32.SafeHandles.SafeFileHandle Pololu.WinusbHelper.WinUsbDeviceHandles::deviceHandle
    IL_015f:  callvirt   instance void [mscorlib]System.Runtime.InteropServices.SafeHandle::Close()
    IL_0164:  ldloc.s    V_8
    IL_0166:  ldstr      "Unable to initialize WinUSB."
    IL_016b:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(int32,
                                                                                           string)
    IL_0170:  throw

    IL_0171:  ldc.i4     0x15e
    IL_0176:  stloc.s    V_9
    IL_0178:  ldloca.s   V_0
    IL_017a:  ldfld      native int Pololu.WinusbHelper.WinUsbDeviceHandles::winusbHandle
    IL_017f:  ldc.i4.0
    IL_0180:  ldc.i4.3
    IL_0181:  ldc.i4.4
    IL_0182:  ldloca.s   V_9
    IL_0184:  conv.u
    IL_0185:  call       bool Pololu.WinusbHelper.Winusb::WinUsb_SetPipePolicy(native int,
                                                                               uint8,
                                                                               uint32,
                                                                               uint32,
                                                                               void*)
    IL_018a:  stloc.2
    IL_018b:  ldloc.2
    IL_018c:  brtrue.s   IL_01a8

    IL_018e:  call       int32 [mscorlib]System.Runtime.InteropServices.Marshal::GetLastWin32Error()
    IL_0193:  stloc.s    V_10
    IL_0195:  ldloc.0
    IL_0196:  call       void Pololu.WinusbHelper.Winusb::disconnect(valuetype Pololu.WinusbHelper.WinUsbDeviceHandles)
    IL_019b:  ldloc.s    V_10
    IL_019d:  ldstr      "Unable to set control transfer timeout."
    IL_01a2:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(int32,
                                                                                           string)
    IL_01a7:  throw

    IL_01a8:  ldloc.0
    IL_01a9:  ret
  } // end of method Winusb::listConnect

  .method assembly hidebysig static void 
          disconnect(valuetype Pololu.WinusbHelper.WinUsbDeviceHandles handles) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  8
    IL_0000:  ldarga.s   handles
    IL_0002:  ldfld      native int Pololu.WinusbHelper.WinUsbDeviceHandles::winusbHandle
    IL_0007:  call       bool Pololu.WinusbHelper.Winusb::WinUsb_Free(native int)
    IL_000c:  pop
    IL_000d:  ldarga.s   handles
    IL_000f:  ldfld      class [mscorlib]Microsoft.Win32.SafeHandles.SafeFileHandle Pololu.WinusbHelper.WinUsbDeviceHandles::deviceHandle
    IL_0014:  callvirt   instance void [mscorlib]System.Runtime.InteropServices.SafeHandle::Close()
    IL_0019:  ret
  } // end of method Winusb::disconnect

  .method assembly hidebysig static native int 
          notificationRegister(valuetype [mscorlib]System.Guid deviceInterfaceGuid,
                               native int formHandle) cil managed
  {
    // Code size       74 (0x4a)
    .maxstack  3
    .locals init (valuetype Pololu.WinusbHelper.Winusb/DEV_BROADCAST_DEVICEINTERFACE V_0,
             native int V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    Pololu.WinusbHelper.Winusb/DEV_BROADCAST_DEVICEINTERFACE
    IL_0008:  ldloca.s   V_0
    IL_000a:  sizeof     Pololu.WinusbHelper.Winusb/DEV_BROADCAST_DEVICEINTERFACE
    IL_0010:  stfld      uint32 Pololu.WinusbHelper.Winusb/DEV_BROADCAST_DEVICEINTERFACE::dbcc_size
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldc.i4.5
    IL_0018:  stfld      uint32 Pololu.WinusbHelper.Winusb/DEV_BROADCAST_DEVICEINTERFACE::dbcc_devicetype
    IL_001d:  ldloca.s   V_0
    IL_001f:  ldarg.0
    IL_0020:  stfld      valuetype [mscorlib]System.Guid Pololu.WinusbHelper.Winusb/DEV_BROADCAST_DEVICEINTERFACE::dbcc_classguid
    IL_0025:  ldarg.1
    IL_0026:  ldloca.s   V_0
    IL_0028:  conv.u
    IL_0029:  ldc.i4.0
    IL_002a:  call       native int Pololu.WinusbHelper.Winusb::RegisterDeviceNotification(native int,
                                                                                           valuetype Pololu.WinusbHelper.Winusb/DEV_BROADCAST_DEVICEINTERFACE*,
                                                                                           uint32)
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  ldsfld     native int [mscorlib]System.IntPtr::Zero
    IL_0036:  call       bool [mscorlib]System.IntPtr::op_Equality(native int,
                                                                   native int)
    IL_003b:  brfalse.s  IL_0048

    IL_003d:  ldstr      "There was an error registering for device attachme"
    + "nt/removal notifications."
    IL_0042:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_0047:  throw

    IL_0048:  ldloc.1
    IL_0049:  ret
  } // end of method Winusb::notificationRegister

  .method assembly hidebysig static void 
          notificationUnregister(native int deviceNotificationHandle) cil managed
  {
    // Code size       22 (0x16)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       bool Pololu.WinusbHelper.Winusb::UnregisterDeviceNotification(native int)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brtrue.s   IL_0015

    IL_000a:  ldstr      "There was an error unregistering device attachment"
    + "/removal notifications."
    IL_000f:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_0014:  throw

    IL_0015:  ret
  } // end of method Winusb::notificationUnregister

  .method assembly hidebysig static class [mscorlib]System.Collections.Generic.IList`1<string> 
          getPortNames(string deviceInstanceIdPrefix) cil managed
  {
    // Code size       153 (0x99)
    .maxstack  4
    .locals init (native int V_0,
             uint32 V_1,
             class [mscorlib]System.Collections.Generic.IList`1<string> V_2,
             uint32 V_3,
             valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA V_4,
             string V_5,
             string V_6)
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance string [mscorlib]System.String::ToUpperInvariant()
    IL_0006:  starg.s    deviceInstanceIdPrefix
    IL_0008:  ldsflda    valuetype [mscorlib]System.Guid Pololu.WinusbHelper.Winusb::GUID_DEVCLASS_PORTS
    IL_000d:  ldstr      "USB"
    IL_0012:  ldsfld     native int [mscorlib]System.IntPtr::Zero
    IL_0017:  ldc.i4.2
    IL_0018:  call       native int Pololu.WinusbHelper.Winusb::SetupDiGetClassDevs(valuetype [mscorlib]System.Guid&,
                                                                                    string,
                                                                                    native int,
                                                                                    uint32)
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldc.i4.m1
    IL_0020:  newobj     instance void [mscorlib]System.IntPtr::.ctor(int32)
    IL_0025:  call       bool [mscorlib]System.IntPtr::op_Equality(native int,
                                                                   native int)
    IL_002a:  brfalse.s  IL_0037

    IL_002c:  ldstr      "Unable to create a list of connected USB-to-serial"
    + " adapters."
    IL_0031:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_0036:  throw

    IL_0037:  ldloc.0
    IL_0038:  call       uint32 Pololu.WinusbHelper.Winusb::listSize(native int)
    IL_003d:  stloc.1
    IL_003e:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<string>::.ctor()
    IL_0043:  stloc.2
    .try
    {
      IL_0044:  ldc.i4.0
      IL_0045:  stloc.3
      IL_0046:  br.s       IL_0089

      IL_0048:  ldloc.0
      IL_0049:  ldloc.3
      IL_004a:  call       valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA Pololu.WinusbHelper.Winusb::listGetDevinfoData(native int,
                                                                                                                               uint32)
      IL_004f:  stloc.s    V_4
      IL_0051:  ldloca.s   V_4
      IL_0053:  ldfld      int32 Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA::DevInst
      IL_0058:  call       string Pololu.WinusbHelper.Winusb::getDeviceInstanceId(int32)
      IL_005d:  stloc.s    V_5
      IL_005f:  ldloc.s    V_5
      IL_0061:  callvirt   instance string [mscorlib]System.String::ToUpperInvariant()
      IL_0066:  ldarg.0
      IL_0067:  callvirt   instance bool [mscorlib]System.String::StartsWith(string)
      IL_006c:  brfalse.s  IL_0085

      IL_006e:  ldloc.0
      IL_006f:  ldloc.s    V_4
      IL_0071:  ldstr      "PortName"
      IL_0076:  call       string Pololu.WinusbHelper.Winusb::getCustomDeviceProperty(native int,
                                                                                      valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA,
                                                                                      string)
      IL_007b:  stloc.s    V_6
      IL_007d:  ldloc.2
      IL_007e:  ldloc.s    V_6
      IL_0080:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<string>::Add(!0)
      IL_0085:  ldloc.3
      IL_0086:  ldc.i4.1
      IL_0087:  add
      IL_0088:  stloc.3
      IL_0089:  ldloc.3
      IL_008a:  ldloc.1
      IL_008b:  blt.un.s   IL_0048

      IL_008d:  leave.s    IL_0097

    }  // end .try
    finally
    {
      IL_008f:  ldloc.0
      IL_0090:  call       int32 Pololu.WinusbHelper.Winusb::SetupDiDestroyDeviceInfoList(native int)
      IL_0095:  pop
      IL_0096:  endfinally
    }  // end handler
    IL_0097:  ldloc.2
    IL_0098:  ret
  } // end of method Winusb::getPortNames

  .method private hidebysig static string 
          getCustomDeviceProperty(native int listHandle,
                                  valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA devinfoData,
                                  string propertyName) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  8
    .locals init (uint8[] V_0,
             bool V_1,
             uint8& pinned V_2,
             string V_3)
    IL_0000:  ldc.i4.s   100
    IL_0002:  newarr     [mscorlib]System.Byte
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldelema    [mscorlib]System.Byte
    IL_000f:  stloc.2
    IL_0010:  ldarg.0
    IL_0011:  ldarga.s   devinfoData
    IL_0013:  ldarg.2
    IL_0014:  ldc.i4.0
    IL_0015:  ldc.i4.0
    IL_0016:  conv.i
    IL_0017:  ldloc.2
    IL_0018:  conv.i
    IL_0019:  ldc.i4.s   100
    IL_001b:  ldc.i4.0
    IL_001c:  conv.i
    IL_001d:  call       bool Pololu.WinusbHelper.Winusb::SetupDiGetCustomDeviceProperty(native int,
                                                                                         valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA&,
                                                                                         string,
                                                                                         uint32,
                                                                                         uint32*,
                                                                                         uint8*,
                                                                                         uint32,
                                                                                         uint32*)
    IL_0022:  stloc.1
    IL_0023:  ldloc.1
    IL_0024:  brtrue.s   IL_0031

    IL_0026:  ldstr      "There was an error getting the port name."
    IL_002b:  newobj     instance void [System]System.ComponentModel.Win32Exception::.ctor(string)
    IL_0030:  throw

    IL_0031:  ldloc.2
    IL_0032:  conv.i
    IL_0033:  newobj     instance void [mscorlib]System.String::.ctor(char*)
    IL_0038:  stloc.3
    IL_0039:  leave.s    IL_003b

    IL_003b:  ldloc.3
    IL_003c:  ret
  } // end of method Winusb::getCustomDeviceProperty

  .method private hidebysig static pinvokeimpl("setupapi.dll" lasterr winapi) 
          int32  SetupDiDestroyDeviceInfoList(native int DeviceInfoSet) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("setupapi.dll" lasterr winapi) 
          bool  SetupDiEnumDeviceInfo(native int DeviceInfoSet,
                                      uint32 index,
                                      valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA& DeviceInfoData) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("setupapi.dll" lasterr winapi) 
          bool  SetupDiEnumDeviceInterfaces(native int DeviceInfoSet,
                                            native int DeviceInfoData,
                                            valuetype [mscorlib]System.Guid& InterfaceClassGuid,
                                            uint32 MemberIndex,
                                            valuetype Pololu.WinusbHelper.Winusb/SP_DEVICE_INTERFACE_DATA& DeviceInterfaceData) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("setupapi.dll" autochar lasterr winapi) 
          native int  SetupDiGetClassDevs(valuetype [mscorlib]System.Guid& ClassGuid,
                                          string  marshal( lptstr) Enumerator,
                                          native int hwndParent,
                                          uint32 Flags) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("setupapi.dll" autochar lasterr winapi) 
          bool  SetupDiGetDeviceInterfaceDetail(native int DeviceInfoSet,
                                                valuetype Pololu.WinusbHelper.Winusb/SP_DEVICE_INTERFACE_DATA& DeviceInterfaceData,
                                                native int DeviceInterfaceDetailData,
                                                uint32 DeviceInterfaceDetailDataSize,
                                                uint32& RequiredSize,
                                                native int DeviceInfoData) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("setupapi.dll" autochar lasterr winapi) 
          bool  SetupDiGetCustomDeviceProperty(native int DeviceInfoSet,
                                               valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA& DeviceInfoData,
                                               string CustomPropertyName,
                                               uint32 Flags,
                                               uint32* PropertyRegDataType,
                                               uint8* PropertyBuffer,
                                               uint32 PropertyBufferSize,
                                               uint32* RequiredSize) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("setupapi.dll" autochar lasterr winapi) 
          bool  SetupDiGetDeviceRegistryProperty(native int DeviceInfoSet,
                                                 valuetype Pololu.WinusbHelper.Winusb/SP_DEVINFO_DATA& DeviceInfoData,
                                                 uint32 Property,
                                                 native int PropertyRegDataType,
                                                 uint8* PropertyBuffer,
                                                 uint32 PropertyBufferSize,
                                                 native int RequiredSize) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("kernel32.dll" autochar lasterr winapi) 
          class [mscorlib]Microsoft.Win32.SafeHandles.SafeFileHandle 
          CreateFile(string lpFileName,
                     uint32 dwDesiredAccess,
                     uint32 dwShareMode,
                     native int lpSecurityAttributes,
                     uint32 dwCreationDisposition,
                     uint32 dwFlagsAndAttributes,
                     native int hTemplateFile) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("winusb.dll" lasterr winapi) 
          bool  WinUsb_Initialize(class [mscorlib]Microsoft.Win32.SafeHandles.SafeFileHandle DeviceHandle,
                                  native int& InterfaceHandle) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("winusb.dll" lasterr winapi) 
          bool  WinUsb_Free(native int InterfaceHandle) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("winusb.dll" lasterr winapi) 
          bool  WinUsb_SetPipePolicy(native int InterfaceHandle,
                                     uint8 PipeID,
                                     uint32 PolicyType,
                                     uint32 ValueLength,
                                     void* 'value') cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("cfgmgr32.dll" lasterr winapi) 
          uint32  CM_Get_Parent([out] int32& pdnDevInst,
                                int32 dnDevInst,
                                uint32 ulFlags) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("cfgmgr32.dll" unicode lasterr winapi) 
          uint32  CM_Get_Device_IDW(int32 dnDevInst,
                                    native int Buffer,
                                    uint32 BufferLen,
                                    uint32 ulFlags) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("user32.dll" autochar lasterr winapi) 
          native int  RegisterDeviceNotification(native int hRecipient,
                                                 valuetype Pololu.WinusbHelper.Winusb/DEV_BROADCAST_DEVICEINTERFACE* NotificationFilter,
                                                 uint32 Flags) cil managed preservesig
  {
  }
  .method private hidebysig static pinvokeimpl("user32.dll" lasterr winapi) 
          bool  UnregisterDeviceNotification(native int Handle) cil managed preservesig
  {
  }
  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       48 (0x30)
    .maxstack  12
    IL_0000:  ldc.i4     0x4d36e978
    IL_0005:  ldc.i4     0xe325
    IL_000a:  ldc.i4     0x11ce
    IL_000f:  ldc.i4     0xbf
    IL_0014:  ldc.i4     0xc1
    IL_0019:  ldc.i4.8
    IL_001a:  ldc.i4.0
    IL_001b:  ldc.i4.s   43
    IL_001d:  ldc.i4     0xe1
    IL_0022:  ldc.i4.3
    IL_0023:  ldc.i4.s   24
    IL_0025:  newobj     instance void [mscorlib]System.Guid::.ctor(uint32,
                                                                    uint16,
                                                                    uint16,
                                                                    uint8,
                                                                    uint8,
                                                                    uint8,
                                                                    uint8,
                                                                    uint8,
                                                                    uint8,
                                                                    uint8,
                                                                    uint8)
    IL_002a:  stsfld     valuetype [mscorlib]System.Guid Pololu.WinusbHelper.Winusb::GUID_DEVCLASS_PORTS
    IL_002f:  ret
  } // end of method Winusb::.cctor

} // end of class Pololu.WinusbHelper.Winusb


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file UsbWrapper.res
