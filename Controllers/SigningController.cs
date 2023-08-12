using FindProgrammingProject.FunctionalClasses.SigningLogic;
using FindProgrammingProject.FunctionalClasses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;
using FindProgrammingProject.Models.ObjectModels;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using FindProgrammingProject.Models.DbContexts;

namespace FindProgrammingProject.Controllers
{
    [ApiController]
    [Route("authentication")]
    public class SigningController : ControllerBase
    {
        private ISignClass signClass;
        private IVerification? verification;
        private ICodeGenerator? codeGenerator;
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private IReset? reset;
        private ISender? sender;
        private ILogger<SigningController> logger;
        private IConfiguration configuration;
        private ICreation creation;
        private IJwtTokenGenerator jwtTokenGenerator;
        public SigningController(UserManager<User> userManager,
            SignInManager<User> signInManager, ILogger<SigningController> logger, IConfiguration configuration, IJwtTokenGenerator jwtTokenGenerator, Context context)
        {
            this.signClass = new SignClass(userManager, signInManager, jwtTokenGenerator, configuration, context);
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.configuration = configuration;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }
        //This action will either sign in person or return sign in view
        [HttpPost]
        [Route("signin")]
        public async Task<string> SignIn([FromHeader]string Email, [FromHeader]string Password)
        {
            try
            {
                string jsonResponse;
                verification = new AuthorizationTokenVerification(configuration);
                if (Email == "" && Password == "")
                {
                    jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.CredentialsNotSet}\", \"Exception\":false}}";
                    return jsonResponse;
                }
                else
                {
                    var response = await signClass.SignIn(Email, Password);
                    if(response.Substring(0,2) != "ey")
                    {
                        jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{response}\", \"Exception\":false}}";
                        return jsonResponse;
                    }
                    var verificatinoResponse = await verification.Verify(Email, response);
					if (verificatinoResponse == SigningResult.Success)
                    {
                        var user = await userManager.FindByEmailAsync(Email);
                        var refreshToken = jwtTokenGenerator.RefreshTokenGenerator();
                        user.RefreshToken = refreshToken;
                        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                        await userManager.UpdateAsync(user);
                        jsonResponse = $"{{\"Success\":true, \"Token\":\"{response}\", \"RefreshToken\":\"{refreshToken}\", \"ErrorMessage\":\"{SigningResult.Success}\", \"Exception\":false}}";
                        return jsonResponse;
                    }
                    else if(verificatinoResponse == SigningResult.AccountLockedOut)
                    {
						logger.LogWarning($"Failed loggin with account locked out on account with Email: {Email}");
						jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{response}\", \"Exception\":false}}";
						return jsonResponse;
					}
					logger.LogWarning($"Failed loggin on account with Email: {Email}");
					jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{response}\", \"Exception\":false}}";
					return jsonResponse;
				}
            }
            catch (Exception ex)
            {
                var jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{ex.Message}\", \"Exception\":true}}";
                Response.StatusCode = 500;
                logger.LogError(JsonSerializer.Serialize(ex.StackTrace) + ex.Message);
                return jsonResponse;
            }

        }
        //This action will either sign up person or return sign up view
        [HttpPost]
        [Route("signup")]
        public async Task<string> SignUp([FromHeader]string Email, [FromHeader] string Password, [FromHeader]string UserName)
        {
            string jsonResponse;
            try
            {
                verification = new AuthorizationTokenVerification(configuration);
                Email = HttpUtility.UrlDecode(Email);
                Password = HttpUtility.UrlDecode(Password);
                UserName = HttpUtility.UrlDecode(UserName);
                if (Email == "" || Password == "" || UserName == "")
                {
                    jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.CredentialsNotSet}\", \"Exception\":false}}";
                    return jsonResponse;
                }

                SigningResult signUpResult = await signClass.SignUp(Email, UserName, Password);
                if (signUpResult == SigningResult.Success)
                {
                    jsonResponse = $"{{\"Success\":true, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.Success}\", \"Exception\":false}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{signUpResult}\", \"Exception\":false}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.LogError(JsonSerializer.Serialize(ex.StackTrace)+ ex.Message);
                jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{ex.Message}\", \"Exception\":true}}";
                return jsonResponse;
            }
        }
        [HttpGet]
        [Route("refreshtoken")]
        public async Task<string> RefreshToken([FromHeader] string JwtToken, [FromHeader] string RefreshToken, [FromHeader] string Email)
		{
			string jsonResponse = "";
			try
            {
                if (string.IsNullOrEmpty(JwtToken) || string.IsNullOrWhiteSpace(JwtToken)
                    || string.IsNullOrEmpty(RefreshToken) || string.IsNullOrWhiteSpace(RefreshToken)
                    || string.IsNullOrEmpty(Email) || string.IsNullOrWhiteSpace(Email)
                    )
                {
                    jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.CredentialsNotSet}\", \"Exception\":false}}";
                    return jsonResponse;
                }
                var Key = Encoding.UTF8.GetBytes(configuration.GetValue<string>("IssuerSigningKey"));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration.GetValue<string>("ValidJwtIssuer"),
                    ValidAudience = $"{configuration.GetValue<string>("ValidJwtAudience")}",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    var principal = tokenHandler.ValidateToken(JwtToken, tokenValidationParameters, out SecurityToken securityToken);
                    if (securityToken is not JwtSecurityToken jwtSecurityToken)
                    {
                        jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.InvalidAccessTokenOrRefreshToken}\", \"Exception\":false}}";
                        return jsonResponse;
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("expired"))
                    {
                        jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.InvalidAccessTokenOrRefreshToken}\", \"Exception\":false}}";
                        return jsonResponse;
                    }
                }

                var user = await userManager.FindByEmailAsync(Email);
                if (user == null || user.RefreshToken != RefreshToken || user.RefreshTokenExpiryTime < DateTime.Now)
                {
                    jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.InvalidAccessTokenOrRefreshToken}\", \"Exception\":false}}";
                    return jsonResponse;
                }
                var jwtToken = jwtTokenGenerator.GetJwtToken(user);
                var refreshToken = jwtTokenGenerator.RefreshTokenGenerator();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                await userManager.UpdateAsync(user);
                jsonResponse = $"{{\"Success\":true, \"Token\":\"{jwtToken}\", \"RefreshToken\":\"{refreshToken}\", \"ErrorMessage\":\"{SigningResult.Success}\", \"Exception\":false}}";
                return jsonResponse;
            }
            catch(Exception ex)
            {
				Response.StatusCode = 500;
				logger.LogError(JsonSerializer.Serialize(ex.StackTrace)+ ex.Message);
				jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{ex.Message}\", \"Exception\":true}}";
				return jsonResponse;
			}
        }
        [HttpGet]
        [Route("getauthorizationtoken")]
        private async Task<string> GetAuthorizationToken(string Email)
        {
            try
            {
                Email = HttpUtility.UrlDecode(Email);
                JwtTokenGenerator tokenGenerator = new JwtTokenGenerator();
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    string token = tokenGenerator.GetJwtToken(user);
                    string tokenResponse = $"{{\"Message\":\"{token}\", \"Token\":\"true\"}}";
                    return tokenResponse;
                }
                string jsonResponse = $"{{\"Message\":\"{SigningResult.EmailNotFound}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.LogError(JsonSerializer.Serialize(ex.StackTrace)+ ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpPost]
        [Route("verifyauthorizationtoken")]
        public async Task<string> VerifyAuthorizationToken([FromHeader] string Email, [FromHeader] string Token)
        {
            try
            {
                Email = HttpUtility.UrlDecode(Email);
                Token = HttpUtility.UrlDecode(Token);
                string jsonResponse;
                if (Email != "")
                {
                    verification = new AuthorizationTokenVerification(configuration);
                    SigningResult result = await verification.Verify(Email, Token);
                    jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                    
                }
                jsonResponse = $"{{\"Message\":\"{SigningResult.EmailNotFound}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.LogError(JsonSerializer.Serialize(ex.StackTrace)+ ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("sendresetpasswordcode")]
        public async Task<string> SendResetPasswordCode(string Email)
        {
            string jsonResponse;
            try
            {
                Email = HttpUtility.UrlDecode(Email);
                sender = new MailSender(configuration);
                codeGenerator = new PasswordResetCodeGenerator(userManager, sender);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    SigningResult result = await codeGenerator.GenerateCode(user);
                    if(result == SigningResult.Success)
                    {
                        jsonResponse = $"{{\"Success\":true, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{result}\", \"Exception\":false}}";
                        return jsonResponse;
                    }
                    jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{result}\", \"Exception\":false}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.EmailNotFound}\", \"Exception\":false}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.LogError(JsonSerializer.Serialize(ex.StackTrace)+ ex.Message); 
                jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{ex.Message}\", \"Exception\":true}}";
                return jsonResponse;
            }
        }
        [HttpGet]
        [Route("getresetpasswordcode")]
        private async Task<string> GetResetPasswordCode(string Email)
        {
            try
            {
                string jsonResponse;
                Email = HttpUtility.UrlDecode(Email);
                //sender = new MailSender(configuration);
                codeGenerator = new PasswordResetCodeGenerator(userManager, null);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    string result = await codeGenerator.GetCode(user);

                    jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Message\":\"{SigningResult.EmailNotFound}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.LogError(JsonSerializer.Serialize(ex.StackTrace)+ ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpPost]
        [Route("verifyresetpasswordtoken")]
        public async Task<string> VerifyResetPasswordToken([FromHeader] string Email, [FromHeader] string Token)
		{
			string jsonResponse;
			try
            {
                if (Email == "" || Token == "")
                {
					jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.CredentialsNotSet}\", \"Exception\":false}}";
					return jsonResponse;
				}
                verification = new PasswordResetTokenVerifiction(userManager);
                SigningResult result = await verification.Verify(Email, Token);
                if (result == SigningResult.Success)
                {
					jsonResponse = $"{{\"Success\":true, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.Success}\", \"Exception\":false}}";
					return jsonResponse;
				}
				jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.IncorrectToken}\", \"Exception\":false}}";
				return jsonResponse;
			}
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.LogError(JsonSerializer.Serialize(ex.StackTrace)+ ex.Message);
				jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{ex.Message}\", \"Exception\":true}}";
				return jsonResponse;
			}
        }
        [HttpPost]
        [Route("setnewpassword")]
        public async Task<string> SetNewPassword([FromHeader] string NewPassword,[FromHeader] string Token, [FromHeader] string Email)
		{
			string jsonResponse;
			try
            {
                if (NewPassword == "" || Token == "" || Email == "")
                {
					jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.CredentialsNotSet}\", \"Exception\":false}}";
					return jsonResponse;
				}
                reset = new ResetPassword(userManager, new PasswordResetTokenVerifiction(userManager));
                var result = await reset.Reset(NewPassword, Token, Email);
                if(result == SigningResult.Success)
                {
					jsonResponse = $"{{\"Success\":true, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.Success}\", \"Exception\":false}}";
					return jsonResponse;
				}
				jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{result}\", \"Exception\":false}}";
				return jsonResponse;
			}
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.LogError(JsonSerializer.Serialize(ex.StackTrace)+ ex.Message);
				jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{ex.Message}\", \"Exception\":true}}";
				return jsonResponse;
			}
        }
        [HttpGet]
        [Route("sendemailverificationcode")]
        public async Task<string> SendEmailVerificationCode(string Email)
        {
            try
            {
                string jsonResponse;
                Email = HttpUtility.UrlDecode(Email);
                sender = new MailSender(configuration);
                codeGenerator = new EmailVerificationCodeGenerator(userManager, sender);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    SigningResult result = await codeGenerator.GenerateCode(user);

                    jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Message\":\"{SigningResult.EmailNotFound}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.LogError(JsonSerializer.Serialize(ex.StackTrace)+ ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("getemailverificationcode")]
        private async Task<string> GetEmailVerificationCode(string Email)
        {
            try
            {
                string jsonResponse;
                codeGenerator = new EmailVerificationCodeGenerator(userManager, null);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    string result = await codeGenerator.GetCode(user);
                    result = HttpUtility.UrlEncode(result);
                    jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Message\":\"{SigningResult.EmailNotFound}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.LogError(JsonSerializer.Serialize(ex.StackTrace)+ ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        
        [HttpPost]
        [Route("verifyemail")]
        public async Task<string> VerifyEmail([FromHeader] string Email, [FromHeader] string Token)
        {
			string jsonResponse;
			try
            {
                
                if(Email == "" || Token == "")
                {
					jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.CredentialsNotSet}\", \"Exception\":false}}";
					return jsonResponse;
				}
                verification = new EmailTokenVerification(userManager, signInManager);
                SigningResult result = await verification.Verify(Email, Token);
                if(result == SigningResult.Success)
                {
					jsonResponse = $"{{\"Success\":true, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{SigningResult.Success}\", \"Exception\":false}}";
					return jsonResponse;
				}
				jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{result}\", \"Exception\":false}}";
				return jsonResponse;
			}
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.LogError(JsonSerializer.Serialize(ex.StackTrace)+ ex.Message);
				jsonResponse = $"{{\"Success\":false, \"Token\":\"\", \"RefreshToken\":\"\", \"ErrorMessage\":\"{ex.Message}\", \"Exception\":true}}";
				return jsonResponse;
			}
        }       
        [HttpPost]
        [Route("thirdpartysignin")]
        public async Task<string> ThirdPartySignIn([FromHeader] string LoginProvider, [FromHeader] string ProviderKey, [FromHeader] string Email, [FromHeader] string ProviderDisplayName)
        {
            try
            {
                verification = new AuthorizationTokenVerification(configuration);
                UserLoginInfo lg = new UserLoginInfo(LoginProvider,ProviderKey, ProviderDisplayName);
                string jsonResponse;
                var response = await signClass.ThirdPartySignIn(LoginProvider, ProviderKey, Email, lg);
                if(await verification.Verify(Email, response) == SigningResult.Success)
                {
                    jsonResponse = $"{{\"Message\":\"{response}\", \"Token\":\"true\"}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Message\":\"{response}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.LogError(JsonSerializer.Serialize(ex.StackTrace)+ ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
    }
}
