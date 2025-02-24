
using BackTareas.Data;
using BackTareas.Repository;
using BackTareas.Repository.IRepository;
using BackTareas.Service;
using BackTareas.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Telegram.Bot;
using Quartz;
using Quartz.Impl;
using BackTareas.Jobs;

namespace BackTareas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Conexion"));
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

  
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("NuevaPolitica", app =>
                {
                    app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddScoped<IWorkRepository,WorkRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserReminderRepository,UserReminderRepository>();

            builder.Services.AddSingleton<CreatedToken>();

            builder.Services.AddSingleton<ITelegramBotClient>(providee =>
              new TelegramBotClient("7252831197:AAHefSGGiynsZqrNgV9oTkC_Hyk1A4zJVVQ"));

            builder.Services.AddSingleton<IReminderRepository, ReminderRepository>();
            builder.Services.AddSingleton<ReminderService>();
            builder.Services.AddTransient<ReminderJob>();

            builder.Services.AddLogging(loggin =>
            {
                loggin.AddConsole();
            });

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Registrar Quartz
            builder.Services.AddQuartz(config =>
            {
                config.UseMicrosoftDependencyInjectionJobFactory(); // Habilitar la inyección de dependencias
                config.UseSimpleTypeLoader();
                config.UseInMemoryStore();
                config.UseDefaultThreadPool();
            }).AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            builder.Services.AddSingleton(provider =>
            {
                var schedulerFactory = new StdSchedulerFactory();
                return schedulerFactory.GetScheduler().Result; // Obtiene la instancia del scheduler
            });


            builder.Services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
                };
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

            app.UseCors("NuevaPolitica");
            app.MapControllers();

            app.Run();
        }
    }
}
