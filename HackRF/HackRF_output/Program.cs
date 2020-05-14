using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        private static Complex* _rxBufferPtr;
        private static UnsafeBuffer _rxBuffer;
        private static UnsafeBuffer _txBuffer;
        private static HackRFmode Mode;
        private static CancellationTokenSource cancelTokenSource;
        private static CancellationToken token;

        private static sFile RxFile;


        static void Main(string[] args)
        {
            if (_rxBuffer == null)
            {
                _rxBuffer = UnsafeBuffer.Create((int)SampleRate, sizeof(Complex));
                _rxBufferPtr = (Complex*)_rxBuffer;
            }
                        
            Controller = new HackRF_Controller();

            Console.Title = "HackRF Samples View";
            Console.WriteLine("Hello HackRF User!");
            Console.WriteLine("Нажмите ECS для отмены");

            Controller.SampleRate = SampleRate;
            Controller.Frequency = 100000000;

            Controller.VGAGain = 40;
            Controller.LNAGain = 24;

            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;

            //:TODO логика выбора y
            Mode = HackRFmode.RX;

            if (Mode == HackRFmode.RX)
            {
                RxFile = new sFile("TxFile.s");
                RxFile.Open();
                Controller.StartRx();
                Task ReceivingSamples = new Task(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Прием остановлен");
                        return;
                    }

                    Controller.SamplesAvailable += Controller_SamplesAvailable;

                });

                ReceivingSamples.Start();
            }

            Console.CancelKeyPress += Console_CancelKeyPress;
            while (true)
            {
             
                if (iqStream != null)
                {
                    iqStream.Read(_rxBufferPtr, (int)SampleRate);
                    sbyte[] iBuffer = new sbyte[(int)SampleRate];
                    sbyte[] qBuffer = new sbyte[(int)SampleRate];

                    for (int i = 0; i < iqStream.Length; i++)
                    {
                        iBuffer[i] = FromFloatToSbyte(_rxBufferPtr[i].Real);
                        qBuffer[i] = FromFloatToSbyte(_rxBufferPtr[i].Imag);
                    }
                    RxFile.WriteFile(iBuffer,qBuffer);
                    Console.WriteLine(iBuffer.Length.ToString());
                }
            }

            //Console.Read();

            //if (TxMode)
            //{
            //    //FileStream fs = new FileStream(filename, FileMode.Open);
            //    byte[] array = new byte[fs.Length];
            //    fs.Read(array, 0, array.Length);
            //    sbyte[] iqArray = Array.ConvertAll(array, b => unchecked((sbyte)b));
            //    // TO DO Convert iqArray To ComplexFifoStream
            //    Controller.StartTx();
            //}


        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            cancelTokenSource.Cancel();
            RxFile.Close();
        }

        private static sbyte FromFloatToSbyte(float sample)
        {
            sample *= 127;
            if (sample > 127) sample = 127;
            else if (sample < -127) sample = -127;
            return (sbyte)sample;
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




            // TO DO if TX Mode
            //if (TXmode)
            //{
            //    samps.Buffer =
            //}
        }
    }

    internal enum HackRFmode
    {
        TX,
        RX
    }
}
