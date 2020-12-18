using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRmetricWeb.Models
{
    public class Info
    {
        public string Iteration { get; set; }
        public string UserName { get; set; }
        public int OpenPRCount { get; set; }
        public int ClosedPRCount { get; set; }
        public int All { get; set; }
        public double TotalPoints { get; set; }
        public string Id { get; set; }
    }
}
