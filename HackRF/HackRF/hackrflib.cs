using System;
using System.Runtime.InteropServices;

namespace HackRF
{
    public unsafe struct hackrf_transfer
    {
        public IntPtr device;
        public byte* buffer;
        public int valid_length;
        public IntPtr rx_ctx;
        public IntPtr tx_ctx;
    }

    public unsafe delegate int hackrf_sample_block_cb_fn(hackrf_transfer* ptr);
    public class hackrflib
    {
        private const string lib = "hackrf";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_init();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_exit();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_open(out IntPtr dev);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_close(IntPtr dev);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_set_sample_rate", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_sample_rate(IntPtr dev, double rate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="freq"></param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_set_freq", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_freq(IntPtr dev, long freq);

        [DllImport(lib, EntryPoint = "hackrf_set_amp_enable", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_amp_enable(IntPtr dev, byte value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="bandwidth_hz"></param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_set_baseband_filter_bandwidth", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_baseband_filter_bandwidth(IntPtr dev, uint bandwidth_hz);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bandwidth_hz"></param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_compute_baseband_filter_bw_round_down_lt", CallingConvention = CallingConvention.StdCall)]
        public static extern uint hackrf_compute_baseband_filter_bw_round_down_lt(uint bandwidth_hz);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bandwidth_hz"></param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_compute_baseband_filter_bw", CallingConvention = CallingConvention.StdCall)]
        public static extern uint hackrf_compute_baseband_filter_bw(uint bandwidth_hz);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_set_lna_gain", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_lna_gain(IntPtr dev, uint value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(lib, EntryPoint = "hackrf_set_vga_gain", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_vga_gain(IntPtr dev, uint value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="sample_block"></param>
        /// <param name="rx_ctx"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_start_rx(
            IntPtr dev,
            hackrf_sample_block_cb_fn sample_block,
            IntPtr rx_ctx);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]

        public static extern int hackrf_stop_rx(IntPtr dev);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hackrf_is_streaming(IntPtr dev);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr hackrf_board_id_name_native(uint index);
        
        public static string hackrf_board_id_name(uint index)
        {
            try
            {
                var strptr = hackrf_board_id_name_native(index);
                return Marshal.PtrToStringAnsi(strptr);
            }
            catch (EntryPointNotFoundException e)
            {
                Console.WriteLine("{0}:\n   {1}", e.GetType().Name, e.Message);
                return "HackRF";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr hackrf_set_txvga_gain(IntPtr dev, uint value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="sample_block"></param>
        /// <param name="tx_ctx"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr hackrf_start_tx(IntPtr dev, hackrf_sample_block_cb_fn sample_block, IntPtr tx_ctx);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="sample_block"></param>
        /// <param name="tx_ctx"></param>
        /// <returns></returns>
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr hackrf_stop_tx(IntPtr dev, hackrf_sample_block_cb_fn sample_block, IntPtr tx_ctx);



    }
}

