using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agil_Recettes
{
    class Machine
    {
        public string Id { get; set; }
        public string AncIndx { get; set; }
        public string NvIndx { get; set; }

        public Machine()
        {

        }
        public Machine(string id,string ancindx,string nvindx)
        {
            Id = id;
            AncIndx = ancindx;
            NvIndx = nvindx;
        }
    }
}
