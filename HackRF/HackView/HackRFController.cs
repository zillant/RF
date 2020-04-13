using System;


namespace HackView
{
    public class HackRFController
    {
        private int res;
        private IntPtr dev;
        /// <summary>
        /// 
        ///
        /// </summary>
        public void Init()
        {
            res = NativeMethods.hackrf_init();
            if (res != 0) throw new ApplicationException("Cannot init HackRF");
            res = NativeMethods.hackrf_open(out dev);
            if (res != 0) throw new ApplicationException("Cannot open HackRF");
        }

        public void Config(long freq, double SampleRate, bool AmpEnable, uint lnaGain, uint vgaGain, bool basebandFilterEnable)
        {
            res = NativeMethods.hackrf_set_freq(dev, freq);
            res += NativeMethods.hackrf_set_sample_rate(dev, SampleRate);
            res += NativeMethods.hackrf_set_amp_enable(dev, AmpEnable ? (byte)1 : (byte)0);
            res += NativeMethods.hackrf_set_lna_gain(dev, lnaGain);
            res += NativeMethods.hackrf_set_vga_gain(dev, vgaGain);
            res += NativeMethods.hackrf_set_baseband_filter_bandwidth(dev, (uint)NativeMethods.hackrf_compute_baseband_filter_bw_round_down_lt((uint)SampleRate));
            if (res != 0) throw new ApplicationException("Configuration error. Cannot set parameters");
        }
    }
}
