using System;
using System.Runtime.InteropServices;

namespace HackRF
{
    public unsafe struct hackrf_transfer
    {
        public IntPtr device;
        public byte* buffer;
        public int buffer_length;
        public int valid_length;
        public IntPtr rx_ctx;
        public IntPtr tx_ctx;
    }

    public unsafe delegate int hackrf_sample_block_cb_fn(hackrf_transfer* ptr);
    public static class hackrflib
    {
        private const string lib = "hackrf";

        /// <summary>
        /// Initialize libHackRF, including global libUSB context to support multiple HackRF hardware devices.
        /// </summary>
        /// <returns>HackRF status </returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_init();

        /// <summary>
        /// Cleanly shutdown libHackRF and the underlying USB context. This does not stop in progress transfers or close the HackRF hardware.
        /// hackrf_close() should be called before this to cleanly close the connection to the hardware.
        /// </summary>
        /// <returns>HackRF status</returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_exit();

        /// <summary>
        /// Device connection
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <returns>HackRF status</returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_open(out IntPtr dev);

        /// <summary>
        /// Close USB connection with HackRF
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_close(IntPtr dev);
        
        /// <summary>
        /// Setting Sample rate
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <param name="rate">sample rate</param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_set_sample_rate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_sample_rate(IntPtr dev, double rate);

        /// <summary>
        /// Setting cental frequency
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <param name="freq">cenral freq</param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_set_freq", CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_freq(IntPtr dev, long freq);

        /// <summary>
        /// Toggle the antenna port power for external amplifiers.
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <param name="value">(byte) 1 -> antenna on, 0 -> antenna off </param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_set_amp_enable", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_amp_enable(IntPtr dev, byte value);

        /// <summary>
        /// Setting baseband filter on HackRF
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <param name="bandwidth_hz">Filter bandwidth in Hz</param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_set_baseband_filter_bandwidth", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_baseband_filter_bandwidth(IntPtr dev, uint bandwidth_hz);

        /// <summary>
        /// Compute nearest freq for bw filter (manual filter)
        /// </summary>
        /// <param name="bandwidth_hz">Filter bandwidth in Hz</param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_compute_baseband_filter_bw_round_down_lt", CallingConvention = CallingConvention.StdCall)]
        public static extern uint hackrf_compute_baseband_filter_bw_round_down_lt(uint bandwidth_hz);

        /// <summary>
        /// Compute best default value depending on sample rate (auto filter).
        /// </summary>
        /// <param name="bandwidth_hz"></param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_compute_baseband_filter_bw", CallingConvention = CallingConvention.StdCall)]
        public static extern uint hackrf_compute_baseband_filter_bw(uint bandwidth_hz);

        /// <summary>
        /// Set LNA Gain, Range 0-40 (step 8dB)
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <param name="value">LNA gaiun in dB</param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_set_lna_gain", CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_set_lna_gain(IntPtr dev, UInt32 value);
        
        /// <summary>
        /// Set VGA gain. Range 0-62 (step 2dB)
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <param name="value">VGA gain in dB</param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_set_vga_gain", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_vga_gain(IntPtr dev, uint value);

        /// <summary>
        /// Start Rx
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <param name="sample_block">Buffer struct ptr</param>
        /// <param name="rx_ctx"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_start_rx(
            IntPtr dev,
            hackrf_sample_block_cb_fn sample_block,
            IntPtr rx_ctx);

        /// <summary>
        /// Stop Rx
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_stop_rx(IntPtr dev);

        /// <summary>
        /// Check whether or not the HackRF device is currently streaming samples, either to or from the host system.
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_is_streaming(IntPtr dev);

        //[DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        //private static extern IntPtr hackrf_board_id_name_native(uint index);
        
        ///// <summary>
        ///// read HackRF ID
        ///// </summary>
        ///// <param name="index"></param>
        ///// <returns></returns>
        //public static string hackrf_board_id_name(uint index)
        //{
        //    try
        //    {
        //        var strptr = hackrf_board_id_name_native(index);
        //        return Marshal.PtrToStringAnsi(strptr);
        //    }
        //    catch (EntryPointNotFoundException e)
        //    {
        //        Console.WriteLine("{0}:\n   {1}", e.GetType().Name, e.Message);
        //        return "HackRF";
        //    }
        //}

        /// <summary>
        /// Set Tx VGA gain. Range 0-47 (step 1dB)
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr hackrf_set_txvga_gain(IntPtr dev, uint value);

        /// <summary>
        /// Start Tx
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <param name="sample_block">Buffer struct ptr</param>
        /// <param name="tx_ctx"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr hackrf_start_tx(IntPtr dev, hackrf_sample_block_cb_fn sample_block, IntPtr tx_ctx);

        /// <summary>
        /// Stop Tx
        /// </summary>
        /// <param name="dev">device ptr</param>
        /// <param name="sample_block">Buffer struct ptr</param>
        /// <param name="tx_ctx"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr hackrf_stop_tx(IntPtr dev, hackrf_sample_block_cb_fn sample_block, IntPtr tx_ctx);



    }
}

