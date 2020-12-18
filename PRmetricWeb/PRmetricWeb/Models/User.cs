using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRmetricWeb.Models
{
    public class User
    {
        public string Name { get; set; }
        public int Open { get; set; }
        public int Closed { get; set; }
        public int All { get; set; }
        public double TotalPoints { get; set; }
        public string Id { get; set; }
    }
}
