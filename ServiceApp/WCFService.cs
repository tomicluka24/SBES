﻿using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApp
{
    public class WCFService : IWCFContract
    {
        public void TestCommunication()
        {
            Console.WriteLine("Communication established.");
        }
    }
}
