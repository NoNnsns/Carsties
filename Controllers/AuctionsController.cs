using System.Security.Cryptography.X509Certificates;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
  private readonly AuctionDbContext _context;
  private readonly IMapper _mapper;

  public AuctionsController(AuctionDbContext context,IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(){
    List<Auction> auctions = await _context.Auctions.Include(i=> i.Item)
                                    .OrderBy(d=>d.Item.Make)
                                    .ToListAsync();
    return _mapper.Map<List<Auction>,List<AuctionDto>>(auctions);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id){
    var auction =await _context.Auctions.Include(i=> i.Item)
                                   .FirstOrDefaultAsync(i => i.Id == id);
    if(auction is null) 
      return NotFound();
    
    return _mapper.Map<AuctionDto>(auction);

  }
  [HttpPost]
  public async Task<ActionResult<AuctionDto>> CreateAuction([FromBody]CreateAuctionDto  responseAuction){
    var auction = _mapper.Map<Auction>(responseAuction);

    //get user
    auction.Seller = "test";
    _context.Auctions.Add(auction);

    bool saveResult = await _context.SaveChangesAsync() > 0 ;
    if(!saveResult) 
      return  BadRequest("Could not save changes in the db");

    return CreatedAtAction(nameof(GetAuctionById),
                           new { auction.Id},
                           _mapper.Map<AuctionDto>(auction));

  }
  [HttpPut("{id}")]
  public async Task<ActionResult> UpdatAuction(Guid id, [FromBody] UpdateAuctionDto responseAuction){
    var auction = await _context.Auctions.Include(i=>i.Item)
                                   .FirstOrDefaultAsync(a=>a.Id == id);
    if(auction is null) 
      return NotFound();

    //Check seller == userName
    auction.Item.Make = responseAuction.Make ?? auction.Item.Make;
    auction.Item.Model = responseAuction.Model?? auction.Item.Model;
    auction.Item.Color = responseAuction.Color?? auction.Item.Color;
    auction.Item.Mileage = responseAuction.Mileage ?? auction.Item.Mileage;
    auction.Item.Year = responseAuction.Year ?? auction.Item.Year;
    
    bool isSave = await _context.SaveChangesAsync() > 0;
    if(!isSave) 
      return BadRequest("Could not save changes in the db");
    
    return Ok();
  } 
  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteAuction(Guid id){
    var auction =await _context.Auctions.FirstOrDefaultAsync(i=> i.Id == id);
    if(auction == null) 
      return NotFound();
    
    _context.Auctions.Remove(auction);
    bool isSave =await _context.SaveChangesAsync() > 0;
    if(!isSave) 
      return BadRequest("Could not save changes in the db");
    
    return Ok();
  }
}
