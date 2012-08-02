using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace SampleServer
{
    public class CalcService : ICalcService
    {
        public string SetValue(int value)
        {
            Console.WriteLine("値 {0} を受信", value);
            return "Conguratulation !";
        }
    }
}
