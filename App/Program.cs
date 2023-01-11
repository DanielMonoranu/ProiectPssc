using App.Data;
using App.Domain;
using App.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using App.Data;
using App.Domain;
using App.Data.Repositories;

namespace App
{
        class Program
        {

            private static readonly Random random = new Random();

            //private static string ConnectionString = "Server= localhost\\PSSC;Database=PSSCdb;Integrated Security=True;";
        private static string ConnectionString = "Server=(localdb)\\PSSC;Database=PSSCdb;Trusted_Connection=True;MultipleActiveResultSets=true";

        static async Task Main(string[] args)
            {
                using ILoggerFactory loggerFactory = ConfigureLoggerFactory();
                ILogger<CancelOrderWorkflow> logger = loggerFactory.CreateLogger<CancelOrderWorkflow>();

                var listOfOrders = ReadListOfOrders().ToArray();

                ShipOrderCommand command = new ShipOrderCommand(listOfOrders);
                var dbContextBuilder = new DbContextOptionsBuilder<OrdersContext>()
                                                .UseSqlServer(ConnectionString)
                                                .UseLoggerFactory(loggerFactory);

                OrdersContext ordersContext = new OrdersContext(dbContextBuilder.Options);
                OrdersRepository ordersRepository = new (ordersContext);
                OrderRegistrationsRepository orderRegistrationsRepository = new (ordersContext);
                
            CancelOrderWorkflow workflow = new(ordersRepository, orderRegistrationsRepository, logger);
                var result = await workflow.ExecuteAsync(command);

                result.Match(
                        whenShopOrderShippedFaildEvent: @event =>
                        {
                            Console.WriteLine($"Cancellation failed: {@event.Reason}");
                            return @event;
                        },
                        whenShopOrderShippedScucceededEvent: @event =>
                        {
                            Console.WriteLine($"Cancellation Succeded!");
                            Console.WriteLine(@event.Csv);
                            return @event;
                        }
                    );
            }

            private static ILoggerFactory ConfigureLoggerFactory()
            {
                return LoggerFactory.Create(builder =>
                                    builder.AddSimpleConsole(options =>
                                    {
                                        options.IncludeScopes = true;
                                        options.SingleLine = true;
                                        options.TimestampFormat = "hh:mm:ss ";
                                    })
                                    .AddProvider(new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()));
            }

            private static List<UnvalidatedOrder> ReadListOfOrders()
            {
                List<UnvalidatedOrder> listOfGrades = new();
                do
                {
                    //read registration number and grade and create a list of greads
                    var registrationNumber = ReadValue("Registration Number: ");
                    if (string.IsNullOrEmpty(registrationNumber))
                    {
                        break;
                    }

                    var examGrade = ReadValue("Order: ");
                    if (string.IsNullOrEmpty(examGrade))
                    {
                        break;
                    }

                    listOfGrades.Add(new(registrationNumber, examGrade));
                } while (true);
                return listOfGrades;
            }

            private static string? ReadValue(string prompt)
            {
                Console.Write(prompt);
                return Console.ReadLine();
            }


    }
}
