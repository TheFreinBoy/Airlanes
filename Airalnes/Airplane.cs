using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airalnes
{
    public class Airplane
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}