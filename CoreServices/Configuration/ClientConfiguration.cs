using CoreServices.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreServices.Configuration
{
    public class ClientConfiguration : IClientConfiguration
    {
        public string ClientName { get; set; }

        public DateTime InvokedDateTime { get; set; }
    }
}
