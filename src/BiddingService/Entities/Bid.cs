using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Entities;

namespace BiddingService.Entities
{
    public class Bid : Entity
    {
        //public string ID { get; set; }
        public string AuctionId { get; set; }
        public string Bidder { get; set; }
        public DateTime BidTime { get; set; } = DateTime.UtcNow;
        public int Amount { get; set; }
        public BidStatus BidStatus { get; set; }
    }
}