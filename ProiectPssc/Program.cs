using Domain;
using Domain.Models;
using LanguageExt;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ProiectPssc
{
    class Program
    {

        static async Task Main(string[] args)
        {

            var listOfDeliveries = ReadListOfDeliveries().ToArray();
            CancelDeliveryCommand command = new(listOfDeliveries);
            CancelDeliveryWorkflow workflow = new();
            var result =await workflow.ExecuteAsync(command, CheckDeliveryExists);

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


                listOfDeliveries.Add(new(deliveryNumber, status, orderId));
            } while (true);
            return listOfDeliveries;
        }




        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        private static TryAsync<bool> CheckDeliveryExists(DeliveryNumber delivery)
        {
            Func<Task<bool>> func = async () =>
            {
                return true;
            };
            return TryAsync(func);
        }

    }
}