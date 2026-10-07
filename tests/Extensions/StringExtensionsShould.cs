using Kros.Extensions;
using System;
using System.Globalization;
using Xunit;

namespace Kros.Utils.UnitTests.Extensions
{
    public class StringExtensionsShould
    {
        [Fact]
        public void RemoveDiacriticsFromString()
        {
            const string value = "áäčďéěíľĺňńóôŕřšťúýž ÁÄČĎÉĚÍĽĹŇŃÓÔŔŘŠŤÚÝŽ";
            const string expected = "aacdeeillnnoorrstuyz AACDEEILLNNOORRSTUYZ";
            string actual = value.RemoveDiacritics();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnTrueWhenStringIsNullOrEmpty()
        {
            string? value = null;
            bool actual = value!.IsNullOrEmpty();
            Assert.True(actual, "value = null");

            value = "";
            actual = value.IsNullOrEmpty();
            Assert.True(actual, "value = \"\"");

            value = "lorem";
            actual = value.IsNullOrEmpty();
            Assert.False(actual, "value = \"lorem\"");
        }

        [Fact]
        public void ReturnTrueWhenStringIsNullOrWhitespace()
        {
            string? value = null;
            bool actual = value!.IsNullOrWhiteSpace();
            Assert.True(actual, "value = null");

            value = "";
            actual = value.IsNullOrWhiteSpace();
            Assert.True(actual, "value = \"\"");

            value = " \t \r \n ";
            actual = value.IsNullOrWhiteSpace();
            Assert.True(actual, "value = \" \\t \\r \\n \"");

            value = "lorem";
            actual = value.IsNullOrWhiteSpace();
            Assert.False(actual, "value = \"lorem\"");
        }

        [Fact]
        public void ReturnCorrectStartOfString()
        {
            const string value = "lorem ipsum dolor";
            Assert.Equal("lorem", value.Left(5));
            Assert.Equal(value, value.Left(500));
            Action action = () => value.Left(-5);
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void ReturnCorrectEndOfString()
        {
            const string value = "lorem ipsum dolor";
            Assert.Equal("dolor", value.Right(5));
            Assert.Equal(value, value.Right(500));
            Action action = () => value.Right(-5);
            Assert.Throws<ArgumentException>(action);
        }

        #region string.Format

        [Fact]
        public void CorrectlyFormatString()
        {
            Assert.Equal("a 0 b", "a {0} b".Format(0));
            Assert.Equal("a 0 1 b", "a {0} {1} b".Format(0, 1));
            Assert.Equal("a 0 1 2 b", "a {0} {1} {2} b".Format(0, 1, 2));
            Assert.Equal("a 0 1 2 3 4 b", "a {0} {1} {2} {3} {4} b".Format(0, 1, 2, 3, 4));

            CultureInfo sk = new CultureInfo("sk-SK");
            Assert.Equal("a 1,5 b", "a {0} b".Format(sk, 1.5));
            Assert.Equal("a 1,5 1,5 b", "a {0} {1} b".Format(sk, 1.5, 1.5));
            Assert.Equal("a 1,5 1,5 1,5 b", "a {0} {1} {2} b".Format(sk, 1.5, 1.5, 1.5));
            Assert.Equal("a 1,5 1,5 1,5 1,5 1,5 b", "a {0} {1} {2} {3} {4} b".Format(sk, 1.5, 1.5, 1.5, 1.5, 1.5));

            CultureInfo en = new CultureInfo("en-US");
            Assert.Equal("a 1.5 b", "a {0} b".Format(en, 1.5));
            Assert.Equal("a 1.5 1.5 b", "a {0} {1} b".Format(en, 1.5, 1.5));
            Assert.Equal("a 1.5 1.5 1.5 b", "a {0} {1} {2} b".Format(en, 1.5, 1.5, 1.5));
            Assert.Equal("a 1.5 1.5 1.5 1.5 1.5 b", "a {0} {1} {2} {3} {4} b".Format(en, 1.5, 1.5, 1.5, 1.5, 1.5));
        }

        #endregion

        #region RemoveNewLines

        [Fact]
        public void ReturnNullWhenInputIsNull()
        {
            string? actual = null;

            Assert.Null(actual.RemoveNewLines());
        }

        [Fact]
        public void ReturEmptyStringWhenInputIsEmptyString()
        {
            Assert.Equal(string.Empty, string.Empty.RemoveNewLines());
        }

        [Fact]
        public void ReturnSameStringIfItDoesNotContainNewLines()
        {
            string actual = "I ate all new lines. Yummy! No left for you";

            Assert.Equal(actual, actual.RemoveNewLines());
        }

        [Fact]
        public void RemoveStandardEnvironmentNewLines()
        {
            string actual = string.Format("{0}Lorem{0}Ipsum{0}", Environment.NewLine);

            Assert.Equal("LoremIpsum", actual.RemoveNewLines());
        }

        [Fact]
        public void RemoveNewLineCharacters()
        {
            string actual = "Meanwhile\r\n when copying\r from\n Excel\r\n";

            Assert.Equal("Meanwhile when copying from Excel", actual.RemoveNewLines());
        }

        #endregion
    }
}
