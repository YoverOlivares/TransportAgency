using Microsoft.EntityFrameworkCore;
using TransportAgency.Data.Context;
using TransportAgency.Data.Repositories.Interfaces;
using TransportAgency.Data.Repositories.Implementations;
using TransportAgency.Business.Interfaces;
using TransportAgency.Business.Services;
using TransportAgency.Models.Entities;
using RouteEntity = TransportAgency.Models.Entities.Route;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework
builder.Services.AddDbContext<TransportAgencyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Register Specific Repositories
builder.Services.AddScoped<IBusRepository, BusRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<ITripRepository, TripRepository>();

// Register Entity Repositories for Dependency Injection
builder.Services.AddScoped<IGenericRepository<Customer>, GenericRepository<Customer>>();
builder.Services.AddScoped<IGenericRepository<Bus>, GenericRepository<Bus>>();
builder.Services.AddScoped<IGenericRepository<RouteEntity>, GenericRepository<RouteEntity>>();
builder.Services.AddScoped<IGenericRepository<Trip>, GenericRepository<Trip>>();
builder.Services.AddScoped<IGenericRepository<Seat>, GenericRepository<Seat>>();
builder.Services.AddScoped<IGenericRepository<Sale>, GenericRepository<Sale>>();

// Register Business Services
builder.Services.AddScoped<IBusService, BusService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IPdfService, PdfService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();