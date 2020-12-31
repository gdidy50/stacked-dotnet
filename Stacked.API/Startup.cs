using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Stacked.API.Middleware;
using Stacked.API.Models;
using Stacked.API.Validators;
using Stacked.Data;
using Stacked.Models;
using Stacked.Services;
using Stacked.Services.Interfaces;
using Stacked.Services.Serialization;

namespace Stacked.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(c => c.AddProfile<EntityMappingProfile>(), typeof(Startup));

            services.AddDbContext<BlogDbContext>(opts
                => opts.UseSqlServer(Configuration.GetConnectionString("stacked.dev")));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddTransient<IArticleService, ArticleService>();

            services.AddTransient<IValidator<ArticleDto>, ArticleValidator>();
            services.AddTransient<IValidator<TagDto>, TagValidator>();
            services.AddTransient<IValidator<CommentDto>, CommentValidator>();
            services.AddTransient<IValidator<UserDto>, UserValidator>();
            services.AddTransient<IValidator<ManyArticlesRequest>, ManyArticlesRequestValidator>();
            services.AddTransient<IValidator<ManyCommentsRequest>, ManyCommentsRequestValidator>();
            services.AddTransient<IValidator<ManyTagsRequest>, ManyTagsRequestValidator>();
            services.AddTransient<IValidator<ManyUsersRequest>, ManyUsersRequestValidator>();

            // services.AddControllers();
            services.AddMvc(opts => opts.Filters.Add<ValidationMiddleware>())
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    };
                })
                .AddFluentValidation();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Stacked.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stacked.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder => builder
                                    .WithOrigins("http://localhost:8080")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
