using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreServices.Interface
{
    public interface IClientConfiguration
    {
        string ClientName { get; set; }

        DateTime InvokedDateTime { get; set; }
    }
}
