using System;
using System.Linq.Expressions;
using Parquet.Plus.Exceptions;

namespace Parquet.Plus.Extensions
{
    /// <summary>
    /// Parquet extensions
    /// </summary>
    public static class ParquetExtensions
    {
        // Setter
        /// <summary>
        /// Creates setter method from column to property 
        /// </summary>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <typeparam name="TProperty">Type of property, also column</typeparam>
        /// <param name="propertySelector">Target property selector</param>
        /// <returns>Setter method from column to property</returns>
        public static Action<TModel, TProperty> BuildSetter<TModel, TProperty>(this Expression<Func<TModel, TProperty>> propertySelector)
        {
            var parameterProperty = Expression.Parameter(typeof(TProperty));

            if (!(propertySelector.Body is MemberExpression member))
            {
                throw new PropertySelectorException();
            }

            var memberExpression = (ParameterExpression)member.Expression;
            var assignExp = Expression.Assign(member, parameterProperty);
            var lambda = Expression.Lambda<Action<TModel, TProperty>>(assignExp, memberExpression, parameterProperty);
            return lambda.Compile();
        }

        /// <summary>
        /// Creates setter method from column to property with different types
        /// </summary>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <typeparam name="TColumn">Type of column</typeparam>
        /// <param name="propertySelector">Target property selector</param>
        /// <param name="mapperToPropertyExp">Mapper expression</param>
        /// <returns>Setter method from column to property</returns>
        public static Action<TModel, TColumn> BuildSetter<TModel, TProperty, TColumn>(this Expression<Func<TModel, TProperty>> propertySelector, Expression<Func<TColumn, TProperty>> mapperToPropertyExp)
        {
            var assignExp = Expression.Assign(propertySelector.Body, mapperToPropertyExp.Body);
            var lambda = Expression.Lambda<Action<TModel, TColumn>>(assignExp, propertySelector.Parameters[0], mapperToPropertyExp.Parameters[0]);
            return lambda.Compile();
        }

        // Getter
        /// <summary>
        /// Creates getter method from property to column
        /// </summary>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <typeparam name="TProperty">Type of property, also column</typeparam>
        /// <param name="propertySelector">Source property selector</param>
        /// <returns>Getter method from property to column</returns>
        public static Func<TModel, TProperty> BuildGetter<TModel, TProperty>(this Expression<Func<TModel, TProperty>> propertySelector)
        {
            if (!(propertySelector.Body is MemberExpression member))
            {
                throw new PropertySelectorException();
            }

            var memberExpression = (ParameterExpression)member.Expression;
            var lambda = Expression.Lambda<Func<TModel, TProperty>>(member, memberExpression);
            return lambda.Compile();
        }

        /// <summary>
        /// Creates geter method from property to column with different types
        /// </summary>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <typeparam name="TColumn">Type of column</typeparam>
        /// <param name="propertySelector">Source property selector</param>
        /// <param name="mapperToColumnExp">Mapper extension</param>
        /// <returns>Getter method from property to column</returns>
        public static Func<TModel, TColumn> BuildGetter<TModel, TProperty, TColumn>(this Expression<Func<TModel, TProperty>> propertySelector, Expression<Func<TProperty, TColumn>> mapperToColumnExp)
        {
            var invoke = Expression.Invoke(mapperToColumnExp, propertySelector.Body);
            var lambda = Expression.Lambda<Func<TModel, TColumn>>(invoke, propertySelector.Parameters[0]);
            return lambda.Compile();
        }
    }
}