using System.ComponentModel.DataAnnotations.Schema;

namespace Innovativo.Models
{
    public class EficaciaCanal
    {
        public int ID { get; set; }

        [ForeignKey("EficaciaCanalID")]
        public virtual EficaciaCanaisRelatorio EficaciaCanaisRelatorio { get; set; }

        public int EficaciaCanalID { get; set; }
        public int Visitantes { get; set; }
        public int Leads { get; set; }
        public int Oportunidades { get; set; }
        public int Vendas { get; set; }
    }
}