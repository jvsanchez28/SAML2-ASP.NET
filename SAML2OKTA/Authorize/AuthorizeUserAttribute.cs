using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAML2OKTA.Authorize
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                // The user is not authorized => no need to continue
                return false;
            }

            // At this stage we know that the user is authorized => we can fetch
            // the username
            string username = httpContext.User.Identity.Name;

            // Now let's fetch the account number from the request
            string account = httpContext.Request["accountNumber"];

            // All that's left is to verify if the current user is the owner 
            // of the account
            return IsAccountOwner(username, account);
        }

        private bool IsAccountOwner(string username, string account)
        {
            // TODO: query the backend to perform the necessary verifications
            return true;
        }
    }
}