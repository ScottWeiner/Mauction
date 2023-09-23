using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionUpdatedFaultConsumer : IConsumer<Fault<AuctionUpdated>>
    {
        // DONE: Implement class 
        // DONE: Consume() method should handle System.ArgumentException reasonably, otherwise Console.WriteLine something
        public async Task Consume(ConsumeContext<Fault<AuctionUpdated>> context)
        {
            Console.WriteLine("--> Consuming faulty update");

            var exception = context.Message.Exceptions.First();
            Console.WriteLine("!!!! " + exception.ExceptionType + " !!!!!");
            if (exception.ExceptionType == "System.ArgumentException" || exception.ExceptionType == "System.Exception")
            {

                Console.WriteLine("You better check something out because the AuctionUpdatedFaultConsumer has a problem.");


                await context.Publish<AuctionUpdated>(context.Message.Message);
            }
            else
            {
                Console.WriteLine("Not an ArgumentException. Update error dashboard somewhere...");
            }
        }
    }
}