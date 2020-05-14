using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackRF_output
{
     public class sFile
    {
        private FileStream fs;
        private BinaryWriter _bw;
        private byte[] iqByte;
        private sbyte[] iqSbyte;
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

        public sbyte[] ReadFile()
        {
            fs = new FileStream(_fileName, FileMode.Open);
            int length = (int)fs.Length;
            fs.Read(iqByte, 0, length);
            iqSbyte = Array.ConvertAll(iqByte, b => unchecked((sbyte)b));
            fs.Close();
            fs.Dispose();
            return iqSbyte;
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
