using CleanArchitecture.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain
{
    public class Video : BaseDomainModel
    {
        public Video() {
            Actores = new HashSet<Actor>();
        }
        public string? Nombre { get; set; }
        public int StreamerId { get; set; } //Este nombre va por convencion para hacer llave foranea, sino se sigue la convencion hay que usar el FluentAPI
        public virtual Streamer? Streamer { get; set; } //Se ocupara para la llave foranea
        public virtual ICollection<Actor> Actores { get; set; }
        public virtual Director Director { get; set; }

    }
}
