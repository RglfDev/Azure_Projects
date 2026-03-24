using Microsoft.EntityFrameworkCore;

namespace ProductosWebAPI.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //List<Products_DB> products = new List<Products_DB>()
            //{
            //    new Products_DB{Id=1, Name="Kayak",Category="Watersports",Price=286M},
            //    new Products_DB{Id=2,Name="Lifejacket",Category="Watersports",Price=48.95M},
            //    new Products_DB{Id=3,Name="Soccer Ball",Category="Soccer",Price=19.50M},
            //    new Products_DB{Id=4,Name="Corner Flags",Category="Soccer",Price=34.95M},
            //    new Products_DB{Id=5,Name="Stadium",Category="Soccer",Price=79500M},
            //    new Products_DB{Id=6,Name="Thinking Cap",Category="Chess",Price=16.00M},
            //    new Products_DB{Id=7,Name="TUnsteady Chair",Category="Chess",Price=29.95M},
            //    new Products_DB{Id=8,Name="Human Chess Board",Category="Chess",Price=75M},
            //    new Products_DB{Id=9,Name="Bling Bling King",Category="Chess",Price=1200M}
            //};
            //modelBuilder.Entity<Products_DB>().HasData(products);
        }
    }
}
