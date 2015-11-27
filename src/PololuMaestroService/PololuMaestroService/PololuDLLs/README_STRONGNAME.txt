
When updating Pololu DLLs, you cannot just copy new DLLs from, say, pololu_usb_sdk_120712 here.
These DLLs are not signed, and will not compile in MRDS.

You need to:
	1. Update the DLLs in the "orig" folder
	2. In the VS 2010 command prompt run reassemble.bat - uncomment the part which is producing .il files
	3. edit Usc.il - insert line ".publickeytoken = (35 29 79 22 70 4B 28 CD )" into references to UsbWrapper, Bytecode and Sequencer (which are now signed):

			.assembly extern UsbWrapper
			{
			  .publickeytoken = (35 29 79 22 70 4B 28 CD )
			  .ver 1:5:0:0
			}
			.....
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

	4. run reassemble.bat (comment out parts that disassemble DLLs into .ils).

Then you might need to delete and reassign references to these DLLs in the project.

Best luck!

see http://scmay.wordpress.com/2008/07/17/assembly-generation-failed-referenced-assembly-does-not-have-a-strong-name/
see http://www.geekbeing.com/2010/01/30/sign-me-up-scotty-signing-managed-assemblies-with-ildasm-and-ilasm-duo


