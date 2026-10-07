using Kros.IO;
using System;
using Xunit;

namespace Kros.Utils.UnitTest.IO
{
    public class PathHelperShould
    {
        #region BuildPath

        [Fact]
        public void ThrowArgumentNullExceptionWhenInputIsNull()
        {
            Action action = () => PathHelper.BuildPath(null!);
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenAnyPartIsNull()
        {
            Action action = () => PathHelper.BuildPath("lorem", null!, "ipsum");
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenAnyPartContainsInvalidPathCharacters()
        {
            Action action = () => PathHelper.BuildPath("lorem", "ips|um");
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void CombinePathParts()
            => Assert.Equal(@"lorem/ipsum/dolor/sit/amet", PathHelper.BuildPath("lorem", "ipsum", "dolor", "sit", "amet"));

        [Fact]
        public void CombinePathPartsWithDirectorySeparatorAtBeginning()
            => Assert.Equal(@"/lorem/ipsum/dolor/sit/amet", PathHelper.BuildPath("\\lorem", "\\ipsum", "\\dolor\\sit", "\\amet"));

        [Fact]
        public void CombinePathPartsWithVolumeInfo()
            => Assert.Equal(@"c:/lorem/ipsum/dolor/sit/amet", PathHelper.BuildPath("c:", "lorem", "ipsum", "dolor", "sit", "amet"));

        [Fact]
        public void CombinePathPartsWithVolumeInfoAndDirectorySeparator()
            => Assert.Equal(@"c:/lorem/ipsum/dolor/sit/amet", PathHelper.BuildPath("c:\\", "lorem", "ipsum", "dolor", "sit", "amet"));

        [Fact]
        public void InsertOnlyOneSeparatorBetweenParts()
            => Assert.Equal(@"/lorem/ipsum/dolor/sit/amet/", PathHelper.BuildPath("\\lorem\\", "\\ipsum\\", "\\dolor\\", "\\sit\\", "\\amet\\"));

        #endregion

        #region ReplaceInvalidPathChars

        [Fact]
        public void ReturnEmptyStringWhenPathNameIsNull()
            => Assert.Equal(string.Empty, PathHelper.ReplaceInvalidPathChars(null!));

        [Fact]
        public void ReplaceWithEmptyStringInPathNameWhenReplacementIsNull()
            => Assert.Equal("az", PathHelper.ReplaceInvalidPathChars("a*z", null!));

        [Fact]
        public void ReplaceInvalidCharsInPathName()
            => Assert.Equal("a-b-c-d-e-f-g", PathHelper.ReplaceInvalidPathChars("a\\b/c*d<e>f>g"));

        [Fact]
        public void ReplaceInvalidCharGroupsWithSingleReplacementInPathName()
            => Assert.Equal("a-b-c", PathHelper.ReplaceInvalidPathChars("a\\/*b<>c"));

        [Fact]
        public void ReplaceInvalidCharsInPathNameWithCustomReplacement()
            => Assert.Equal("a=b=c=d=e=f=g", PathHelper.ReplaceInvalidPathChars("a\\b/c*d<e>f>g", "="));

        #endregion

        #region GetTempPath

        [Fact]
        public void GetTempPathDoesntEndWithSlash()
        {
            string systemTempPath = System.IO.Path.GetTempPath();
            string expected = systemTempPath.Remove(systemTempPath.Length - 1, 1);
            string actual = PathHelper.GetTempPath();

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
