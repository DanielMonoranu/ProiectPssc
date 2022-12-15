using Domain;
using Domain.Models;

namespace ProiectPssc
{
    class Program
    {

        static void Main(string[] args)
        {
            var listOfDeliveries = ReadListOfDeliveries().ToArray();
            /*
                foreach (var delivery in listOfDeliveries)
                {
                    Console.WriteLine(delivery);
                }
                */
            CancelDeliveryCommand command = new(listOfDeliveries);
            CancelDeliveryWorkflow workflow = new CancelDeliveryWorkflow();
            var result = workflow.Execute(command, (deliveryNumber) => true);
            result.Match(
                whenCancelDeliveryFailedEvent: failedEvent =>
                {
                    Console.WriteLine($"Cancel deliveries failed:{failedEvent.Reason}");
                 return failedEvent;
                },

                whenCancelDeliverySucceededEvent: succeededEvent =>
                {
                    Console.WriteLine("Cancel succeded.");
                    Console.WriteLine(succeededEvent.csv);
                    return succeededEvent;
                }

                );


        }

        private static List<ValidDelivery> ReadListOfDeliveries()
        {
            List<ValidDelivery> listOfDeliveries = new();
            do
            {
                var deliveryNumber = ReadValue("Delivery Number:");
                if (string.IsNullOrEmpty(deliveryNumber)) { break; }

                var productName = ReadValue("Product Name:");
                if (string.IsNullOrEmpty(productName)) { break; }

                var productPrice = ReadValue("Delivery Price:");
                if (string.IsNullOrEmpty(productPrice)) { break; }

                var deliveryNumberToAdd = new DeliveryNumber(deliveryNumber);
                var productToAdd = new Product(productName, productPrice);

                listOfDeliveries.Add(new(deliveryNumberToAdd, productToAdd));
            } while (true);
            return listOfDeliveries;
        }




        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

    }
}