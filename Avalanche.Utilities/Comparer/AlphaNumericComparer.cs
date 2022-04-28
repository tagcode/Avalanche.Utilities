﻿// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections;
using System.Globalization;
using System.Text;

/// <summary>
/// Alpha numeric string comparer. 
/// 
/// Considers number sequences and text sequences as one individual comparable tokens .
/// 
/// For example: strings "a1", "a10", "a9" would be sorted to "a1", "a9", "a10".
/// 
/// Decimal separators are not supported, as they could be mixed with other separators.
/// Exponents are not supported.
/// </summary>
public class AlphaNumericComparer : IComparer, IComparer<string>
{
    static AlphaNumericComparer _InvariantCulture = new AlphaNumericComparer(Token.Comparer.InvariantCulture);
    static AlphaNumericComparer _InvariantCultureIgnoreCase = new AlphaNumericComparer(Token.Comparer.InvariantCultureIgnoreCase);
    static AlphaNumericComparer _CurrentCulture = new AlphaNumericComparer(Token.Comparer.CurrentCulture);
    static AlphaNumericComparer _CurrentCultureIgnoreCase = new AlphaNumericComparer(Token.Comparer.CurrentCultureIgnoreCase);
    static AlphaNumericComparer _CurrentUICulture = new AlphaNumericComparer(Token.Comparer.CurrentUICulture);
    static AlphaNumericComparer _CurrentUICultureIgnoreCase = new AlphaNumericComparer(Token.Comparer.CurrentUICultureIgnoreCase);

    /// <summary>Comparer</summary>
    public static AlphaNumericComparer InvariantCulture => _InvariantCulture;
    /// <summary>Comparer</summary>
    public static AlphaNumericComparer InvariantCultureIgnoreCase => _InvariantCultureIgnoreCase;
    /// <summary>Comparer</summary>
    public static AlphaNumericComparer CurrentCulture => _CurrentCulture;
    /// <summary>Comparer</summary>
    public static AlphaNumericComparer CurrentCultureIgnoreCase => _CurrentCultureIgnoreCase;
    /// <summary>Comparer</summary>
    public static AlphaNumericComparer CurrentUICulture => _CurrentUICulture;
    /// <summary>Comparer</summary>
    public static AlphaNumericComparer CurrentUICultureIgnoreCase => _CurrentUICultureIgnoreCase;

    /// <summary>Singleton instance</summary>
    public static AlphaNumericComparer Default => _InvariantCulture;

    /// <summary>Token specific comparer</summary>
    IComparer<Token> tokenComparer;

    /// <summary>Create alphanumeric comparer</summary>
    /// <param name="tokenComparer"></param>
    public AlphaNumericComparer(IComparer<Token> tokenComparer)
    {
        this.tokenComparer = tokenComparer ?? throw new ArgumentNullException(nameof(tokenComparer));
    }

    /// <summary>Compare uncasted objects. Calls ToString().</summary>
    public int Compare(object? x, object? y)
        => Compare(x?.ToString(), y?.ToString());

    /// <summary>Compare strings.</summary>
    public int Compare(string? x, string? y)
    {
        // Handle nulls
        if (x == null && y == null) return 0;
        if (x == null && y != null) return -1;
        if (x != null && y == null) return 1;

        // Token enumerators
        TokenEnumerator x_etor = new TokenEnumerator(x!), y_etor = new TokenEnumerator(y!);

        // Has next tokens
        bool x_next = x_etor.MoveNext(), y_next = y_etor.MoveNext();
        // Get tokens
        Token x_token = x_etor.Current, y_token = y_etor.Current;
        // Iterate
        while (x_next && y_next)
        {
            // Equal length
            if (x_token.Length == y_token.Length || x_token.Kind == Kind.Numeric || y_token.Kind == Kind.Numeric)
            {
                // Compare tokens
                int d = tokenComparer.Compare(x_token, y_token);
                // Difference
                if (d != 0) return d;
                // Read more
                x_next = x_etor.MoveNext();
                y_next = y_etor.MoveNext();
                x_token = x_etor.Current;
                y_token = y_etor.Current;
            }
            // x is shorter
            else if (x_token.Length < y_token.Length)
            {
                Token y_left = y_token.Substring(0, x_token.Length);
                // Compare tokens
                int d = tokenComparer.Compare(x_token, y_left);
                // Difference
                if (d != 0) return d;
                // Read more
                y_token = y_token.Substring(x_token.Length, y_token.Length - x_token.Length);
                x_next = x_etor.MoveNext();
                x_token = x_etor.Current;
            }
            else
            // y is shorter
            {
                Token x_left = x_token.Substring(0, y_token.Length);
                // Compare tokens
                int d = tokenComparer.Compare(x_left, y_token);
                // Difference
                if (d != 0) return d;
                // Read more
                x_token = x_token.Substring(y_token.Length, x_token.Length - y_token.Length);
                y_next = y_etor.MoveNext();
                y_token = y_etor.Current;
            }
        }

        // Has more x, end of y.
        if (x_next && !y_next) return 1;
        // End of x, has more x.
        if (!x_next && y_next) return -1;
        // Equals
        return 0;
    }

    /// <summary>token enumerable</summary>
    struct TokenEnumerable : IEnumerable<Token>
    {
        String str;
        public TokenEnumerable(String str)
        {
            this.str = str;
        }
        public IEnumerator<Token> GetEnumerator() => new TokenEnumerator(str);
        IEnumerator IEnumerable.GetEnumerator() => new TokenEnumerator(str);
    }

    /// <summary>Enumerator</summary>
    struct TokenEnumerator : IEnumerator<Token>
    {
        Token current;
        object IEnumerator.Current => current;

        /// <summary>Source string. Nulled on dispose</summary>
        String? str;
        /// <summary>Character index, -1=start</summary>
        int ix;
        /// <summary>Current token</summary>
        public Token Current => current;

        /// <summary>Create enumerator</summary>
        /// <param name="str"></param>
        public TokenEnumerator(String str)
        {
            this.str = str;
            this.ix = -1;
            this.current = new Token(Kind.Other, "");
        }
        /// <summary>Dispose</summary>
        public void Dispose() => str = null;
        /// <summary>Move cursor to beginning.</summary>
        public void Reset()
        {
            this.ix = -1;
            this.current = new Token(Kind.Other, "");
        }

        enum State
        {
            Unset = 0,
            CouldBeNumber = 20,
            Number = 21,
            Text = 30,
            Other = 40
        }

        public bool MoveNext()
        {
            // Null
            if (str == null) return false;

            // End of string
            if (++ix >= str.Length) return false;

            // Start index, end index (exclusive)
            int startIx = ix, endIx = ix;

            // Unset state
            State state = State.Unset;

            // Move until end or change of token
            while (ix < str.Length)
            {
                // Get char and move
                char c = str[ix++];

                // Start of token
                if (state == State.Unset)
                {
                    // Character
                    if (char.IsLetter(c)) { endIx = ix; state = State.Text; continue; }
                    // Number
                    if (char.IsNumber(c)) { endIx = ix; state = State.Number; continue; }
                    // - / +
                    if (c == '-' || c == '+') { endIx = ix; state = State.CouldBeNumber; continue; }

                    endIx = ix;
                    state = State.Other;
                    continue;
                }

                // +/- [?]
                if (state == State.CouldBeNumber)
                {
                    if (char.IsNumber(c)) { endIx = ix; state = State.Number; continue; }
                    state = State.Other;
                }

                // 0-9
                if (state == State.Number)
                {
                    if (char.IsNumber(c)) { endIx = ix; continue; }
                    break;
                }

                // a-zA-Z + others
                if (state == State.Text)
                {
                    if (char.IsLetter(c)) { endIx = ix; continue; }
                    break;
                }

                // Other
                {
                    if (char.IsLetter(c) || char.IsNumber(c)) break;
                    endIx = ix;
                    continue;
                }
            }

            // Got nothing
            if (state == State.Unset) { current = Token.Empty; return false; }
            // Move cursor
            ix = endIx - 1;
            // Choose kind
            Kind kind = state == State.CouldBeNumber || state == State.Other ? Kind.Other :
                state == State.Text ? Kind.Text :
                Kind.Numeric;
            // Create current token
            current = new Token(kind, str, startIx, endIx - startIx);
            // Ok.
            return true;
        }
    }

    /// <summary>Token kind</summary>
    public enum Kind : int
    {
        /// <summary>Numeric segment</summary>
        Numeric = 1,
        /// <summary>Text segment</summary>
        Text = 2,
        /// <summary>Other characters</summary>
        Other = 3
    }

    /// <summary>Token info</summary>
    public struct Token : IEquatable<Token>
    {
        /// <summary>Character at <paramref name="ix"/></summary>
        public char this[int ix] => String[Start + ix];
        /// <summary>Empty string ""</summary>
        public static Token Empty = new Token(Kind.Other, "");

        /// <summary>Token kind</summary>
        public readonly Kind Kind;

        /// <summary>Start index</summary>
        public readonly int Start;
        /// <summary>Length</summary>
        public readonly int Length;
        /// <summary>String</summary>
        public readonly string String;

        /// <summary>Implicit converter</summary>
        public static implicit operator String(Token str) => str.String.Substring(str.Start, str.Length);

        /// <summary>Create token</summary>
        /// <param name="kind"></param>
        /// <param name="str"></param>
        public Token(Kind kind, string str)
        {
            this.Kind = kind;
            this.String = str ?? throw new ArgumentNullException(nameof(str));
            this.Start = 0;
            this.Length = str.Length;
        }

        /// <summary>
        /// Create span of characters.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="str"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public Token(Kind kind, string str, int start, int length)
        {
            this.Kind = kind;
            this.String = str ?? throw new ArgumentNullException(nameof(str));
            if (start < 0 || start > str.Length) throw new ArgumentOutOfRangeException(nameof(start));
            if (length < 0 || start + length > str.Length) throw new ArgumentOutOfRangeException(nameof(length));
            this.Start = start;
            this.Length = length;
        }

        /// <summary>
        /// Create substring token
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns>substring token</returns>
        public Token Substring(int start, int length)
            => new Token(Kind, String, this.Start + start, length);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            // Not token
            if (obj is not Token ss) return false;
            // Token kind mismatch
            if (ss.Kind != Kind) return false;
            // Position or reference mismatch
            if (ss.String == String && ss.Start == Start && ss.Length == Length) return true;
            //if (ss.hashcode != hashcode) return false;
            // Length mismatch
            if (ss.Length != Length) return false;
            // Compare each character
            for (int i = 0; i < Length; i++)
            {
                char c1 = String[Start + i], c2 = ss.String[ss.Start + i];
                if (c1 != c2) return false;
            }
            // Equals
            return true;
        }

        /// <summary></summary>
        public static bool operator ==(Token a, Token b)
        {
            if (a.Kind != b.Kind) return false;
            if (a.String == b.String && a.Start == b.Start && a.Length == b.Length) return true;
            //if (ss.hashcode != hashcode) return false;
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                char c1 = a.String[a.Start + i], c2 = b.String[b.Start + i];
                if (c1 != c2) return false;
            }
            return true;
        }

        /// <summary></summary>
        public static bool operator !=(Token a, Token b)
        {
            if (a.Kind == b.Kind) return false;
            if (a.String == b.String && a.Start == b.Start && a.Length == b.Length) return false;
            //if (ss.hashcode != hashcode) return false;
            if (a.Length != b.Length) return true;
            for (int i = 0; i < a.Length; i++)
            {
                char c1 = a.String[a.Start + i], c2 = b.String[b.Start + i];
                if (c1 != c2) return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // Calculate hashcode from content
            int hash = unchecked((int)2166136261);
            for (int i = 0; i < Length; i++)
            {
                hash ^= (int)String[i + Start];
                hash *= 16777619;
            }
            return hash;
        }

        /// <summary>Append to string builder</summary>
        public void AppendTo(StringBuilder sb) => sb.Append(String, Start, Length);
        /// <inheritdoc/>
        public override string ToString() => String.Substring(Start, Length);
        /// <inheritdoc/>
        public bool Equals(Token other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<Token>
        {
            static Comparer _InvariantCulture = new Comparer(CultureInfo.InvariantCulture, ignoreCase: false);
            static Comparer _InvariantCulture_IgnoreCase = new Comparer(CultureInfo.InvariantCulture, ignoreCase: true);
            static Comparer _CurrentCulture = new Comparer(() => CultureInfo.CurrentCulture, ignoreCase: false);
            static Comparer _CurrentCulture_IgnoreCase = new Comparer(() => CultureInfo.CurrentCulture, ignoreCase: true);
            static Comparer _CurrentUICulture = new Comparer(() => CultureInfo.CurrentUICulture, ignoreCase: false);
            static Comparer _CurrentUICulture_IgnoreCase = new Comparer(() => CultureInfo.CurrentUICulture, ignoreCase: true);

            /// <summary>Comparer</summary>
            public static Comparer InvariantCulture => _InvariantCulture;
            /// <summary>Comparer</summary>
            public static Comparer InvariantCultureIgnoreCase => _InvariantCulture_IgnoreCase;
            /// <summary>Comparer</summary>
            public static Comparer CurrentCulture => _CurrentCulture;
            /// <summary>Comparer</summary>
            public static Comparer CurrentCultureIgnoreCase => _CurrentCulture_IgnoreCase;
            /// <summary>Comparer</summary>
            public static Comparer CurrentUICulture => _CurrentUICulture;
            /// <summary>Comparer</summary>
            public static Comparer CurrentUICultureIgnoreCase => _CurrentUICulture_IgnoreCase;

            /// <summary>Culture Info function</summary>
            Func<CultureInfo>? CultureInfoFunc;
            /// <summary>Culture Info</summary>
            CultureInfo? CultureInfo;
            /// <summary>Ignore Case</summary>
            public readonly bool IgnoreCase;

            /// <summary>Create comparer</summary>
            /// <param name="cultureInfo"></param>
            /// <param name="ignoreCase"></param>
            public Comparer(CultureInfo cultureInfo, bool ignoreCase)
            {
                this.CultureInfoFunc = null;
                this.CultureInfo = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
                this.IgnoreCase = ignoreCase;
            }

            /// <summary>Create comparer</summary>
            /// <param name="cultureInfoFunc"></param>
            /// <param name="ignoreCase"></param>
            public Comparer(Func<CultureInfo> cultureInfoFunc, bool ignoreCase)
            {
                this.CultureInfoFunc = cultureInfoFunc ?? throw new ArgumentNullException(nameof(cultureInfoFunc));
                this.CultureInfo = null;
                this.IgnoreCase = ignoreCase;
            }

            /// <summary>Compare tokens</summary>
            public int Compare(Token x, Token y)
            {
                // Compare kinds
                int d = ((int)x.Kind) - ((int)y.Kind);

                // Different kinds
                if (d != 0)
                {
                    // Min Length
                    int minLength = x.Length < y.Length ? x.Length : y.Length;
                    // Compare common characters
                    int c = string.Compare(x.String, x.Start, y.String, y.Start, minLength, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                    // Difference
                    if (c != 0) return c;
                    // Compare by length
                    return x.Length - y.Length;
                }

                // Zero length
                if (x.Length == 0 || y.Length == 0)
                    return x.Length - y.Length;

                // Compare strings
                if (x.Kind == Kind.Text)
                {
                    // Min Length
                    int minLength = x.Length < y.Length ? x.Length : y.Length;
                    // Compare common characters
                    d = string.Compare(x.String, x.Start, y.String, y.Start, minLength, IgnoreCase, CultureInfo ?? CultureInfoFunc!());
                    // Difference
                    if (d != 0) return d;
                    // Compare by length
                    return x.Length - y.Length;
                }

                // Compare other string sequences with ordinal comparer
                if (x.Kind == Kind.Other)
                {
                    // Min Length
                    int minLength = x.Length < y.Length ? x.Length : y.Length;
                    // Compare common characters
                    d = string.Compare(x.String, x.Start, y.String, y.Start, minLength, StringComparison.Ordinal);
                    // Difference
                    if (d != 0) return d;
                    // Compare by length
                    return x.Length - y.Length;
                }

                // Compare numbers
                if (x.Kind == Kind.Numeric)
                {
                    // Get culture
                    CultureInfo ci = CultureInfo ?? CultureInfoFunc!();
                    // Ordinal comparison
                    if (x.Length >= 28 || y.Length >= 28)
                    {
                        // Min Length
                        int minLength = x.Length < y.Length ? x.Length : y.Length;
                        // Compare common characters
                        d = string.Compare(x.String, x.Start, y.String, y.Start, minLength, StringComparison.Ordinal);
                        // Difference
                        if (d != 0) return d;
                        // Compare by length
                        return x.Length - y.Length;
                    }
                    // Decimal comparison
                    if (x.Length >= 18 || y.Length >= 18)
                    {
                        // Parse
                        decimal x_value = decimal.Parse(x, ci), y_value = decimal.Parse(y, ci);
                        // Compare
                        d = x_value.CompareTo(y_value);
                        // Delta
                        if (d != 0) return d;
                    }
                    else
                    // Int64 comparison
                    {
                        // Parse
                        long x_value = long.Parse(x, ci), y_value = long.Parse(y, ci);
                        // Compare
                        d = x_value.CompareTo(y_value);
                        // Delta
                        if (d != 0) return d;
                    }
                    // Compare by length again
                    return x.Length - y.Length;
                }

                // Shouldn't go here
                return 0;
            }
        }

        /// <summary>EqualityComparer</summary>
        public class EqualityComparer : IEqualityComparer<Token>
        {
            /// <summary>Singleton instance</summary>
            private static EqualityComparer instance => new EqualityComparer();
            /// <summary>Singleton instance</summary>
            public static EqualityComparer Instance => instance;

            /// <summary>Compare for equal content</summary>
            public bool Equals(Token x, Token y)
            {
                // Check kind
                if (x.Kind != y.Kind) return false;

                // Check for content equality
                if (x.String == y.String && x.Start == y.Start && x.Length == y.Length) return true;

                // Compare hashcode
                //if (x.hashcode != y.hashcode) return false;

                // Compare lengths
                if (x.Length != y.Length) return false;

                // Compare characters
                for (int i = 0; i < x.Length; i++)
                {
                    char c1 = x.String[x.Start + i], c2 = y.String[y.Start + i];
                    if (c1 != c2) return false;
                }

                return true;
            }

            /// <summary>Calculate hashcode</summary>
            public int GetHashCode(Token obj) => obj.GetHashCode();
        }
    }


}
