using FindProgrammingProject.FunctionalClasses.SigningLogic;
using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace FindProgrammingProject_UnitTests.FunctionalClassesTests.SigningLogic
{
    public class ResetClass_Tests
    {
        [Theory]
        [InlineData("marek@gmail.com","token","marekb","marekb")]
        public async Task ResetPasswordTest(string email, string token,string password, string newPassword)
        {
            //arrange
            var result = SigningResult.Success;
            //act
            
            var store = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            User user = new User {Email = email};
            userManagerMock.Setup(x => x.FindByEmailAsync(email)).Returns(Task.FromResult(user));
            userManagerMock.Setup(x => x.ResetPasswordAsync(user, token, newPassword)).Returns(Task.FromResult(IdentityResult.Success));
            var passwordVerificationMock = new Mock<PasswordResetTokenVerifiction>(userManagerMock.Object);
            passwordVerificationMock.Setup(x => x.Verify(HttpUtility.UrlEncode(email), HttpUtility.UrlDecode(token))).Returns(Task.FromResult(SigningResult.Success));
            IReset reset = new ResetPassword(userManagerMock.Object,passwordVerificationMock.Object);
            var actual = await reset.Reset(newPassword, password, token, email);
            //assert
            Assert.Equal(result,actual);
        }
    }
}
