
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System.Xml
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System.Windows.Forms
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Sequencer
{
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 06 50 6F 6C 6F 6C 75 00 00 )                // ...Pololu..
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 09 53 65 71 75 65 6E 63 65 72 00 00 )       // ...Sequencer..
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 1D 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. 
                                                                                                  50 6F 6C 6F 6C 75 20 32 30 30 39 2D 32 30 31 30   // Pololu 2009-2010
                                                                                                  00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 09 53 65 71 75 65 6E 63 65 72 00 00 )       // ...Sequencer..
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 65 61 34 64 61 62 32 65 2D 62 63 36 36   // ..$ea4dab2e-bc66
                                                                                                  2D 34 38 34 39 2D 39 61 36 30 2D 30 64 63 64 36   // -4849-9a60-0dcd6
                                                                                                  36 65 39 63 35 35 30 00 00 )                      // 6e9c550..
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 32 2E 30 2E 31 00 00 )             // ...1.2.0.1..
  .custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 1A 2E 4E 45 54 46 72 61 6D 65 77 6F 72 6B   // ....NETFramework
                                                                                                        2C 56 65 72 73 69 6F 6E 3D 76 34 2E 30 01 00 54   // ,Version=v4.0..T
                                                                                                        0E 14 46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C   // ..FrameworkDispl
                                                                                                        61 79 4E 61 6D 65 10 2E 4E 45 54 20 46 72 61 6D   // ayName..NET Fram
                                                                                                        65 77 6F 72 6B 20 34 )                            // ework 4

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .hash algorithm 0x00008004
  .ver 1:2:4582:40579
}
.module Sequencer.dll
// MVID: {10A0F017-2B84-4FDA-A43F-095672C47D3B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00470000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi serializable beforefieldinit Pololu.Usc.Sequencer.Frame
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 06 74 61 72 67 65 74 00 00 )                // ...target..
  .field public string name
  .field private uint16[] privateTargets
  .field public uint16 length_ms
  .method public hidebysig specialname instance uint16 
          get_target(int32 channel) cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init ([0] uint16 CS$1$0000,
             [1] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint16[] Pololu.Usc.Sequencer.Frame::privateTargets
    IL_0007:  brfalse.s  IL_0016

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  ldfld      uint16[] Pololu.Usc.Sequencer.Frame::privateTargets
    IL_0010:  ldlen
    IL_0011:  conv.i4
    IL_0012:  clt
    IL_0014:  br.s       IL_0017

    IL_0016:  ldc.i4.0
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  brtrue.s   IL_0020

    IL_001b:  nop
    IL_001c:  ldc.i4.0
    IL_001d:  stloc.0
    IL_001e:  br.s       IL_002b

    IL_0020:  ldarg.0
    IL_0021:  ldfld      uint16[] Pololu.Usc.Sequencer.Frame::privateTargets
    IL_0026:  ldarg.1
    IL_0027:  ldelem.u2
    IL_0028:  stloc.0
    IL_0029:  br.s       IL_002b

    IL_002b:  ldloc.0
    IL_002c:  ret
  } // end of method Frame::get_target

  .method public hidebysig specialname instance void 
          set_targets(uint16[] 'value') cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  stfld      uint16[] Pololu.Usc.Sequencer.Frame::privateTargets
    IL_0008:  ret
  } // end of method Frame::set_targets

  .method public hidebysig instance string 
          getTargetsString() cil managed
  {
    // Code size       84 (0x54)
    .maxstack  3
    .locals init ([0] string targetsString,
             [1] int32 i,
             [2] string CS$1$0000,
             [3] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldstr      ""
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_003f

    IL_000b:  nop
    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.0
    IL_000e:  ceq
    IL_0010:  stloc.3
    IL_0011:  ldloc.3
    IL_0012:  brtrue.s   IL_0022

    IL_0014:  nop
    IL_0015:  ldloc.0
    IL_0016:  ldstr      " "
    IL_001b:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0020:  stloc.0
    IL_0021:  nop
    IL_0022:  ldloc.0
    IL_0023:  ldarg.0
    IL_0024:  ldfld      uint16[] Pololu.Usc.Sequencer.Frame::privateTargets
    IL_0029:  ldloc.1
    IL_002a:  ldelema    [mscorlib]System.UInt16
    IL_002f:  call       instance string [mscorlib]System.UInt16::ToString()
    IL_0034:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0039:  stloc.0
    IL_003a:  nop
    IL_003b:  ldloc.1
    IL_003c:  ldc.i4.1
    IL_003d:  add
    IL_003e:  stloc.1
    IL_003f:  ldloc.1
    IL_0040:  ldarg.0
    IL_0041:  ldfld      uint16[] Pololu.Usc.Sequencer.Frame::privateTargets
    IL_0046:  ldlen
    IL_0047:  conv.i4
    IL_0048:  clt
    IL_004a:  stloc.3
    IL_004b:  ldloc.3
    IL_004c:  brtrue.s   IL_000b

    IL_004e:  ldloc.0
    IL_004f:  stloc.2
    IL_0050:  br.s       IL_0052

    IL_0052:  ldloc.2
    IL_0053:  ret
  } // end of method Frame::getTargetsString

  .method private hidebysig instance string 
          getTabSeparatedString() cil managed
  {
    // Code size       92 (0x5c)
    .maxstack  3
    .locals init ([0] string tabString,
             [1] uint16 target,
             [2] string CS$1$0000,
             [3] uint16[] CS$6$0001,
             [4] int32 CS$7$0002,
             [5] bool CS$4$0003)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      string Pololu.Usc.Sequencer.Frame::name
    IL_0007:  ldstr      "\t"
    IL_000c:  ldarg.0
    IL_000d:  ldfld      uint16 Pololu.Usc.Sequencer.Frame::length_ms
    IL_0012:  box        [mscorlib]System.UInt16
    IL_0017:  call       string [mscorlib]System.String::Concat(object,
                                                                object,
                                                                object)
    IL_001c:  stloc.0
    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  ldfld      uint16[] Pololu.Usc.Sequencer.Frame::privateTargets
    IL_0024:  stloc.3
    IL_0025:  ldc.i4.0
    IL_0026:  stloc.s    CS$7$0002
    IL_0028:  br.s       IL_0049

    IL_002a:  ldloc.3
    IL_002b:  ldloc.s    CS$7$0002
    IL_002d:  ldelem.u2
    IL_002e:  stloc.1
    IL_002f:  nop
    IL_0030:  ldloc.0
    IL_0031:  ldstr      "\t"
    IL_0036:  ldloc.1
    IL_0037:  box        [mscorlib]System.UInt16
    IL_003c:  call       string [mscorlib]System.String::Concat(object,
                                                                object,
                                                                object)
    IL_0041:  stloc.0
    IL_0042:  nop
    IL_0043:  ldloc.s    CS$7$0002
    IL_0045:  ldc.i4.1
    IL_0046:  add
    IL_0047:  stloc.s    CS$7$0002
    IL_0049:  ldloc.s    CS$7$0002
    IL_004b:  ldloc.3
    IL_004c:  ldlen
    IL_004d:  conv.i4
    IL_004e:  clt
    IL_0050:  stloc.s    CS$4$0003
    IL_0052:  ldloc.s    CS$4$0003
    IL_0054:  brtrue.s   IL_002a

    IL_0056:  ldloc.0
    IL_0057:  stloc.2
    IL_0058:  br.s       IL_005a

    IL_005a:  ldloc.2
    IL_005b:  ret
  } // end of method Frame::getTabSeparatedString

  .method public hidebysig instance void 
          setTargetsFromString(string targetsString,
                               uint8 servoCount) cil managed
  {
    // Code size       88 (0x58)
    .maxstack  4
    .locals init ([0] uint16[] tmpTargets,
             [1] string[] targetStrings,
             [2] int32 i,
             [3] char[] CS$0$0000,
             [4] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  newarr     [mscorlib]System.UInt16
    IL_0007:  stloc.0
    IL_0008:  ldarg.1
    IL_0009:  ldc.i4.1
    IL_000a:  newarr     [mscorlib]System.Char
    IL_000f:  stloc.3
    IL_0010:  ldloc.3
    IL_0011:  ldc.i4.0
    IL_0012:  ldc.i4.s   32
    IL_0014:  stelem.i2
    IL_0015:  ldloc.3
    IL_0016:  ldc.i4.1
    IL_0017:  callvirt   instance string[] [mscorlib]System.String::Split(char[],
                                                                          valuetype [mscorlib]System.StringSplitOptions)
    IL_001c:  stloc.1
    IL_001d:  ldc.i4.0
    IL_001e:  stloc.2
    IL_001f:  br.s       IL_003c

    IL_0021:  nop
    .try
    {
      IL_0022:  nop
      IL_0023:  ldloc.0
      IL_0024:  ldloc.2
      IL_0025:  ldloc.1
      IL_0026:  ldloc.2
      IL_0027:  ldelem.ref
      IL_0028:  call       uint16 [mscorlib]System.UInt16::Parse(string)
      IL_002d:  stelem.i2
      IL_002e:  nop
      IL_002f:  leave.s    IL_0036

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0031:  pop
      IL_0032:  nop
      IL_0033:  nop
      IL_0034:  leave.s    IL_0036

    }  // end handler
    IL_0036:  nop
    IL_0037:  nop
    IL_0038:  ldloc.2
    IL_0039:  ldc.i4.1
    IL_003a:  add
    IL_003b:  stloc.2
    IL_003c:  ldloc.2
    IL_003d:  ldloc.1
    IL_003e:  ldlen
    IL_003f:  conv.i4
    IL_0040:  bge.s      IL_0048

    IL_0042:  ldloc.2
    IL_0043:  ldarg.2
    IL_0044:  clt
    IL_0046:  br.s       IL_0049

    IL_0048:  ldc.i4.0
    IL_0049:  stloc.s    CS$4$0001
    IL_004b:  ldloc.s    CS$4$0001
    IL_004d:  brtrue.s   IL_0021

    IL_004f:  ldarg.0
    IL_0050:  ldloc.0
    IL_0051:  call       instance void Pololu.Usc.Sequencer.Frame::set_targets(uint16[])
    IL_0056:  nop
    IL_0057:  ret
  } // end of method Frame::setTargetsFromString

  .method public hidebysig instance void 
          writeXml(class [System.Xml]System.Xml.XmlWriter writer) cil managed
  {
    // Code size       75 (0x4b)
    .maxstack  3
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldstr      "Frame"
    IL_0007:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteStartElement(string)
    IL_000c:  nop
    IL_000d:  ldarg.1
    IL_000e:  ldstr      "name"
    IL_0013:  ldarg.0
    IL_0014:  ldfld      string Pololu.Usc.Sequencer.Frame::name
    IL_0019:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_001e:  nop
    IL_001f:  ldarg.1
    IL_0020:  ldstr      "duration"
    IL_0025:  ldarg.0
    IL_0026:  ldflda     uint16 Pololu.Usc.Sequencer.Frame::length_ms
    IL_002b:  call       instance string [mscorlib]System.UInt16::ToString()
    IL_0030:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_0035:  nop
    IL_0036:  ldarg.1
    IL_0037:  ldarg.0
    IL_0038:  call       instance string Pololu.Usc.Sequencer.Frame::getTargetsString()
    IL_003d:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteString(string)
    IL_0042:  nop
    IL_0043:  ldarg.1
    IL_0044:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteEndElement()
    IL_0049:  nop
    IL_004a:  ret
  } // end of method Frame::writeXml

  .method public hidebysig static void  copyToClipboard(class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> frames) cil managed
  {
    // Code size       165 (0xa5)
    .maxstack  3
    .locals init ([0] class [System.Windows.Forms]System.Windows.Forms.DataObject data,
             [1] class [mscorlib]System.Text.StringBuilder sb,
             [2] class Pololu.Usc.Sequencer.Frame frame,
             [3] bool CS$4$0000,
             [4] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Sequencer.Frame> CS$5$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::get_Count()
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  ldc.i4.0
    IL_000b:  ceq
    IL_000d:  stloc.3
    IL_000e:  ldloc.3
    IL_000f:  brtrue.s   IL_0017

    IL_0011:  nop
    IL_0012:  br         IL_00a4

    IL_0017:  newobj     instance void [System.Windows.Forms]System.Windows.Forms.DataObject::.ctor()
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldarg.0
    IL_001f:  callvirt   instance !0[] class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::ToArray()
    IL_0024:  callvirt   instance void [System.Windows.Forms]System.Windows.Forms.DataObject::SetData(object)
    IL_0029:  nop
    IL_002a:  ldarg.0
    IL_002b:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::get_Count()
    IL_0030:  ldc.i4.1
    IL_0031:  ceq
    IL_0033:  ldc.i4.0
    IL_0034:  ceq
    IL_0036:  stloc.3
    IL_0037:  ldloc.3
    IL_0038:  brtrue.s   IL_004a

    IL_003a:  nop
    IL_003b:  ldloc.0
    IL_003c:  ldarg.0
    IL_003d:  ldc.i4.0
    IL_003e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::get_Item(int32)
    IL_0043:  callvirt   instance void [System.Windows.Forms]System.Windows.Forms.DataObject::SetData(object)
    IL_0048:  nop
    IL_0049:  nop
    IL_004a:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_004f:  stloc.1
    IL_0050:  nop
    IL_0051:  ldarg.0
    IL_0052:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::GetEnumerator()
    IL_0057:  stloc.s    CS$5$0001
    .try
    {
      IL_0059:  br.s       IL_0072

      IL_005b:  ldloca.s   CS$5$0001
      IL_005d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Sequencer.Frame>::get_Current()
      IL_0062:  stloc.2
      IL_0063:  nop
      IL_0064:  ldloc.1
      IL_0065:  ldloc.2
      IL_0066:  callvirt   instance string Pololu.Usc.Sequencer.Frame::getTabSeparatedString()
      IL_006b:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::AppendLine(string)
      IL_0070:  pop
      IL_0071:  nop
      IL_0072:  ldloca.s   CS$5$0001
      IL_0074:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Sequencer.Frame>::MoveNext()
      IL_0079:  stloc.3
      IL_007a:  ldloc.3
      IL_007b:  brtrue.s   IL_005b

      IL_007d:  leave.s    IL_008e

    }  // end .try
    finally
    {
      IL_007f:  ldloca.s   CS$5$0001
      IL_0081:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Sequencer.Frame>
      IL_0087:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_008c:  nop
      IL_008d:  endfinally
    }  // end handler
    IL_008e:  nop
    IL_008f:  ldloc.0
    IL_0090:  ldloc.1
    IL_0091:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0096:  callvirt   instance void [System.Windows.Forms]System.Windows.Forms.DataObject::SetText(string)
    IL_009b:  nop
    IL_009c:  ldloc.0
    IL_009d:  ldc.i4.1
    IL_009e:  call       void [System.Windows.Forms]System.Windows.Forms.Clipboard::SetDataObject(object,
                                                                                                  bool)
    IL_00a3:  nop
    IL_00a4:  ret
  } // end of method Frame::copyToClipboard

  .method public hidebysig static class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> 
          getFromClipboard() cil managed
  {
    // Code size       447 (0x1bf)
    .maxstack  4
    .locals init ([0] class Pololu.Usc.Sequencer.Frame[] frameArray,
             [1] class Pololu.Usc.Sequencer.Frame frameDirect,
             [2] class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> '<>g__initLocal0',
             [3] class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> frames,
             [4] string[] rows,
             [5] string row,
             [6] string[] parts,
             [7] class Pololu.Usc.Sequencer.Frame frame,
             [8] class [mscorlib]System.Collections.Generic.List`1<uint16> targets,
             [9] int32 i,
             [10] class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> CS$1$0000,
             [11] bool CS$4$0001,
             [12] char[] CS$0$0002,
             [13] string[] CS$6$0003,
             [14] int32 CS$7$0004)
    IL_0000:  nop
    IL_0001:  ldstr      "Pololu.Usc.Sequencer.Frame[]"
    IL_0006:  call       object [System.Windows.Forms]System.Windows.Forms.Clipboard::GetData(string)
    IL_000b:  isinst     class Pololu.Usc.Sequencer.Frame[]
    IL_0010:  stloc.0
    IL_0011:  ldloc.0
    IL_0012:  ldnull
    IL_0013:  ceq
    IL_0015:  stloc.s    CS$4$0001
    IL_0017:  ldloc.s    CS$4$0001
    IL_0019:  brtrue.s   IL_0029

    IL_001b:  nop
    IL_001c:  ldloc.0
    IL_001d:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0022:  stloc.s    CS$1$0000
    IL_0024:  br         IL_01bc

    IL_0029:  ldstr      "Pololu.Usc.Sequencer.Frame"
    IL_002e:  call       object [System.Windows.Forms]System.Windows.Forms.Clipboard::GetData(string)
    IL_0033:  isinst     Pololu.Usc.Sequencer.Frame
    IL_0038:  stloc.1
    IL_0039:  ldloc.1
    IL_003a:  ldnull
    IL_003b:  ceq
    IL_003d:  stloc.s    CS$4$0001
    IL_003f:  ldloc.s    CS$4$0001
    IL_0041:  brtrue.s   IL_005a

    IL_0043:  nop
    IL_0044:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::.ctor()
    IL_0049:  stloc.2
    IL_004a:  ldloc.2
    IL_004b:  ldloc.1
    IL_004c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::Add(!0)
    IL_0051:  nop
    IL_0052:  ldloc.2
    IL_0053:  stloc.s    CS$1$0000
    IL_0055:  br         IL_01bc

    IL_005a:  call       bool [System.Windows.Forms]System.Windows.Forms.Clipboard::ContainsText()
    IL_005f:  ldc.i4.0
    IL_0060:  ceq
    IL_0062:  stloc.s    CS$4$0001
    IL_0064:  ldloc.s    CS$4$0001
    IL_0066:  brtrue     IL_01b7

    IL_006b:  nop
    IL_006c:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::.ctor()
    IL_0071:  stloc.3
    IL_0072:  call       string [System.Windows.Forms]System.Windows.Forms.Clipboard::GetText()
    IL_0077:  ldc.i4.1
    IL_0078:  newarr     [mscorlib]System.Char
    IL_007d:  stloc.s    CS$0$0002
    IL_007f:  ldloc.s    CS$0$0002
    IL_0081:  ldc.i4.0
    IL_0082:  ldc.i4.s   10
    IL_0084:  stelem.i2
    IL_0085:  ldloc.s    CS$0$0002
    IL_0087:  callvirt   instance string[] [mscorlib]System.String::Split(char[])
    IL_008c:  stloc.s    rows
    IL_008e:  nop
    IL_008f:  ldloc.s    rows
    IL_0091:  stloc.s    CS$6$0003
    IL_0093:  ldc.i4.0
    IL_0094:  stloc.s    CS$7$0004
    IL_0096:  br         IL_01a1

    IL_009b:  ldloc.s    CS$6$0003
    IL_009d:  ldloc.s    CS$7$0004
    IL_009f:  ldelem.ref
    IL_00a0:  stloc.s    row
    IL_00a2:  nop
    IL_00a3:  ldloc.s    row
    IL_00a5:  ldc.i4.1
    IL_00a6:  newarr     [mscorlib]System.Char
    IL_00ab:  stloc.s    CS$0$0002
    IL_00ad:  ldloc.s    CS$0$0002
    IL_00af:  ldc.i4.0
    IL_00b0:  ldc.i4.s   9
    IL_00b2:  stelem.i2
    IL_00b3:  ldloc.s    CS$0$0002
    IL_00b5:  callvirt   instance string[] [mscorlib]System.String::Split(char[])
    IL_00ba:  stloc.s    parts
    IL_00bc:  ldloc.s    parts
    IL_00be:  ldlen
    IL_00bf:  conv.i4
    IL_00c0:  ldc.i4.3
    IL_00c1:  clt
    IL_00c3:  ldc.i4.0
    IL_00c4:  ceq
    IL_00c6:  stloc.s    CS$4$0001
    IL_00c8:  ldloc.s    CS$4$0001
    IL_00ca:  brtrue.s   IL_00d2

    IL_00cc:  nop
    IL_00cd:  br         IL_019b

    IL_00d2:  newobj     instance void Pololu.Usc.Sequencer.Frame::.ctor()
    IL_00d7:  stloc.s    frame
    IL_00d9:  ldloc.s    frame
    IL_00db:  ldloc.s    parts
    IL_00dd:  ldc.i4.0
    IL_00de:  ldelem.ref
    IL_00df:  stfld      string Pololu.Usc.Sequencer.Frame::name
    IL_00e4:  ldloc.s    frame
    IL_00e6:  ldfld      string Pololu.Usc.Sequencer.Frame::name
    IL_00eb:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_00f0:  ldc.i4.s   80
    IL_00f2:  cgt
    IL_00f4:  ldc.i4.0
    IL_00f5:  ceq
    IL_00f7:  stloc.s    CS$4$0001
    IL_00f9:  ldloc.s    CS$4$0001
    IL_00fb:  brtrue.s   IL_0115

    IL_00fd:  nop
    IL_00fe:  ldloc.s    frame
    IL_0100:  ldloc.s    frame
    IL_0102:  ldfld      string Pololu.Usc.Sequencer.Frame::name
    IL_0107:  ldc.i4.0
    IL_0108:  ldc.i4.s   80
    IL_010a:  callvirt   instance string [mscorlib]System.String::Substring(int32,
                                                                            int32)
    IL_010f:  stfld      string Pololu.Usc.Sequencer.Frame::name
    IL_0114:  nop
    .try
    {
      IL_0115:  nop
      IL_0116:  ldloc.s    frame
      IL_0118:  ldloc.s    parts
      IL_011a:  ldc.i4.1
      IL_011b:  ldelem.ref
      IL_011c:  call       uint16 [mscorlib]System.UInt16::Parse(string)
      IL_0121:  stfld      uint16 Pololu.Usc.Sequencer.Frame::length_ms
      IL_0126:  nop
      IL_0127:  leave.s    IL_013a

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0129:  pop
      IL_012a:  nop
      IL_012b:  ldloc.s    frame
      IL_012d:  ldc.i4     0x1f4
      IL_0132:  stfld      uint16 Pololu.Usc.Sequencer.Frame::length_ms
      IL_0137:  nop
      IL_0138:  leave.s    IL_013a

    }  // end handler
    IL_013a:  nop
    IL_013b:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<uint16>::.ctor()
    IL_0140:  stloc.s    targets
    IL_0142:  ldc.i4.2
    IL_0143:  stloc.s    i
    IL_0145:  br.s       IL_0174

    IL_0147:  nop
    .try
    {
      IL_0148:  nop
      IL_0149:  ldloc.s    targets
      IL_014b:  ldloc.s    parts
      IL_014d:  ldloc.s    i
      IL_014f:  ldelem.ref
      IL_0150:  call       uint16 [mscorlib]System.UInt16::Parse(string)
      IL_0155:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint16>::Add(!0)
      IL_015a:  nop
      IL_015b:  nop
      IL_015c:  leave.s    IL_016c

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_015e:  pop
      IL_015f:  nop
      IL_0160:  ldloc.s    targets
      IL_0162:  ldc.i4.0
      IL_0163:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint16>::Add(!0)
      IL_0168:  nop
      IL_0169:  nop
      IL_016a:  leave.s    IL_016c

    }  // end handler
    IL_016c:  nop
    IL_016d:  nop
    IL_016e:  ldloc.s    i
    IL_0170:  ldc.i4.1
    IL_0171:  add
    IL_0172:  stloc.s    i
    IL_0174:  ldloc.s    i
    IL_0176:  ldloc.s    parts
    IL_0178:  ldlen
    IL_0179:  conv.i4
    IL_017a:  clt
    IL_017c:  stloc.s    CS$4$0001
    IL_017e:  ldloc.s    CS$4$0001
    IL_0180:  brtrue.s   IL_0147

    IL_0182:  ldloc.s    frame
    IL_0184:  ldloc.s    targets
    IL_0186:  callvirt   instance !0[] class [mscorlib]System.Collections.Generic.List`1<uint16>::ToArray()
    IL_018b:  callvirt   instance void Pololu.Usc.Sequencer.Frame::set_targets(uint16[])
    IL_0190:  nop
    IL_0191:  ldloc.3
    IL_0192:  ldloc.s    frame
    IL_0194:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::Add(!0)
    IL_0199:  nop
    IL_019a:  nop
    IL_019b:  ldloc.s    CS$7$0004
    IL_019d:  ldc.i4.1
    IL_019e:  add
    IL_019f:  stloc.s    CS$7$0004
    IL_01a1:  ldloc.s    CS$7$0004
    IL_01a3:  ldloc.s    CS$6$0003
    IL_01a5:  ldlen
    IL_01a6:  conv.i4
    IL_01a7:  clt
    IL_01a9:  stloc.s    CS$4$0001
    IL_01ab:  ldloc.s    CS$4$0001
    IL_01ad:  brtrue     IL_009b

    IL_01b2:  ldloc.3
    IL_01b3:  stloc.s    CS$1$0000
    IL_01b5:  br.s       IL_01bc

    IL_01b7:  ldnull
    IL_01b8:  stloc.s    CS$1$0000
    IL_01ba:  br.s       IL_01bc

    IL_01bc:  ldloc.s    CS$1$0000
    IL_01be:  ret
  } // end of method Frame::getFromClipboard

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Frame::.ctor

  .property instance uint16 target(int32)
  {
    .get instance uint16 Pololu.Usc.Sequencer.Frame::get_target(int32)
  } // end of property Frame::target
  .property instance uint16[] targets()
  {
    .set instance void Pololu.Usc.Sequencer.Frame::set_targets(uint16[])
  } // end of property Frame::targets
} // end of class Pololu.Usc.Sequencer.Frame

.class public auto ansi beforefieldinit Pololu.Usc.Sequencer.Sequence
       extends [mscorlib]System.Object
{
  .class auto ansi nested private beforefieldinit FrameKeyNameComparer
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IComparer`1<string>
  {
    .method public hidebysig newslot virtual final 
            instance int32  Compare(string x,
                                    string y) cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  2
      .locals init ([0] int32 CS$1$0000)
      IL_0000:  nop
      .try
      {
        IL_0001:  nop
        IL_0002:  ldarg.1
        IL_0003:  call       uint16 [mscorlib]System.UInt16::Parse(string)
        IL_0008:  ldarg.2
        IL_0009:  call       uint16 [mscorlib]System.UInt16::Parse(string)
        IL_000e:  sub
        IL_000f:  stloc.0
        IL_0010:  leave.s    IL_0018

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0012:  pop
        IL_0013:  nop
        IL_0014:  ldc.i4.0
        IL_0015:  stloc.0
        IL_0016:  leave.s    IL_0018

      }  // end handler
      IL_0018:  nop
      IL_0019:  ldloc.0
      IL_001a:  ret
    } // end of method FrameKeyNameComparer::Compare

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method FrameKeyNameComparer::.ctor

  } // end of class FrameKeyNameComparer

  .field public string name
  .field public class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> frames
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(string name) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::.ctor()
    IL_0006:  stfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> Pololu.Usc.Sequencer.Sequence::frames
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0011:  nop
    IL_0012:  nop
    IL_0013:  ldarg.0
    IL_0014:  ldarg.1
    IL_0015:  stfld      string Pololu.Usc.Sequencer.Sequence::name
    IL_001a:  nop
    IL_001b:  ret
  } // end of method Sequence::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::.ctor()
    IL_0006:  stfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> Pololu.Usc.Sequencer.Sequence::frames
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0011:  nop
    IL_0012:  nop
    IL_0013:  nop
    IL_0014:  ret
  } // end of method Sequence::.ctor

  .method public hidebysig static void  saveSequencesInRegistry(class [mscorlib]System.Collections.Generic.IList`1<class Pololu.Usc.Sequencer.Sequence> list,
                                                                class [mscorlib]Microsoft.Win32.RegistryKey parentKey) cil managed
  {
    // Code size       320 (0x140)
    .maxstack  4
    .locals init ([0] class [mscorlib]Microsoft.Win32.RegistryKey sequencesKey,
             [1] int32 sequenceIndex,
             [2] class Pololu.Usc.Sequencer.Sequence sequence,
             [3] string sequenceKeyName,
             [4] class [mscorlib]Microsoft.Win32.RegistryKey sequenceKey,
             [5] int32 frameIndex,
             [6] class Pololu.Usc.Sequencer.Frame frame,
             [7] string frameKeyName,
             [8] class [mscorlib]Microsoft.Win32.RegistryKey frameKey,
             [9] bool CS$4$0000)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.1
      IL_0003:  ldstr      "sequences"
      IL_0008:  callvirt   instance void [mscorlib]Microsoft.Win32.RegistryKey::DeleteValue(string)
      IL_000d:  nop
      IL_000e:  nop
      IL_000f:  leave.s    IL_0016

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0011:  pop
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  leave.s    IL_0016

    }  // end handler
    IL_0016:  nop
    .try
    {
      IL_0017:  nop
      IL_0018:  ldarg.1
      IL_0019:  ldstr      "sequences"
      IL_001e:  callvirt   instance void [mscorlib]Microsoft.Win32.RegistryKey::DeleteSubKeyTree(string)
      IL_0023:  nop
      IL_0024:  nop
      IL_0025:  leave.s    IL_002c

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0027:  pop
      IL_0028:  nop
      IL_0029:  nop
      IL_002a:  leave.s    IL_002c

    }  // end handler
    IL_002c:  nop
    IL_002d:  ldarg.1
    IL_002e:  ldstr      "sequences"
    IL_0033:  callvirt   instance class [mscorlib]Microsoft.Win32.RegistryKey [mscorlib]Microsoft.Win32.RegistryKey::CreateSubKey(string)
    IL_0038:  pop
    IL_0039:  ldarg.1
    IL_003a:  ldstr      "sequences"
    IL_003f:  ldc.i4.1
    IL_0040:  callvirt   instance class [mscorlib]Microsoft.Win32.RegistryKey [mscorlib]Microsoft.Win32.RegistryKey::OpenSubKey(string,
                                                                                                                                bool)
    IL_0045:  stloc.0
    IL_0046:  ldc.i4.0
    IL_0047:  stloc.1
    IL_0048:  br         IL_0126

    IL_004d:  nop
    IL_004e:  ldarg.0
    IL_004f:  ldloc.1
    IL_0050:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class Pololu.Usc.Sequencer.Sequence>::get_Item(int32)
    IL_0055:  stloc.2
    IL_0056:  ldloca.s   sequenceIndex
    IL_0058:  ldstr      "d2"
    IL_005d:  call       instance string [mscorlib]System.Int32::ToString(string)
    IL_0062:  stloc.3
    IL_0063:  ldloc.0
    IL_0064:  ldloc.3
    IL_0065:  callvirt   instance class [mscorlib]Microsoft.Win32.RegistryKey [mscorlib]Microsoft.Win32.RegistryKey::CreateSubKey(string)
    IL_006a:  stloc.s    sequenceKey
    IL_006c:  ldloc.s    sequenceKey
    IL_006e:  ldstr      "name"
    IL_0073:  ldloc.2
    IL_0074:  ldfld      string Pololu.Usc.Sequencer.Sequence::name
    IL_0079:  ldc.i4.1
    IL_007a:  callvirt   instance void [mscorlib]Microsoft.Win32.RegistryKey::SetValue(string,
                                                                                       object,
                                                                                       valuetype [mscorlib]Microsoft.Win32.RegistryValueKind)
    IL_007f:  nop
    IL_0080:  ldc.i4.0
    IL_0081:  stloc.s    frameIndex
    IL_0083:  br.s       IL_0101

    IL_0085:  nop
    IL_0086:  ldloc.2
    IL_0087:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> Pololu.Usc.Sequencer.Sequence::frames
    IL_008c:  ldloc.s    frameIndex
    IL_008e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::get_Item(int32)
    IL_0093:  stloc.s    frame
    IL_0095:  ldloca.s   frameIndex
    IL_0097:  ldstr      "d4"
    IL_009c:  call       instance string [mscorlib]System.Int32::ToString(string)
    IL_00a1:  stloc.s    frameKeyName
    IL_00a3:  ldloc.s    sequenceKey
    IL_00a5:  ldloc.s    frameKeyName
    IL_00a7:  callvirt   instance class [mscorlib]Microsoft.Win32.RegistryKey [mscorlib]Microsoft.Win32.RegistryKey::CreateSubKey(string)
    IL_00ac:  stloc.s    frameKey
    IL_00ae:  ldloc.s    frameKey
    IL_00b0:  ldstr      "name"
    IL_00b5:  ldloc.s    frame
    IL_00b7:  ldfld      string Pololu.Usc.Sequencer.Frame::name
    IL_00bc:  ldc.i4.1
    IL_00bd:  callvirt   instance void [mscorlib]Microsoft.Win32.RegistryKey::SetValue(string,
                                                                                       object,
                                                                                       valuetype [mscorlib]Microsoft.Win32.RegistryValueKind)
    IL_00c2:  nop
    IL_00c3:  ldloc.s    frameKey
    IL_00c5:  ldstr      "duration"
    IL_00ca:  ldloc.s    frame
    IL_00cc:  ldfld      uint16 Pololu.Usc.Sequencer.Frame::length_ms
    IL_00d1:  box        [mscorlib]System.Int32
    IL_00d6:  ldc.i4.4
    IL_00d7:  callvirt   instance void [mscorlib]Microsoft.Win32.RegistryKey::SetValue(string,
                                                                                       object,
                                                                                       valuetype [mscorlib]Microsoft.Win32.RegistryValueKind)
    IL_00dc:  nop
    IL_00dd:  ldloc.s    frameKey
    IL_00df:  ldstr      "targets"
    IL_00e4:  ldloc.s    frame
    IL_00e6:  callvirt   instance string Pololu.Usc.Sequencer.Frame::getTargetsString()
    IL_00eb:  ldc.i4.1
    IL_00ec:  callvirt   instance void [mscorlib]Microsoft.Win32.RegistryKey::SetValue(string,
                                                                                       object,
                                                                                       valuetype [mscorlib]Microsoft.Win32.RegistryValueKind)
    IL_00f1:  nop
    IL_00f2:  ldloc.s    frameKey
    IL_00f4:  callvirt   instance void [mscorlib]Microsoft.Win32.RegistryKey::Close()
    IL_00f9:  nop
    IL_00fa:  nop
    IL_00fb:  ldloc.s    frameIndex
    IL_00fd:  ldc.i4.1
    IL_00fe:  add
    IL_00ff:  stloc.s    frameIndex
    IL_0101:  ldloc.s    frameIndex
    IL_0103:  ldloc.2
    IL_0104:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> Pololu.Usc.Sequencer.Sequence::frames
    IL_0109:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::get_Count()
    IL_010e:  clt
    IL_0110:  stloc.s    CS$4$0000
    IL_0112:  ldloc.s    CS$4$0000
    IL_0114:  brtrue     IL_0085

    IL_0119:  ldloc.s    sequenceKey
    IL_011b:  callvirt   instance void [mscorlib]Microsoft.Win32.RegistryKey::Close()
    IL_0120:  nop
    IL_0121:  nop
    IL_0122:  ldloc.1
    IL_0123:  ldc.i4.1
    IL_0124:  add
    IL_0125:  stloc.1
    IL_0126:  ldloc.1
    IL_0127:  ldarg.0
    IL_0128:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class Pololu.Usc.Sequencer.Sequence>::get_Count()
    IL_012d:  clt
    IL_012f:  stloc.s    CS$4$0000
    IL_0131:  ldloc.s    CS$4$0000
    IL_0133:  brtrue     IL_004d

    IL_0138:  ldloc.0
    IL_0139:  callvirt   instance void [mscorlib]Microsoft.Win32.RegistryKey::Close()
    IL_013e:  nop
    IL_013f:  ret
  } // end of method Sequence::saveSequencesInRegistry

  .method public hidebysig static class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Sequence> 
          readSequencesFromRegistry(class [mscorlib]Microsoft.Win32.RegistryKey parentKey,
                                    uint8 servoCount) cil managed
  {
    // Code size       510 (0x1fe)
    .maxstack  4
    .locals init ([0] class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Sequence> sequences,
             [1] class [mscorlib]Microsoft.Win32.RegistryKey sequencesKey,
             [2] class Pololu.Usc.Sequencer.Sequence/FrameKeyNameComparer fknc,
             [3] string sequenceKeyName,
             [4] class [mscorlib]Microsoft.Win32.RegistryKey sequenceKey,
             [5] string sequenceName,
             [6] class Pololu.Usc.Sequencer.Sequence sequence,
             [7] class [mscorlib]System.Collections.Generic.List`1<string> frameKeyNames,
             [8] class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> frames,
             [9] string frameKeyName,
             [10] class [mscorlib]Microsoft.Win32.RegistryKey frameKey,
             [11] class Pololu.Usc.Sequencer.Frame frame,
             [12] valuetype [mscorlib]System.Nullable`1<int32> length_ms,
             [13] class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Sequence> CS$1$0000,
             [14] bool CS$4$0001,
             [15] string[] CS$6$0002,
             [16] int32 CS$7$0003,
             [17] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string> CS$5$0004)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Sequence>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldarg.0
    IL_0008:  ldstr      "sequences"
    IL_000d:  callvirt   instance class [mscorlib]Microsoft.Win32.RegistryKey [mscorlib]Microsoft.Win32.RegistryKey::OpenSubKey(string)
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  ldnull
    IL_0015:  ceq
    IL_0017:  ldc.i4.0
    IL_0018:  ceq
    IL_001a:  stloc.s    CS$4$0001
    IL_001c:  ldloc.s    CS$4$0001
    IL_001e:  brtrue.s   IL_0028

    IL_0020:  ldloc.0
    IL_0021:  stloc.s    CS$1$0000
    IL_0023:  br         IL_01fb

    IL_0028:  newobj     instance void Pololu.Usc.Sequencer.Sequence/FrameKeyNameComparer::.ctor()
    IL_002d:  stloc.2
    IL_002e:  ldloc.1
    IL_002f:  ldnull
    IL_0030:  ceq
    IL_0032:  ldc.i4.0
    IL_0033:  ceq
    IL_0035:  stloc.s    CS$4$0001
    IL_0037:  ldloc.s    CS$4$0001
    IL_0039:  brtrue.s   IL_0043

    IL_003b:  ldloc.0
    IL_003c:  stloc.s    CS$1$0000
    IL_003e:  br         IL_01fb

    IL_0043:  nop
    IL_0044:  ldloc.1
    IL_0045:  callvirt   instance string[] [mscorlib]Microsoft.Win32.RegistryKey::GetSubKeyNames()
    IL_004a:  stloc.s    CS$6$0002
    IL_004c:  ldc.i4.0
    IL_004d:  stloc.s    CS$7$0003
    IL_004f:  br         IL_01e5

    IL_0054:  ldloc.s    CS$6$0002
    IL_0056:  ldloc.s    CS$7$0003
    IL_0058:  ldelem.ref
    IL_0059:  stloc.3
    IL_005a:  nop
    IL_005b:  ldloc.1
    IL_005c:  ldloc.3
    IL_005d:  callvirt   instance class [mscorlib]Microsoft.Win32.RegistryKey [mscorlib]Microsoft.Win32.RegistryKey::OpenSubKey(string)
    IL_0062:  stloc.s    sequenceKey
    IL_0064:  ldloc.s    sequenceKey
    IL_0066:  ldstr      "name"
    IL_006b:  ldnull
    IL_006c:  callvirt   instance object [mscorlib]Microsoft.Win32.RegistryKey::GetValue(string,
                                                                                         object)
    IL_0071:  isinst     [mscorlib]System.String
    IL_0076:  stloc.s    sequenceName
    IL_0078:  ldloc.s    sequenceName
    IL_007a:  ldnull
    IL_007b:  ceq
    IL_007d:  ldc.i4.0
    IL_007e:  ceq
    IL_0080:  stloc.s    CS$4$0001
    IL_0082:  ldloc.s    CS$4$0001
    IL_0084:  brtrue.s   IL_0095

    IL_0086:  nop
    IL_0087:  ldstr      "Sequence "
    IL_008c:  ldloc.3
    IL_008d:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0092:  stloc.s    sequenceName
    IL_0094:  nop
    IL_0095:  ldloc.s    sequenceName
    IL_0097:  newobj     instance void Pololu.Usc.Sequencer.Sequence::.ctor(string)
    IL_009c:  stloc.s    sequence
    IL_009e:  ldloc.s    sequenceKey
    IL_00a0:  callvirt   instance string[] [mscorlib]Microsoft.Win32.RegistryKey::GetSubKeyNames()
    IL_00a5:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<string>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_00aa:  stloc.s    frameKeyNames
    IL_00ac:  ldloc.s    frameKeyNames
    IL_00ae:  ldloc.2
    IL_00af:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Sort(class [mscorlib]System.Collections.Generic.IComparer`1<!0>)
    IL_00b4:  nop
    IL_00b5:  ldloc.s    sequenceKey
    IL_00b7:  callvirt   instance int32 [mscorlib]Microsoft.Win32.RegistryKey::get_SubKeyCount()
    IL_00bc:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::.ctor(int32)
    IL_00c1:  stloc.s    frames
    IL_00c3:  nop
    IL_00c4:  ldloc.s    frameKeyNames
    IL_00c6:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<string>::GetEnumerator()
    IL_00cb:  stloc.s    CS$5$0004
    .try
    {
      IL_00cd:  br         IL_01aa

      IL_00d2:  ldloca.s   CS$5$0004
      IL_00d4:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::get_Current()
      IL_00d9:  stloc.s    frameKeyName
      IL_00db:  nop
      IL_00dc:  ldloc.s    sequenceKey
      IL_00de:  ldloc.s    frameKeyName
      IL_00e0:  callvirt   instance class [mscorlib]Microsoft.Win32.RegistryKey [mscorlib]Microsoft.Win32.RegistryKey::OpenSubKey(string)
      IL_00e5:  stloc.s    frameKey
      IL_00e7:  ldloc.s    frameKey
      IL_00e9:  ldnull
      IL_00ea:  ceq
      IL_00ec:  ldc.i4.0
      IL_00ed:  ceq
      IL_00ef:  stloc.s    CS$4$0001
      IL_00f1:  ldloc.s    CS$4$0001
      IL_00f3:  brtrue.s   IL_00fa

      IL_00f5:  br         IL_01aa

      IL_00fa:  newobj     instance void Pololu.Usc.Sequencer.Frame::.ctor()
      IL_00ff:  stloc.s    frame
      IL_0101:  ldloc.s    frame
      IL_0103:  ldloc.s    frameKey
      IL_0105:  ldstr      "name"
      IL_010a:  ldnull
      IL_010b:  callvirt   instance object [mscorlib]Microsoft.Win32.RegistryKey::GetValue(string,
                                                                                           object)
      IL_0110:  isinst     [mscorlib]System.String
      IL_0115:  stfld      string Pololu.Usc.Sequencer.Frame::name
      IL_011a:  ldloc.s    frame
      IL_011c:  ldfld      string Pololu.Usc.Sequencer.Frame::name
      IL_0121:  ldnull
      IL_0122:  ceq
      IL_0124:  ldc.i4.0
      IL_0125:  ceq
      IL_0127:  stloc.s    CS$4$0001
      IL_0129:  ldloc.s    CS$4$0001
      IL_012b:  brtrue.s   IL_0142

      IL_012d:  nop
      IL_012e:  ldloc.s    frame
      IL_0130:  ldstr      "Frame "
      IL_0135:  ldloc.s    frameKeyName
      IL_0137:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_013c:  stfld      string Pololu.Usc.Sequencer.Frame::name
      IL_0141:  nop
      IL_0142:  ldloc.s    frameKey
      IL_0144:  ldstr      "duration"
      IL_0149:  ldstr      ""
      IL_014e:  callvirt   instance object [mscorlib]Microsoft.Win32.RegistryKey::GetValue(string,
                                                                                           object)
      IL_0153:  isinst     valuetype [mscorlib]System.Nullable`1<int32>
      IL_0158:  unbox.any  valuetype [mscorlib]System.Nullable`1<int32>
      IL_015d:  stloc.s    length_ms
      IL_015f:  ldloca.s   length_ms
      IL_0161:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
      IL_0166:  ldc.i4.0
      IL_0167:  ceq
      IL_0169:  stloc.s    CS$4$0001
      IL_016b:  ldloc.s    CS$4$0001
      IL_016d:  brtrue.s   IL_0180

      IL_016f:  nop
      IL_0170:  ldloc.s    frame
      IL_0172:  ldloca.s   length_ms
      IL_0174:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::get_Value()
      IL_0179:  conv.u2
      IL_017a:  stfld      uint16 Pololu.Usc.Sequencer.Frame::length_ms
      IL_017f:  nop
      IL_0180:  ldloc.s    frame
      IL_0182:  ldloc.s    frameKey
      IL_0184:  ldstr      "targets"
      IL_0189:  ldstr      ""
      IL_018e:  callvirt   instance object [mscorlib]Microsoft.Win32.RegistryKey::GetValue(string,
                                                                                           object)
      IL_0193:  isinst     [mscorlib]System.String
      IL_0198:  ldarg.1
      IL_0199:  callvirt   instance void Pololu.Usc.Sequencer.Frame::setTargetsFromString(string,
                                                                                          uint8)
      IL_019e:  nop
      IL_019f:  ldloc.s    frames
      IL_01a1:  ldloc.s    frame
      IL_01a3:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::Add(!0)
      IL_01a8:  nop
      IL_01a9:  nop
      IL_01aa:  ldloca.s   CS$5$0004
      IL_01ac:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::MoveNext()
      IL_01b1:  stloc.s    CS$4$0001
      IL_01b3:  ldloc.s    CS$4$0001
      IL_01b5:  brtrue     IL_00d2

      IL_01ba:  leave.s    IL_01cb

    }  // end .try
    finally
    {
      IL_01bc:  ldloca.s   CS$5$0004
      IL_01be:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>
      IL_01c4:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_01c9:  nop
      IL_01ca:  endfinally
    }  // end handler
    IL_01cb:  nop
    IL_01cc:  ldloc.s    sequence
    IL_01ce:  ldloc.s    frames
    IL_01d0:  stfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> Pololu.Usc.Sequencer.Sequence::frames
    IL_01d5:  ldloc.0
    IL_01d6:  ldloc.s    sequence
    IL_01d8:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Sequence>::Add(!0)
    IL_01dd:  nop
    IL_01de:  nop
    IL_01df:  ldloc.s    CS$7$0003
    IL_01e1:  ldc.i4.1
    IL_01e2:  add
    IL_01e3:  stloc.s    CS$7$0003
    IL_01e5:  ldloc.s    CS$7$0003
    IL_01e7:  ldloc.s    CS$6$0002
    IL_01e9:  ldlen
    IL_01ea:  conv.i4
    IL_01eb:  clt
    IL_01ed:  stloc.s    CS$4$0001
    IL_01ef:  ldloc.s    CS$4$0001
    IL_01f1:  brtrue     IL_0054

    IL_01f6:  ldloc.0
    IL_01f7:  stloc.s    CS$1$0000
    IL_01f9:  br.s       IL_01fb

    IL_01fb:  ldloc.s    CS$1$0000
    IL_01fd:  ret
  } // end of method Sequence::readSequencesFromRegistry

  .method private hidebysig instance string 
          generateScript(class [mscorlib]System.Collections.Generic.List`1<uint8> enabled_channels,
                         class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>> needed_channel_lists) cil managed
  {
    // Code size       610 (0x262)
    .maxstack  4
    .locals init ([0] string script,
             [1] class Pololu.Usc.Sequencer.Frame last_frame,
             [2] class Pololu.Usc.Sequencer.Frame frame,
             [3] class [mscorlib]System.Collections.Generic.List`1<uint8> needed_channels,
             [4] class [mscorlib]System.Collections.Generic.List`1<uint16> changed_targets,
             [5] uint8 channel,
             [6] bool found,
             [7] class [mscorlib]System.Collections.Generic.List`1<uint8> existing_list,
             [8] int32 i,
             [9] uint8 targetsOnThisLine,
             [10] uint16 target,
             [11] string CS$1$0000,
             [12] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Sequencer.Frame> CS$5$0001,
             [13] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<uint8> CS$5$0002,
             [14] bool CS$4$0003,
             [15] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [mscorlib]System.Collections.Generic.List`1<uint8>> CS$5$0004,
             [16] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<uint16> CS$5$0005)
    IL_0000:  nop
    IL_0001:  ldstr      ""
    IL_0006:  stloc.0
    IL_0007:  ldnull
    IL_0008:  stloc.1
    IL_0009:  nop
    IL_000a:  ldarg.0
    IL_000b:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame> Pololu.Usc.Sequencer.Sequence::frames
    IL_0010:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Frame>::GetEnumerator()
    IL_0015:  stloc.s    CS$5$0001
    .try
    {
      IL_0017:  br         IL_0238

      IL_001c:  ldloca.s   CS$5$0001
      IL_001e:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Sequencer.Frame>::get_Current()
      IL_0023:  stloc.2
      IL_0024:  nop
      IL_0025:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::.ctor()
      IL_002a:  stloc.3
      IL_002b:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<uint16>::.ctor()
      IL_0030:  stloc.s    changed_targets
      IL_0032:  nop
      IL_0033:  ldarg.1
      IL_0034:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<uint8>::GetEnumerator()
      IL_0039:  stloc.s    CS$5$0002
      .try
      {
        IL_003b:  br.s       IL_0081

        IL_003d:  ldloca.s   CS$5$0002
        IL_003f:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<uint8>::get_Current()
        IL_0044:  stloc.s    channel
        IL_0046:  nop
        IL_0047:  ldloc.1
        IL_0048:  brfalse.s  IL_005e

        IL_004a:  ldloc.2
        IL_004b:  ldloc.s    channel
        IL_004d:  callvirt   instance uint16 Pololu.Usc.Sequencer.Frame::get_target(int32)
        IL_0052:  ldloc.1
        IL_0053:  ldloc.s    channel
        IL_0055:  callvirt   instance uint16 Pololu.Usc.Sequencer.Frame::get_target(int32)
        IL_005a:  ceq
        IL_005c:  br.s       IL_005f

        IL_005e:  ldc.i4.0
        IL_005f:  stloc.s    CS$4$0003
        IL_0061:  ldloc.s    CS$4$0003
        IL_0063:  brtrue.s   IL_0080

        IL_0065:  nop
        IL_0066:  ldloc.3
        IL_0067:  ldloc.s    channel
        IL_0069:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
        IL_006e:  nop
        IL_006f:  ldloc.s    changed_targets
        IL_0071:  ldloc.2
        IL_0072:  ldloc.s    channel
        IL_0074:  callvirt   instance uint16 Pololu.Usc.Sequencer.Frame::get_target(int32)
        IL_0079:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint16>::Add(!0)
        IL_007e:  nop
        IL_007f:  nop
        IL_0080:  nop
        IL_0081:  ldloca.s   CS$5$0002
        IL_0083:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<uint8>::MoveNext()
        IL_0088:  stloc.s    CS$4$0003
        IL_008a:  ldloc.s    CS$4$0003
        IL_008c:  brtrue.s   IL_003d

        IL_008e:  leave.s    IL_009f

      }  // end .try
      finally
      {
        IL_0090:  ldloca.s   CS$5$0002
        IL_0092:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<uint8>
        IL_0098:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_009d:  nop
        IL_009e:  endfinally
      }  // end handler
      IL_009f:  nop
      IL_00a0:  ldloc.2
      IL_00a1:  stloc.1
      IL_00a2:  ldloc.s    changed_targets
      IL_00a4:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint16>::get_Count()
      IL_00a9:  ldc.i4.0
      IL_00aa:  ceq
      IL_00ac:  stloc.s    CS$4$0003
      IL_00ae:  ldloc.s    CS$4$0003
      IL_00b0:  brtrue     IL_0157

      IL_00b5:  nop
      IL_00b6:  ldc.i4.0
      IL_00b7:  stloc.s    found
      IL_00b9:  nop
      IL_00ba:  ldarg.2
      IL_00bb:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>>::GetEnumerator()
      IL_00c0:  stloc.s    CS$5$0004
      .try
      {
        IL_00c2:  br.s       IL_0125

        IL_00c4:  ldloca.s   CS$5$0004
        IL_00c6:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [mscorlib]System.Collections.Generic.List`1<uint8>>::get_Current()
        IL_00cb:  stloc.s    existing_list
        IL_00cd:  nop
        IL_00ce:  ldloc.s    existing_list
        IL_00d0:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
        IL_00d5:  ldloc.3
        IL_00d6:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
        IL_00db:  ceq
        IL_00dd:  stloc.s    CS$4$0003
        IL_00df:  ldloc.s    CS$4$0003
        IL_00e1:  brtrue.s   IL_00e5

        IL_00e3:  br.s       IL_0125

        IL_00e5:  ldc.i4.0
        IL_00e6:  stloc.s    i
        IL_00e8:  br.s       IL_010d

        IL_00ea:  nop
        IL_00eb:  ldloc.s    existing_list
        IL_00ed:  ldloc.s    i
        IL_00ef:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Item(int32)
        IL_00f4:  ldloc.3
        IL_00f5:  ldloc.s    i
        IL_00f7:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Item(int32)
        IL_00fc:  ceq
        IL_00fe:  stloc.s    CS$4$0003
        IL_0100:  ldloc.s    CS$4$0003
        IL_0102:  brtrue.s   IL_0106

        IL_0104:  br.s       IL_0123

        IL_0106:  nop
        IL_0107:  ldloc.s    i
        IL_0109:  ldc.i4.1
        IL_010a:  add
        IL_010b:  stloc.s    i
        IL_010d:  ldloc.s    i
        IL_010f:  ldloc.s    existing_list
        IL_0111:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
        IL_0116:  clt
        IL_0118:  stloc.s    CS$4$0003
        IL_011a:  ldloc.s    CS$4$0003
        IL_011c:  brtrue.s   IL_00ea

        IL_011e:  ldc.i4.1
        IL_011f:  stloc.s    found
        IL_0121:  br.s       IL_0132

        IL_0123:  br.s       IL_0125

        IL_0125:  ldloca.s   CS$5$0004
        IL_0127:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [mscorlib]System.Collections.Generic.List`1<uint8>>::MoveNext()
        IL_012c:  stloc.s    CS$4$0003
        IL_012e:  ldloc.s    CS$4$0003
        IL_0130:  brtrue.s   IL_00c4

        IL_0132:  leave.s    IL_0143

      }  // end .try
      finally
      {
        IL_0134:  ldloca.s   CS$5$0004
        IL_0136:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [mscorlib]System.Collections.Generic.List`1<uint8>>
        IL_013c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0141:  nop
        IL_0142:  endfinally
      }  // end handler
      IL_0143:  nop
      IL_0144:  ldloc.s    found
      IL_0146:  stloc.s    CS$4$0003
      IL_0148:  ldloc.s    CS$4$0003
      IL_014a:  brtrue.s   IL_0156

      IL_014c:  nop
      IL_014d:  ldarg.2
      IL_014e:  ldloc.3
      IL_014f:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>>::Add(!0)
      IL_0154:  nop
      IL_0155:  nop
      IL_0156:  nop
      IL_0157:  ldloc.0
      IL_0158:  ldstr      "  "
      IL_015d:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0162:  stloc.0
      IL_0163:  ldloc.0
      IL_0164:  ldloc.2
      IL_0165:  ldfld      uint16 Pololu.Usc.Sequencer.Frame::length_ms
      IL_016a:  box        [mscorlib]System.UInt16
      IL_016f:  ldstr      " "
      IL_0174:  call       string [mscorlib]System.String::Concat(object,
                                                                  object,
                                                                  object)
      IL_0179:  stloc.0
      IL_017a:  ldloc.3
      IL_017b:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
      IL_0180:  ldc.i4.0
      IL_0181:  ceq
      IL_0183:  ldc.i4.0
      IL_0184:  ceq
      IL_0186:  stloc.s    CS$4$0003
      IL_0188:  ldloc.s    CS$4$0003
      IL_018a:  brtrue.s   IL_019f

      IL_018c:  nop
      IL_018d:  ldloc.0
      IL_018e:  ldstr      "delay"
      IL_0193:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0198:  stloc.0
      IL_0199:  nop
      IL_019a:  br         IL_0220

      IL_019f:  nop
      IL_01a0:  ldc.i4.0
      IL_01a1:  stloc.s    targetsOnThisLine
      IL_01a3:  nop
      IL_01a4:  ldloc.s    changed_targets
      IL_01a6:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<uint16>::GetEnumerator()
      IL_01ab:  stloc.s    CS$5$0005
      .try
      {
        IL_01ad:  br.s       IL_01f3

        IL_01af:  ldloca.s   CS$5$0005
        IL_01b1:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<uint16>::get_Current()
        IL_01b6:  stloc.s    target
        IL_01b8:  nop
        IL_01b9:  ldloc.s    targetsOnThisLine
        IL_01bb:  ldc.i4.6
        IL_01bc:  ceq
        IL_01be:  ldc.i4.0
        IL_01bf:  ceq
        IL_01c1:  stloc.s    CS$4$0003
        IL_01c3:  ldloc.s    CS$4$0003
        IL_01c5:  brtrue.s   IL_01d8

        IL_01c7:  nop
        IL_01c8:  ldloc.0
        IL_01c9:  ldstr      "\n  "
        IL_01ce:  call       string [mscorlib]System.String::Concat(string,
                                                                    string)
        IL_01d3:  stloc.0
        IL_01d4:  ldc.i4.0
        IL_01d5:  stloc.s    targetsOnThisLine
        IL_01d7:  nop
        IL_01d8:  ldloc.s    targetsOnThisLine
        IL_01da:  ldc.i4.1
        IL_01db:  add
        IL_01dc:  conv.u1
        IL_01dd:  stloc.s    targetsOnThisLine
        IL_01df:  ldloc.0
        IL_01e0:  ldloc.s    target
        IL_01e2:  box        [mscorlib]System.UInt16
        IL_01e7:  ldstr      " "
        IL_01ec:  call       string [mscorlib]System.String::Concat(object,
                                                                    object,
                                                                    object)
        IL_01f1:  stloc.0
        IL_01f2:  nop
        IL_01f3:  ldloca.s   CS$5$0005
        IL_01f5:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<uint16>::MoveNext()
        IL_01fa:  stloc.s    CS$4$0003
        IL_01fc:  ldloc.s    CS$4$0003
        IL_01fe:  brtrue.s   IL_01af

        IL_0200:  leave.s    IL_0211

      }  // end .try
      finally
      {
        IL_0202:  ldloca.s   CS$5$0005
        IL_0204:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<uint16>
        IL_020a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_020f:  nop
        IL_0210:  endfinally
      }  // end handler
      IL_0211:  nop
      IL_0212:  ldloc.0
      IL_0213:  ldloc.3
      IL_0214:  call       string Pololu.Usc.Sequencer.Sequence::getFrameSubroutineName(class [mscorlib]System.Collections.Generic.List`1<uint8>)
      IL_0219:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_021e:  stloc.0
      IL_021f:  nop
      IL_0220:  ldloc.0
      IL_0221:  ldstr      " # "
      IL_0226:  ldloc.2
      IL_0227:  ldfld      string Pololu.Usc.Sequencer.Frame::name
      IL_022c:  ldstr      "\n"
      IL_0231:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string,
                                                                  string)
      IL_0236:  stloc.0
      IL_0237:  nop
      IL_0238:  ldloca.s   CS$5$0001
      IL_023a:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Sequencer.Frame>::MoveNext()
      IL_023f:  stloc.s    CS$4$0003
      IL_0241:  ldloc.s    CS$4$0003
      IL_0243:  brtrue     IL_001c

      IL_0248:  leave.s    IL_0259

    }  // end .try
    finally
    {
      IL_024a:  ldloca.s   CS$5$0001
      IL_024c:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Sequencer.Frame>
      IL_0252:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0257:  nop
      IL_0258:  endfinally
    }  // end handler
    IL_0259:  nop
    IL_025a:  ldloc.0
    IL_025b:  stloc.s    CS$1$0000
    IL_025d:  br.s       IL_025f

    IL_025f:  ldloc.s    CS$1$0000
    IL_0261:  ret
  } // end of method Sequence::generateScript

  .method public hidebysig static string 
          getFrameSubroutineName(class [mscorlib]System.Collections.Generic.List`1<uint8> channels) cil managed
  {
    // Code size       330 (0x14a)
    .maxstack  3
    .locals init ([0] string name,
             [1] int32 index,
             [2] int32 blockEnd,
             [3] uint8 startChannel,
             [4] uint8 endChannel,
             [5] string CS$1$0000,
             [6] bool CS$4$0001,
             [7] object CS$0$0002,
             [8] object[] CS$0$0003)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  ldc.i4.0
    IL_000b:  ceq
    IL_000d:  stloc.s    CS$4$0001
    IL_000f:  ldloc.s    CS$4$0001
    IL_0011:  brtrue.s   IL_001f

    IL_0013:  nop
    IL_0014:  ldstr      "getFrameSubroutineName: Expected channels list to "
    + "be non-empty."
    IL_0019:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_001e:  throw

    IL_001f:  ldstr      "frame_"
    IL_0024:  stloc.0
    IL_0025:  ldc.i4.0
    IL_0026:  stloc.1
    IL_0027:  br         IL_013f

    IL_002c:  nop
    IL_002d:  ldloc.1
    IL_002e:  ldc.i4.1
    IL_002f:  add
    IL_0030:  stloc.2
    IL_0031:  br.s       IL_0039

    IL_0033:  nop
    IL_0034:  ldloc.2
    IL_0035:  ldc.i4.1
    IL_0036:  add
    IL_0037:  stloc.2
    IL_0038:  nop
    IL_0039:  ldloc.2
    IL_003a:  ldarg.0
    IL_003b:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
    IL_0040:  bge.s      IL_0058

    IL_0042:  ldarg.0
    IL_0043:  ldloc.2
    IL_0044:  ldc.i4.1
    IL_0045:  sub
    IL_0046:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Item(int32)
    IL_004b:  ldc.i4.1
    IL_004c:  add
    IL_004d:  ldarg.0
    IL_004e:  ldloc.2
    IL_004f:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Item(int32)
    IL_0054:  ceq
    IL_0056:  br.s       IL_0059

    IL_0058:  ldc.i4.0
    IL_0059:  stloc.s    CS$4$0001
    IL_005b:  ldloc.s    CS$4$0001
    IL_005d:  brtrue.s   IL_0033

    IL_005f:  ldarg.0
    IL_0060:  ldloc.1
    IL_0061:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Item(int32)
    IL_0066:  stloc.3
    IL_0067:  ldarg.0
    IL_0068:  ldloc.2
    IL_0069:  ldc.i4.1
    IL_006a:  sub
    IL_006b:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Item(int32)
    IL_0070:  stloc.s    endChannel
    IL_0072:  ldloc.s    endChannel
    IL_0074:  ldloc.3
    IL_0075:  ceq
    IL_0077:  ldc.i4.0
    IL_0078:  ceq
    IL_007a:  stloc.s    CS$4$0001
    IL_007c:  ldloc.s    CS$4$0001
    IL_007e:  brtrue.s   IL_0094

    IL_0080:  nop
    IL_0081:  ldloc.0
    IL_0082:  ldloc.3
    IL_0083:  box        [mscorlib]System.Byte
    IL_0088:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_008d:  stloc.0
    IL_008e:  nop
    IL_008f:  br         IL_0118

    IL_0094:  ldloc.s    endChannel
    IL_0096:  ldloc.3
    IL_0097:  ldc.i4.1
    IL_0098:  add
    IL_0099:  ceq
    IL_009b:  ldc.i4.0
    IL_009c:  ceq
    IL_009e:  stloc.s    CS$4$0001
    IL_00a0:  ldloc.s    CS$4$0001
    IL_00a2:  brtrue.s   IL_00df

    IL_00a4:  nop
    IL_00a5:  ldloc.0
    IL_00a6:  stloc.s    CS$0$0002
    IL_00a8:  ldc.i4.4
    IL_00a9:  newarr     [mscorlib]System.Object
    IL_00ae:  stloc.s    CS$0$0003
    IL_00b0:  ldloc.s    CS$0$0003
    IL_00b2:  ldc.i4.0
    IL_00b3:  ldloc.s    CS$0$0002
    IL_00b5:  stelem.ref
    IL_00b6:  ldloc.s    CS$0$0003
    IL_00b8:  ldc.i4.1
    IL_00b9:  ldloc.3
    IL_00ba:  box        [mscorlib]System.Byte
    IL_00bf:  stelem.ref
    IL_00c0:  ldloc.s    CS$0$0003
    IL_00c2:  ldc.i4.2
    IL_00c3:  ldstr      "_"
    IL_00c8:  stelem.ref
    IL_00c9:  ldloc.s    CS$0$0003
    IL_00cb:  ldc.i4.3
    IL_00cc:  ldloc.s    endChannel
    IL_00ce:  box        [mscorlib]System.Byte
    IL_00d3:  stelem.ref
    IL_00d4:  ldloc.s    CS$0$0003
    IL_00d6:  call       string [mscorlib]System.String::Concat(object[])
    IL_00db:  stloc.0
    IL_00dc:  nop
    IL_00dd:  br.s       IL_0118

    IL_00df:  nop
    IL_00e0:  ldloc.0
    IL_00e1:  stloc.s    CS$0$0002
    IL_00e3:  ldc.i4.4
    IL_00e4:  newarr     [mscorlib]System.Object
    IL_00e9:  stloc.s    CS$0$0003
    IL_00eb:  ldloc.s    CS$0$0003
    IL_00ed:  ldc.i4.0
    IL_00ee:  ldloc.s    CS$0$0002
    IL_00f0:  stelem.ref
    IL_00f1:  ldloc.s    CS$0$0003
    IL_00f3:  ldc.i4.1
    IL_00f4:  ldloc.3
    IL_00f5:  box        [mscorlib]System.Byte
    IL_00fa:  stelem.ref
    IL_00fb:  ldloc.s    CS$0$0003
    IL_00fd:  ldc.i4.2
    IL_00fe:  ldstr      ".."
    IL_0103:  stelem.ref
    IL_0104:  ldloc.s    CS$0$0003
    IL_0106:  ldc.i4.3
    IL_0107:  ldloc.s    endChannel
    IL_0109:  box        [mscorlib]System.Byte
    IL_010e:  stelem.ref
    IL_010f:  ldloc.s    CS$0$0003
    IL_0111:  call       string [mscorlib]System.String::Concat(object[])
    IL_0116:  stloc.0
    IL_0117:  nop
    IL_0118:  ldloc.2
    IL_0119:  ldarg.0
    IL_011a:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
    IL_011f:  ceq
    IL_0121:  ldc.i4.0
    IL_0122:  ceq
    IL_0124:  stloc.s    CS$4$0001
    IL_0126:  ldloc.s    CS$4$0001
    IL_0128:  brtrue.s   IL_0130

    IL_012a:  nop
    IL_012b:  ldloc.0
    IL_012c:  stloc.s    CS$1$0000
    IL_012e:  br.s       IL_0147

    IL_0130:  ldloc.2
    IL_0131:  stloc.1
    IL_0132:  ldloc.0
    IL_0133:  ldstr      "_"
    IL_0138:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_013d:  stloc.0
    IL_013e:  nop
    IL_013f:  ldc.i4.1
    IL_0140:  stloc.s    CS$4$0001
    IL_0142:  br         IL_002c

    IL_0147:  ldloc.s    CS$1$0000
    IL_0149:  ret
  } // end of method Sequence::getFrameSubroutineName

  .method public hidebysig static string 
          generateFrameSubroutine(class [mscorlib]System.Collections.Generic.List`1<uint8> channels) cil managed
  {
    // Code size       115 (0x73)
    .maxstack  4
    .locals init ([0] string script,
             [1] int32 i,
             [2] string CS$1$0000,
             [3] uint8 CS$0$0001,
             [4] bool CS$4$0002)
    IL_0000:  nop
    IL_0001:  ldstr      "sub "
    IL_0006:  ldarg.0
    IL_0007:  call       string Pololu.Usc.Sequencer.Sequence::getFrameSubroutineName(class [mscorlib]System.Collections.Generic.List`1<uint8>)
    IL_000c:  ldstr      "\n"
    IL_0011:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0016:  stloc.0
    IL_0017:  ldarg.0
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
    IL_001d:  ldc.i4.1
    IL_001e:  sub
    IL_001f:  stloc.1
    IL_0020:  br.s       IL_0048

    IL_0022:  nop
    IL_0023:  ldloc.0
    IL_0024:  ldstr      "  "
    IL_0029:  ldarg.0
    IL_002a:  ldloc.1
    IL_002b:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Item(int32)
    IL_0030:  stloc.3
    IL_0031:  ldloca.s   CS$0$0001
    IL_0033:  call       instance string [mscorlib]System.Byte::ToString()
    IL_0038:  ldstr      " servo\n"
    IL_003d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string,
                                                                string)
    IL_0042:  stloc.0
    IL_0043:  nop
    IL_0044:  ldloc.1
    IL_0045:  ldc.i4.1
    IL_0046:  sub
    IL_0047:  stloc.1
    IL_0048:  ldloc.1
    IL_0049:  ldc.i4.0
    IL_004a:  clt
    IL_004c:  ldc.i4.0
    IL_004d:  ceq
    IL_004f:  stloc.s    CS$4$0002
    IL_0051:  ldloc.s    CS$4$0002
    IL_0053:  brtrue.s   IL_0022

    IL_0055:  ldloc.0
    IL_0056:  ldstr      "  delay\n"
    IL_005b:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0060:  stloc.0
    IL_0061:  ldloc.0
    IL_0062:  ldstr      "  return\n"
    IL_0067:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_006c:  stloc.0
    IL_006d:  ldloc.0
    IL_006e:  stloc.2
    IL_006f:  br.s       IL_0071

    IL_0071:  ldloc.2
    IL_0072:  ret
  } // end of method Sequence::generateFrameSubroutine

  .method public hidebysig instance string 
          generateLoopedScript(class [mscorlib]System.Collections.Generic.List`1<uint8> enabled_channels) cil managed
  {
    // Code size       132 (0x84)
    .maxstack  4
    .locals init ([0] class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>> needed_channel_lists,
             [1] string script,
             [2] class [mscorlib]System.Collections.Generic.List`1<uint8> needed_channels,
             [3] string CS$1$0000,
             [4] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [mscorlib]System.Collections.Generic.List`1<uint8>> CS$5$0001,
             [5] bool CS$4$0002)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldstr      "# "
    IL_000c:  ldarg.0
    IL_000d:  ldfld      string Pololu.Usc.Sequencer.Sequence::name
    IL_0012:  ldstr      "\nbegin\n"
    IL_0017:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_001c:  stloc.1
    IL_001d:  ldloc.1
    IL_001e:  ldarg.0
    IL_001f:  ldarg.1
    IL_0020:  ldloc.0
    IL_0021:  call       instance string Pololu.Usc.Sequencer.Sequence::generateScript(class [mscorlib]System.Collections.Generic.List`1<uint8>,
                                                                                       class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>>)
    IL_0026:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_002b:  stloc.1
    IL_002c:  ldloc.1
    IL_002d:  ldstr      "repeat\n\n"
    IL_0032:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0037:  stloc.1
    IL_0038:  nop
    IL_0039:  ldloc.0
    IL_003a:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>>::GetEnumerator()
    IL_003f:  stloc.s    CS$5$0001
    .try
    {
      IL_0041:  br.s       IL_005f

      IL_0043:  ldloca.s   CS$5$0001
      IL_0045:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [mscorlib]System.Collections.Generic.List`1<uint8>>::get_Current()
      IL_004a:  stloc.2
      IL_004b:  nop
      IL_004c:  ldloc.1
      IL_004d:  ldloc.2
      IL_004e:  call       string Pololu.Usc.Sequencer.Sequence::generateFrameSubroutine(class [mscorlib]System.Collections.Generic.List`1<uint8>)
      IL_0053:  ldstr      "\n"
      IL_0058:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string)
      IL_005d:  stloc.1
      IL_005e:  nop
      IL_005f:  ldloca.s   CS$5$0001
      IL_0061:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [mscorlib]System.Collections.Generic.List`1<uint8>>::MoveNext()
      IL_0066:  stloc.s    CS$4$0002
      IL_0068:  ldloc.s    CS$4$0002
      IL_006a:  brtrue.s   IL_0043

      IL_006c:  leave.s    IL_007d

    }  // end .try
    finally
    {
      IL_006e:  ldloca.s   CS$5$0001
      IL_0070:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [mscorlib]System.Collections.Generic.List`1<uint8>>
      IL_0076:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_007b:  nop
      IL_007c:  endfinally
    }  // end handler
    IL_007d:  nop
    IL_007e:  ldloc.1
    IL_007f:  stloc.3
    IL_0080:  br.s       IL_0082

    IL_0082:  ldloc.3
    IL_0083:  ret
  } // end of method Sequence::generateLoopedScript

  .method public hidebysig instance string 
          generateSubroutine(class [mscorlib]System.Collections.Generic.List`1<uint8> enabled_channels,
                             class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>> needed_channel_lists) cil managed
  {
    // Code size       148 (0x94)
    .maxstack  4
    .locals init ([0] string nice_name,
             [1] class [System]System.Text.RegularExpressions.Regex exp,
             [2] string script,
             [3] string CS$1$0000,
             [4] string[] CS$0$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      string Pololu.Usc.Sequencer.Sequence::name
    IL_0007:  stloc.0
    IL_0008:  ldstr      "\\s+"
    IL_000d:  newobj     instance void [System]System.Text.RegularExpressions.Regex::.ctor(string)
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  ldloc.0
    IL_0015:  ldstr      "_"
    IL_001a:  callvirt   instance string [System]System.Text.RegularExpressions.Regex::Replace(string,
                                                                                               string)
    IL_001f:  stloc.0
    IL_0020:  ldstr      "[^a-z0-9_]"
    IL_0025:  ldc.i4.1
    IL_0026:  newobj     instance void [System]System.Text.RegularExpressions.Regex::.ctor(string,
                                                                                           valuetype [System]System.Text.RegularExpressions.RegexOptions)
    IL_002b:  stloc.1
    IL_002c:  ldloc.1
    IL_002d:  ldloc.0
    IL_002e:  ldstr      ""
    IL_0033:  callvirt   instance string [System]System.Text.RegularExpressions.Regex::Replace(string,
                                                                                               string)
    IL_0038:  stloc.0
    IL_0039:  ldc.i4.5
    IL_003a:  newarr     [mscorlib]System.String
    IL_003f:  stloc.s    CS$0$0001
    IL_0041:  ldloc.s    CS$0$0001
    IL_0043:  ldc.i4.0
    IL_0044:  ldstr      "# "
    IL_0049:  stelem.ref
    IL_004a:  ldloc.s    CS$0$0001
    IL_004c:  ldc.i4.1
    IL_004d:  ldarg.0
    IL_004e:  ldfld      string Pololu.Usc.Sequencer.Sequence::name
    IL_0053:  stelem.ref
    IL_0054:  ldloc.s    CS$0$0001
    IL_0056:  ldc.i4.2
    IL_0057:  ldstr      "\nsub "
    IL_005c:  stelem.ref
    IL_005d:  ldloc.s    CS$0$0001
    IL_005f:  ldc.i4.3
    IL_0060:  ldloc.0
    IL_0061:  stelem.ref
    IL_0062:  ldloc.s    CS$0$0001
    IL_0064:  ldc.i4.4
    IL_0065:  ldstr      "\n"
    IL_006a:  stelem.ref
    IL_006b:  ldloc.s    CS$0$0001
    IL_006d:  call       string [mscorlib]System.String::Concat(string[])
    IL_0072:  stloc.2
    IL_0073:  ldloc.2
    IL_0074:  ldarg.0
    IL_0075:  ldarg.1
    IL_0076:  ldarg.2
    IL_0077:  call       instance string Pololu.Usc.Sequencer.Sequence::generateScript(class [mscorlib]System.Collections.Generic.List`1<uint8>,
                                                                                       class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>>)
    IL_007c:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0081:  stloc.2
    IL_0082:  ldloc.2
    IL_0083:  ldstr      "  return\n"
    IL_0088:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_008d:  stloc.2
    IL_008e:  ldloc.2
    IL_008f:  stloc.3
    IL_0090:  br.s       IL_0092

    IL_0092:  ldloc.3
    IL_0093:  ret
  } // end of method Sequence::generateSubroutine

  .method public hidebysig static string 
          generateSubroutineList(class [mscorlib]System.Collections.Generic.List`1<uint8> enabled_channels,
                                 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Sequence> sequences) cil managed
  {
    // Code size       158 (0x9e)
    .maxstack  4
    .locals init ([0] class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>> needed_channel_lists,
             [1] string script,
             [2] class Pololu.Usc.Sequencer.Sequence sequence,
             [3] class [mscorlib]System.Collections.Generic.List`1<uint8> channel_list,
             [4] string CS$1$0000,
             [5] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Sequencer.Sequence> CS$5$0001,
             [6] bool CS$4$0002,
             [7] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [mscorlib]System.Collections.Generic.List`1<uint8>> CS$5$0003)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldstr      ""
    IL_000c:  stloc.1
    IL_000d:  nop
    IL_000e:  ldarg.1
    IL_000f:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Sequencer.Sequence>::GetEnumerator()
    IL_0014:  stloc.s    CS$5$0001
    .try
    {
      IL_0016:  br.s       IL_0031

      IL_0018:  ldloca.s   CS$5$0001
      IL_001a:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Sequencer.Sequence>::get_Current()
      IL_001f:  stloc.2
      IL_0020:  nop
      IL_0021:  ldloc.1
      IL_0022:  ldloc.2
      IL_0023:  ldarg.0
      IL_0024:  ldloc.0
      IL_0025:  callvirt   instance string Pololu.Usc.Sequencer.Sequence::generateSubroutine(class [mscorlib]System.Collections.Generic.List`1<uint8>,
                                                                                             class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>>)
      IL_002a:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_002f:  stloc.1
      IL_0030:  nop
      IL_0031:  ldloca.s   CS$5$0001
      IL_0033:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Sequencer.Sequence>::MoveNext()
      IL_0038:  stloc.s    CS$4$0002
      IL_003a:  ldloc.s    CS$4$0002
      IL_003c:  brtrue.s   IL_0018

      IL_003e:  leave.s    IL_004f

    }  // end .try
    finally
    {
      IL_0040:  ldloca.s   CS$5$0001
      IL_0042:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Sequencer.Sequence>
      IL_0048:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_004d:  nop
      IL_004e:  endfinally
    }  // end handler
    IL_004f:  nop
    IL_0050:  nop
    IL_0051:  ldloc.0
    IL_0052:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<uint8>>::GetEnumerator()
    IL_0057:  stloc.s    CS$5$0003
    .try
    {
      IL_0059:  br.s       IL_0077

      IL_005b:  ldloca.s   CS$5$0003
      IL_005d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [mscorlib]System.Collections.Generic.List`1<uint8>>::get_Current()
      IL_0062:  stloc.3
      IL_0063:  nop
      IL_0064:  ldloc.1
      IL_0065:  ldstr      "\n"
      IL_006a:  ldloc.3
      IL_006b:  call       string Pololu.Usc.Sequencer.Sequence::generateFrameSubroutine(class [mscorlib]System.Collections.Generic.List`1<uint8>)
      IL_0070:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string)
      IL_0075:  stloc.1
      IL_0076:  nop
      IL_0077:  ldloca.s   CS$5$0003
      IL_0079:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [mscorlib]System.Collections.Generic.List`1<uint8>>::MoveNext()
      IL_007e:  stloc.s    CS$4$0002
      IL_0080:  ldloc.s    CS$4$0002
      IL_0082:  brtrue.s   IL_005b

      IL_0084:  leave.s    IL_0095

    }  // end .try
    finally
    {
      IL_0086:  ldloca.s   CS$5$0003
      IL_0088:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [mscorlib]System.Collections.Generic.List`1<uint8>>
      IL_008e:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0093:  nop
      IL_0094:  endfinally
    }  // end handler
    IL_0095:  nop
    IL_0096:  ldloc.1
    IL_0097:  stloc.s    CS$1$0000
    IL_0099:  br.s       IL_009b

    IL_009b:  ldloc.s    CS$1$0000
    IL_009d:  ret
  } // end of method Sequence::generateSubroutineList

} // end of class Pololu.Usc.Sequencer.Sequence


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file Sequencer.res
