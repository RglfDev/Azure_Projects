using System.ComponentModel.DataAnnotations;

namespace ProductosWebAPI.Models
{
    public class Categories_DB
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}
