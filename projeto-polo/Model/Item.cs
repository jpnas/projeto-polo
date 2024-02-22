using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projeto_polo.Model
{
    public class Item
    {
        public string Indicador {  get; set; }
        public DateTime Data { get; set; }
        public DateTime DataReferencia { get; set; }
        public float Media { get; set; }
        public float Mediana { get; set; }
        public float DesvioPadrao { get; set; }
        public float Minimo { get; set; }
        public float Maximo { get; set; }
        public int numeroRespondentes { get; set; }
        public int baseCalculo { get; set; }
    }
}
