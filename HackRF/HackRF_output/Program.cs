using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackRF;
using SDRSharp.Radio;

namespace HackRF_output
{
    unsafe class Program : IFrontendController
    {
        private static HackRF_Controller Controller;
        private static SDRSharp.Radio.SamplesAvailableDelegate availableDelegate;

        public bool IsSoundCardBased => throw new NotImplementedException();

        public string SoundCardHint => throw new NotImplementedException();

        public double Samplerate => throw new NotImplementedException();

        public long Frequency { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        static void Main(string[] args)
        {
            Controller = new HackRF_Controller();
            // Console.WriteLine(Controller.Board_ID);

            Controller.LNAGain = 10;
            Controller.SampleRate = 1024000;
            Controller.Frequency = 137100000;
            Controller.SamplesAvailable += Controller_SamplesAvailable;
            Console.ReadKey();

        }

        private static void Controller_SamplesAvailable(object sender, SamplesAvailableEventArgs samps)
        {
            Program program = new Program();
            availableDelegate(program, samps.Buffer, samps.Length);
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void Start(SamplesAvailableDelegate callback)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void ShowSettingGUI(System.Windows.Forms.IWin32Window parent)
        {
            throw new NotImplementedException();
        }

        public void HideSettingGUI()
        {
            throw new NotImplementedException();
        }
    }
}
