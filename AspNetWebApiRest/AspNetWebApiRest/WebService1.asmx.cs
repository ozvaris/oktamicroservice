using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Net.Http.Headers;
using System.Text;

namespace AspNetWebApiRest
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        public AuthenticationHeaderValue SoapHeader;

       
        [WebMethod]
            
        public string HelloWorld()
        {
            if (SoapHeader == null)
                return "Please call Auth Method";

            //if (!SoapHeader.IsUserCredentialsValid(SoapHeader))
            //    return "Please call AuthenticationMethod() first";


            return "Hello World";// + HttpRuntime.Cache[SoapHeader.AuthenticationToken];
        }
    }


    public class BasicAuthHttpModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication application)
        {
            application.AuthenticateRequest += new
                EventHandler(this.OnAuthenticateRequest);
            application.EndRequest += new
                EventHandler(this.OnEndRequest);
        }

        public void OnAuthenticateRequest(object source, EventArgs
                            eventArgs)
        {
            HttpApplication app = (HttpApplication)source;

            string authHeader = app.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authHeader))
            {
                string authStr = app.Request.Headers["Authorization"];

                if (authStr == null || authStr.Length == 0)
                {
                    return;
                }

                authStr = authStr.Trim();
                if (authStr.IndexOf("Basic", 0) != 0)
                {
                    return;
                }

                authStr = authStr.Trim();

                string encodedCredentials = authStr.Substring(6);

                byte[] decodedBytes =
                Convert.FromBase64String(encodedCredentials);
                string s = new ASCIIEncoding().GetString(decodedBytes);

                string[] userPass = s.Split(new char[] { ':' });
                string username = userPass[0];
                string password = userPass[1];

                if (!MyUserValidator.Validate(username, password))
                {
                    DenyAccess(app);
                    return;
                }
            }
            else
            {
                app.Response.StatusCode = 401;
                app.Response.End();
            }
        }
        public void OnEndRequest(object source, EventArgs eventArgs)
        {
            if (HttpContext.Current.Response.StatusCode == 401)
            {
                HttpContext context = HttpContext.Current;
                context.Response.StatusCode = 401;
                context.Response.AddHeader("WWW-Authenticate", "Basic Realm");
            }
        }

        private void DenyAccess(HttpApplication app)
        {
            app.Response.StatusCode = 401;
            app.Response.StatusDescription = "Access Denied";
            app.Response.Write("401 Access Denied");
            app.CompleteRequest();
        }
    }
}
