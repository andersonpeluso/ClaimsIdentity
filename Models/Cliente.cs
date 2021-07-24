using System.Collections.Generic;

namespace Innovativo.Models
{
    public class Cliente
    {
        public int ID { get; set; }
        public string NomeFantasia { get; set; }
        public virtual List<EficaciaCanaisRelatorio> EficaciaCanalRelatorioLista { get; set; }
        public virtual List<Usuario> UsuarioLista { get; set; }
    }
}