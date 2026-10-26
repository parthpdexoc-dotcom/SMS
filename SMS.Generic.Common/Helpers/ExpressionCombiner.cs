using System.Linq.Expressions;

namespace SMS.Generic.Common.Helpers
{
    public static class ExpressionCombiner
    {
        public static Expression<Func<T, bool>> AndAlso<T>(
            Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            var parameter = left.Parameters[0];
            var rightBody = new ReplaceParameterVisitor(right.Parameters[0], parameter).Visit(right.Body)!;
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, rightBody), parameter);
        }
    }
}
