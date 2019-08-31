using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Schedule.Extensions
{
    /// <summary>
    ///     Expression extensions.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary> Builds the path. </summary>
        /// <returns> The path. </returns>
        /// <param name="path"> Path. </param>
        /// <param name="context"> Expression context. </param>
        /// <param name="objectPropertyType"> Expression context type. </param>
        public static (PropertyInfo propertyType, Expression propertyExpression, bool isValid) BuildPath(string path, Expression context, Type objectPropertyType)
        {
            var parts = path.Split('.');

            var res = ((PropertyInfo)null, (Expression)null, false);

            foreach (var part in parts)
            {
                var prop = objectPropertyType
                    .GetProperties()
                    .FirstOrDefault(x =>
                        string.Equals(x.Name, part, StringComparison.InvariantCultureIgnoreCase));

                if (prop == null)
                {
                    return (null, null, false);
                }

                context = Expression.Property(context, prop.Name);
                res = (prop, context, true);
                objectPropertyType = prop.PropertyType;
            }

            return res;
        }
    }
}