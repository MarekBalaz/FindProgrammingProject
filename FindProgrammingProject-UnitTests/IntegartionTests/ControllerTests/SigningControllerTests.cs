using FindProgrammingProject.FunctionalClasses.SigningLogic;
using FindProgrammingProject.Models;
using FindProgrammingProject_UnitTests.IntegartionTests.HelpClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace FindProgrammingProject_UnitTests.IntegartionTests.ControllerTests
{
    public class SigningControllerTests : IClassFixture<WebAppFactory<Program>>
    {
        private readonly WebAppFactory<Program> webServer;
        public SigningControllerTests(WebAppFactory<Program> _webServer)
        {
            //var x = _webServer.Server.Host;
            webServer = _webServer;
        }

        [Fact]
        public async Task TestMethod()
        {
            var client = webServer.CreateClient();
            var response = await client.GetAsync("/Test");
            var data = await response.Content.ReadAsStringAsync();
            Assert.Equal("Success",data);
        }
        [Theory]
        [InlineData("marekgamingacc@gmail.com", "MarekBalaz5", true)]
        [InlineData("marekgamingacc@gmail.com", "MarekBalaz", false)]
        [InlineData("", "marekbalaz", false)]
        [InlineData("marekgamingacc@gmail.com", "", false)]
        [InlineData("", "", false)]
        public async Task SignIn(string Email, string Password, bool equal)
        {
            //arrange
            bool arrangedResult = true;
            var client = webServer.CreateClient();
            //act
            

            bool actualResult = false;
            HttpResponseMessage message = await client.GetAsync($"/signin?Email={Email}&Password={Password}");
            var httpResponseContent = await message.Content.ReadAsStringAsync();
            actualResult = new JwtSecurityTokenHandler().CanReadToken(httpResponseContent);
            //assert
            if(equal == true)
            {
                Assert.Equal(arrangedResult, actualResult);
            }
            else
            {
                Assert.NotEqual(arrangedResult, actualResult);
            }
            
        }
        [Theory]
        [InlineData("marekgamingacc@gmail.com", "MarekBalaz5", "MarekBalaz5", "Marek", true)]
        [InlineData("marekbalaz81102@gmail.com@gmail.com", "MarekBalaz5", "MarekBalaz5", "Marek",false)]
        [InlineData("marekgamingacc@gmail.com", "Mark", "Mark", "Marek", false)]
        [InlineData("noemail", "MarekBalaz5", "MarekBalaz5", "Marek", false)]
        [InlineData("marekgamingacc@gmail.com", "", "MarekBalaz5", "Marek", false)]
        [InlineData("marekgamingacc@gmail.com", "MarekBalaz5", "Marekbalaz", "Marek", false)]
        public async Task SignUp(string Email, string Password, string PasswordConfirmation, string Nickname, bool equal)
        {
            //arrange
            bool arrangedResult = true;
            var client = webServer.CreateClient();
            //act
            
            bool actualResult = false;
            HttpResponseMessage message = await client.GetAsync($"/signup?Email={HttpUtility.UrlEncode(Email)}&Password={Password}&PasswordConfirmation={PasswordConfirmation}&Nickname={Nickname}");
            var httpResponseContent = await message.Content.ReadAsStringAsync();
            actualResult = new JwtSecurityTokenHandler().CanReadToken(httpResponseContent);
            //assert
            if (equal == true)
            {
                Assert.Equal(arrangedResult, actualResult);
            }
            else
            {
                Assert.NotEqual(arrangedResult, actualResult);
            }
        }
        [Theory]
        [InlineData("", "", false)]
        [InlineData("marekgamingacc@gmail.com", "", false)]
        [InlineData("", "token", false)]
        [InlineData("marekgamingacc@gmail.com", "token", true)]
        [InlineData("marekgamingacc@gmail.com", "invalidToken", false)]
        public async Task VerifyAuthorizationToken(string Email, string Token, bool equal)
        {
            //arrange
            var arrangedResult = SigningResult.Success.ToString();
            var client = webServer.CreateClient();
            if (Token == "token")
            {
                var tokenResponse = await client.GetAsync($"/getauthorizationtoken?Email={Email}");
                Token = await tokenResponse.Content.ReadAsStringAsync();
            }
            //act
            var actualResult = "";
            HttpResponseMessage response = await client.GetAsync($"/verifyauthorizationtoken?Email={Email}&Token={Token}");
            actualResult = await response.Content.ReadAsStringAsync();
            //assert
            if (equal == true)
            {
                Assert.Equal(arrangedResult, actualResult);
            }
            else
            {
                Assert.NotEqual(arrangedResult, actualResult);
            }
        }
        [Theory]
        [InlineData("marekgamingacc@gmail.com",true)]
        [InlineData("marek.balaz.1@icloud.com",false)]
        public async Task SendResetPasswordCode(string Email, bool equal)
        {
            //arrange
            var arrangedResult = SigningResult.Success.ToString();
            var client = webServer.CreateClient();
            //act
            var actualResult = SignInResult.Failed.ToString();
            HttpResponseMessage message = await client.GetAsync($"/sendresetpasswordcode?Email={Email}");
            actualResult = await message.Content.ReadAsStringAsync();
            //assert
            if (equal == true)
            {
                Assert.Equal(arrangedResult, actualResult);
            }
            else
            {
                Assert.NotEqual(arrangedResult, actualResult);
            }
        }
        [Theory]
        [InlineData("marekgamingacc@gmail.com", "token",true)]
        [InlineData("marekgamingacc@gmail.com", "invalidToken",false)]
        public async Task VerifyResetPasswordToken(string Email, string Token, bool equal)
        {
            //arrange
            var arrangedResult = SigningResult.Success.ToString();
            var client = webServer.CreateClient();
            if (Token == "token")
            {
                var tokenResponse = await client.GetAsync($"/getresetpasswordcode?Email={Email}");
                Token = await tokenResponse.Content.ReadAsStringAsync();
            }
            //act
            var actualresult = "";
            HttpResponseMessage response = await client.GetAsync($"/verifyresetpasswordtoken?email={Email}&token={Token}");
            actualresult = await response.Content.ReadAsStringAsync();
            //assert
            if(equal == true)
            {
                Assert.Equal(arrangedResult, actualresult);
            }
            else
            {
                Assert.NotEqual(arrangedResult,actualresult);
            }
            
        }
        [Theory]
        [InlineData("abc", "bcd", "marekgamingacc@gmail.com", "token", false)]
        [InlineData("Abcdefgh123", "Abcdefgh123", "marekgamingacc@gmail.com", "invalidToken", false)]
        [InlineData("Abcdefgh123", "Abcdefgh123", "marekgamingacc@gmail.com", "token", true)]
        public async Task SetNewPassword(string newPassword, string newPasswordRepeated, string email,string token, bool equal)
        {
            //arrange
            var arrangedResult = SigningResult.Success.ToString();
            var client = webServer.CreateClient();
            if (token == "token")
            {
                var tokenResponse = await client.GetAsync($"/getresetpasswordcode?Email={email}");
                token = await tokenResponse.Content.ReadAsStringAsync();
            }
            //act
            var httpResponseMessage = await client.GetAsync($"/setnewpassword?newPassword={newPassword}&newPasswordRepeated={newPasswordRepeated}&token={token}&email={email}");
            var actualResult = await httpResponseMessage.Content.ReadAsStringAsync();
            //assert
            if (equal == true)
            {
                Assert.Equal(arrangedResult, actualResult);
            }
            else
            {
                Assert.NotEqual(arrangedResult, actualResult);
            }
        }
        [Theory]
        [InlineData("marek.balaz.1@icloud.com",false)]
        [InlineData("marekgamingacc@gmail.com",true)]
        public async Task SendEmailVerificationCode(string Email, bool equal)
        {
            //arrange
            var arrangedResult = SigningResult.Success.ToString();
            var client = webServer.CreateClient();
            //act
            var httpResponse = await client.GetAsync($"/sendemailverificationcode?Email={Email}");
            var actualResult = await httpResponse.Content.ReadAsStringAsync();
            //assert
            if(equal == true)
            {
                Assert.Equal(arrangedResult, actualResult);
            }
            else
            {
                Assert.NotEqual(arrangedResult, actualResult);
            }
            
        }
        [Theory]
        [InlineData("marekgamingacc@gmail.com", "token",true)]
        [InlineData("marekgamingacc@gmail.com", "invalidToken",false)]
        public async Task VerifyEmail(string Email, string Token, bool equal)
        {
            //arrange
            var arrangedResult = SigningResult.Success.ToString();
            var client = webServer.CreateClient();
            //Email = HttpUtility.UrlEncode(Email);
            if (Token == "token")
            {
                var httpTokenResponse = await client.GetAsync($"/getemailverificationcode?Email={Email}");
                Token = await httpTokenResponse.Content.ReadAsStringAsync();
            }
            //act
            var httpResponseMessage = await client.GetAsync($"/verifyemail?Email={Email}&Token={Token}");
            var actualResult = await httpResponseMessage.Content.ReadAsStringAsync();
            //assert
            if(equal == true)
            {
                Assert.Equal(arrangedResult, actualResult);
            }
            else
            {
                Assert.NotEqual(arrangedResult,actualResult);
            }
            
        }
        [Theory]
        [InlineData("marekgamingacc@gmail.com", "Google", "", "Google")]
        [InlineData("marek.balaz.1@icloud.com", "Google", "", "Google")]
        [InlineData("marekgamingacc@gmail.com", "Google", "", "Google")]
        //here we need to take apart ExternalLoginInfo class into variables
        public async Task ThirdPartySignIn(string Email, string LoginProvider, string ProviderKey, string DisplayName)
        {
            //arrange
            bool arrangedResult = true;
            var client = webServer.CreateClient();
            List<Claim> Claims = new List<Claim> { new Claim(ClaimTypes.Email, Email) };
            var claims = new ClaimsPrincipal(new ClaimsIdentity(Claims));
            ExternalLoginInfo externalLoginInfo = new ExternalLoginInfo(claims, LoginProvider, ProviderKey, DisplayName);
            //act
            bool actualResult = false;
            HttpResponseMessage message = await client.GetAsync($"signing/thirdpartysignin?externalLoginInfo={JsonSerializer.Serialize(externalLoginInfo)}");
            var httpResponseContent = await message.Content.ReadAsStringAsync();
            actualResult = new JwtSecurityTokenHandler().CanReadToken(httpResponseContent);
            //assert
            Assert.Equal(arrangedResult, actualResult);
        }
    }
}
