using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackRF_output
{
     public unsafe class sFile
    {
        private FileStream fs;
        private BinaryWriter _bw;
        private BinaryReader _br;
        private byte[] iqByte;
        private sbyte[] iqSbyte;
        private sbyte[] iSamples;
        private sbyte[] qSamples;
        private Complex* txBufferPtr;
        private UnsafeBuffer txBuffer;
        private const long MaxStreamLength = 2147483648;
        private readonly string _fileName;
        
        public sFile(string filename)
        {
            _fileName = filename;
        }

        public void Open()
        {
            if (_bw == null)
            {
                _bw = new BinaryWriter(File.Create(_fileName));
            }


        }

        public void Close()
        {
            if (_bw != null)
            {
                _bw.Flush();
                _bw.Close();
                _bw = null;
            }
        }

        public Complex* ReadFile(int bufferLength)
        {

            if (_br == null)
            {
                _br = new BinaryReader(File.Open(_fileName, FileMode.Open));
            }
            
            if (txBuffer == null)
            {
                txBuffer = UnsafeBuffer.Create(bufferLength, sizeof(Complex));
                txBufferPtr = (Complex*)txBuffer;
            }
            var length = (int)_br.BaseStream.Length;

            iSamples = new sbyte[length / 2];
            qSamples = new sbyte[length / 2];

            for (int i = 0; i < length / 2; i++)
            {
                iSamples[i] = _br.ReadSByte();
                qSamples[i] = _br.ReadSByte();
            }

            for (int i = 0; i < length / 2; i++)
            {
                txBufferPtr[i].Real = iSamples[i];
                txBufferPtr[i].Imag = qSamples[i];
            }            

            return txBufferPtr;
        }

        public void WriteFile(sbyte[] iArray, sbyte[] qArray)
        {
            int length;
            
            if (iArray.Length == qArray.Length)
            {
                length = iArray.Length + qArray.Length;
            }
            else length = 0;

            sbyte[] iqArray = new sbyte[length * 2];

            var sample = 0;

            for (int i = 0; i < length; i++)
            {
                iqArray[i] = iArray[sample];
                i++;
                iqArray[i] = qArray[sample];
                sample++;
            }

            iqByte = Array.ConvertAll(iqArray, b => unchecked((byte)b));

            if (_bw != null)
            {
                int toWrite = (int)Math.Min(MaxStreamLength - _bw.BaseStream.Length, iqByte.Length);

                _bw.Write(iqByte, 0, toWrite);
            }
        }
    }
}
