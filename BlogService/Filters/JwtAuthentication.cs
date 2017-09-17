using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Jose;



// since .NET does not have a existing package providing whole JWT token verification
// therefore we have to do the whole process, below provides the filter to protect specific
// api entry

namespace BlogService.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class JwtAuthenticationAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            string authorizationHeader = actionContext.Request.Headers.Authorization?.ToString();

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                string[] auth = authorizationHeader.Split(' ');

                if (auth.Length == 2 && auth[0] == "JWT")
                {
                    var secretKey = ConfigurationManager.AppSettings["secretKey"];
                    byte[] secretKeyBytes = Encoding.ASCII.GetBytes(secretKey);

                    try
                    {
                        var jsonPayload = Jose.JWT.Decode(auth[1], secretKeyBytes);
                        return true;
                    }
                    catch (Jose.JoseException)
                    {
                        return false;
                    }
                }

                return false;
            }
            return false;
        }
    }
}
