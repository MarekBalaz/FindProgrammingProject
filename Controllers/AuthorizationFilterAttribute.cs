using Microsoft.AspNetCore.Mvc.Filters;

namespace FindProgrammingProject.Controllers
{
    public class AuthorizationFilterAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            
        }
    }
}