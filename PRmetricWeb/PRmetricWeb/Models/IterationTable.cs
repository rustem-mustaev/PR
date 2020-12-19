using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRmetricWeb.Models
{
    public class IterationTable
    {
        public string Iteration { get; set; }
        public string UserName { get; set; }
        public int Open { get; set; }
        public int Closed { get; set; }
        public int All { get; set; }
    }
}
