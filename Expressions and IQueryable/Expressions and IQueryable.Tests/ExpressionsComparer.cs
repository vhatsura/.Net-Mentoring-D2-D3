using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionsAndIQueryable.Tests
{
    internal static class ExpressionsComparer
    {
        public static bool ExpressionEqual(Expression x, Expression y)
        {
            // deal with the simple cases first...
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            if (x.NodeType != y.NodeType
                || x.Type != y.Type) return false;

            switch (x.NodeType)
            {
                case ExpressionType.Lambda:
                    return IsLambdaExpressionsEqual(x as LambdaExpression, y as LambdaExpression);
                case ExpressionType.MemberAccess:
                    MemberExpression mex = (MemberExpression)x, mey = (MemberExpression)y;
                    return mex.Member == mey.Member; // should really test down-stream expression

                case ExpressionType.Parameter:
                    return IsParameterExpressionsEqual(x as ParameterExpression, y as ParameterExpression);

                case ExpressionType.Convert:
                case ExpressionType.Increment:
                case ExpressionType.Decrement:
                    return IsUnaryExpressionsEqual(x as UnaryExpression, y as UnaryExpression);

                case ExpressionType.Add:
                case ExpressionType.Subtract:
                    return IsBinaryExpressionsEqual(y as BinaryExpression, x as BinaryExpression);

                case ExpressionType.Constant:
                    return IsConstantExpressionsEqual(x as ConstantExpression, y as ConstantExpression);

                default:
                    throw new NotImplementedException(x.NodeType.ToString());
            }
        }

        private static bool IsUnaryExpressionsEqual(UnaryExpression x, UnaryExpression y)
        {
            return x != null && y != null &&
                   ExpressionEqual(x.Operand, y.Operand) &&
                   (x.Method == null && y.Method == null || x.Method != null && x.Method.Equals(y.Method));
        }

        private static bool IsBinaryExpressionsEqual(BinaryExpression x, BinaryExpression y)
        {
            return x != null && y != null &&
                          ExpressionEqual(x.Left, y.Left) &&   //equal not equivalent
                          ExpressionEqual(x.Right, y.Right);
        }

        private static bool IsParameterExpressionsEqual(ParameterExpression x, ParameterExpression y)
        {
            return x != null && y != null &&
                       x.Name == y.Name &&
                       x.IsByRef == y.IsByRef;
        }

        private static bool IsConstantExpressionsEqual(ConstantExpression x, ConstantExpression y)
        {
            return x != null && y != null &&
                       x.Value.Equals(y.Value);
        }

        private static bool IsLambdaExpressionsEqual(LambdaExpression x, LambdaExpression y)
        {
            return x != null && y != null &&
                ExpressionEqual(x.Body, y.Body) &&
                IsParameterCollectionsEqual(x.Parameters, y.Parameters);
        }

        private static bool IsParameterCollectionsEqual(
            ReadOnlyCollection<ParameterExpression> x,
            ReadOnlyCollection<ParameterExpression> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;

            return x.Count == y.Count && x.Select((t, i) => ExpressionEqual(t, y[i])).All(result => result);
        }
    }
}
