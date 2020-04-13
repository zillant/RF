using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackView
{
    class Program
    {
        private static HackRFController Controller;
        static void Main(string[] args)
        {
            Console.Title = "HackRF#";
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("Hello HackRF User!");
            Controller = new HackRFController();
            Controller.Init();
            
            Console.ReadKey();
        }
    }
}
