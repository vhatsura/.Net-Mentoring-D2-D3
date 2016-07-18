using System.Collections.Generic;
using System.Linq;
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
            if (IsChangeParametersMode)
            {
                return base.VisitBinary(node);
            }
            var parser = new BinaryExpressionParser();
            parser.Parse(node);

            if (parser.IsIncrement)
            {
                return Expression.Increment(parser.Parameter);
            }

            return parser.IsDecrement ? Expression.Decrement(parser.Parameter) : base.VisitBinary(node);
        }

        private Dictionary<string, object> m_Values;

        private bool IsChangeParametersMode => m_Values != null;

        public Expression ChangeParametersToConstants(Expression expression, Dictionary<string, object> values)
        {
            if (expression.NodeType == ExpressionType.Lambda)
            {
                m_Values = values;

                var resultExpression = (LambdaExpression) this.VisitAndConvert(expression, string.Empty);
                if (resultExpression != null)
                {
                    var parameters = resultExpression.Parameters.Where(parameter => !values.ContainsKey(parameter.Name));

                    return Expression.Lambda(resultExpression.Body, parameters);
                }
            }

            return expression;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return Expression.Lambda(Visit(node.Body), node.Parameters);
        }

        protected override Expression VisitParameter(ParameterExpression expression)
        {
            if (m_Values != null && m_Values.ContainsKey(expression.Name))
            {
                return Expression.Constant(m_Values[expression.Name]);
            }

            return base.VisitParameter(expression);
        }
    }
}
