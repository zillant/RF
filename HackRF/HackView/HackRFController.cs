using HackRF;
using System;


namespace HackView
{
    class HackRFController
    {
        private int res;
        private IntPtr dev;
        /// <summary>
        /// 
        ///
        /// </summary>
        public void Init()
        {
            res = hackrflib.hackrf_init();
            if (res != 0) throw new ApplicationException("Cannot init HackRF");
            res = hackrflib.hackrf_open(out dev);
            if (res != 0) throw new ApplicationException("Cannot open HackRF");
        }
    }
}
