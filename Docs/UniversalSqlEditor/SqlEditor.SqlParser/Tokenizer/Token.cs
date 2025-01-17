#region

using System;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace SqlEditor.SqlParser.Tokenizer
{
    public class Token
    {
        public Token()
        {
            Type = TokenType.None;
            Value = "";
        }

        public Token(string value, TokenType type)
        {
            Value = value;
            Type = type;
        }

        public string Value { get; set; }
        public TokenType Type { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var casted = obj as Token;
            if (casted == (Token) null)
                return false;

            return Value.Equals(casted.Value) && Type.Equals(casted.Type);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Token one, string two)
        {
            if (ReferenceEquals(one, null) && ReferenceEquals(two, null))
                return true;

            if (ReferenceEquals(one, null) || ReferenceEquals(two, null))
                return false;

            return one.Value != null && one.Value.ToLower() == two.ToLower();
        }

        public static bool operator !=(Token one, string two)
        {
            return !(one == two);
        }

        public static bool operator ==(Token one, Token two)
        {
            if (ReferenceEquals(one, null) && ReferenceEquals(two, null))
                return true;

            if (ReferenceEquals(one, null) || ReferenceEquals(two, null))
                return false;

            return one.Value.ToLower() == two.Value.ToLower();
        }

        public static bool operator !=(Token one, Token two)
        {
            return !(one == two);
        }

        public bool IsEmpty()
        {
            return Value.Length == 0;
        }

        public bool IsTypeIn(params TokenType[] tokenTypes)
        {
            return tokenTypes.Any(tt => tt == Type);
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Value, Type);
        }
    }
}