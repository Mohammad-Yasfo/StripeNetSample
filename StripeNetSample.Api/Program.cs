using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StripeNetSample.Api.Extensions;
using StripeNetSample.Application.Payment.Contracts;
using StripeNetSample.Application.Payment.Services;
using StripeNetSample.Infrastructure.Payment.Stripe.Services;
using StripeNetSample.Repositories.Payment;
using StripeNetSample.Repositories.Payment.Profiles;
using StripeNetSample.Repositories.Payment.Repositories;
using StripeNetSample.Repositories.Payment.Transaction.Profiles;
using StripeNetSample.Repositories.Payment.Transaction.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentsConnectionString")));

builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentProviderService, StripePaymentProviderService>();

builder.Services.AddScoped<IPaymentAccountsRepository, PaymentAccountsRepository>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddSingleton(builder.Configuration.GetStripeConfiguration());
builder.Services.AddSingleton(builder.Configuration.GetWebAppsConfiguration());

var config = new MapperConfiguration(mc =>
{
    mc.AddProfile(new PaymentMappingProfile());
    mc.AddProfile(new TrasactionMappingProfile());
});
IMapper mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "StripeNetSample Payment APIs",
        Version = "v1",
        Description = "API documentation for StripeNetSample Payment services."
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
