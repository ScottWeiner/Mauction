using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;

        private readonly IPublishEndpoint _publishEndpoint;

        public AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions(string date)
        {
            var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

            if (!string.IsNullOrEmpty(date))
            {
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            }

            return await query.ProjectTo<AuctionDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();

            return _mapper.Map<AuctionDTO>(auction);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDto)
        {
            // 1) Map to Auction Entity objec
            var auction = _mapper.Map<Auction>(auctionDto);

            // DONE: Add current user as Seller
            auction.Seller = User.Identity.Name;

            // 2) Add record to context
            _context.Auctions.Add(auction);

            // 3) Map to Auction DTO 
            var newAuction = _mapper.Map<AuctionDTO>(auction);

            // 4) Map to AuctionCreated contract & publish to endpoint
            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            // 5) Save record
            var result = await _context.SaveChangesAsync() > 0;

            // 6) Return either BadRequest or CreatedAtAction depending on successful save
            if (!result) return BadRequest("Could not save changes to the DB");
            return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, newAuction);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<AuctionDTO>> UpdateAuction(Guid id, UpdateAuctionDTO auctionDto)
        {
            // 1) Find record of the Auction being updated
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();

            if (auction.Id != id) return NotFound();

            // 2) Update Auction Entity with details of the the UpdateAuctionDTO

            if (auction.Seller != User.Identity.Name) return Forbid();

            auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
            auction.Item.Year = auctionDto.Year ?? auction.Item.Year;
            auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;

            // 3) Map to to AuctionUpdated Contract & publish
            await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

            // 4) Save changed record in context & return correct response depending on success/fail.
            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Problem saving changes.");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);


            if (auction == null) return NotFound();

            if (auction.Seller != User.Identity.Name) return Forbid();

            _context.Auctions.Remove(auction);
            await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });
            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not update DB");

            return Ok();

        }
    }


}