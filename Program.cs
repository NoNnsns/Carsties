using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(opt=> opt.UseNpgsql(builder.Configuration.GetConnectionString("KeyBase")));

//autoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

try{
  DbInitializer.InitDb(app);
}
catch(Exception e){
  Console.WriteLine(e);
}

app.Run();
