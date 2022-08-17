using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FindProgrammingProject.Models;

namespace FindProgrammingProject_UnitTests.IntegartionTests.HelpClasses
{
    public class IntegrationTest
    {
        protected readonly HttpClient httpClient;
        public IntegrationTest()
        {

            var webAppFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<UserContext>));

                services.Remove(descriptor);
                services.AddDbContext<UserContext>(x => x.UseInMemoryDatabase("InMemoryTestDb"));
            }));
            httpClient = webAppFactory.CreateClient();
        }
    }
}
