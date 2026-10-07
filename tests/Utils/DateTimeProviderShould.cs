using System;
using Xunit;

namespace Kros.Utils.UnitTests.Utils
{
    public class DateTimeProviderShould
    {
        [Fact]
        public void ReturnCorrectDateTime()
        {
            Assert.Equal(DateTime.Now.Date, DateTimeProvider.Now.Date);
        }

        [Fact]
        public void InjectCustomDateTime()
        {
            var expected = new DateTime(2017, 10, 11, 5, 22, 33, 11);
            using (DateTimeProvider.InjectActualDateTime(expected))
            {
                Assert.Equal(expected, DateTimeProvider.Now);
            }
        }

        [Fact]
        public void ReturnCorrectDateAfterDispose()
        {
            var expected = new DateTime(2017, 10, 11, 5, 22, 33, 11);
            using (DateTimeProvider.InjectActualDateTime(expected))
            {
            }

            Assert.Equal(DateTime.Now.Date, DateTimeProvider.Now.Date);
        }

        [Fact]
        public void ReturnCorrectUtcAndLocalValues()
        {
            var injected = new DateTime(2019, 8, 10, 14, 30, 0, DateTimeKind.Utc);
            var tzOffset = TimeZoneInfo.Local.GetUtcOffset(injected);
            var local = injected + tzOffset;

            using (DateTimeProvider.InjectActualDateTime(injected))
            {
                Assert.Equal(injected, DateTimeProvider.UtcNow);
                Assert.Equal(local, DateTimeProvider.Now);
            }
        }
    }
}
