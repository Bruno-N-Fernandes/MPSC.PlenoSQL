#region

using System;

#endregion

namespace SqlEditor.SqlParser.Expressions
{
    public class CriteriaExpression : Expression
    {
        private string _operator;

        public CriteriaExpression(Expression parent) : base(parent)
        {
            Left = new Expression(this);
            Right = new Expression(this);
        }

        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public string Operator
        {
            get { return _operator; }
            set { _operator = value.ToUpper(); }
        }

        public override string Value
        {
            get { return String.Format("{0} {1} {2}", Left.Value, Operator, Right.Value); }
        }

        public override bool CanInline
        {
            get { return Operator != Constants.Or; }
        }
    }
}