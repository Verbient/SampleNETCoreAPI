using MyApp.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyApp.Models;

namespace MyApp.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<Enums.UserRoles> _roles;

        public AuthorizeAttribute(params Enums.UserRoles[] roles)
        {
            //_roles = roles ?? new Enums.UserRoles[] { };

            if (roles == null)
            {
                _roles = Enum.GetValues(typeof(Enums.UserRoles)).Cast<Enums.UserRoles>().ToList();
            }
            else
            {
                _roles = roles;
            }
        }

        public AuthorizeAttribute(string AllowedRolesCSV)
        {
            _roles = _roles ?? null!;
            var list = AllowedRolesCSV.Split(',');
            foreach (var word in list)
            {
                var roles = Enum.GetValues(typeof(Enums.UserRoles)).Cast<Enums.UserRoles>().Where(m => m.ToString().ToLower().Contains(word.Trim().ToLower())).ToList();
                if(roles != null)
                {
                    if(_roles==null)
                    {
                        _roles = roles;
                    }
                    else
                    {
                        ((List<Enums.UserRoles>)_roles).AddRange(roles);
                    }
                }
            }
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            bool skipAuthorization = filterContext.ActionDescriptor.EndpointMetadata
                                 .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));

            if (skipAuthorization)
            {
                return;
            }
            var authAttributes = filterContext.ActionDescriptor.EndpointMetadata
                                 .Where(em => em.GetType() == typeof(AuthorizeAttribute)).Cast<AuthorizeAttribute>().ToList();


            if (!IsAllowed(authAttributes, (JWTAuth)filterContext.HttpContext.Items?["Account"]!))
            {
                filterContext.Result = new JsonResult(new { message = "Unauthorized user" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }


        // Only last Authorize attribute will be considered. Ex in case of Controller and Action, Controller attribue will be ignored
        
        private bool IsAllowed(List<AuthorizeAttribute> attrList, JWTAuth loggedUserRole)
        {
            if (attrList != null & loggedUserRole == null)
            {
                return false;
            }
            else if(attrList!.Last()._roles.Count==0 && loggedUserRole != null) // Simple Authorize attribute without any role definition
            {
                return true;
            }

            List<string> roles = new() { "customer", "bronzeagent", "silveragent", "goldagent", "platinumagent", "admin", "superadmin" };

            foreach (var item in attrList!.Last()._roles)
            {
                int exist = roles.IndexOf(item.ToString().ToLower());
                if (exist == -1)
                {
                    throw new CustomException($"Role <{item}> not defined in list of roles");
                }
                if (loggedUserRole!.RoleName.ToString().ToLower() == item.ToString().ToLower())
                {
                    return true;
                }
            }
            return false;
        }
    }
}