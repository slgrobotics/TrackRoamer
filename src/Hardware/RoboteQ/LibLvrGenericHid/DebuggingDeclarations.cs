using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices; 
using System.Windows.Forms;

/*
The following code is adapted from "generichid_cs_46" USB application example by Jan Axelson
for more information see see http://www.lvr.com/hidpage.htm
*/

namespace LibLvrGenericHid
{   
    public sealed partial class Debugging  
    {         
        internal const Int16 FORMAT_MESSAGE_FROM_SYSTEM = 0X1000;         
       
        [ DllImport( "kernel32.dll", CharSet=CharSet.Auto, SetLastError=true ) ]
        internal static extern Int32 FormatMessage( Int32 dwFlags, ref Int64 lpSource, Int32 dwMessageId, Int32 dwLanguageZId, String lpBuffer, Int32 nSize, Int32 Arguments );        
    } 
} 
