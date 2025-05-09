using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airalnes.Models
{
    public class FlightFormData
    {
        public string From { get; set; }
        public string To { get; set; }
        public string DepartureDate { get; set; }
        public string ArrivalDate { get; set; }
        public string FlightClass { get; set; }
        public int AirplaneId { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public int Capacity { get; set; }
    }

}
