using FindProgrammingProject.Models.DbContexts;
using FindProgrammingProject.Models.ObjectModels;
using Microsoft.AspNetCore.Identity;
using Stripe;
using System.Security.Cryptography;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{
    public class Creation : ICreation
    {
        private UserManager<User> userManager;
        private Context context;
        public Creation(UserManager<User> userManager, Context context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<User> Create(string Email, string Password, string Nickname)
        {
			var u1 = await userManager.FindByNameAsync(Nickname);
			if (u1 != null)
			{
				return new User { UserName = "Exist" };
			}
            var random = new Random();
            string affiliateCode = "";
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            do
            {
                affiliateCode = "";
                for (int i = 0; i < 4; i++)
                {
                    int index = random.Next(0, 25);
                    affiliateCode += alphabet[index];
                }
            } while (context.Users.FirstOrDefault(x => x.AffiliateCode == affiliateCode) != null);
            

            
			User user = new User
			{
				Email = Email,
				UserName = Nickname,
				Description = "",
				ProgrammingLanguages = new List<string>(),
				ProjectTypes = new List<string>(),
				WebSocketId = "",
				ProfilePicture = null,
				PictureFormat = "png",
                AffiliateCode = affiliateCode
			};
			var fs = new FileStream("C:/Users/marek/source/repos/FindProgrammingProject - User Authentication/FindProgrammingProject/Pictures/BasicProfilePicture.png", FileMode.Open, FileAccess.Read);
            using (var stream = new MemoryStream())
            {
                fs.CopyTo(stream);
                user.ProfilePicture = stream.ToArray();
			}
            var options = new CustomerCreateOptions
            {
                Email = user.Email,
                Name = user.UserName
            };
            var service = new CustomerService();
            var response = service.Create(options);

			var couponOptions = new CouponCreateOptions
			{
				Duration = "forever",
				Id = affiliateCode,
				PercentOff = 0.01m,
                Currency = "EUR"
			};
			var couponService = new CouponService();
			couponService.Create(couponOptions);


			user.StripeId = response.Id;
            var result = await userManager.CreateAsync(user,Password);
            if(result.Succeeded)
            {
                user = context.Users.FirstOrDefault(x => x.Email == Email && x.UserName == Nickname);
                UserPlan plan = new UserPlan
                {

                    Id = Guid.NewGuid().ToString(),
                    ActiveFrom = DateTime.Now.Date,
                    ActiveTo = DateTime.Now.Date.AddMonths(1),
                    Cost = 0,
                    UserId = user.Id,
                    IsMonthly = true,
                    PlanType = PlanType.Free,
                };
                using (var transaction = context.Database.BeginTransaction())
                {
					context.UserPlans.Add(plan);
					int changes = context.SaveChanges();
					if (changes == 1)
					{
                        transaction.Commit();
					}
                    else
                    {
						transaction.Rollback();
                        var deleteResponse = await userManager.DeleteAsync(user);
						return new User { UserName = "Error" };
					}

				}
                
                return user;
            }
            else
            {
                return new User { UserName = "Error" };
            }
        }
    }
    public interface ICreation
    {
        public Task<User> Create(string Email, string Password, string Nickname);
    }
}
