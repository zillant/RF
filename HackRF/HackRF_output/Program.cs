using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackRF;
using SDRSharp.Radio;

namespace HackRF_output
{
    unsafe class Program
    {
        private static HackRF_Controller Controller;
        private static SamplesAvailableDelegate SamplesAvailableDelegate;

        private static ComplexFifoStream iqStream;
        private static double SampleRate = 8000000;
        private static int _droppedBuffers;

        private static Complex* _bufferPtr;
        private static UnsafeBuffer _buffer;

        static void Main(string[] args)
        {
            if (_buffer == null)
            {
                _buffer = UnsafeBuffer.Create((int)SampleRate, sizeof(Complex));
                _bufferPtr = (Complex*)_buffer;
            }
            Controller = new HackRF_Controller();
            Console.Title = "HackRF Samples View";
            Console.ReadKey();

      
            
            Controller.SampleRate = SampleRate;
            Controller.Frequency = 100000000;

            Controller.VGAGain = 40;
            Controller.LNAGain = 24;

            Controller.StartRx();
           

            while (true)
            {
                Controller.SamplesAvailable += Controller_SamplesAvailable;
                
            }

        }

        private static void Controller_SamplesAvailable(object sender, SamplesAvailableEventArgs samps)
        {

            // TO DO поднять комплексный буффер
            if (iqStream == null) iqStream = new ComplexFifoStream(BlockMode.None);

            if (iqStream.Length < SampleRate)
            {
                iqStream.Write(samps.Buffer, samps.Length);
            }
            else
            {
                _droppedBuffers++;
            }

            if (iqStream != null)
            {
                iqStream.Read(_bufferPtr, (int)SampleRate);

                for (int i = 0; i < SampleRate; i++)
                {
                    Console.WriteLine(String.Format("{0:0.000000}\t{1:0.000000}", _bufferPtr[i].Real, _bufferPtr[i].Imag));
                    //Console.WriteLine($"{_bufferPtr[i].Imag}           {_bufferPtr[i].Real}");
                }
            }

        }

    }
}
