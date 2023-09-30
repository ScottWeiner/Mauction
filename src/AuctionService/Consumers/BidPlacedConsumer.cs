using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using AuctionService.Entities;
using Contracts;
using AuctionService.Data;
using MassTransit.Courier.Contracts;

namespace AuctionService.Consumers
{
    public class BidPlacedConsumer : IConsumer<BidPlaced>
    {
        private readonly AuctionDbContext _context;
        public BidPlacedConsumer(AuctionDbContext context)
        {
            _context = context;
        }
        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("--> Consuming BidPlaced");

            var auction = await _context.Auctions.FindAsync(context.Message.AuctionId);

            if (auction.CurrentHighBid == null
                || context.Message.BidStatus.Contains("Accepted") && auction.CurrentHighBid < context.Message.Amount)
            {
                auction.CurrentHighBid = context.Message.Amount;
                await _context.SaveChangesAsync();
            }
        }
    }
}