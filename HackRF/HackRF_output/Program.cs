using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackRF;
using SDRSharp.Radio;
using System.IO;

namespace HackRF_output
{
    unsafe class Program
    {
        private static HackRF_Controller Controller;
        private static SamplesAvailableDelegate SamplesAvailableDelegate;

        private static ComplexFifoStream iqStream;
        private static double SampleRate = 8000000;
        private static int _droppedBuffers;

        private static Complex* _rxBufferPtr;
        private static UnsafeBuffer _rxBuffer;
        private static UnsafeBuffer _txBuffer;
        private static bool TxMode;

        static void Main(string[] args)
        {
            if (_rxBuffer == null)
            {
                _rxBuffer = UnsafeBuffer.Create((int)SampleRate, sizeof(Complex));
                _rxBufferPtr = (Complex*)_rxBuffer;
            }
            Controller = new HackRF_Controller();
            Console.Title = "HackRF Samples View";
            Console.ReadKey();



            Controller.SampleRate = SampleRate;
            Controller.Frequency = 100000000;

            Controller.VGAGain = 40;
            Controller.LNAGain = 24;


            //if (TxMode)
            //{
            //    //FileStream fs = new FileStream(filename, FileMode.Open);
            //    byte[] array = new byte[fs.Length];
            //    fs.Read(array, 0, array.Length);
            //    sbyte[] iqArray = Array.ConvertAll(array, b => unchecked((sbyte)b));
            //    // TO DO Convert iqArray To ComplexFifoStream
            //    Controller.StartTx();
            //}

            while (true)
            {
                Controller.SamplesAvailable += Controller_SamplesAvailable;
            }

        }

        private static void Controller_SamplesAvailable(object sender, SamplesAvailableEventArgs samps)
        {

            // IF RX Mode
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
                iqStream.Read(_rxBufferPtr, (int)SampleRate);

                for (int i = 0; i < SampleRate; i++)
                {
                    Console.WriteLine(String.Format("{0:0.000000}\t{1:0.000000}", _rxBufferPtr[i].Real, _rxBufferPtr[i].Imag));
                    //Console.WriteLine($"{_bufferPtr[i].Imag}           {_bufferPtr[i].Real}");
                }
            }

            // TO DO if TX Mode
            //if (TXmode)
            //{
            //    samps.Buffer =
            //}
        }

        //private static void ReadSFile(string filename, out sbyte[] iqArray)
        //{
            
        //}

    } 
}
