using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SampleServer
{
    [ServiceContract]
    public interface ICalcService
    {
        [OperationContract]
        string SetValue(int value);
    }

}
