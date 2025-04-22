using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airalnes
{
    public class Airport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string IATACode { get; set; }

        public override string ToString()
        {
            return $"{Country} ({IATACode})";
        }
    }
}
