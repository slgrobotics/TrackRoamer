using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

/*
 * C# Wrapper around the Oscilloscope DLL
 * 
 * (C)2006 Dustin Spicuzza
 *
* This library interface is free software; you can redistribute it and/or
* modify it under the terms of the GNU Lesser General Public
* License as published by the Free Software Foundation; only
* version 2.1 of the License.
* 
* Some comments/function declarations taken from the original "oscilloscope-lib" documentation
*
 */

// regsvr32 Osc_DLL.dll

namespace LibOscilloscope
{
    public sealed class Oscilloscope : IDisposable
    {
        #region External declarations

        /*
 *	C-style function declarations
 * 
int (__cdecl * AtOpenLib) (int Prm);
int (__cdecl * ScopeCreate) (int Prm ,  char  * P_IniName,  char * P_IniSuffix);
int (__cdecl * ScopeDestroy) (int ScopeHandle);
int (__cdecl * ScopeShow) (int ScopeHandle);
int (__cdecl * ScopeHide) (int ScopeHandle);
int (__cdecl * ScopeCleanBuffers) (int ScopeHandle);
int (__cdecl * ShowNext) (int ScopeHandle, double * PArrDbl);
int (__cdecl * ExternalNext) (int ScopeHandle, double * PDbl);
int (__cdecl * QuickUpDate) (int ScopeHandle);
 */
        [DllImport("Osc_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int AtOpenLib
            (int Prm);

        [DllImport("Osc_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ScopeCreate(
            int Prm,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder P_IniName,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder P_IniSuffix);

        [DllImport("Osc_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ScopeDestroy
            (int ScopeHandle);

        [DllImport("Osc_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ScopeShow
            (int ScopeHandle);

        [DllImport("Osc_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ScopeHide
            (int ScopeHandle);

        [DllImport("Osc_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ScopeCleanBuffers
            (int ScopeHandle);

        [DllImport("Osc_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ShowNext
            (int ScopeHandle,
            double[] PArrDbl);

        [DllImport("Osc_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ExternalNext
            (int ScopeHandle,
            ref double PDbl);

        [DllImport("Osc_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int QuickUpDate
            (int ScopeHandle);

        #endregion

        #region Static members
        static bool initialized = false;

        /// <summary>
        /// Creates a new Oscilloscope. Returns the object if successful,
        /// otherwise returns null. Generally, if it returns null then
        /// it cannot find the correct DLL to load
        /// </summary>
        /// <returns>Oscilloscope instance</returns>
        public static Oscilloscope Create()
        {
            return Create(null, null);
        }

        /// <summary>
        /// Creates a new Oscilloscope. Returns the object if successful,
        /// otherwise returns null. Generally, if it returns null then
        /// it cannot find the correct DLL to load
        /// </summary>
        /// <param name="IniName">Name of INI file with scope settings</param>
        /// <param name="IniSuffix">Section name suffix (see manual)</param>
        /// <returns>Oscilloscope instance</returns>
        public static Oscilloscope Create(string IniName, string IniSuffix)
        {

            int handle;

            try
            {
                if (!initialized)
                {
                    // initialize
                    if (AtOpenLib(0) == -1)
                        return null;
                    // set to true
                    initialized = true;
                }

            }
            catch
            {
                // return 
                return null;
            }

            // create the scope
            handle = ScopeCreate(0, new StringBuilder(IniName), new StringBuilder(IniSuffix));

            if (handle != 0)
            {
                return new Oscilloscope(handle);
            }

            return null;
        }

        #endregion

        int scopeHandle;
        bool _disposed = false;


        private Oscilloscope()
        {
        }

        private Oscilloscope(int handle)
        {
            scopeHandle = handle;
        }

        ~Oscilloscope()
        {
            Dispose();
        }

        /// <summary>
        /// Shows the scope
        /// </summary>
        public void Show()
        {
            if (!_disposed)
                ScopeShow(scopeHandle);
        }

        /// <summary>
        /// Hides the scope from view
        /// </summary>
        public void Hide()
        {
            if (!_disposed)
                ScopeHide(scopeHandle);
        }

        /// <summary>
        /// Clears the buffer of the scope
        /// </summary>
        public void Clear()
        {
            if (!_disposed)
                ScopeCleanBuffers(scopeHandle);
        }

        /// <summary>
        /// Add data to the scope
        /// </summary>
        /// <param name="beam1">Data for first beam</param>
        /// <param name="beam2">Data for second beam</param>
        /// <param name="beam3">Data for third beam</param>
        public void AddData(double beam1, double beam2, double beam3)
        {
            if (!_disposed)
            {

                double[] PArrDbl = new double[3];
                PArrDbl[0] = beam1;
                PArrDbl[1] = beam2;
                PArrDbl[2] = beam3;

                ShowNext(scopeHandle, PArrDbl);
            }
        }

        /// <summary>
        /// Add data to the 'external' trigger function signal
        /// </summary>
        /// <param name="data">The data</param>
        public void AddExternalData(double data)
        {
            if (!_disposed)
                ExternalNext(scopeHandle, ref data);
        }

        /// <summary>
        /// Quickly refreshes screen of oscilloscope. Calling this function is
        /// not usually required. Recommended for using in situations when 
        /// intensive data stream is going into oscilloscope
        /// </summary>
        public void Update()
        {
            if (!_disposed)
            {
                QuickUpDate(scopeHandle);
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                ScopeDestroy(scopeHandle);
                _disposed = true;
            }
        }

        /// <summary>
        /// True if object is already disposed
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return _disposed;
            }
        }

        #endregion
    }
}
