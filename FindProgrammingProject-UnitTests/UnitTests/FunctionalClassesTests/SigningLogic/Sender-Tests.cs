using FindProgrammingProject.FunctionalClasses.SigningLogic;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FindProgrammingProject_UnitTests.UnitTests.FunctionalClassesTests.SigningLogic
{
    public class SenderTests
    {
        private IConfiguration config;
        public SenderTests()
        {
            config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }
        [Fact]
        public async Task MailSenderTest()
        {
            //arrange
            var result = SigningResult.Success;
            //act
            ISender sender = new MailSender(config);
            var actual = await sender.SendCode("EncodedEmail","EncodedToken");
            //assert
            Assert.Equal(result, actual);
        }
        //public async Task SmsSenderTest()
        //{
        //    var result = SigningResult.Success;
        //    //act
        //    ISender sender = new SmsSender();
        //    var actual = await sender.SendCode("EncodedPhoneNumber", "EncodedToken");
        //    //assert
        //    Assert.Equal(result, actual);
        //}
    }
}
