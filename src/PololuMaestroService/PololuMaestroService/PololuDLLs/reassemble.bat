
REM  When updating Pololu DLLs, you cannot just copy new DLLs from, say, pololu_usb_sdk_120712 here.
REM  These DLLs are not signed, and will not compile in MRDS.
REM  You need to:
REM  	1. Update the DLLs in the "orig" folder
REM  	2. In the VS 2010 command prompt run reassemble.bat - uncomment the part which is producing .il files
REM  	3. edit Usc.il - insert line ".publickeytoken = (35 29 79 22 70 4B 28 CD )" into references to UsbWrapper, Bytecode and Sequencer (which are now signed)
REM          (see README_STRONGNAME.txt)
REM  	4. run reassemble.bat (comment out parts that disassemble DLLs into .ils).
REM  Then you might need to delete and reassign references to these DLLs in the project.


REM  see http://scmay.wordpress.com/2008/07/17/assembly-generation-failed-referenced-assembly-does-not-have-a-strong-name/
REM  see http://www.geekbeing.com/2010/01/30/sign-me-up-scotty-signing-managed-assemblies-with-ildasm-and-ilasm-duo

REM sn -k sgKey.snk

REM uncomment the following to produce il files:
REM ildasm orig/Sequencer.dll /out:Sequencer.il
REM ildasm orig/Bytecode.dll /out:Bytecode.il
REM ildasm orig/UsbWrapper.dll /out:UsbWrapper.il
REM ildasm orig/Usc.dll /out:Usc.il

ilasm Sequencer.il /res:Sequencer.res /dll /key:sgKey.snk /out:Sequencer.dll

ilasm Bytecode.il /res:Bytecode.res /dll /key:sgKey.snk /out:Bytecode.dll

ilasm UsbWrapper.il /res:UsbWrapper.res /dll /key:sgKey.snk /out:UsbWrapper.dll

ilasm Usc.il /res:Usc.res /dll /key:sgKey.snk /out:Usc.dll

PAUSE

