using FluentValidation;
using Behaviors.Application.Behaviors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PNUStudentPortal.Infrastructure.Services.OTP;
using Shopverse.Application.Behaviors;
using Shopverse.Application.Features.Identity.Commands.RegisterUser;
using Shopverse.Application.Services.Attachments;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Interfaces.Attachments;
using Shopverse.Domain.Interfaces.Communication;
using Shopverse.Domain.Interfaces.OTP;
using Shopverse.Domain.Interfaces.Security;
using Shopverse.Infrastructure.Services;
using Shopverse.Infrastructure.Services.Attachments;
using Shopverse.Infrastructure.Services.Communication;
using Shopverse.Infrastructure.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddValidatorsFromAssembly(typeof(RegisterUserHandler).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PaginationBehavior<,>));
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IFileStorage, LocalFileStorage>();
            services.AddScoped<AttachmentService>();

            return services;
        }
    }
}
