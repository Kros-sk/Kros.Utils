using Kros.Net;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Kros.Utils.UnitTests.Net
{
    public class HttpResponseMessageExtensionsTests
    {
        private const string HtmlContent = @"<html>
<head>
    <meta charset=""UTF-8"">
</head>
<body>
    <form method=""post"" action=""/"">
        <h3>Login</h3>
        <label for=""Email"">E-mail</label>
        <input type=""email"" id=""Email"" name=""Email"" value="""" />
        <label for=""Password"">Password</label>
        <input type=""password"" id=""Password"" name=""Password"" />
        <input name=""__RequestVerificationToken"" type=""hidden"" value=""anti-forgery-token"" />
        <input name=""RememberMe"" type=""hidden"" value=""false"" />
    </form>
  </body>
</html>";

        [Fact]
        public async Task AntiForgeryTokenMustBeNullIfResponseIsNotSuccessful()
        {
            var message = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(HtmlContent)
            };
            string? token = await message.GetAntiForgeryTokenAsync();
            Assert.Null(token);
        }

        [Fact]
        public async Task ShouldExtractAntiForgeryToken()
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(HtmlContent)
            };
            string? token = await message.GetAntiForgeryTokenAsync();
            Assert.Equal("anti-forgery-token", token);
        }

        [Fact]
        public void CookieShouldBeSetAndRetrieved()
        {
            var response = new HttpResponseMessage();
            response.SetCookie("cookie1", "value1");

            var expectedCookie = new SetCookieHeaderValue("cookie1", "value1");

            IList<SetCookieHeaderValue> actualCookies = response.GetCookies();
            SetCookieHeaderValue actualCookie = Assert.Single(actualCookies);
            Assert.Equal(expectedCookie.ToString(), actualCookie.ToString());
        }

        [Fact]
        public void CookiesShouldBeSetAndRetrieved()
        {
            var cookies = new Dictionary<string, string>()
            {
                { "cookie1", "value1" },
                { "cookie2", "value2" },
                { "cookie3", "value3" }
            };

            var response = new HttpResponseMessage();
            response.SetCookies(cookies);

            var expectedCookies = new List<SetCookieHeaderValue>()
            {
                new SetCookieHeaderValue("cookie1", "value1"),
                new SetCookieHeaderValue("cookie2", "value2"),
                new SetCookieHeaderValue("cookie3", "value3")
            };

            IList<SetCookieHeaderValue> actualCookies = response.GetCookies();
            Assert.Equal(expectedCookies.Select(c => c.ToString()), actualCookies.Select(c => c.ToString()));
        }

        [Fact]
        public void CookiesShouldBeSetAndRetrievedAsDictionary()
        {
            var cookies = new Dictionary<string, string>()
            {
                { "cookie1", "value1" },
                { "cookie2", "value2" },
                { "cookie3", "value3" }
            };

            var response = new HttpResponseMessage();
            response.SetCookies(cookies);

            IDictionary<string, string> actualCookies = response.GetCookieValues();
            Assert.Equal(cookies, actualCookies);
        }
    }
}
