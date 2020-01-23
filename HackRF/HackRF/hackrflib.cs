using System;
using System.Runtime.InteropServices;

namespace HackRF
{
    public class hackrflib
    {
        private const string lib = "hackrf";
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_init();

        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_exit();


    }
}
