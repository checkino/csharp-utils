using Xunit;

namespace Utils.Wildcard.Tests
{
    public class WildcardDigestTests
    {
        [Theory]
        [InlineData("ab*", "cd", false)]
        [InlineData("ab?", "cd", false)]
        [InlineData("ab*", "ab", true)]
        [InlineData("ab?", "ab", true)]
        [InlineData("`", "`", true)]
        [InlineData("A", "A", true)]
        [InlineData("B", "A", false)]
        [InlineData("", "", true)]
        [InlineData("", "A", true)]
        public void WildcardDigestReturnsFalseForNonMatches(string wildcard, string testString, bool maybeMatches)
        {
            WildcardDigest wildcardDigest = WildcardDigest.Create(wildcard);
            WildcardDigest testStringDigest = WildcardDigest.Create(testString);

            Assert.Equal(maybeMatches, testStringDigest.MaybeMatches(wildcardDigest));
        }
    }
}