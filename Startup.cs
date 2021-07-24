using AutoMapper;
using Innovativo.Models;
using Innovativo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Innovativo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            string connection = Configuration["ConexaoMySql:MySqlConnectionString"];
            services.AddDbContext<InnovativoContext>(options => options.UseLazyLoadingProxies().UseMySql(connection));
            services.AddMvc();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection(nameof(AppSettings));
            services.Configure<AppSettings>(appSettingsSection);

            // configuração do jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Segredo);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var usuarioService = context.HttpContext.RequestServices.GetRequiredService<IUsuarioService>();
                        var usuarioID = int.Parse(context.Principal.Identity.Name);
                        var usuario = usuarioService.ObterPorID(usuarioID);
                        
                        if (usuario is null)
                            context.Fail("Unauthorized");
                        
                        return Task.CompletedTask;
                    }
                };

                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IEficaciaCanaisService, EficaciaCanaisService>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddAutoMapper();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();
            
            ConfigurarCors(app, false);
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void ConfigurarCors(IApplicationBuilder app, bool habilitarTudo)
        {
            var corsPolicyBuilder = new CorsPolicyBuilder();

            if (habilitarTudo)
            {
                app.UseCors(builder =>
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                );
            }
            else
            {
                app.UseCors(builder =>
                    builder
                    .WithOrigins("http://marketing62.tempsite.ws")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                );
            }

            app.UseCors(builder => builder = corsPolicyBuilder);
        }
    }
}