using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HackRF;
using SDRSharp.Radio;

namespace HackRF
{
    public unsafe class HackRF_Controller
    {
        private IntPtr _device;
        public int HackRF_Result { get; set; }
        public string Board_ID { get; private set; }
        public bool isStreaming {private get; set; }
        
        public event SamplesAvailableDelegate SamplesAvailable;

        private readonly SamplesAvailableEventArgs _eventArgs = new SamplesAvailableEventArgs();

        private GCHandle _gcHandle;
        private hackrf_sample_block_cb_fn _HackRFCallback = HackRFSampleAvailable;

        private static readonly UnsafeBuffer _lutBuffer = UnsafeBuffer.Create(256, sizeof(float));

        private UInt32 _lnaGain;
        private uint _vgaGain;

        private double _sampleRate = 2048000;
        private bool _amp;

        private UnsafeBuffer _iqBuffer;
        private long _Frequency;

        private Complex* _iqPtr;
        private static readonly float* _lutPtr;

        public delegate void SamplesAvailableDelegate(object sender, SamplesAvailableEventArgs samps);

        static HackRF_Controller()
        {
            _lutPtr = (float*)_lutBuffer;

            const float scale = 1.0f / 127.0f;
            for (var i = 0; i < 256; i++)
            {
                _lutPtr[i] = (i - 128) * scale;
            }
        }
        public HackRF_Controller()
        {
            var res = hackrflib.hackrf_init();
            if (res != 0) throw new ApplicationException("Cannot init HackRF");

            res = hackrflib.hackrf_open(out _device);
            if (res != 0) throw new ApplicationException("Cannot open HackRF");

            _gcHandle = GCHandle.Alloc(this);
        }

        public void Dispose()
        {
            if (isStreaming)
            {
                hackrflib.hackrf_close(_device);
                hackrflib.hackrf_exit();
            }
           if (_gcHandle.IsAllocated) _gcHandle.Free();
            GC.SuppressFinalize(this);
        }

        private int HackRFSamplesAvailable(hackrf_transfer* samplesPtr )
        {
                        
            return 0;
        }

        public void StopRx()
        {
            if (!isStreaming) return;
            hackrflib.hackrf_stop_rx(_device);
        }

        public void StopTx()
        {
            if (!isStreaming) return;
            hackrflib.hackrf_stop_tx(_device, _HackRFCallback, (IntPtr)_gcHandle);
        }

        public void StartRx()
        {
            var r = hackrflib.hackrf_start_rx(_device, _HackRFCallback, (IntPtr)_gcHandle);
            if (r != 0) throw new ApplicationException("hackrf_start_rx() error");
        }

        public void StartTx()
        {
            var r = hackrflib.hackrf_start_tx(_device, _HackRFCallback, (IntPtr)_gcHandle);
            //if (r != 0) throw new ApplicationException("hackrf_start_tx() error");
        }

        public UInt32 LNAGain
        {
            get { return _lnaGain; }
            set
            {
                _lnaGain = value;
                if (_device != IntPtr.Zero)
                {
                    hackrflib.hackrf_set_lna_gain(_device, _lnaGain);
                }
            }
        }

        public uint VGAGain
        {
            get { return _vgaGain; }
            set
            {
                _vgaGain = value;
                if (_device != IntPtr.Zero)
                {
                    hackrflib.hackrf_set_vga_gain(_device, _vgaGain);
                }
            }
        }

        public bool EnableAmp
        {
            get { return _amp; }
            set
            {
                _amp = value;
                if (_device != IntPtr.Zero)
                {
                    hackrflib.hackrf_set_amp_enable(_device, (byte)(_amp ? 1 : 0));
                }
            }
        }

        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                _sampleRate = value;
                if (_device != IntPtr.Zero)
                {
                    hackrflib.hackrf_set_sample_rate(_device, _sampleRate);
                }
            }
        }

        public long Frequency
        {
            get { return _Frequency; }
            set
            {
                _Frequency = value;
                if (_device != IntPtr.Zero)
                {
                    hackrflib.hackrf_set_freq(_device, _Frequency);
                }
            }
        }

        public bool IsStreaming
        {
            get { return isStreaming; }
        }

        private void ComplexSamplesAvailable(Complex* buffer, int length)
        {
            if (SamplesAvailable != null)
            {
                _eventArgs.Buffer = buffer;
                _eventArgs.Length = length;
                SamplesAvailable(this, _eventArgs);
            }
        }

        private static int HackRFSampleAvailable(hackrf_transfer* ptr)
        {
            byte* buf = ptr->buffer;
            int len = ptr->buffer_length;
            IntPtr ctx = ptr->rx_ctx;

            var gcHandle = GCHandle.FromIntPtr(ctx);

            if (!gcHandle.IsAllocated) return -1;

            var instance = (HackRF_Controller)gcHandle.Target;

            var sampleCount = (int)len / 2;
            if (instance._iqBuffer == null || instance._iqBuffer.Length != sampleCount)
            {
                instance._iqBuffer = UnsafeBuffer.Create(sampleCount, sizeof(Complex));
                instance._iqPtr = (Complex*)instance._iqBuffer;
            }

            var ptrIq = instance._iqPtr;
            for (var i = 0; i < sampleCount; i++)
            {
                ptrIq->Imag = _lutPtr[*buf++];
                ptrIq->Real = _lutPtr[*buf++];
                ptrIq++;
            }

            instance.ComplexSamplesAvailable(instance._iqPtr, instance._iqBuffer.Length);

            return 0;
        }
    }

    public unsafe sealed class SamplesAvailableEventArgs : EventArgs
    {
        public int Length { get; set; }
        public Complex* Buffer { get; set; }
    }
}
