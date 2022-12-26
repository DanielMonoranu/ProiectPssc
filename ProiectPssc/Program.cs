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
                }*/
                
            CancelDeliveryCommand command = new(listOfDeliveries);
                CancelDeliveryWorkflow workflow = new CancelDeliveryWorkflow();
            var result = workflow.Execute(command, (deliveryNumber) => true);

            result.Match(
                whenCancelDeliveryFailedEvent: eventResult =>
                {
                    Console.WriteLine($"Cancel failed:{eventResult.Reason}");
                    return eventResult;
                },
                whenCancelDeliverySucceededEvent: eventResult =>
                {
                    Console.WriteLine("Cancel succeded.");
                    Console.WriteLine(eventResult.MessageShown);
                    return eventResult;
                }
                );
            
            /*        var result = workflow.Execute(command, (deliveryNumber) => true);
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

        */
        }

        private static List<UnvalidatedDelivery> ReadListOfDeliveries()
        {
            List<UnvalidatedDelivery> listOfDeliveries = new();
            do
            {
                var deliveryNumber = ReadValue("Delivery Number:");
                if (string.IsNullOrEmpty(deliveryNumber)) { break; }

                var status = ReadValue("Delivery Status:");
                if (string.IsNullOrEmpty(status)) { break; }

                var orderId = ReadValue("Order Id:");
                if (string.IsNullOrEmpty(orderId)) { break; }

                //var deliveryNumberToAdd = new DeliveryNumber(deliveryNumber);
               // var deliveryEntryToAdd = new DeliveryEntry(Int32.Parse(status), Int32.Parse(orderId));

                listOfDeliveries.Add(new(deliveryNumber, status, orderId));
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