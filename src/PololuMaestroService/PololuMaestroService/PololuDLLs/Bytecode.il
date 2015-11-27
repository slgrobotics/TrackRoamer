
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v2.0.50727
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
.assembly Bytecode
{
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 1D 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. 
                                                                                                  50 6F 6C 6F 6C 75 20 32 30 30 39 2D 32 30 31 30   // Pololu 2009-2010
                                                                                                  00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 08 42 79 74 65 63 6F 64 65 00 00 )          // ...Bytecode..
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 06 50 6F 6C 6F 6C 75 00 00 )                // ...Pololu..
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 08 42 79 74 65 63 6F 64 65 00 00 )          // ...Bytecode..
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 66 38 64 38 38 38 34 36 2D 31 33 38 63   // ..$f8d88846-138c
                                                                                                  2D 34 34 36 65 2D 39 38 66 66 2D 35 39 33 61 36   // -446e-98ff-593a6
                                                                                                  32 32 39 30 66 33 33 00 00 )                      // 2290f33..
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 31 2E 30 2E 30 00 00 )             // ...1.1.0.0..
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .hash algorithm 0x00008004
  .ver 1:1:4576:23127
}
.module Bytecode.dll
// MVID: {5AEA8FB0-1F0C-4576-B4DC-F09310EA45E3}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00270000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit Pololu.Usc.Bytecode.BytecodeInstruction
       extends [mscorlib]System.Object
{
  .field private string privateFilename
  .field private int32 privateLineNumber
  .field private int32 privateColumnNumber
  .field private valuetype Pololu.Usc.Bytecode.Opcode privateOpcode
  .field private class [mscorlib]System.Collections.Generic.List`1<int32> privateLiteralArguments
  .field private bool privateIsLabel
  .field private bool privateIsJumpToLabel
  .field private string privateLabelName
  .field private bool privateIsSubroutine
  .field private bool privateIsCall
  .method public hidebysig specialname instance string 
          get_filename() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string Pololu.Usc.Bytecode.BytecodeInstruction::privateFilename
    IL_0006:  ret
  } // end of method BytecodeInstruction::get_filename

  .method public hidebysig specialname instance int32 
          get_lineNumber() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Pololu.Usc.Bytecode.BytecodeInstruction::privateLineNumber
    IL_0006:  ret
  } // end of method BytecodeInstruction::get_lineNumber

  .method public hidebysig specialname instance int32 
          get_columnNumber() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Pololu.Usc.Bytecode.BytecodeInstruction::privateColumnNumber
    IL_0006:  ret
  } // end of method BytecodeInstruction::get_columnNumber

  .method public hidebysig specialname instance valuetype Pololu.Usc.Bytecode.Opcode 
          get_opcode() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::privateOpcode
    IL_0006:  ret
  } // end of method BytecodeInstruction::get_opcode

  .method public hidebysig specialname instance class [mscorlib]System.Collections.Generic.List`1<int32> 
          get_literalArguments() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::privateLiteralArguments
    IL_0006:  ret
  } // end of method BytecodeInstruction::get_literalArguments

  .method public hidebysig instance void 
          addLiteralArgument(int32 'value',
                             bool isMiniMaestro) cil managed
  {
    // Code size       68 (0x44)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::privateLiteralArguments
    IL_0006:  ldarg.1
    IL_0007:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_000c:  ldarg.2
    IL_000d:  brtrue.s   IL_0029

    IL_000f:  ldarg.0
    IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::privateLiteralArguments
    IL_0015:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_001a:  ldc.i4.s   32
    IL_001c:  ble.s      IL_0029

    IL_001e:  ldstr      "Too many literals (> 32) in a row: this will overf"
    + "low the stack."
    IL_0023:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0028:  throw

    IL_0029:  ldarg.0
    IL_002a:  ldfld      class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::privateLiteralArguments
    IL_002f:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0034:  ldc.i4.s   126
    IL_0036:  ble.s      IL_0043

    IL_0038:  ldstr      "Too many literals (> 126) in a row: this will over"
    + "flow the stack."
    IL_003d:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0042:  throw

    IL_0043:  ret
  } // end of method BytecodeInstruction::addLiteralArgument

  .method public hidebysig instance void 
          setOpcode(uint8 'value') cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
    IL_0006:  brfalse.s  IL_0013

    IL_0008:  ldstr      "The opcode has already been set."
    IL_000d:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0012:  throw

    IL_0013:  ldarg.0
    IL_0014:  ldarg.1
    IL_0015:  stfld      valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::privateOpcode
    IL_001a:  ret
  } // end of method BytecodeInstruction::setOpcode

  .method public hidebysig specialname instance bool 
          get_isLabel() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Pololu.Usc.Bytecode.BytecodeInstruction::privateIsLabel
    IL_0006:  ret
  } // end of method BytecodeInstruction::get_isLabel

  .method public hidebysig specialname instance bool 
          get_isJumpToLabel() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Pololu.Usc.Bytecode.BytecodeInstruction::privateIsJumpToLabel
    IL_0006:  ret
  } // end of method BytecodeInstruction::get_isJumpToLabel

  .method public hidebysig specialname instance string 
          get_labelName() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string Pololu.Usc.Bytecode.BytecodeInstruction::privateLabelName
    IL_0006:  ret
  } // end of method BytecodeInstruction::get_labelName

  .method public hidebysig specialname instance bool 
          get_isSubroutine() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Pololu.Usc.Bytecode.BytecodeInstruction::privateIsSubroutine
    IL_0006:  ret
  } // end of method BytecodeInstruction::get_isSubroutine

  .method public hidebysig specialname instance bool 
          get_isCall() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Pololu.Usc.Bytecode.BytecodeInstruction::privateIsCall
    IL_0006:  ret
  } // end of method BytecodeInstruction::get_isCall

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(valuetype Pololu.Usc.Bytecode.Opcode op,
                               string filename,
                               int32 lineNumber,
                               int32 columnNumber) cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0006:  stfld      class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::privateLiteralArguments
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0011:  ldarg.0
    IL_0012:  ldarg.1
    IL_0013:  stfld      valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::privateOpcode
    IL_0018:  ldarg.0
    IL_0019:  ldarg.2
    IL_001a:  stfld      string Pololu.Usc.Bytecode.BytecodeInstruction::privateFilename
    IL_001f:  ldarg.0
    IL_0020:  ldarg.3
    IL_0021:  stfld      int32 Pololu.Usc.Bytecode.BytecodeInstruction::privateLineNumber
    IL_0026:  ldarg.0
    IL_0027:  ldarg.s    columnNumber
    IL_0029:  stfld      int32 Pololu.Usc.Bytecode.BytecodeInstruction::privateColumnNumber
    IL_002e:  ret
  } // end of method BytecodeInstruction::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(valuetype Pololu.Usc.Bytecode.Opcode op,
                               int32 literalArgument,
                               string filename,
                               int32 lineNumber,
                               int32 columnNumber) cil managed
  {
    // Code size       60 (0x3c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0006:  stfld      class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::privateLiteralArguments
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0011:  ldarg.0
    IL_0012:  ldarg.1
    IL_0013:  stfld      valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::privateOpcode
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::privateLiteralArguments
    IL_001e:  ldarg.2
    IL_001f:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0024:  ldarg.0
    IL_0025:  ldarg.3
    IL_0026:  stfld      string Pololu.Usc.Bytecode.BytecodeInstruction::privateFilename
    IL_002b:  ldarg.0
    IL_002c:  ldarg.s    lineNumber
    IL_002e:  stfld      int32 Pololu.Usc.Bytecode.BytecodeInstruction::privateLineNumber
    IL_0033:  ldarg.0
    IL_0034:  ldarg.s    columnNumber
    IL_0036:  stfld      int32 Pololu.Usc.Bytecode.BytecodeInstruction::privateColumnNumber
    IL_003b:  ret
  } // end of method BytecodeInstruction::.ctor

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.List`1<uint8> 
          toByteList() cil managed
  {
    // Code size       387 (0x183)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<uint8> V_0,
             int32 V_1,
             int32 V_2,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32> V_3,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32> V_4)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldarg.0
    IL_0007:  call       instance bool Pololu.Usc.Bytecode.BytecodeInstruction::get_isLabel()
    IL_000c:  brtrue.s   IL_0016

    IL_000e:  ldarg.0
    IL_000f:  call       instance bool Pololu.Usc.Bytecode.BytecodeInstruction::get_isSubroutine()
    IL_0014:  brfalse.s  IL_0018

    IL_0016:  ldloc.0
    IL_0017:  ret

    IL_0018:  ldloc.0
    IL_0019:  ldarg.0
    IL_001a:  call       instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
    IL_001f:  conv.u1
    IL_0020:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
    IL_0025:  ldarg.0
    IL_0026:  call       instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
    IL_002b:  ldc.i4.1
    IL_002c:  beq.s      IL_004a

    IL_002e:  ldarg.0
    IL_002f:  call       instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
    IL_0034:  ldc.i4.6
    IL_0035:  beq.s      IL_004a

    IL_0037:  ldarg.0
    IL_0038:  call       instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
    IL_003d:  ldc.i4.7
    IL_003e:  beq.s      IL_004a

    IL_0040:  ldarg.0
    IL_0041:  call       instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
    IL_0046:  ldc.i4.s   54
    IL_0048:  bne.un.s   IL_00a3

    IL_004a:  ldarg.0
    IL_004b:  call       instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
    IL_0050:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0055:  brtrue.s   IL_006a

    IL_0057:  ldloc.0
    IL_0058:  ldc.i4.0
    IL_0059:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
    IL_005e:  ldloc.0
    IL_005f:  ldc.i4.0
    IL_0060:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
    IL_0065:  br         IL_0181

    IL_006a:  ldloc.0
    IL_006b:  ldarg.0
    IL_006c:  call       instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
    IL_0071:  ldc.i4.0
    IL_0072:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Item(int32)
    IL_0077:  conv.u2
    IL_0078:  ldc.i4     0x100
    IL_007d:  rem
    IL_007e:  conv.u1
    IL_007f:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
    IL_0084:  ldloc.0
    IL_0085:  ldarg.0
    IL_0086:  call       instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
    IL_008b:  ldc.i4.0
    IL_008c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Item(int32)
    IL_0091:  conv.u2
    IL_0092:  ldc.i4     0x100
    IL_0097:  div
    IL_0098:  conv.u1
    IL_0099:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
    IL_009e:  br         IL_0181

    IL_00a3:  ldarg.0
    IL_00a4:  call       instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
    IL_00a9:  ldc.i4.2
    IL_00aa:  bne.un.s   IL_00c4

    IL_00ac:  ldloc.0
    IL_00ad:  ldarg.0
    IL_00ae:  call       instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
    IL_00b3:  ldc.i4.0
    IL_00b4:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Item(int32)
    IL_00b9:  conv.u1
    IL_00ba:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
    IL_00bf:  br         IL_0181

    IL_00c4:  ldarg.0
    IL_00c5:  call       instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
    IL_00ca:  ldc.i4.3
    IL_00cb:  bne.un.s   IL_012e

    IL_00cd:  ldloc.0
    IL_00ce:  ldarg.0
    IL_00cf:  call       instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
    IL_00d4:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_00d9:  ldc.i4.2
    IL_00da:  mul
    IL_00db:  conv.u1
    IL_00dc:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
    IL_00e1:  ldarg.0
    IL_00e2:  call       instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
    IL_00e7:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::GetEnumerator()
    IL_00ec:  stloc.3
    .try
    {
      IL_00ed:  br.s       IL_0115

      IL_00ef:  ldloca.s   V_3
      IL_00f1:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::get_Current()
      IL_00f6:  stloc.1
      IL_00f7:  ldloc.0
      IL_00f8:  ldloc.1
      IL_00f9:  conv.u2
      IL_00fa:  ldc.i4     0x100
      IL_00ff:  rem
      IL_0100:  conv.u1
      IL_0101:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
      IL_0106:  ldloc.0
      IL_0107:  ldloc.1
      IL_0108:  conv.u2
      IL_0109:  ldc.i4     0x100
      IL_010e:  div
      IL_010f:  conv.u1
      IL_0110:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
      IL_0115:  ldloca.s   V_3
      IL_0117:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::MoveNext()
      IL_011c:  brtrue.s   IL_00ef

      IL_011e:  leave.s    IL_0181

    }  // end .try
    finally
    {
      IL_0120:  ldloca.s   V_3
      IL_0122:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>
      IL_0128:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_012d:  endfinally
    }  // end handler
    IL_012e:  ldarg.0
    IL_012f:  call       instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
    IL_0134:  ldc.i4.4
    IL_0135:  bne.un.s   IL_0181

    IL_0137:  ldloc.0
    IL_0138:  ldarg.0
    IL_0139:  call       instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
    IL_013e:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0143:  conv.u1
    IL_0144:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
    IL_0149:  ldarg.0
    IL_014a:  call       instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
    IL_014f:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::GetEnumerator()
    IL_0154:  stloc.s    V_4
    .try
    {
      IL_0156:  br.s       IL_0168

      IL_0158:  ldloca.s   V_4
      IL_015a:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::get_Current()
      IL_015f:  stloc.2
      IL_0160:  ldloc.0
      IL_0161:  ldloc.2
      IL_0162:  conv.u1
      IL_0163:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
      IL_0168:  ldloca.s   V_4
      IL_016a:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::MoveNext()
      IL_016f:  brtrue.s   IL_0158

      IL_0171:  leave.s    IL_0181

    }  // end .try
    finally
    {
      IL_0173:  ldloca.s   V_4
      IL_0175:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>
      IL_017b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0180:  endfinally
    }  // end handler
    IL_0181:  ldloc.0
    IL_0182:  ret
  } // end of method BytecodeInstruction::toByteList

  .method public hidebysig instance void 
          'error'(string msg) cil managed
  {
    // Code size       84 (0x54)
    .maxstack  3
    .locals init (object[] V_0)
    IL_0000:  ldc.i4.7
    IL_0001:  newarr     [mscorlib]System.Object
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldarg.0
    IL_000a:  call       instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_filename()
    IL_000f:  stelem.ref
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.1
    IL_0012:  ldstr      ":"
    IL_0017:  stelem.ref
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.2
    IL_001a:  ldarg.0
    IL_001b:  call       instance int32 Pololu.Usc.Bytecode.BytecodeInstruction::get_lineNumber()
    IL_0020:  box        [mscorlib]System.Int32
    IL_0025:  stelem.ref
    IL_0026:  ldloc.0
    IL_0027:  ldc.i4.3
    IL_0028:  ldstr      ":"
    IL_002d:  stelem.ref
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.4
    IL_0030:  ldarg.0
    IL_0031:  call       instance int32 Pololu.Usc.Bytecode.BytecodeInstruction::get_columnNumber()
    IL_0036:  box        [mscorlib]System.Int32
    IL_003b:  stelem.ref
    IL_003c:  ldloc.0
    IL_003d:  ldc.i4.5
    IL_003e:  ldstr      ": "
    IL_0043:  stelem.ref
    IL_0044:  ldloc.0
    IL_0045:  ldc.i4.6
    IL_0046:  ldarg.1
    IL_0047:  stelem.ref
    IL_0048:  ldloc.0
    IL_0049:  call       string [mscorlib]System.String::Concat(object[])
    IL_004e:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0053:  throw
  } // end of method BytecodeInstruction::'error'

  .method public hidebysig static class Pololu.Usc.Bytecode.BytecodeInstruction 
          newSubroutine(string name,
                        string filename,
                        int32 column_number,
                        int32 line_number) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  5
    .locals init (class Pololu.Usc.Bytecode.BytecodeInstruction V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.3
    IL_0004:  newobj     instance void Pololu.Usc.Bytecode.BytecodeInstruction::.ctor(valuetype Pololu.Usc.Bytecode.Opcode,
                                                                                      string,
                                                                                      int32,
                                                                                      int32)
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  stfld      bool Pololu.Usc.Bytecode.BytecodeInstruction::privateIsSubroutine
    IL_0011:  ldloc.0
    IL_0012:  ldarg.0
    IL_0013:  stfld      string Pololu.Usc.Bytecode.BytecodeInstruction::privateLabelName
    IL_0018:  ldloc.0
    IL_0019:  ret
  } // end of method BytecodeInstruction::newSubroutine

  .method public hidebysig static class Pololu.Usc.Bytecode.BytecodeInstruction 
          newCall(string name,
                  string filename,
                  int32 column_number,
                  int32 line_number) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  5
    .locals init (class Pololu.Usc.Bytecode.BytecodeInstruction V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.3
    IL_0004:  newobj     instance void Pololu.Usc.Bytecode.BytecodeInstruction::.ctor(valuetype Pololu.Usc.Bytecode.Opcode,
                                                                                      string,
                                                                                      int32,
                                                                                      int32)
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  stfld      bool Pololu.Usc.Bytecode.BytecodeInstruction::privateIsCall
    IL_0011:  ldloc.0
    IL_0012:  ldarg.0
    IL_0013:  stfld      string Pololu.Usc.Bytecode.BytecodeInstruction::privateLabelName
    IL_0018:  ldloc.0
    IL_0019:  ret
  } // end of method BytecodeInstruction::newCall

  .method public hidebysig static class Pololu.Usc.Bytecode.BytecodeInstruction 
          newLabel(string name,
                   string filename,
                   int32 column_number,
                   int32 line_number) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  5
    .locals init (class Pololu.Usc.Bytecode.BytecodeInstruction V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.3
    IL_0004:  newobj     instance void Pololu.Usc.Bytecode.BytecodeInstruction::.ctor(valuetype Pololu.Usc.Bytecode.Opcode,
                                                                                      string,
                                                                                      int32,
                                                                                      int32)
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  stfld      bool Pololu.Usc.Bytecode.BytecodeInstruction::privateIsLabel
    IL_0011:  ldloc.0
    IL_0012:  ldarg.0
    IL_0013:  stfld      string Pololu.Usc.Bytecode.BytecodeInstruction::privateLabelName
    IL_0018:  ldloc.0
    IL_0019:  ret
  } // end of method BytecodeInstruction::newLabel

  .method public hidebysig static class Pololu.Usc.Bytecode.BytecodeInstruction 
          newJumpToLabel(string name,
                         string filename,
                         int32 column_number,
                         int32 line_number) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  5
    .locals init (class Pololu.Usc.Bytecode.BytecodeInstruction V_0)
    IL_0000:  ldc.i4.6
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.3
    IL_0004:  newobj     instance void Pololu.Usc.Bytecode.BytecodeInstruction::.ctor(valuetype Pololu.Usc.Bytecode.Opcode,
                                                                                      string,
                                                                                      int32,
                                                                                      int32)
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  stfld      bool Pololu.Usc.Bytecode.BytecodeInstruction::privateIsJumpToLabel
    IL_0011:  ldloc.0
    IL_0012:  ldarg.0
    IL_0013:  stfld      string Pololu.Usc.Bytecode.BytecodeInstruction::privateLabelName
    IL_0018:  ldloc.0
    IL_0019:  ret
  } // end of method BytecodeInstruction::newJumpToLabel

  .method public hidebysig static class Pololu.Usc.Bytecode.BytecodeInstruction 
          newConditionalJumpToLabel(string name,
                                    string filename,
                                    int32 column_number,
                                    int32 line_number) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  5
    .locals init (class Pololu.Usc.Bytecode.BytecodeInstruction V_0)
    IL_0000:  ldc.i4.7
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.3
    IL_0004:  newobj     instance void Pololu.Usc.Bytecode.BytecodeInstruction::.ctor(valuetype Pololu.Usc.Bytecode.Opcode,
                                                                                      string,
                                                                                      int32,
                                                                                      int32)
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  stfld      bool Pololu.Usc.Bytecode.BytecodeInstruction::privateIsJumpToLabel
    IL_0011:  ldloc.0
    IL_0012:  ldarg.0
    IL_0013:  stfld      string Pololu.Usc.Bytecode.BytecodeInstruction::privateLabelName
    IL_0018:  ldloc.0
    IL_0019:  ret
  } // end of method BytecodeInstruction::newConditionalJumpToLabel

  .method public hidebysig instance void 
          completeLiterals() cil managed
  {
    // Code size       153 (0x99)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32> V_2)
    IL_0000:  ldarg.0
    IL_0001:  call       instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
    IL_0006:  ldc.i4.1
    IL_0007:  beq.s      IL_000a

    IL_0009:  ret

    IL_000a:  ldc.i4.0
    IL_000b:  stloc.0
    IL_000c:  ldarg.0
    IL_000d:  call       instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
    IL_0012:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::GetEnumerator()
    IL_0017:  stloc.2
    .try
    {
      IL_0018:  br.s       IL_0030

      IL_001a:  ldloca.s   V_2
      IL_001c:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::get_Current()
      IL_0021:  stloc.1
      IL_0022:  ldloc.1
      IL_0023:  ldc.i4     0xff
      IL_0028:  bgt.s      IL_002e

      IL_002a:  ldloc.1
      IL_002b:  ldc.i4.0
      IL_002c:  bge.s      IL_0030

      IL_002e:  ldc.i4.1
      IL_002f:  stloc.0
      IL_0030:  ldloca.s   V_2
      IL_0032:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::MoveNext()
      IL_0037:  brtrue.s   IL_001a

      IL_0039:  leave.s    IL_0049

    }  // end .try
    finally
    {
      IL_003b:  ldloca.s   V_2
      IL_003d:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>
      IL_0043:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0048:  endfinally
    }  // end handler
    IL_0049:  ldloc.0
    IL_004a:  brfalse.s  IL_0062

    IL_004c:  ldarg.0
    IL_004d:  call       instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
    IL_0052:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0057:  ldc.i4.1
    IL_0058:  ble.s      IL_0062

    IL_005a:  ldarg.0
    IL_005b:  ldc.i4.3
    IL_005c:  stfld      valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::privateOpcode
    IL_0061:  ret

    IL_0062:  ldloc.0
    IL_0063:  brfalse.s  IL_007b

    IL_0065:  ldarg.0
    IL_0066:  call       instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
    IL_006b:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0070:  ldc.i4.1
    IL_0071:  bne.un.s   IL_007b

    IL_0073:  ldarg.0
    IL_0074:  ldc.i4.1
    IL_0075:  stfld      valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::privateOpcode
    IL_007a:  ret

    IL_007b:  ldarg.0
    IL_007c:  call       instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
    IL_0081:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0086:  ldc.i4.1
    IL_0087:  ble.s      IL_0091

    IL_0089:  ldarg.0
    IL_008a:  ldc.i4.4
    IL_008b:  stfld      valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::privateOpcode
    IL_0090:  ret

    IL_0091:  ldarg.0
    IL_0092:  ldc.i4.2
    IL_0093:  stfld      valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::privateOpcode
    IL_0098:  ret
  } // end of method BytecodeInstruction::completeLiterals

  .property instance string filename()
  {
    .get instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_filename()
  } // end of property BytecodeInstruction::filename
  .property instance int32 lineNumber()
  {
    .get instance int32 Pololu.Usc.Bytecode.BytecodeInstruction::get_lineNumber()
  } // end of property BytecodeInstruction::lineNumber
  .property instance int32 columnNumber()
  {
    .get instance int32 Pololu.Usc.Bytecode.BytecodeInstruction::get_columnNumber()
  } // end of property BytecodeInstruction::columnNumber
  .property instance valuetype Pololu.Usc.Bytecode.Opcode
          opcode()
  {
    .get instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
  } // end of property BytecodeInstruction::opcode
  .property instance class [mscorlib]System.Collections.Generic.List`1<int32>
          literalArguments()
  {
    .get instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
  } // end of property BytecodeInstruction::literalArguments
  .property instance bool isLabel()
  {
    .get instance bool Pololu.Usc.Bytecode.BytecodeInstruction::get_isLabel()
  } // end of property BytecodeInstruction::isLabel
  .property instance bool isJumpToLabel()
  {
    .get instance bool Pololu.Usc.Bytecode.BytecodeInstruction::get_isJumpToLabel()
  } // end of property BytecodeInstruction::isJumpToLabel
  .property instance string labelName()
  {
    .get instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
  } // end of property BytecodeInstruction::labelName
  .property instance bool isSubroutine()
  {
    .get instance bool Pololu.Usc.Bytecode.BytecodeInstruction::get_isSubroutine()
  } // end of property BytecodeInstruction::isSubroutine
  .property instance bool isCall()
  {
    .get instance bool Pololu.Usc.Bytecode.BytecodeInstruction::get_isCall()
  } // end of property BytecodeInstruction::isCall
} // end of class Pololu.Usc.Bytecode.BytecodeInstruction

.class public auto ansi sealed Pololu.Usc.Bytecode.BlockType
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype Pololu.Usc.Bytecode.BlockType BEGIN = int32(0x00000000)
  .field public static literal valuetype Pololu.Usc.Bytecode.BlockType IF = int32(0x00000001)
  .field public static literal valuetype Pololu.Usc.Bytecode.BlockType ELSE = int32(0x00000002)
} // end of class Pololu.Usc.Bytecode.BlockType

.class public auto ansi beforefieldinit Pololu.Usc.Bytecode.BytecodeProgram
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .field private static literal uint16 CRC16_POLY = uint16(0xA001)
  .field private class [mscorlib]System.Collections.Generic.List`1<string> privateSourceLines
  .field private class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> instructionList
  .field private int32 maxBlock
  .field private class [System]System.Collections.Generic.Stack`1<int32> openBlocks
  .field private class [System]System.Collections.Generic.Stack`1<valuetype Pololu.Usc.Bytecode.BlockType> openBlockTypes
  .field public class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16> subroutineAddresses
  .field public class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8> subroutineCommands
  .method public hidebysig instance string 
          getSourceLine(int32 line) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<string> Pololu.Usc.Bytecode.BytecodeProgram::privateSourceLines
    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.1
    IL_0008:  sub
    IL_0009:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<string>::get_Item(int32)
    IL_000e:  ret
  } // end of method BytecodeProgram::getSourceLine

  .method public hidebysig instance void 
          addSourceLine(string line) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<string> Pololu.Usc.Bytecode.BytecodeProgram::privateSourceLines
    IL_0006:  ldarg.1
    IL_0007:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
    IL_000c:  ret
  } // end of method BytecodeProgram::addSourceLine

  .method public hidebysig instance int32 
          getSourceLineCount() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<string> Pololu.Usc.Bytecode.BytecodeProgram::privateSourceLines
    IL_0006:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<string>::get_Count()
    IL_000b:  ret
  } // end of method BytecodeProgram::getSourceLineCount

  .method public hidebysig specialname instance class Pololu.Usc.Bytecode.BytecodeInstruction 
          get_Item(int32 index) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0006:  ldarg.1
    IL_0007:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Item(int32)
    IL_000c:  ret
  } // end of method BytecodeProgram::get_Item

  .method public hidebysig specialname instance int32 
          get_Count() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0006:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Count()
    IL_000b:  ret
  } // end of method BytecodeProgram::get_Count

  .method assembly hidebysig instance void 
          addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction instruction) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0006:  ldarg.1
    IL_0007:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::Add(!0)
    IL_000c:  ret
  } // end of method BytecodeProgram::addInstruction

  .method assembly hidebysig instance void 
          addLiteral(int32 'literal',
                     string filename,
                     int32 lineNumber,
                     int32 columnNumber,
                     bool isMiniMaestro) cil managed
  {
    // Code size       94 (0x5e)
    .maxstack  6
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0006:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Count()
    IL_000b:  brfalse.s  IL_002d

    IL_000d:  ldarg.0
    IL_000e:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0013:  ldarg.0
    IL_0014:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0019:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Count()
    IL_001e:  ldc.i4.1
    IL_001f:  sub
    IL_0020:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Item(int32)
    IL_0025:  callvirt   instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
    IL_002a:  ldc.i4.1
    IL_002b:  beq.s      IL_003d

    IL_002d:  ldarg.0
    IL_002e:  ldc.i4.1
    IL_002f:  ldarg.2
    IL_0030:  ldarg.3
    IL_0031:  ldarg.s    columnNumber
    IL_0033:  newobj     instance void Pololu.Usc.Bytecode.BytecodeInstruction::.ctor(valuetype Pololu.Usc.Bytecode.Opcode,
                                                                                      string,
                                                                                      int32,
                                                                                      int32)
    IL_0038:  call       instance void Pololu.Usc.Bytecode.BytecodeProgram::addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction)
    IL_003d:  ldarg.0
    IL_003e:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0043:  ldarg.0
    IL_0044:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0049:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Count()
    IL_004e:  ldc.i4.1
    IL_004f:  sub
    IL_0050:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Item(int32)
    IL_0055:  ldarg.1
    IL_0056:  ldarg.s    isMiniMaestro
    IL_0058:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeInstruction::addLiteralArgument(int32,
                                                                                                   bool)
    IL_005d:  ret
  } // end of method BytecodeProgram::addLiteral

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.List`1<uint8> 
          getByteList() cil managed
  {
    // Code size       67 (0x43)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.List`1<uint8> V_0,
             class Pololu.Usc.Bytecode.BytecodeInstruction V_1,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction> V_2)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldarg.0
    IL_0007:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_000c:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::GetEnumerator()
    IL_0011:  stloc.2
    .try
    {
      IL_0012:  br.s       IL_0028

      IL_0014:  ldloca.s   V_2
      IL_0016:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Current()
      IL_001b:  stloc.1
      IL_001c:  ldloc.0
      IL_001d:  ldloc.1
      IL_001e:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<uint8> Pololu.Usc.Bytecode.BytecodeInstruction::toByteList()
      IL_0023:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::AddRange(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0028:  ldloca.s   V_2
      IL_002a:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::MoveNext()
      IL_002f:  brtrue.s   IL_0014

      IL_0031:  leave.s    IL_0041

    }  // end .try
    finally
    {
      IL_0033:  ldloca.s   V_2
      IL_0035:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>
      IL_003b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0040:  endfinally
    }  // end handler
    IL_0041:  ldloc.0
    IL_0042:  ret
  } // end of method BytecodeProgram::getByteList

  .method assembly hidebysig instance void 
          openBlock(valuetype Pololu.Usc.Bytecode.BlockType blocktype,
                    string filename,
                    int32 line_number,
                    int32 column_number) cil managed
  {
    // Code size       80 (0x50)
    .maxstack  5
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "block_start_"
    IL_0006:  ldarg.0
    IL_0007:  ldflda     int32 Pololu.Usc.Bytecode.BytecodeProgram::maxBlock
    IL_000c:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0011:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0016:  ldarg.2
    IL_0017:  ldarg.3
    IL_0018:  ldarg.s    column_number
    IL_001a:  call       class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeInstruction::newLabel(string,
                                                                                                                         string,
                                                                                                                         int32,
                                                                                                                         int32)
    IL_001f:  call       instance void Pololu.Usc.Bytecode.BytecodeProgram::addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction)
    IL_0024:  ldarg.0
    IL_0025:  ldfld      class [System]System.Collections.Generic.Stack`1<int32> Pololu.Usc.Bytecode.BytecodeProgram::openBlocks
    IL_002a:  ldarg.0
    IL_002b:  ldfld      int32 Pololu.Usc.Bytecode.BytecodeProgram::maxBlock
    IL_0030:  callvirt   instance void class [System]System.Collections.Generic.Stack`1<int32>::Push(!0)
    IL_0035:  ldarg.0
    IL_0036:  ldfld      class [System]System.Collections.Generic.Stack`1<valuetype Pololu.Usc.Bytecode.BlockType> Pololu.Usc.Bytecode.BytecodeProgram::openBlockTypes
    IL_003b:  ldarg.1
    IL_003c:  callvirt   instance void class [System]System.Collections.Generic.Stack`1<valuetype Pololu.Usc.Bytecode.BlockType>::Push(!0)
    IL_0041:  ldarg.0
    IL_0042:  dup
    IL_0043:  ldfld      int32 Pololu.Usc.Bytecode.BytecodeProgram::maxBlock
    IL_0048:  ldc.i4.1
    IL_0049:  add
    IL_004a:  stfld      int32 Pololu.Usc.Bytecode.BytecodeProgram::maxBlock
    IL_004f:  ret
  } // end of method BytecodeProgram::openBlock

  .method assembly hidebysig instance valuetype Pololu.Usc.Bytecode.BlockType 
          getCurrentBlockType() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Stack`1<valuetype Pololu.Usc.Bytecode.BlockType> Pololu.Usc.Bytecode.BytecodeProgram::openBlockTypes
    IL_0006:  callvirt   instance !0 class [System]System.Collections.Generic.Stack`1<valuetype Pololu.Usc.Bytecode.BlockType>::Peek()
    IL_000b:  ret
  } // end of method BytecodeProgram::getCurrentBlockType

  .method assembly hidebysig instance string 
          getCurrentBlockStartLabel() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldstr      "block_start_"
    IL_0005:  ldarg.0
    IL_0006:  ldfld      class [System]System.Collections.Generic.Stack`1<int32> Pololu.Usc.Bytecode.BytecodeProgram::openBlocks
    IL_000b:  callvirt   instance !0 class [System]System.Collections.Generic.Stack`1<int32>::Peek()
    IL_0010:  stloc.0
    IL_0011:  ldloca.s   V_0
    IL_0013:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0018:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_001d:  ret
  } // end of method BytecodeProgram::getCurrentBlockStartLabel

  .method assembly hidebysig instance string 
          getCurrentBlockEndLabel() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldstr      "block_end_"
    IL_0005:  ldarg.0
    IL_0006:  ldfld      class [System]System.Collections.Generic.Stack`1<int32> Pololu.Usc.Bytecode.BytecodeProgram::openBlocks
    IL_000b:  callvirt   instance !0 class [System]System.Collections.Generic.Stack`1<int32>::Peek()
    IL_0010:  stloc.0
    IL_0011:  ldloca.s   V_0
    IL_0013:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0018:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_001d:  ret
  } // end of method BytecodeProgram::getCurrentBlockEndLabel

  .method assembly hidebysig instance string 
          getNextBlockEndLabel() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldstr      "block_end_"
    IL_0005:  ldarg.0
    IL_0006:  ldflda     int32 Pololu.Usc.Bytecode.BytecodeProgram::maxBlock
    IL_000b:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0010:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0015:  ret
  } // end of method BytecodeProgram::getNextBlockEndLabel

  .method public hidebysig instance int32 
          findLabelIndex(string name) cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0036

    IL_0004:  ldarg.0
    IL_0005:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_000a:  ldloc.0
    IL_000b:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Item(int32)
    IL_0010:  callvirt   instance bool Pololu.Usc.Bytecode.BytecodeInstruction::get_isLabel()
    IL_0015:  brfalse.s  IL_0032

    IL_0017:  ldarg.0
    IL_0018:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_001d:  ldloc.0
    IL_001e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Item(int32)
    IL_0023:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
    IL_0028:  ldarg.1
    IL_0029:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002e:  brfalse.s  IL_0032

    IL_0030:  ldloc.0
    IL_0031:  ret

    IL_0032:  ldloc.0
    IL_0033:  ldc.i4.1
    IL_0034:  add
    IL_0035:  stloc.0
    IL_0036:  ldloc.0
    IL_0037:  ldarg.0
    IL_0038:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_003d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Count()
    IL_0042:  blt.s      IL_0004

    IL_0044:  ldstr      "Label not found."
    IL_0049:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_004e:  throw
  } // end of method BytecodeProgram::findLabelIndex

  .method public hidebysig instance class Pololu.Usc.Bytecode.BytecodeInstruction 
          findLabelInstruction(string name) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  call       instance int32 Pololu.Usc.Bytecode.BytecodeProgram::findLabelIndex(string)
    IL_000d:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Item(int32)
    IL_0012:  ret
  } // end of method BytecodeProgram::findLabelInstruction

  .method assembly hidebysig instance void 
          closeBlock(string filename,
                     int32 line_number,
                     int32 column_number) cil managed
  {
    // Code size       56 (0x38)
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "block_end_"
    IL_0006:  ldarg.0
    IL_0007:  ldfld      class [System]System.Collections.Generic.Stack`1<int32> Pololu.Usc.Bytecode.BytecodeProgram::openBlocks
    IL_000c:  callvirt   instance !0 class [System]System.Collections.Generic.Stack`1<int32>::Pop()
    IL_0011:  stloc.0
    IL_0012:  ldloca.s   V_0
    IL_0014:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0019:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_001e:  ldarg.1
    IL_001f:  ldarg.2
    IL_0020:  ldarg.3
    IL_0021:  call       class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeInstruction::newLabel(string,
                                                                                                                         string,
                                                                                                                         int32,
                                                                                                                         int32)
    IL_0026:  call       instance void Pololu.Usc.Bytecode.BytecodeProgram::addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction)
    IL_002b:  ldarg.0
    IL_002c:  ldfld      class [System]System.Collections.Generic.Stack`1<valuetype Pololu.Usc.Bytecode.BlockType> Pololu.Usc.Bytecode.BytecodeProgram::openBlockTypes
    IL_0031:  callvirt   instance !0 class [System]System.Collections.Generic.Stack`1<valuetype Pololu.Usc.Bytecode.BlockType>::Pop()
    IL_0036:  pop
    IL_0037:  ret
  } // end of method BytecodeProgram::closeBlock

  .method assembly hidebysig specialname 
          instance bool  get_blockIsOpen() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Stack`1<int32> Pololu.Usc.Bytecode.BytecodeProgram::openBlocks
    IL_0006:  callvirt   instance int32 class [System]System.Collections.Generic.Stack`1<int32>::get_Count()
    IL_000b:  ldc.i4.0
    IL_000c:  cgt
    IL_000e:  ret
  } // end of method BytecodeProgram::get_blockIsOpen

  .method assembly hidebysig instance void 
          completeJumps() cil managed
  {
    // Code size       240 (0xf0)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> V_0,
             int32 V_1,
             class Pololu.Usc.Bytecode.BytecodeInstruction V_2,
             class Pololu.Usc.Bytecode.BytecodeInstruction V_3,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction> V_4,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction> V_5)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.1
    IL_0008:  ldarg.0
    IL_0009:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_000e:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::GetEnumerator()
    IL_0013:  stloc.s    V_4
    .try
    {
      IL_0015:  br.s       IL_006b

      IL_0017:  ldloca.s   V_4
      IL_0019:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Current()
      IL_001e:  stloc.2
      IL_001f:  ldloc.2
      IL_0020:  callvirt   instance bool Pololu.Usc.Bytecode.BytecodeInstruction::get_isLabel()
      IL_0025:  brfalse.s  IL_005d

      IL_0027:  ldloc.0
      IL_0028:  ldloc.2
      IL_0029:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
      IL_002e:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::ContainsKey(!0)
      IL_0033:  brfalse.s  IL_0050

      IL_0035:  ldloc.2
      IL_0036:  ldstr      "The label "
      IL_003b:  ldloc.2
      IL_003c:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
      IL_0041:  ldstr      " has already been used."
      IL_0046:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string)
      IL_004b:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeInstruction::'error'(string)
      IL_0050:  ldloc.0
      IL_0051:  ldloc.2
      IL_0052:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
      IL_0057:  ldloc.1
      IL_0058:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::set_Item(!0,
                                                                                                                         !1)
      IL_005d:  ldloc.1
      IL_005e:  ldloc.2
      IL_005f:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<uint8> Pololu.Usc.Bytecode.BytecodeInstruction::toByteList()
      IL_0064:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
      IL_0069:  add
      IL_006a:  stloc.1
      IL_006b:  ldloca.s   V_4
      IL_006d:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::MoveNext()
      IL_0072:  brtrue.s   IL_0017

      IL_0074:  leave.s    IL_0084

    }  // end .try
    finally
    {
      IL_0076:  ldloca.s   V_4
      IL_0078:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>
      IL_007e:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0083:  endfinally
    }  // end handler
    IL_0084:  ldarg.0
    IL_0085:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_008a:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::GetEnumerator()
    IL_008f:  stloc.s    V_5
    .try
    {
      IL_0091:  br.s       IL_00d6

      IL_0093:  ldloca.s   V_5
      IL_0095:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Current()
      IL_009a:  stloc.3
      .try
      {
        IL_009b:  ldloc.3
        IL_009c:  callvirt   instance bool Pololu.Usc.Bytecode.BytecodeInstruction::get_isJumpToLabel()
        IL_00a1:  brfalse.s  IL_00b6

        IL_00a3:  ldloc.3
        IL_00a4:  ldloc.0
        IL_00a5:  ldloc.3
        IL_00a6:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
        IL_00ab:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::get_Item(!0)
        IL_00b0:  ldc.i4.0
        IL_00b1:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeInstruction::addLiteralArgument(int32,
                                                                                                       bool)
        IL_00b6:  leave.s    IL_00d6

      }  // end .try
      catch [mscorlib]System.Collections.Generic.KeyNotFoundException 
      {
        IL_00b8:  pop
        IL_00b9:  ldloc.3
        IL_00ba:  ldstr      "The label "
        IL_00bf:  ldloc.3
        IL_00c0:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
        IL_00c5:  ldstr      " was not found."
        IL_00ca:  call       string [mscorlib]System.String::Concat(string,
                                                                    string,
                                                                    string)
        IL_00cf:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeInstruction::'error'(string)
        IL_00d4:  leave.s    IL_00d6

      }  // end handler
      IL_00d6:  ldloca.s   V_5
      IL_00d8:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::MoveNext()
      IL_00dd:  brtrue.s   IL_0093

      IL_00df:  leave.s    IL_00ef

    }  // end .try
    finally
    {
      IL_00e1:  ldloca.s   V_5
      IL_00e3:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>
      IL_00e9:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_00ee:  endfinally
    }  // end handler
    IL_00ef:  ret
  } // end of method BytecodeProgram::completeJumps

  .method assembly hidebysig instance void 
          completeCalls(bool isMiniMaestro) cil managed
  {
    // Code size       462 (0x1ce)
    .maxstack  4
    .locals init (uint32 V_0,
             class Pololu.Usc.Bytecode.BytecodeInstruction V_1,
             class Pololu.Usc.Bytecode.BytecodeInstruction V_2,
             int32 V_3,
             class Pololu.Usc.Bytecode.BytecodeInstruction V_4,
             class Pololu.Usc.Bytecode.BytecodeInstruction V_5,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction> V_6,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction> V_7,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction> V_8,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction> V_9)
    IL_0000:  ldc.i4     0x80
    IL_0005:  stloc.0
    IL_0006:  ldarg.0
    IL_0007:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_000c:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::GetEnumerator()
    IL_0011:  stloc.s    V_6
    .try
    {
      IL_0013:  br.s       IL_008c

      IL_0015:  ldloca.s   V_6
      IL_0017:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Current()
      IL_001c:  stloc.1
      IL_001d:  ldloc.1
      IL_001e:  callvirt   instance bool Pololu.Usc.Bytecode.BytecodeInstruction::get_isSubroutine()
      IL_0023:  brfalse.s  IL_008c

      IL_0025:  ldarg.0
      IL_0026:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8> Pololu.Usc.Bytecode.BytecodeProgram::subroutineCommands
      IL_002b:  ldloc.1
      IL_002c:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
      IL_0031:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8>::ContainsKey(!0)
      IL_0036:  brfalse.s  IL_0053

      IL_0038:  ldloc.1
      IL_0039:  ldstr      "The subroutine "
      IL_003e:  ldloc.1
      IL_003f:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
      IL_0044:  ldstr      " has already been defined."
      IL_0049:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string)
      IL_004e:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeInstruction::'error'(string)
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8> Pololu.Usc.Bytecode.BytecodeProgram::subroutineCommands
      IL_0059:  ldloc.1
      IL_005a:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
      IL_005f:  ldloc.0
      IL_0060:  ldc.i4     0x100
      IL_0065:  blt.un.s   IL_006b

      IL_0067:  ldc.i4.s   54
      IL_0069:  br.s       IL_006d

      IL_006b:  ldloc.0
      IL_006c:  conv.u1
      IL_006d:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8>::set_Item(!0,
                                                                                                                         !1)
      IL_0072:  ldloc.0
      IL_0073:  ldc.i4.1
      IL_0074:  add
      IL_0075:  stloc.0
      IL_0076:  ldloc.0
      IL_0077:  ldc.i4     0xff
      IL_007c:  ble.un.s   IL_008c

      IL_007e:  ldarg.1
      IL_007f:  brtrue.s   IL_008c

      IL_0081:  ldloc.1
      IL_0082:  ldstr      "Too many subroutines.  The limit for the Micro Mae"
      + "stro is 128."
      IL_0087:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeInstruction::'error'(string)
      IL_008c:  ldloca.s   V_6
      IL_008e:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::MoveNext()
      IL_0093:  brtrue.s   IL_0015

      IL_0095:  leave.s    IL_00a5

    }  // end .try
    finally
    {
      IL_0097:  ldloca.s   V_6
      IL_0099:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>
      IL_009f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_00a4:  endfinally
    }  // end handler
    IL_00a5:  ldarg.0
    IL_00a6:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_00ab:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::GetEnumerator()
    IL_00b0:  stloc.s    V_7
    .try
    {
      IL_00b2:  br.s       IL_00fb

      IL_00b4:  ldloca.s   V_7
      IL_00b6:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Current()
      IL_00bb:  stloc.2
      .try
      {
        IL_00bc:  ldloc.2
        IL_00bd:  callvirt   instance bool Pololu.Usc.Bytecode.BytecodeInstruction::get_isCall()
        IL_00c2:  brfalse.s  IL_00db

        IL_00c4:  ldloc.2
        IL_00c5:  ldarg.0
        IL_00c6:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8> Pololu.Usc.Bytecode.BytecodeProgram::subroutineCommands
        IL_00cb:  ldloc.2
        IL_00cc:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
        IL_00d1:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8>::get_Item(!0)
        IL_00d6:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeInstruction::setOpcode(uint8)
        IL_00db:  leave.s    IL_00fb

      }  // end .try
      catch [mscorlib]System.Collections.Generic.KeyNotFoundException 
      {
        IL_00dd:  pop
        IL_00de:  ldloc.2
        IL_00df:  ldstr      "Did not understand '"
        IL_00e4:  ldloc.2
        IL_00e5:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
        IL_00ea:  ldstr      "'"
        IL_00ef:  call       string [mscorlib]System.String::Concat(string,
                                                                    string,
                                                                    string)
        IL_00f4:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeInstruction::'error'(string)
        IL_00f9:  leave.s    IL_00fb

      }  // end handler
      IL_00fb:  ldloca.s   V_7
      IL_00fd:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::MoveNext()
      IL_0102:  brtrue.s   IL_00b4

      IL_0104:  leave.s    IL_0114

    }  // end .try
    finally
    {
      IL_0106:  ldloca.s   V_7
      IL_0108:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>
      IL_010e:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0113:  endfinally
    }  // end handler
    IL_0114:  ldc.i4.0
    IL_0115:  stloc.3
    IL_0116:  ldarg.0
    IL_0117:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_011c:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::GetEnumerator()
    IL_0121:  stloc.s    V_8
    .try
    {
      IL_0123:  br.s       IL_015a

      IL_0125:  ldloca.s   V_8
      IL_0127:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Current()
      IL_012c:  stloc.s    V_4
      IL_012e:  ldloc.s    V_4
      IL_0130:  callvirt   instance bool Pololu.Usc.Bytecode.BytecodeInstruction::get_isSubroutine()
      IL_0135:  brfalse.s  IL_014b

      IL_0137:  ldarg.0
      IL_0138:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16> Pololu.Usc.Bytecode.BytecodeProgram::subroutineAddresses
      IL_013d:  ldloc.s    V_4
      IL_013f:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
      IL_0144:  ldloc.3
      IL_0145:  conv.u2
      IL_0146:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16>::set_Item(!0,
                                                                                                                          !1)
      IL_014b:  ldloc.3
      IL_014c:  ldloc.s    V_4
      IL_014e:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<uint8> Pololu.Usc.Bytecode.BytecodeInstruction::toByteList()
      IL_0153:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
      IL_0158:  add
      IL_0159:  stloc.3
      IL_015a:  ldloca.s   V_8
      IL_015c:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::MoveNext()
      IL_0161:  brtrue.s   IL_0125

      IL_0163:  leave.s    IL_0173

    }  // end .try
    finally
    {
      IL_0165:  ldloca.s   V_8
      IL_0167:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>
      IL_016d:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0172:  endfinally
    }  // end handler
    IL_0173:  ldarg.0
    IL_0174:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0179:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::GetEnumerator()
    IL_017e:  stloc.s    V_9
    .try
    {
      IL_0180:  br.s       IL_01b4

      IL_0182:  ldloca.s   V_9
      IL_0184:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Current()
      IL_0189:  stloc.s    V_5
      IL_018b:  ldloc.s    V_5
      IL_018d:  callvirt   instance valuetype Pololu.Usc.Bytecode.Opcode Pololu.Usc.Bytecode.BytecodeInstruction::get_opcode()
      IL_0192:  ldc.i4.s   54
      IL_0194:  bne.un.s   IL_01b4

      IL_0196:  ldloc.s    V_5
      IL_0198:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<int32> Pololu.Usc.Bytecode.BytecodeInstruction::get_literalArguments()
      IL_019d:  ldarg.0
      IL_019e:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16> Pololu.Usc.Bytecode.BytecodeProgram::subroutineAddresses
      IL_01a3:  ldloc.s    V_5
      IL_01a5:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeInstruction::get_labelName()
      IL_01aa:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16>::get_Item(!0)
      IL_01af:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
      IL_01b4:  ldloca.s   V_9
      IL_01b6:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::MoveNext()
      IL_01bb:  brtrue.s   IL_0182

      IL_01bd:  leave.s    IL_01cd

    }  // end .try
    finally
    {
      IL_01bf:  ldloca.s   V_9
      IL_01c1:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>
      IL_01c7:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_01cc:  endfinally
    }  // end handler
    IL_01cd:  ret
  } // end of method BytecodeProgram::completeCalls

  .method assembly hidebysig instance void 
          completeLiterals() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  1
    .locals init (class Pololu.Usc.Bytecode.BytecodeInstruction V_0,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction> V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0006:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::GetEnumerator()
    IL_000b:  stloc.1
    .try
    {
      IL_000c:  br.s       IL_001c

      IL_000e:  ldloca.s   V_1
      IL_0010:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Current()
      IL_0015:  stloc.0
      IL_0016:  ldloc.0
      IL_0017:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeInstruction::completeLiterals()
      IL_001c:  ldloca.s   V_1
      IL_001e:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::MoveNext()
      IL_0023:  brtrue.s   IL_000e

      IL_0025:  leave.s    IL_0035

    }  // end .try
    finally
    {
      IL_0027:  ldloca.s   V_1
      IL_0029:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>
      IL_002f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0034:  endfinally
    }  // end handler
    IL_0035:  ret
  } // end of method BytecodeProgram::completeLiterals

  .method public hidebysig instance class Pololu.Usc.Bytecode.BytecodeInstruction 
          getInstructionAt(uint16 program_counter) cil managed
  {
    // Code size       86 (0x56)
    .maxstack  2
    .locals init (int32 V_0,
             class Pololu.Usc.Bytecode.BytecodeInstruction V_1,
             class [mscorlib]System.Collections.Generic.List`1<uint8> V_2,
             class Pololu.Usc.Bytecode.BytecodeInstruction V_3,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction> V_4)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0008:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::GetEnumerator()
    IL_000d:  stloc.s    V_4
    .try
    {
      IL_000f:  br.s       IL_0039

      IL_0011:  ldloca.s   V_4
      IL_0013:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::get_Current()
      IL_0018:  stloc.1
      IL_0019:  ldloc.1
      IL_001a:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<uint8> Pololu.Usc.Bytecode.BytecodeInstruction::toByteList()
      IL_001f:  stloc.2
      IL_0020:  ldloc.0
      IL_0021:  ldarg.1
      IL_0022:  blt.s      IL_0030

      IL_0024:  ldloc.2
      IL_0025:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
      IL_002a:  brfalse.s  IL_0030

      IL_002c:  ldloc.1
      IL_002d:  stloc.3
      IL_002e:  leave.s    IL_0054

      IL_0030:  ldloc.0
      IL_0031:  ldloc.2
      IL_0032:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
      IL_0037:  add
      IL_0038:  stloc.0
      IL_0039:  ldloca.s   V_4
      IL_003b:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>::MoveNext()
      IL_0040:  brtrue.s   IL_0011

      IL_0042:  leave.s    IL_0052

    }  // end .try
    finally
    {
      IL_0044:  ldloca.s   V_4
      IL_0046:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Pololu.Usc.Bytecode.BytecodeInstruction>
      IL_004c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0051:  endfinally
    }  // end handler
    IL_0052:  ldnull
    IL_0053:  ret

    IL_0054:  ldloc.3
    IL_0055:  ret
  } // end of method BytecodeProgram::getInstructionAt

  .method public hidebysig instance uint16 
          getCRC() cil managed
  {
    // Code size       189 (0xbd)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.List`1<uint8> V_0,
             uint16[] V_1,
             string V_2,
             uint16 V_3,
             valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<string,uint8> V_4,
             uint16[] V_5,
             int32 V_6)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0x80
    IL_000b:  newarr     [mscorlib]System.UInt16
    IL_0010:  stloc.1
    IL_0011:  ldarg.0
    IL_0012:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8> Pololu.Usc.Bytecode.BytecodeProgram::subroutineCommands
    IL_0017:  callvirt   instance class [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection<!0,!1> class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8>::get_Keys()
    IL_001c:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<!0,!1> class [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection<string,uint8>::GetEnumerator()
    IL_0021:  stloc.s    V_4
    .try
    {
      IL_0023:  br.s       IL_005d

      IL_0025:  ldloca.s   V_4
      IL_0027:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<string,uint8>::get_Current()
      IL_002c:  stloc.2
      IL_002d:  ldarg.0
      IL_002e:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8> Pololu.Usc.Bytecode.BytecodeProgram::subroutineCommands
      IL_0033:  ldloc.2
      IL_0034:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8>::get_Item(!0)
      IL_0039:  ldc.i4.s   54
      IL_003b:  beq.s      IL_005d

      IL_003d:  ldloc.1
      IL_003e:  ldarg.0
      IL_003f:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8> Pololu.Usc.Bytecode.BytecodeProgram::subroutineCommands
      IL_0044:  ldloc.2
      IL_0045:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8>::get_Item(!0)
      IL_004a:  ldc.i4     0x80
      IL_004f:  sub
      IL_0050:  ldarg.0
      IL_0051:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16> Pololu.Usc.Bytecode.BytecodeProgram::subroutineAddresses
      IL_0056:  ldloc.2
      IL_0057:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16>::get_Item(!0)
      IL_005c:  stelem.i2
      IL_005d:  ldloca.s   V_4
      IL_005f:  call       instance bool valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<string,uint8>::MoveNext()
      IL_0064:  brtrue.s   IL_0025

      IL_0066:  leave.s    IL_0076

    }  // end .try
    finally
    {
      IL_0068:  ldloca.s   V_4
      IL_006a:  constrained. valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<string,uint8>
      IL_0070:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0075:  endfinally
    }  // end handler
    IL_0076:  ldloc.1
    IL_0077:  stloc.s    V_5
    IL_0079:  ldc.i4.0
    IL_007a:  stloc.s    V_6
    IL_007c:  br.s       IL_00a2

    IL_007e:  ldloc.s    V_5
    IL_0080:  ldloc.s    V_6
    IL_0082:  ldelem.u2
    IL_0083:  stloc.3
    IL_0084:  ldloc.0
    IL_0085:  ldloc.3
    IL_0086:  ldc.i4     0xff
    IL_008b:  and
    IL_008c:  conv.u1
    IL_008d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
    IL_0092:  ldloc.0
    IL_0093:  ldloc.3
    IL_0094:  ldc.i4.8
    IL_0095:  shr
    IL_0096:  conv.u1
    IL_0097:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::Add(!0)
    IL_009c:  ldloc.s    V_6
    IL_009e:  ldc.i4.1
    IL_009f:  add
    IL_00a0:  stloc.s    V_6
    IL_00a2:  ldloc.s    V_6
    IL_00a4:  ldloc.s    V_5
    IL_00a6:  ldlen
    IL_00a7:  conv.i4
    IL_00a8:  blt.s      IL_007e

    IL_00aa:  ldloc.0
    IL_00ab:  ldarg.0
    IL_00ac:  call       instance class [mscorlib]System.Collections.Generic.List`1<uint8> Pololu.Usc.Bytecode.BytecodeProgram::getByteList()
    IL_00b1:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<uint8>::AddRange(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_00b6:  ldloc.0
    IL_00b7:  call       uint16 Pololu.Usc.Bytecode.BytecodeProgram::CRC(class [mscorlib]System.Collections.Generic.List`1<uint8>)
    IL_00bc:  ret
  } // end of method BytecodeProgram::getCRC

  .method private hidebysig static uint16 
          oneByteCRC(uint8 v) cil managed
  {
    // Code size       40 (0x28)
    .maxstack  2
    .locals init (int32 V_0,
             uint16 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0022

    IL_0006:  ldloc.1
    IL_0007:  ldc.i4.1
    IL_0008:  and
    IL_0009:  ldc.i4.1
    IL_000a:  bne.un.s   IL_0019

    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  shr
    IL_000f:  ldc.i4     0xa001
    IL_0014:  xor
    IL_0015:  conv.u2
    IL_0016:  stloc.1
    IL_0017:  br.s       IL_001e

    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.1
    IL_001b:  shr
    IL_001c:  conv.u2
    IL_001d:  stloc.1
    IL_001e:  ldloc.0
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  ldc.i4.8
    IL_0024:  blt.s      IL_0006

    IL_0026:  ldloc.1
    IL_0027:  ret
  } // end of method BytecodeProgram::oneByteCRC

  .method private hidebysig static uint16 
          CRC(class [mscorlib]System.Collections.Generic.List`1<uint8> message) cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  4
    .locals init (uint16 V_0,
             uint16 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.1
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0021

    IL_0006:  ldloc.1
    IL_0007:  ldc.i4.8
    IL_0008:  shr
    IL_0009:  ldloc.1
    IL_000a:  conv.u1
    IL_000b:  ldarg.0
    IL_000c:  ldloc.0
    IL_000d:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Item(int32)
    IL_0012:  xor
    IL_0013:  conv.u1
    IL_0014:  call       uint16 Pololu.Usc.Bytecode.BytecodeProgram::oneByteCRC(uint8)
    IL_0019:  xor
    IL_001a:  conv.u2
    IL_001b:  stloc.1
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.1
    IL_001e:  add
    IL_001f:  conv.u2
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  ldarg.0
    IL_0023:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Count()
    IL_0028:  blt.s      IL_0006

    IL_002a:  ldloc.1
    IL_002b:  ret
  } // end of method BytecodeProgram::CRC

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       73 (0x49)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<string>::.ctor()
    IL_0006:  stfld      class [mscorlib]System.Collections.Generic.List`1<string> Pololu.Usc.Bytecode.BytecodeProgram::privateSourceLines
    IL_000b:  ldarg.0
    IL_000c:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction>::.ctor()
    IL_0011:  stfld      class [mscorlib]System.Collections.Generic.List`1<class Pololu.Usc.Bytecode.BytecodeInstruction> Pololu.Usc.Bytecode.BytecodeProgram::instructionList
    IL_0016:  ldarg.0
    IL_0017:  newobj     instance void class [System]System.Collections.Generic.Stack`1<int32>::.ctor()
    IL_001c:  stfld      class [System]System.Collections.Generic.Stack`1<int32> Pololu.Usc.Bytecode.BytecodeProgram::openBlocks
    IL_0021:  ldarg.0
    IL_0022:  newobj     instance void class [System]System.Collections.Generic.Stack`1<valuetype Pololu.Usc.Bytecode.BlockType>::.ctor()
    IL_0027:  stfld      class [System]System.Collections.Generic.Stack`1<valuetype Pololu.Usc.Bytecode.BlockType> Pololu.Usc.Bytecode.BytecodeProgram::openBlockTypes
    IL_002c:  ldarg.0
    IL_002d:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16>::.ctor()
    IL_0032:  stfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16> Pololu.Usc.Bytecode.BytecodeProgram::subroutineAddresses
    IL_0037:  ldarg.0
    IL_0038:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8>::.ctor()
    IL_003d:  stfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8> Pololu.Usc.Bytecode.BytecodeProgram::subroutineCommands
    IL_0042:  ldarg.0
    IL_0043:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0048:  ret
  } // end of method BytecodeProgram::.ctor

  .property instance class Pololu.Usc.Bytecode.BytecodeInstruction
          Item(int32)
  {
    .get instance class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeProgram::get_Item(int32)
  } // end of property BytecodeProgram::Item
  .property instance int32 Count()
  {
    .get instance int32 Pololu.Usc.Bytecode.BytecodeProgram::get_Count()
  } // end of property BytecodeProgram::Count
  .property instance bool blockIsOpen()
  {
    .get instance bool Pololu.Usc.Bytecode.BytecodeProgram::get_blockIsOpen()
  } // end of property BytecodeProgram::blockIsOpen
} // end of class Pololu.Usc.Bytecode.BytecodeProgram

.class public abstract auto ansi sealed beforefieldinit Pololu.Usc.Bytecode.BytecodeReader
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested private Mode
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype Pololu.Usc.Bytecode.BytecodeReader/Mode NORMAL = int32(0x00000000)
    .field public static literal valuetype Pololu.Usc.Bytecode.BytecodeReader/Mode GOTO = int32(0x00000001)
    .field public static literal valuetype Pololu.Usc.Bytecode.BytecodeReader/Mode SUBROUTINE = int32(0x00000002)
  } // end of class Mode

  .field private static class [mscorlib]System.Collections.Generic.Dictionary`2<string,valuetype Pololu.Usc.Bytecode.Opcode> dictionary
  .field private static valuetype Pololu.Usc.Bytecode.BytecodeReader/Mode mode
  .method private hidebysig static void  initDictionary() cil managed
  {
    // Code size       86 (0x56)
    .maxstack  4
    .locals init (string[] V_0,
             valuetype Pololu.Usc.Bytecode.Opcode[] V_1,
             int32 V_2)
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,valuetype Pololu.Usc.Bytecode.Opcode> Pololu.Usc.Bytecode.BytecodeReader::dictionary
    IL_0005:  brfalse.s  IL_0008

    IL_0007:  ret

    IL_0008:  ldtoken    Pololu.Usc.Bytecode.Opcode
    IL_000d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0012:  call       string[] [mscorlib]System.Enum::GetNames(class [mscorlib]System.Type)
    IL_0017:  stloc.0
    IL_0018:  ldtoken    Pololu.Usc.Bytecode.Opcode
    IL_001d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0022:  call       class [mscorlib]System.Array [mscorlib]System.Enum::GetValues(class [mscorlib]System.Type)
    IL_0027:  castclass  valuetype Pololu.Usc.Bytecode.Opcode[]
    IL_002c:  stloc.1
    IL_002d:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,valuetype Pololu.Usc.Bytecode.Opcode>::.ctor()
    IL_0032:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,valuetype Pololu.Usc.Bytecode.Opcode> Pololu.Usc.Bytecode.BytecodeReader::dictionary
    IL_0037:  ldc.i4.0
    IL_0038:  stloc.2
    IL_0039:  br.s       IL_004f

    IL_003b:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,valuetype Pololu.Usc.Bytecode.Opcode> Pololu.Usc.Bytecode.BytecodeReader::dictionary
    IL_0040:  ldloc.0
    IL_0041:  ldloc.2
    IL_0042:  ldelem.ref
    IL_0043:  ldloc.1
    IL_0044:  ldloc.2
    IL_0045:  ldelem.i4
    IL_0046:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,valuetype Pololu.Usc.Bytecode.Opcode>::set_Item(!0,
                                                                                                                                                      !1)
    IL_004b:  ldloc.2
    IL_004c:  ldc.i4.1
    IL_004d:  add
    IL_004e:  stloc.2
    IL_004f:  ldloc.2
    IL_0050:  ldloc.0
    IL_0051:  ldlen
    IL_0052:  conv.i4
    IL_0053:  blt.s      IL_003b

    IL_0055:  ret
  } // end of method BytecodeReader::initDictionary

  .method public hidebysig static void  WriteListing(class Pololu.Usc.Bytecode.BytecodeProgram program,
                                                     string filename) cil managed
  {
    // Code size       717 (0x2cd)
    .maxstack  6
    .locals init (class [mscorlib]System.IO.FileStream V_0,
             class [mscorlib]System.IO.StreamWriter V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             class Pololu.Usc.Bytecode.BytecodeInstruction V_5,
             int32 V_6,
             uint8 V_7,
             int32 V_8,
             string[] V_9,
             valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,uint16> V_10,
             string V_11,
             uint8 V_12,
             uint16 V_13,
             int32 V_14,
             valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,uint16> V_15,
             string V_16,
             uint16 V_17,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<uint8> V_18,
             valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<string,uint16> V_19,
             string[] V_20,
             valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<string,uint16> V_21)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.2
    IL_0002:  newobj     instance void [mscorlib]System.IO.FileStream::.ctor(string,
                                                                             valuetype [mscorlib]System.IO.FileMode)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  newobj     instance void [mscorlib]System.IO.StreamWriter::.ctor(class [mscorlib]System.IO.Stream)
    IL_000e:  stloc.1
    IL_000f:  ldc.i4.0
    IL_0010:  stloc.3
    IL_0011:  ldc.i4.0
    IL_0012:  stloc.s    V_4
    IL_0014:  ldnull
    IL_0015:  stloc.s    V_5
    IL_0017:  ldarg.0
    IL_0018:  callvirt   instance int32 Pololu.Usc.Bytecode.BytecodeProgram::get_Count()
    IL_001d:  brfalse.s  IL_0028

    IL_001f:  ldarg.0
    IL_0020:  ldloc.3
    IL_0021:  callvirt   instance class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeProgram::get_Item(int32)
    IL_0026:  stloc.s    V_5
    IL_0028:  ldc.i4.1
    IL_0029:  stloc.2
    IL_002a:  br         IL_0104

    IL_002f:  ldc.i4.0
    IL_0030:  stloc.s    V_6
    IL_0032:  ldloc.1
    IL_0033:  ldloca.s   V_4
    IL_0035:  ldstr      "X4"
    IL_003a:  call       instance string [mscorlib]System.Int32::ToString(string)
    IL_003f:  ldstr      ": "
    IL_0044:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0049:  callvirt   instance void [mscorlib]System.IO.TextWriter::Write(string)
    IL_004e:  br.s       IL_00bb

    IL_0050:  ldloc.s    V_5
    IL_0052:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<uint8> Pololu.Usc.Bytecode.BytecodeInstruction::toByteList()
    IL_0057:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<uint8>::GetEnumerator()
    IL_005c:  stloc.s    V_18
    .try
    {
      IL_005e:  br.s       IL_0087

      IL_0060:  ldloca.s   V_18
      IL_0062:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<uint8>::get_Current()
      IL_0067:  stloc.s    V_7
      IL_0069:  ldloc.1
      IL_006a:  ldloca.s   V_7
      IL_006c:  ldstr      "X2"
      IL_0071:  call       instance string [mscorlib]System.Byte::ToString(string)
      IL_0076:  callvirt   instance void [mscorlib]System.IO.TextWriter::Write(string)
      IL_007b:  ldloc.s    V_4
      IL_007d:  ldc.i4.1
      IL_007e:  add
      IL_007f:  stloc.s    V_4
      IL_0081:  ldloc.s    V_6
      IL_0083:  ldc.i4.2
      IL_0084:  add
      IL_0085:  stloc.s    V_6
      IL_0087:  ldloca.s   V_18
      IL_0089:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<uint8>::MoveNext()
      IL_008e:  brtrue.s   IL_0060

      IL_0090:  leave.s    IL_00a0

    }  // end .try
    finally
    {
      IL_0092:  ldloca.s   V_18
      IL_0094:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<uint8>
      IL_009a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_009f:  endfinally
    }  // end handler
    IL_00a0:  ldloc.3
    IL_00a1:  ldc.i4.1
    IL_00a2:  add
    IL_00a3:  stloc.3
    IL_00a4:  ldloc.3
    IL_00a5:  ldarg.0
    IL_00a6:  callvirt   instance int32 Pololu.Usc.Bytecode.BytecodeProgram::get_Count()
    IL_00ab:  blt.s      IL_00b2

    IL_00ad:  ldnull
    IL_00ae:  stloc.s    V_5
    IL_00b0:  br.s       IL_00bb

    IL_00b2:  ldarg.0
    IL_00b3:  ldloc.3
    IL_00b4:  callvirt   instance class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeProgram::get_Item(int32)
    IL_00b9:  stloc.s    V_5
    IL_00bb:  ldloc.s    V_5
    IL_00bd:  brfalse.s  IL_00c9

    IL_00bf:  ldloc.s    V_5
    IL_00c1:  callvirt   instance int32 Pololu.Usc.Bytecode.BytecodeInstruction::get_lineNumber()
    IL_00c6:  ldloc.2
    IL_00c7:  beq.s      IL_0050

    IL_00c9:  ldc.i4.0
    IL_00ca:  stloc.s    V_8
    IL_00cc:  br.s       IL_00df

    IL_00ce:  ldloc.1
    IL_00cf:  ldstr      " "
    IL_00d4:  callvirt   instance void [mscorlib]System.IO.TextWriter::Write(string)
    IL_00d9:  ldloc.s    V_8
    IL_00db:  ldc.i4.1
    IL_00dc:  add
    IL_00dd:  stloc.s    V_8
    IL_00df:  ldloc.s    V_8
    IL_00e1:  ldc.i4.s   20
    IL_00e3:  ldloc.s    V_6
    IL_00e5:  sub
    IL_00e6:  blt.s      IL_00ce

    IL_00e8:  ldloc.1
    IL_00e9:  ldstr      " -- "
    IL_00ee:  callvirt   instance void [mscorlib]System.IO.TextWriter::Write(string)
    IL_00f3:  ldloc.1
    IL_00f4:  ldarg.0
    IL_00f5:  ldloc.2
    IL_00f6:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeProgram::getSourceLine(int32)
    IL_00fb:  callvirt   instance void [mscorlib]System.IO.TextWriter::WriteLine(string)
    IL_0100:  ldloc.2
    IL_0101:  ldc.i4.1
    IL_0102:  add
    IL_0103:  stloc.2
    IL_0104:  ldloc.2
    IL_0105:  ldarg.0
    IL_0106:  callvirt   instance int32 Pololu.Usc.Bytecode.BytecodeProgram::getSourceLineCount()
    IL_010b:  ble        IL_002f

    IL_0110:  ldloc.1
    IL_0111:  ldstr      ""
    IL_0116:  callvirt   instance void [mscorlib]System.IO.TextWriter::WriteLine(string)
    IL_011b:  ldloc.1
    IL_011c:  ldstr      "Subroutines:"
    IL_0121:  callvirt   instance void [mscorlib]System.IO.TextWriter::WriteLine(string)
    IL_0126:  ldloc.1
    IL_0127:  ldstr      "Hex Decimal Address Name"
    IL_012c:  callvirt   instance void [mscorlib]System.IO.TextWriter::WriteLine(string)
    IL_0131:  ldc.i4     0x80
    IL_0136:  newarr     [mscorlib]System.String
    IL_013b:  stloc.s    V_9
    IL_013d:  ldarg.0
    IL_013e:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16> Pololu.Usc.Bytecode.BytecodeProgram::subroutineAddresses
    IL_0143:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<!0,!1> class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16>::GetEnumerator()
    IL_0148:  stloc.s    V_19
    .try
    {
      IL_014a:  br         IL_01ff

      IL_014f:  ldloca.s   V_19
      IL_0151:  call       instance valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<!0,!1> valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<string,uint16>::get_Current()
      IL_0156:  stloc.s    V_10
      IL_0158:  ldloca.s   V_10
      IL_015a:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,uint16>::get_Key()
      IL_015f:  stloc.s    V_11
      IL_0161:  ldarg.0
      IL_0162:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8> Pololu.Usc.Bytecode.BytecodeProgram::subroutineCommands
      IL_0167:  ldloc.s    V_11
      IL_0169:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8>::get_Item(!0)
      IL_016e:  ldc.i4.s   54
      IL_0170:  beq        IL_01ff

      IL_0175:  ldarg.0
      IL_0176:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8> Pololu.Usc.Bytecode.BytecodeProgram::subroutineCommands
      IL_017b:  ldloc.s    V_11
      IL_017d:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8>::get_Item(!0)
      IL_0182:  ldc.i4     0x80
      IL_0187:  sub
      IL_0188:  conv.u1
      IL_0189:  stloc.s    V_12
      IL_018b:  ldarg.0
      IL_018c:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16> Pololu.Usc.Bytecode.BytecodeProgram::subroutineAddresses
      IL_0191:  ldloc.s    V_11
      IL_0193:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16>::get_Item(!0)
      IL_0198:  stloc.s    V_13
      IL_019a:  ldloc.s    V_9
      IL_019c:  ldloc.s    V_12
      IL_019e:  ldc.i4.7
      IL_019f:  newarr     [mscorlib]System.String
      IL_01a4:  stloc.s    V_20
      IL_01a6:  ldloc.s    V_20
      IL_01a8:  ldc.i4.0
      IL_01a9:  ldloca.s   V_12
      IL_01ab:  ldstr      "X2"
      IL_01b0:  call       instance string [mscorlib]System.Byte::ToString(string)
      IL_01b5:  stelem.ref
      IL_01b6:  ldloc.s    V_20
      IL_01b8:  ldc.i4.1
      IL_01b9:  ldstr      "  "
      IL_01be:  stelem.ref
      IL_01bf:  ldloc.s    V_20
      IL_01c1:  ldc.i4.2
      IL_01c2:  ldloca.s   V_12
      IL_01c4:  ldstr      "D3"
      IL_01c9:  call       instance string [mscorlib]System.Byte::ToString(string)
      IL_01ce:  stelem.ref
      IL_01cf:  ldloc.s    V_20
      IL_01d1:  ldc.i4.3
      IL_01d2:  ldstr      "     "
      IL_01d7:  stelem.ref
      IL_01d8:  ldloc.s    V_20
      IL_01da:  ldc.i4.4
      IL_01db:  ldloca.s   V_13
      IL_01dd:  ldstr      "X4"
      IL_01e2:  call       instance string [mscorlib]System.UInt16::ToString(string)
      IL_01e7:  stelem.ref
      IL_01e8:  ldloc.s    V_20
      IL_01ea:  ldc.i4.5
      IL_01eb:  ldstr      "    "
      IL_01f0:  stelem.ref
      IL_01f1:  ldloc.s    V_20
      IL_01f3:  ldc.i4.6
      IL_01f4:  ldloc.s    V_11
      IL_01f6:  stelem.ref
      IL_01f7:  ldloc.s    V_20
      IL_01f9:  call       string [mscorlib]System.String::Concat(string[])
      IL_01fe:  stelem.ref
      IL_01ff:  ldloca.s   V_19
      IL_0201:  call       instance bool valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<string,uint16>::MoveNext()
      IL_0206:  brtrue     IL_014f

      IL_020b:  leave.s    IL_021b

    }  // end .try
    finally
    {
      IL_020d:  ldloca.s   V_19
      IL_020f:  constrained. valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<string,uint16>
      IL_0215:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_021a:  endfinally
    }  // end handler
    IL_021b:  ldc.i4.0
    IL_021c:  stloc.s    V_14
    IL_021e:  ldc.i4.0
    IL_021f:  stloc.s    V_14
    IL_0221:  br.s       IL_023b

    IL_0223:  ldloc.s    V_9
    IL_0225:  ldloc.s    V_14
    IL_0227:  ldelem.ref
    IL_0228:  brfalse.s  IL_0243

    IL_022a:  ldloc.1
    IL_022b:  ldloc.s    V_9
    IL_022d:  ldloc.s    V_14
    IL_022f:  ldelem.ref
    IL_0230:  callvirt   instance void [mscorlib]System.IO.TextWriter::WriteLine(string)
    IL_0235:  ldloc.s    V_14
    IL_0237:  ldc.i4.1
    IL_0238:  add
    IL_0239:  stloc.s    V_14
    IL_023b:  ldloc.s    V_14
    IL_023d:  ldloc.s    V_9
    IL_023f:  ldlen
    IL_0240:  conv.i4
    IL_0241:  blt.s      IL_0223

    IL_0243:  ldarg.0
    IL_0244:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16> Pololu.Usc.Bytecode.BytecodeProgram::subroutineAddresses
    IL_0249:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<!0,!1> class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16>::GetEnumerator()
    IL_024e:  stloc.s    V_21
    .try
    {
      IL_0250:  br.s       IL_02a7

      IL_0252:  ldloca.s   V_21
      IL_0254:  call       instance valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<!0,!1> valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<string,uint16>::get_Current()
      IL_0259:  stloc.s    V_15
      IL_025b:  ldloca.s   V_15
      IL_025d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,uint16>::get_Key()
      IL_0262:  stloc.s    V_16
      IL_0264:  ldarg.0
      IL_0265:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8> Pololu.Usc.Bytecode.BytecodeProgram::subroutineCommands
      IL_026a:  ldloc.s    V_16
      IL_026c:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint8>::get_Item(!0)
      IL_0271:  ldc.i4.s   54
      IL_0273:  bne.un.s   IL_02a7

      IL_0275:  ldarg.0
      IL_0276:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16> Pololu.Usc.Bytecode.BytecodeProgram::subroutineAddresses
      IL_027b:  ldloc.s    V_16
      IL_027d:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,uint16>::get_Item(!0)
      IL_0282:  stloc.s    V_17
      IL_0284:  ldloc.1
      IL_0285:  ldstr      "--  ---     "
      IL_028a:  ldloca.s   V_17
      IL_028c:  ldstr      "X4"
      IL_0291:  call       instance string [mscorlib]System.UInt16::ToString(string)
      IL_0296:  ldstr      "    "
      IL_029b:  ldloc.s    V_16
      IL_029d:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string,
                                                                  string)
      IL_02a2:  callvirt   instance void [mscorlib]System.IO.TextWriter::WriteLine(string)
      IL_02a7:  ldloca.s   V_21
      IL_02a9:  call       instance bool valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<string,uint16>::MoveNext()
      IL_02ae:  brtrue.s   IL_0252

      IL_02b0:  leave.s    IL_02c0

    }  // end .try
    finally
    {
      IL_02b2:  ldloca.s   V_21
      IL_02b4:  constrained. valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<string,uint16>
      IL_02ba:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_02bf:  endfinally
    }  // end handler
    IL_02c0:  ldloc.1
    IL_02c1:  callvirt   instance void [mscorlib]System.IO.TextWriter::Close()
    IL_02c6:  ldloc.0
    IL_02c7:  callvirt   instance void [mscorlib]System.IO.Stream::Close()
    IL_02cc:  ret
  } // end of method BytecodeReader::WriteListing

  .method public hidebysig static class Pololu.Usc.Bytecode.BytecodeProgram 
          Read(string program,
               bool isMiniMaestro) cil managed
  {
    // Code size       356 (0x164)
    .maxstack  6
    .locals init (string V_0,
             int32 V_1,
             int32 V_2,
             class Pololu.Usc.Bytecode.BytecodeProgram V_3,
             string[] V_4,
             string[] V_5,
             string V_6,
             string V_7,
             string V_8,
             class Pololu.Usc.Bytecode.BytecodeInstruction V_9,
             string[] V_10,
             char[] V_11,
             string[] V_12,
             int32 V_13)
    IL_0000:  call       void Pololu.Usc.Bytecode.BytecodeReader::initDictionary()
    IL_0005:  ldc.i4.0
    IL_0006:  stloc.2
    IL_0007:  newobj     instance void Pololu.Usc.Bytecode.BytecodeProgram::.ctor()
    IL_000c:  stloc.3
    IL_000d:  ldc.i4.0
    IL_000e:  stsfld     valuetype Pololu.Usc.Bytecode.BytecodeReader/Mode Pololu.Usc.Bytecode.BytecodeReader::mode
    IL_0013:  ldarg.0
    IL_0014:  brtrue.s   IL_001d

    IL_0016:  ldstr      ""
    IL_001b:  starg.s    program
    IL_001d:  ldarg.0
    IL_001e:  ldc.i4.2
    IL_001f:  newarr     [mscorlib]System.String
    IL_0024:  stloc.s    V_10
    IL_0026:  ldloc.s    V_10
    IL_0028:  ldc.i4.0
    IL_0029:  ldstr      "\r\n"
    IL_002e:  stelem.ref
    IL_002f:  ldloc.s    V_10
    IL_0031:  ldc.i4.1
    IL_0032:  ldstr      "\n"
    IL_0037:  stelem.ref
    IL_0038:  ldloc.s    V_10
    IL_003a:  ldc.i4.0
    IL_003b:  callvirt   instance string[] [mscorlib]System.String::Split(string[],
                                                                          valuetype [mscorlib]System.StringSplitOptions)
    IL_0040:  stloc.s    V_4
    IL_0042:  ldc.i4.1
    IL_0043:  stloc.1
    IL_0044:  br         IL_011f

    IL_0049:  ldloc.s    V_4
    IL_004b:  ldloc.1
    IL_004c:  ldc.i4.1
    IL_004d:  sub
    IL_004e:  ldelem.ref
    IL_004f:  stloc.0
    IL_0050:  ldloc.3
    IL_0051:  ldloc.0
    IL_0052:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::addSourceLine(string)
    IL_0057:  ldc.i4.1
    IL_0058:  stloc.2
    IL_0059:  ldloc.0
    IL_005a:  ldstr      "#.*"
    IL_005f:  ldstr      ""
    IL_0064:  call       string [System]System.Text.RegularExpressions.Regex::Replace(string,
                                                                                      string,
                                                                                      string)
    IL_0069:  stloc.0
    IL_006a:  ldloc.0
    IL_006b:  ldc.i4.2
    IL_006c:  newarr     [mscorlib]System.Char
    IL_0071:  stloc.s    V_11
    IL_0073:  ldloc.s    V_11
    IL_0075:  ldc.i4.0
    IL_0076:  ldc.i4.s   32
    IL_0078:  stelem.i2
    IL_0079:  ldloc.s    V_11
    IL_007b:  ldc.i4.1
    IL_007c:  ldc.i4.s   9
    IL_007e:  stelem.i2
    IL_007f:  ldloc.s    V_11
    IL_0081:  callvirt   instance string[] [mscorlib]System.String::Split(char[])
    IL_0086:  stloc.s    V_5
    IL_0088:  ldloc.s    V_5
    IL_008a:  stloc.s    V_12
    IL_008c:  ldc.i4.0
    IL_008d:  stloc.s    V_13
    IL_008f:  br.s       IL_0110

    IL_0091:  ldloc.s    V_12
    IL_0093:  ldloc.s    V_13
    IL_0095:  ldelem.ref
    IL_0096:  stloc.s    V_6
    IL_0098:  ldloc.s    V_6
    IL_009a:  ldstr      ""
    IL_009f:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00a4:  brfalse.s  IL_00ac

    IL_00a6:  ldloc.2
    IL_00a7:  ldc.i4.1
    IL_00a8:  add
    IL_00a9:  stloc.2
    IL_00aa:  br.s       IL_010a

    IL_00ac:  ldloc.s    V_6
    IL_00ae:  callvirt   instance string [mscorlib]System.String::ToUpper()
    IL_00b3:  stloc.s    V_7
    IL_00b5:  ldsfld     valuetype Pololu.Usc.Bytecode.BytecodeReader/Mode Pololu.Usc.Bytecode.BytecodeReader::mode
    IL_00ba:  brtrue.s   IL_00ce

    IL_00bc:  ldloc.s    V_7
    IL_00be:  ldloc.3
    IL_00bf:  ldstr      "script"
    IL_00c4:  ldloc.1
    IL_00c5:  ldloc.2
    IL_00c6:  ldarg.1
    IL_00c7:  call       void Pololu.Usc.Bytecode.BytecodeReader::parseString(string,
                                                                              class Pololu.Usc.Bytecode.BytecodeProgram,
                                                                              string,
                                                                              int32,
                                                                              int32,
                                                                              bool)
    IL_00cc:  br.s       IL_00fe

    IL_00ce:  ldsfld     valuetype Pololu.Usc.Bytecode.BytecodeReader/Mode Pololu.Usc.Bytecode.BytecodeReader::mode
    IL_00d3:  ldc.i4.1
    IL_00d4:  bne.un.s   IL_00e7

    IL_00d6:  ldloc.s    V_7
    IL_00d8:  ldloc.3
    IL_00d9:  ldstr      "script"
    IL_00de:  ldloc.1
    IL_00df:  ldloc.2
    IL_00e0:  call       void Pololu.Usc.Bytecode.BytecodeReader::parseGoto(string,
                                                                            class Pololu.Usc.Bytecode.BytecodeProgram,
                                                                            string,
                                                                            int32,
                                                                            int32)
    IL_00e5:  br.s       IL_00fe

    IL_00e7:  ldsfld     valuetype Pololu.Usc.Bytecode.BytecodeReader/Mode Pololu.Usc.Bytecode.BytecodeReader::mode
    IL_00ec:  ldc.i4.2
    IL_00ed:  bne.un.s   IL_00fe

    IL_00ef:  ldloc.s    V_7
    IL_00f1:  ldloc.3
    IL_00f2:  ldstr      "script"
    IL_00f7:  ldloc.1
    IL_00f8:  ldloc.2
    IL_00f9:  call       void Pololu.Usc.Bytecode.BytecodeReader::parseSubroutine(string,
                                                                                  class Pololu.Usc.Bytecode.BytecodeProgram,
                                                                                  string,
                                                                                  int32,
                                                                                  int32)
    IL_00fe:  ldloc.2
    IL_00ff:  ldloc.s    V_7
    IL_0101:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_0106:  ldc.i4.1
    IL_0107:  add
    IL_0108:  add
    IL_0109:  stloc.2
    IL_010a:  ldloc.s    V_13
    IL_010c:  ldc.i4.1
    IL_010d:  add
    IL_010e:  stloc.s    V_13
    IL_0110:  ldloc.s    V_13
    IL_0112:  ldloc.s    V_12
    IL_0114:  ldlen
    IL_0115:  conv.i4
    IL_0116:  blt        IL_0091

    IL_011b:  ldloc.1
    IL_011c:  ldc.i4.1
    IL_011d:  add
    IL_011e:  stloc.1
    IL_011f:  ldloc.1
    IL_0120:  ldloc.s    V_4
    IL_0122:  ldlen
    IL_0123:  conv.i4
    IL_0124:  ble        IL_0049

    IL_0129:  ldloc.3
    IL_012a:  callvirt   instance bool Pololu.Usc.Bytecode.BytecodeProgram::get_blockIsOpen()
    IL_012f:  brfalse.s  IL_014f

    IL_0131:  ldloc.3
    IL_0132:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeProgram::getCurrentBlockStartLabel()
    IL_0137:  stloc.s    V_8
    IL_0139:  ldloc.3
    IL_013a:  ldloc.s    V_8
    IL_013c:  callvirt   instance class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeProgram::findLabelInstruction(string)
    IL_0141:  stloc.s    V_9
    IL_0143:  ldloc.s    V_9
    IL_0145:  ldstr      "BEGIN block was never closed."
    IL_014a:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeInstruction::'error'(string)
    IL_014f:  ldloc.3
    IL_0150:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::completeLiterals()
    IL_0155:  ldloc.3
    IL_0156:  ldarg.1
    IL_0157:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::completeCalls(bool)
    IL_015c:  ldloc.3
    IL_015d:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::completeJumps()
    IL_0162:  ldloc.3
    IL_0163:  ret
  } // end of method BytecodeReader::Read

  .method private hidebysig static void  parseGoto(string s,
                                                   class Pololu.Usc.Bytecode.BytecodeProgram bytecode_program,
                                                   string filename,
                                                   int32 line_number,
                                                   int32 column_number) cil managed
  {
    // Code size       33 (0x21)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldstr      "USER_"
    IL_0006:  ldarg.0
    IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000c:  ldarg.2
    IL_000d:  ldarg.3
    IL_000e:  ldarg.s    column_number
    IL_0010:  call       class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeInstruction::newJumpToLabel(string,
                                                                                                                               string,
                                                                                                                               int32,
                                                                                                                               int32)
    IL_0015:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction)
    IL_001a:  ldc.i4.0
    IL_001b:  stsfld     valuetype Pololu.Usc.Bytecode.BytecodeReader/Mode Pololu.Usc.Bytecode.BytecodeReader::mode
    IL_0020:  ret
  } // end of method BytecodeReader::parseGoto

  .method private hidebysig static void  parseSubroutine(string s,
                                                         class Pololu.Usc.Bytecode.BytecodeProgram bytecode_program,
                                                         string filename,
                                                         int32 line_number,
                                                         int32 column_number) cil managed
  {
    // Code size       153 (0x99)
    .maxstack  5
    .locals init (string V_0,
             string[] V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  call       bool Pololu.Usc.Bytecode.BytecodeReader::looksLikeLiteral(string)
    IL_0006:  brfalse.s  IL_001e

    IL_0008:  ldstr      "The name "
    IL_000d:  ldarg.0
    IL_000e:  ldstr      " is not valid as a subroutine name (it looks like "
    + "a number)."
    IL_0013:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0018:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_001d:  throw

    IL_001e:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,valuetype Pololu.Usc.Bytecode.Opcode> Pololu.Usc.Bytecode.BytecodeReader::dictionary
    IL_0023:  ldarg.0
    IL_0024:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,valuetype Pololu.Usc.Bytecode.Opcode>::ContainsKey(!0)
    IL_0029:  brfalse.s  IL_0041

    IL_002b:  ldstr      "The name "
    IL_0030:  ldarg.0
    IL_0031:  ldstr      " is not valid as a subroutine name (it is a built-"
    + "in command)."
    IL_0036:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_003b:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0040:  throw

    IL_0041:  ldtoken    Pololu.Usc.Bytecode.Keyword
    IL_0046:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004b:  call       string[] [mscorlib]System.Enum::GetNames(class [mscorlib]System.Type)
    IL_0050:  stloc.1
    IL_0051:  ldc.i4.0
    IL_0052:  stloc.2
    IL_0053:  br.s       IL_007c

    IL_0055:  ldloc.1
    IL_0056:  ldloc.2
    IL_0057:  ldelem.ref
    IL_0058:  stloc.0
    IL_0059:  ldloc.0
    IL_005a:  ldarg.0
    IL_005b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0060:  brfalse.s  IL_0078

    IL_0062:  ldstr      "The name "
    IL_0067:  ldarg.0
    IL_0068:  ldstr      " is not valid as a subroutine name (it is a keywor"
    + "d)."
    IL_006d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0072:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0077:  throw

    IL_0078:  ldloc.2
    IL_0079:  ldc.i4.1
    IL_007a:  add
    IL_007b:  stloc.2
    IL_007c:  ldloc.2
    IL_007d:  ldloc.1
    IL_007e:  ldlen
    IL_007f:  conv.i4
    IL_0080:  blt.s      IL_0055

    IL_0082:  ldarg.1
    IL_0083:  ldarg.0
    IL_0084:  ldarg.2
    IL_0085:  ldarg.3
    IL_0086:  ldarg.s    column_number
    IL_0088:  call       class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeInstruction::newSubroutine(string,
                                                                                                                              string,
                                                                                                                              int32,
                                                                                                                              int32)
    IL_008d:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction)
    IL_0092:  ldc.i4.0
    IL_0093:  stsfld     valuetype Pololu.Usc.Bytecode.BytecodeReader/Mode Pololu.Usc.Bytecode.BytecodeReader::mode
    IL_0098:  ret
  } // end of method BytecodeReader::parseSubroutine

  .method private hidebysig static bool  looksLikeLiteral(string s) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "^-\?[0-9.]+$"
    IL_0006:  call       class [System]System.Text.RegularExpressions.Match [System]System.Text.RegularExpressions.Regex::Match(string,
                                                                                                                                string)
    IL_000b:  callvirt   instance bool [System]System.Text.RegularExpressions.Group::get_Success()
    IL_0010:  brtrue.s   IL_0023

    IL_0012:  ldarg.0
    IL_0013:  ldstr      "^0[xX][0-9a-fA-F.]+$"
    IL_0018:  call       class [System]System.Text.RegularExpressions.Match [System]System.Text.RegularExpressions.Regex::Match(string,
                                                                                                                                string)
    IL_001d:  callvirt   instance bool [System]System.Text.RegularExpressions.Group::get_Success()
    IL_0022:  ret

    IL_0023:  ldc.i4.1
    IL_0024:  ret
  } // end of method BytecodeReader::looksLikeLiteral

  .method private hidebysig static void  parseString(string s,
                                                     class Pololu.Usc.Bytecode.BytecodeProgram bytecode_program,
                                                     string filename,
                                                     int32 line_number,
                                                     int32 column_number,
                                                     bool isMiniMaestro) cil managed
  {
    // Code size       1506 (0x5e2)
    .maxstack  6
    .locals init (int32 V_0,
             valuetype [mscorlib]System.Decimal V_1,
             class [mscorlib]System.Exception V_2,
             class [System]System.Text.RegularExpressions.Match V_3,
             valuetype Pololu.Usc.Bytecode.Opcode V_4,
             object[] V_5,
             object[] V_6,
             object[] V_7,
             object[] V_8,
             object[] V_9,
             object[] V_10,
             object[] V_11,
             object[] V_12)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  call       bool Pololu.Usc.Bytecode.BytecodeReader::looksLikeLiteral(string)
      IL_0006:  brfalse    IL_01a8

      IL_000b:  ldarg.0
      IL_000c:  ldstr      "0X"
      IL_0011:  callvirt   instance bool [mscorlib]System.String::StartsWith(string)
      IL_0016:  brfalse    IL_00d4

      IL_001b:  ldarg.0
      IL_001c:  ldc.i4.2
      IL_001d:  callvirt   instance string [mscorlib]System.String::Substring(int32)
      IL_0022:  ldc.i4     0x203
      IL_0027:  call       int64 [mscorlib]System.Int64::Parse(string,
                                                               valuetype [mscorlib]System.Globalization.NumberStyles)
      IL_002c:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int64)
      IL_0031:  stloc.1
      IL_0032:  ldloc.1
      IL_0033:  ldc.i4     0xffff
      IL_0038:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
      IL_003d:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                         valuetype [mscorlib]System.Decimal)
      IL_0042:  brtrue.s   IL_0052

      IL_0044:  ldloc.1
      IL_0045:  ldc.i4.0
      IL_0046:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
      IL_004b:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                      valuetype [mscorlib]System.Decimal)
      IL_0050:  brfalse.s  IL_00a8

      IL_0052:  ldc.i4.7
      IL_0053:  newarr     [mscorlib]System.Object
      IL_0058:  stloc.s    V_5
      IL_005a:  ldloc.s    V_5
      IL_005c:  ldc.i4.0
      IL_005d:  ldstr      "Value "
      IL_0062:  stelem.ref
      IL_0063:  ldloc.s    V_5
      IL_0065:  ldc.i4.1
      IL_0066:  ldarg.0
      IL_0067:  stelem.ref
      IL_0068:  ldloc.s    V_5
      IL_006a:  ldc.i4.2
      IL_006b:  ldstr      " is not in the allowed range of "
      IL_0070:  stelem.ref
      IL_0071:  ldloc.s    V_5
      IL_0073:  ldc.i4.3
      IL_0074:  ldc.i4.0
      IL_0075:  box        [mscorlib]System.UInt16
      IL_007a:  stelem.ref
      IL_007b:  ldloc.s    V_5
      IL_007d:  ldc.i4.4
      IL_007e:  ldstr      " to "
      IL_0083:  stelem.ref
      IL_0084:  ldloc.s    V_5
      IL_0086:  ldc.i4.5
      IL_0087:  ldc.i4     0xffff
      IL_008c:  box        [mscorlib]System.UInt16
      IL_0091:  stelem.ref
      IL_0092:  ldloc.s    V_5
      IL_0094:  ldc.i4.6
      IL_0095:  ldstr      "."
      IL_009a:  stelem.ref
      IL_009b:  ldloc.s    V_5
      IL_009d:  call       string [mscorlib]System.String::Concat(object[])
      IL_00a2:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_00a7:  throw

      IL_00a8:  ldloc.1
      IL_00a9:  call       uint16 [mscorlib]System.Decimal::op_Explicit(valuetype [mscorlib]System.Decimal)
      IL_00ae:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(uint16)
      IL_00b3:  ldloc.1
      IL_00b4:  call       bool [mscorlib]System.Decimal::op_Inequality(valuetype [mscorlib]System.Decimal,
                                                                        valuetype [mscorlib]System.Decimal)
      IL_00b9:  brfalse    IL_0182

      IL_00be:  ldstr      "Value "
      IL_00c3:  ldarg.0
      IL_00c4:  ldstr      " must be an integer."
      IL_00c9:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string)
      IL_00ce:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_00d3:  throw

      IL_00d4:  ldarg.0
      IL_00d5:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::Parse(string)
      IL_00da:  stloc.1
      IL_00db:  ldloc.1
      IL_00dc:  ldc.i4     0x7fff
      IL_00e1:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
      IL_00e6:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                         valuetype [mscorlib]System.Decimal)
      IL_00eb:  brtrue.s   IL_00ff

      IL_00ed:  ldloc.1
      IL_00ee:  ldc.i4     0xffff8000
      IL_00f3:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
      IL_00f8:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                      valuetype [mscorlib]System.Decimal)
      IL_00fd:  brfalse.s  IL_0159

      IL_00ff:  ldc.i4.7
      IL_0100:  newarr     [mscorlib]System.Object
      IL_0105:  stloc.s    V_6
      IL_0107:  ldloc.s    V_6
      IL_0109:  ldc.i4.0
      IL_010a:  ldstr      "Value "
      IL_010f:  stelem.ref
      IL_0110:  ldloc.s    V_6
      IL_0112:  ldc.i4.1
      IL_0113:  ldarg.0
      IL_0114:  stelem.ref
      IL_0115:  ldloc.s    V_6
      IL_0117:  ldc.i4.2
      IL_0118:  ldstr      " is not in the allowed range of "
      IL_011d:  stelem.ref
      IL_011e:  ldloc.s    V_6
      IL_0120:  ldc.i4.3
      IL_0121:  ldc.i4     0xffff8000
      IL_0126:  box        [mscorlib]System.Int16
      IL_012b:  stelem.ref
      IL_012c:  ldloc.s    V_6
      IL_012e:  ldc.i4.4
      IL_012f:  ldstr      " to "
      IL_0134:  stelem.ref
      IL_0135:  ldloc.s    V_6
      IL_0137:  ldc.i4.5
      IL_0138:  ldc.i4     0x7fff
      IL_013d:  box        [mscorlib]System.Int16
      IL_0142:  stelem.ref
      IL_0143:  ldloc.s    V_6
      IL_0145:  ldc.i4.6
      IL_0146:  ldstr      "."
      IL_014b:  stelem.ref
      IL_014c:  ldloc.s    V_6
      IL_014e:  call       string [mscorlib]System.String::Concat(object[])
      IL_0153:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_0158:  throw

      IL_0159:  ldloc.1
      IL_015a:  call       int16 [mscorlib]System.Decimal::op_Explicit(valuetype [mscorlib]System.Decimal)
      IL_015f:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int16)
      IL_0164:  ldloc.1
      IL_0165:  call       bool [mscorlib]System.Decimal::op_Inequality(valuetype [mscorlib]System.Decimal,
                                                                        valuetype [mscorlib]System.Decimal)
      IL_016a:  brfalse.s  IL_0182

      IL_016c:  ldstr      "Value "
      IL_0171:  ldarg.0
      IL_0172:  ldstr      " must be an integer."
      IL_0177:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string)
      IL_017c:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_0181:  throw

      IL_0182:  ldloc.1
      IL_0183:  ldc.i4     0xffff
      IL_0188:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
      IL_018d:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                   valuetype [mscorlib]System.Decimal)
      IL_0192:  call       int64 [mscorlib]System.Decimal::op_Explicit(valuetype [mscorlib]System.Decimal)
      IL_0197:  conv.i2
      IL_0198:  stloc.0
      IL_0199:  ldarg.1
      IL_019a:  ldloc.0
      IL_019b:  ldarg.2
      IL_019c:  ldarg.3
      IL_019d:  ldarg.s    column_number
      IL_019f:  ldarg.s    isMiniMaestro
      IL_01a1:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::addLiteral(int32,
                                                                                         string,
                                                                                         int32,
                                                                                         int32,
                                                                                         bool)
      IL_01a6:  leave.s    IL_01e0

      IL_01a8:  leave.s    IL_01c7

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_01aa:  stloc.2
      IL_01ab:  ldstr      "Error parsing "
      IL_01b0:  ldarg.0
      IL_01b1:  ldstr      ": "
      IL_01b6:  ldloc.2
      IL_01b7:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_01bc:  call       string [mscorlib]System.String::Concat(string,
                                                                  string,
                                                                  string,
                                                                  string)
      IL_01c1:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_01c6:  throw

    }  // end handler
    IL_01c7:  ldarg.0
    IL_01c8:  ldc.i4.0
    IL_01c9:  box        Pololu.Usc.Bytecode.Keyword
    IL_01ce:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_01d3:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_01d8:  brfalse.s  IL_01e1

    IL_01da:  ldc.i4.1
    IL_01db:  stsfld     valuetype Pololu.Usc.Bytecode.BytecodeReader/Mode Pololu.Usc.Bytecode.BytecodeReader::mode
    IL_01e0:  ret

    IL_01e1:  ldarg.0
    IL_01e2:  ldc.i4.1
    IL_01e3:  box        Pololu.Usc.Bytecode.Keyword
    IL_01e8:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_01ed:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_01f2:  brfalse.s  IL_01fb

    IL_01f4:  ldc.i4.2
    IL_01f5:  stsfld     valuetype Pololu.Usc.Bytecode.BytecodeReader/Mode Pololu.Usc.Bytecode.BytecodeReader::mode
    IL_01fa:  ret

    IL_01fb:  ldarg.0
    IL_01fc:  ldstr      "(.*):$"
    IL_0201:  call       class [System]System.Text.RegularExpressions.Match [System]System.Text.RegularExpressions.Regex::Match(string,
                                                                                                                                string)
    IL_0206:  stloc.3
    IL_0207:  ldloc.3
    IL_0208:  callvirt   instance bool [System]System.Text.RegularExpressions.Group::get_Success()
    IL_020d:  brfalse.s  IL_023a

    IL_020f:  ldarg.1
    IL_0210:  ldstr      "USER_"
    IL_0215:  ldloc.3
    IL_0216:  callvirt   instance class [System]System.Text.RegularExpressions.GroupCollection [System]System.Text.RegularExpressions.Match::get_Groups()
    IL_021b:  ldc.i4.1
    IL_021c:  callvirt   instance class [System]System.Text.RegularExpressions.Group [System]System.Text.RegularExpressions.GroupCollection::get_Item(int32)
    IL_0221:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0226:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_022b:  ldarg.2
    IL_022c:  ldarg.3
    IL_022d:  ldarg.s    column_number
    IL_022f:  call       class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeInstruction::newLabel(string,
                                                                                                                         string,
                                                                                                                         int32,
                                                                                                                         int32)
    IL_0234:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction)
    IL_0239:  ret

    IL_023a:  ldarg.0
    IL_023b:  ldc.i4.2
    IL_023c:  box        Pololu.Usc.Bytecode.Keyword
    IL_0241:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0246:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_024b:  brfalse.s  IL_0259

    IL_024d:  ldarg.1
    IL_024e:  ldc.i4.0
    IL_024f:  ldarg.2
    IL_0250:  ldarg.3
    IL_0251:  ldarg.s    column_number
    IL_0253:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::openBlock(valuetype Pololu.Usc.Bytecode.BlockType,
                                                                                      string,
                                                                                      int32,
                                                                                      int32)
    IL_0258:  ret

    IL_0259:  ldarg.0
    IL_025a:  ldc.i4.3
    IL_025b:  box        Pololu.Usc.Bytecode.Keyword
    IL_0260:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0265:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_026a:  brfalse.s  IL_0295

    IL_026c:  ldarg.1
    IL_026d:  callvirt   instance valuetype Pololu.Usc.Bytecode.BlockType Pololu.Usc.Bytecode.BytecodeProgram::getCurrentBlockType()
    IL_0272:  brfalse.s  IL_027f

    IL_0274:  ldstr      "WHILE must be inside a BEGIN...REPEAT block"
    IL_0279:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_027e:  throw

    IL_027f:  ldarg.1
    IL_0280:  ldarg.1
    IL_0281:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeProgram::getCurrentBlockEndLabel()
    IL_0286:  ldarg.2
    IL_0287:  ldarg.3
    IL_0288:  ldarg.s    column_number
    IL_028a:  call       class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeInstruction::newConditionalJumpToLabel(string,
                                                                                                                                          string,
                                                                                                                                          int32,
                                                                                                                                          int32)
    IL_028f:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction)
    IL_0294:  ret

    IL_0295:  ldarg.0
    IL_0296:  ldc.i4.4
    IL_0297:  box        Pololu.Usc.Bytecode.Keyword
    IL_029c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_02a1:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_02a6:  brfalse    IL_032b

    .try
    {
      IL_02ab:  ldarg.1
      IL_02ac:  callvirt   instance valuetype Pololu.Usc.Bytecode.BlockType Pololu.Usc.Bytecode.BytecodeProgram::getCurrentBlockType()
      IL_02b1:  brfalse.s  IL_02be

      IL_02b3:  ldstr      "REPEAT must end a BEGIN...REPEAT block"
      IL_02b8:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_02bd:  throw

      IL_02be:  ldarg.1
      IL_02bf:  ldarg.1
      IL_02c0:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeProgram::getCurrentBlockStartLabel()
      IL_02c5:  ldarg.2
      IL_02c6:  ldarg.3
      IL_02c7:  ldarg.s    column_number
      IL_02c9:  call       class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeInstruction::newJumpToLabel(string,
                                                                                                                                 string,
                                                                                                                                 int32,
                                                                                                                                 int32)
      IL_02ce:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction)
      IL_02d3:  ldarg.1
      IL_02d4:  ldarg.2
      IL_02d5:  ldarg.3
      IL_02d6:  ldarg.s    column_number
      IL_02d8:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::closeBlock(string,
                                                                                         int32,
                                                                                         int32)
      IL_02dd:  leave.s    IL_032a

    }  // end .try
    catch [mscorlib]System.InvalidOperationException 
    {
      IL_02df:  pop
      IL_02e0:  ldc.i4.6
      IL_02e1:  newarr     [mscorlib]System.Object
      IL_02e6:  stloc.s    V_7
      IL_02e8:  ldloc.s    V_7
      IL_02ea:  ldc.i4.0
      IL_02eb:  ldarg.2
      IL_02ec:  stelem.ref
      IL_02ed:  ldloc.s    V_7
      IL_02ef:  ldc.i4.1
      IL_02f0:  ldstr      ":"
      IL_02f5:  stelem.ref
      IL_02f6:  ldloc.s    V_7
      IL_02f8:  ldc.i4.2
      IL_02f9:  ldarg.3
      IL_02fa:  box        [mscorlib]System.Int32
      IL_02ff:  stelem.ref
      IL_0300:  ldloc.s    V_7
      IL_0302:  ldc.i4.3
      IL_0303:  ldstr      ":"
      IL_0308:  stelem.ref
      IL_0309:  ldloc.s    V_7
      IL_030b:  ldc.i4.4
      IL_030c:  ldarg.s    column_number
      IL_030e:  box        [mscorlib]System.Int32
      IL_0313:  stelem.ref
      IL_0314:  ldloc.s    V_7
      IL_0316:  ldc.i4.5
      IL_0317:  ldstr      ": Found REPEAT without a corresponding BEGIN"
      IL_031c:  stelem.ref
      IL_031d:  ldloc.s    V_7
      IL_031f:  call       string [mscorlib]System.String::Concat(object[])
      IL_0324:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_0329:  throw

    }  // end handler
    IL_032a:  ret

    IL_032b:  ldarg.0
    IL_032c:  ldc.i4.5
    IL_032d:  box        Pololu.Usc.Bytecode.Keyword
    IL_0332:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0337:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_033c:  brfalse.s  IL_035f

    IL_033e:  ldarg.1
    IL_033f:  ldc.i4.1
    IL_0340:  ldarg.2
    IL_0341:  ldarg.3
    IL_0342:  ldarg.s    column_number
    IL_0344:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::openBlock(valuetype Pololu.Usc.Bytecode.BlockType,
                                                                                      string,
                                                                                      int32,
                                                                                      int32)
    IL_0349:  ldarg.1
    IL_034a:  ldarg.1
    IL_034b:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeProgram::getCurrentBlockEndLabel()
    IL_0350:  ldarg.2
    IL_0351:  ldarg.3
    IL_0352:  ldarg.s    column_number
    IL_0354:  call       class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeInstruction::newConditionalJumpToLabel(string,
                                                                                                                                          string,
                                                                                                                                          int32,
                                                                                                                                          int32)
    IL_0359:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction)
    IL_035e:  ret

    IL_035f:  ldarg.0
    IL_0360:  ldc.i4.6
    IL_0361:  box        Pololu.Usc.Bytecode.Keyword
    IL_0366:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_036b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0370:  brfalse.s  IL_03e7

    .try
    {
      IL_0372:  ldarg.1
      IL_0373:  callvirt   instance valuetype Pololu.Usc.Bytecode.BlockType Pololu.Usc.Bytecode.BytecodeProgram::getCurrentBlockType()
      IL_0378:  ldc.i4.1
      IL_0379:  beq.s      IL_038f

      IL_037b:  ldarg.1
      IL_037c:  callvirt   instance valuetype Pololu.Usc.Bytecode.BlockType Pololu.Usc.Bytecode.BytecodeProgram::getCurrentBlockType()
      IL_0381:  ldc.i4.2
      IL_0382:  beq.s      IL_038f

      IL_0384:  ldstr      "ENDIF must end an IF...ENDIF or an IF...ELSE...END"
      + "IF block."
      IL_0389:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_038e:  throw

      IL_038f:  ldarg.1
      IL_0390:  ldarg.2
      IL_0391:  ldarg.3
      IL_0392:  ldarg.s    column_number
      IL_0394:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::closeBlock(string,
                                                                                         int32,
                                                                                         int32)
      IL_0399:  leave.s    IL_03e6

    }  // end .try
    catch [mscorlib]System.InvalidOperationException 
    {
      IL_039b:  pop
      IL_039c:  ldc.i4.6
      IL_039d:  newarr     [mscorlib]System.Object
      IL_03a2:  stloc.s    V_8
      IL_03a4:  ldloc.s    V_8
      IL_03a6:  ldc.i4.0
      IL_03a7:  ldarg.2
      IL_03a8:  stelem.ref
      IL_03a9:  ldloc.s    V_8
      IL_03ab:  ldc.i4.1
      IL_03ac:  ldstr      ":"
      IL_03b1:  stelem.ref
      IL_03b2:  ldloc.s    V_8
      IL_03b4:  ldc.i4.2
      IL_03b5:  ldarg.3
      IL_03b6:  box        [mscorlib]System.Int32
      IL_03bb:  stelem.ref
      IL_03bc:  ldloc.s    V_8
      IL_03be:  ldc.i4.3
      IL_03bf:  ldstr      ":"
      IL_03c4:  stelem.ref
      IL_03c5:  ldloc.s    V_8
      IL_03c7:  ldc.i4.4
      IL_03c8:  ldarg.s    column_number
      IL_03ca:  box        [mscorlib]System.Int32
      IL_03cf:  stelem.ref
      IL_03d0:  ldloc.s    V_8
      IL_03d2:  ldc.i4.5
      IL_03d3:  ldstr      ": Found ENDIF without a corresponding IF"
      IL_03d8:  stelem.ref
      IL_03d9:  ldloc.s    V_8
      IL_03db:  call       string [mscorlib]System.String::Concat(object[])
      IL_03e0:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_03e5:  throw

    }  // end handler
    IL_03e6:  ret

    IL_03e7:  ldarg.0
    IL_03e8:  ldc.i4.7
    IL_03e9:  box        Pololu.Usc.Bytecode.Keyword
    IL_03ee:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_03f3:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_03f8:  brfalse    IL_0489

    .try
    {
      IL_03fd:  ldarg.1
      IL_03fe:  callvirt   instance valuetype Pololu.Usc.Bytecode.BlockType Pololu.Usc.Bytecode.BytecodeProgram::getCurrentBlockType()
      IL_0403:  ldc.i4.1
      IL_0404:  beq.s      IL_0411

      IL_0406:  ldstr      "ELSE must be part of an IF...ELSE...ENDIF block."
      IL_040b:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_0410:  throw

      IL_0411:  ldarg.1
      IL_0412:  ldarg.1
      IL_0413:  callvirt   instance string Pololu.Usc.Bytecode.BytecodeProgram::getNextBlockEndLabel()
      IL_0418:  ldarg.2
      IL_0419:  ldarg.3
      IL_041a:  ldarg.s    column_number
      IL_041c:  call       class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeInstruction::newJumpToLabel(string,
                                                                                                                                 string,
                                                                                                                                 int32,
                                                                                                                                 int32)
      IL_0421:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction)
      IL_0426:  ldarg.1
      IL_0427:  ldarg.2
      IL_0428:  ldarg.3
      IL_0429:  ldarg.s    column_number
      IL_042b:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::closeBlock(string,
                                                                                         int32,
                                                                                         int32)
      IL_0430:  ldarg.1
      IL_0431:  ldc.i4.2
      IL_0432:  ldarg.2
      IL_0433:  ldarg.3
      IL_0434:  ldarg.s    column_number
      IL_0436:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::openBlock(valuetype Pololu.Usc.Bytecode.BlockType,
                                                                                        string,
                                                                                        int32,
                                                                                        int32)
      IL_043b:  leave.s    IL_0488

    }  // end .try
    catch [mscorlib]System.InvalidOperationException 
    {
      IL_043d:  pop
      IL_043e:  ldc.i4.6
      IL_043f:  newarr     [mscorlib]System.Object
      IL_0444:  stloc.s    V_9
      IL_0446:  ldloc.s    V_9
      IL_0448:  ldc.i4.0
      IL_0449:  ldarg.2
      IL_044a:  stelem.ref
      IL_044b:  ldloc.s    V_9
      IL_044d:  ldc.i4.1
      IL_044e:  ldstr      ":"
      IL_0453:  stelem.ref
      IL_0454:  ldloc.s    V_9
      IL_0456:  ldc.i4.2
      IL_0457:  ldarg.3
      IL_0458:  box        [mscorlib]System.Int32
      IL_045d:  stelem.ref
      IL_045e:  ldloc.s    V_9
      IL_0460:  ldc.i4.3
      IL_0461:  ldstr      ":"
      IL_0466:  stelem.ref
      IL_0467:  ldloc.s    V_9
      IL_0469:  ldc.i4.4
      IL_046a:  ldarg.s    column_number
      IL_046c:  box        [mscorlib]System.Int32
      IL_0471:  stelem.ref
      IL_0472:  ldloc.s    V_9
      IL_0474:  ldc.i4.5
      IL_0475:  ldstr      ": Found ELSE without a corresponding IF"
      IL_047a:  stelem.ref
      IL_047b:  ldloc.s    V_9
      IL_047d:  call       string [mscorlib]System.String::Concat(object[])
      IL_0482:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_0487:  throw

    }  // end handler
    IL_0488:  ret

    .try
    {
      IL_0489:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,valuetype Pololu.Usc.Bytecode.Opcode> Pololu.Usc.Bytecode.BytecodeReader::dictionary
      IL_048e:  ldarg.0
      IL_048f:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,valuetype Pololu.Usc.Bytecode.Opcode>::get_Item(!0)
      IL_0494:  stloc.s    V_4
      IL_0496:  ldloc.s    V_4
      IL_0498:  ldc.i4.1
      IL_0499:  beq.s      IL_04aa

      IL_049b:  ldloc.s    V_4
      IL_049d:  ldc.i4.2
      IL_049e:  beq.s      IL_04aa

      IL_04a0:  ldloc.s    V_4
      IL_04a2:  ldc.i4.3
      IL_04a3:  beq.s      IL_04aa

      IL_04a5:  ldloc.s    V_4
      IL_04a7:  ldc.i4.4
      IL_04a8:  bne.un.s   IL_04f4

      IL_04aa:  ldc.i4.6
      IL_04ab:  newarr     [mscorlib]System.Object
      IL_04b0:  stloc.s    V_10
      IL_04b2:  ldloc.s    V_10
      IL_04b4:  ldc.i4.0
      IL_04b5:  ldarg.2
      IL_04b6:  stelem.ref
      IL_04b7:  ldloc.s    V_10
      IL_04b9:  ldc.i4.1
      IL_04ba:  ldstr      ":"
      IL_04bf:  stelem.ref
      IL_04c0:  ldloc.s    V_10
      IL_04c2:  ldc.i4.2
      IL_04c3:  ldarg.3
      IL_04c4:  box        [mscorlib]System.Int32
      IL_04c9:  stelem.ref
      IL_04ca:  ldloc.s    V_10
      IL_04cc:  ldc.i4.3
      IL_04cd:  ldstr      ":"
      IL_04d2:  stelem.ref
      IL_04d3:  ldloc.s    V_10
      IL_04d5:  ldc.i4.4
      IL_04d6:  ldarg.s    column_number
      IL_04d8:  box        [mscorlib]System.Int32
      IL_04dd:  stelem.ref
      IL_04de:  ldloc.s    V_10
      IL_04e0:  ldc.i4.5
      IL_04e1:  ldstr      ": Literal commands may not be used directly in a p"
      + "rogram.  Integers should be entered directly."
      IL_04e6:  stelem.ref
      IL_04e7:  ldloc.s    V_10
      IL_04e9:  call       string [mscorlib]System.String::Concat(object[])
      IL_04ee:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_04f3:  throw

      IL_04f4:  ldloc.s    V_4
      IL_04f6:  ldc.i4.6
      IL_04f7:  beq.s      IL_04fe

      IL_04f9:  ldloc.s    V_4
      IL_04fb:  ldc.i4.7
      IL_04fc:  bne.un.s   IL_0548

      IL_04fe:  ldc.i4.6
      IL_04ff:  newarr     [mscorlib]System.Object
      IL_0504:  stloc.s    V_11
      IL_0506:  ldloc.s    V_11
      IL_0508:  ldc.i4.0
      IL_0509:  ldarg.2
      IL_050a:  stelem.ref
      IL_050b:  ldloc.s    V_11
      IL_050d:  ldc.i4.1
      IL_050e:  ldstr      ":"
      IL_0513:  stelem.ref
      IL_0514:  ldloc.s    V_11
      IL_0516:  ldc.i4.2
      IL_0517:  ldarg.3
      IL_0518:  box        [mscorlib]System.Int32
      IL_051d:  stelem.ref
      IL_051e:  ldloc.s    V_11
      IL_0520:  ldc.i4.3
      IL_0521:  ldstr      ":"
      IL_0526:  stelem.ref
      IL_0527:  ldloc.s    V_11
      IL_0529:  ldc.i4.4
      IL_052a:  ldarg.s    column_number
      IL_052c:  box        [mscorlib]System.Int32
      IL_0531:  stelem.ref
      IL_0532:  ldloc.s    V_11
      IL_0534:  ldc.i4.5
      IL_0535:  ldstr      ": Jumps may not be used directly in a program."
      IL_053a:  stelem.ref
      IL_053b:  ldloc.s    V_11
      IL_053d:  call       string [mscorlib]System.String::Concat(object[])
      IL_0542:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_0547:  throw

      IL_0548:  ldarg.s    isMiniMaestro
      IL_054a:  brtrue.s   IL_05b6

      IL_054c:  ldloc.s    V_4
      IL_054e:  conv.u1
      IL_054f:  ldc.i4.s   50
      IL_0551:  blt.s      IL_05b6

      IL_0553:  ldc.i4.8
      IL_0554:  newarr     [mscorlib]System.Object
      IL_0559:  stloc.s    V_12
      IL_055b:  ldloc.s    V_12
      IL_055d:  ldc.i4.0
      IL_055e:  ldarg.2
      IL_055f:  stelem.ref
      IL_0560:  ldloc.s    V_12
      IL_0562:  ldc.i4.1
      IL_0563:  ldstr      ":"
      IL_0568:  stelem.ref
      IL_0569:  ldloc.s    V_12
      IL_056b:  ldc.i4.2
      IL_056c:  ldarg.3
      IL_056d:  box        [mscorlib]System.Int32
      IL_0572:  stelem.ref
      IL_0573:  ldloc.s    V_12
      IL_0575:  ldc.i4.3
      IL_0576:  ldstr      ":"
      IL_057b:  stelem.ref
      IL_057c:  ldloc.s    V_12
      IL_057e:  ldc.i4.4
      IL_057f:  ldarg.s    column_number
      IL_0581:  box        [mscorlib]System.Int32
      IL_0586:  stelem.ref
      IL_0587:  ldloc.s    V_12
      IL_0589:  ldc.i4.5
      IL_058a:  ldstr      ": "
      IL_058f:  stelem.ref
      IL_0590:  ldloc.s    V_12
      IL_0592:  ldc.i4.6
      IL_0593:  ldloc.s    V_4
      IL_0595:  box        Pololu.Usc.Bytecode.Opcode
      IL_059a:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_059f:  stelem.ref
      IL_05a0:  ldloc.s    V_12
      IL_05a2:  ldc.i4.7
      IL_05a3:  ldstr      " is only available on the Mini Maestro 12, 18, and"
      + " 24."
      IL_05a8:  stelem.ref
      IL_05a9:  ldloc.s    V_12
      IL_05ab:  call       string [mscorlib]System.String::Concat(object[])
      IL_05b0:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
      IL_05b5:  throw

      IL_05b6:  ldarg.1
      IL_05b7:  ldloc.s    V_4
      IL_05b9:  ldarg.2
      IL_05ba:  ldarg.3
      IL_05bb:  ldarg.s    column_number
      IL_05bd:  newobj     instance void Pololu.Usc.Bytecode.BytecodeInstruction::.ctor(valuetype Pololu.Usc.Bytecode.Opcode,
                                                                                        string,
                                                                                        int32,
                                                                                        int32)
      IL_05c2:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction)
      IL_05c7:  leave      IL_01e0

    }  // end .try
    catch [mscorlib]System.Collections.Generic.KeyNotFoundException 
    {
      IL_05cc:  pop
      IL_05cd:  ldarg.1
      IL_05ce:  ldarg.0
      IL_05cf:  ldarg.2
      IL_05d0:  ldarg.3
      IL_05d1:  ldarg.s    column_number
      IL_05d3:  call       class Pololu.Usc.Bytecode.BytecodeInstruction Pololu.Usc.Bytecode.BytecodeInstruction::newCall(string,
                                                                                                                          string,
                                                                                                                          int32,
                                                                                                                          int32)
      IL_05d8:  callvirt   instance void Pololu.Usc.Bytecode.BytecodeProgram::addInstruction(class Pololu.Usc.Bytecode.BytecodeInstruction)
      IL_05dd:  leave      IL_01e0
    }  // end handler
  } // end of method BytecodeReader::parseString

} // end of class Pololu.Usc.Bytecode.BytecodeReader

.class public auto ansi sealed Pololu.Usc.Bytecode.Keyword
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype Pololu.Usc.Bytecode.Keyword GOTO = int32(0x00000000)
  .field public static literal valuetype Pololu.Usc.Bytecode.Keyword SUB = int32(0x00000001)
  .field public static literal valuetype Pololu.Usc.Bytecode.Keyword BEGIN = int32(0x00000002)
  .field public static literal valuetype Pololu.Usc.Bytecode.Keyword WHILE = int32(0x00000003)
  .field public static literal valuetype Pololu.Usc.Bytecode.Keyword REPEAT = int32(0x00000004)
  .field public static literal valuetype Pololu.Usc.Bytecode.Keyword IF = int32(0x00000005)
  .field public static literal valuetype Pololu.Usc.Bytecode.Keyword ENDIF = int32(0x00000006)
  .field public static literal valuetype Pololu.Usc.Bytecode.Keyword ELSE = int32(0x00000007)
} // end of class Pololu.Usc.Bytecode.Keyword

.class public auto ansi sealed Pololu.Usc.Bytecode.Opcode
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode QUIT = int32(0x00000000)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode LITERAL = int32(0x00000001)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode LITERAL8 = int32(0x00000002)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode LITERAL_N = int32(0x00000003)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode LITERAL8_N = int32(0x00000004)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode RETURN = int32(0x00000005)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode JUMP = int32(0x00000006)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode JUMP_Z = int32(0x00000007)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode DELAY = int32(0x00000008)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode GET_MS = int32(0x00000009)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode DEPTH = int32(0x0000000A)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode DROP = int32(0x0000000B)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode DUP = int32(0x0000000C)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode OVER = int32(0x0000000D)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode PICK = int32(0x0000000E)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode SWAP = int32(0x0000000F)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode ROT = int32(0x00000010)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode ROLL = int32(0x00000011)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode BITWISE_NOT = int32(0x00000012)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode BITWISE_AND = int32(0x00000013)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode BITWISE_OR = int32(0x00000014)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode BITWISE_XOR = int32(0x00000015)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode SHIFT_RIGHT = int32(0x00000016)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode SHIFT_LEFT = int32(0x00000017)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode LOGICAL_NOT = int32(0x00000018)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode LOGICAL_AND = int32(0x00000019)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode LOGICAL_OR = int32(0x0000001A)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode NEGATE = int32(0x0000001B)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode PLUS = int32(0x0000001C)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode MINUS = int32(0x0000001D)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode TIMES = int32(0x0000001E)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode DIVIDE = int32(0x0000001F)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode MOD = int32(0x00000020)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode POSITIVE = int32(0x00000021)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode NEGATIVE = int32(0x00000022)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode NONZERO = int32(0x00000023)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode EQUALS = int32(0x00000024)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode NOT_EQUALS = int32(0x00000025)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode MIN = int32(0x00000026)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode MAX = int32(0x00000027)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode LESS_THAN = int32(0x00000028)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode GREATER_THAN = int32(0x00000029)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode SERVO = int32(0x0000002A)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode SERVO_8BIT = int32(0x0000002B)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode SPEED = int32(0x0000002C)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode ACCELERATION = int32(0x0000002D)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode GET_POSITION = int32(0x0000002E)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode GET_MOVING_STATE = int32(0x0000002F)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode LED_ON = int32(0x00000030)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode LED_OFF = int32(0x00000031)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode PWM = int32(0x00000032)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode PEEK = int32(0x00000033)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode POKE = int32(0x00000034)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode SERIAL_SEND_BYTE = int32(0x00000035)
  .field public static literal valuetype Pololu.Usc.Bytecode.Opcode CALL = int32(0x00000036)
} // end of class Pololu.Usc.Bytecode.Opcode


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file Bytecode.res
