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
        private byte[] iqByte;
        private sbyte[] iqSbyte;
        public sbyte[] ReadFile(string filename)
        {
            fs = new FileStream(filename, FileMode.Open);
            int length = (int)fs.Length;
            fs.Read(iqByte, 0, length);
            iqSbyte = Array.ConvertAll(iqByte, b => unchecked((sbyte)b));
            fs.Close();
            fs.Dispose();
            return iqSbyte;
        }

        public void WriteFile(string filename, sbyte[] iqArray)
        {
            fs = new FileStream(filename, FileMode.OpenOrCreate);
            int length = iqArray.Length;
            iqByte = Array.ConvertAll(iqSbyte, b => unchecked((byte)b));
            fs.Write(iqByte, 0, length);
            fs.Close();
            fs.Dispose();
        }
    }
}
