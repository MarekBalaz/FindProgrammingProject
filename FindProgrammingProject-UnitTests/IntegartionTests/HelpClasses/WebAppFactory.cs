using FindProgrammingProject.FunctionalClasses.SigningLogic;
using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindProgrammingProject_UnitTests.IntegartionTests.HelpClasses
{
    public class WebAppFactory<T> : WebApplicationFactory<T> where T : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<UserContext>));

                services.Remove(descriptor);
                services.AddDbContext<UserContext>(x =>
                {
                    x.UseSqlServer("Data Source=BALON\\SQLEXPRESS;Initial Catalog=IntegrationTestDatabase;Integrated Security=True;Pooling=False");

                });
            });
        }
    }
}
