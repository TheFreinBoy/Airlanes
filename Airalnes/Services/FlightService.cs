using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Airalnes.Models;
using Airalnes.Helpers;

namespace Airalnes.Services
{
    public class FlightService
    {
        private readonly DatabaseHelper _dbHelper;

        public FlightService()
        {
            _dbHelper = new DatabaseHelper();
        }

        public void CreateFlight(FlightFormData data)
        {
            _dbHelper.CreateFlight(
                from: data.From,
                to: data.To,
                departure: data.DepartureDate,
                returnDate: data.ArrivalDate,
                flightClass: data.FlightClass,
                airplaneId: data.AirplaneId,
                flightNumber: data.FlightNumber,
                capacity: data.Capacity,
                timeDP: data.DepartureTime,
                timeAR: data.ArrivalTime
            );
        }
    }
}
