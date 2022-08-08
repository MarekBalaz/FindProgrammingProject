using FindProgrammingProject.FunctionalClasses.SigningLogic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace FindProgrammingProject_UnitTests.IntegartionTests
{
    public class SigningControllerTests : IClassFixture<WebAppFactory<Program>>
    {
        private WebAppFactory<Program> webServer;
        public SigningControllerTests(WebAppFactory<Program> _webServer)
        {
            webServer = _webServer;
        }

        [Theory]
        [InlineData("marekgamingacc@gmail.com","marekbalaz")]
        [InlineData("marekgamingacc@gmail.com","MarekBalaz")]
        [InlineData("", "marekbalaz")]
        [InlineData("marekgamingacc@gmail.com", "")]
        [InlineData("","")]
        public async Task SignIn(string Email, string Password)
        {
            //arrange
            bool arrangedResult = true;
            var client = webServer.CreateClient();
            //act
            bool actualResult = false;
            HttpResponseMessage message = await client.GetAsync($"/signing/signin?Email={Email}&Password={Password}");
            var httpResponseContent = await message.Content.ReadAsStringAsync();
            actualResult = new JwtSecurityTokenHandler().CanReadToken(httpResponseContent);
            //assert
            Assert.Equal(arrangedResult, actualResult);
        }
        [Theory]
        [InlineData("marekgamingacc@gmail.com", "marekbalaz","marekbalaz","Marek")]
        [InlineData("marekbalaz81102@gmail.com@gmail.com", "marekbalaz", "marekbalaz", "Marek")]
        [InlineData("marekgamingacc@gmail.com", "Mark", "Mark", "Marek")]
        [InlineData("", "marekbalaz", "marekbalaz", "Marek")]
        [InlineData("marekgamingacc@gmail.com", "", "marekbalaz", "Marek")]
        [InlineData("marekgamingacc@gmail.com", "marekbalaz", "Marekbalaz", "Marek")]
        public async Task SignUp(string Email, string Password, string PasswordConfirmation, string Nickname)
        {
            //arrange
            bool arrangedResult = true;
            var client = webServer.CreateClient();
            //act
            bool actualResult = false;
            HttpResponseMessage message = await client.GetAsync($"/signing/signup?Email={Email}&Password={Password}&PasswordConfirmation={PasswordConfirmation}&Nickname={Nickname}");
            var httpResponseContent = await message.Content.ReadAsStringAsync();
            actualResult = new JwtSecurityTokenHandler().CanReadToken(httpResponseContent);
            //assert
            Assert.Equal(arrangedResult, actualResult);
        }
        [Theory]
        [InlineData("", "")]
        [InlineData("marekgamingacc@gmail.com", "")]
        [InlineData("", "token")]
        [InlineData("marekgamingacc@gmail.com", "token")]
        [InlineData("marekgamingacc@gmail.com", "invalidToken")]
        public async Task VerifyAuthorizationToken(string Email, string Token)
        {
            //arrange
            var arrangedResult = SigningResult.Success.ToString();
            var client = webServer.CreateClient();
            if (Token == "token")
            {
                var tokenResponse = await client.GetAsync($"signing/getauthorizationtoken?Email={Email}");
                Token = await tokenResponse.Content.ReadAsStringAsync();
            }
            //act
            var actualresult = "";
            HttpResponseMessage response = await client.GetAsync($"singing/verifyauthorizationtoken?Email={Email}&Token={Token}");
            actualresult = await response.Content.ReadAsStringAsync();
            //assert
            Assert.Equal(arrangedResult, actualresult);
        }
        [Theory]
        [InlineData("marekgamingacc@gmail.com")]
        [InlineData("marek.balaz.1@icloud.com")]
        public async Task SendResetPasswordCode(string Email)
        {
            //arrange
            var arrangedResult = SignInResult.Success.ToString();
            var client = webServer.CreateClient();
            //act
            var actualResult = SignInResult.Failed.ToString();
            HttpResponseMessage message = await client.GetAsync($"signing/sendresetpasswordcode?Email={Email}");
            actualResult = await message.Content.ReadAsStringAsync();
            //assert
            Assert.Equal(arrangedResult,actualResult);
        }
        [Theory]
        [InlineData("", "")]
        [InlineData("marekgamingacc@gmail.com", "")]
        [InlineData("", "token")]
        [InlineData("marekgamingacc@gmail.com", "token")]
        [InlineData("marekgamingacc@gmail.com", "invalidToken")]
        public async Task VerifyResetPasswordToken(string Email, string Token)
        {
            //arrange
            var arrangedResult = SigningResult.Success.ToString();
            var client = webServer.CreateClient();
            if(Token == "token")
            {
                var tokenResponse = await client.GetAsync($"signing/getpasswordresetcode?Email={Email}");
                Token = await tokenResponse.Content.ReadAsStringAsync();
            }
            //act
            var actualresult = "";
            HttpResponseMessage response = await client.GetAsync($"singing/verifyresetpasswordtoken?email={Email}&token={Token}");
            actualresult = await response.Content.ReadAsStringAsync();
            //assert
            Assert.Equal(arrangedResult,actualresult);
        }
        [Theory]
        [InlineData("", "", "", "")]
        [InlineData("abc", "bcd", "marekgamingacc@gmail.com", "token")]
        [InlineData("abc", "abc", "marekgamingacc@gmail.com", "invalidToken")]
        [InlineData("abc", "abd", "marekgamingacc@gmail.com", "token")]
        public async Task SetNewPassword(string newPassword, string newPasswordRepeated, string token, string email)
        {
            //arrange
            var arrangedResult = SigningResult.Success.ToString();
            var client = webServer.CreateClient();
            if(token == "token")
            {
                var tokenResponse = await client.GetAsync($"signing/getpasswordresetcode?Email={email}");
                token = await tokenResponse.Content.ReadAsStringAsync();
            }
            //act
            var httpResponseMessage = await client.GetAsync($"signing/setnewpassword?newPassword={newPassword}&newPasswordRepeated={newPasswordRepeated}&token={token}&email={email}");
            var actualResult = await httpResponseMessage.Content.ReadAsStringAsync();
            //assert
            Assert.Equal(arrangedResult, actualResult);
        }
        [Theory]
        [InlineData("")]
        [InlineData("marek.balaz.1@icloud.com")]
        [InlineData("marekgamingacc@gmail.com")]
        public async Task SendEmailVerificationCode(string Email)
        {
            //arrange
            var arrangedResult = SigningResult.Success.ToString();
            var client = webServer.CreateClient();
            //act
            var httpResponse = await client.GetAsync($"signing/sendemailverificationcode?Email={Email}");
            var actualResult = await httpResponse.Content.ReadAsStringAsync();
            //assert
            Assert.Equal(arrangedResult, actualResult);
        }
        [Theory]
        [InlineData("", "")]
        [InlineData("marekgamingacc@gmail.com", "")]
        [InlineData("", "token")]
        [InlineData("marekgamingacc@gmail.com", "token")]
        [InlineData("marekgamingacc@gmail.com", "invalidToken")]
        public async Task VerifyEmail(string Email, string Token)
        {
            //arrange
            var arrangedResult = SigningResult.Success.ToString();
            var client = webServer.CreateClient();
            if(Token == "token")
            {
                var httpTokenResponse = await client.GetAsync($"singing/sendemailverificationcode?Email={Email}");
                Token = await httpTokenResponse.Content.ReadAsStringAsync();
            }
            //act
            var httpResponseMessage = await client.GetAsync($"singing/verifyemail?Email={Email}&Token={Token}");
            var actualResult = await httpResponseMessage.Content.ReadAsStringAsync();
            //assert
            Assert.Equal(arrangedResult,actualResult);
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
            ExternalLoginInfo externalLoginInfo = new ExternalLoginInfo(claims,LoginProvider,ProviderKey,DisplayName);
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
