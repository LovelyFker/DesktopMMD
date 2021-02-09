using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class WinDllImport
{
    [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr handle, String message, String title, int type);
}
