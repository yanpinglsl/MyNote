using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Demo2
{
    public class LongTimeTask
    {
        public LongTimeTask()
        {
            Thread.Sleep(2000);
        }
    }
}
