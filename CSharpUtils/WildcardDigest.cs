namespace Utils.Wildcard
{
    /// <summary>
    /// Accelerate case-insensitive wildcard matches (or regular string contains matches)
    /// by creating a digest of the target string, a digest of the query string
    /// and calling target.MaybeMatches(wildcard). The cost of the check is
    /// a comparison and bitwise and on a ulong.
    /// </summary>
    public struct WildcardDigest
    {
        private const string Characters = " !#$%&'()+,-./0123456789:;<=>@[]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{}";
        private const int MaxAsciiValue = 127;
        private static readonly ulong[] Bits;
        private ulong value;

        /// <summary>
        /// Create a string digest
        /// </summary>
        /// <param name="s">The string to digest.</param>
        /// <returns>A digest which can be used to quickly eliminate
        /// strings which will not match a wildcard check.</returns>
        public static WildcardDigest Create(string s)
        {
            WildcardDigest digest = new WildcardDigest();

            for (int index = 0; index < s.Length; ++index)
                digest.value |= s[index] == '?' || s[index] == '*' ? 0
                 : s[index] > MaxAsciiValue ? 1 : Bits[s[index]];

            return digest;
        }

        /// <summary>
        /// Return true if the string which produced this digest might
        /// match the digest produced by the wildcard string.
        /// </summary>
        /// <param name="wildcard">The digest of the wildcard pattern</param>
        /// <returns>True, if but not only if the string is a match.</returns>
        public bool MaybeMatches(WildcardDigest wildcard)
            => wildcard.value == (value & wildcard.value);

        static WildcardDigest()
        {
            Bits = new ulong[MaxAsciiValue + 1];

            // Each character uses a bit from the 64 bit mask
            // 1 is used to indicate that the string contains
            // a character not in the characters set.

            for (int index = 0; index <= MaxAsciiValue; ++index)
                Bits[index] = 1;

            ulong bit = 1;

            for (int index = 0; index < Characters.Length; ++index)
            {
                int c = Characters[index];
                Bits[c] = bit <<= 1;
                if (c >= 'A' && c <= 'Z')
                    Bits[c | 32] = bit;
            }
        }
    }
}