using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GR.DynamicEntityStorage.Utils
{
    public class QueryTranslator : ExpressionVisitor
    {
        private StringBuilder sb;
        private string _orderBy = string.Empty;
        private int? _skip = null;
        private int? _take = null;
        private string _whereClause = string.Empty;

        public int? Skip
        {
            get
            {
                return _skip;
            }
        }

        public int? Take
        {
            get
            {
                return _take;
            }
        }

        public string OrderBy
        {
            get
            {
                return _orderBy;
            }
        }

        public string WhereClause
        {
            get
            {
                return _whereClause;
            }
        }

        public QueryTranslator()
        {
        }

        public string Translate<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            this.sb = new StringBuilder();
            this.Visit(expression);
            _whereClause = this.sb.ToString();
            return _whereClause;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                this.Visit(m.Arguments[0]);
                var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                return m;
            }
            else if (m.Method.Name == "Take")
            {
                if (this.ParseTakeExpression(m))
                {
                    var nextExpression = m.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (m.Method.Name == "Skip")
            {
                if (this.ParseSkipExpression(m))
                {
                    var nextExpression = m.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (m.Method.Name == "OrderBy")
            {
                if (this.ParseOrderByExpression(m, "ASC"))
                {
                    var nextExpression = m.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (m.Method.Name == "OrderByDescending")
            {
                if (this.ParseOrderByExpression(m, "DESC"))
                {
                    var nextExpression = m.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (m.Method.Name == "Contains")
            {
                var nextExpression = m.Arguments[0];
                sb.AppendFormat(" {0} LIKE '%", nextExpression);
                return this.Visit(nextExpression);
            }
            else if (m.Method.Name == "StartsWith")
            {
                var nextExpression = m.Arguments[0];
                sb.AppendFormat(" {0} LIKE '%", nextExpression);
                return this.Visit(nextExpression);
            }
            throw new NotSupportedException($"The method '{m.Method.Name}' is not supported");
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException($"The unary operator '{u.NodeType}' is not supported");
            }
            return u;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            //sb.Append("(");
            this.Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.Or:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.OrElse:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.Equal:
                    sb.Append(IsNullConstant(b.Right) ? " IS " : " = ");
                    break;

                case ExpressionType.NotEqual:
                    sb.Append(IsNullConstant(b.Right) ? " IS NOT " : " <> ");
                    break;

                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;

                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;

                default:
                    throw new NotSupportedException($"The binary operator '{b.NodeType}' is not supported");

            }

            this.Visit(b.Right);
            //sb.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            var q = c.Value as IQueryable;

            if (q == null && c.Value == null)
            {
                sb.Append("NULL");
            }
            else if (q == null)
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        sb.Append(((bool)c.Value) ? 1 : 0);
                        break;

                    case TypeCode.String:
                        sb.Append("'");
                        sb.Append(c.Value);
                        sb.Append("'");
                        break;

                    case TypeCode.DateTime:
                        sb.Append("'");
                        sb.Append(c.Value);
                        sb.Append("'");
                        break;

                    case TypeCode.Object:
                        throw new NotSupportedException($"The constant for '{c.Value}' is not supported");

                    default:
                        sb.Append(c.Value);
                        break;
                }
            }

            return c;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null && (m.Expression.NodeType == ExpressionType.Parameter || m.Expression.NodeType == ExpressionType.Constant))
            {
                sb.Append(m.Member.Name);
                return m;
            }

            throw new NotSupportedException($"The member '{m.Member.Name}' is not supported");
        }

        protected bool IsNullConstant(Expression exp)
        {
            return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
        }

        private bool ParseOrderByExpression(MethodCallExpression expression, string order)
        {
            var unary = (UnaryExpression)expression.Arguments[1];
            var lambdaExpression = (LambdaExpression)unary.Operand;

            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            if (!(lambdaExpression.Body is MemberExpression body)) return false;
            _orderBy = string.IsNullOrEmpty(_orderBy) ? $"{body.Member.Name} {order}" : $"{_orderBy}, {body.Member.Name} {order}";

            return true;

        }

        private bool ParseTakeExpression(MethodCallExpression expression)
        {
            var sizeExpression = (ConstantExpression)expression.Arguments[1];

            if (!int.TryParse(sizeExpression.Value.ToString(), out var size)) return false;
            _take = size;
            return true;

        }

        private bool ParseSkipExpression(MethodCallExpression expression)
        {
            var sizeExpression = (ConstantExpression)expression.Arguments[1];

            if (int.TryParse(sizeExpression.Value.ToString(), out var size))
            {
                _skip = size;
                return true;
            }

            return false;
        }
    }
}
