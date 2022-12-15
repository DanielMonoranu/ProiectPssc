using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public record Product
    {
        public string Name { get; }
        public double Price { get; }

        public Product(string name, double price)
        {

            Price = price;
            Name = name;
        }
        public Product(string name, string price)
        {
            try
            {
                Price = Convert.ToDouble(price);

            }
            catch (Exception e)
            {
                Console.WriteLine("Price must be a number");
            }
            Name = name;
        }


        private static bool IsValid(double price)
        {
            return false;
        }
    }
}
