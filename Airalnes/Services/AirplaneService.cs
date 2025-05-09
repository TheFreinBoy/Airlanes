using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Airalnes.Models;
using Airalnes.Helpers;

namespace Airalnes.Services
{
    public class AirplaneService
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        public List<Airplane> GetAllAirplanes()
        {
            return dbHelper.GetAirplanes();
        }

        public List<Airport> GetAllAirports()
        {
            return dbHelper.GetAirports();
        }
    }
}
