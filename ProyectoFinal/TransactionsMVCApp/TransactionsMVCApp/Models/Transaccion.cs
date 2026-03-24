using System.ComponentModel.DataAnnotations;

namespace TransactionsMVCApp.Models
{
    public class Transaccion
    {
        [Key]
        public int TransaccionId { get; set; }

        [Required(ErrorMessage = "Debes introducir un monto")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un tipo de transacción")]
        [StringLength(50)]
        public string? TipoTransaccion { get; set; }

        [Required(ErrorMessage = "Debes introducir una cuenta de destino")]
        [StringLength(16, ErrorMessage = "La cuenta debe tener 16 caracteres")]
        [MinLength(16, ErrorMessage = "La cuenta debe tener 16 caracteres")]
        public string? CuentaDestino { get; set; }

        [StringLength(250, ErrorMessage = "Máximo 250 caracteres")]
        public string? DetallesAdicionales { get; set; }

        public string? Estado { get; set; } 

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaProcesamiento { get; set; }

        public DateTime? FechaNotificacion { get; set; }

        public ICollection<EventoTransaccion>? Eventos { get; set; }
        public Notificacion? Notificacion { get; set; }
    }
}
