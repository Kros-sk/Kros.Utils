using FluentAssertions;
using Kros.Extensions;
using Xunit;

namespace Kros.Utils.UnitTests.Extensions
{
    public class UriExtensionsShould
    {
        [Theory]
        [InlineData("google.com", "google.com")]
        [InlineData("sub.google.com", "google.com")]
        [InlineData("www.google.com", "google.com")]
        [InlineData("www.sub1.google.com", "google.com")]
        [InlineData("https://www.google.com", "google.com")]
        [InlineData("https://google.com", "google.com")]
        [InlineData("https://google.com:433", "google.com")]
        [InlineData("http://google.com", "google.com")]
        [InlineData("https://google.com/asdf", "google.com")]
        [InlineData("https://google.sk", "google.sk")]
        [InlineData("https://subdomain.google.com", "google.com")]
        [InlineData("https://sub2.sub1.google.com", "google.com")]
        [InlineData("https://com", "com")]
        public void GetDomainFromUri(string uri, string expectedDomain)
        {
            uri.GetDomain()
                .Should()
                .Be(expectedDomain);
        }
    }
}
