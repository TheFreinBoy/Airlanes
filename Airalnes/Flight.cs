using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airalnes
{
    public class Flight
    {
        public int Id { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public string Departure { get; set; }
        public string ReturnDate { get; set; }
        public string Class { get; set; }
        public string AirplaneName { get; set; }
        public string FlightNumber { get; set; }
        public int Capacity { get; set; }
        public string TimeDP { get; set; }
        public string TimeAR { get; set; }
    }

}
