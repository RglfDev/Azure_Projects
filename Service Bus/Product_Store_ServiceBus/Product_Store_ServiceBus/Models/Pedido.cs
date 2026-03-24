namespace Product_Store_ServiceBus.Models
{
    public class Pedido
    {
        public int PedidoId { get; set; }
        public string? ClienteEmail { get; set; }
        public string? Producto { get; set; }
        public decimal Precio { get; set; }
    }
}
