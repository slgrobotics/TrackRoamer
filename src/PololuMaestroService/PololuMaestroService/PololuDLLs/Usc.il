
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern UsbWrapper
{
  .publickeytoken = (35 29 79 22 70 4B 28 CD )
  .ver 1:5:0:0
}
.assembly extern System.Xml
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern Bytecode
{
  .publickeytoken = (35 29 79 22 70 4B 28 CD )
  .ver 1:1:4576:23127
}
.assembly extern Sequencer
{
  .publickeytoken = (35 29 79 22 70 4B 28 CD )
  .ver 1:2:4582:40579
}
.assembly extern mscorlib as mscorlib_5
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly Usc
{
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 03 55 73 63 00 00 )                         // ...Usc..
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 1A 2E 4E 45 54 46 72 61 6D 65 77 6F 72 6B   // ....NETFramework
                                                                                                        2C 56 65 72 73 69 6F 6E 3D 76 34 2E 30 01 00 54   // ,Version=v4.0..T
                                                                                                        0E 14 46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C   // ..FrameworkDispl
                                                                                                        61 79 4E 61 6D 65 10 2E 4E 45 54 20 46 72 61 6D   // ayName..NET Fram
                                                                                                        65 77 6F 72 6B 20 34 )                            // ework 4
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 06 50 6F 6C 6F 6C 75 00 00 )                // ...Pololu..
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 03 55 73 63 00 00 )                         // ...Usc..
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 1D 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. 
                                                                                                  50 6F 6C 6F 6C 75 20 32 30 30 39 2D 32 30 31 31   // Pololu 2009-2011
                                                                                                  00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 66 65 34 36 63 64 37 66 2D 63 39 34 65   // ..$fe46cd7f-c94e
                                                                                                  2D 34 34 39 66 2D 38 36 64 34 2D 63 66 66 34 32   // -449f-86d4-cff42
                                                                                                  38 61 34 35 32 33 34 00 00 )                      // 8a45234..
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 33 2E 30 2E 30 00 00 )             // ...1.3.0.0..
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 1:3:0:0
}
.module Usc.dll
// MVID: {3600D2BD-7367-489C-997C-DF4745537D41}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00810000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit Pololu.Usc.ConfigurationFile
       extends [mscorlib]System.Object
{
  .method public hidebysig static class Pololu.Usc.UscSettings 
          load(class [mscorlib]System.IO.StreamReader sr,
               class [mscorlib]System.Collections.Generic.List`1<string> warnings) cil managed
  {
    // Code size       2779 (0xadb)
    .maxstack  5
    .locals init ([0] class [System.Xml]System.Xml.XmlReader reader,
             [1] class Pololu.Usc.UscSettings settings,
             [2] string script,
             [3] class [mscorlib]System.Collections.Generic.Dictionary`2<string,string> xParams,
             [4] class [System.Xml]System.Xml.XmlReader channelsReader,
             [5] class [mscorlib]System.Collections.Generic.Dictionary`2<string,string> xChannel,
             [6] class Pololu.Usc.ChannelSetting cs,
             [7] class [System.Xml]System.Xml.XmlReader sequencesReader,
             [8] class [Sequencer]Pololu.Usc.Sequencer.Sequence sequence,
             [9] class [mscorlib]System.Collections.Generic.Dictionary`2<string,string> sequenceAttributes,
             [10] class [System.Xml]System.Xml.XmlReader framesReader,
             [11] class [Sequencer]Pololu.Usc.Sequencer.Frame frame,
             [12] class [mscorlib]System.Collections.Generic.Dictionary`2<string,string> frameAttributes,
             [13] class [System.Xml]System.Xml.XmlException e,
             [14] class [mscorlib]System.Exception V_14,
             [15] class Pololu.Usc.UscSettings CS$1$0000,
             [16] bool CS$4$0001,
             [17] string CS$4$0002,
             [18] string[] CS$0$0003)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       class [System.Xml]System.Xml.XmlReader [System.Xml]System.Xml.XmlReader::Create(class [mscorlib]System.IO.TextReader)
    IL_0007:  stloc.0
    IL_0008:  newobj     instance void Pololu.Usc.UscSettings::.ctor()
    IL_000d:  stloc.1
    IL_000e:  ldstr      ""
    IL_0013:  stloc.2
    IL_0014:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::.ctor()
    IL_0019:  stloc.3
    IL_001a:  ldloc.0
    IL_001b:  ldstr      "UscSettings"
    IL_0020:  callvirt   instance bool [System.Xml]System.Xml.XmlReader::ReadToFollowing(string)
    IL_0025:  pop
    IL_0026:  ldloc.0
    IL_0027:  ldloc.3
    IL_0028:  call       void Pololu.Usc.ConfigurationFile::readAttributes(class [System.Xml]System.Xml.XmlReader,
                                                                           class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>)
    IL_002d:  nop
    IL_002e:  ldloc.0
    IL_002f:  callvirt   instance class [System.Xml]System.Xml.XmlReader [System.Xml]System.Xml.XmlReader::ReadSubtree()
    IL_0034:  stloc.0
    IL_0035:  ldloc.3
    IL_0036:  ldstr      "version"
    IL_003b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::ContainsKey(!0)
    IL_0040:  stloc.s    CS$4$0001
    IL_0042:  ldloc.s    CS$4$0001
    IL_0044:  brtrue.s   IL_0056

    IL_0046:  nop
    IL_0047:  ldarg.1
    IL_0048:  ldstr      "This file has no version number, so it might have "
    + "been read incorrectly."
    IL_004d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_0052:  nop
    IL_0053:  nop
    IL_0054:  br.s       IL_0097

    IL_0056:  ldloc.3
    IL_0057:  ldstr      "version"
    IL_005c:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0061:  ldstr      "1"
    IL_0066:  call       bool [mscorlib]System.String::op_Inequality(string,
                                                                     string)
    IL_006b:  ldc.i4.0
    IL_006c:  ceq
    IL_006e:  stloc.s    CS$4$0001
    IL_0070:  ldloc.s    CS$4$0001
    IL_0072:  brtrue.s   IL_0097

    IL_0074:  nop
    IL_0075:  ldarg.1
    IL_0076:  ldstr      "Unrecognized settings file version \""
    IL_007b:  ldloc.3
    IL_007c:  ldstr      "version"
    IL_0081:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0086:  ldstr      "\"."
    IL_008b:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0090:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_0095:  nop
    IL_0096:  nop
    IL_0097:  ldloc.0
    IL_0098:  callvirt   instance bool [System.Xml]System.Xml.XmlReader::Read()
    IL_009d:  pop
    IL_009e:  br         IL_07a8

    IL_00a3:  nop
    IL_00a4:  ldloc.0
    IL_00a5:  callvirt   instance valuetype [System.Xml]System.Xml.XmlNodeType [System.Xml]System.Xml.XmlReader::get_NodeType()
    IL_00aa:  ldc.i4.1
    IL_00ab:  bne.un.s   IL_00c2

    IL_00ad:  ldloc.0
    IL_00ae:  callvirt   instance string [System.Xml]System.Xml.XmlReader::get_Name()
    IL_00b3:  ldstr      "Channels"
    IL_00b8:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00bd:  ldc.i4.0
    IL_00be:  ceq
    IL_00c0:  br.s       IL_00c3

    IL_00c2:  ldc.i4.1
    IL_00c3:  stloc.s    CS$4$0001
    IL_00c5:  ldloc.s    CS$4$0001
    IL_00c7:  brtrue     IL_0461

    IL_00cc:  nop
    IL_00cd:  ldloc.0
    IL_00ce:  ldloc.3
    IL_00cf:  call       void Pololu.Usc.ConfigurationFile::readAttributes(class [System.Xml]System.Xml.XmlReader,
                                                                           class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>)
    IL_00d4:  nop
    IL_00d5:  ldloc.0
    IL_00d6:  callvirt   instance class [System.Xml]System.Xml.XmlReader [System.Xml]System.Xml.XmlReader::ReadSubtree()
    IL_00db:  stloc.s    channelsReader
    IL_00dd:  br         IL_040e

    IL_00e2:  nop
    IL_00e3:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::.ctor()
    IL_00e8:  stloc.s    xChannel
    IL_00ea:  ldloc.s    channelsReader
    IL_00ec:  ldloc.s    xChannel
    IL_00ee:  call       void Pololu.Usc.ConfigurationFile::readAttributes(class [System.Xml]System.Xml.XmlReader,
                                                                           class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>)
    IL_00f3:  nop
    IL_00f4:  newobj     instance void Pololu.Usc.ChannelSetting::.ctor()
    IL_00f9:  stloc.s    cs
    IL_00fb:  ldstr      "name"
    IL_0100:  ldloc.s    xChannel
    IL_0102:  ldarg.1
    IL_0103:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0108:  ldc.i4.0
    IL_0109:  ceq
    IL_010b:  stloc.s    CS$4$0001
    IL_010d:  ldloc.s    CS$4$0001
    IL_010f:  brtrue.s   IL_0126

    IL_0111:  nop
    IL_0112:  ldloc.s    cs
    IL_0114:  ldloc.s    xChannel
    IL_0116:  ldstr      "name"
    IL_011b:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0120:  stfld      string Pololu.Usc.ChannelSetting::name
    IL_0125:  nop
    IL_0126:  ldstr      "mode"
    IL_012b:  ldloc.s    xChannel
    IL_012d:  ldarg.1
    IL_012e:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0133:  ldc.i4.0
    IL_0134:  ceq
    IL_0136:  stloc.s    CS$4$0001
    IL_0138:  ldloc.s    CS$4$0001
    IL_013a:  brtrue     IL_01de

    IL_013f:  nop
    IL_0140:  ldloc.s    xChannel
    IL_0142:  ldstr      "mode"
    IL_0147:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_014c:  callvirt   instance string [mscorlib]System.String::ToLower()
    IL_0151:  stloc.s    CS$4$0002
    IL_0153:  ldloc.s    CS$4$0002
    IL_0155:  brfalse.s  IL_01b9

    IL_0157:  ldloc.s    CS$4$0002
    IL_0159:  ldstr      "servomultiplied"
    IL_015e:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0163:  brtrue.s   IL_0191

    IL_0165:  ldloc.s    CS$4$0002
    IL_0167:  ldstr      "servo"
    IL_016c:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0171:  brtrue.s   IL_019b

    IL_0173:  ldloc.s    CS$4$0002
    IL_0175:  ldstr      "input"
    IL_017a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_017f:  brtrue.s   IL_01a5

    IL_0181:  ldloc.s    CS$4$0002
    IL_0183:  ldstr      "output"
    IL_0188:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_018d:  brtrue.s   IL_01af

    IL_018f:  br.s       IL_01b9

    IL_0191:  ldloc.s    cs
    IL_0193:  ldc.i4.1
    IL_0194:  stfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_0199:  br.s       IL_01dd

    IL_019b:  ldloc.s    cs
    IL_019d:  ldc.i4.0
    IL_019e:  stfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_01a3:  br.s       IL_01dd

    IL_01a5:  ldloc.s    cs
    IL_01a7:  ldc.i4.3
    IL_01a8:  stfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_01ad:  br.s       IL_01dd

    IL_01af:  ldloc.s    cs
    IL_01b1:  ldc.i4.2
    IL_01b2:  stfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_01b7:  br.s       IL_01dd

    IL_01b9:  ldarg.1
    IL_01ba:  ldstr      "Invalid mode \""
    IL_01bf:  ldloc.s    xChannel
    IL_01c1:  ldstr      "mode"
    IL_01c6:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_01cb:  ldstr      "\"."
    IL_01d0:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_01d5:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_01da:  nop
    IL_01db:  br.s       IL_01dd

    IL_01dd:  nop
    IL_01de:  ldstr      "homemode"
    IL_01e3:  ldloc.s    xChannel
    IL_01e5:  ldarg.1
    IL_01e6:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_01eb:  ldc.i4.0
    IL_01ec:  ceq
    IL_01ee:  stloc.s    CS$4$0001
    IL_01f0:  ldloc.s    CS$4$0001
    IL_01f2:  brtrue     IL_027e

    IL_01f7:  nop
    IL_01f8:  ldloc.s    xChannel
    IL_01fa:  ldstr      "homemode"
    IL_01ff:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0204:  callvirt   instance string [mscorlib]System.String::ToLower()
    IL_0209:  stloc.s    CS$4$0002
    IL_020b:  ldloc.s    CS$4$0002
    IL_020d:  brfalse.s  IL_0259

    IL_020f:  ldloc.s    CS$4$0002
    IL_0211:  ldstr      "goto"
    IL_0216:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_021b:  brtrue.s   IL_023b

    IL_021d:  ldloc.s    CS$4$0002
    IL_021f:  ldstr      "off"
    IL_0224:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0229:  brtrue.s   IL_0245

    IL_022b:  ldloc.s    CS$4$0002
    IL_022d:  ldstr      "ignore"
    IL_0232:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0237:  brtrue.s   IL_024f

    IL_0239:  br.s       IL_0259

    IL_023b:  ldloc.s    cs
    IL_023d:  ldc.i4.2
    IL_023e:  stfld      valuetype Pololu.Usc.HomeMode Pololu.Usc.ChannelSetting::homeMode
    IL_0243:  br.s       IL_027d

    IL_0245:  ldloc.s    cs
    IL_0247:  ldc.i4.0
    IL_0248:  stfld      valuetype Pololu.Usc.HomeMode Pololu.Usc.ChannelSetting::homeMode
    IL_024d:  br.s       IL_027d

    IL_024f:  ldloc.s    cs
    IL_0251:  ldc.i4.1
    IL_0252:  stfld      valuetype Pololu.Usc.HomeMode Pololu.Usc.ChannelSetting::homeMode
    IL_0257:  br.s       IL_027d

    IL_0259:  ldarg.1
    IL_025a:  ldstr      "Invalid homemode \""
    IL_025f:  ldloc.s    xChannel
    IL_0261:  ldstr      "homemode"
    IL_0266:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_026b:  ldstr      "\"."
    IL_0270:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0275:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_027a:  nop
    IL_027b:  br.s       IL_027d

    IL_027d:  nop
    IL_027e:  ldstr      "min"
    IL_0283:  ldloc.s    xChannel
    IL_0285:  ldarg.1
    IL_0286:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_028b:  ldc.i4.0
    IL_028c:  ceq
    IL_028e:  stloc.s    CS$4$0001
    IL_0290:  ldloc.s    CS$4$0001
    IL_0292:  brtrue.s   IL_02b5

    IL_0294:  nop
    IL_0295:  ldloc.s    xChannel
    IL_0297:  ldstr      "min"
    IL_029c:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_02a1:  ldloc.s    cs
    IL_02a3:  ldflda     uint16 Pololu.Usc.ChannelSetting::minimum
    IL_02a8:  ldstr      "min"
    IL_02ad:  ldarg.1
    IL_02ae:  call       void Pololu.Usc.ConfigurationFile::parseU16(string,
                                                                     uint16&,
                                                                     string,
                                                                     class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_02b3:  nop
    IL_02b4:  nop
    IL_02b5:  ldstr      "max"
    IL_02ba:  ldloc.s    xChannel
    IL_02bc:  ldarg.1
    IL_02bd:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_02c2:  ldc.i4.0
    IL_02c3:  ceq
    IL_02c5:  stloc.s    CS$4$0001
    IL_02c7:  ldloc.s    CS$4$0001
    IL_02c9:  brtrue.s   IL_02ec

    IL_02cb:  nop
    IL_02cc:  ldloc.s    xChannel
    IL_02ce:  ldstr      "max"
    IL_02d3:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_02d8:  ldloc.s    cs
    IL_02da:  ldflda     uint16 Pololu.Usc.ChannelSetting::maximum
    IL_02df:  ldstr      "max"
    IL_02e4:  ldarg.1
    IL_02e5:  call       void Pololu.Usc.ConfigurationFile::parseU16(string,
                                                                     uint16&,
                                                                     string,
                                                                     class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_02ea:  nop
    IL_02eb:  nop
    IL_02ec:  ldstr      "home"
    IL_02f1:  ldloc.s    xChannel
    IL_02f3:  ldarg.1
    IL_02f4:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_02f9:  ldc.i4.0
    IL_02fa:  ceq
    IL_02fc:  stloc.s    CS$4$0001
    IL_02fe:  ldloc.s    CS$4$0001
    IL_0300:  brtrue.s   IL_0323

    IL_0302:  nop
    IL_0303:  ldloc.s    xChannel
    IL_0305:  ldstr      "home"
    IL_030a:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_030f:  ldloc.s    cs
    IL_0311:  ldflda     uint16 Pololu.Usc.ChannelSetting::home
    IL_0316:  ldstr      "home"
    IL_031b:  ldarg.1
    IL_031c:  call       void Pololu.Usc.ConfigurationFile::parseU16(string,
                                                                     uint16&,
                                                                     string,
                                                                     class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0321:  nop
    IL_0322:  nop
    IL_0323:  ldstr      "speed"
    IL_0328:  ldloc.s    xChannel
    IL_032a:  ldarg.1
    IL_032b:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0330:  ldc.i4.0
    IL_0331:  ceq
    IL_0333:  stloc.s    CS$4$0001
    IL_0335:  ldloc.s    CS$4$0001
    IL_0337:  brtrue.s   IL_035a

    IL_0339:  nop
    IL_033a:  ldloc.s    xChannel
    IL_033c:  ldstr      "speed"
    IL_0341:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0346:  ldloc.s    cs
    IL_0348:  ldflda     uint16 Pololu.Usc.ChannelSetting::speed
    IL_034d:  ldstr      "speed"
    IL_0352:  ldarg.1
    IL_0353:  call       void Pololu.Usc.ConfigurationFile::parseU16(string,
                                                                     uint16&,
                                                                     string,
                                                                     class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0358:  nop
    IL_0359:  nop
    IL_035a:  ldstr      "acceleration"
    IL_035f:  ldloc.s    xChannel
    IL_0361:  ldarg.1
    IL_0362:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0367:  ldc.i4.0
    IL_0368:  ceq
    IL_036a:  stloc.s    CS$4$0001
    IL_036c:  ldloc.s    CS$4$0001
    IL_036e:  brtrue.s   IL_0391

    IL_0370:  nop
    IL_0371:  ldloc.s    xChannel
    IL_0373:  ldstr      "acceleration"
    IL_0378:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_037d:  ldloc.s    cs
    IL_037f:  ldflda     uint8 Pololu.Usc.ChannelSetting::acceleration
    IL_0384:  ldstr      "acceleration"
    IL_0389:  ldarg.1
    IL_038a:  call       void Pololu.Usc.ConfigurationFile::parseU8(string,
                                                                    uint8&,
                                                                    string,
                                                                    class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_038f:  nop
    IL_0390:  nop
    IL_0391:  ldstr      "neutral"
    IL_0396:  ldloc.s    xChannel
    IL_0398:  ldarg.1
    IL_0399:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_039e:  ldc.i4.0
    IL_039f:  ceq
    IL_03a1:  stloc.s    CS$4$0001
    IL_03a3:  ldloc.s    CS$4$0001
    IL_03a5:  brtrue.s   IL_03c8

    IL_03a7:  nop
    IL_03a8:  ldloc.s    xChannel
    IL_03aa:  ldstr      "neutral"
    IL_03af:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_03b4:  ldloc.s    cs
    IL_03b6:  ldflda     uint16 Pololu.Usc.ChannelSetting::neutral
    IL_03bb:  ldstr      "neutral"
    IL_03c0:  ldarg.1
    IL_03c1:  call       void Pololu.Usc.ConfigurationFile::parseU16(string,
                                                                     uint16&,
                                                                     string,
                                                                     class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_03c6:  nop
    IL_03c7:  nop
    IL_03c8:  ldstr      "range"
    IL_03cd:  ldloc.s    xChannel
    IL_03cf:  ldarg.1
    IL_03d0:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_03d5:  ldc.i4.0
    IL_03d6:  ceq
    IL_03d8:  stloc.s    CS$4$0001
    IL_03da:  ldloc.s    CS$4$0001
    IL_03dc:  brtrue.s   IL_03ff

    IL_03de:  nop
    IL_03df:  ldloc.s    xChannel
    IL_03e1:  ldstr      "range"
    IL_03e6:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_03eb:  ldloc.s    cs
    IL_03ed:  ldflda     uint16 Pololu.Usc.ChannelSetting::range
    IL_03f2:  ldstr      "range"
    IL_03f7:  ldarg.1
    IL_03f8:  call       void Pololu.Usc.ConfigurationFile::parseU16(string,
                                                                     uint16&,
                                                                     string,
                                                                     class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_03fd:  nop
    IL_03fe:  nop
    IL_03ff:  ldloc.1
    IL_0400:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting> Pololu.Usc.UscSettings::channelSettings
    IL_0405:  ldloc.s    cs
    IL_0407:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting>::Add(!0)
    IL_040c:  nop
    IL_040d:  nop
    IL_040e:  ldloc.s    channelsReader
    IL_0410:  ldstr      "Channel"
    IL_0415:  callvirt   instance bool [System.Xml]System.Xml.XmlReader::ReadToFollowing(string)
    IL_041a:  stloc.s    CS$4$0001
    IL_041c:  ldloc.s    CS$4$0001
    IL_041e:  brtrue     IL_00e2

    IL_0423:  ldloc.s    channelsReader
    IL_0425:  ldstr      "Channel"
    IL_042a:  callvirt   instance bool [System.Xml]System.Xml.XmlReader::ReadToFollowing(string)
    IL_042f:  ldc.i4.0
    IL_0430:  ceq
    IL_0432:  stloc.s    CS$4$0001
    IL_0434:  ldloc.s    CS$4$0001
    IL_0436:  brtrue.s   IL_045b

    IL_0438:  nop
    IL_0439:  ldarg.1
    IL_043a:  ldstr      "More than "
    IL_043f:  ldloc.1
    IL_0440:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_0445:  box        [mscorlib]System.Byte
    IL_044a:  ldstr      " channel elements were found.  The extra elements "
    + "have been discarded."
    IL_044f:  call       string [mscorlib]System.String::Concat(object,
                                                                object,
                                                                object)
    IL_0454:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_0459:  nop
    IL_045a:  nop
    IL_045b:  nop
    IL_045c:  br         IL_07a7

    IL_0461:  ldloc.0
    IL_0462:  callvirt   instance valuetype [System.Xml]System.Xml.XmlNodeType [System.Xml]System.Xml.XmlReader::get_NodeType()
    IL_0467:  ldc.i4.1
    IL_0468:  bne.un.s   IL_047f

    IL_046a:  ldloc.0
    IL_046b:  callvirt   instance string [System.Xml]System.Xml.XmlReader::get_Name()
    IL_0470:  ldstr      "Sequences"
    IL_0475:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_047a:  ldc.i4.0
    IL_047b:  ceq
    IL_047d:  br.s       IL_0480

    IL_047f:  ldc.i4.1
    IL_0480:  stloc.s    CS$4$0001
    IL_0482:  ldloc.s    CS$4$0001
    IL_0484:  brtrue     IL_071a

    IL_0489:  nop
    IL_048a:  ldloc.0
    IL_048b:  callvirt   instance class [System.Xml]System.Xml.XmlReader [System.Xml]System.Xml.XmlReader::ReadSubtree()
    IL_0490:  stloc.s    sequencesReader
    IL_0492:  br         IL_06ff

    IL_0497:  nop
    IL_0498:  newobj     instance void [Sequencer]Pololu.Usc.Sequencer.Sequence::.ctor()
    IL_049d:  stloc.s    sequence
    IL_049f:  ldloc.1
    IL_04a0:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence> Pololu.Usc.UscSettings::sequences
    IL_04a5:  ldloc.s    sequence
    IL_04a7:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence>::Add(!0)
    IL_04ac:  nop
    IL_04ad:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::.ctor()
    IL_04b2:  stloc.s    sequenceAttributes
    IL_04b4:  ldloc.s    sequencesReader
    IL_04b6:  ldloc.s    sequenceAttributes
    IL_04b8:  call       void Pololu.Usc.ConfigurationFile::readAttributes(class [System.Xml]System.Xml.XmlReader,
                                                                           class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>)
    IL_04bd:  nop
    IL_04be:  ldloc.s    sequenceAttributes
    IL_04c0:  ldstr      "name"
    IL_04c5:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::ContainsKey(!0)
    IL_04ca:  ldc.i4.0
    IL_04cb:  ceq
    IL_04cd:  stloc.s    CS$4$0001
    IL_04cf:  ldloc.s    CS$4$0001
    IL_04d1:  brtrue.s   IL_04ea

    IL_04d3:  nop
    IL_04d4:  ldloc.s    sequence
    IL_04d6:  ldloc.s    sequenceAttributes
    IL_04d8:  ldstr      "name"
    IL_04dd:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_04e2:  stfld      string [Sequencer]Pololu.Usc.Sequencer.Sequence::name
    IL_04e7:  nop
    IL_04e8:  br.s       IL_052a

    IL_04ea:  nop
    IL_04eb:  ldloc.s    sequence
    IL_04ed:  ldstr      "Sequence "
    IL_04f2:  ldloc.1
    IL_04f3:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence> Pololu.Usc.UscSettings::sequences
    IL_04f8:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence>::get_Count()
    IL_04fd:  box        [mscorlib]System.Int32
    IL_0502:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0507:  stfld      string [Sequencer]Pololu.Usc.Sequencer.Sequence::name
    IL_050c:  ldarg.1
    IL_050d:  ldstr      "No name found for sequence "
    IL_0512:  ldloc.s    sequence
    IL_0514:  ldfld      string [Sequencer]Pololu.Usc.Sequencer.Sequence::name
    IL_0519:  ldstr      "."
    IL_051e:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0523:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_0528:  nop
    IL_0529:  nop
    IL_052a:  ldloc.0
    IL_052b:  callvirt   instance class [System.Xml]System.Xml.XmlReader [System.Xml]System.Xml.XmlReader::ReadSubtree()
    IL_0530:  stloc.s    framesReader
    IL_0532:  br         IL_06e9

    IL_0537:  nop
    IL_0538:  newobj     instance void [Sequencer]Pololu.Usc.Sequencer.Frame::.ctor()
    IL_053d:  stloc.s    frame
    IL_053f:  ldloc.s    sequence
    IL_0541:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Frame> [Sequencer]Pololu.Usc.Sequencer.Sequence::frames
    IL_0546:  ldloc.s    frame
    IL_0548:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Frame>::Add(!0)
    IL_054d:  nop
    IL_054e:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::.ctor()
    IL_0553:  stloc.s    frameAttributes
    IL_0555:  ldloc.s    framesReader
    IL_0557:  ldloc.s    frameAttributes
    IL_0559:  call       void Pololu.Usc.ConfigurationFile::readAttributes(class [System.Xml]System.Xml.XmlReader,
                                                                           class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>)
    IL_055e:  nop
    IL_055f:  ldloc.s    frameAttributes
    IL_0561:  ldstr      "name"
    IL_0566:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::ContainsKey(!0)
    IL_056b:  ldc.i4.0
    IL_056c:  ceq
    IL_056e:  stloc.s    CS$4$0001
    IL_0570:  ldloc.s    CS$4$0001
    IL_0572:  brtrue.s   IL_058b

    IL_0574:  nop
    IL_0575:  ldloc.s    frame
    IL_0577:  ldloc.s    frameAttributes
    IL_0579:  ldstr      "name"
    IL_057e:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0583:  stfld      string [Sequencer]Pololu.Usc.Sequencer.Frame::name
    IL_0588:  nop
    IL_0589:  br.s       IL_05f6

    IL_058b:  nop
    IL_058c:  ldloc.s    frame
    IL_058e:  ldstr      "Frame "
    IL_0593:  ldloc.s    sequence
    IL_0595:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Frame> [Sequencer]Pololu.Usc.Sequencer.Sequence::frames
    IL_059a:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Frame>::get_Count()
    IL_059f:  box        [mscorlib]System.Int32
    IL_05a4:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_05a9:  stfld      string [Sequencer]Pololu.Usc.Sequencer.Frame::name
    IL_05ae:  ldarg.1
    IL_05af:  ldc.i4.5
    IL_05b0:  newarr     [mscorlib]System.String
    IL_05b5:  stloc.s    CS$0$0003
    IL_05b7:  ldloc.s    CS$0$0003
    IL_05b9:  ldc.i4.0
    IL_05ba:  ldstr      "No name found for "
    IL_05bf:  stelem.ref
    IL_05c0:  ldloc.s    CS$0$0003
    IL_05c2:  ldc.i4.1
    IL_05c3:  ldloc.s    frame
    IL_05c5:  ldfld      string [Sequencer]Pololu.Usc.Sequencer.Frame::name
    IL_05ca:  stelem.ref
    IL_05cb:  ldloc.s    CS$0$0003
    IL_05cd:  ldc.i4.2
    IL_05ce:  ldstr      " in sequence \""
    IL_05d3:  stelem.ref
    IL_05d4:  ldloc.s    CS$0$0003
    IL_05d6:  ldc.i4.3
    IL_05d7:  ldloc.s    sequence
    IL_05d9:  ldfld      string [Sequencer]Pololu.Usc.Sequencer.Sequence::name
    IL_05de:  stelem.ref
    IL_05df:  ldloc.s    CS$0$0003
    IL_05e1:  ldc.i4.4
    IL_05e2:  ldstr      "\"."
    IL_05e7:  stelem.ref
    IL_05e8:  ldloc.s    CS$0$0003
    IL_05ea:  call       string [mscorlib]System.String::Concat(string[])
    IL_05ef:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_05f4:  nop
    IL_05f5:  nop
    IL_05f6:  ldloc.s    frameAttributes
    IL_05f8:  ldstr      "duration"
    IL_05fd:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::ContainsKey(!0)
    IL_0602:  ldc.i4.0
    IL_0603:  ceq
    IL_0605:  stloc.s    CS$4$0001
    IL_0607:  ldloc.s    CS$4$0001
    IL_0609:  brtrue.s   IL_0669

    IL_060b:  nop
    IL_060c:  ldloc.s    frameAttributes
    IL_060e:  ldstr      "duration"
    IL_0613:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0618:  ldloc.s    frame
    IL_061a:  ldflda     uint16 [Sequencer]Pololu.Usc.Sequencer.Frame::length_ms
    IL_061f:  ldc.i4.5
    IL_0620:  newarr     [mscorlib]System.String
    IL_0625:  stloc.s    CS$0$0003
    IL_0627:  ldloc.s    CS$0$0003
    IL_0629:  ldc.i4.0
    IL_062a:  ldstr      "Duration for frame \""
    IL_062f:  stelem.ref
    IL_0630:  ldloc.s    CS$0$0003
    IL_0632:  ldc.i4.1
    IL_0633:  ldloc.s    frame
    IL_0635:  ldfld      string [Sequencer]Pololu.Usc.Sequencer.Frame::name
    IL_063a:  stelem.ref
    IL_063b:  ldloc.s    CS$0$0003
    IL_063d:  ldc.i4.2
    IL_063e:  ldstr      "\" in sequence \""
    IL_0643:  stelem.ref
    IL_0644:  ldloc.s    CS$0$0003
    IL_0646:  ldc.i4.3
    IL_0647:  ldloc.s    sequence
    IL_0649:  ldfld      string [Sequencer]Pololu.Usc.Sequencer.Sequence::name
    IL_064e:  stelem.ref
    IL_064f:  ldloc.s    CS$0$0003
    IL_0651:  ldc.i4.4
    IL_0652:  ldstr      "\"."
    IL_0657:  stelem.ref
    IL_0658:  ldloc.s    CS$0$0003
    IL_065a:  call       string [mscorlib]System.String::Concat(string[])
    IL_065f:  ldarg.1
    IL_0660:  call       void Pololu.Usc.ConfigurationFile::parseU16(string,
                                                                     uint16&,
                                                                     string,
                                                                     class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0665:  nop
    IL_0666:  nop
    IL_0667:  br.s       IL_06d4

    IL_0669:  nop
    IL_066a:  ldloc.s    frame
    IL_066c:  ldstr      "Frame "
    IL_0671:  ldloc.s    sequence
    IL_0673:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Frame> [Sequencer]Pololu.Usc.Sequencer.Sequence::frames
    IL_0678:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Frame>::get_Count()
    IL_067d:  box        [mscorlib]System.Int32
    IL_0682:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0687:  stfld      string [Sequencer]Pololu.Usc.Sequencer.Frame::name
    IL_068c:  ldarg.1
    IL_068d:  ldc.i4.5
    IL_068e:  newarr     [mscorlib]System.String
    IL_0693:  stloc.s    CS$0$0003
    IL_0695:  ldloc.s    CS$0$0003
    IL_0697:  ldc.i4.0
    IL_0698:  ldstr      "No duration found for frame \""
    IL_069d:  stelem.ref
    IL_069e:  ldloc.s    CS$0$0003
    IL_06a0:  ldc.i4.1
    IL_06a1:  ldloc.s    frame
    IL_06a3:  ldfld      string [Sequencer]Pololu.Usc.Sequencer.Frame::name
    IL_06a8:  stelem.ref
    IL_06a9:  ldloc.s    CS$0$0003
    IL_06ab:  ldc.i4.2
    IL_06ac:  ldstr      "\" in sequence \""
    IL_06b1:  stelem.ref
    IL_06b2:  ldloc.s    CS$0$0003
    IL_06b4:  ldc.i4.3
    IL_06b5:  ldloc.s    sequence
    IL_06b7:  ldfld      string [Sequencer]Pololu.Usc.Sequencer.Sequence::name
    IL_06bc:  stelem.ref
    IL_06bd:  ldloc.s    CS$0$0003
    IL_06bf:  ldc.i4.4
    IL_06c0:  ldstr      "\"."
    IL_06c5:  stelem.ref
    IL_06c6:  ldloc.s    CS$0$0003
    IL_06c8:  call       string [mscorlib]System.String::Concat(string[])
    IL_06cd:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_06d2:  nop
    IL_06d3:  nop
    IL_06d4:  ldloc.s    frame
    IL_06d6:  ldloc.0
    IL_06d7:  callvirt   instance string [System.Xml]System.Xml.XmlReader::ReadElementContentAsString()
    IL_06dc:  ldloc.1
    IL_06dd:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_06e2:  callvirt   instance void [Sequencer]Pololu.Usc.Sequencer.Frame::setTargetsFromString(string,
                                                                                                   uint8)
    IL_06e7:  nop
    IL_06e8:  nop
    IL_06e9:  ldloc.s    framesReader
    IL_06eb:  ldstr      "Frame"
    IL_06f0:  callvirt   instance bool [System.Xml]System.Xml.XmlReader::ReadToFollowing(string)
    IL_06f5:  stloc.s    CS$4$0001
    IL_06f7:  ldloc.s    CS$4$0001
    IL_06f9:  brtrue     IL_0537

    IL_06fe:  nop
    IL_06ff:  ldloc.s    sequencesReader
    IL_0701:  ldstr      "Sequence"
    IL_0706:  callvirt   instance bool [System.Xml]System.Xml.XmlReader::ReadToFollowing(string)
    IL_070b:  stloc.s    CS$4$0001
    IL_070d:  ldloc.s    CS$4$0001
    IL_070f:  brtrue     IL_0497

    IL_0714:  nop
    IL_0715:  br         IL_07a7

    IL_071a:  ldloc.0
    IL_071b:  callvirt   instance valuetype [System.Xml]System.Xml.XmlNodeType [System.Xml]System.Xml.XmlReader::get_NodeType()
    IL_0720:  ldc.i4.1
    IL_0721:  bne.un.s   IL_0738

    IL_0723:  ldloc.0
    IL_0724:  callvirt   instance string [System.Xml]System.Xml.XmlReader::get_Name()
    IL_0729:  ldstr      "Script"
    IL_072e:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0733:  ldc.i4.0
    IL_0734:  ceq
    IL_0736:  br.s       IL_0739

    IL_0738:  ldc.i4.1
    IL_0739:  stloc.s    CS$4$0001
    IL_073b:  ldloc.s    CS$4$0001
    IL_073d:  brtrue.s   IL_0752

    IL_073f:  nop
    IL_0740:  ldloc.0
    IL_0741:  ldloc.3
    IL_0742:  call       void Pololu.Usc.ConfigurationFile::readAttributes(class [System.Xml]System.Xml.XmlReader,
                                                                           class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>)
    IL_0747:  nop
    IL_0748:  ldloc.0
    IL_0749:  callvirt   instance string [System.Xml]System.Xml.XmlReader::ReadElementContentAsString()
    IL_074e:  stloc.2
    IL_074f:  nop
    IL_0750:  br.s       IL_07a7

    IL_0752:  ldloc.0
    IL_0753:  callvirt   instance valuetype [System.Xml]System.Xml.XmlNodeType [System.Xml]System.Xml.XmlReader::get_NodeType()
    IL_0758:  ldc.i4.1
    IL_0759:  ceq
    IL_075b:  ldc.i4.0
    IL_075c:  ceq
    IL_075e:  stloc.s    CS$4$0001
    IL_0760:  ldloc.s    CS$4$0001
    IL_0762:  brtrue.s   IL_07a7

    IL_0764:  nop
    .try
    {
      IL_0765:  nop
      IL_0766:  ldloc.3
      IL_0767:  ldloc.0
      IL_0768:  callvirt   instance string [System.Xml]System.Xml.XmlReader::get_Name()
      IL_076d:  ldloc.0
      IL_076e:  callvirt   instance string [System.Xml]System.Xml.XmlReader::ReadElementContentAsString()
      IL_0773:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::set_Item(!0,
                                                                                                                          !1)
      IL_0778:  nop
      IL_0779:  nop
      IL_077a:  leave.s    IL_07a5

    }  // end .try
    catch [System.Xml]System.Xml.XmlException 
    {
      IL_077c:  stloc.s    e
      IL_077e:  nop
      IL_077f:  ldarg.1
      IL_0780:  ldstr      "Unable to parse element \""
      IL_0785:  ldloc.0
      IL_0786:  callvirt   instance string [System.Xml]System.Xml.XmlReader::get_Name()
      IL_078b:  ldstr      "\": "
      IL_0790:  ldloc.s    e
      IL_0792:  callvirt   instance string [mscorlib]System.Exception::get_Message()
      IL_0797:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string,
                                                                  string)
      IL_079c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
      IL_07a1:  nop
      IL_07a2:  nop
      IL_07a3:  leave.s    IL_07a5

    }  // end handler
    IL_07a5:  nop
    IL_07a6:  nop
    IL_07a7:  nop
    IL_07a8:  ldloc.0
    IL_07a9:  callvirt   instance bool [System.Xml]System.Xml.XmlReader::Read()
    IL_07ae:  stloc.s    CS$4$0001
    IL_07b0:  ldloc.s    CS$4$0001
    IL_07b2:  brtrue     IL_00a3

    IL_07b7:  ldloc.0
    IL_07b8:  callvirt   instance void [System.Xml]System.Xml.XmlReader::Close()
    IL_07bd:  nop
    .try
    {
      IL_07be:  nop
      IL_07bf:  ldloc.1
      IL_07c0:  ldloc.2
      IL_07c1:  callvirt   instance void Pololu.Usc.UscSettings::setAndCompileScript(string)
      IL_07c6:  nop
      IL_07c7:  nop
      IL_07c8:  leave.s    IL_07ef

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_07ca:  stloc.s    V_14
      IL_07cc:  nop
      IL_07cd:  ldarg.1
      IL_07ce:  ldstr      "Error compiling script from XML file: "
      IL_07d3:  ldloc.s    V_14
      IL_07d5:  callvirt   instance string [mscorlib]System.Exception::get_Message()
      IL_07da:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_07df:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
      IL_07e4:  nop
      IL_07e5:  ldloc.1
      IL_07e6:  ldc.i4.1
      IL_07e7:  stfld      bool Pololu.Usc.UscSettings::scriptInconsistent
      IL_07ec:  nop
      IL_07ed:  leave.s    IL_07ef

    }  // end handler
    IL_07ef:  nop
    IL_07f0:  ldstr      "NeverSuspend"
    IL_07f5:  ldloc.3
    IL_07f6:  ldarg.1
    IL_07f7:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_07fc:  ldc.i4.0
    IL_07fd:  ceq
    IL_07ff:  stloc.s    CS$4$0001
    IL_0801:  ldloc.s    CS$4$0001
    IL_0803:  brtrue.s   IL_0824

    IL_0805:  nop
    IL_0806:  ldloc.3
    IL_0807:  ldstr      "NeverSuspend"
    IL_080c:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0811:  ldloc.1
    IL_0812:  ldflda     bool Pololu.Usc.UscSettings::neverSuspend
    IL_0817:  ldstr      "NeverSuspend"
    IL_081c:  ldarg.1
    IL_081d:  call       void Pololu.Usc.ConfigurationFile::parseBool(string,
                                                                      bool&,
                                                                      string,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0822:  nop
    IL_0823:  nop
    IL_0824:  ldstr      "SerialMode"
    IL_0829:  ldloc.3
    IL_082a:  ldarg.1
    IL_082b:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0830:  ldc.i4.0
    IL_0831:  ceq
    IL_0833:  stloc.s    CS$4$0001
    IL_0835:  ldloc.s    CS$4$0001
    IL_0837:  brtrue.s   IL_089c

    IL_0839:  nop
    IL_083a:  ldloc.3
    IL_083b:  ldstr      "SerialMode"
    IL_0840:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0845:  stloc.s    CS$4$0002
    IL_0847:  ldloc.s    CS$4$0002
    IL_0849:  brfalse.s  IL_0877

    IL_084b:  ldloc.s    CS$4$0002
    IL_084d:  ldstr      "UART_FIXED_BAUD_RATE"
    IL_0852:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0857:  brtrue.s   IL_0880

    IL_0859:  ldloc.s    CS$4$0002
    IL_085b:  ldstr      "USB_DUAL_PORT"
    IL_0860:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0865:  brtrue.s   IL_0889

    IL_0867:  ldloc.s    CS$4$0002
    IL_0869:  ldstr      "USB_CHAINED"
    IL_086e:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0873:  brtrue.s   IL_0892

    IL_0875:  br.s       IL_0877

    IL_0877:  ldloc.1
    IL_0878:  ldc.i4.2
    IL_0879:  stfld      valuetype Pololu.Usc.uscSerialMode Pololu.Usc.UscSettings::serialMode
    IL_087e:  br.s       IL_089b

    IL_0880:  ldloc.1
    IL_0881:  ldc.i4.3
    IL_0882:  stfld      valuetype Pololu.Usc.uscSerialMode Pololu.Usc.UscSettings::serialMode
    IL_0887:  br.s       IL_089b

    IL_0889:  ldloc.1
    IL_088a:  ldc.i4.0
    IL_088b:  stfld      valuetype Pololu.Usc.uscSerialMode Pololu.Usc.UscSettings::serialMode
    IL_0890:  br.s       IL_089b

    IL_0892:  ldloc.1
    IL_0893:  ldc.i4.1
    IL_0894:  stfld      valuetype Pololu.Usc.uscSerialMode Pololu.Usc.UscSettings::serialMode
    IL_0899:  br.s       IL_089b

    IL_089b:  nop
    IL_089c:  ldstr      "FixedBaudRate"
    IL_08a1:  ldloc.3
    IL_08a2:  ldarg.1
    IL_08a3:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_08a8:  ldc.i4.0
    IL_08a9:  ceq
    IL_08ab:  stloc.s    CS$4$0001
    IL_08ad:  ldloc.s    CS$4$0001
    IL_08af:  brtrue.s   IL_08d0

    IL_08b1:  nop
    IL_08b2:  ldloc.3
    IL_08b3:  ldstr      "FixedBaudRate"
    IL_08b8:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_08bd:  ldloc.1
    IL_08be:  ldflda     uint32 Pololu.Usc.UscSettings::fixedBaudRate
    IL_08c3:  ldstr      "FixedBaudRate"
    IL_08c8:  ldarg.1
    IL_08c9:  call       void Pololu.Usc.ConfigurationFile::parseU32(string,
                                                                     uint32&,
                                                                     string,
                                                                     class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_08ce:  nop
    IL_08cf:  nop
    IL_08d0:  ldstr      "SerialTimeout"
    IL_08d5:  ldloc.3
    IL_08d6:  ldarg.1
    IL_08d7:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_08dc:  ldc.i4.0
    IL_08dd:  ceq
    IL_08df:  stloc.s    CS$4$0001
    IL_08e1:  ldloc.s    CS$4$0001
    IL_08e3:  brtrue.s   IL_0904

    IL_08e5:  nop
    IL_08e6:  ldloc.3
    IL_08e7:  ldstr      "SerialTimeout"
    IL_08ec:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_08f1:  ldloc.1
    IL_08f2:  ldflda     uint16 Pololu.Usc.UscSettings::serialTimeout
    IL_08f7:  ldstr      "SerialTimeout"
    IL_08fc:  ldarg.1
    IL_08fd:  call       void Pololu.Usc.ConfigurationFile::parseU16(string,
                                                                     uint16&,
                                                                     string,
                                                                     class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0902:  nop
    IL_0903:  nop
    IL_0904:  ldstr      "EnableCrc"
    IL_0909:  ldloc.3
    IL_090a:  ldarg.1
    IL_090b:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0910:  ldc.i4.0
    IL_0911:  ceq
    IL_0913:  stloc.s    CS$4$0001
    IL_0915:  ldloc.s    CS$4$0001
    IL_0917:  brtrue.s   IL_0938

    IL_0919:  nop
    IL_091a:  ldloc.3
    IL_091b:  ldstr      "EnableCrc"
    IL_0920:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0925:  ldloc.1
    IL_0926:  ldflda     bool Pololu.Usc.UscSettings::enableCrc
    IL_092b:  ldstr      "EnableCrc"
    IL_0930:  ldarg.1
    IL_0931:  call       void Pololu.Usc.ConfigurationFile::parseBool(string,
                                                                      bool&,
                                                                      string,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0936:  nop
    IL_0937:  nop
    IL_0938:  ldstr      "SerialDeviceNumber"
    IL_093d:  ldloc.3
    IL_093e:  ldarg.1
    IL_093f:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0944:  ldc.i4.0
    IL_0945:  ceq
    IL_0947:  stloc.s    CS$4$0001
    IL_0949:  ldloc.s    CS$4$0001
    IL_094b:  brtrue.s   IL_096c

    IL_094d:  nop
    IL_094e:  ldloc.3
    IL_094f:  ldstr      "SerialDeviceNumber"
    IL_0954:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0959:  ldloc.1
    IL_095a:  ldflda     uint8 Pololu.Usc.UscSettings::serialDeviceNumber
    IL_095f:  ldstr      "SerialDeviceNumber"
    IL_0964:  ldarg.1
    IL_0965:  call       void Pololu.Usc.ConfigurationFile::parseU8(string,
                                                                    uint8&,
                                                                    string,
                                                                    class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_096a:  nop
    IL_096b:  nop
    IL_096c:  ldstr      "SerialMiniSscOffset"
    IL_0971:  ldloc.3
    IL_0972:  ldarg.1
    IL_0973:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0978:  ldc.i4.0
    IL_0979:  ceq
    IL_097b:  stloc.s    CS$4$0001
    IL_097d:  ldloc.s    CS$4$0001
    IL_097f:  brtrue.s   IL_09a0

    IL_0981:  nop
    IL_0982:  ldloc.3
    IL_0983:  ldstr      "SerialMiniSscOffset"
    IL_0988:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_098d:  ldloc.1
    IL_098e:  ldflda     uint8 Pololu.Usc.UscSettings::miniSscOffset
    IL_0993:  ldstr      "SerialMiniSscOffset"
    IL_0998:  ldarg.1
    IL_0999:  call       void Pololu.Usc.ConfigurationFile::parseU8(string,
                                                                    uint8&,
                                                                    string,
                                                                    class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_099e:  nop
    IL_099f:  nop
    IL_09a0:  ldstr      "ScriptDone"
    IL_09a5:  ldloc.3
    IL_09a6:  ldarg.1
    IL_09a7:  call       bool Pololu.Usc.ConfigurationFile::assertKey(string,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_09ac:  ldc.i4.0
    IL_09ad:  ceq
    IL_09af:  stloc.s    CS$4$0001
    IL_09b1:  ldloc.s    CS$4$0001
    IL_09b3:  brtrue.s   IL_09d4

    IL_09b5:  nop
    IL_09b6:  ldloc.3
    IL_09b7:  ldstr      "ScriptDone"
    IL_09bc:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_09c1:  ldloc.1
    IL_09c2:  ldflda     bool Pololu.Usc.UscSettings::scriptDone
    IL_09c7:  ldstr      "ScriptDone"
    IL_09cc:  ldarg.1
    IL_09cd:  call       void Pololu.Usc.ConfigurationFile::parseBool(string,
                                                                      bool&,
                                                                      string,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_09d2:  nop
    IL_09d3:  nop
    IL_09d4:  ldloc.3
    IL_09d5:  ldstr      "ServosAvailable"
    IL_09da:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::ContainsKey(!0)
    IL_09df:  ldc.i4.0
    IL_09e0:  ceq
    IL_09e2:  stloc.s    CS$4$0001
    IL_09e4:  ldloc.s    CS$4$0001
    IL_09e6:  brtrue.s   IL_0a07

    IL_09e8:  nop
    IL_09e9:  ldloc.3
    IL_09ea:  ldstr      "ServosAvailable"
    IL_09ef:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_09f4:  ldloc.1
    IL_09f5:  ldflda     uint8 Pololu.Usc.UscSettings::servosAvailable
    IL_09fa:  ldstr      "ServosAvailable"
    IL_09ff:  ldarg.1
    IL_0a00:  call       void Pololu.Usc.ConfigurationFile::parseU8(string,
                                                                    uint8&,
                                                                    string,
                                                                    class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0a05:  nop
    IL_0a06:  nop
    IL_0a07:  ldloc.3
    IL_0a08:  ldstr      "ServoPeriod"
    IL_0a0d:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::ContainsKey(!0)
    IL_0a12:  ldc.i4.0
    IL_0a13:  ceq
    IL_0a15:  stloc.s    CS$4$0001
    IL_0a17:  ldloc.s    CS$4$0001
    IL_0a19:  brtrue.s   IL_0a3a

    IL_0a1b:  nop
    IL_0a1c:  ldloc.3
    IL_0a1d:  ldstr      "ServoPeriod"
    IL_0a22:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0a27:  ldloc.1
    IL_0a28:  ldflda     uint8 Pololu.Usc.UscSettings::servoPeriod
    IL_0a2d:  ldstr      "ServoPeriod"
    IL_0a32:  ldarg.1
    IL_0a33:  call       void Pololu.Usc.ConfigurationFile::parseU8(string,
                                                                    uint8&,
                                                                    string,
                                                                    class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0a38:  nop
    IL_0a39:  nop
    IL_0a3a:  ldloc.3
    IL_0a3b:  ldstr      "EnablePullups"
    IL_0a40:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::ContainsKey(!0)
    IL_0a45:  ldc.i4.0
    IL_0a46:  ceq
    IL_0a48:  stloc.s    CS$4$0001
    IL_0a4a:  ldloc.s    CS$4$0001
    IL_0a4c:  brtrue.s   IL_0a6d

    IL_0a4e:  nop
    IL_0a4f:  ldloc.3
    IL_0a50:  ldstr      "EnablePullups"
    IL_0a55:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0a5a:  ldloc.1
    IL_0a5b:  ldflda     bool Pololu.Usc.UscSettings::enablePullups
    IL_0a60:  ldstr      "EnablePullups"
    IL_0a65:  ldarg.1
    IL_0a66:  call       void Pololu.Usc.ConfigurationFile::parseBool(string,
                                                                      bool&,
                                                                      string,
                                                                      class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0a6b:  nop
    IL_0a6c:  nop
    IL_0a6d:  ldloc.3
    IL_0a6e:  ldstr      "MiniMaestroServoPeriod"
    IL_0a73:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::ContainsKey(!0)
    IL_0a78:  ldc.i4.0
    IL_0a79:  ceq
    IL_0a7b:  stloc.s    CS$4$0001
    IL_0a7d:  ldloc.s    CS$4$0001
    IL_0a7f:  brtrue.s   IL_0aa0

    IL_0a81:  nop
    IL_0a82:  ldloc.3
    IL_0a83:  ldstr      "MiniMaestroServoPeriod"
    IL_0a88:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0a8d:  ldloc.1
    IL_0a8e:  ldflda     uint32 Pololu.Usc.UscSettings::miniMaestroServoPeriod
    IL_0a93:  ldstr      "MiniMaestroServoPeriod"
    IL_0a98:  ldarg.1
    IL_0a99:  call       void Pololu.Usc.ConfigurationFile::parseU32(string,
                                                                     uint32&,
                                                                     string,
                                                                     class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0a9e:  nop
    IL_0a9f:  nop
    IL_0aa0:  ldloc.3
    IL_0aa1:  ldstr      "ServoMultiplier"
    IL_0aa6:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::ContainsKey(!0)
    IL_0aab:  ldc.i4.0
    IL_0aac:  ceq
    IL_0aae:  stloc.s    CS$4$0001
    IL_0ab0:  ldloc.s    CS$4$0001
    IL_0ab2:  brtrue.s   IL_0ad3

    IL_0ab4:  nop
    IL_0ab5:  ldloc.3
    IL_0ab6:  ldstr      "ServoMultiplier"
    IL_0abb:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Item(!0)
    IL_0ac0:  ldloc.1
    IL_0ac1:  ldflda     uint16 Pololu.Usc.UscSettings::servoMultiplier
    IL_0ac6:  ldstr      "ServoMultiplier"
    IL_0acb:  ldarg.1
    IL_0acc:  call       void Pololu.Usc.ConfigurationFile::parseU16(string,
                                                                     uint16&,
                                                                     string,
                                                                     class [mscorlib]System.Collections.Generic.List`1<string>)
    IL_0ad1:  nop
    IL_0ad2:  nop
    IL_0ad3:  ldloc.1
    IL_0ad4:  stloc.s    CS$1$0000
    IL_0ad6:  br.s       IL_0ad8

    IL_0ad8:  ldloc.s    CS$1$0000
    IL_0ada:  ret
  } // end of method ConfigurationFile::load

  .method private hidebysig static void  readAttributes(class [System.Xml]System.Xml.XmlReader reader,
                                                        class [mscorlib]System.Collections.Generic.Dictionary`2<string,string> attributes) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  3
    .locals init ([0] bool CS$4$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance bool [System.Xml]System.Xml.XmlReader::get_HasAttributes()
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  brtrue.s   IL_0031

    IL_000e:  nop
    IL_000f:  br.s       IL_0026

    IL_0011:  nop
    IL_0012:  ldarg.1
    IL_0013:  ldarg.0
    IL_0014:  callvirt   instance string [System.Xml]System.Xml.XmlReader::get_Name()
    IL_0019:  ldarg.0
    IL_001a:  callvirt   instance string [System.Xml]System.Xml.XmlReader::ReadContentAsString()
    IL_001f:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::set_Item(!0,
                                                                                                                        !1)
    IL_0024:  nop
    IL_0025:  nop
    IL_0026:  ldarg.0
    IL_0027:  callvirt   instance bool [System.Xml]System.Xml.XmlReader::MoveToNextAttribute()
    IL_002c:  stloc.0
    IL_002d:  ldloc.0
    IL_002e:  brtrue.s   IL_0011

    IL_0030:  nop
    IL_0031:  ldarg.0
    IL_0032:  callvirt   instance bool [System.Xml]System.Xml.XmlReader::MoveToElement()
    IL_0037:  pop
    IL_0038:  ret
  } // end of method ConfigurationFile::readAttributes

  .method private hidebysig static void  parseBool(string input,
                                                   bool& output,
                                                   string name,
                                                   class [mscorlib]System.Collections.Generic.List`1<string> warnings) cil managed
  {
    // Code size       50 (0x32)
    .maxstack  5
    .locals init ([0] bool result,
             [1] bool CS$4$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldloca.s   result
    IL_0004:  call       bool [mscorlib]System.Boolean::TryParse(string,
                                                                 bool&)
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  brtrue.s   IL_0017

    IL_0010:  nop
    IL_0011:  ldarg.1
    IL_0012:  ldloc.0
    IL_0013:  stind.i1
    IL_0014:  nop
    IL_0015:  br.s       IL_0031

    IL_0017:  nop
    IL_0018:  ldarg.3
    IL_0019:  ldarg.2
    IL_001a:  ldstr      ": Invalid integer value \""
    IL_001f:  ldarg.0
    IL_0020:  ldstr      "\"."
    IL_0025:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string,
                                                                string)
    IL_002a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_002f:  nop
    IL_0030:  nop
    IL_0031:  ret
  } // end of method ConfigurationFile::parseBool

  .method private hidebysig static void  parseU8(string input,
                                                 uint8& output,
                                                 string name,
                                                 class [mscorlib]System.Collections.Generic.List`1<string> warnings) cil managed
  {
    // Code size       50 (0x32)
    .maxstack  5
    .locals init ([0] uint8 result,
             [1] bool CS$4$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldloca.s   result
    IL_0004:  call       bool [mscorlib]System.Byte::TryParse(string,
                                                              uint8&)
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  brtrue.s   IL_0017

    IL_0010:  nop
    IL_0011:  ldarg.1
    IL_0012:  ldloc.0
    IL_0013:  stind.i1
    IL_0014:  nop
    IL_0015:  br.s       IL_0031

    IL_0017:  nop
    IL_0018:  ldarg.3
    IL_0019:  ldarg.2
    IL_001a:  ldstr      ": Invalid integer value \""
    IL_001f:  ldarg.0
    IL_0020:  ldstr      "\"."
    IL_0025:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string,
                                                                string)
    IL_002a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_002f:  nop
    IL_0030:  nop
    IL_0031:  ret
  } // end of method ConfigurationFile::parseU8

  .method private hidebysig static void  parseU16(string input,
                                                  uint16& output,
                                                  string name,
                                                  class [mscorlib]System.Collections.Generic.List`1<string> warnings) cil managed
  {
    // Code size       50 (0x32)
    .maxstack  5
    .locals init ([0] uint16 result,
             [1] bool CS$4$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldloca.s   result
    IL_0004:  call       bool [mscorlib]System.UInt16::TryParse(string,
                                                                uint16&)
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  brtrue.s   IL_0017

    IL_0010:  nop
    IL_0011:  ldarg.1
    IL_0012:  ldloc.0
    IL_0013:  stind.i2
    IL_0014:  nop
    IL_0015:  br.s       IL_0031

    IL_0017:  nop
    IL_0018:  ldarg.3
    IL_0019:  ldarg.2
    IL_001a:  ldstr      ": Invalid integer value \""
    IL_001f:  ldarg.0
    IL_0020:  ldstr      "\"."
    IL_0025:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string,
                                                                string)
    IL_002a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_002f:  nop
    IL_0030:  nop
    IL_0031:  ret
  } // end of method ConfigurationFile::parseU16

  .method private hidebysig static void  parseU32(string input,
                                                  uint32& output,
                                                  string name,
                                                  class [mscorlib]System.Collections.Generic.List`1<string> warnings) cil managed
  {
    // Code size       50 (0x32)
    .maxstack  5
    .locals init ([0] uint32 result,
             [1] bool CS$4$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldloca.s   result
    IL_0004:  call       bool [mscorlib]System.UInt32::TryParse(string,
                                                                uint32&)
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  brtrue.s   IL_0017

    IL_0010:  nop
    IL_0011:  ldarg.1
    IL_0012:  ldloc.0
    IL_0013:  stind.i4
    IL_0014:  nop
    IL_0015:  br.s       IL_0031

    IL_0017:  nop
    IL_0018:  ldarg.3
    IL_0019:  ldarg.2
    IL_001a:  ldstr      ": Invalid integer value \""
    IL_001f:  ldarg.0
    IL_0020:  ldstr      "\"."
    IL_0025:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string,
                                                                string)
    IL_002a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_002f:  nop
    IL_0030:  nop
    IL_0031:  ret
  } // end of method ConfigurationFile::parseU32

  .method private hidebysig static bool  assertKey(string key,
                                                   class [mscorlib]System.Collections.Generic.Dictionary`2<string,string> params_from_xml,
                                                   class [mscorlib]System.Collections.Generic.List`1<string> warnings) cil managed
  {
    // Code size       50 (0x32)
    .maxstack  4
    .locals init ([0] bool CS$1$0000,
             [1] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.0
    IL_0003:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::ContainsKey(!0)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.1
    IL_000c:  ldloc.1
    IL_000d:  brtrue.s   IL_0014

    IL_000f:  nop
    IL_0010:  ldc.i4.1
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_0030

    IL_0014:  nop
    IL_0015:  ldarg.2
    IL_0016:  ldstr      "The "
    IL_001b:  ldarg.0
    IL_001c:  ldstr      " setting was missing."
    IL_0021:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0026:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_002b:  nop
    IL_002c:  ldc.i4.0
    IL_002d:  stloc.0
    IL_002e:  br.s       IL_0030

    IL_0030:  ldloc.0
    IL_0031:  ret
  } // end of method ConfigurationFile::assertKey

  .method public hidebysig static void  save(class Pololu.Usc.UscSettings settings,
                                             class [mscorlib]System.IO.StreamWriter sw) cil managed
  {
    // Code size       1071 (0x42f)
    .maxstack  5
    .locals init ([0] class [System.Xml]System.Xml.XmlTextWriter writer,
             [1] uint8 i,
             [2] class Pololu.Usc.ChannelSetting setting,
             [3] class [Sequencer]Pololu.Usc.Sequencer.Sequence sequence,
             [4] class [Sequencer]Pololu.Usc.Sequencer.Frame frame,
             [5] bool CS$4$0000,
             [6] valuetype [mscorlib]System.Decimal CS$0$0001,
             [7] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sequencer]Pololu.Usc.Sequencer.Sequence> CS$5$0002,
             [8] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sequencer]Pololu.Usc.Sequencer.Frame> CS$5$0003)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void [System.Xml]System.Xml.XmlTextWriter::.ctor(class [mscorlib]System.IO.TextWriter)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  callvirt   instance void [System.Xml]System.Xml.XmlTextWriter::set_Formatting(valuetype [System.Xml]System.Xml.Formatting)
    IL_000f:  nop
    IL_0010:  ldloc.0
    IL_0011:  ldstr      "Pololu Maestro servo controller settings file, htt"
    + "p://www.pololu.com/catalog/product/1350"
    IL_0016:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteComment(string)
    IL_001b:  nop
    IL_001c:  ldloc.0
    IL_001d:  ldstr      "UscSettings"
    IL_0022:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteStartElement(string)
    IL_0027:  nop
    IL_0028:  ldloc.0
    IL_0029:  ldstr      "version"
    IL_002e:  ldstr      "1"
    IL_0033:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_0038:  nop
    IL_0039:  ldloc.0
    IL_003a:  ldstr      "NeverSuspend"
    IL_003f:  ldarg.0
    IL_0040:  ldfld      bool Pololu.Usc.UscSettings::neverSuspend
    IL_0045:  brtrue.s   IL_004e

    IL_0047:  ldstr      "false"
    IL_004c:  br.s       IL_0053

    IL_004e:  ldstr      "true"
    IL_0053:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteElementString(string,
                                                                                            string)
    IL_0058:  nop
    IL_0059:  ldloc.0
    IL_005a:  ldstr      "SerialMode"
    IL_005f:  ldarg.0
    IL_0060:  ldfld      valuetype Pololu.Usc.uscSerialMode Pololu.Usc.UscSettings::serialMode
    IL_0065:  box        Pololu.Usc.uscSerialMode
    IL_006a:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_006f:  ldstr      "SERIAL_MODE_"
    IL_0074:  ldstr      ""
    IL_0079:  callvirt   instance string [mscorlib]System.String::Replace(string,
                                                                          string)
    IL_007e:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteElementString(string,
                                                                                            string)
    IL_0083:  nop
    IL_0084:  ldloc.0
    IL_0085:  ldstr      "FixedBaudRate"
    IL_008a:  ldarg.0
    IL_008b:  ldflda     uint32 Pololu.Usc.UscSettings::fixedBaudRate
    IL_0090:  call       instance string [mscorlib]System.UInt32::ToString()
    IL_0095:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteElementString(string,
                                                                                            string)
    IL_009a:  nop
    IL_009b:  ldloc.0
    IL_009c:  ldstr      "SerialTimeout"
    IL_00a1:  ldarg.0
    IL_00a2:  ldflda     uint16 Pololu.Usc.UscSettings::serialTimeout
    IL_00a7:  call       instance string [mscorlib]System.UInt16::ToString()
    IL_00ac:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteElementString(string,
                                                                                            string)
    IL_00b1:  nop
    IL_00b2:  ldloc.0
    IL_00b3:  ldstr      "EnableCrc"
    IL_00b8:  ldarg.0
    IL_00b9:  ldfld      bool Pololu.Usc.UscSettings::enableCrc
    IL_00be:  brtrue.s   IL_00c7

    IL_00c0:  ldstr      "false"
    IL_00c5:  br.s       IL_00cc

    IL_00c7:  ldstr      "true"
    IL_00cc:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteElementString(string,
                                                                                            string)
    IL_00d1:  nop
    IL_00d2:  ldloc.0
    IL_00d3:  ldstr      "SerialDeviceNumber"
    IL_00d8:  ldarg.0
    IL_00d9:  ldflda     uint8 Pololu.Usc.UscSettings::serialDeviceNumber
    IL_00de:  call       instance string [mscorlib]System.Byte::ToString()
    IL_00e3:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteElementString(string,
                                                                                            string)
    IL_00e8:  nop
    IL_00e9:  ldloc.0
    IL_00ea:  ldstr      "SerialMiniSscOffset"
    IL_00ef:  ldarg.0
    IL_00f0:  ldflda     uint8 Pololu.Usc.UscSettings::miniSscOffset
    IL_00f5:  call       instance string [mscorlib]System.Byte::ToString()
    IL_00fa:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteElementString(string,
                                                                                            string)
    IL_00ff:  nop
    IL_0100:  ldarg.0
    IL_0101:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_0106:  ldc.i4.s   18
    IL_0108:  cgt
    IL_010a:  ldc.i4.0
    IL_010b:  ceq
    IL_010d:  stloc.s    CS$4$0000
    IL_010f:  ldloc.s    CS$4$0000
    IL_0111:  brtrue.s   IL_0135

    IL_0113:  nop
    IL_0114:  ldloc.0
    IL_0115:  ldstr      "EnablePullups"
    IL_011a:  ldarg.0
    IL_011b:  ldfld      bool Pololu.Usc.UscSettings::enablePullups
    IL_0120:  brtrue.s   IL_0129

    IL_0122:  ldstr      "false"
    IL_0127:  br.s       IL_012e

    IL_0129:  ldstr      "true"
    IL_012e:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteElementString(string,
                                                                                            string)
    IL_0133:  nop
    IL_0134:  nop
    IL_0135:  ldloc.0
    IL_0136:  ldstr      "Channels"
    IL_013b:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteStartElement(string)
    IL_0140:  nop
    IL_0141:  ldarg.0
    IL_0142:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_0147:  ldc.i4.6
    IL_0148:  ceq
    IL_014a:  ldc.i4.0
    IL_014b:  ceq
    IL_014d:  stloc.s    CS$4$0000
    IL_014f:  ldloc.s    CS$4$0000
    IL_0151:  brtrue.s   IL_0185

    IL_0153:  nop
    IL_0154:  ldloc.0
    IL_0155:  ldstr      "ServosAvailable"
    IL_015a:  ldarg.0
    IL_015b:  ldflda     uint8 Pololu.Usc.UscSettings::servosAvailable
    IL_0160:  call       instance string [mscorlib]System.Byte::ToString()
    IL_0165:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_016a:  nop
    IL_016b:  ldloc.0
    IL_016c:  ldstr      "ServoPeriod"
    IL_0171:  ldarg.0
    IL_0172:  ldflda     uint8 Pololu.Usc.UscSettings::servoPeriod
    IL_0177:  call       instance string [mscorlib]System.Byte::ToString()
    IL_017c:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_0181:  nop
    IL_0182:  nop
    IL_0183:  br.s       IL_01b5

    IL_0185:  nop
    IL_0186:  ldloc.0
    IL_0187:  ldstr      "MiniMaestroServoPeriod"
    IL_018c:  ldarg.0
    IL_018d:  ldflda     uint32 Pololu.Usc.UscSettings::miniMaestroServoPeriod
    IL_0192:  call       instance string [mscorlib]System.UInt32::ToString()
    IL_0197:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_019c:  nop
    IL_019d:  ldloc.0
    IL_019e:  ldstr      "ServoMultiplier"
    IL_01a3:  ldarg.0
    IL_01a4:  ldflda     uint16 Pololu.Usc.UscSettings::servoMultiplier
    IL_01a9:  call       instance string [mscorlib]System.UInt16::ToString()
    IL_01ae:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_01b3:  nop
    IL_01b4:  nop
    IL_01b5:  ldloc.0
    IL_01b6:  ldstr      "Period = "
    IL_01bb:  ldarg.0
    IL_01bc:  callvirt   instance valuetype [mscorlib]System.Decimal Pololu.Usc.UscSettings::get_periodInMicroseconds()
    IL_01c1:  ldc.i4     0x3e8
    IL_01c6:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_01cb:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_01d0:  stloc.s    CS$0$0001
    IL_01d2:  ldloca.s   CS$0$0001
    IL_01d4:  call       instance string [mscorlib]System.Decimal::ToString()
    IL_01d9:  ldstr      " ms"
    IL_01de:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_01e3:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteComment(string)
    IL_01e8:  nop
    IL_01e9:  ldc.i4.0
    IL_01ea:  stloc.1
    IL_01eb:  br         IL_031a

    IL_01f0:  nop
    IL_01f1:  ldarg.0
    IL_01f2:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting> Pololu.Usc.UscSettings::channelSettings
    IL_01f7:  ldloc.1
    IL_01f8:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting>::get_Item(int32)
    IL_01fd:  stloc.2
    IL_01fe:  ldloc.0
    IL_01ff:  ldstr      "Channel "
    IL_0204:  ldloca.s   i
    IL_0206:  call       instance string [mscorlib]System.Byte::ToString()
    IL_020b:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0210:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteComment(string)
    IL_0215:  nop
    IL_0216:  ldloc.0
    IL_0217:  ldstr      "Channel"
    IL_021c:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteStartElement(string)
    IL_0221:  nop
    IL_0222:  ldloc.0
    IL_0223:  ldstr      "name"
    IL_0228:  ldloc.2
    IL_0229:  ldfld      string Pololu.Usc.ChannelSetting::name
    IL_022e:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_0233:  nop
    IL_0234:  ldloc.0
    IL_0235:  ldstr      "mode"
    IL_023a:  ldloc.2
    IL_023b:  ldfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_0240:  box        Pololu.Usc.ChannelMode
    IL_0245:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_024a:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_024f:  nop
    IL_0250:  ldloc.0
    IL_0251:  ldstr      "min"
    IL_0256:  ldloc.2
    IL_0257:  ldflda     uint16 Pololu.Usc.ChannelSetting::minimum
    IL_025c:  call       instance string [mscorlib]System.UInt16::ToString()
    IL_0261:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_0266:  nop
    IL_0267:  ldloc.0
    IL_0268:  ldstr      "max"
    IL_026d:  ldloc.2
    IL_026e:  ldflda     uint16 Pololu.Usc.ChannelSetting::maximum
    IL_0273:  call       instance string [mscorlib]System.UInt16::ToString()
    IL_0278:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_027d:  nop
    IL_027e:  ldloc.0
    IL_027f:  ldstr      "homemode"
    IL_0284:  ldloc.2
    IL_0285:  ldfld      valuetype Pololu.Usc.HomeMode Pololu.Usc.ChannelSetting::homeMode
    IL_028a:  box        Pololu.Usc.HomeMode
    IL_028f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0294:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_0299:  nop
    IL_029a:  ldloc.0
    IL_029b:  ldstr      "home"
    IL_02a0:  ldloc.2
    IL_02a1:  ldflda     uint16 Pololu.Usc.ChannelSetting::home
    IL_02a6:  call       instance string [mscorlib]System.UInt16::ToString()
    IL_02ab:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_02b0:  nop
    IL_02b1:  ldloc.0
    IL_02b2:  ldstr      "speed"
    IL_02b7:  ldloc.2
    IL_02b8:  ldflda     uint16 Pololu.Usc.ChannelSetting::speed
    IL_02bd:  call       instance string [mscorlib]System.UInt16::ToString()
    IL_02c2:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_02c7:  nop
    IL_02c8:  ldloc.0
    IL_02c9:  ldstr      "acceleration"
    IL_02ce:  ldloc.2
    IL_02cf:  ldflda     uint8 Pololu.Usc.ChannelSetting::acceleration
    IL_02d4:  call       instance string [mscorlib]System.Byte::ToString()
    IL_02d9:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_02de:  nop
    IL_02df:  ldloc.0
    IL_02e0:  ldstr      "neutral"
    IL_02e5:  ldloc.2
    IL_02e6:  ldflda     uint16 Pololu.Usc.ChannelSetting::neutral
    IL_02eb:  call       instance string [mscorlib]System.UInt16::ToString()
    IL_02f0:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_02f5:  nop
    IL_02f6:  ldloc.0
    IL_02f7:  ldstr      "range"
    IL_02fc:  ldloc.2
    IL_02fd:  ldflda     uint16 Pololu.Usc.ChannelSetting::range
    IL_0302:  call       instance string [mscorlib]System.UInt16::ToString()
    IL_0307:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_030c:  nop
    IL_030d:  ldloc.0
    IL_030e:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteEndElement()
    IL_0313:  nop
    IL_0314:  nop
    IL_0315:  ldloc.1
    IL_0316:  ldc.i4.1
    IL_0317:  add
    IL_0318:  conv.u1
    IL_0319:  stloc.1
    IL_031a:  ldloc.1
    IL_031b:  ldarg.0
    IL_031c:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_0321:  clt
    IL_0323:  stloc.s    CS$4$0000
    IL_0325:  ldloc.s    CS$4$0000
    IL_0327:  brtrue     IL_01f0

    IL_032c:  ldloc.0
    IL_032d:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteEndElement()
    IL_0332:  nop
    IL_0333:  ldloc.0
    IL_0334:  ldstr      "Sequences"
    IL_0339:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteStartElement(string)
    IL_033e:  nop
    IL_033f:  nop
    IL_0340:  ldarg.0
    IL_0341:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence> Pololu.Usc.UscSettings::sequences
    IL_0346:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence>::GetEnumerator()
    IL_034b:  stloc.s    CS$5$0002
    .try
    {
      IL_034d:  br.s       IL_03c1

      IL_034f:  ldloca.s   CS$5$0002
      IL_0351:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sequencer]Pololu.Usc.Sequencer.Sequence>::get_Current()
      IL_0356:  stloc.3
      IL_0357:  nop
      IL_0358:  ldloc.0
      IL_0359:  ldstr      "Sequence"
      IL_035e:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteStartElement(string)
      IL_0363:  nop
      IL_0364:  ldloc.0
      IL_0365:  ldstr      "name"
      IL_036a:  ldloc.3
      IL_036b:  ldfld      string [Sequencer]Pololu.Usc.Sequencer.Sequence::name
      IL_0370:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                                string)
      IL_0375:  nop
      IL_0376:  nop
      IL_0377:  ldloc.3
      IL_0378:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Frame> [Sequencer]Pololu.Usc.Sequencer.Sequence::frames
      IL_037d:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Frame>::GetEnumerator()
      IL_0382:  stloc.s    CS$5$0003
      .try
      {
        IL_0384:  br.s       IL_039a

        IL_0386:  ldloca.s   CS$5$0003
        IL_0388:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sequencer]Pololu.Usc.Sequencer.Frame>::get_Current()
        IL_038d:  stloc.s    frame
        IL_038f:  nop
        IL_0390:  ldloc.s    frame
        IL_0392:  ldloc.0
        IL_0393:  callvirt   instance void [Sequencer]Pololu.Usc.Sequencer.Frame::writeXml(class [System.Xml]System.Xml.XmlWriter)
        IL_0398:  nop
        IL_0399:  nop
        IL_039a:  ldloca.s   CS$5$0003
        IL_039c:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sequencer]Pololu.Usc.Sequencer.Frame>::MoveNext()
        IL_03a1:  stloc.s    CS$4$0000
        IL_03a3:  ldloc.s    CS$4$0000
        IL_03a5:  brtrue.s   IL_0386

        IL_03a7:  leave.s    IL_03b8

      }  // end .try
      finally
      {
        IL_03a9:  ldloca.s   CS$5$0003
        IL_03ab:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sequencer]Pololu.Usc.Sequencer.Frame>
        IL_03b1:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_03b6:  nop
        IL_03b7:  endfinally
      }  // end handler
      IL_03b8:  nop
      IL_03b9:  ldloc.0
      IL_03ba:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteEndElement()
      IL_03bf:  nop
      IL_03c0:  nop
      IL_03c1:  ldloca.s   CS$5$0002
      IL_03c3:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sequencer]Pololu.Usc.Sequencer.Sequence>::MoveNext()
      IL_03c8:  stloc.s    CS$4$0000
      IL_03ca:  ldloc.s    CS$4$0000
      IL_03cc:  brtrue.s   IL_034f

      IL_03ce:  leave.s    IL_03df

    }  // end .try
    finally
    {
      IL_03d0:  ldloca.s   CS$5$0002
      IL_03d2:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class [Sequencer]Pololu.Usc.Sequencer.Sequence>
      IL_03d8:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_03dd:  nop
      IL_03de:  endfinally
    }  // end handler
    IL_03df:  nop
    IL_03e0:  ldloc.0
    IL_03e1:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteEndElement()
    IL_03e6:  nop
    IL_03e7:  ldloc.0
    IL_03e8:  ldstr      "Script"
    IL_03ed:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteStartElement(string)
    IL_03f2:  nop
    IL_03f3:  ldloc.0
    IL_03f4:  ldstr      "ScriptDone"
    IL_03f9:  ldarg.0
    IL_03fa:  ldfld      bool Pololu.Usc.UscSettings::scriptDone
    IL_03ff:  brtrue.s   IL_0408

    IL_0401:  ldstr      "false"
    IL_0406:  br.s       IL_040d

    IL_0408:  ldstr      "true"
    IL_040d:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteAttributeString(string,
                                                                                              string)
    IL_0412:  nop
    IL_0413:  ldloc.0
    IL_0414:  ldarg.0
    IL_0415:  callvirt   instance string Pololu.Usc.UscSettings::get_script()
    IL_041a:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteString(string)
    IL_041f:  nop
    IL_0420:  ldloc.0
    IL_0421:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteEndElement()
    IL_0426:  nop
    IL_0427:  ldloc.0
    IL_0428:  callvirt   instance void [System.Xml]System.Xml.XmlWriter::WriteEndElement()
    IL_042d:  nop
    IL_042e:  ret
  } // end of method ConfigurationFile::save

} // end of class Pololu.Usc.ConfigurationFile

.class public auto ansi beforefieldinit Pololu.Usc.UscSettings
       extends [mscorlib]System.Object
{
  .field public uint8 servosAvailable
  .field public uint8 servoPeriod
  .field public uint32 miniMaestroServoPeriod
  .field public uint16 servoMultiplier
  .field public valuetype Pololu.Usc.uscSerialMode serialMode
  .field public uint32 fixedBaudRate
  .field public bool enableCrc
  .field public bool neverSuspend
  .field public uint8 serialDeviceNumber
  .field public uint8 miniSscOffset
  .field public uint16 serialTimeout
  .field public bool scriptDone
  .field public class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting> channelSettings
  .field public bool enablePullups
  .field public bool scriptInconsistent
  .field private string privateScript
  .field private class [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram privateProgram
  .field public class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence> sequences
  .method public hidebysig specialname instance string 
          get_script() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init ([0] string CS$1$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      string Pololu.Usc.UscSettings::privateScript
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method UscSettings::get_script

  .method public hidebysig instance void 
          setAndCompileScript(string script) cil managed
  {
    // Code size       40 (0x28)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  stfld      string Pololu.Usc.UscSettings::privateScript
    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  call       instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_0010:  ldc.i4.6
    IL_0011:  ceq
    IL_0013:  ldc.i4.0
    IL_0014:  ceq
    IL_0016:  call       class [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram [Bytecode]Pololu.Usc.Bytecode.BytecodeReader::Read(string,
                                                                                                                                bool)
    IL_001b:  stfld      class [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram Pololu.Usc.UscSettings::privateProgram
    IL_0020:  ldarg.0
    IL_0021:  ldarg.1
    IL_0022:  stfld      string Pololu.Usc.UscSettings::privateScript
    IL_0027:  ret
  } // end of method UscSettings::setAndCompileScript

  .method public hidebysig specialname instance valuetype [mscorlib]System.Decimal 
          get_periodInMicroseconds() cil managed
  {
    // Code size       57 (0x39)
    .maxstack  2
    .locals init ([0] valuetype [mscorlib]System.Decimal CS$1$0000,
             [1] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_0007:  ldc.i4.6
    IL_0008:  ceq
    IL_000a:  ldc.i4.0
    IL_000b:  ceq
    IL_000d:  stloc.1
    IL_000e:  ldloc.1
    IL_000f:  brtrue.s   IL_0026

    IL_0011:  nop
    IL_0012:  ldarg.0
    IL_0013:  ldfld      uint8 Pololu.Usc.UscSettings::servoPeriod
    IL_0018:  ldarg.0
    IL_0019:  ldfld      uint8 Pololu.Usc.UscSettings::servosAvailable
    IL_001e:  call       valuetype [mscorlib]System.Decimal Pololu.Usc.Usc::periodToMicroseconds(uint16,
                                                                                                 uint8)
    IL_0023:  stloc.0
    IL_0024:  br.s       IL_0037

    IL_0026:  nop
    IL_0027:  ldarg.0
    IL_0028:  ldfld      uint32 Pololu.Usc.UscSettings::miniMaestroServoPeriod
    IL_002d:  ldc.i4.4
    IL_002e:  div.un
    IL_002f:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(uint32)
    IL_0034:  stloc.0
    IL_0035:  br.s       IL_0037

    IL_0037:  ldloc.0
    IL_0038:  ret
  } // end of method UscSettings::get_periodInMicroseconds

  .method public hidebysig specialname instance class [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram 
          get_bytecodeProgram() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init ([0] class [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram CS$1$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      class [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram Pololu.Usc.UscSettings::privateProgram
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method UscSettings::get_bytecodeProgram

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       150 (0x96)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.6
    IL_0002:  stfld      uint8 Pololu.Usc.UscSettings::servosAvailable
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4     0x9c
    IL_000d:  stfld      uint8 Pololu.Usc.UscSettings::servoPeriod
    IL_0012:  ldarg.0
    IL_0013:  ldc.i4     0x13880
    IL_0018:  stfld      uint32 Pololu.Usc.UscSettings::miniMaestroServoPeriod
    IL_001d:  ldarg.0
    IL_001e:  ldc.i4.1
    IL_001f:  stfld      uint16 Pololu.Usc.UscSettings::servoMultiplier
    IL_0024:  ldarg.0
    IL_0025:  ldc.i4.2
    IL_0026:  stfld      valuetype Pololu.Usc.uscSerialMode Pololu.Usc.UscSettings::serialMode
    IL_002b:  ldarg.0
    IL_002c:  ldc.i4     0x2580
    IL_0031:  stfld      uint32 Pololu.Usc.UscSettings::fixedBaudRate
    IL_0036:  ldarg.0
    IL_0037:  ldc.i4.0
    IL_0038:  stfld      bool Pololu.Usc.UscSettings::enableCrc
    IL_003d:  ldarg.0
    IL_003e:  ldc.i4.0
    IL_003f:  stfld      bool Pololu.Usc.UscSettings::neverSuspend
    IL_0044:  ldarg.0
    IL_0045:  ldc.i4.s   12
    IL_0047:  stfld      uint8 Pololu.Usc.UscSettings::serialDeviceNumber
    IL_004c:  ldarg.0
    IL_004d:  ldc.i4.0
    IL_004e:  stfld      uint8 Pololu.Usc.UscSettings::miniSscOffset
    IL_0053:  ldarg.0
    IL_0054:  ldc.i4.0
    IL_0055:  stfld      uint16 Pololu.Usc.UscSettings::serialTimeout
    IL_005a:  ldarg.0
    IL_005b:  ldc.i4.1
    IL_005c:  stfld      bool Pololu.Usc.UscSettings::scriptDone
    IL_0061:  ldarg.0
    IL_0062:  ldc.i4.0
    IL_0063:  stfld      bool Pololu.Usc.UscSettings::enablePullups
    IL_0068:  ldarg.0
    IL_0069:  ldc.i4.0
    IL_006a:  stfld      bool Pololu.Usc.UscSettings::scriptInconsistent
    IL_006f:  ldarg.0
    IL_0070:  ldnull
    IL_0071:  stfld      string Pololu.Usc.UscSettings::privateScript
    IL_0076:  ldarg.0
    IL_0077:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence>::.ctor()
    IL_007c:  stfld      class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence> Pololu.Usc.UscSettings::sequences
    IL_0081:  ldarg.0
    IL_0082:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0087:  nop
    IL_0088:  nop
    IL_0089:  ldarg.0
    IL_008a:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting>::.ctor()
    IL_008f:  stfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting> Pololu.Usc.UscSettings::channelSettings
    IL_0094:  nop
    IL_0095:  ret
  } // end of method UscSettings::.ctor

  .method public hidebysig specialname instance uint8 
          get_servoCount() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  1
    .locals init ([0] uint8 CS$1$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting> Pololu.Usc.UscSettings::channelSettings
    IL_0007:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting>::get_Count()
    IL_000c:  conv.u1
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method UscSettings::get_servoCount

  .property instance string script()
  {
    .get instance string Pololu.Usc.UscSettings::get_script()
  } // end of property UscSettings::script
  .property instance valuetype [mscorlib]System.Decimal
          periodInMicroseconds()
  {
    .get instance valuetype [mscorlib]System.Decimal Pololu.Usc.UscSettings::get_periodInMicroseconds()
  } // end of property UscSettings::periodInMicroseconds
  .property instance class [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram
          bytecodeProgram()
  {
    .get instance class [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram Pololu.Usc.UscSettings::get_bytecodeProgram()
  } // end of property UscSettings::bytecodeProgram
  .property instance uint8 servoCount()
  {
    .get instance uint8 Pololu.Usc.UscSettings::get_servoCount()
  } // end of property UscSettings::servoCount
} // end of class Pololu.Usc.UscSettings

.class public auto ansi beforefieldinit Pololu.Usc.ChannelSetting
       extends [mscorlib]System.Object
{
  .field public string name
  .field public valuetype Pololu.Usc.ChannelMode mode
  .field public valuetype Pololu.Usc.HomeMode homeMode
  .field public uint16 home
  .field public uint16 minimum
  .field public uint16 maximum
  .field public uint16 neutral
  .field public uint16 range
  .field public uint16 speed
  .field public uint8 acceleration
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       102 (0x66)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldstr      ""
    IL_0006:  stfld      string Pololu.Usc.ChannelSetting::name
    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.0
    IL_000d:  stfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.0
    IL_0014:  stfld      valuetype Pololu.Usc.HomeMode Pololu.Usc.ChannelSetting::homeMode
    IL_0019:  ldarg.0
    IL_001a:  ldc.i4     0x1770
    IL_001f:  stfld      uint16 Pololu.Usc.ChannelSetting::home
    IL_0024:  ldarg.0
    IL_0025:  ldc.i4     0xf80
    IL_002a:  stfld      uint16 Pololu.Usc.ChannelSetting::minimum
    IL_002f:  ldarg.0
    IL_0030:  ldc.i4     0x1f40
    IL_0035:  stfld      uint16 Pololu.Usc.ChannelSetting::maximum
    IL_003a:  ldarg.0
    IL_003b:  ldc.i4     0x1770
    IL_0040:  stfld      uint16 Pololu.Usc.ChannelSetting::neutral
    IL_0045:  ldarg.0
    IL_0046:  ldc.i4     0x771
    IL_004b:  stfld      uint16 Pololu.Usc.ChannelSetting::range
    IL_0050:  ldarg.0
    IL_0051:  ldc.i4.0
    IL_0052:  stfld      uint16 Pololu.Usc.ChannelSetting::speed
    IL_0057:  ldarg.0
    IL_0058:  ldc.i4.0
    IL_0059:  stfld      uint8 Pololu.Usc.ChannelSetting::acceleration
    IL_005e:  ldarg.0
    IL_005f:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0064:  nop
    IL_0065:  ret
  } // end of method ChannelSetting::.ctor

} // end of class Pololu.Usc.ChannelSetting

.class public auto ansi sealed Pololu.Usc.ChannelMode
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype Pololu.Usc.ChannelMode Servo = int32(0x00000000)
  .field public static literal valuetype Pololu.Usc.ChannelMode ServoMultiplied = int32(0x00000001)
  .field public static literal valuetype Pololu.Usc.ChannelMode Output = int32(0x00000002)
  .field public static literal valuetype Pololu.Usc.ChannelMode Input = int32(0x00000003)
} // end of class Pololu.Usc.ChannelMode

.class public auto ansi sealed Pololu.Usc.HomeMode
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype Pololu.Usc.HomeMode Off = int32(0x00000000)
  .field public static literal valuetype Pololu.Usc.HomeMode Ignore = int32(0x00000001)
  .field public static literal valuetype Pololu.Usc.HomeMode Goto = int32(0x00000002)
} // end of class Pololu.Usc.HomeMode

.class interface public abstract auto ansi Pololu.Usc.IUscSettingsHolder
{
  .method public hidebysig newslot abstract virtual 
          instance void  setUscSettings(class Pololu.Usc.UscSettings settings,
                                        bool newScript) cil managed
  {
  } // end of method IUscSettingsHolder::setUscSettings

  .method public hidebysig newslot abstract virtual 
          instance class Pololu.Usc.UscSettings 
          getUscSettings() cil managed
  {
  } // end of method IUscSettingsHolder::getUscSettings

} // end of class Pololu.Usc.IUscSettingsHolder

.class public auto ansi beforefieldinit Pololu.Usc.Usc
       extends [UsbWrapper]Pololu.UsbWrapper.UsbDevice
       implements Pololu.Usc.IUscSettingsHolder
{
  .class sequential ansi sealed nested family beforefieldinit Range
         extends [mscorlib]System.ValueType
  {
    .field public uint8 bytes
    .field public int32 minimumValue
    .field public int32 maximumValue
    .field static assembly valuetype Pololu.Usc.Usc/Range u32
    .field static assembly valuetype Pololu.Usc.Usc/Range u16
    .field static assembly valuetype Pololu.Usc.Usc/Range u12
    .field static assembly valuetype Pololu.Usc.Usc/Range u10
    .field static assembly valuetype Pololu.Usc.Usc/Range u8
    .field static assembly valuetype Pololu.Usc.Usc/Range u7
    .field static assembly valuetype Pololu.Usc.Usc/Range boolean
    .method assembly hidebysig specialname rtspecialname 
            instance void  .ctor(uint8 bytes,
                                 int32 minimumValue,
                                 int32 maximumValue) cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  stfld      uint8 Pololu.Usc.Usc/Range::bytes
      IL_0008:  ldarg.0
      IL_0009:  ldarg.2
      IL_000a:  stfld      int32 Pololu.Usc.Usc/Range::minimumValue
      IL_000f:  ldarg.0
      IL_0010:  ldarg.3
      IL_0011:  stfld      int32 Pololu.Usc.Usc/Range::maximumValue
      IL_0016:  ret
    } // end of method Range::.ctor

    .method public hidebysig specialname 
            instance bool  get_signed() cil managed
    {
      // Code size       15 (0xf)
      .maxstack  2
      .locals init ([0] bool CS$1$0000)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Pololu.Usc.Usc/Range::minimumValue
      IL_0007:  ldc.i4.0
      IL_0008:  clt
      IL_000a:  stloc.0
      IL_000b:  br.s       IL_000d

      IL_000d:  ldloc.0
      IL_000e:  ret
    } // end of method Range::get_signed

    .method private hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       113 (0x71)
      .maxstack  4
      IL_0000:  ldc.i4.4
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4     0x7fffffff
      IL_0007:  newobj     instance void Pololu.Usc.Usc/Range::.ctor(uint8,
                                                                     int32,
                                                                     int32)
      IL_000c:  stsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u32
      IL_0011:  ldc.i4.2
      IL_0012:  ldc.i4.0
      IL_0013:  ldc.i4     0xffff
      IL_0018:  newobj     instance void Pololu.Usc.Usc/Range::.ctor(uint8,
                                                                     int32,
                                                                     int32)
      IL_001d:  stsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u16
      IL_0022:  ldc.i4.2
      IL_0023:  ldc.i4.0
      IL_0024:  ldc.i4     0xfff
      IL_0029:  newobj     instance void Pololu.Usc.Usc/Range::.ctor(uint8,
                                                                     int32,
                                                                     int32)
      IL_002e:  stsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u12
      IL_0033:  ldc.i4.2
      IL_0034:  ldc.i4.0
      IL_0035:  ldc.i4     0x3ff
      IL_003a:  newobj     instance void Pololu.Usc.Usc/Range::.ctor(uint8,
                                                                     int32,
                                                                     int32)
      IL_003f:  stsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u10
      IL_0044:  ldc.i4.1
      IL_0045:  ldc.i4.0
      IL_0046:  ldc.i4     0xff
      IL_004b:  newobj     instance void Pololu.Usc.Usc/Range::.ctor(uint8,
                                                                     int32,
                                                                     int32)
      IL_0050:  stsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u8
      IL_0055:  ldc.i4.1
      IL_0056:  ldc.i4.0
      IL_0057:  ldc.i4.s   127
      IL_0059:  newobj     instance void Pololu.Usc.Usc/Range::.ctor(uint8,
                                                                     int32,
                                                                     int32)
      IL_005e:  stsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u7
      IL_0063:  ldc.i4.1
      IL_0064:  ldc.i4.0
      IL_0065:  ldc.i4.1
      IL_0066:  newobj     instance void Pololu.Usc.Usc/Range::.ctor(uint8,
                                                                     int32,
                                                                     int32)
      IL_006b:  stsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::boolean
      IL_0070:  ret
    } // end of method Range::.cctor

    .property instance bool signed()
    {
      .get instance bool Pololu.Usc.Usc/Range::get_signed()
    } // end of property Range::signed
  } // end of class Range

  .field public static literal uint16 vendorID = uint16(0x1FFB)
  .field public static literal string englishName = "Maestro USB servo controller"
  .field private static literal int32 INSTRUCTION_FREQUENCY = int32(0x00B71B00)
  .field private static literal uint8 servoParameterBytes = uint8(0x09)
  .field public static literal int32 MicroMaestroStackSize = int32(0x00000020)
  .field public static literal int32 MicroMaestroCallStackSize = int32(0x0000000A)
  .field public static literal int32 MiniMaestroStackSize = int32(0x0000007E)
  .field public static literal int32 MiniMaestroCallStackSize = int32(0x0000007E)
  .field public static valuetype [mscorlib]System.Guid deviceInterfaceGuid
  .field public static uint16[] productIDArray
  .field public initonly uint8 servoCount
  .field private uint8 privateFirmwareVersionMajor
  .field private uint8 privateFirmwareVersionMinor
  .method public hidebysig specialname static 
          string[]  get_bootloaderDeviceInstanceIdPrefixes() cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  3
    .locals init ([0] string[] CS$1$0000,
             [1] string[] CS$0$0001)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  newarr     [mscorlib]System.String
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  ldstr      "USB\\VID_1FFB&PID_0088"
    IL_000f:  stelem.ref
    IL_0010:  ldloc.1
    IL_0011:  ldc.i4.1
    IL_0012:  ldstr      "USB\\VID_1FFB&PID_008D"
    IL_0017:  stelem.ref
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.2
    IL_001a:  ldstr      "USB\\VID_1FFB&PID_008E"
    IL_001f:  stelem.ref
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.3
    IL_0022:  ldstr      "USB\\VID_1FFB&PID_008F"
    IL_0027:  stelem.ref
    IL_0028:  ldloc.1
    IL_0029:  stloc.0
    IL_002a:  br.s       IL_002c

    IL_002c:  ldloc.0
    IL_002d:  ret
  } // end of method Usc::get_bootloaderDeviceInstanceIdPrefixes

  .method public hidebysig specialname static 
          string  get_shortProductName() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init ([0] string CS$1$0000)
    IL_0000:  nop
    IL_0001:  ldstr      "Maestro"
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method Usc::get_shortProductName

  .method private hidebysig static uint16 
          exponentialSpeedToNormalSpeed(uint8 exponentialSpeed) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  4
    .locals init ([0] int32 mantissa,
             [1] int32 exponent,
             [2] uint16 CS$1$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.3
    IL_0003:  shr
    IL_0004:  stloc.0
    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.7
    IL_0007:  and
    IL_0008:  stloc.1
    IL_0009:  ldloc.0
    IL_000a:  ldc.i4.1
    IL_000b:  ldloc.1
    IL_000c:  ldc.i4.s   31
    IL_000e:  and
    IL_000f:  shl
    IL_0010:  mul
    IL_0011:  conv.u2
    IL_0012:  stloc.2
    IL_0013:  br.s       IL_0015

    IL_0015:  ldloc.2
    IL_0016:  ret
  } // end of method Usc::exponentialSpeedToNormalSpeed

  .method private hidebysig static uint8 
          normalSpeedToExponentialSpeed(uint16 normalSpeed) cil managed
  {
    // Code size       67 (0x43)
    .maxstack  3
    .locals init ([0] uint16 mantissa,
             [1] uint8 exponent,
             [2] uint8 CS$1$0000,
             [3] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_003d

    IL_0007:  nop
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.s   32
    IL_000b:  clt
    IL_000d:  ldc.i4.0
    IL_000e:  ceq
    IL_0010:  stloc.3
    IL_0011:  ldloc.3
    IL_0012:  brtrue.s   IL_001e

    IL_0014:  nop
    IL_0015:  ldloc.1
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.3
    IL_0018:  shl
    IL_0019:  add
    IL_001a:  conv.u1
    IL_001b:  stloc.2
    IL_001c:  br.s       IL_0041

    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.7
    IL_0020:  ceq
    IL_0022:  ldc.i4.0
    IL_0023:  ceq
    IL_0025:  stloc.3
    IL_0026:  ldloc.3
    IL_0027:  brtrue.s   IL_0032

    IL_0029:  nop
    IL_002a:  ldc.i4     0xff
    IL_002f:  stloc.2
    IL_0030:  br.s       IL_0041

    IL_0032:  ldloc.1
    IL_0033:  ldc.i4.1
    IL_0034:  add
    IL_0035:  conv.u1
    IL_0036:  stloc.1
    IL_0037:  ldloc.0
    IL_0038:  ldc.i4.1
    IL_0039:  shr
    IL_003a:  conv.u2
    IL_003b:  stloc.0
    IL_003c:  nop
    IL_003d:  ldc.i4.1
    IL_003e:  stloc.3
    IL_003f:  br.s       IL_0007

    IL_0041:  ldloc.2
    IL_0042:  ret
  } // end of method Usc::normalSpeedToExponentialSpeed

  .method public hidebysig static valuetype [mscorlib]System.Decimal 
          positionToMicroseconds(uint16 position) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init ([0] valuetype [mscorlib]System.Decimal CS$1$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(uint16)
    IL_0007:  ldc.i4.4
    IL_0008:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_000d:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0012:  stloc.0
    IL_0013:  br.s       IL_0015

    IL_0015:  ldloc.0
    IL_0016:  ret
  } // end of method Usc::positionToMicroseconds

  .method public hidebysig static uint16 
          microsecondsToPosition(valuetype [mscorlib]System.Decimal us) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init ([0] uint16 CS$1$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.4
    IL_0003:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0008:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_000d:  call       uint16 [mscorlib]System.Decimal::op_Explicit(valuetype [mscorlib]System.Decimal)
    IL_0012:  stloc.0
    IL_0013:  br.s       IL_0015

    IL_0015:  ldloc.0
    IL_0016:  ret
  } // end of method Usc::microsecondsToPosition

  .method public hidebysig static valuetype [mscorlib]System.Decimal 
          periodToMicroseconds(uint16 period,
                               uint8 servos_available) cil managed
  {
    // Code size       50 (0x32)
    .maxstack  3
    .locals init ([0] valuetype [mscorlib]System.Decimal CS$1$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(uint16)
    IL_0007:  ldc.i4     0x100
    IL_000c:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0011:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0016:  ldarg.1
    IL_0017:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(uint8)
    IL_001c:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0021:  ldc.i4.s   12
    IL_0023:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0028:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_002d:  stloc.0
    IL_002e:  br.s       IL_0030

    IL_0030:  ldloc.0
    IL_0031:  ret
  } // end of method Usc::periodToMicroseconds

  .method public hidebysig static uint8  microsecondsToPeriod(valuetype [mscorlib]System.Decimal us,
                                                              uint8 servos_avaiable) cil managed
  {
    // Code size       55 (0x37)
    .maxstack  3
    .locals init ([0] uint8 CS$1$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4     0x100
    IL_0007:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_000c:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0011:  ldc.i4.s   12
    IL_0013:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0018:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_001d:  ldarg.1
    IL_001e:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(uint8)
    IL_0023:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0028:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Math::Round(valuetype [mscorlib]System.Decimal)
    IL_002d:  call       uint8 [mscorlib]System.Decimal::op_Explicit(valuetype [mscorlib]System.Decimal)
    IL_0032:  stloc.0
    IL_0033:  br.s       IL_0035

    IL_0035:  ldloc.0
    IL_0036:  ret
  } // end of method Usc::microsecondsToPeriod

  .method private hidebysig static uint32 
          convertSpbrgToBps(uint16 spbrg) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  3
    .locals init ([0] uint32 CS$1$0000,
             [1] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ceq
    IL_0005:  ldc.i4.0
    IL_0006:  ceq
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  brtrue.s   IL_0011

    IL_000c:  nop
    IL_000d:  ldc.i4.0
    IL_000e:  stloc.0
    IL_000f:  br.s       IL_0023

    IL_0011:  ldc.i4     0xb71b00
    IL_0016:  ldarg.0
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  ldc.i4.2
    IL_001a:  div
    IL_001b:  add
    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.1
    IL_001e:  add
    IL_001f:  div
    IL_0020:  stloc.0
    IL_0021:  br.s       IL_0023

    IL_0023:  ldloc.0
    IL_0024:  ret
  } // end of method Usc::convertSpbrgToBps

  .method private hidebysig static uint16 
          convertBpsToSpbrg(uint32 bps) cil managed
  {
    // Code size       34 (0x22)
    .maxstack  3
    .locals init ([0] uint16 CS$1$0000,
             [1] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ceq
    IL_0005:  ldc.i4.0
    IL_0006:  ceq
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  brtrue.s   IL_0011

    IL_000c:  nop
    IL_000d:  ldc.i4.0
    IL_000e:  stloc.0
    IL_000f:  br.s       IL_0020

    IL_0011:  ldc.i4     0xb71b00
    IL_0016:  ldarg.0
    IL_0017:  ldc.i4.2
    IL_0018:  div.un
    IL_0019:  sub
    IL_001a:  ldarg.0
    IL_001b:  div.un
    IL_001c:  conv.u2
    IL_001d:  stloc.0
    IL_001e:  br.s       IL_0020

    IL_0020:  ldloc.0
    IL_0021:  ret
  } // end of method Usc::convertBpsToSpbrg

  .method private hidebysig instance uint8 
          channelToPort(uint8 channel) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  2
    .locals init ([0] uint8 CS$1$0000,
             [1] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.3
    IL_0003:  cgt
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  brtrue.s   IL_000e

    IL_0009:  nop
    IL_000a:  ldarg.1
    IL_000b:  stloc.0
    IL_000c:  br.s       IL_0037

    IL_000e:  ldarg.1
    IL_000f:  ldc.i4.6
    IL_0010:  clt
    IL_0012:  ldc.i4.0
    IL_0013:  ceq
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  brtrue.s   IL_0021

    IL_0019:  nop
    IL_001a:  ldarg.1
    IL_001b:  ldc.i4.2
    IL_001c:  add
    IL_001d:  conv.u1
    IL_001e:  stloc.0
    IL_001f:  br.s       IL_0037

    IL_0021:  ldstr      "Invalid channel number "
    IL_0026:  ldarg.1
    IL_0027:  box        [mscorlib]System.Byte
    IL_002c:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0031:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string)
    IL_0036:  throw

    IL_0037:  ldloc.0
    IL_0038:  ret
  } // end of method Usc::channelToPort

  .method private hidebysig instance valuetype Pololu.Usc.uscParameter 
          specifyServo(valuetype Pololu.Usc.uscParameter p,
                       uint8 servo) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  3
    .locals init ([0] valuetype Pololu.Usc.uscParameter CS$1$0000)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldc.i4.s   9
    IL_0005:  mul
    IL_0006:  add
    IL_0007:  conv.u1
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method Usc::specifyServo

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [UsbWrapper]Pololu.UsbWrapper.DeviceListItem deviceListItem) cil managed
  {
    // Code size       149 (0x95)
    .maxstack  3
    .locals init ([0] uint16 CS$4$0000,
             [1] uint16 CS$0$0001)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4     0xff
    IL_0006:  stfld      uint8 Pololu.Usc.Usc::privateFirmwareVersionMajor
    IL_000b:  ldarg.0
    IL_000c:  ldc.i4     0xff
    IL_0011:  stfld      uint8 Pololu.Usc.Usc::privateFirmwareVersionMinor
    IL_0016:  ldarg.0
    IL_0017:  ldarg.1
    IL_0018:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::.ctor(class [UsbWrapper]Pololu.UsbWrapper.DeviceListItem)
    IL_001d:  nop
    IL_001e:  nop
    IL_001f:  ldarg.0
    IL_0020:  call       instance uint16 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::getProductID()
    IL_0025:  stloc.0
    IL_0026:  ldloc.0
    IL_0027:  ldc.i4     0x89
    IL_002c:  sub
    IL_002d:  switch     ( 
                          IL_0044,
                          IL_004d,
                          IL_0057,
                          IL_0061)
    IL_0042:  br.s       IL_006b

    IL_0044:  ldarg.0
    IL_0045:  ldc.i4.6
    IL_0046:  stfld      uint8 Pololu.Usc.Usc::servoCount
    IL_004b:  br.s       IL_0093

    IL_004d:  ldarg.0
    IL_004e:  ldc.i4.s   12
    IL_0050:  stfld      uint8 Pololu.Usc.Usc::servoCount
    IL_0055:  br.s       IL_0093

    IL_0057:  ldarg.0
    IL_0058:  ldc.i4.s   18
    IL_005a:  stfld      uint8 Pololu.Usc.Usc::servoCount
    IL_005f:  br.s       IL_0093

    IL_0061:  ldarg.0
    IL_0062:  ldc.i4.s   24
    IL_0064:  stfld      uint8 Pololu.Usc.Usc::servoCount
    IL_0069:  br.s       IL_0093

    IL_006b:  ldstr      "Unknown product id "
    IL_0070:  ldarg.0
    IL_0071:  call       instance uint16 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::getProductID()
    IL_0076:  stloc.1
    IL_0077:  ldloca.s   CS$0$0001
    IL_0079:  ldstr      "x2"
    IL_007e:  call       instance string [mscorlib]System.UInt16::ToString(string)
    IL_0083:  ldstr      "."
    IL_0088:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_008d:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0092:  throw

    IL_0093:  nop
    IL_0094:  ret
  } // end of method Usc::.ctor

  .method family hidebysig specialname instance bool 
          get_microMaestro() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  2
    .locals init ([0] bool CS$1$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_0007:  ldc.i4.6
    IL_0008:  ceq
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method Usc::get_microMaestro

  .method public hidebysig specialname instance uint8 
          get_stackSize() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  2
    .locals init ([0] uint8 CS$1$0000,
             [1] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  stloc.1
    IL_000b:  ldloc.1
    IL_000c:  brtrue.s   IL_0014

    IL_000e:  nop
    IL_000f:  ldc.i4.s   32
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_001a

    IL_0014:  nop
    IL_0015:  ldc.i4.s   126
    IL_0017:  stloc.0
    IL_0018:  br.s       IL_001a

    IL_001a:  ldloc.0
    IL_001b:  ret
  } // end of method Usc::get_stackSize

  .method public hidebysig specialname instance uint8 
          get_callStackSize() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  2
    .locals init ([0] uint8 CS$1$0000,
             [1] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  stloc.1
    IL_000b:  ldloc.1
    IL_000c:  brtrue.s   IL_0014

    IL_000e:  nop
    IL_000f:  ldc.i4.s   10
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_001a

    IL_0014:  nop
    IL_0015:  ldc.i4.s   126
    IL_0017:  stloc.0
    IL_0018:  br.s       IL_001a

    IL_001a:  ldloc.0
    IL_001b:  ret
  } // end of method Usc::get_callStackSize

  .method public hidebysig static class [mscorlib]System.Collections.Generic.List`1<class [UsbWrapper]Pololu.UsbWrapper.DeviceListItem> 
          getConnectedDevices() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  2
    .locals init ([0] class [mscorlib]System.Collections.Generic.List`1<class [UsbWrapper]Pololu.UsbWrapper.DeviceListItem> CS$1$0000)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldsfld     valuetype [mscorlib]System.Guid Pololu.Usc.Usc::deviceInterfaceGuid
      IL_0007:  call       class [mscorlib_5]System.Collections.Generic.List`1<class [UsbWrapper]Pololu.UsbWrapper.DeviceListItem> [UsbWrapper]Pololu.UsbWrapper.UsbDevice::getDeviceList(valuetype [mscorlib_5]System.Guid)
      IL_000c:  stloc.0
      IL_000d:  leave.s    IL_0023

    }  // end .try
    catch [mscorlib]System.NotImplementedException 
    {
      IL_000f:  pop
      IL_0010:  nop
      IL_0011:  ldc.i4     0x1ffb
      IL_0016:  ldsfld     uint16[] Pololu.Usc.Usc::productIDArray
      IL_001b:  call       class [mscorlib_5]System.Collections.Generic.List`1<class [UsbWrapper]Pololu.UsbWrapper.DeviceListItem> [UsbWrapper]Pololu.UsbWrapper.UsbDevice::getDeviceList(uint32,
                                                                                                                                                                                          uint16[])
      IL_0020:  stloc.0
      IL_0021:  leave.s    IL_0023

    }  // end handler
    IL_0023:  nop
    IL_0024:  ldloc.0
    IL_0025:  ret
  } // end of method Usc::getConnectedDevices

  .method public hidebysig specialname instance uint16 
          get_firmwareVersionMajor() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  2
    .locals init ([0] uint16 CS$1$0000,
             [1] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint8 Pololu.Usc.Usc::privateFirmwareVersionMajor
    IL_0007:  ldc.i4     0xff
    IL_000c:  ceq
    IL_000e:  ldc.i4.0
    IL_000f:  ceq
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  brtrue.s   IL_001e

    IL_0015:  nop
    IL_0016:  ldarg.0
    IL_0017:  call       instance void Pololu.Usc.Usc::getFirmwareVersion()
    IL_001c:  nop
    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  ldfld      uint8 Pololu.Usc.Usc::privateFirmwareVersionMajor
    IL_0024:  stloc.0
    IL_0025:  br.s       IL_0027

    IL_0027:  ldloc.0
    IL_0028:  ret
  } // end of method Usc::get_firmwareVersionMajor

  .method public hidebysig specialname instance uint8 
          get_firmwareVersionMinor() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  2
    .locals init ([0] uint8 CS$1$0000,
             [1] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint8 Pololu.Usc.Usc::privateFirmwareVersionMajor
    IL_0007:  ldc.i4     0xff
    IL_000c:  ceq
    IL_000e:  ldc.i4.0
    IL_000f:  ceq
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  brtrue.s   IL_001e

    IL_0015:  nop
    IL_0016:  ldarg.0
    IL_0017:  call       instance void Pololu.Usc.Usc::getFirmwareVersion()
    IL_001c:  nop
    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  ldfld      uint8 Pololu.Usc.Usc::privateFirmwareVersionMinor
    IL_0024:  stloc.0
    IL_0025:  br.s       IL_0027

    IL_0027:  ldloc.0
    IL_0028:  ret
  } // end of method Usc::get_firmwareVersionMinor

  .method public hidebysig specialname instance string 
          get_firmwareVersionString() cil managed
  {
    // Code size       49 (0x31)
    .maxstack  4
    .locals init ([0] string CS$1$0000,
             [1] uint16 CS$0$0001,
             [2] uint8 CS$0$0002)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance uint16 Pololu.Usc.Usc::get_firmwareVersionMajor()
    IL_0007:  stloc.1
    IL_0008:  ldloca.s   CS$0$0001
    IL_000a:  call       instance string [mscorlib]System.UInt16::ToString()
    IL_000f:  ldstr      "."
    IL_0014:  ldarg.0
    IL_0015:  call       instance uint8 Pololu.Usc.Usc::get_firmwareVersionMinor()
    IL_001a:  stloc.2
    IL_001b:  ldloca.s   CS$0$0002
    IL_001d:  ldstr      "D2"
    IL_0022:  call       instance string [mscorlib]System.Byte::ToString(string)
    IL_0027:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_002c:  stloc.0
    IL_002d:  br.s       IL_002f

    IL_002f:  ldloc.0
    IL_0030:  ret
  } // end of method Usc::get_firmwareVersionString

  .method private hidebysig instance void 
          getFirmwareVersion() cil managed
  {
    // Code size       103 (0x67)
    .maxstack  6
    .locals init ([0] uint8[] buffer,
             [1] class [mscorlib]System.Exception exception)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   14
    IL_0003:  newarr     [mscorlib]System.Byte
    IL_0008:  stloc.0
    .try
    {
      IL_0009:  nop
      IL_000a:  ldarg.0
      IL_000b:  ldc.i4     0x80
      IL_0010:  ldc.i4.6
      IL_0011:  ldc.i4     0x100
      IL_0016:  ldc.i4.0
      IL_0017:  ldloc.0
      IL_0018:  call       instance uint32 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                    uint8,
                                                                                                    uint16,
                                                                                                    uint16,
                                                                                                    uint8[])
      IL_001d:  pop
      IL_001e:  nop
      IL_001f:  leave.s    IL_002f

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0021:  stloc.1
      IL_0022:  nop
      IL_0023:  ldstr      "There was an error getting the firmware version fr"
      + "om the device."
      IL_0028:  ldloc.1
      IL_0029:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_002e:  throw

    }  // end handler
    IL_002f:  nop
    IL_0030:  ldarg.0
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.s   12
    IL_0034:  ldelem.u1
    IL_0035:  ldc.i4.s   15
    IL_0037:  and
    IL_0038:  ldloc.0
    IL_0039:  ldc.i4.s   12
    IL_003b:  ldelem.u1
    IL_003c:  ldc.i4.4
    IL_003d:  shr
    IL_003e:  ldc.i4.s   15
    IL_0040:  and
    IL_0041:  ldc.i4.s   10
    IL_0043:  mul
    IL_0044:  add
    IL_0045:  conv.u1
    IL_0046:  stfld      uint8 Pololu.Usc.Usc::privateFirmwareVersionMinor
    IL_004b:  ldarg.0
    IL_004c:  ldloc.0
    IL_004d:  ldc.i4.s   13
    IL_004f:  ldelem.u1
    IL_0050:  ldc.i4.s   15
    IL_0052:  and
    IL_0053:  ldloc.0
    IL_0054:  ldc.i4.s   13
    IL_0056:  ldelem.u1
    IL_0057:  ldc.i4.4
    IL_0058:  shr
    IL_0059:  ldc.i4.s   15
    IL_005b:  and
    IL_005c:  ldc.i4.s   10
    IL_005e:  mul
    IL_005f:  add
    IL_0060:  conv.u1
    IL_0061:  stfld      uint8 Pololu.Usc.Usc::privateFirmwareVersionMajor
    IL_0066:  ret
  } // end of method Usc::getFirmwareVersion

  .method public hidebysig instance void 
          eraseScript() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Exception e)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.s   64
      IL_0005:  ldc.i4     0xa0
      IL_000a:  ldc.i4.0
      IL_000b:  ldc.i4.0
      IL_000c:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                  uint8,
                                                                                                  uint16,
                                                                                                  uint16)
      IL_0011:  nop
      IL_0012:  nop
      IL_0013:  leave.s    IL_0023

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0015:  stloc.0
      IL_0016:  nop
      IL_0017:  ldstr      "There was an error erasing the script."
      IL_001c:  ldloc.0
      IL_001d:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0022:  throw

    }  // end handler
    IL_0023:  nop
    IL_0024:  ret
  } // end of method Usc::eraseScript

  .method public hidebysig instance void 
          restartScriptAtSubroutine(uint8 subroutine) cil managed
  {
    // Code size       53 (0x35)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Exception e)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.s   64
      IL_0005:  ldc.i4     0xa3
      IL_000a:  ldc.i4.0
      IL_000b:  ldarg.1
      IL_000c:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                  uint8,
                                                                                                  uint16,
                                                                                                  uint16)
      IL_0011:  nop
      IL_0012:  nop
      IL_0013:  leave.s    IL_0033

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0015:  stloc.0
      IL_0016:  nop
      IL_0017:  ldstr      "There was an error restarting the script at subrou"
      + "tine "
      IL_001c:  ldarg.1
      IL_001d:  box        [mscorlib]System.Byte
      IL_0022:  ldstr      "."
      IL_0027:  call       string [mscorlib]System.String::Concat(object,
                                                                  object,
                                                                  object)
      IL_002c:  ldloc.0
      IL_002d:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0032:  throw

    }  // end handler
    IL_0033:  nop
    IL_0034:  ret
  } // end of method Usc::restartScriptAtSubroutine

  .method public hidebysig instance void 
          restartScriptAtSubroutineWithParameter(uint8 subroutine,
                                                 int16 parameter) cil managed
  {
    // Code size       54 (0x36)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Exception e)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.s   64
      IL_0005:  ldc.i4     0xa4
      IL_000a:  ldarg.2
      IL_000b:  conv.u2
      IL_000c:  ldarg.1
      IL_000d:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                  uint8,
                                                                                                  uint16,
                                                                                                  uint16)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  leave.s    IL_0034

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0016:  stloc.0
      IL_0017:  nop
      IL_0018:  ldstr      "There was an error restarting the script with a pa"
      + "rameter at subroutine "
      IL_001d:  ldarg.1
      IL_001e:  box        [mscorlib]System.Byte
      IL_0023:  ldstr      "."
      IL_0028:  call       string [mscorlib]System.String::Concat(object,
                                                                  object,
                                                                  object)
      IL_002d:  ldloc.0
      IL_002e:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0033:  throw

    }  // end handler
    IL_0034:  nop
    IL_0035:  ret
  } // end of method Usc::restartScriptAtSubroutineWithParameter

  .method public hidebysig instance void 
          restartScript() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Exception e)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.s   64
      IL_0005:  ldc.i4     0xa5
      IL_000a:  ldc.i4.0
      IL_000b:  ldc.i4.0
      IL_000c:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                  uint8,
                                                                                                  uint16,
                                                                                                  uint16)
      IL_0011:  nop
      IL_0012:  nop
      IL_0013:  leave.s    IL_0023

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0015:  stloc.0
      IL_0016:  nop
      IL_0017:  ldstr      "There was an error restarting the script."
      IL_001c:  ldloc.0
      IL_001d:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0022:  throw

    }  // end handler
    IL_0023:  nop
    IL_0024:  ret
  } // end of method Usc::restartScript

  .method public hidebysig instance void 
          writeScript(class [mscorlib]System.Collections.Generic.List`1<uint8> bytecode) cil managed
  {
    // Code size       170 (0xaa)
    .maxstack  6
    .locals init ([0] uint16 block,
             [1] uint8[] block_bytes,
             [2] uint16 j,
             [3] class [mscorlib]System.Exception e,
             [4] bool CS$4$0000)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br         IL_0091

    IL_0008:  nop
    IL_0009:  ldc.i4.s   16
    IL_000b:  newarr     [mscorlib]System.Byte
    IL_0010:  stloc.1
    IL_0011:  ldc.i4.0
    IL_0012:  stloc.2
    IL_0013:  br.s       IL_004c

    IL_0015:  nop
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.s   16
    IL_0019:  mul
    IL_001a:  ldloc.2
    IL_001b:  add
    IL_001c:  ldarg.1
    IL_001d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
    IL_0022:  clt
    IL_0024:  ldc.i4.0
    IL_0025:  ceq
    IL_0027:  stloc.s    CS$4$0000
    IL_0029:  ldloc.s    CS$4$0000
    IL_002b:  brtrue.s   IL_003e

    IL_002d:  ldloc.1
    IL_002e:  ldloc.2
    IL_002f:  ldarg.1
    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.s   16
    IL_0033:  mul
    IL_0034:  ldloc.2
    IL_0035:  add
    IL_0036:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Item(int32)
    IL_003b:  stelem.i1
    IL_003c:  br.s       IL_0046

    IL_003e:  ldloc.1
    IL_003f:  ldloc.2
    IL_0040:  ldc.i4     0xff
    IL_0045:  stelem.i1
    IL_0046:  nop
    IL_0047:  ldloc.2
    IL_0048:  ldc.i4.1
    IL_0049:  add
    IL_004a:  conv.u2
    IL_004b:  stloc.2
    IL_004c:  ldloc.2
    IL_004d:  ldc.i4.s   16
    IL_004f:  clt
    IL_0051:  stloc.s    CS$4$0000
    IL_0053:  ldloc.s    CS$4$0000
    IL_0055:  brtrue.s   IL_0015

    .try
    {
      IL_0057:  nop
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.s   64
      IL_005b:  ldc.i4     0xa1
      IL_0060:  ldc.i4.0
      IL_0061:  ldloc.0
      IL_0062:  ldloc.1
      IL_0063:  call       instance uint32 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                    uint8,
                                                                                                    uint16,
                                                                                                    uint16,
                                                                                                    uint8[])
      IL_0068:  pop
      IL_0069:  nop
      IL_006a:  leave.s    IL_008a

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_006c:  stloc.3
      IL_006d:  nop
      IL_006e:  ldstr      "There was an error writing script block "
      IL_0073:  ldloc.0
      IL_0074:  box        [mscorlib]System.UInt16
      IL_0079:  ldstr      "."
      IL_007e:  call       string [mscorlib]System.String::Concat(object,
                                                                  object,
                                                                  object)
      IL_0083:  ldloc.3
      IL_0084:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0089:  throw

    }  // end handler
    IL_008a:  nop
    IL_008b:  nop
    IL_008c:  ldloc.0
    IL_008d:  ldc.i4.1
    IL_008e:  add
    IL_008f:  conv.u2
    IL_0090:  stloc.0
    IL_0091:  ldloc.0
    IL_0092:  ldarg.1
    IL_0093:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
    IL_0098:  ldc.i4.s   15
    IL_009a:  add
    IL_009b:  ldc.i4.s   16
    IL_009d:  div
    IL_009e:  clt
    IL_00a0:  stloc.s    CS$4$0000
    IL_00a2:  ldloc.s    CS$4$0000
    IL_00a4:  brtrue     IL_0008

    IL_00a9:  ret
  } // end of method Usc::writeScript

  .method public hidebysig instance void 
          setSubroutines(class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16> subroutineAddresses,
                         class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8> subroutineCommands) cil managed
  {
    // Code size       324 (0x144)
    .maxstack  6
    .locals init ([0] uint8[] subroutineData,
             [1] uint16 i,
             [2] valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,uint16> kvp,
             [3] string name,
             [4] uint8 bytecode,
             [5] uint16 block,
             [6] uint8[] block_bytes,
             [7] uint16 j,
             [8] class [mscorlib]System.Exception e,
             [9] bool CS$4$0000,
             [10] valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<string,uint16> CS$5$0001)
    IL_0000:  nop
    IL_0001:  ldc.i4     0x100
    IL_0006:  newarr     [mscorlib]System.Byte
    IL_000b:  stloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  stloc.1
    IL_000e:  br.s       IL_001d

    IL_0010:  ldloc.0
    IL_0011:  ldloc.1
    IL_0012:  ldc.i4     0xff
    IL_0017:  stelem.i1
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  conv.u2
    IL_001c:  stloc.1
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4     0x100
    IL_0023:  clt
    IL_0025:  stloc.s    CS$4$0000
    IL_0027:  ldloc.s    CS$4$0000
    IL_0029:  brtrue.s   IL_0010

    IL_002b:  nop
    IL_002c:  ldarg.1
    IL_002d:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<!0,!1> class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16>::GetEnumerator()
    IL_0032:  stloc.s    CS$5$0001
    .try
    {
      IL_0034:  br.s       IL_0094

      IL_0036:  ldloca.s   CS$5$0001
      IL_0038:  call       instance valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<!0,!1> valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<string,uint16>::get_Current()
      IL_003d:  stloc.2
      IL_003e:  nop
      IL_003f:  ldloca.s   kvp
      IL_0041:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,uint16>::get_Key()
      IL_0046:  stloc.3
      IL_0047:  ldarg.2
      IL_0048:  ldloc.3
      IL_0049:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8>::get_Item(!0)
      IL_004e:  stloc.s    bytecode
      IL_0050:  ldloc.s    bytecode
      IL_0052:  ldc.i4.s   54
      IL_0054:  ceq
      IL_0056:  ldc.i4.0
      IL_0057:  ceq
      IL_0059:  stloc.s    CS$4$0000
      IL_005b:  ldloc.s    CS$4$0000
      IL_005d:  brtrue.s   IL_0061

      IL_005f:  br.s       IL_0094

      IL_0061:  ldloc.0
      IL_0062:  ldc.i4.2
      IL_0063:  ldloc.s    bytecode
      IL_0065:  ldc.i4     0x80
      IL_006a:  sub
      IL_006b:  mul
      IL_006c:  ldloca.s   kvp
      IL_006e:  call       instance !1 valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,uint16>::get_Value()
      IL_0073:  ldc.i4     0x100
      IL_0078:  rem
      IL_0079:  conv.u1
      IL_007a:  stelem.i1
      IL_007b:  ldloc.0
      IL_007c:  ldc.i4.2
      IL_007d:  ldloc.s    bytecode
      IL_007f:  ldc.i4     0x80
      IL_0084:  sub
      IL_0085:  mul
      IL_0086:  ldc.i4.1
      IL_0087:  add
      IL_0088:  ldloca.s   kvp
      IL_008a:  call       instance !1 valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,uint16>::get_Value()
      IL_008f:  ldc.i4.8
      IL_0090:  shr
      IL_0091:  conv.u1
      IL_0092:  stelem.i1
      IL_0093:  nop
      IL_0094:  ldloca.s   CS$5$0001
      IL_0096:  call       instance bool valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<string,uint16>::MoveNext()
      IL_009b:  stloc.s    CS$4$0000
      IL_009d:  ldloc.s    CS$4$0000
      IL_009f:  brtrue.s   IL_0036

      IL_00a1:  leave.s    IL_00b2

    }  // end .try
    finally
    {
      IL_00a3:  ldloca.s   CS$5$0001
      IL_00a5:  constrained. valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<string,uint16>
      IL_00ab:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_00b0:  nop
      IL_00b1:  endfinally
    }  // end handler
    IL_00b2:  nop
    IL_00b3:  ldc.i4.0
    IL_00b4:  stloc.s    block
    IL_00b6:  br.s       IL_0134

    IL_00b8:  nop
    IL_00b9:  ldc.i4.s   16
    IL_00bb:  newarr     [mscorlib]System.Byte
    IL_00c0:  stloc.s    block_bytes
    IL_00c2:  ldc.i4.0
    IL_00c3:  stloc.s    j
    IL_00c5:  br.s       IL_00df

    IL_00c7:  nop
    IL_00c8:  ldloc.s    block_bytes
    IL_00ca:  ldloc.s    j
    IL_00cc:  ldloc.0
    IL_00cd:  ldloc.s    block
    IL_00cf:  ldc.i4.s   16
    IL_00d1:  mul
    IL_00d2:  ldloc.s    j
    IL_00d4:  add
    IL_00d5:  ldelem.u1
    IL_00d6:  stelem.i1
    IL_00d7:  nop
    IL_00d8:  ldloc.s    j
    IL_00da:  ldc.i4.1
    IL_00db:  add
    IL_00dc:  conv.u2
    IL_00dd:  stloc.s    j
    IL_00df:  ldloc.s    j
    IL_00e1:  ldc.i4.s   16
    IL_00e3:  clt
    IL_00e5:  stloc.s    CS$4$0000
    IL_00e7:  ldloc.s    CS$4$0000
    IL_00e9:  brtrue.s   IL_00c7

    .try
    {
      IL_00eb:  nop
      IL_00ec:  ldarg.0
      IL_00ed:  ldc.i4.s   64
      IL_00ef:  ldc.i4     0xa1
      IL_00f4:  ldc.i4.0
      IL_00f5:  ldloc.s    block
      IL_00f7:  ldarg.0
      IL_00f8:  call       instance uint32 Pololu.Usc.Usc::get_subroutineOffsetBlocks()
      IL_00fd:  add
      IL_00fe:  conv.u2
      IL_00ff:  ldloc.s    block_bytes
      IL_0101:  call       instance uint32 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                    uint8,
                                                                                                    uint16,
                                                                                                    uint16,
                                                                                                    uint8[])
      IL_0106:  pop
      IL_0107:  nop
      IL_0108:  leave.s    IL_012b

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_010a:  stloc.s    e
      IL_010c:  nop
      IL_010d:  ldstr      "There was an error writing subroutine block "
      IL_0112:  ldloc.s    block
      IL_0114:  box        [mscorlib]System.UInt16
      IL_0119:  ldstr      "."
      IL_011e:  call       string [mscorlib]System.String::Concat(object,
                                                                  object,
                                                                  object)
      IL_0123:  ldloc.s    e
      IL_0125:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_012a:  throw

    }  // end handler
    IL_012b:  nop
    IL_012c:  nop
    IL_012d:  ldloc.s    block
    IL_012f:  ldc.i4.1
    IL_0130:  add
    IL_0131:  conv.u2
    IL_0132:  stloc.s    block
    IL_0134:  ldloc.s    block
    IL_0136:  ldc.i4.s   16
    IL_0138:  clt
    IL_013a:  stloc.s    CS$4$0000
    IL_013c:  ldloc.s    CS$4$0000
    IL_013e:  brtrue     IL_00b8

    IL_0143:  ret
  } // end of method Usc::setSubroutines

  .method private hidebysig specialname instance uint32 
          get_subroutineOffsetBlocks() cil managed
  {
    // Code size       80 (0x50)
    .maxstack  2
    .locals init ([0] uint32 CS$1$0000,
             [1] uint16 CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance uint16 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::getProductID()
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  ldc.i4     0x89
    IL_000e:  sub
    IL_000f:  switch     ( 
                          IL_0026,
                          IL_002b,
                          IL_0033,
                          IL_003b)
    IL_0024:  br.s       IL_0043

    IL_0026:  ldc.i4.s   64
    IL_0028:  stloc.0
    IL_0029:  br.s       IL_004e

    IL_002b:  ldc.i4     0x200
    IL_0030:  stloc.0
    IL_0031:  br.s       IL_004e

    IL_0033:  ldc.i4     0x200
    IL_0038:  stloc.0
    IL_0039:  br.s       IL_004e

    IL_003b:  ldc.i4     0x200
    IL_0040:  stloc.0
    IL_0041:  br.s       IL_004e

    IL_0043:  ldstr      "unknown product ID"
    IL_0048:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_004d:  throw

    IL_004e:  ldloc.0
    IL_004f:  ret
  } // end of method Usc::get_subroutineOffsetBlocks

  .method public hidebysig instance void 
          setScriptDone(uint8 'value') cil managed
  {
    // Code size       37 (0x25)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Exception e)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.s   64
      IL_0005:  ldc.i4     0xa2
      IL_000a:  ldarg.1
      IL_000b:  ldc.i4.0
      IL_000c:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                  uint8,
                                                                                                  uint16,
                                                                                                  uint16)
      IL_0011:  nop
      IL_0012:  nop
      IL_0013:  leave.s    IL_0023

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0015:  stloc.0
      IL_0016:  nop
      IL_0017:  ldstr      "There was an error setting the script done."
      IL_001c:  ldloc.0
      IL_001d:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0022:  throw

    }  // end handler
    IL_0023:  nop
    IL_0024:  ret
  } // end of method Usc::setScriptDone

  .method public hidebysig instance void 
          startBootloader() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Exception e)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.s   64
      IL_0005:  ldc.i4     0xff
      IL_000a:  ldc.i4.0
      IL_000b:  ldc.i4.0
      IL_000c:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                  uint8,
                                                                                                  uint16,
                                                                                                  uint16)
      IL_0011:  nop
      IL_0012:  nop
      IL_0013:  leave.s    IL_0023

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0015:  stloc.0
      IL_0016:  nop
      IL_0017:  ldstr      "There was an error entering bootloader mode."
      IL_001c:  ldloc.0
      IL_001d:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0022:  throw

    }  // end handler
    IL_0023:  nop
    IL_0024:  ret
  } // end of method Usc::startBootloader

  .method public hidebysig instance void 
          reinitialize() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.s   50
    IL_0004:  call       instance void Pololu.Usc.Usc::reinitialize(int32)
    IL_0009:  nop
    IL_000a:  ret
  } // end of method Usc::reinitialize

  .method private hidebysig instance void 
          reinitialize(int32 waitTime) cil managed
  {
    // Code size       65 (0x41)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Exception e,
             [1] valuetype Pololu.Usc.MaestroVariables variables,
             [2] bool CS$4$0000)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.s   64
      IL_0005:  ldc.i4     0x90
      IL_000a:  ldc.i4.0
      IL_000b:  ldc.i4.0
      IL_000c:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                  uint8,
                                                                                                  uint16,
                                                                                                  uint16)
      IL_0011:  nop
      IL_0012:  nop
      IL_0013:  leave.s    IL_0023

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0015:  stloc.0
      IL_0016:  nop
      IL_0017:  ldstr      "There was an error re-initializing the device."
      IL_001c:  ldloc.0
      IL_001d:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0022:  throw

    }  // end handler
    IL_0023:  nop
    IL_0024:  ldarg.1
    IL_0025:  call       void [mscorlib]System.Threading.Thread::Sleep(int32)
    IL_002a:  nop
    IL_002b:  ldarg.0
    IL_002c:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_0031:  stloc.2
    IL_0032:  ldloc.2
    IL_0033:  brtrue.s   IL_0040

    IL_0035:  nop
    IL_0036:  ldarg.0
    IL_0037:  ldloca.s   variables
    IL_0039:  call       instance void Pololu.Usc.Usc::getVariables(valuetype Pololu.Usc.MaestroVariables&)
    IL_003e:  nop
    IL_003f:  nop
    IL_0040:  ret
  } // end of method Usc::reinitialize

  .method public hidebysig instance void 
          clearErrors() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Exception e)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.s   64
      IL_0005:  ldc.i4     0x86
      IL_000a:  ldc.i4.0
      IL_000b:  ldc.i4.0
      IL_000c:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                  uint8,
                                                                                                  uint16,
                                                                                                  uint16)
      IL_0011:  nop
      IL_0012:  nop
      IL_0013:  leave.s    IL_0023

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0015:  stloc.0
      IL_0016:  nop
      IL_0017:  ldstr      "There was a USB communication error while clearing"
      + " the servo errors."
      IL_001c:  ldloc.0
      IL_001d:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0022:  throw

    }  // end handler
    IL_0023:  nop
    IL_0024:  ret
  } // end of method Usc::clearErrors

  .method public hidebysig instance void 
          getVariables([out] valuetype Pololu.Usc.MaestroVariables& variables,
                       [out] int16[]& stack,
                       [out] uint16[]& callStack,
                       [out] valuetype Pololu.Usc.ServoStatus[]& servos) cil managed
  {
    // Code size       66 (0x42)
    .maxstack  5
    .locals init ([0] bool CS$4$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  brtrue.s   IL_001e

    IL_000e:  nop
    IL_000f:  ldarg.0
    IL_0010:  ldarg.1
    IL_0011:  ldarg.2
    IL_0012:  ldarg.3
    IL_0013:  ldarg.s    servos
    IL_0015:  call       instance void Pololu.Usc.Usc::getVariablesMicroMaestro(valuetype Pololu.Usc.MaestroVariables&,
                                                                                int16[]&,
                                                                                uint16[]&,
                                                                                valuetype Pololu.Usc.ServoStatus[]&)
    IL_001a:  nop
    IL_001b:  nop
    IL_001c:  br.s       IL_0041

    IL_001e:  nop
    IL_001f:  ldarg.0
    IL_0020:  ldarg.1
    IL_0021:  call       instance void Pololu.Usc.Usc::getVariablesMiniMaestro(valuetype Pololu.Usc.MaestroVariables&)
    IL_0026:  nop
    IL_0027:  ldarg.0
    IL_0028:  ldarg.s    servos
    IL_002a:  call       instance void Pololu.Usc.Usc::getVariablesMiniMaestro(valuetype Pololu.Usc.ServoStatus[]&)
    IL_002f:  nop
    IL_0030:  ldarg.0
    IL_0031:  ldarg.2
    IL_0032:  call       instance void Pololu.Usc.Usc::getVariablesMiniMaestro(int16[]&)
    IL_0037:  nop
    IL_0038:  ldarg.0
    IL_0039:  ldarg.3
    IL_003a:  call       instance void Pololu.Usc.Usc::getVariablesMiniMaestro(uint16[]&)
    IL_003f:  nop
    IL_0040:  nop
    IL_0041:  ret
  } // end of method Usc::getVariables

  .method public hidebysig instance void 
          getVariables([out] valuetype Pololu.Usc.MaestroVariables& variables) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  5
    .locals init ([0] valuetype Pololu.Usc.ServoStatus[] servos,
             [1] int16[] stack,
             [2] uint16[] callStack,
             [3] bool CS$4$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  stloc.3
    IL_000b:  ldloc.3
    IL_000c:  brtrue.s   IL_0020

    IL_000e:  nop
    IL_000f:  ldarg.0
    IL_0010:  ldarg.1
    IL_0011:  ldloca.s   stack
    IL_0013:  ldloca.s   callStack
    IL_0015:  ldloca.s   servos
    IL_0017:  call       instance void Pololu.Usc.Usc::getVariablesMicroMaestro(valuetype Pololu.Usc.MaestroVariables&,
                                                                                int16[]&,
                                                                                uint16[]&,
                                                                                valuetype Pololu.Usc.ServoStatus[]&)
    IL_001c:  nop
    IL_001d:  nop
    IL_001e:  br.s       IL_002a

    IL_0020:  nop
    IL_0021:  ldarg.0
    IL_0022:  ldarg.1
    IL_0023:  call       instance void Pololu.Usc.Usc::getVariablesMiniMaestro(valuetype Pololu.Usc.MaestroVariables&)
    IL_0028:  nop
    IL_0029:  nop
    IL_002a:  ret
  } // end of method Usc::getVariables

  .method public hidebysig instance void 
          getVariables([out] valuetype Pololu.Usc.ServoStatus[]& servos) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  5
    .locals init ([0] valuetype Pololu.Usc.MaestroVariables variables,
             [1] int16[] stack,
             [2] uint16[] callStack,
             [3] bool CS$4$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  stloc.3
    IL_000b:  ldloc.3
    IL_000c:  brtrue.s   IL_0020

    IL_000e:  nop
    IL_000f:  ldarg.0
    IL_0010:  ldloca.s   variables
    IL_0012:  ldloca.s   stack
    IL_0014:  ldloca.s   callStack
    IL_0016:  ldarg.1
    IL_0017:  call       instance void Pololu.Usc.Usc::getVariablesMicroMaestro(valuetype Pololu.Usc.MaestroVariables&,
                                                                                int16[]&,
                                                                                uint16[]&,
                                                                                valuetype Pololu.Usc.ServoStatus[]&)
    IL_001c:  nop
    IL_001d:  nop
    IL_001e:  br.s       IL_002a

    IL_0020:  nop
    IL_0021:  ldarg.0
    IL_0022:  ldarg.1
    IL_0023:  call       instance void Pololu.Usc.Usc::getVariablesMiniMaestro(valuetype Pololu.Usc.ServoStatus[]&)
    IL_0028:  nop
    IL_0029:  nop
    IL_002a:  ret
  } // end of method Usc::getVariables

  .method public hidebysig instance void 
          getVariables([out] int16[]& stack) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  5
    .locals init ([0] valuetype Pololu.Usc.MaestroVariables variables,
             [1] valuetype Pololu.Usc.ServoStatus[] servos,
             [2] uint16[] callStack,
             [3] bool CS$4$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  stloc.3
    IL_000b:  ldloc.3
    IL_000c:  brtrue.s   IL_0020

    IL_000e:  nop
    IL_000f:  ldarg.0
    IL_0010:  ldloca.s   variables
    IL_0012:  ldarg.1
    IL_0013:  ldloca.s   callStack
    IL_0015:  ldloca.s   servos
    IL_0017:  call       instance void Pololu.Usc.Usc::getVariablesMicroMaestro(valuetype Pololu.Usc.MaestroVariables&,
                                                                                int16[]&,
                                                                                uint16[]&,
                                                                                valuetype Pololu.Usc.ServoStatus[]&)
    IL_001c:  nop
    IL_001d:  nop
    IL_001e:  br.s       IL_002a

    IL_0020:  nop
    IL_0021:  ldarg.0
    IL_0022:  ldarg.1
    IL_0023:  call       instance void Pololu.Usc.Usc::getVariablesMiniMaestro(int16[]&)
    IL_0028:  nop
    IL_0029:  nop
    IL_002a:  ret
  } // end of method Usc::getVariables

  .method public hidebysig instance void 
          getVariables([out] uint16[]& callStack) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  5
    .locals init ([0] valuetype Pololu.Usc.MaestroVariables variables,
             [1] int16[] stack,
             [2] valuetype Pololu.Usc.ServoStatus[] servos,
             [3] bool CS$4$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  stloc.3
    IL_000b:  ldloc.3
    IL_000c:  brtrue.s   IL_0020

    IL_000e:  nop
    IL_000f:  ldarg.0
    IL_0010:  ldloca.s   variables
    IL_0012:  ldloca.s   stack
    IL_0014:  ldarg.1
    IL_0015:  ldloca.s   servos
    IL_0017:  call       instance void Pololu.Usc.Usc::getVariablesMicroMaestro(valuetype Pololu.Usc.MaestroVariables&,
                                                                                int16[]&,
                                                                                uint16[]&,
                                                                                valuetype Pololu.Usc.ServoStatus[]&)
    IL_001c:  nop
    IL_001d:  nop
    IL_001e:  br.s       IL_002a

    IL_0020:  nop
    IL_0021:  ldarg.0
    IL_0022:  ldarg.1
    IL_0023:  call       instance void Pololu.Usc.Usc::getVariablesMiniMaestro(uint16[]&)
    IL_0028:  nop
    IL_0029:  nop
    IL_002a:  ret
  } // end of method Usc::getVariables

  .method private hidebysig instance void 
          getVariablesMicroMaestro([out] valuetype Pololu.Usc.MaestroVariables& variables,
                                   [out] int16[]& stack,
                                   [out] uint16[]& callStack,
                                   [out] valuetype Pololu.Usc.ServoStatus[]& servos) cil managed
  {
    // Code size       394 (0x18a)
    .maxstack  6
    .locals init ([0] uint8[] 'array',
             [1] class [mscorlib]System.Exception e,
             [2] uint8& pinned pointer,
             [3] valuetype Pololu.Usc.MicroMaestroVariables tmp,
             [4] uint8 i,
             [5] uint8[] CS$0$0000,
             [6] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  sizeof     Pololu.Usc.MicroMaestroVariables
    IL_0007:  ldarg.0
    IL_0008:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_000d:  sizeof     Pololu.Usc.ServoStatus
    IL_0013:  mul
    IL_0014:  add
    IL_0015:  newarr     [mscorlib]System.Byte
    IL_001a:  stloc.0
    .try
    {
      IL_001b:  nop
      IL_001c:  ldarg.0
      IL_001d:  ldc.i4     0xc0
      IL_0022:  ldc.i4     0x83
      IL_0027:  ldc.i4.0
      IL_0028:  ldc.i4.0
      IL_0029:  ldloc.0
      IL_002a:  call       instance uint32 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                    uint8,
                                                                                                    uint16,
                                                                                                    uint16,
                                                                                                    uint8[])
      IL_002f:  pop
      IL_0030:  nop
      IL_0031:  leave.s    IL_0041

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0033:  stloc.1
      IL_0034:  nop
      IL_0035:  ldstr      "There was an error getting the device variables."
      IL_003a:  ldloc.1
      IL_003b:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0040:  throw

    }  // end handler
    IL_0041:  nop
    IL_0042:  ldloc.0
    IL_0043:  dup
    IL_0044:  stloc.s    CS$0$0000
    IL_0046:  brfalse.s  IL_004e

    IL_0048:  ldloc.s    CS$0$0000
    IL_004a:  ldlen
    IL_004b:  conv.i4
    IL_004c:  brtrue.s   IL_0053

    IL_004e:  ldc.i4.0
    IL_004f:  conv.u
    IL_0050:  stloc.2
    IL_0051:  br.s       IL_005c

    IL_0053:  ldloc.s    CS$0$0000
    IL_0055:  ldc.i4.0
    IL_0056:  ldelema    [mscorlib]System.Byte
    IL_005b:  stloc.2
    IL_005c:  nop
    IL_005d:  ldloc.2
    IL_005e:  conv.i
    IL_005f:  ldobj      Pololu.Usc.MicroMaestroVariables
    IL_0064:  stloc.3
    IL_0065:  ldarg.1
    IL_0066:  ldloca.s   tmp
    IL_0068:  ldfld      uint8 Pololu.Usc.MicroMaestroVariables::stackPointer
    IL_006d:  stfld      uint8 Pololu.Usc.MaestroVariables::stackPointer
    IL_0072:  ldarg.1
    IL_0073:  ldloca.s   tmp
    IL_0075:  ldfld      uint8 Pololu.Usc.MicroMaestroVariables::callStackPointer
    IL_007a:  stfld      uint8 Pololu.Usc.MaestroVariables::callStackPointer
    IL_007f:  ldarg.1
    IL_0080:  ldloca.s   tmp
    IL_0082:  ldfld      uint16 Pololu.Usc.MicroMaestroVariables::errors
    IL_0087:  stfld      uint16 Pololu.Usc.MaestroVariables::errors
    IL_008c:  ldarg.1
    IL_008d:  ldloca.s   tmp
    IL_008f:  ldfld      uint16 Pololu.Usc.MicroMaestroVariables::programCounter
    IL_0094:  stfld      uint16 Pololu.Usc.MaestroVariables::programCounter
    IL_0099:  ldarg.1
    IL_009a:  ldloca.s   tmp
    IL_009c:  ldfld      uint8 Pololu.Usc.MicroMaestroVariables::scriptDone
    IL_00a1:  stfld      uint8 Pololu.Usc.MaestroVariables::scriptDone
    IL_00a6:  ldarg.1
    IL_00a7:  ldc.i4.0
    IL_00a8:  stfld      uint8 Pololu.Usc.MaestroVariables::performanceFlags
    IL_00ad:  ldarg.s    servos
    IL_00af:  ldarg.0
    IL_00b0:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_00b5:  newarr     Pololu.Usc.ServoStatus
    IL_00ba:  stind.ref
    IL_00bb:  ldc.i4.0
    IL_00bc:  stloc.s    i
    IL_00be:  br.s       IL_00f1

    IL_00c0:  nop
    IL_00c1:  ldarg.s    servos
    IL_00c3:  ldind.ref
    IL_00c4:  ldloc.s    i
    IL_00c6:  ldelema    Pololu.Usc.ServoStatus
    IL_00cb:  ldloc.2
    IL_00cc:  conv.i
    IL_00cd:  sizeof     Pololu.Usc.MicroMaestroVariables
    IL_00d3:  add
    IL_00d4:  sizeof     Pololu.Usc.ServoStatus
    IL_00da:  conv.i
    IL_00db:  ldloc.s    i
    IL_00dd:  mul
    IL_00de:  add
    IL_00df:  ldobj      Pololu.Usc.ServoStatus
    IL_00e4:  stobj      Pololu.Usc.ServoStatus
    IL_00e9:  nop
    IL_00ea:  ldloc.s    i
    IL_00ec:  ldc.i4.1
    IL_00ed:  add
    IL_00ee:  conv.u1
    IL_00ef:  stloc.s    i
    IL_00f1:  ldloc.s    i
    IL_00f3:  ldarg.0
    IL_00f4:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_00f9:  clt
    IL_00fb:  stloc.s    CS$4$0001
    IL_00fd:  ldloc.s    CS$4$0001
    IL_00ff:  brtrue.s   IL_00c0

    IL_0101:  ldarg.2
    IL_0102:  ldarg.1
    IL_0103:  ldfld      uint8 Pololu.Usc.MaestroVariables::stackPointer
    IL_0108:  newarr     [mscorlib]System.Int16
    IL_010d:  stind.ref
    IL_010e:  ldc.i4.0
    IL_010f:  stloc.s    i
    IL_0111:  br.s       IL_0135

    IL_0113:  nop
    IL_0114:  ldarg.2
    IL_0115:  ldind.ref
    IL_0116:  ldloc.s    i
    IL_0118:  ldloca.s   tmp
    IL_011a:  ldflda     valuetype Pololu.Usc.MicroMaestroVariables/'<stack>e__FixedBuffer1' Pololu.Usc.MicroMaestroVariables::stack
    IL_011f:  ldflda     int16 Pololu.Usc.MicroMaestroVariables/'<stack>e__FixedBuffer1'::FixedElementField
    IL_0124:  conv.u
    IL_0125:  ldloc.s    i
    IL_0127:  conv.i
    IL_0128:  ldc.i4.2
    IL_0129:  mul
    IL_012a:  add
    IL_012b:  ldind.i2
    IL_012c:  stelem.i2
    IL_012d:  nop
    IL_012e:  ldloc.s    i
    IL_0130:  ldc.i4.1
    IL_0131:  add
    IL_0132:  conv.u1
    IL_0133:  stloc.s    i
    IL_0135:  ldloc.s    i
    IL_0137:  ldarg.2
    IL_0138:  ldind.ref
    IL_0139:  ldlen
    IL_013a:  conv.i4
    IL_013b:  clt
    IL_013d:  stloc.s    CS$4$0001
    IL_013f:  ldloc.s    CS$4$0001
    IL_0141:  brtrue.s   IL_0113

    IL_0143:  ldarg.3
    IL_0144:  ldarg.1
    IL_0145:  ldfld      uint8 Pololu.Usc.MaestroVariables::callStackPointer
    IL_014a:  newarr     [mscorlib]System.UInt16
    IL_014f:  stind.ref
    IL_0150:  ldc.i4.0
    IL_0151:  stloc.s    i
    IL_0153:  br.s       IL_0177

    IL_0155:  nop
    IL_0156:  ldarg.3
    IL_0157:  ldind.ref
    IL_0158:  ldloc.s    i
    IL_015a:  ldloca.s   tmp
    IL_015c:  ldflda     valuetype Pololu.Usc.MicroMaestroVariables/'<callStack>e__FixedBuffer2' Pololu.Usc.MicroMaestroVariables::callStack
    IL_0161:  ldflda     uint16 Pololu.Usc.MicroMaestroVariables/'<callStack>e__FixedBuffer2'::FixedElementField
    IL_0166:  conv.u
    IL_0167:  ldloc.s    i
    IL_0169:  conv.i
    IL_016a:  ldc.i4.2
    IL_016b:  mul
    IL_016c:  add
    IL_016d:  ldind.u2
    IL_016e:  stelem.i2
    IL_016f:  nop
    IL_0170:  ldloc.s    i
    IL_0172:  ldc.i4.1
    IL_0173:  add
    IL_0174:  conv.u1
    IL_0175:  stloc.s    i
    IL_0177:  ldloc.s    i
    IL_0179:  ldarg.3
    IL_017a:  ldind.ref
    IL_017b:  ldlen
    IL_017c:  conv.i4
    IL_017d:  clt
    IL_017f:  stloc.s    CS$4$0001
    IL_0181:  ldloc.s    CS$4$0001
    IL_0183:  brtrue.s   IL_0155

    IL_0185:  nop
    IL_0186:  ldc.i4.0
    IL_0187:  conv.u
    IL_0188:  stloc.2
    IL_0189:  ret
  } // end of method Usc::getVariablesMicroMaestro

  .method private hidebysig instance void 
          getVariablesMiniMaestro([out] valuetype Pololu.Usc.MaestroVariables& variables) cil managed
  {
    // Code size       217 (0xd9)
    .maxstack  7
    .locals init ([0] valuetype Pololu.Usc.MiniMaestroVariables tmp,
             [1] uint32 bytesRead,
             [2] class [mscorlib]System.Exception e,
             [3] bool CS$4$0000,
             [4] object[] CS$0$0001)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4     0xc0
      IL_0008:  ldc.i4     0x83
      IL_000d:  ldc.i4.0
      IL_000e:  ldc.i4.0
      IL_000f:  ldloca.s   tmp
      IL_0011:  conv.u
      IL_0012:  sizeof     Pololu.Usc.MiniMaestroVariables
      IL_0018:  conv.u2
      IL_0019:  call       instance uint32 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                    uint8,
                                                                                                    uint16,
                                                                                                    uint16,
                                                                                                    void*,
                                                                                                    uint16)
      IL_001e:  stloc.1
      IL_001f:  ldloc.1
      IL_0020:  conv.u8
      IL_0021:  sizeof     Pololu.Usc.MiniMaestroVariables
      IL_0027:  conv.i8
      IL_0028:  ceq
      IL_002a:  stloc.3
      IL_002b:  ldloc.3
      IL_002c:  brtrue.s   IL_0078

      IL_002e:  nop
      IL_002f:  ldc.i4.5
      IL_0030:  newarr     [mscorlib]System.Object
      IL_0035:  stloc.s    CS$0$0001
      IL_0037:  ldloc.s    CS$0$0001
      IL_0039:  ldc.i4.0
      IL_003a:  ldstr      "Short read: "
      IL_003f:  stelem.ref
      IL_0040:  ldloc.s    CS$0$0001
      IL_0042:  ldc.i4.1
      IL_0043:  ldloc.1
      IL_0044:  box        [mscorlib]System.UInt32
      IL_0049:  stelem.ref
      IL_004a:  ldloc.s    CS$0$0001
      IL_004c:  ldc.i4.2
      IL_004d:  ldstr      " < "
      IL_0052:  stelem.ref
      IL_0053:  ldloc.s    CS$0$0001
      IL_0055:  ldc.i4.3
      IL_0056:  sizeof     Pololu.Usc.MiniMaestroVariables
      IL_005c:  box        [mscorlib]System.Int32
      IL_0061:  stelem.ref
      IL_0062:  ldloc.s    CS$0$0001
      IL_0064:  ldc.i4.4
      IL_0065:  ldstr      "."
      IL_006a:  stelem.ref
      IL_006b:  ldloc.s    CS$0$0001
      IL_006d:  call       string [mscorlib]System.String::Concat(object[])
      IL_0072:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_0077:  throw

      IL_0078:  ldarg.1
      IL_0079:  ldloca.s   tmp
      IL_007b:  ldfld      uint8 Pololu.Usc.MiniMaestroVariables::stackPointer
      IL_0080:  stfld      uint8 Pololu.Usc.MaestroVariables::stackPointer
      IL_0085:  ldarg.1
      IL_0086:  ldloca.s   tmp
      IL_0088:  ldfld      uint8 Pololu.Usc.MiniMaestroVariables::callStackPointer
      IL_008d:  stfld      uint8 Pololu.Usc.MaestroVariables::callStackPointer
      IL_0092:  ldarg.1
      IL_0093:  ldloca.s   tmp
      IL_0095:  ldfld      uint16 Pololu.Usc.MiniMaestroVariables::errors
      IL_009a:  stfld      uint16 Pololu.Usc.MaestroVariables::errors
      IL_009f:  ldarg.1
      IL_00a0:  ldloca.s   tmp
      IL_00a2:  ldfld      uint16 Pololu.Usc.MiniMaestroVariables::programCounter
      IL_00a7:  stfld      uint16 Pololu.Usc.MaestroVariables::programCounter
      IL_00ac:  ldarg.1
      IL_00ad:  ldloca.s   tmp
      IL_00af:  ldfld      uint8 Pololu.Usc.MiniMaestroVariables::scriptDone
      IL_00b4:  stfld      uint8 Pololu.Usc.MaestroVariables::scriptDone
      IL_00b9:  ldarg.1
      IL_00ba:  ldloca.s   tmp
      IL_00bc:  ldfld      uint8 Pololu.Usc.MiniMaestroVariables::performanceFlags
      IL_00c1:  stfld      uint8 Pololu.Usc.MaestroVariables::performanceFlags
      IL_00c6:  nop
      IL_00c7:  leave.s    IL_00d7

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_00c9:  stloc.2
      IL_00ca:  nop
      IL_00cb:  ldstr      "Error getting variables from device."
      IL_00d0:  ldloc.2
      IL_00d1:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_00d6:  throw

    }  // end handler
    IL_00d7:  nop
    IL_00d8:  ret
  } // end of method Usc::getVariablesMiniMaestro

  .method private hidebysig instance void 
          getVariablesMiniMaestro([out] valuetype Pololu.Usc.ServoStatus[]& servos) cil managed
  {
    // Code size       251 (0xfb)
    .maxstack  6
    .locals init ([0] uint8[] servoSettingsArray,
             [1] uint32 bytesRead,
             [2] uint8& pinned pointer,
             [3] uint8 i,
             [4] class [mscorlib]System.Exception e,
             [5] bool CS$4$0000,
             [6] object[] CS$0$0001,
             [7] uint8[] CS$0$0002)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldfld      uint8 Pololu.Usc.Usc::servoCount
      IL_0008:  sizeof     Pololu.Usc.ServoStatus
      IL_000e:  mul
      IL_000f:  newarr     [mscorlib]System.Byte
      IL_0014:  stloc.0
      IL_0015:  ldarg.0
      IL_0016:  ldc.i4     0xc0
      IL_001b:  ldc.i4     0x87
      IL_0020:  ldc.i4.0
      IL_0021:  ldc.i4.0
      IL_0022:  ldloc.0
      IL_0023:  call       instance uint32 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                    uint8,
                                                                                                    uint16,
                                                                                                    uint16,
                                                                                                    uint8[])
      IL_0028:  stloc.1
      IL_0029:  ldloc.1
      IL_002a:  conv.u8
      IL_002b:  ldloc.0
      IL_002c:  ldlen
      IL_002d:  conv.i4
      IL_002e:  conv.i8
      IL_002f:  ceq
      IL_0031:  stloc.s    CS$4$0000
      IL_0033:  ldloc.s    CS$4$0000
      IL_0035:  brtrue.s   IL_007e

      IL_0037:  nop
      IL_0038:  ldc.i4.5
      IL_0039:  newarr     [mscorlib]System.Object
      IL_003e:  stloc.s    CS$0$0001
      IL_0040:  ldloc.s    CS$0$0001
      IL_0042:  ldc.i4.0
      IL_0043:  ldstr      "Short read: "
      IL_0048:  stelem.ref
      IL_0049:  ldloc.s    CS$0$0001
      IL_004b:  ldc.i4.1
      IL_004c:  ldloc.1
      IL_004d:  box        [mscorlib]System.UInt32
      IL_0052:  stelem.ref
      IL_0053:  ldloc.s    CS$0$0001
      IL_0055:  ldc.i4.2
      IL_0056:  ldstr      " < "
      IL_005b:  stelem.ref
      IL_005c:  ldloc.s    CS$0$0001
      IL_005e:  ldc.i4.3
      IL_005f:  ldloc.0
      IL_0060:  ldlen
      IL_0061:  conv.i4
      IL_0062:  box        [mscorlib]System.Int32
      IL_0067:  stelem.ref
      IL_0068:  ldloc.s    CS$0$0001
      IL_006a:  ldc.i4.4
      IL_006b:  ldstr      "."
      IL_0070:  stelem.ref
      IL_0071:  ldloc.s    CS$0$0001
      IL_0073:  call       string [mscorlib]System.String::Concat(object[])
      IL_0078:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_007d:  throw

      IL_007e:  ldarg.1
      IL_007f:  ldarg.0
      IL_0080:  ldfld      uint8 Pololu.Usc.Usc::servoCount
      IL_0085:  newarr     Pololu.Usc.ServoStatus
      IL_008a:  stind.ref
      .try
      {
        IL_008b:  ldloc.0
        IL_008c:  dup
        IL_008d:  stloc.s    CS$0$0002
        IL_008f:  brfalse.s  IL_0097

        IL_0091:  ldloc.s    CS$0$0002
        IL_0093:  ldlen
        IL_0094:  conv.i4
        IL_0095:  brtrue.s   IL_009c

        IL_0097:  ldc.i4.0
        IL_0098:  conv.u
        IL_0099:  stloc.2
        IL_009a:  br.s       IL_00a5

        IL_009c:  ldloc.s    CS$0$0002
        IL_009e:  ldc.i4.0
        IL_009f:  ldelema    [mscorlib]System.Byte
        IL_00a4:  stloc.2
        IL_00a5:  nop
        IL_00a6:  ldc.i4.0
        IL_00a7:  stloc.3
        IL_00a8:  br.s       IL_00cf

        IL_00aa:  nop
        IL_00ab:  ldarg.1
        IL_00ac:  ldind.ref
        IL_00ad:  ldloc.3
        IL_00ae:  ldelema    Pololu.Usc.ServoStatus
        IL_00b3:  ldloc.2
        IL_00b4:  conv.i
        IL_00b5:  sizeof     Pololu.Usc.ServoStatus
        IL_00bb:  conv.i
        IL_00bc:  ldloc.3
        IL_00bd:  mul
        IL_00be:  add
        IL_00bf:  ldobj      Pololu.Usc.ServoStatus
        IL_00c4:  stobj      Pololu.Usc.ServoStatus
        IL_00c9:  nop
        IL_00ca:  ldloc.3
        IL_00cb:  ldc.i4.1
        IL_00cc:  add
        IL_00cd:  conv.u1
        IL_00ce:  stloc.3
        IL_00cf:  ldloc.3
        IL_00d0:  ldarg.0
        IL_00d1:  ldfld      uint8 Pololu.Usc.Usc::servoCount
        IL_00d6:  clt
        IL_00d8:  stloc.s    CS$4$0000
        IL_00da:  ldloc.s    CS$4$0000
        IL_00dc:  brtrue.s   IL_00aa

        IL_00de:  nop
        IL_00df:  leave.s    IL_00e5

      }  // end .try
      finally
      {
        IL_00e1:  ldc.i4.0
        IL_00e2:  conv.u
        IL_00e3:  stloc.2
        IL_00e4:  endfinally
      }  // end handler
      IL_00e5:  nop
      IL_00e6:  nop
      IL_00e7:  leave.s    IL_00f9

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_00e9:  stloc.s    e
      IL_00eb:  nop
      IL_00ec:  ldstr      "Error getting channel settings from device."
      IL_00f1:  ldloc.s    e
      IL_00f3:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_00f8:  throw

    }  // end handler
    IL_00f9:  nop
    IL_00fa:  ret
  } // end of method Usc::getVariablesMiniMaestro

  .method private hidebysig instance void 
          getVariablesMiniMaestro([out] int16[]& stack) cil managed
  {
    // Code size       101 (0x65)
    .maxstack  8
    .locals init ([0] int16& pinned pointer,
             [1] uint32 bytesRead,
             [2] class [mscorlib]System.Exception e,
             [3] int16[] CS$0$0000)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.1
      IL_0003:  ldc.i4.s   126
      IL_0005:  newarr     [mscorlib]System.Int16
      IL_000a:  stind.ref
      .try
      {
        IL_000b:  ldarg.1
        IL_000c:  ldind.ref
        IL_000d:  dup
        IL_000e:  stloc.3
        IL_000f:  brfalse.s  IL_0016

        IL_0011:  ldloc.3
        IL_0012:  ldlen
        IL_0013:  conv.i4
        IL_0014:  brtrue.s   IL_001b

        IL_0016:  ldc.i4.0
        IL_0017:  conv.u
        IL_0018:  stloc.0
        IL_0019:  br.s       IL_0023

        IL_001b:  ldloc.3
        IL_001c:  ldc.i4.0
        IL_001d:  ldelema    [mscorlib]System.Int16
        IL_0022:  stloc.0
        IL_0023:  nop
        IL_0024:  ldarg.0
        IL_0025:  ldc.i4     0xc0
        IL_002a:  ldc.i4     0x88
        IL_002f:  ldc.i4.0
        IL_0030:  ldc.i4.0
        IL_0031:  ldloc.0
        IL_0032:  conv.i
        IL_0033:  ldc.i4.2
        IL_0034:  ldarg.1
        IL_0035:  ldind.ref
        IL_0036:  ldlen
        IL_0037:  conv.i4
        IL_0038:  mul
        IL_0039:  conv.u2
        IL_003a:  call       instance uint32 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                      uint8,
                                                                                                      uint16,
                                                                                                      uint16,
                                                                                                      void*,
                                                                                                      uint16)
        IL_003f:  stloc.1
        IL_0040:  ldarg.1
        IL_0041:  ldloc.1
        IL_0042:  ldc.i4.2
        IL_0043:  div.un
        IL_0044:  call       void [mscorlib]System.Array::Resize<int16>(!!0[]&,
                                                                        int32)
        IL_0049:  nop
        IL_004a:  nop
        IL_004b:  leave.s    IL_0051

      }  // end .try
      finally
      {
        IL_004d:  ldc.i4.0
        IL_004e:  conv.u
        IL_004f:  stloc.0
        IL_0050:  endfinally
      }  // end handler
      IL_0051:  nop
      IL_0052:  nop
      IL_0053:  leave.s    IL_0063

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0055:  stloc.2
      IL_0056:  nop
      IL_0057:  ldstr      "Error getting stack from device."
      IL_005c:  ldloc.2
      IL_005d:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0062:  throw

    }  // end handler
    IL_0063:  nop
    IL_0064:  ret
  } // end of method Usc::getVariablesMiniMaestro

  .method private hidebysig instance void 
          getVariablesMiniMaestro([out] uint16[]& callStack) cil managed
  {
    // Code size       101 (0x65)
    .maxstack  8
    .locals init ([0] uint16& pinned pointer,
             [1] uint32 bytesRead,
             [2] class [mscorlib]System.Exception e,
             [3] uint16[] CS$0$0000)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.1
      IL_0003:  ldc.i4.s   126
      IL_0005:  newarr     [mscorlib]System.UInt16
      IL_000a:  stind.ref
      .try
      {
        IL_000b:  ldarg.1
        IL_000c:  ldind.ref
        IL_000d:  dup
        IL_000e:  stloc.3
        IL_000f:  brfalse.s  IL_0016

        IL_0011:  ldloc.3
        IL_0012:  ldlen
        IL_0013:  conv.i4
        IL_0014:  brtrue.s   IL_001b

        IL_0016:  ldc.i4.0
        IL_0017:  conv.u
        IL_0018:  stloc.0
        IL_0019:  br.s       IL_0023

        IL_001b:  ldloc.3
        IL_001c:  ldc.i4.0
        IL_001d:  ldelema    [mscorlib]System.UInt16
        IL_0022:  stloc.0
        IL_0023:  nop
        IL_0024:  ldarg.0
        IL_0025:  ldc.i4     0xc0
        IL_002a:  ldc.i4     0x89
        IL_002f:  ldc.i4.0
        IL_0030:  ldc.i4.0
        IL_0031:  ldloc.0
        IL_0032:  conv.i
        IL_0033:  ldc.i4.2
        IL_0034:  ldarg.1
        IL_0035:  ldind.ref
        IL_0036:  ldlen
        IL_0037:  conv.i4
        IL_0038:  mul
        IL_0039:  conv.u2
        IL_003a:  call       instance uint32 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                      uint8,
                                                                                                      uint16,
                                                                                                      uint16,
                                                                                                      void*,
                                                                                                      uint16)
        IL_003f:  stloc.1
        IL_0040:  ldarg.1
        IL_0041:  ldloc.1
        IL_0042:  ldc.i4.2
        IL_0043:  div.un
        IL_0044:  call       void [mscorlib]System.Array::Resize<uint16>(!!0[]&,
                                                                         int32)
        IL_0049:  nop
        IL_004a:  nop
        IL_004b:  leave.s    IL_0051

      }  // end .try
      finally
      {
        IL_004d:  ldc.i4.0
        IL_004e:  conv.u
        IL_004f:  stloc.0
        IL_0050:  endfinally
      }  // end handler
      IL_0051:  nop
      IL_0052:  nop
      IL_0053:  leave.s    IL_0063

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0055:  stloc.2
      IL_0056:  nop
      IL_0057:  ldstr      "Error getting call stack from device."
      IL_005c:  ldloc.2
      IL_005d:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0062:  throw

    }  // end handler
    IL_0063:  nop
    IL_0064:  ret
  } // end of method Usc::getVariablesMiniMaestro

  .method public hidebysig instance void 
          setTarget(uint8 servo,
                    uint16 'value') cil managed
  {
    // Code size       87 (0x57)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Exception e,
             [1] object[] CS$0$0000)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.s   64
      IL_0005:  ldc.i4     0x85
      IL_000a:  ldarg.2
      IL_000b:  ldarg.1
      IL_000c:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                  uint8,
                                                                                                  uint16,
                                                                                                  uint16)
      IL_0011:  nop
      IL_0012:  nop
      IL_0013:  leave.s    IL_0055

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0015:  stloc.0
      IL_0016:  nop
      IL_0017:  ldc.i4.5
      IL_0018:  newarr     [mscorlib]System.Object
      IL_001d:  stloc.1
      IL_001e:  ldloc.1
      IL_001f:  ldc.i4.0
      IL_0020:  ldstr      "Failed to set target of servo "
      IL_0025:  stelem.ref
      IL_0026:  ldloc.1
      IL_0027:  ldc.i4.1
      IL_0028:  ldarg.1
      IL_0029:  box        [mscorlib]System.Byte
      IL_002e:  stelem.ref
      IL_002f:  ldloc.1
      IL_0030:  ldc.i4.2
      IL_0031:  ldstr      " to "
      IL_0036:  stelem.ref
      IL_0037:  ldloc.1
      IL_0038:  ldc.i4.3
      IL_0039:  ldarg.2
      IL_003a:  box        [mscorlib]System.UInt16
      IL_003f:  stelem.ref
      IL_0040:  ldloc.1
      IL_0041:  ldc.i4.4
      IL_0042:  ldstr      "."
      IL_0047:  stelem.ref
      IL_0048:  ldloc.1
      IL_0049:  call       string [mscorlib]System.String::Concat(object[])
      IL_004e:  ldloc.0
      IL_004f:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0054:  throw

    }  // end handler
    IL_0055:  nop
    IL_0056:  ret
  } // end of method Usc::setTarget

  .method public hidebysig instance void 
          setSpeed(uint8 servo,
                   uint16 'value') cil managed
  {
    // Code size       87 (0x57)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Exception e,
             [1] object[] CS$0$0000)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.s   64
      IL_0005:  ldc.i4     0x84
      IL_000a:  ldarg.2
      IL_000b:  ldarg.1
      IL_000c:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                  uint8,
                                                                                                  uint16,
                                                                                                  uint16)
      IL_0011:  nop
      IL_0012:  nop
      IL_0013:  leave.s    IL_0055

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0015:  stloc.0
      IL_0016:  nop
      IL_0017:  ldc.i4.5
      IL_0018:  newarr     [mscorlib]System.Object
      IL_001d:  stloc.1
      IL_001e:  ldloc.1
      IL_001f:  ldc.i4.0
      IL_0020:  ldstr      "Failed to set speed of servo "
      IL_0025:  stelem.ref
      IL_0026:  ldloc.1
      IL_0027:  ldc.i4.1
      IL_0028:  ldarg.1
      IL_0029:  box        [mscorlib]System.Byte
      IL_002e:  stelem.ref
      IL_002f:  ldloc.1
      IL_0030:  ldc.i4.2
      IL_0031:  ldstr      " to "
      IL_0036:  stelem.ref
      IL_0037:  ldloc.1
      IL_0038:  ldc.i4.3
      IL_0039:  ldarg.2
      IL_003a:  box        [mscorlib]System.UInt16
      IL_003f:  stelem.ref
      IL_0040:  ldloc.1
      IL_0041:  ldc.i4.4
      IL_0042:  ldstr      "."
      IL_0047:  stelem.ref
      IL_0048:  ldloc.1
      IL_0049:  call       string [mscorlib]System.String::Concat(object[])
      IL_004e:  ldloc.0
      IL_004f:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0054:  throw

    }  // end handler
    IL_0055:  nop
    IL_0056:  ret
  } // end of method Usc::setSpeed

  .method public hidebysig instance void 
          setAcceleration(uint8 servo,
                          uint16 'value') cil managed
  {
    // Code size       94 (0x5e)
    .maxstack  6
    .locals init ([0] class [mscorlib]System.Exception e,
             [1] object[] CS$0$0000)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.s   64
      IL_0005:  ldc.i4     0x84
      IL_000a:  ldarg.2
      IL_000b:  ldarg.1
      IL_000c:  ldc.i4     0x80
      IL_0011:  or
      IL_0012:  conv.u1
      IL_0013:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                  uint8,
                                                                                                  uint16,
                                                                                                  uint16)
      IL_0018:  nop
      IL_0019:  nop
      IL_001a:  leave.s    IL_005c

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_001c:  stloc.0
      IL_001d:  nop
      IL_001e:  ldc.i4.5
      IL_001f:  newarr     [mscorlib]System.Object
      IL_0024:  stloc.1
      IL_0025:  ldloc.1
      IL_0026:  ldc.i4.0
      IL_0027:  ldstr      "Failed to set acceleration of servo "
      IL_002c:  stelem.ref
      IL_002d:  ldloc.1
      IL_002e:  ldc.i4.1
      IL_002f:  ldarg.1
      IL_0030:  box        [mscorlib]System.Byte
      IL_0035:  stelem.ref
      IL_0036:  ldloc.1
      IL_0037:  ldc.i4.2
      IL_0038:  ldstr      " to "
      IL_003d:  stelem.ref
      IL_003e:  ldloc.1
      IL_003f:  ldc.i4.3
      IL_0040:  ldarg.2
      IL_0041:  box        [mscorlib]System.UInt16
      IL_0046:  stelem.ref
      IL_0047:  ldloc.1
      IL_0048:  ldc.i4.4
      IL_0049:  ldstr      "."
      IL_004e:  stelem.ref
      IL_004f:  ldloc.1
      IL_0050:  call       string [mscorlib]System.String::Concat(object[])
      IL_0055:  ldloc.0
      IL_0056:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_005b:  throw

    }  // end handler
    IL_005c:  nop
    IL_005d:  ret
  } // end of method Usc::setAcceleration

  .method public hidebysig newslot virtual final 
          instance void  setUscSettings(class Pololu.Usc.UscSettings settings,
                                        bool newScript) cil managed
  {
    // Code size       1192 (0x4a8)
    .maxstack  5
    .locals init ([0] uint8 multiplier,
             [1] class [mscorlib]Microsoft.Win32.RegistryKey key,
             [2] uint8 ioMask,
             [3] uint8 outputMask,
             [4] uint8[] channelModeBytes,
             [5] uint8 i,
             [6] class Pololu.Usc.ChannelSetting setting,
             [7] valuetype Pololu.Usc.HomeMode correctedHomeMode,
             [8] uint16 home,
             [9] class [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram program,
             [10] class [mscorlib]System.Collections.Generic.List`1<uint8> byteList,
             [11] bool CS$4$0000,
             [12] uint8[] CS$0$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.3
    IL_0003:  ldarg.1
    IL_0004:  ldfld      valuetype Pololu.Usc.uscSerialMode Pololu.Usc.UscSettings::serialMode
    IL_0009:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_000e:  nop
    IL_000f:  ldarg.0
    IL_0010:  ldc.i4.4
    IL_0011:  ldarg.1
    IL_0012:  ldfld      uint32 Pololu.Usc.UscSettings::fixedBaudRate
    IL_0017:  call       uint16 Pololu.Usc.Usc::convertBpsToSpbrg(uint32)
    IL_001c:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_0021:  nop
    IL_0022:  ldarg.0
    IL_0023:  ldc.i4.8
    IL_0024:  ldarg.1
    IL_0025:  ldfld      bool Pololu.Usc.UscSettings::enableCrc
    IL_002a:  brtrue.s   IL_002f

    IL_002c:  ldc.i4.0
    IL_002d:  br.s       IL_0030

    IL_002f:  ldc.i4.1
    IL_0030:  conv.u2
    IL_0031:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_0036:  nop
    IL_0037:  ldarg.0
    IL_0038:  ldc.i4.s   9
    IL_003a:  ldarg.1
    IL_003b:  ldfld      bool Pololu.Usc.UscSettings::neverSuspend
    IL_0040:  brtrue.s   IL_0045

    IL_0042:  ldc.i4.0
    IL_0043:  br.s       IL_0046

    IL_0045:  ldc.i4.1
    IL_0046:  conv.u2
    IL_0047:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_004c:  nop
    IL_004d:  ldarg.0
    IL_004e:  ldc.i4.s   10
    IL_0050:  ldarg.1
    IL_0051:  ldfld      uint8 Pololu.Usc.UscSettings::serialDeviceNumber
    IL_0056:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_005b:  nop
    IL_005c:  ldarg.0
    IL_005d:  ldc.i4.s   25
    IL_005f:  ldarg.1
    IL_0060:  ldfld      uint8 Pololu.Usc.UscSettings::miniSscOffset
    IL_0065:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_006a:  nop
    IL_006b:  ldarg.0
    IL_006c:  ldc.i4.6
    IL_006d:  ldarg.1
    IL_006e:  ldfld      uint16 Pololu.Usc.UscSettings::serialTimeout
    IL_0073:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_0078:  nop
    IL_0079:  ldarg.0
    IL_007a:  ldc.i4.s   24
    IL_007c:  ldarg.1
    IL_007d:  ldfld      bool Pololu.Usc.UscSettings::scriptDone
    IL_0082:  brtrue.s   IL_0087

    IL_0084:  ldc.i4.0
    IL_0085:  br.s       IL_0088

    IL_0087:  ldc.i4.1
    IL_0088:  conv.u2
    IL_0089:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_008e:  nop
    IL_008f:  ldarg.0
    IL_0090:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_0095:  ldc.i4.6
    IL_0096:  ceq
    IL_0098:  ldc.i4.0
    IL_0099:  ceq
    IL_009b:  stloc.s    CS$4$0000
    IL_009d:  ldloc.s    CS$4$0000
    IL_009f:  brtrue.s   IL_00c1

    IL_00a1:  nop
    IL_00a2:  ldarg.0
    IL_00a3:  ldc.i4.1
    IL_00a4:  ldarg.1
    IL_00a5:  ldfld      uint8 Pololu.Usc.UscSettings::servosAvailable
    IL_00aa:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_00af:  nop
    IL_00b0:  ldarg.0
    IL_00b1:  ldc.i4.2
    IL_00b2:  ldarg.1
    IL_00b3:  ldfld      uint8 Pololu.Usc.UscSettings::servoPeriod
    IL_00b8:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_00bd:  nop
    IL_00be:  nop
    IL_00bf:  br.s       IL_0139

    IL_00c1:  nop
    IL_00c2:  ldarg.0
    IL_00c3:  ldc.i4.s   18
    IL_00c5:  ldarg.1
    IL_00c6:  ldfld      uint32 Pololu.Usc.UscSettings::miniMaestroServoPeriod
    IL_00cb:  ldc.i4     0xff
    IL_00d0:  and
    IL_00d1:  conv.u1
    IL_00d2:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_00d7:  nop
    IL_00d8:  ldarg.0
    IL_00d9:  ldc.i4.s   19
    IL_00db:  ldarg.1
    IL_00dc:  ldfld      uint32 Pololu.Usc.UscSettings::miniMaestroServoPeriod
    IL_00e1:  ldc.i4.8
    IL_00e2:  shr.un
    IL_00e3:  conv.u2
    IL_00e4:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_00e9:  nop
    IL_00ea:  ldarg.1
    IL_00eb:  ldfld      uint16 Pololu.Usc.UscSettings::servoMultiplier
    IL_00f0:  ldc.i4.1
    IL_00f1:  clt
    IL_00f3:  ldc.i4.0
    IL_00f4:  ceq
    IL_00f6:  stloc.s    CS$4$0000
    IL_00f8:  ldloc.s    CS$4$0000
    IL_00fa:  brtrue.s   IL_0102

    IL_00fc:  nop
    IL_00fd:  ldc.i4.0
    IL_00fe:  stloc.0
    IL_00ff:  nop
    IL_0100:  br.s       IL_012e

    IL_0102:  ldarg.1
    IL_0103:  ldfld      uint16 Pololu.Usc.UscSettings::servoMultiplier
    IL_0108:  ldc.i4     0x100
    IL_010d:  cgt
    IL_010f:  ldc.i4.0
    IL_0110:  ceq
    IL_0112:  stloc.s    CS$4$0000
    IL_0114:  ldloc.s    CS$4$0000
    IL_0116:  brtrue.s   IL_0122

    IL_0118:  nop
    IL_0119:  ldc.i4     0xff
    IL_011e:  stloc.0
    IL_011f:  nop
    IL_0120:  br.s       IL_012e

    IL_0122:  nop
    IL_0123:  ldarg.1
    IL_0124:  ldfld      uint16 Pololu.Usc.UscSettings::servoMultiplier
    IL_0129:  ldc.i4.1
    IL_012a:  sub
    IL_012b:  conv.u1
    IL_012c:  stloc.0
    IL_012d:  nop
    IL_012e:  ldarg.0
    IL_012f:  ldc.i4.s   26
    IL_0131:  ldloc.0
    IL_0132:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_0137:  nop
    IL_0138:  nop
    IL_0139:  ldarg.0
    IL_013a:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_013f:  ldc.i4.s   18
    IL_0141:  cgt
    IL_0143:  ldc.i4.0
    IL_0144:  ceq
    IL_0146:  stloc.s    CS$4$0000
    IL_0148:  ldloc.s    CS$4$0000
    IL_014a:  brtrue.s   IL_0164

    IL_014c:  nop
    IL_014d:  ldarg.0
    IL_014e:  ldc.i4.s   21
    IL_0150:  ldarg.1
    IL_0151:  ldfld      bool Pololu.Usc.UscSettings::enablePullups
    IL_0156:  brtrue.s   IL_015b

    IL_0158:  ldc.i4.0
    IL_0159:  br.s       IL_015c

    IL_015b:  ldc.i4.1
    IL_015c:  conv.u2
    IL_015d:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_0162:  nop
    IL_0163:  nop
    IL_0164:  ldarg.0
    IL_0165:  call       instance class [mscorlib]Microsoft.Win32.RegistryKey Pololu.Usc.Usc::openRegistryKey()
    IL_016a:  stloc.1
    IL_016b:  ldc.i4.0
    IL_016c:  stloc.2
    IL_016d:  ldc.i4.0
    IL_016e:  stloc.3
    IL_016f:  ldc.i4.6
    IL_0170:  newarr     [mscorlib]System.Byte
    IL_0175:  stloc.s    CS$0$0001
    IL_0177:  ldloc.s    CS$0$0001
    IL_0179:  stloc.s    channelModeBytes
    IL_017b:  ldc.i4.0
    IL_017c:  stloc.s    i
    IL_017e:  br         IL_035f

    IL_0183:  nop
    IL_0184:  ldarg.1
    IL_0185:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting> Pololu.Usc.UscSettings::channelSettings
    IL_018a:  ldloc.s    i
    IL_018c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting>::get_Item(int32)
    IL_0191:  stloc.s    setting
    IL_0193:  ldloc.1
    IL_0194:  ldstr      "servoName"
    IL_0199:  ldloca.s   i
    IL_019b:  ldstr      "d2"
    IL_01a0:  call       instance string [mscorlib]System.Byte::ToString(string)
    IL_01a5:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_01aa:  ldloc.s    setting
    IL_01ac:  ldfld      string Pololu.Usc.ChannelSetting::name
    IL_01b1:  ldc.i4.1
    IL_01b2:  callvirt   instance void [mscorlib]Microsoft.Win32.RegistryKey::SetValue(string,
                                                                                       object,
                                                                                       valuetype [mscorlib]Microsoft.Win32.RegistryValueKind)
    IL_01b7:  nop
    IL_01b8:  ldarg.0
    IL_01b9:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_01be:  ldc.i4.0
    IL_01bf:  ceq
    IL_01c1:  stloc.s    CS$4$0000
    IL_01c3:  ldloc.s    CS$4$0000
    IL_01c5:  brtrue.s   IL_0226

    IL_01c7:  nop
    IL_01c8:  ldloc.s    setting
    IL_01ca:  ldfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_01cf:  ldc.i4.3
    IL_01d0:  beq.s      IL_01e1

    IL_01d2:  ldloc.s    setting
    IL_01d4:  ldfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_01d9:  ldc.i4.2
    IL_01da:  ceq
    IL_01dc:  ldc.i4.0
    IL_01dd:  ceq
    IL_01df:  br.s       IL_01e2

    IL_01e1:  ldc.i4.0
    IL_01e2:  stloc.s    CS$4$0000
    IL_01e4:  ldloc.s    CS$4$0000
    IL_01e6:  brtrue.s   IL_01fc

    IL_01e8:  nop
    IL_01e9:  ldloc.2
    IL_01ea:  ldc.i4.1
    IL_01eb:  ldarg.0
    IL_01ec:  ldloc.s    i
    IL_01ee:  call       instance uint8 Pololu.Usc.Usc::channelToPort(uint8)
    IL_01f3:  ldc.i4.s   31
    IL_01f5:  and
    IL_01f6:  shl
    IL_01f7:  conv.u1
    IL_01f8:  or
    IL_01f9:  conv.u1
    IL_01fa:  stloc.2
    IL_01fb:  nop
    IL_01fc:  ldloc.s    setting
    IL_01fe:  ldfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_0203:  ldc.i4.2
    IL_0204:  ceq
    IL_0206:  ldc.i4.0
    IL_0207:  ceq
    IL_0209:  stloc.s    CS$4$0000
    IL_020b:  ldloc.s    CS$4$0000
    IL_020d:  brtrue.s   IL_0223

    IL_020f:  nop
    IL_0210:  ldloc.3
    IL_0211:  ldc.i4.1
    IL_0212:  ldarg.0
    IL_0213:  ldloc.s    i
    IL_0215:  call       instance uint8 Pololu.Usc.Usc::channelToPort(uint8)
    IL_021a:  ldc.i4.s   31
    IL_021c:  and
    IL_021d:  shl
    IL_021e:  conv.u1
    IL_021f:  or
    IL_0220:  conv.u1
    IL_0221:  stloc.3
    IL_0222:  nop
    IL_0223:  nop
    IL_0224:  br.s       IL_0253

    IL_0226:  nop
    IL_0227:  ldloc.s    channelModeBytes
    IL_0229:  ldloc.s    i
    IL_022b:  ldc.i4.2
    IL_022c:  shr
    IL_022d:  ldelema    [mscorlib]System.Byte
    IL_0232:  dup
    IL_0233:  ldobj      [mscorlib]System.Byte
    IL_0238:  ldloc.s    setting
    IL_023a:  ldfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_023f:  conv.u1
    IL_0240:  ldloc.s    i
    IL_0242:  ldc.i4.3
    IL_0243:  and
    IL_0244:  ldc.i4.1
    IL_0245:  shl
    IL_0246:  ldc.i4.s   31
    IL_0248:  and
    IL_0249:  shl
    IL_024a:  conv.u1
    IL_024b:  or
    IL_024c:  conv.u1
    IL_024d:  stobj      [mscorlib]System.Byte
    IL_0252:  nop
    IL_0253:  ldloc.s    setting
    IL_0255:  ldfld      valuetype Pololu.Usc.HomeMode Pololu.Usc.ChannelSetting::homeMode
    IL_025a:  stloc.s    correctedHomeMode
    IL_025c:  ldloc.s    setting
    IL_025e:  ldfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_0263:  ldc.i4.3
    IL_0264:  ceq
    IL_0266:  ldc.i4.0
    IL_0267:  ceq
    IL_0269:  stloc.s    CS$4$0000
    IL_026b:  ldloc.s    CS$4$0000
    IL_026d:  brtrue.s   IL_0274

    IL_026f:  nop
    IL_0270:  ldc.i4.1
    IL_0271:  stloc.s    correctedHomeMode
    IL_0273:  nop
    IL_0274:  ldloc.s    correctedHomeMode
    IL_0276:  ldc.i4.0
    IL_0277:  ceq
    IL_0279:  ldc.i4.0
    IL_027a:  ceq
    IL_027c:  stloc.s    CS$4$0000
    IL_027e:  ldloc.s    CS$4$0000
    IL_0280:  brtrue.s   IL_0287

    IL_0282:  ldc.i4.0
    IL_0283:  stloc.s    home
    IL_0285:  br.s       IL_02a3

    IL_0287:  ldloc.s    correctedHomeMode
    IL_0289:  ldc.i4.1
    IL_028a:  ceq
    IL_028c:  ldc.i4.0
    IL_028d:  ceq
    IL_028f:  stloc.s    CS$4$0000
    IL_0291:  ldloc.s    CS$4$0000
    IL_0293:  brtrue.s   IL_029a

    IL_0295:  ldc.i4.1
    IL_0296:  stloc.s    home
    IL_0298:  br.s       IL_02a3

    IL_029a:  ldloc.s    setting
    IL_029c:  ldfld      uint16 Pololu.Usc.ChannelSetting::home
    IL_02a1:  stloc.s    home
    IL_02a3:  ldarg.0
    IL_02a4:  ldarg.0
    IL_02a5:  ldc.i4.s   30
    IL_02a7:  ldloc.s    i
    IL_02a9:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_02ae:  ldloc.s    home
    IL_02b0:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_02b5:  nop
    IL_02b6:  ldarg.0
    IL_02b7:  ldarg.0
    IL_02b8:  ldc.i4.s   32
    IL_02ba:  ldloc.s    i
    IL_02bc:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_02c1:  ldloc.s    setting
    IL_02c3:  ldfld      uint16 Pololu.Usc.ChannelSetting::minimum
    IL_02c8:  ldc.i4.s   64
    IL_02ca:  div
    IL_02cb:  conv.u2
    IL_02cc:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_02d1:  nop
    IL_02d2:  ldarg.0
    IL_02d3:  ldarg.0
    IL_02d4:  ldc.i4.s   33
    IL_02d6:  ldloc.s    i
    IL_02d8:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_02dd:  ldloc.s    setting
    IL_02df:  ldfld      uint16 Pololu.Usc.ChannelSetting::maximum
    IL_02e4:  ldc.i4.s   64
    IL_02e6:  div
    IL_02e7:  conv.u2
    IL_02e8:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_02ed:  nop
    IL_02ee:  ldarg.0
    IL_02ef:  ldarg.0
    IL_02f0:  ldc.i4.s   34
    IL_02f2:  ldloc.s    i
    IL_02f4:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_02f9:  ldloc.s    setting
    IL_02fb:  ldfld      uint16 Pololu.Usc.ChannelSetting::neutral
    IL_0300:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_0305:  nop
    IL_0306:  ldarg.0
    IL_0307:  ldarg.0
    IL_0308:  ldc.i4.s   36
    IL_030a:  ldloc.s    i
    IL_030c:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_0311:  ldloc.s    setting
    IL_0313:  ldfld      uint16 Pololu.Usc.ChannelSetting::range
    IL_0318:  ldc.i4.s   127
    IL_031a:  div
    IL_031b:  conv.u2
    IL_031c:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_0321:  nop
    IL_0322:  ldarg.0
    IL_0323:  ldarg.0
    IL_0324:  ldc.i4.s   37
    IL_0326:  ldloc.s    i
    IL_0328:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_032d:  ldloc.s    setting
    IL_032f:  ldfld      uint16 Pololu.Usc.ChannelSetting::speed
    IL_0334:  call       uint8 Pololu.Usc.Usc::normalSpeedToExponentialSpeed(uint16)
    IL_0339:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_033e:  nop
    IL_033f:  ldarg.0
    IL_0340:  ldarg.0
    IL_0341:  ldc.i4.s   38
    IL_0343:  ldloc.s    i
    IL_0345:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_034a:  ldloc.s    setting
    IL_034c:  ldfld      uint8 Pololu.Usc.ChannelSetting::acceleration
    IL_0351:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_0356:  nop
    IL_0357:  nop
    IL_0358:  ldloc.s    i
    IL_035a:  ldc.i4.1
    IL_035b:  add
    IL_035c:  conv.u1
    IL_035d:  stloc.s    i
    IL_035f:  ldloc.s    i
    IL_0361:  ldarg.0
    IL_0362:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_0367:  clt
    IL_0369:  stloc.s    CS$4$0000
    IL_036b:  ldloc.s    CS$4$0000
    IL_036d:  brtrue     IL_0183

    IL_0372:  ldarg.0
    IL_0373:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_0378:  ldc.i4.0
    IL_0379:  ceq
    IL_037b:  stloc.s    CS$4$0000
    IL_037d:  ldloc.s    CS$4$0000
    IL_037f:  brtrue.s   IL_0399

    IL_0381:  nop
    IL_0382:  ldarg.0
    IL_0383:  ldc.i4.s   16
    IL_0385:  ldloc.2
    IL_0386:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_038b:  nop
    IL_038c:  ldarg.0
    IL_038d:  ldc.i4.s   17
    IL_038f:  ldloc.3
    IL_0390:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_0395:  nop
    IL_0396:  nop
    IL_0397:  br.s       IL_03c6

    IL_0399:  nop
    IL_039a:  ldc.i4.0
    IL_039b:  stloc.s    i
    IL_039d:  br.s       IL_03ba

    IL_039f:  nop
    IL_03a0:  ldarg.0
    IL_03a1:  ldc.i4.s   12
    IL_03a3:  ldloc.s    i
    IL_03a5:  add
    IL_03a6:  conv.u1
    IL_03a7:  ldloc.s    channelModeBytes
    IL_03a9:  ldloc.s    i
    IL_03ab:  ldelem.u1
    IL_03ac:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_03b1:  nop
    IL_03b2:  nop
    IL_03b3:  ldloc.s    i
    IL_03b5:  ldc.i4.1
    IL_03b6:  add
    IL_03b7:  conv.u1
    IL_03b8:  stloc.s    i
    IL_03ba:  ldloc.s    i
    IL_03bc:  ldc.i4.6
    IL_03bd:  clt
    IL_03bf:  stloc.s    CS$4$0000
    IL_03c1:  ldloc.s    CS$4$0000
    IL_03c3:  brtrue.s   IL_039f

    IL_03c5:  nop
    IL_03c6:  ldarg.2
    IL_03c7:  ldc.i4.0
    IL_03c8:  ceq
    IL_03ca:  stloc.s    CS$4$0000
    IL_03cc:  ldloc.s    CS$4$0000
    IL_03ce:  brtrue     IL_0493

    IL_03d3:  nop
    IL_03d4:  ldarg.0
    IL_03d5:  ldc.i4.1
    IL_03d6:  call       instance void Pololu.Usc.Usc::setScriptDone(uint8)
    IL_03db:  nop
    IL_03dc:  ldarg.1
    IL_03dd:  callvirt   instance class [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram Pololu.Usc.UscSettings::get_bytecodeProgram()
    IL_03e2:  stloc.s    program
    IL_03e4:  ldloc.s    program
    IL_03e6:  callvirt   instance class [mscorlib_5]System.Collections.Generic.List`1<uint8> [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram::getByteList()
    IL_03eb:  stloc.s    byteList
    IL_03ed:  ldloc.s    byteList
    IL_03ef:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
    IL_03f4:  ldarg.0
    IL_03f5:  call       instance uint16 Pololu.Usc.Usc::get_maxScriptLength()
    IL_03fa:  cgt
    IL_03fc:  ldc.i4.0
    IL_03fd:  ceq
    IL_03ff:  stloc.s    CS$4$0000
    IL_0401:  ldloc.s    CS$4$0000
    IL_0403:  brtrue.s   IL_0427

    IL_0405:  nop
    IL_0406:  ldstr      "Script too long for device ("
    IL_040b:  ldloc.s    byteList
    IL_040d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
    IL_0412:  box        [mscorlib]System.Int32
    IL_0417:  ldstr      " bytes)"
    IL_041c:  call       string [mscorlib]System.String::Concat(object,
                                                                object,
                                                                object)
    IL_0421:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0426:  throw

    IL_0427:  ldloc.s    byteList
    IL_0429:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
    IL_042e:  ldarg.0
    IL_042f:  call       instance uint16 Pololu.Usc.Usc::get_maxScriptLength()
    IL_0434:  clt
    IL_0436:  ldc.i4.0
    IL_0437:  ceq
    IL_0439:  stloc.s    CS$4$0000
    IL_043b:  ldloc.s    CS$4$0000
    IL_043d:  brtrue.s   IL_044a

    IL_043f:  nop
    IL_0440:  ldloc.s    byteList
    IL_0442:  ldc.i4.0
    IL_0443:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
    IL_0448:  nop
    IL_0449:  nop
    IL_044a:  ldarg.0
    IL_044b:  call       instance void Pololu.Usc.Usc::eraseScript()
    IL_0450:  nop
    IL_0451:  ldarg.0
    IL_0452:  ldloc.s    program
    IL_0454:  ldfld      class [mscorlib_5]System.Collections.Generic.Dictionary`2<string,uint16> [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram::subroutineAddresses
    IL_0459:  ldloc.s    program
    IL_045b:  ldfld      class [mscorlib_5]System.Collections.Generic.Dictionary`2<string,uint8> [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram::subroutineCommands
    IL_0460:  call       instance void Pololu.Usc.Usc::setSubroutines(class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16>,
                                                                      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8>)
    IL_0465:  nop
    IL_0466:  ldarg.0
    IL_0467:  ldloc.s    byteList
    IL_0469:  call       instance void Pololu.Usc.Usc::writeScript(class [mscorlib]System.Collections.Generic.List`1<uint8>)
    IL_046e:  nop
    IL_046f:  ldarg.0
    IL_0470:  ldc.i4.s   22
    IL_0472:  ldloc.s    program
    IL_0474:  callvirt   instance uint16 [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram::getCRC()
    IL_0479:  call       instance void Pololu.Usc.Usc::setRawParameter(valuetype Pololu.Usc.uscParameter,
                                                                       uint16)
    IL_047e:  nop
    IL_047f:  ldloc.1
    IL_0480:  ldstr      "script"
    IL_0485:  ldarg.1
    IL_0486:  callvirt   instance string Pololu.Usc.UscSettings::get_script()
    IL_048b:  ldc.i4.1
    IL_048c:  callvirt   instance void [mscorlib]Microsoft.Win32.RegistryKey::SetValue(string,
                                                                                       object,
                                                                                       valuetype [mscorlib]Microsoft.Win32.RegistryValueKind)
    IL_0491:  nop
    IL_0492:  nop
    IL_0493:  ldarg.1
    IL_0494:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence> Pololu.Usc.UscSettings::sequences
    IL_0499:  ldloc.1
    IL_049a:  call       void [Sequencer]Pololu.Usc.Sequencer.Sequence::saveSequencesInRegistry(class [mscorlib]System.Collections.Generic.IList`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence>,
                                                                                                class [mscorlib]Microsoft.Win32.RegistryKey)
    IL_049f:  nop
    IL_04a0:  ldloc.1
    IL_04a1:  callvirt   instance void [mscorlib]Microsoft.Win32.RegistryKey::Close()
    IL_04a6:  nop
    IL_04a7:  ret
  } // end of method Usc::setUscSettings

  .method private hidebysig instance class [mscorlib]Microsoft.Win32.RegistryKey 
          openRegistryKey() cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  3
    .locals init ([0] string keyname,
             [1] class [mscorlib]Microsoft.Win32.RegistryKey key,
             [2] class [mscorlib]Microsoft.Win32.RegistryKey CS$1$0000,
             [3] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldstr      "Software\\Pololu\\Maestro USB servo controller\\"
    IL_0006:  ldarg.0
    IL_0007:  call       instance string [UsbWrapper]Pololu.UsbWrapper.UsbDevice::getSerialNumber()
    IL_000c:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0011:  stloc.0
    IL_0012:  ldsfld     class [mscorlib]Microsoft.Win32.RegistryKey [mscorlib]Microsoft.Win32.Registry::CurrentUser
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.1
    IL_0019:  callvirt   instance class [mscorlib]Microsoft.Win32.RegistryKey [mscorlib]Microsoft.Win32.RegistryKey::OpenSubKey(string,
                                                                                                                                bool)
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldnull
    IL_0021:  ceq
    IL_0023:  ldc.i4.0
    IL_0024:  ceq
    IL_0026:  stloc.3
    IL_0027:  ldloc.3
    IL_0028:  brtrue.s   IL_0038

    IL_002a:  nop
    IL_002b:  ldsfld     class [mscorlib]Microsoft.Win32.RegistryKey [mscorlib]Microsoft.Win32.Registry::CurrentUser
    IL_0030:  ldloc.0
    IL_0031:  callvirt   instance class [mscorlib]Microsoft.Win32.RegistryKey [mscorlib]Microsoft.Win32.RegistryKey::CreateSubKey(string)
    IL_0036:  stloc.1
    IL_0037:  nop
    IL_0038:  ldloc.1
    IL_0039:  stloc.2
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.2
    IL_003d:  ret
  } // end of method Usc::openRegistryKey

  .method private hidebysig instance void 
          setRawParameter(valuetype Pololu.Usc.uscParameter parameter,
                          uint16 'value') cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  4
    .locals init ([0] valuetype Pololu.Usc.Usc/Range range,
             [1] int32 bytes)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  call       valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc::getRange(valuetype Pololu.Usc.uscParameter)
    IL_0007:  stloc.0
    IL_0008:  ldarg.2
    IL_0009:  ldloca.s   range
    IL_000b:  ldfld      int32 Pololu.Usc.Usc/Range::minimumValue
    IL_0010:  ldloca.s   range
    IL_0012:  ldfld      int32 Pololu.Usc.Usc/Range::maximumValue
    IL_0017:  ldarg.1
    IL_0018:  box        Pololu.Usc.uscParameter
    IL_001d:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0022:  call       void Pololu.Usc.Usc::requireArgumentRange(uint32,
                                                                   int32,
                                                                   int32,
                                                                   string)
    IL_0027:  nop
    IL_0028:  ldloca.s   range
    IL_002a:  ldfld      uint8 Pololu.Usc.Usc/Range::bytes
    IL_002f:  stloc.1
    IL_0030:  ldarg.0
    IL_0031:  ldarg.1
    IL_0032:  ldarg.2
    IL_0033:  ldloc.1
    IL_0034:  call       instance void Pololu.Usc.Usc::setRawParameterNoChecks(uint16,
                                                                               uint16,
                                                                               int32)
    IL_0039:  nop
    IL_003a:  ret
  } // end of method Usc::setRawParameter

  .method private hidebysig instance void 
          setRawParameterNoChecks(uint16 parameter,
                                  uint16 'value',
                                  int32 bytes) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  5
    .locals init ([0] uint16 index,
             [1] class [mscorlib]System.Exception e)
    IL_0000:  nop
    IL_0001:  ldarg.3
    IL_0002:  ldc.i4.8
    IL_0003:  shl
    IL_0004:  ldarg.1
    IL_0005:  add
    IL_0006:  conv.u2
    IL_0007:  stloc.0
    .try
    {
      IL_0008:  nop
      IL_0009:  ldarg.0
      IL_000a:  ldc.i4.s   64
      IL_000c:  ldc.i4     0x82
      IL_0011:  ldarg.2
      IL_0012:  ldloc.0
      IL_0013:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                  uint8,
                                                                                                  uint16,
                                                                                                  uint16)
      IL_0018:  nop
      IL_0019:  nop
      IL_001a:  leave.s    IL_003b

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_001c:  stloc.1
      IL_001d:  nop
      IL_001e:  ldstr      "There was an error setting parameter "
      IL_0023:  ldarga.s   parameter
      IL_0025:  call       instance string [mscorlib]System.UInt16::ToString()
      IL_002a:  ldstr      " on the device."
      IL_002f:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string)
      IL_0034:  ldloc.1
      IL_0035:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_003a:  throw

    }  // end handler
    IL_003b:  nop
    IL_003c:  ret
  } // end of method Usc::setRawParameterNoChecks

  .method private hidebysig instance uint16 
          getRawParameter(valuetype Pololu.Usc.uscParameter parameter) cil managed
  {
    // Code size       194 (0xc2)
    .maxstack  6
    .locals init ([0] valuetype Pololu.Usc.Usc/Range range,
             [1] uint16 'value',
             [2] uint8[] 'array',
             [3] class [mscorlib]System.Exception e,
             [4] uint8& pinned pointer,
             [5] uint16 CS$1$0000,
             [6] bool CS$4$0001,
             [7] uint8[] CS$0$0002)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  call       valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc::getRange(valuetype Pololu.Usc.uscParameter)
    IL_0007:  stloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  stloc.1
    IL_000a:  ldloca.s   range
    IL_000c:  ldfld      uint8 Pololu.Usc.Usc/Range::bytes
    IL_0011:  newarr     [mscorlib]System.Byte
    IL_0016:  stloc.2
    .try
    {
      IL_0017:  nop
      IL_0018:  ldarg.0
      IL_0019:  ldc.i4     0xc0
      IL_001e:  ldc.i4     0x81
      IL_0023:  ldc.i4.0
      IL_0024:  ldarg.1
      IL_0025:  ldloc.2
      IL_0026:  call       instance uint32 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                    uint8,
                                                                                                    uint16,
                                                                                                    uint16,
                                                                                                    uint8[])
      IL_002b:  pop
      IL_002c:  nop
      IL_002d:  leave.s    IL_0052

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_002f:  stloc.3
      IL_0030:  nop
      IL_0031:  ldstr      "There was an error getting parameter "
      IL_0036:  ldarg.1
      IL_0037:  box        Pololu.Usc.uscParameter
      IL_003c:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0041:  ldstr      " from the device."
      IL_0046:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string)
      IL_004b:  ldloc.3
      IL_004c:  newobj     instance void [mscorlib]System.Exception::.ctor(string,
                                                                           class [mscorlib]System.Exception)
      IL_0051:  throw

    }  // end handler
    IL_0052:  nop
    IL_0053:  ldloca.s   range
    IL_0055:  ldfld      uint8 Pololu.Usc.Usc/Range::bytes
    IL_005a:  ldc.i4.1
    IL_005b:  ceq
    IL_005d:  ldc.i4.0
    IL_005e:  ceq
    IL_0060:  stloc.s    CS$4$0001
    IL_0062:  ldloc.s    CS$4$0001
    IL_0064:  brtrue.s   IL_0091

    IL_0066:  nop
    IL_0067:  ldloc.2
    IL_0068:  dup
    IL_0069:  stloc.s    CS$0$0002
    IL_006b:  brfalse.s  IL_0073

    IL_006d:  ldloc.s    CS$0$0002
    IL_006f:  ldlen
    IL_0070:  conv.i4
    IL_0071:  brtrue.s   IL_0079

    IL_0073:  ldc.i4.0
    IL_0074:  conv.u
    IL_0075:  stloc.s    pointer
    IL_0077:  br.s       IL_0083

    IL_0079:  ldloc.s    CS$0$0002
    IL_007b:  ldc.i4.0
    IL_007c:  ldelema    [mscorlib]System.Byte
    IL_0081:  stloc.s    pointer
    IL_0083:  nop
    IL_0084:  ldloc.s    pointer
    IL_0086:  conv.i
    IL_0087:  ldind.u1
    IL_0088:  stloc.1
    IL_0089:  nop
    IL_008a:  ldc.i4.0
    IL_008b:  conv.u
    IL_008c:  stloc.s    pointer
    IL_008e:  nop
    IL_008f:  br.s       IL_00ba

    IL_0091:  nop
    IL_0092:  ldloc.2
    IL_0093:  dup
    IL_0094:  stloc.s    CS$0$0002
    IL_0096:  brfalse.s  IL_009e

    IL_0098:  ldloc.s    CS$0$0002
    IL_009a:  ldlen
    IL_009b:  conv.i4
    IL_009c:  brtrue.s   IL_00a4

    IL_009e:  ldc.i4.0
    IL_009f:  conv.u
    IL_00a0:  stloc.s    pointer
    IL_00a2:  br.s       IL_00ae

    IL_00a4:  ldloc.s    CS$0$0002
    IL_00a6:  ldc.i4.0
    IL_00a7:  ldelema    [mscorlib]System.Byte
    IL_00ac:  stloc.s    pointer
    IL_00ae:  nop
    IL_00af:  ldloc.s    pointer
    IL_00b1:  conv.i
    IL_00b2:  ldind.u2
    IL_00b3:  stloc.1
    IL_00b4:  nop
    IL_00b5:  ldc.i4.0
    IL_00b6:  conv.u
    IL_00b7:  stloc.s    pointer
    IL_00b9:  nop
    IL_00ba:  ldloc.1
    IL_00bb:  stloc.s    CS$1$0000
    IL_00bd:  br.s       IL_00bf

    IL_00bf:  ldloc.s    CS$1$0000
    IL_00c1:  ret
  } // end of method Usc::getRawParameter

  .method public hidebysig newslot virtual final 
          instance class Pololu.Usc.UscSettings 
          getUscSettings() cil managed
  {
    // Code size       1103 (0x44f)
    .maxstack  6
    .locals init ([0] class Pololu.Usc.UscSettings settings,
             [1] uint32 tmp,
             [2] uint8 ioMask,
             [3] uint8 outputMask,
             [4] uint8[] channelModeBytes,
             [5] uint8 i,
             [6] class Pololu.Usc.ChannelSetting setting,
             [7] uint8 bitmask,
             [8] uint16 home,
             [9] class [mscorlib]Microsoft.Win32.RegistryKey key,
             [10] string script,
             [11] class [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram program,
             [12] class Pololu.Usc.UscSettings CS$1$0000,
             [13] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  newobj     instance void Pololu.Usc.UscSettings::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.3
    IL_000a:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_000f:  conv.u1
    IL_0010:  stfld      valuetype Pololu.Usc.uscSerialMode Pololu.Usc.UscSettings::serialMode
    IL_0015:  ldloc.0
    IL_0016:  ldarg.0
    IL_0017:  ldc.i4.4
    IL_0018:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_001d:  call       uint32 Pololu.Usc.Usc::convertSpbrgToBps(uint16)
    IL_0022:  stfld      uint32 Pololu.Usc.UscSettings::fixedBaudRate
    IL_0027:  ldloc.0
    IL_0028:  ldarg.0
    IL_0029:  ldc.i4.8
    IL_002a:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_002f:  ldc.i4.0
    IL_0030:  ceq
    IL_0032:  ldc.i4.0
    IL_0033:  ceq
    IL_0035:  stfld      bool Pololu.Usc.UscSettings::enableCrc
    IL_003a:  ldloc.0
    IL_003b:  ldarg.0
    IL_003c:  ldc.i4.s   9
    IL_003e:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_0043:  ldc.i4.0
    IL_0044:  ceq
    IL_0046:  ldc.i4.0
    IL_0047:  ceq
    IL_0049:  stfld      bool Pololu.Usc.UscSettings::neverSuspend
    IL_004e:  ldloc.0
    IL_004f:  ldarg.0
    IL_0050:  ldc.i4.s   10
    IL_0052:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_0057:  conv.u1
    IL_0058:  stfld      uint8 Pololu.Usc.UscSettings::serialDeviceNumber
    IL_005d:  ldloc.0
    IL_005e:  ldarg.0
    IL_005f:  ldc.i4.s   25
    IL_0061:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_0066:  conv.u1
    IL_0067:  stfld      uint8 Pololu.Usc.UscSettings::miniSscOffset
    IL_006c:  ldloc.0
    IL_006d:  ldarg.0
    IL_006e:  ldc.i4.6
    IL_006f:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_0074:  stfld      uint16 Pololu.Usc.UscSettings::serialTimeout
    IL_0079:  ldloc.0
    IL_007a:  ldarg.0
    IL_007b:  ldc.i4.s   24
    IL_007d:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_0082:  ldc.i4.0
    IL_0083:  ceq
    IL_0085:  ldc.i4.0
    IL_0086:  ceq
    IL_0088:  stfld      bool Pololu.Usc.UscSettings::scriptDone
    IL_008d:  ldarg.0
    IL_008e:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_0093:  ldc.i4.6
    IL_0094:  ceq
    IL_0096:  ldc.i4.0
    IL_0097:  ceq
    IL_0099:  stloc.s    CS$4$0001
    IL_009b:  ldloc.s    CS$4$0001
    IL_009d:  brtrue.s   IL_00bf

    IL_009f:  nop
    IL_00a0:  ldloc.0
    IL_00a1:  ldarg.0
    IL_00a2:  ldc.i4.1
    IL_00a3:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_00a8:  conv.u1
    IL_00a9:  stfld      uint8 Pololu.Usc.UscSettings::servosAvailable
    IL_00ae:  ldloc.0
    IL_00af:  ldarg.0
    IL_00b0:  ldc.i4.2
    IL_00b1:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_00b6:  conv.u1
    IL_00b7:  stfld      uint8 Pololu.Usc.UscSettings::servoPeriod
    IL_00bc:  nop
    IL_00bd:  br.s       IL_00f0

    IL_00bf:  nop
    IL_00c0:  ldarg.0
    IL_00c1:  ldc.i4.s   19
    IL_00c3:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_00c8:  ldc.i4.8
    IL_00c9:  shl
    IL_00ca:  stloc.1
    IL_00cb:  ldloc.1
    IL_00cc:  ldarg.0
    IL_00cd:  ldc.i4.s   18
    IL_00cf:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_00d4:  conv.u1
    IL_00d5:  or
    IL_00d6:  stloc.1
    IL_00d7:  ldloc.0
    IL_00d8:  ldloc.1
    IL_00d9:  stfld      uint32 Pololu.Usc.UscSettings::miniMaestroServoPeriod
    IL_00de:  ldloc.0
    IL_00df:  ldarg.0
    IL_00e0:  ldc.i4.s   26
    IL_00e2:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_00e7:  ldc.i4.1
    IL_00e8:  add
    IL_00e9:  conv.u2
    IL_00ea:  stfld      uint16 Pololu.Usc.UscSettings::servoMultiplier
    IL_00ef:  nop
    IL_00f0:  ldarg.0
    IL_00f1:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_00f6:  ldc.i4.s   18
    IL_00f8:  cgt
    IL_00fa:  ldc.i4.0
    IL_00fb:  ceq
    IL_00fd:  stloc.s    CS$4$0001
    IL_00ff:  ldloc.s    CS$4$0001
    IL_0101:  brtrue.s   IL_0119

    IL_0103:  nop
    IL_0104:  ldloc.0
    IL_0105:  ldarg.0
    IL_0106:  ldc.i4.s   21
    IL_0108:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_010d:  ldc.i4.0
    IL_010e:  ceq
    IL_0110:  ldc.i4.0
    IL_0111:  ceq
    IL_0113:  stfld      bool Pololu.Usc.UscSettings::enablePullups
    IL_0118:  nop
    IL_0119:  ldc.i4.0
    IL_011a:  stloc.2
    IL_011b:  ldc.i4.0
    IL_011c:  stloc.3
    IL_011d:  ldc.i4.6
    IL_011e:  newarr     [mscorlib]System.Byte
    IL_0123:  stloc.s    channelModeBytes
    IL_0125:  ldarg.0
    IL_0126:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_012b:  ldc.i4.0
    IL_012c:  ceq
    IL_012e:  stloc.s    CS$4$0001
    IL_0130:  ldloc.s    CS$4$0001
    IL_0132:  brtrue.s   IL_014c

    IL_0134:  nop
    IL_0135:  ldarg.0
    IL_0136:  ldc.i4.s   16
    IL_0138:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_013d:  conv.u1
    IL_013e:  stloc.2
    IL_013f:  ldarg.0
    IL_0140:  ldc.i4.s   17
    IL_0142:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_0147:  conv.u1
    IL_0148:  stloc.3
    IL_0149:  nop
    IL_014a:  br.s       IL_0179

    IL_014c:  nop
    IL_014d:  ldc.i4.0
    IL_014e:  stloc.s    i
    IL_0150:  br.s       IL_016d

    IL_0152:  nop
    IL_0153:  ldloc.s    channelModeBytes
    IL_0155:  ldloc.s    i
    IL_0157:  ldarg.0
    IL_0158:  ldc.i4.s   12
    IL_015a:  ldloc.s    i
    IL_015c:  add
    IL_015d:  conv.u1
    IL_015e:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_0163:  conv.u1
    IL_0164:  stelem.i1
    IL_0165:  nop
    IL_0166:  ldloc.s    i
    IL_0168:  ldc.i4.1
    IL_0169:  add
    IL_016a:  conv.u1
    IL_016b:  stloc.s    i
    IL_016d:  ldloc.s    i
    IL_016f:  ldc.i4.6
    IL_0170:  clt
    IL_0172:  stloc.s    CS$4$0001
    IL_0174:  ldloc.s    CS$4$0001
    IL_0176:  brtrue.s   IL_0152

    IL_0178:  nop
    IL_0179:  ldc.i4.0
    IL_017a:  stloc.s    i
    IL_017c:  br         IL_0326

    IL_0181:  nop
    IL_0182:  newobj     instance void Pololu.Usc.ChannelSetting::.ctor()
    IL_0187:  stloc.s    setting
    IL_0189:  ldarg.0
    IL_018a:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_018f:  ldc.i4.0
    IL_0190:  ceq
    IL_0192:  stloc.s    CS$4$0001
    IL_0194:  ldloc.s    CS$4$0001
    IL_0196:  brtrue.s   IL_01ee

    IL_0198:  nop
    IL_0199:  ldc.i4.1
    IL_019a:  ldarg.0
    IL_019b:  ldloc.s    i
    IL_019d:  call       instance uint8 Pololu.Usc.Usc::channelToPort(uint8)
    IL_01a2:  ldc.i4.s   31
    IL_01a4:  and
    IL_01a5:  shl
    IL_01a6:  conv.u1
    IL_01a7:  stloc.s    bitmask
    IL_01a9:  ldloc.2
    IL_01aa:  ldloc.s    bitmask
    IL_01ac:  and
    IL_01ad:  ldc.i4.0
    IL_01ae:  ceq
    IL_01b0:  ldc.i4.0
    IL_01b1:  ceq
    IL_01b3:  stloc.s    CS$4$0001
    IL_01b5:  ldloc.s    CS$4$0001
    IL_01b7:  brtrue.s   IL_01c5

    IL_01b9:  nop
    IL_01ba:  ldloc.s    setting
    IL_01bc:  ldc.i4.0
    IL_01bd:  stfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_01c2:  nop
    IL_01c3:  br.s       IL_01eb

    IL_01c5:  ldloc.3
    IL_01c6:  ldloc.s    bitmask
    IL_01c8:  and
    IL_01c9:  ldc.i4.0
    IL_01ca:  ceq
    IL_01cc:  ldc.i4.0
    IL_01cd:  ceq
    IL_01cf:  stloc.s    CS$4$0001
    IL_01d1:  ldloc.s    CS$4$0001
    IL_01d3:  brtrue.s   IL_01e1

    IL_01d5:  nop
    IL_01d6:  ldloc.s    setting
    IL_01d8:  ldc.i4.3
    IL_01d9:  stfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_01de:  nop
    IL_01df:  br.s       IL_01eb

    IL_01e1:  nop
    IL_01e2:  ldloc.s    setting
    IL_01e4:  ldc.i4.2
    IL_01e5:  stfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_01ea:  nop
    IL_01eb:  nop
    IL_01ec:  br.s       IL_020a

    IL_01ee:  nop
    IL_01ef:  ldloc.s    setting
    IL_01f1:  ldloc.s    channelModeBytes
    IL_01f3:  ldloc.s    i
    IL_01f5:  ldc.i4.2
    IL_01f6:  shr
    IL_01f7:  ldelem.u1
    IL_01f8:  ldloc.s    i
    IL_01fa:  ldc.i4.3
    IL_01fb:  and
    IL_01fc:  ldc.i4.1
    IL_01fd:  shl
    IL_01fe:  ldc.i4.s   31
    IL_0200:  and
    IL_0201:  shr
    IL_0202:  ldc.i4.3
    IL_0203:  and
    IL_0204:  stfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_0209:  nop
    IL_020a:  ldarg.0
    IL_020b:  ldarg.0
    IL_020c:  ldc.i4.s   30
    IL_020e:  ldloc.s    i
    IL_0210:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_0215:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_021a:  stloc.s    home
    IL_021c:  ldloc.s    home
    IL_021e:  ldc.i4.0
    IL_021f:  ceq
    IL_0221:  ldc.i4.0
    IL_0222:  ceq
    IL_0224:  stloc.s    CS$4$0001
    IL_0226:  ldloc.s    CS$4$0001
    IL_0228:  brtrue.s   IL_023e

    IL_022a:  nop
    IL_022b:  ldloc.s    setting
    IL_022d:  ldc.i4.0
    IL_022e:  stfld      valuetype Pololu.Usc.HomeMode Pololu.Usc.ChannelSetting::homeMode
    IL_0233:  ldloc.s    setting
    IL_0235:  ldc.i4.0
    IL_0236:  stfld      uint16 Pololu.Usc.ChannelSetting::home
    IL_023b:  nop
    IL_023c:  br.s       IL_0273

    IL_023e:  ldloc.s    home
    IL_0240:  ldc.i4.1
    IL_0241:  ceq
    IL_0243:  ldc.i4.0
    IL_0244:  ceq
    IL_0246:  stloc.s    CS$4$0001
    IL_0248:  ldloc.s    CS$4$0001
    IL_024a:  brtrue.s   IL_0260

    IL_024c:  nop
    IL_024d:  ldloc.s    setting
    IL_024f:  ldc.i4.1
    IL_0250:  stfld      valuetype Pololu.Usc.HomeMode Pololu.Usc.ChannelSetting::homeMode
    IL_0255:  ldloc.s    setting
    IL_0257:  ldc.i4.0
    IL_0258:  stfld      uint16 Pololu.Usc.ChannelSetting::home
    IL_025d:  nop
    IL_025e:  br.s       IL_0273

    IL_0260:  nop
    IL_0261:  ldloc.s    setting
    IL_0263:  ldc.i4.2
    IL_0264:  stfld      valuetype Pololu.Usc.HomeMode Pololu.Usc.ChannelSetting::homeMode
    IL_0269:  ldloc.s    setting
    IL_026b:  ldloc.s    home
    IL_026d:  stfld      uint16 Pololu.Usc.ChannelSetting::home
    IL_0272:  nop
    IL_0273:  ldloc.s    setting
    IL_0275:  ldc.i4.s   64
    IL_0277:  ldarg.0
    IL_0278:  ldarg.0
    IL_0279:  ldc.i4.s   32
    IL_027b:  ldloc.s    i
    IL_027d:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_0282:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_0287:  mul
    IL_0288:  conv.u2
    IL_0289:  stfld      uint16 Pololu.Usc.ChannelSetting::minimum
    IL_028e:  ldloc.s    setting
    IL_0290:  ldc.i4.s   64
    IL_0292:  ldarg.0
    IL_0293:  ldarg.0
    IL_0294:  ldc.i4.s   33
    IL_0296:  ldloc.s    i
    IL_0298:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_029d:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_02a2:  mul
    IL_02a3:  conv.u2
    IL_02a4:  stfld      uint16 Pololu.Usc.ChannelSetting::maximum
    IL_02a9:  ldloc.s    setting
    IL_02ab:  ldarg.0
    IL_02ac:  ldarg.0
    IL_02ad:  ldc.i4.s   34
    IL_02af:  ldloc.s    i
    IL_02b1:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_02b6:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_02bb:  stfld      uint16 Pololu.Usc.ChannelSetting::neutral
    IL_02c0:  ldloc.s    setting
    IL_02c2:  ldc.i4.s   127
    IL_02c4:  ldarg.0
    IL_02c5:  ldarg.0
    IL_02c6:  ldc.i4.s   36
    IL_02c8:  ldloc.s    i
    IL_02ca:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_02cf:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_02d4:  mul
    IL_02d5:  conv.u2
    IL_02d6:  stfld      uint16 Pololu.Usc.ChannelSetting::range
    IL_02db:  ldloc.s    setting
    IL_02dd:  ldarg.0
    IL_02de:  ldarg.0
    IL_02df:  ldc.i4.s   37
    IL_02e1:  ldloc.s    i
    IL_02e3:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_02e8:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_02ed:  conv.u1
    IL_02ee:  call       uint16 Pololu.Usc.Usc::exponentialSpeedToNormalSpeed(uint8)
    IL_02f3:  stfld      uint16 Pololu.Usc.ChannelSetting::speed
    IL_02f8:  ldloc.s    setting
    IL_02fa:  ldarg.0
    IL_02fb:  ldarg.0
    IL_02fc:  ldc.i4.s   38
    IL_02fe:  ldloc.s    i
    IL_0300:  call       instance valuetype Pololu.Usc.uscParameter Pololu.Usc.Usc::specifyServo(valuetype Pololu.Usc.uscParameter,
                                                                                                 uint8)
    IL_0305:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
    IL_030a:  conv.u1
    IL_030b:  stfld      uint8 Pololu.Usc.ChannelSetting::acceleration
    IL_0310:  ldloc.0
    IL_0311:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting> Pololu.Usc.UscSettings::channelSettings
    IL_0316:  ldloc.s    setting
    IL_0318:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting>::Add(!0)
    IL_031d:  nop
    IL_031e:  nop
    IL_031f:  ldloc.s    i
    IL_0321:  ldc.i4.1
    IL_0322:  add
    IL_0323:  conv.u1
    IL_0324:  stloc.s    i
    IL_0326:  ldloc.s    i
    IL_0328:  ldarg.0
    IL_0329:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_032e:  clt
    IL_0330:  stloc.s    CS$4$0001
    IL_0332:  ldloc.s    CS$4$0001
    IL_0334:  brtrue     IL_0181

    IL_0339:  ldarg.0
    IL_033a:  call       instance class [mscorlib]Microsoft.Win32.RegistryKey Pololu.Usc.Usc::openRegistryKey()
    IL_033f:  stloc.s    key
    IL_0341:  ldloc.s    key
    IL_0343:  ldnull
    IL_0344:  ceq
    IL_0346:  stloc.s    CS$4$0001
    IL_0348:  ldloc.s    CS$4$0001
    IL_034a:  brtrue     IL_0447

    IL_034f:  nop
    IL_0350:  ldc.i4.0
    IL_0351:  stloc.s    i
    IL_0353:  br.s       IL_0397

    IL_0355:  nop
    IL_0356:  ldloc.0
    IL_0357:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting> Pololu.Usc.UscSettings::channelSettings
    IL_035c:  ldloc.s    i
    IL_035e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting>::get_Item(int32)
    IL_0363:  ldloc.s    key
    IL_0365:  ldstr      "servoName"
    IL_036a:  ldloca.s   i
    IL_036c:  ldstr      "d2"
    IL_0371:  call       instance string [mscorlib]System.Byte::ToString(string)
    IL_0376:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_037b:  ldstr      ""
    IL_0380:  callvirt   instance object [mscorlib]Microsoft.Win32.RegistryKey::GetValue(string,
                                                                                         object)
    IL_0385:  castclass  [mscorlib]System.String
    IL_038a:  stfld      string Pololu.Usc.ChannelSetting::name
    IL_038f:  nop
    IL_0390:  ldloc.s    i
    IL_0392:  ldc.i4.1
    IL_0393:  add
    IL_0394:  conv.u1
    IL_0395:  stloc.s    i
    IL_0397:  ldloc.s    i
    IL_0399:  ldarg.0
    IL_039a:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_039f:  clt
    IL_03a1:  stloc.s    CS$4$0001
    IL_03a3:  ldloc.s    CS$4$0001
    IL_03a5:  brtrue.s   IL_0355

    IL_03a7:  ldloc.s    key
    IL_03a9:  ldstr      "script"
    IL_03ae:  callvirt   instance object [mscorlib]Microsoft.Win32.RegistryKey::GetValue(string)
    IL_03b3:  castclass  [mscorlib]System.String
    IL_03b8:  stloc.s    script
    IL_03ba:  ldloc.s    script
    IL_03bc:  ldnull
    IL_03bd:  ceq
    IL_03bf:  ldc.i4.0
    IL_03c0:  ceq
    IL_03c2:  stloc.s    CS$4$0001
    IL_03c4:  ldloc.s    CS$4$0001
    IL_03c6:  brtrue.s   IL_03cf

    IL_03c8:  ldstr      ""
    IL_03cd:  stloc.s    script
    .try
    {
      IL_03cf:  nop
      IL_03d0:  ldloc.0
      IL_03d1:  ldloc.s    script
      IL_03d3:  callvirt   instance void Pololu.Usc.UscSettings::setAndCompileScript(string)
      IL_03d8:  nop
      IL_03d9:  ldloc.0
      IL_03da:  callvirt   instance class [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram Pololu.Usc.UscSettings::get_bytecodeProgram()
      IL_03df:  stloc.s    program
      IL_03e1:  ldloc.s    program
      IL_03e3:  callvirt   instance class [mscorlib_5]System.Collections.Generic.List`1<uint8> [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram::getByteList()
      IL_03e8:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
      IL_03ed:  ldarg.0
      IL_03ee:  call       instance uint16 Pololu.Usc.Usc::get_maxScriptLength()
      IL_03f3:  cgt
      IL_03f5:  ldc.i4.0
      IL_03f6:  ceq
      IL_03f8:  stloc.s    CS$4$0001
      IL_03fa:  ldloc.s    CS$4$0001
      IL_03fc:  brtrue.s   IL_0405

      IL_03fe:  nop
      IL_03ff:  newobj     instance void [mscorlib]System.Exception::.ctor()
      IL_0404:  throw

      IL_0405:  ldloc.s    program
      IL_0407:  callvirt   instance uint16 [Bytecode]Pololu.Usc.Bytecode.BytecodeProgram::getCRC()
      IL_040c:  ldarg.0
      IL_040d:  ldc.i4.s   22
      IL_040f:  call       instance uint16 Pololu.Usc.Usc::getRawParameter(valuetype Pololu.Usc.uscParameter)
      IL_0414:  ceq
      IL_0416:  stloc.s    CS$4$0001
      IL_0418:  ldloc.s    CS$4$0001
      IL_041a:  brtrue.s   IL_0423

      IL_041c:  nop
      IL_041d:  newobj     instance void [mscorlib]System.Exception::.ctor()
      IL_0422:  throw

      IL_0423:  nop
      IL_0424:  leave.s    IL_0432

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0426:  pop
      IL_0427:  nop
      IL_0428:  ldloc.0
      IL_0429:  ldc.i4.1
      IL_042a:  stfld      bool Pololu.Usc.UscSettings::scriptInconsistent
      IL_042f:  nop
      IL_0430:  leave.s    IL_0432

    }  // end handler
    IL_0432:  nop
    IL_0433:  ldloc.0
    IL_0434:  ldloc.s    key
    IL_0436:  ldarg.0
    IL_0437:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_043c:  call       class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence> [Sequencer]Pololu.Usc.Sequencer.Sequence::readSequencesFromRegistry(class [mscorlib]Microsoft.Win32.RegistryKey,
                                                                                                                                                                                               uint8)
    IL_0441:  stfld      class [mscorlib]System.Collections.Generic.List`1<class [Sequencer]Pololu.Usc.Sequencer.Sequence> Pololu.Usc.UscSettings::sequences
    IL_0446:  nop
    IL_0447:  ldloc.0
    IL_0448:  stloc.s    CS$1$0000
    IL_044a:  br.s       IL_044c

    IL_044c:  ldloc.s    CS$1$0000
    IL_044e:  ret
  } // end of method Usc::getUscSettings

  .method public hidebysig specialname instance uint16 
          get_maxScriptLength() cil managed
  {
    // Code size       34 (0x22)
    .maxstack  2
    .locals init ([0] uint16 CS$1$0000,
             [1] bool CS$4$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  stloc.1
    IL_000b:  ldloc.1
    IL_000c:  brtrue.s   IL_0017

    IL_000e:  nop
    IL_000f:  ldc.i4     0x400
    IL_0014:  stloc.0
    IL_0015:  br.s       IL_0020

    IL_0017:  nop
    IL_0018:  ldc.i4     0x2000
    IL_001d:  stloc.0
    IL_001e:  br.s       IL_0020

    IL_0020:  ldloc.0
    IL_0021:  ret
  } // end of method Usc::get_maxScriptLength

  .method private hidebysig static void  requireArgumentRange(uint32 argumentValue,
                                                              int32 minimum,
                                                              int32 maximum,
                                                              string argumentName) cil managed
  {
    // Code size       107 (0x6b)
    .maxstack  3
    .locals init ([0] bool CS$4$0000,
             [1] object[] CS$0$0001)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  conv.u8
    IL_0003:  ldarg.1
    IL_0004:  conv.i8
    IL_0005:  blt.s      IL_0012

    IL_0007:  ldarg.0
    IL_0008:  conv.u8
    IL_0009:  ldarg.2
    IL_000a:  conv.i8
    IL_000b:  cgt
    IL_000d:  ldc.i4.0
    IL_000e:  ceq
    IL_0010:  br.s       IL_0013

    IL_0012:  ldc.i4.0
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  brtrue.s   IL_006a

    IL_0017:  nop
    IL_0018:  ldc.i4.8
    IL_0019:  newarr     [mscorlib]System.Object
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.0
    IL_0021:  ldstr      "The "
    IL_0026:  stelem.ref
    IL_0027:  ldloc.1
    IL_0028:  ldc.i4.1
    IL_0029:  ldarg.3
    IL_002a:  stelem.ref
    IL_002b:  ldloc.1
    IL_002c:  ldc.i4.2
    IL_002d:  ldstr      " must be between "
    IL_0032:  stelem.ref
    IL_0033:  ldloc.1
    IL_0034:  ldc.i4.3
    IL_0035:  ldarg.1
    IL_0036:  box        [mscorlib]System.Int32
    IL_003b:  stelem.ref
    IL_003c:  ldloc.1
    IL_003d:  ldc.i4.4
    IL_003e:  ldstr      " and "
    IL_0043:  stelem.ref
    IL_0044:  ldloc.1
    IL_0045:  ldc.i4.5
    IL_0046:  ldarg.2
    IL_0047:  box        [mscorlib]System.Int32
    IL_004c:  stelem.ref
    IL_004d:  ldloc.1
    IL_004e:  ldc.i4.6
    IL_004f:  ldstr      " but the value given was "
    IL_0054:  stelem.ref
    IL_0055:  ldloc.1
    IL_0056:  ldc.i4.7
    IL_0057:  ldarg.0
    IL_0058:  box        [mscorlib]System.UInt32
    IL_005d:  stelem.ref
    IL_005e:  ldloc.1
    IL_005f:  call       string [mscorlib]System.String::Concat(object[])
    IL_0064:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string)
    IL_0069:  throw

    IL_006a:  ret
  } // end of method Usc::requireArgumentRange

  .method public hidebysig instance void 
          restoreDefaultConfiguration() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4     0xff
    IL_0008:  ldc.i4.1
    IL_0009:  call       instance void Pololu.Usc.Usc::setRawParameterNoChecks(uint16,
                                                                               uint16,
                                                                               int32)
    IL_000e:  nop
    IL_000f:  ldarg.0
    IL_0010:  ldc.i4     0x5dc
    IL_0015:  call       instance void Pololu.Usc.Usc::reinitialize(int32)
    IL_001a:  nop
    IL_001b:  ret
  } // end of method Usc::restoreDefaultConfiguration

  .method public hidebysig instance void 
          fixSettings(class Pololu.Usc.UscSettings settings,
                      class [mscorlib]System.Collections.Generic.List`1<string> warnings) cil managed
  {
    // Code size       724 (0x2d4)
    .maxstack  4
    .locals init ([0] class Pololu.Usc.ChannelSetting cs,
             [1] bool servoIgnoreWarningShown,
             [2] class Pololu.Usc.ChannelSetting setting,
             [3] bool CS$4$0000,
             [4] object[] CS$0$0001,
             [5] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.ChannelSetting> CS$5$0002,
             [6] valuetype Pololu.Usc.ChannelMode CS$4$0003)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_0007:  ldarg.0
    IL_0008:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_000d:  cgt
    IL_000f:  ldc.i4.0
    IL_0010:  ceq
    IL_0012:  stloc.3
    IL_0013:  ldloc.3
    IL_0014:  brtrue.s   IL_0086

    IL_0016:  nop
    IL_0017:  ldarg.2
    IL_0018:  ldc.i4.5
    IL_0019:  newarr     [mscorlib]System.Object
    IL_001e:  stloc.s    CS$0$0001
    IL_0020:  ldloc.s    CS$0$0001
    IL_0022:  ldc.i4.0
    IL_0023:  ldstr      "The settings loaded include settings for "
    IL_0028:  stelem.ref
    IL_0029:  ldloc.s    CS$0$0001
    IL_002b:  ldc.i4.1
    IL_002c:  ldarg.1
    IL_002d:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_0032:  box        [mscorlib]System.Byte
    IL_0037:  stelem.ref
    IL_0038:  ldloc.s    CS$0$0001
    IL_003a:  ldc.i4.2
    IL_003b:  ldstr      " channels, but this device has only "
    IL_0040:  stelem.ref
    IL_0041:  ldloc.s    CS$0$0001
    IL_0043:  ldc.i4.3
    IL_0044:  ldarg.0
    IL_0045:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_004a:  box        [mscorlib]System.Byte
    IL_004f:  stelem.ref
    IL_0050:  ldloc.s    CS$0$0001
    IL_0052:  ldc.i4.4
    IL_0053:  ldstr      " channels.  The extra channel settings will be ign"
    + "ored."
    IL_0058:  stelem.ref
    IL_0059:  ldloc.s    CS$0$0001
    IL_005b:  call       string [mscorlib]System.String::Concat(object[])
    IL_0060:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_0065:  nop
    IL_0066:  ldarg.1
    IL_0067:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting> Pololu.Usc.UscSettings::channelSettings
    IL_006c:  ldarg.0
    IL_006d:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_0072:  ldarg.1
    IL_0073:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_0078:  ldarg.0
    IL_0079:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_007e:  sub
    IL_007f:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting>::RemoveRange(int32,
                                                                                                                                       int32)
    IL_0084:  nop
    IL_0085:  nop
    IL_0086:  ldarg.1
    IL_0087:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_008c:  ldarg.0
    IL_008d:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_0092:  clt
    IL_0094:  ldc.i4.0
    IL_0095:  ceq
    IL_0097:  stloc.3
    IL_0098:  ldloc.3
    IL_0099:  brtrue     IL_013e

    IL_009e:  nop
    IL_009f:  ldarg.2
    IL_00a0:  ldc.i4.5
    IL_00a1:  newarr     [mscorlib]System.Object
    IL_00a6:  stloc.s    CS$0$0001
    IL_00a8:  ldloc.s    CS$0$0001
    IL_00aa:  ldc.i4.0
    IL_00ab:  ldstr      "The settings loaded include settings for only "
    IL_00b0:  stelem.ref
    IL_00b1:  ldloc.s    CS$0$0001
    IL_00b3:  ldc.i4.1
    IL_00b4:  ldarg.1
    IL_00b5:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_00ba:  box        [mscorlib]System.Byte
    IL_00bf:  stelem.ref
    IL_00c0:  ldloc.s    CS$0$0001
    IL_00c2:  ldc.i4.2
    IL_00c3:  ldstr      " channels, but this device has "
    IL_00c8:  stelem.ref
    IL_00c9:  ldloc.s    CS$0$0001
    IL_00cb:  ldc.i4.3
    IL_00cc:  ldarg.0
    IL_00cd:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_00d2:  box        [mscorlib]System.Byte
    IL_00d7:  stelem.ref
    IL_00d8:  ldloc.s    CS$0$0001
    IL_00da:  ldc.i4.4
    IL_00db:  ldstr      " channels.  The other channels will be initialized"
    + " with default settings."
    IL_00e0:  stelem.ref
    IL_00e1:  ldloc.s    CS$0$0001
    IL_00e3:  call       string [mscorlib]System.String::Concat(object[])
    IL_00e8:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_00ed:  nop
    IL_00ee:  br.s       IL_012b

    IL_00f0:  nop
    IL_00f1:  newobj     instance void Pololu.Usc.ChannelSetting::.ctor()
    IL_00f6:  stloc.0
    IL_00f7:  ldarg.0
    IL_00f8:  call       instance bool Pololu.Usc.Usc::get_microMaestro()
    IL_00fd:  brfalse.s  IL_010f

    IL_00ff:  ldarg.1
    IL_0100:  ldfld      uint8 Pololu.Usc.UscSettings::servosAvailable
    IL_0105:  ldarg.1
    IL_0106:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_010b:  cgt
    IL_010d:  br.s       IL_0110

    IL_010f:  ldc.i4.1
    IL_0110:  stloc.3
    IL_0111:  ldloc.3
    IL_0112:  brtrue.s   IL_011d

    IL_0114:  nop
    IL_0115:  ldloc.0
    IL_0116:  ldc.i4.3
    IL_0117:  stfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
    IL_011c:  nop
    IL_011d:  ldarg.1
    IL_011e:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting> Pololu.Usc.UscSettings::channelSettings
    IL_0123:  ldloc.0
    IL_0124:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting>::Add(!0)
    IL_0129:  nop
    IL_012a:  nop
    IL_012b:  ldarg.1
    IL_012c:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_0131:  ldarg.0
    IL_0132:  ldfld      uint8 Pololu.Usc.Usc::servoCount
    IL_0137:  clt
    IL_0139:  stloc.3
    IL_013a:  ldloc.3
    IL_013b:  brtrue.s   IL_00f0

    IL_013d:  nop
    IL_013e:  ldarg.1
    IL_013f:  callvirt   instance uint8 Pololu.Usc.UscSettings::get_servoCount()
    IL_0144:  ldc.i4.6
    IL_0145:  bne.un.s   IL_015e

    IL_0147:  ldarg.0
    IL_0148:  call       instance uint16 Pololu.Usc.Usc::get_firmwareVersionMajor()
    IL_014d:  ldc.i4.1
    IL_014e:  bgt.s      IL_015e

    IL_0150:  ldarg.0
    IL_0151:  call       instance uint8 Pololu.Usc.Usc::get_firmwareVersionMinor()
    IL_0156:  ldc.i4.0
    IL_0157:  ceq
    IL_0159:  ldc.i4.0
    IL_015a:  ceq
    IL_015c:  br.s       IL_015f

    IL_015e:  ldc.i4.1
    IL_015f:  stloc.3
    IL_0160:  ldloc.3
    IL_0161:  brtrue.s   IL_01d7

    IL_0163:  nop
    IL_0164:  ldc.i4.0
    IL_0165:  stloc.1
    IL_0166:  nop
    IL_0167:  ldarg.1
    IL_0168:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting> Pololu.Usc.UscSettings::channelSettings
    IL_016d:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting>::GetEnumerator()
    IL_0172:  stloc.s    CS$5$0002
    .try
    {
      IL_0174:  br.s       IL_01b9

      IL_0176:  ldloca.s   CS$5$0002
      IL_0178:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.ChannelSetting>::get_Current()
      IL_017d:  stloc.2
      IL_017e:  nop
      IL_017f:  ldloc.2
      IL_0180:  ldfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
      IL_0185:  brtrue.s   IL_0195

      IL_0187:  ldloc.2
      IL_0188:  ldfld      valuetype Pololu.Usc.HomeMode Pololu.Usc.ChannelSetting::homeMode
      IL_018d:  ldc.i4.1
      IL_018e:  ceq
      IL_0190:  ldc.i4.0
      IL_0191:  ceq
      IL_0193:  br.s       IL_0196

      IL_0195:  ldc.i4.1
      IL_0196:  stloc.3
      IL_0197:  ldloc.3
      IL_0198:  brtrue.s   IL_01b8

      IL_019a:  nop
      IL_019b:  ldloc.2
      IL_019c:  ldc.i4.0
      IL_019d:  stfld      valuetype Pololu.Usc.HomeMode Pololu.Usc.ChannelSetting::homeMode
      IL_01a2:  ldloc.1
      IL_01a3:  stloc.3
      IL_01a4:  ldloc.3
      IL_01a5:  brtrue.s   IL_01b7

      IL_01a7:  nop
      IL_01a8:  ldarg.2
      IL_01a9:  ldstr      "Ignore mode does not work for servo channels on th"
      + "e Micro Maestro 6-Channel Servo Controller firmware versions prior to 1"
      + ".01.\nYour channels will be changed to Off mode.\nVisit Pololu.com for a "
      + "firmware upgrade."
      IL_01ae:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
      IL_01b3:  nop
      IL_01b4:  ldc.i4.1
      IL_01b5:  stloc.1
      IL_01b6:  nop
      IL_01b7:  nop
      IL_01b8:  nop
      IL_01b9:  ldloca.s   CS$5$0002
      IL_01bb:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.ChannelSetting>::MoveNext()
      IL_01c0:  stloc.3
      IL_01c1:  ldloc.3
      IL_01c2:  brtrue.s   IL_0176

      IL_01c4:  leave.s    IL_01d5

    }  // end .try
    finally
    {
      IL_01c6:  ldloca.s   CS$5$0002
      IL_01c8:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.ChannelSetting>
      IL_01ce:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_01d3:  nop
      IL_01d4:  endfinally
    }  // end handler
    IL_01d5:  nop
    IL_01d6:  nop
    IL_01d7:  nop
    IL_01d8:  ldarg.1
    IL_01d9:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting> Pololu.Usc.UscSettings::channelSettings
    IL_01de:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.ChannelSetting>::GetEnumerator()
    IL_01e3:  stloc.s    CS$5$0002
    .try
    {
      IL_01e5:  br         IL_028c

      IL_01ea:  ldloca.s   CS$5$0002
      IL_01ec:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.ChannelSetting>::get_Current()
      IL_01f1:  stloc.0
      IL_01f2:  nop
      IL_01f3:  ldloc.0
      IL_01f4:  ldfld      valuetype Pololu.Usc.ChannelMode Pololu.Usc.ChannelSetting::mode
      IL_01f9:  stloc.s    CS$4$0003
      IL_01fb:  ldloc.s    CS$4$0003
      IL_01fd:  ldc.i4.2
      IL_01fe:  sub
      IL_01ff:  switch     ( 
                            IL_024e,
                            IL_020e)
      IL_020c:  br.s       IL_028b

      IL_020e:  nop
      IL_020f:  ldloc.0
      IL_0210:  ldc.i4.1
      IL_0211:  stfld      valuetype Pololu.Usc.HomeMode Pololu.Usc.ChannelSetting::homeMode
      IL_0216:  ldloc.0
      IL_0217:  ldc.i4.0
      IL_0218:  stfld      uint16 Pololu.Usc.ChannelSetting::minimum
      IL_021d:  ldloc.0
      IL_021e:  ldc.i4     0x400
      IL_0223:  stfld      uint16 Pololu.Usc.ChannelSetting::maximum
      IL_0228:  ldloc.0
      IL_0229:  ldc.i4.0
      IL_022a:  stfld      uint16 Pololu.Usc.ChannelSetting::speed
      IL_022f:  ldloc.0
      IL_0230:  ldc.i4.0
      IL_0231:  stfld      uint8 Pololu.Usc.ChannelSetting::acceleration
      IL_0236:  ldloc.0
      IL_0237:  ldc.i4     0x400
      IL_023c:  stfld      uint16 Pololu.Usc.ChannelSetting::neutral
      IL_0241:  ldloc.0
      IL_0242:  ldc.i4     0x771
      IL_0247:  stfld      uint16 Pololu.Usc.ChannelSetting::range
      IL_024c:  br.s       IL_028b

      IL_024e:  nop
      IL_024f:  ldloc.0
      IL_0250:  ldc.i4     0xf92
      IL_0255:  stfld      uint16 Pololu.Usc.ChannelSetting::minimum
      IL_025a:  ldloc.0
      IL_025b:  ldc.i4     0x1f40
      IL_0260:  stfld      uint16 Pololu.Usc.ChannelSetting::maximum
      IL_0265:  ldloc.0
      IL_0266:  ldc.i4.0
      IL_0267:  stfld      uint16 Pololu.Usc.ChannelSetting::speed
      IL_026c:  ldloc.0
      IL_026d:  ldc.i4.0
      IL_026e:  stfld      uint8 Pololu.Usc.ChannelSetting::acceleration
      IL_0273:  ldloc.0
      IL_0274:  ldc.i4     0x1770
      IL_0279:  stfld      uint16 Pololu.Usc.ChannelSetting::neutral
      IL_027e:  ldloc.0
      IL_027f:  ldc.i4     0x771
      IL_0284:  stfld      uint16 Pololu.Usc.ChannelSetting::range
      IL_0289:  br.s       IL_028b

      IL_028b:  nop
      IL_028c:  ldloca.s   CS$5$0002
      IL_028e:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.ChannelSetting>::MoveNext()
      IL_0293:  stloc.3
      IL_0294:  ldloc.3
      IL_0295:  brtrue     IL_01ea

      IL_029a:  leave.s    IL_02ab

    }  // end .try
    finally
    {
      IL_029c:  ldloca.s   CS$5$0002
      IL_029e:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.ChannelSetting>
      IL_02a4:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_02a9:  nop
      IL_02aa:  endfinally
    }  // end handler
    IL_02ab:  nop
    IL_02ac:  ldarg.1
    IL_02ad:  ldfld      uint8 Pololu.Usc.UscSettings::serialDeviceNumber
    IL_02b2:  ldc.i4     0x80
    IL_02b7:  clt
    IL_02b9:  stloc.3
    IL_02ba:  ldloc.3
    IL_02bb:  brtrue.s   IL_02d3

    IL_02bd:  nop
    IL_02be:  ldarg.1
    IL_02bf:  ldc.i4.s   12
    IL_02c1:  stfld      uint8 Pololu.Usc.UscSettings::serialDeviceNumber
    IL_02c6:  ldarg.2
    IL_02c7:  ldstr      "The serial device number must be less than 128.  I"
    + "t will be changed to 12."
    IL_02cc:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_02d1:  nop
    IL_02d2:  nop
    IL_02d3:  ret
  } // end of method Usc::fixSettings

  .method family hidebysig static valuetype Pololu.Usc.Usc/Range 
          getRange(valuetype Pololu.Usc.uscParameter parameterId) cil managed
  {
    // Code size       455 (0x1c7)
    .maxstack  4
    .locals init ([0] valuetype Pololu.Usc.Usc/Range CS$1$0000,
             [1] bool CS$4$0001,
             [2] valuetype Pololu.Usc.uscParameter CS$4$0002,
             [3] int32 CS$4$0003)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ceq
    IL_0005:  ldc.i4.0
    IL_0006:  ceq
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u8
    IL_0011:  stloc.0
    IL_0012:  br         IL_01c5

    IL_0017:  ldarg.0
    IL_0018:  stloc.2
    IL_0019:  ldloc.2
    IL_001a:  ldc.i4.1
    IL_001b:  sub
    IL_001c:  switch     ( 
                          IL_008e,
                          IL_0099,
                          IL_00d0,
                          IL_0118,
                          IL_0148,
                          IL_00f7,
                          IL_0148,
                          IL_0102,
                          IL_00ec,
                          IL_010d,
                          IL_00de,
                          IL_00c5,
                          IL_00c5,
                          IL_00c5,
                          IL_00c5,
                          IL_00c5,
                          IL_00c5,
                          IL_00a4,
                          IL_00af,
                          IL_0148,
                          IL_00c5,
                          IL_0135,
                          IL_0148,
                          IL_0140,
                          IL_0123,
                          IL_00ba)
    IL_0089:  br         IL_0148

    IL_008e:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u8
    IL_0093:  stloc.0
    IL_0094:  br         IL_01c5

    IL_0099:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u8
    IL_009e:  stloc.0
    IL_009f:  br         IL_01c5

    IL_00a4:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u8
    IL_00a9:  stloc.0
    IL_00aa:  br         IL_01c5

    IL_00af:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u16
    IL_00b4:  stloc.0
    IL_00b5:  br         IL_01c5

    IL_00ba:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u8
    IL_00bf:  stloc.0
    IL_00c0:  br         IL_01c5

    IL_00c5:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u8
    IL_00ca:  stloc.0
    IL_00cb:  br         IL_01c5

    IL_00d0:  ldc.i4.1
    IL_00d1:  ldc.i4.0
    IL_00d2:  ldc.i4.3
    IL_00d3:  newobj     instance void Pololu.Usc.Usc/Range::.ctor(uint8,
                                                                   int32,
                                                                   int32)
    IL_00d8:  stloc.0
    IL_00d9:  br         IL_01c5

    IL_00de:  ldc.i4.1
    IL_00df:  ldc.i4.0
    IL_00e0:  ldc.i4.1
    IL_00e1:  newobj     instance void Pololu.Usc.Usc/Range::.ctor(uint8,
                                                                   int32,
                                                                   int32)
    IL_00e6:  stloc.0
    IL_00e7:  br         IL_01c5

    IL_00ec:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::boolean
    IL_00f1:  stloc.0
    IL_00f2:  br         IL_01c5

    IL_00f7:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u16
    IL_00fc:  stloc.0
    IL_00fd:  br         IL_01c5

    IL_0102:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::boolean
    IL_0107:  stloc.0
    IL_0108:  br         IL_01c5

    IL_010d:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u7
    IL_0112:  stloc.0
    IL_0113:  br         IL_01c5

    IL_0118:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u16
    IL_011d:  stloc.0
    IL_011e:  br         IL_01c5

    IL_0123:  ldc.i4.1
    IL_0124:  ldc.i4.0
    IL_0125:  ldc.i4     0xfe
    IL_012a:  newobj     instance void Pololu.Usc.Usc/Range::.ctor(uint8,
                                                                   int32,
                                                                   int32)
    IL_012f:  stloc.0
    IL_0130:  br         IL_01c5

    IL_0135:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u16
    IL_013a:  stloc.0
    IL_013b:  br         IL_01c5

    IL_0140:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::boolean
    IL_0145:  stloc.0
    IL_0146:  br.s       IL_01c5

    IL_0148:  ldarg.0
    IL_0149:  ldc.i4.s   30
    IL_014b:  sub
    IL_014c:  ldc.i4.s   9
    IL_014e:  rem
    IL_014f:  ldc.i4.s   30
    IL_0151:  add
    IL_0152:  stloc.3
    IL_0153:  ldloc.3
    IL_0154:  ldc.i4.s   30
    IL_0156:  sub
    IL_0157:  switch     ( 
                          IL_0182,
                          IL_01a5,
                          IL_019d,
                          IL_019d,
                          IL_0182,
                          IL_01a5,
                          IL_0191,
                          IL_019d,
                          IL_019d)
    IL_0180:  br.s       IL_01a5

    IL_0182:  ldc.i4.2
    IL_0183:  ldc.i4.0
    IL_0184:  ldc.i4     0x7eb8
    IL_0189:  newobj     instance void Pololu.Usc.Usc/Range::.ctor(uint8,
                                                                   int32,
                                                                   int32)
    IL_018e:  stloc.0
    IL_018f:  br.s       IL_01c5

    IL_0191:  ldc.i4.1
    IL_0192:  ldc.i4.1
    IL_0193:  ldc.i4.s   50
    IL_0195:  newobj     instance void Pololu.Usc.Usc/Range::.ctor(uint8,
                                                                   int32,
                                                                   int32)
    IL_019a:  stloc.0
    IL_019b:  br.s       IL_01c5

    IL_019d:  ldsfld     valuetype Pololu.Usc.Usc/Range Pololu.Usc.Usc/Range::u8
    IL_01a2:  stloc.0
    IL_01a3:  br.s       IL_01c5

    IL_01a5:  ldstr      "Invalid parameterId "
    IL_01aa:  ldarg.0
    IL_01ab:  box        Pololu.Usc.uscParameter
    IL_01b0:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_01b5:  ldstr      ", can not determine the range of this parameter."
    IL_01ba:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_01bf:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string)
    IL_01c4:  throw

    IL_01c5:  ldloc.0
    IL_01c6:  ret
  } // end of method Usc::getRange

  .method public hidebysig instance void 
          setPWM(uint16 dutyCycle,
                 uint16 period) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.s   64
    IL_0004:  ldc.i4     0x8a
    IL_0009:  ldarg.1
    IL_000a:  ldarg.2
    IL_000b:  call       instance void [UsbWrapper]Pololu.UsbWrapper.UsbDevice::controlTransfer(uint8,
                                                                                                uint8,
                                                                                                uint16,
                                                                                                uint16)
    IL_0010:  nop
    IL_0011:  ret
  } // end of method Usc::setPWM

  .method public hidebysig instance void 
          disablePWM() cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  3
    .locals init ([0] bool CS$4$0000)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance uint16 [UsbWrapper]Pololu.UsbWrapper.UsbDevice::getProductID()
    IL_0007:  ldc.i4     0x8a
    IL_000c:  ceq
    IL_000e:  ldc.i4.0
    IL_000f:  ceq
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  brtrue.s   IL_0020

    IL_0015:  ldarg.0
    IL_0016:  ldc.i4.8
    IL_0017:  ldc.i4.0
    IL_0018:  call       instance void Pololu.Usc.Usc::setTarget(uint8,
                                                                 uint16)
    IL_001d:  nop
    IL_001e:  br.s       IL_002a

    IL_0020:  ldarg.0
    IL_0021:  ldc.i4.s   12
    IL_0023:  ldc.i4.0
    IL_0024:  call       instance void Pololu.Usc.Usc::setTarget(uint8,
                                                                 uint16)
    IL_0029:  nop
    IL_002a:  ret
  } // end of method Usc::disablePWM

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  8
    IL_0000:  ldstr      "e0fbe39f-7670-4db6-9b1a-1dfb141014a7"
    IL_0005:  newobj     instance void [mscorlib]System.Guid::.ctor(string)
    IL_000a:  stsfld     valuetype [mscorlib]System.Guid Pololu.Usc.Usc::deviceInterfaceGuid
    IL_000f:  ldc.i4.4
    IL_0010:  newarr     [mscorlib]System.UInt16
    IL_0015:  dup
    IL_0016:  ldtoken    field int64 '<PrivateImplementationDetails>{3600D2BD-7367-489C-997C-DF4745537D41}'::'$$method0x600004f-1'
    IL_001b:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0020:  stsfld     uint16[] Pololu.Usc.Usc::productIDArray
    IL_0025:  ret
  } // end of method Usc::.cctor

  .property string[] bootloaderDeviceInstanceIdPrefixes()
  {
    .get string[] Pololu.Usc.Usc::get_bootloaderDeviceInstanceIdPrefixes()
  } // end of property Usc::bootloaderDeviceInstanceIdPrefixes
  .property string shortProductName()
  {
    .get string Pololu.Usc.Usc::get_shortProductName()
  } // end of property Usc::shortProductName
  .property instance bool microMaestro()
  {
    .get instance bool Pololu.Usc.Usc::get_microMaestro()
  } // end of property Usc::microMaestro
  .property instance uint8 stackSize()
  {
    .get instance uint8 Pololu.Usc.Usc::get_stackSize()
  } // end of property Usc::stackSize
  .property instance uint8 callStackSize()
  {
    .get instance uint8 Pololu.Usc.Usc::get_callStackSize()
  } // end of property Usc::callStackSize
  .property instance uint16 firmwareVersionMajor()
  {
    .get instance uint16 Pololu.Usc.Usc::get_firmwareVersionMajor()
  } // end of property Usc::firmwareVersionMajor
  .property instance uint8 firmwareVersionMinor()
  {
    .get instance uint8 Pololu.Usc.Usc::get_firmwareVersionMinor()
  } // end of property Usc::firmwareVersionMinor
  .property instance string firmwareVersionString()
  {
    .get instance string Pololu.Usc.Usc::get_firmwareVersionString()
  } // end of property Usc::firmwareVersionString
  .property instance uint32 subroutineOffsetBlocks()
  {
    .get instance uint32 Pololu.Usc.Usc::get_subroutineOffsetBlocks()
  } // end of property Usc::subroutineOffsetBlocks
  .property instance uint16 maxScriptLength()
  {
    .get instance uint16 Pololu.Usc.Usc::get_maxScriptLength()
  } // end of property Usc::maxScriptLength
} // end of class Pololu.Usc.Usc

.class public auto ansi sealed Pololu.Usc.uscRequest
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_GET_PARAMETER = int32(0x00000081)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_SET_PARAMETER = int32(0x00000082)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_GET_VARIABLES = int32(0x00000083)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_SET_SERVO_VARIABLE = int32(0x00000084)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_SET_TARGET = int32(0x00000085)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_CLEAR_ERRORS = int32(0x00000086)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_GET_SERVO_SETTINGS = int32(0x00000087)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_GET_STACK = int32(0x00000088)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_GET_CALL_STACK = int32(0x00000089)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_SET_PWM = int32(0x0000008A)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_REINITIALIZE = int32(0x00000090)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_ERASE_SCRIPT = int32(0x000000A0)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_WRITE_SCRIPT = int32(0x000000A1)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_SET_SCRIPT_DONE = int32(0x000000A2)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_RESTART_SCRIPT_AT_SUBROUTINE = int32(0x000000A3)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_RESTART_SCRIPT_AT_SUBROUTINE_WITH_PARAMETER = int32(0x000000A4)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_RESTART_SCRIPT = int32(0x000000A5)
  .field public static literal valuetype Pololu.Usc.uscRequest REQUEST_START_BOOTLOADER = int32(0x000000FF)
} // end of class Pololu.Usc.uscRequest

.class public sequential ansi sealed beforefieldinit Pololu.Usc.ServoStatus
       extends [mscorlib]System.ValueType
{
  .pack 1
  .size 0
  .field public uint16 position
  .field public uint16 target
  .field public uint16 speed
  .field public uint8 acceleration
} // end of class Pololu.Usc.ServoStatus

.class public sequential ansi sealed beforefieldinit Pololu.Usc.MaestroVariables
       extends [mscorlib]System.ValueType
{
  .field public uint8 stackPointer
  .field public uint8 callStackPointer
  .field public uint16 errors
  .field public uint16 programCounter
  .field public uint8 scriptDone
  .field public uint8 performanceFlags
} // end of class Pololu.Usc.MaestroVariables

.class public sequential ansi sealed beforefieldinit Pololu.Usc.MicroMaestroVariables
       extends [mscorlib]System.ValueType
{
  .pack 1
  .size 0
  .class sequential ansi sealed nested public beforefieldinit '<buffer>e__FixedBuffer0'
         extends [mscorlib]System.ValueType
  {
    .pack 0
    .size 6
    .custom instance void [mscorlib]System.Runtime.CompilerServices.UnsafeValueTypeAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int16 FixedElementField
  } // end of class '<buffer>e__FixedBuffer0'

  .class sequential ansi sealed nested public beforefieldinit '<stack>e__FixedBuffer1'
         extends [mscorlib]System.ValueType
  {
    .pack 0
    .size 64
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.UnsafeValueTypeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int16 FixedElementField
  } // end of class '<stack>e__FixedBuffer1'

  .class sequential ansi sealed nested public beforefieldinit '<callStack>e__FixedBuffer2'
         extends [mscorlib]System.ValueType
  {
    .pack 0
    .size 20
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.UnsafeValueTypeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public uint16 FixedElementField
  } // end of class '<callStack>e__FixedBuffer2'

  .field public uint8 stackPointer
  .field public uint8 callStackPointer
  .field public uint16 errors
  .field public uint16 programCounter
  .field public valuetype Pololu.Usc.MicroMaestroVariables/'<buffer>e__FixedBuffer0' buffer
  .custom instance void [mscorlib]System.Runtime.CompilerServices.FixedBufferAttribute::.ctor(class [mscorlib]System.Type,
                                                                                              int32) = ( 01 00 59 53 79 73 74 65 6D 2E 49 6E 74 31 36 2C   // ..YSystem.Int16,
                                                                                                         20 6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73 69   //  mscorlib, Versi
                                                                                                         6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C 74   // on=4.0.0.0, Cult
                                                                                                         75 72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75 62   // ure=neutral, Pub
                                                                                                         6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61   // licKeyToken=b77a
                                                                                                         35 63 35 36 31 39 33 34 65 30 38 39 03 00 00 00   // 5c561934e089....
                                                                                                         00 00 ) 
  .field public valuetype Pololu.Usc.MicroMaestroVariables/'<stack>e__FixedBuffer1' stack
  .custom instance void [mscorlib]System.Runtime.CompilerServices.FixedBufferAttribute::.ctor(class [mscorlib]System.Type,
                                                                                              int32) = ( 01 00 59 53 79 73 74 65 6D 2E 49 6E 74 31 36 2C   // ..YSystem.Int16,
                                                                                                         20 6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73 69   //  mscorlib, Versi
                                                                                                         6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C 74   // on=4.0.0.0, Cult
                                                                                                         75 72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75 62   // ure=neutral, Pub
                                                                                                         6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61   // licKeyToken=b77a
                                                                                                         35 63 35 36 31 39 33 34 65 30 38 39 20 00 00 00   // 5c561934e089 ...
                                                                                                         00 00 ) 
  .field public valuetype Pololu.Usc.MicroMaestroVariables/'<callStack>e__FixedBuffer2' callStack
  .custom instance void [mscorlib]System.Runtime.CompilerServices.FixedBufferAttribute::.ctor(class [mscorlib]System.Type,
                                                                                              int32) = ( 01 00 5A 53 79 73 74 65 6D 2E 55 49 6E 74 31 36   // ..ZSystem.UInt16
                                                                                                         2C 20 6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73   // , mscorlib, Vers
                                                                                                         69 6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C   // ion=4.0.0.0, Cul
                                                                                                         74 75 72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75   // ture=neutral, Pu
                                                                                                         62 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37   // blicKeyToken=b77
                                                                                                         61 35 63 35 36 31 39 33 34 65 30 38 39 0A 00 00   // a5c561934e089...
                                                                                                         00 00 00 ) 
  .field public uint8 scriptDone
  .field public uint8 buffer2
} // end of class Pololu.Usc.MicroMaestroVariables

.class public sequential ansi sealed beforefieldinit Pololu.Usc.MiniMaestroVariables
       extends [mscorlib]System.ValueType
{
  .pack 1
  .size 0
  .field public uint8 stackPointer
  .field public uint8 callStackPointer
  .field public uint16 errors
  .field public uint16 programCounter
  .field public uint8 scriptDone
  .field public uint8 performanceFlags
} // end of class Pololu.Usc.MiniMaestroVariables

.class public auto ansi sealed Pololu.Usc.uscParameter
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname uint8 value__
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_INITIALIZED = uint8(0x00)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVOS_AVAILABLE = uint8(0x01)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO_PERIOD = uint8(0x02)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERIAL_MODE = uint8(0x03)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERIAL_FIXED_BAUD_RATE = uint8(0x04)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERIAL_TIMEOUT = uint8(0x06)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERIAL_ENABLE_CRC = uint8(0x08)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERIAL_NEVER_SUSPEND = uint8(0x09)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERIAL_DEVICE_NUMBER = uint8(0x0A)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERIAL_BAUD_DETECT_TYPE = uint8(0x0B)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_IO_MASK_C = uint8(0x10)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_OUTPUT_MASK_C = uint8(0x11)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_CHANNEL_MODES_0_3 = uint8(0x0C)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_CHANNEL_MODES_4_7 = uint8(0x0D)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_CHANNEL_MODES_8_11 = uint8(0x0E)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_CHANNEL_MODES_12_15 = uint8(0x0F)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_CHANNEL_MODES_16_19 = uint8(0x10)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_CHANNEL_MODES_20_23 = uint8(0x11)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_MINI_MAESTRO_SERVO_PERIOD_L = uint8(0x12)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_MINI_MAESTRO_SERVO_PERIOD_HU = uint8(0x13)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_ENABLE_PULLUPS = uint8(0x15)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SCRIPT_CRC = uint8(0x16)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SCRIPT_DONE = uint8(0x18)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERIAL_MINI_SSC_OFFSET = uint8(0x19)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO_MULTIPLIER = uint8(0x1A)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO0_HOME = uint8(0x1E)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO0_MIN = uint8(0x20)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO0_MAX = uint8(0x21)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO0_NEUTRAL = uint8(0x22)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO0_RANGE = uint8(0x24)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO0_SPEED = uint8(0x25)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO0_ACCELERATION = uint8(0x26)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO1_HOME = uint8(0x27)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO1_MIN = uint8(0x29)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO1_MAX = uint8(0x2A)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO1_NEUTRAL = uint8(0x2B)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO1_RANGE = uint8(0x2D)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO1_SPEED = uint8(0x2E)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO1_ACCELERATION = uint8(0x2F)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO2_HOME = uint8(0x30)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO2_MIN = uint8(0x32)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO2_MAX = uint8(0x33)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO2_NEUTRAL = uint8(0x34)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO2_RANGE = uint8(0x36)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO2_SPEED = uint8(0x37)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO2_ACCELERATION = uint8(0x38)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO3_HOME = uint8(0x39)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO3_MIN = uint8(0x3B)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO3_MAX = uint8(0x3C)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO3_NEUTRAL = uint8(0x3D)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO3_RANGE = uint8(0x3F)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO3_SPEED = uint8(0x40)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO3_ACCELERATION = uint8(0x41)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO4_HOME = uint8(0x42)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO4_MIN = uint8(0x44)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO4_MAX = uint8(0x45)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO4_NEUTRAL = uint8(0x46)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO4_RANGE = uint8(0x48)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO4_SPEED = uint8(0x49)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO4_ACCELERATION = uint8(0x4A)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO5_HOME = uint8(0x4B)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO5_MIN = uint8(0x4D)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO5_MAX = uint8(0x4E)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO5_NEUTRAL = uint8(0x4F)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO5_RANGE = uint8(0x51)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO5_SPEED = uint8(0x52)
  .field public static literal valuetype Pololu.Usc.uscParameter PARAMETER_SERVO5_ACCELERATION = uint8(0x53)
} // end of class Pololu.Usc.uscParameter

.class public auto ansi sealed Pololu.Usc.uscSerialMode
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname uint8 value__
  .field public static literal valuetype Pololu.Usc.uscSerialMode SERIAL_MODE_USB_DUAL_PORT = uint8(0x00)
  .field public static literal valuetype Pololu.Usc.uscSerialMode SERIAL_MODE_USB_CHAINED = uint8(0x01)
  .field public static literal valuetype Pololu.Usc.uscSerialMode SERIAL_MODE_UART_DETECT_BAUD_RATE = uint8(0x02)
  .field public static literal valuetype Pololu.Usc.uscSerialMode SERIAL_MODE_UART_FIXED_BAUD_RATE = uint8(0x03)
} // end of class Pololu.Usc.uscSerialMode

.class public auto ansi sealed Pololu.Usc.uscError
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname uint8 value__
  .field public static literal valuetype Pololu.Usc.uscError ERROR_SERIAL_SIGNAL = uint8(0x00)
  .field public static literal valuetype Pololu.Usc.uscError ERROR_SERIAL_OVERRUN = uint8(0x01)
  .field public static literal valuetype Pololu.Usc.uscError ERROR_SERIAL_BUFFER_FULL = uint8(0x02)
  .field public static literal valuetype Pololu.Usc.uscError ERROR_SERIAL_CRC = uint8(0x03)
  .field public static literal valuetype Pololu.Usc.uscError ERROR_SERIAL_PROTOCOL = uint8(0x04)
  .field public static literal valuetype Pololu.Usc.uscError ERROR_SERIAL_TIMEOUT = uint8(0x05)
  .field public static literal valuetype Pololu.Usc.uscError ERROR_SCRIPT_STACK = uint8(0x06)
  .field public static literal valuetype Pololu.Usc.uscError ERROR_SCRIPT_CALL_STACK = uint8(0x07)
  .field public static literal valuetype Pololu.Usc.uscError ERROR_SCRIPT_PROGRAM_COUNTER = uint8(0x08)
} // end of class Pololu.Usc.uscError

.class public auto ansi sealed Pololu.Usc.performanceFlag
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname uint8 value__
  .field public static literal valuetype Pololu.Usc.performanceFlag PERROR_ADVANCED_UPDATE = uint8(0x00)
  .field public static literal valuetype Pololu.Usc.performanceFlag PERROR_BASIC_UPDATE = uint8(0x01)
  .field public static literal valuetype Pololu.Usc.performanceFlag PERROR_PERIOD = uint8(0x02)
} // end of class Pololu.Usc.performanceFlag

.class private auto ansi '<PrivateImplementationDetails>{3600D2BD-7367-489C-997C-DF4745537D41}'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field static assembly int64 '$$method0x600004f-1' at I_00005538
} // end of class '<PrivateImplementationDetails>{3600D2BD-7367-489C-997C-DF4745537D41}'


// =============================================================

.data cil I_00005538 = bytearray (
                 89 00 8A 00 8B 00 8C 00) 
// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file Usc.res
