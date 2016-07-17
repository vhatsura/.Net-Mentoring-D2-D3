using System.Linq.Expressions;

namespace ExpressionsAndIQueryable
{
    public sealed class CustomExpressionTransformer : ExpressionVisitor
    {
        private sealed class BinaryExpressionParser
        {
            public ParameterExpression Parameter { get; private set; }
            public ConstantExpression Constant { get; private set; }

            public bool IsIncrement => IsValid && Type == ExpressionType.Add;

            public bool IsDecrement => IsValid && Type == ExpressionType.Subtract;

            private ExpressionType Type { get; set; }

            private bool IsValid => Parameter != null && Constant != null && Constant.Type == typeof(int) && (int)Constant.Value == 1;

            public void Parse(BinaryExpression node)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                        ParseAddBinaryExpression(node);
                        break;
                    case ExpressionType.Subtract:
                        ParseSubtractBinaryExpression(node);
                        break;
                    default:
                        Type = node.NodeType;
                        break;
                }
            }

            private void ParseAddBinaryExpression(BinaryExpression node)
            {
                Type = node.NodeType;

                if (node.Left.NodeType == ExpressionType.Parameter)
                {
                    Parameter = (ParameterExpression)node.Left;
                }
                else if (node.Left.NodeType == ExpressionType.Constant)
                {
                    Constant = (ConstantExpression)node.Left;
                }

                if (node.Right.NodeType == ExpressionType.Parameter)
                {
                    Parameter = (ParameterExpression)node.Right;
                }
                else if (node.Right.NodeType == ExpressionType.Constant)
                {
                    Constant = (ConstantExpression)node.Right;
                }
            }

            private void ParseSubtractBinaryExpression(BinaryExpression node)
            {
                Type = node.NodeType;

                if (node.Left.NodeType == ExpressionType.Parameter)
                {
                    Parameter = (ParameterExpression)node.Left;
                    if (node.Right.NodeType == ExpressionType.Constant)
                    {
                        Constant = (ConstantExpression)node.Right;
                    }
                }
            }
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var parser = new BinaryExpressionParser();
            parser.Parse(node);

            if (parser.IsIncrement)
            {
                return Expression.Increment(parser.Parameter);
            }

            return parser.IsDecrement ? Expression.Decrement(parser.Parameter) : base.VisitBinary(node);
        }
    }
}
