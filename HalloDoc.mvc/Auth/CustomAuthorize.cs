using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HalloDoc.mvc.Auth
{

    [AttributeUsage(AttributeTargets.All)]
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly string _role;
        public CustomAuthorize(string role="")
        {
            _role = role;
        }

        private bool isAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtServices = context.HttpContext.RequestServices.GetService<IJwtService>();
            if (jwtServices == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AdminLogin" }));
                return;
            }



            var request = context.HttpContext.Request;
            var token = request.Cookies["jwt"];

            if (token == null || !jwtServices.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                if (isAjaxRequest(request))
                {
                    context.Result = new JsonResult(new { error = "Failed to Authenticate User" })
                    {
                        StatusCode = 401
                    };
                }
                else
                {

                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AdminLogin" }));
                }
                return;
            }
            var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

            if (roleClaim == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AdminLogin" }));
                return;
            }

            if (string.IsNullOrWhiteSpace(_role) || roleClaim.Value != _role)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));

            }
        }
    }
}
