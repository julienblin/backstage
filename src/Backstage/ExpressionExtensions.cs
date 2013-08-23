namespace Backstage
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Extension methods for <see cref="Expression"/>.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Gets the property name from the <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">
        /// The expression. The body must be a <see cref="MemberExpression"/> or a <see cref="UnaryExpression"/>.
        /// </param>
        /// <typeparam name="T">
        /// The base object type.
        /// </typeparam>
        /// <returns>
        /// The property name.
        /// </returns>
        /// <exception cref="BackstageException">
        /// If the <paramref name="expression"/> cannot be interpreted.
        /// </exception>
        public static string GetPropertyName<T>(this Expression<Func<T, object>> expression)
        {
            expression.ThrowIfNull("expression");

            var memberExpr = expression.Body as MemberExpression;
            if (memberExpr == null)
            {
                var unaryExpr = expression.Body as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member.Name;
            }

            throw new BackstageException(Resources.CouldNotDeterminePropertyName.Format(expression));
        }
    }
}
