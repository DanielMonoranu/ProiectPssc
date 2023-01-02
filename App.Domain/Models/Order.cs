using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using LanguageExt;
using static LanguageExt.Prelude;

namespace App.Domain.Models
{
    public record Order
    {
        public decimal OrderValue { get; }
        public bool Delivered { get; }

        public int OrderId { get; }

        public Order(decimal Value, int orderId)
        {
            if (IsValid(Value, false))
            {
                OrderValue = Value;
                Delivered = false;
                OrderId = orderId;
            }
            else
            {
                throw new InvalidOrderException($"{Value:0.##} is an invalid order value.");
            }
        }

        public Order(decimal Value, bool delivered, int orderId) {
            if (IsValid(Value, delivered))
            {
                OrderValue = Value;
                Delivered = delivered;
                OrderId = orderId;
            }
            else {
                throw new InvalidOrderException($"{Value:0.##} is an invalid order value.");
            }
        }

        public static Option<Order> TryParseOrder(string gradeString) {

            string[] words = gradeString.Split(' ');
            if (decimal.TryParse(words[0], out decimal numericGrade))
            {
                if (words.Length > 1)
                {
                    if (bool.TryParse(words[1], out bool shipped))
                    {
                        if (int.TryParse(words[2], out int orderId))
                        {
                            if (IsValid(numericGrade, shipped))
                            {
                                return Some<Order>(new(numericGrade, shipped, orderId));
                            }
                            else return None;
                        }
                        else return None;
                    }
                    else return None;
                }
                else return None;
            }
            else return None;
        }

/*        public static bool TryParseGrade(string gradeString, out Order order)
        {
            bool isValid = false;
            order = null;
            string[] words = gradeString.Split(' ');
            if (decimal.TryParse(words[0], out decimal numericGrade))
            {
                if (words.Length > 1)
                {
                    if (bool.TryParse(words[1], out bool shipped))
                    {
                        if (IsValid(numericGrade, shipped))
                        {
                            isValid = true;
                            order = new(numericGrade, shipped);
                        }
                    }
                    else
                    {
                        if (IsValid(numericGrade, true))
                        {
                            isValid = true;
                            order = new(numericGrade, true);
                        }
                    }
                }
            }
            return isValid;
        }*/

        public Order AddVAT()
        {
            var roundedValue = OrderValue + OrderValue * 19 / 100;
            return new Order(roundedValue, Delivered, OrderId);
        }

        public override string ToString() {

            return $"{OrderValue:0.##} {Delivered}";
        }

        private static bool IsValid(decimal numericValue,bool shipped) => numericValue > 0 && numericValue <= 10000 && !shipped;
    }
}
