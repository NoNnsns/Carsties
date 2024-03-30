using System.Net;
using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class DbInitializer
{
  public static void InitDb(WebApplication app){
    using var scope = app.Services.CreateScope(); // (using) dispose services after use 
    SeedData(scope.ServiceProvider.GetService<AuctionDbContext>());
  }
  private static void SeedData(AuctionDbContext context){
    context.Database.Migrate();
    if(context.Auctions.Any()){
      Console.WriteLine("Already have data - no need to seed");
      return ;
    }
    var auctions = new List<Auction>();
    {
      new Auction{
        Id = Guid.NewGuid(),
        Status = Status.Live,
        ReservePrice = 20000,
        Seller = "bob",
        AuctionEnd = DateTime.UtcNow.AddDays(30),
        Item = new Item(){
          Make = "Ford",
          Model = "Model T",
          Color = "Rust",
          Mileage = 12302,
          Year = 1938,
          ImageUrl = "some/url"
        }
      };
    }
    context.AddRange(auctions);
    context.SaveChanges();
  }
}
