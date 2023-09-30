using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Xsl;
using AuctionService.Data;
using Contracts;
using MassTransit;
using AuctionService.Entities;


namespace AuctionService.Consumers
{
    public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
    {
        private readonly AuctionDbContext _context;
        public AuctionFinishedConsumer(AuctionDbContext context)
        {
            _context = context;

        }
        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("--> Consuming BidPlaced");

            var auction = await _context.Auctions.FindAsync(context.Message.AuctionId);

            if (context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = context.Message.Amount;

            }

            auction.Status = auction.SoldAmount > auction.ReservePrice ?
                Status.Finished : Status.ReserveNotMet;

            await _context.SaveChangesAsync();
        }
    }
}