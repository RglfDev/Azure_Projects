using System.ComponentModel.DataAnnotations;

namespace ProductosWebAPI.Models
{
    public class Products_DB
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public Categories_DB? Category { get; set; }
        public decimal Price { get; set; }
    }
}
