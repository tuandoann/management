using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace QUANLYTIEC.Models
{
    public partial class ConnectionEFDataFirst
    {
        public ConnectionEFDataFirst(string connnectionStr): base(connnectionStr)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
    }
}