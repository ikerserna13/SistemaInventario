using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Store
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Nombre es Requerido")]
        [MaxLength(60, ErrorMessage ="Nombre debe ser máxmio 60 Caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Descripción es Requerido")]
        [MaxLength(60, ErrorMessage = "Nombre debe ser máxmio 100 Caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage ="El estado es Requerido")]
        public bool Estado { get; set; }
    }
}
