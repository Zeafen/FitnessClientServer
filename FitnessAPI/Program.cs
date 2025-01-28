using FitnessAPI.domain;
using FitnessAPI.FitnessDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Recipes_API.Domain.Implementations;
using Recipes_API.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

var conf = new TokenConfig(builder.Configuration["JWT:issuer"], builder.Configuration["JWT:audience"], (long)1000 * 60 * 60 * 24 * 30, builder.Configuration["JWT:secret"]);
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireClaim("Role", "Admin");
    });
    opts.AddPolicy("LeaderPolicy", policy =>
    {
        policy.RequireClaim("Role", "Leader");
    });
    opts.AddPolicy("TrainerPolicy", policy =>
    {
        policy.RequireClaim("Role", "Trainer");
    });
});
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = conf.issuer,
            ValidAudience = conf.audience,
            IssuerSigningKey = conf.GetSymmetricSecurityKey(),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true
        };
    });


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<GymContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));
builder.Services.AddSingleton(conf);
builder.Services.AddSingleton<IHashingService, SHA256HashingService>();
builder.Services.AddSingleton<ITokenService, JwtTokenService>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "static")),
    RequestPath = "/static"
});
app.UseAuthorization();

app.MapControllers();

app.Run();
