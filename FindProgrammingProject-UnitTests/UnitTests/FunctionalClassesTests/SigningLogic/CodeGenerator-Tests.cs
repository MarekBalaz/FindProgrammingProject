using FindProgrammingProject.FunctionalClasses;
using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace FindProgrammingProject_UnitTests.FunctionalClassesTests.SigningLogic
{
    public class CodeGeneratorTests
    {
        //private Mock<UserManager<User>> userManager = new Mock<UserManager<User>>();
        [Fact]
        public async Task CodeGeneratingTest()
        {

            //arrange
            var arrangedResponse = SignUpResult.Success;
            //act
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null,);
            User user = new User {Email="marek.balaz.1@icloud.com" };
            mgr.Setup(x => x.GenerateEmailConfirmationTokenAsync(user)).ReturnsAsync("12345");
            EmailVerificationCodeGenerator cg = new EmailVerificationCodeGenerator(mgr.Object);
            var actualResponse = await cg.GenerateCode(user);
            //assert
            Assert.Equal(arrangedResponse, actualResponse);


        }
    }
}
