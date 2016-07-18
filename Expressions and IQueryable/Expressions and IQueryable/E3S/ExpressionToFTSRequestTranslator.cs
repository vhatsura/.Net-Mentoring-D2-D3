using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionsAndIQueryable.E3S
{
	public class ExpressionToFTSRequestTranslator : ExpressionVisitor
	{
		StringBuilder resultString;

		public string Translate(Expression exp)
		{
			resultString = new StringBuilder();
			Visit(exp);

			return resultString.ToString();
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.DeclaringType == typeof(Queryable)
				&& node.Method.Name == "Where")
			{
				var predicate = node.Arguments[1];
				Visit(predicate);

			    if (node.Arguments[0].NodeType == ExpressionType.Call)
			    {
			        return this.VisitMethodCall((MethodCallExpression) node.Arguments[0]);
			    }

				return node;
			}
		    if (node.Method.DeclaringType == typeof (string) &&
		        node.Method.Name == "StartsWith")
		    {
                IsStartsWith = true;
			    var result = base.VisitMethodCall(node);
		        IsStartsWith = false;

                return result;
		    }
            if (node.Method.DeclaringType == typeof(string) &&
                node.Method.Name == "EndsWith")
            {
                IsEndsWith = true;
                var result = base.VisitMethodCall(node);
                IsEndsWith = false;

                return result;
            }
            if (node.Method.DeclaringType == typeof(string) &&
                node.Method.Name == "Contains")
            {
                IsContains = true;
                var result = base.VisitMethodCall(node);
                IsContains = false;

                return result;
            }
            return base.VisitMethodCall(node);
		}

        private bool IsStartsWith { get; set; }
        private bool IsEndsWith { get; set; }
        private bool IsContains { get; set; }

        protected override Expression VisitBinary(BinaryExpression node)
		{
			switch (node.NodeType)
			{
				case ExpressionType.Equal:
			        if (node.Left.NodeType == ExpressionType.MemberAccess && node.Right.NodeType == ExpressionType.Constant)
			        {
			            Visit(node.Left);
			            resultString.Append("(");
			            Visit(node.Right);
			            resultString.Append(")");
			        }
                    else if(node.Left.NodeType == ExpressionType.Constant && node.Right.NodeType == ExpressionType.MemberAccess)
			        {
                        Visit(node.Right);
                        resultString.Append("(");
                        Visit(node.Left);
                        resultString.Append(")");
                    }
			        else
			        {
			            throw new NotSupportedException($"One of operands should be property or field, another should be constant. Left node type: {node.Left.NodeType}, Right node type: {node.Right.NodeType}");
			        }

					break;

				default:
					throw new NotSupportedException($"Operation {node.NodeType} is not supported");
			};

			return node;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			resultString.Append(node.Member.Name).Append(":");

			return base.VisitMember(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
		    if (IsStartsWith)
		    {
		        if (node.Value.GetType() != typeof (string))
		        {
		            throw new NotSupportedException($"Type of value {node.Value.GetType()} is not supported");
		        }
		        resultString.AppendFormat("({0}*)", node.Value);

                return node;
		    }
		    if (IsEndsWith)
		    {
                if (node.Value.GetType() != typeof(string))
                {
                    throw new NotSupportedException($"Type of value {node.Value.GetType()} is not supported");
                }
                resultString.AppendFormat("(*{0})", node.Value);

                return node;
            }
		    if (IsContains)
		    {
                if (node.Value.GetType() != typeof(string))
                {
                    throw new NotSupportedException($"Type of value {node.Value.GetType()} is not supported");
                }
                resultString.AppendFormat("(*{0}*)", node.Value);

                return node;
            }

			resultString.Append(node.Value);

			return node;
		}
	}
}
