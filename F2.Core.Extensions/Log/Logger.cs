using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Core.Extensions.Log
{
    public class Logger
    {
        public static readonly log4net.ILog Log = log4net.LogManager.GetLogger("log");
    }
}
