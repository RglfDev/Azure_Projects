namespace TransactionsMVCApp.Models
{
    
    public class ColaTransaccionesPendientes
    {
        public int TransaccionId { get; set; }
        public decimal Monto { get; set; }
        public string TipoTransaccion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
        
    
}
