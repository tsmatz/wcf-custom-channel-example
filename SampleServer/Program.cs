using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri uri = new Uri(@"baka.file://c:\tmp\test.txt");
            ServiceHost sv = new ServiceHost(typeof(CalcService), uri);
            //host.AddServiceEndpoint(
            //    typeof(ITestService),
            //    new TcpChunkingBinding(),
            //    "ep1");
            //sv.AddServiceEndpoint(typeof(ICalcService), new 
            sv.Open();
            Console.WriteLine("終了するにはキーを入力");
            Console.ReadLine();
            sv.Close();
        }
    }
}
