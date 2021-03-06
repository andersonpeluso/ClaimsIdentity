using System;

namespace Innovativo.Models
{
    public class EficaciaCanaisRelatorio
    {
        public int ID { get; set; }

        public virtual Cliente Cliente { get; set; }
        public int IdCliente { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }

        public virtual EficaciaCanalBuscaPaga BuscaPaga { get; set; }
        public virtual EficaciaCanalDireto Direto { get; set; }
        public virtual EficaciaCanalEmail Email { get; set; }
        public virtual EficaciaCanalOrganico Organico { get; set; }
        public virtual EficaciaCanalReferencia Referencia { get; set; }
    }
}