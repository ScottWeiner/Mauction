using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
    {
        public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
        {
            Console.WriteLine("--> Consuming faulty creation");

            var exception = context.Message.Exceptions.First();
            Console.WriteLine("!!!! " + exception.ExceptionType + " !!!!!");
            if (exception.ExceptionType == "System.ArgumentException")
            {

                context.Message.Message.Model = "Foobar";


                await context.Publish<AuctionCreated>(context.Message.Message);
            }
            else
            {
                Console.WriteLine("Not an ArgumentException. Update error dashboard somewhere...");
            }
        }
    }
}