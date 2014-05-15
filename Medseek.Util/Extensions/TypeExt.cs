namespace Medseek.Util.Extensions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for working with types.
    /// </summary>
    public static class TypeExt
    {
        /// <summary>
        /// Gets a <see cref="MethodInfo"/> describing the method associated 
        /// with the expression.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object defining the desired method.
        /// </typeparam>
        /// <param name="obj">
        /// The object on which the method is defined.
        /// </param>
        /// <param name="expression">
        /// An expression identifying the desired method.
        /// </param>
        /// <returns>
        /// An object describing the method information.
        /// </returns>
        public static MethodInfo GetMethod<T>(this T obj, Expression<Action<T>> expression)
        {
            return GetMethodInternal(expression);
        }

        /// <summary>
        /// Gets a <see cref="MethodInfo"/> describing the method associated 
        /// with the expression.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object defining the desired method.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of object returned by the desired method.
        /// </typeparam>
        /// <param name="obj">
        /// The object on which the method is defined.
        /// </param>
        /// <param name="expression">
        /// An expression identifying the desired method.
        /// </param>
        /// <returns>
        /// An object describing the method information.
        /// </returns>
        public static MethodInfo GetMethod<T, TResult>(this T obj, Expression<Func<T, TResult>> expression)
        {
            return GetMethodInternal(expression);
        }

        /// <summary>
        /// Gets a <see cref="MethodInfo"/> describing the method associated 
        /// with the expression.
        /// </summary>
        /// <typeparam name="T">
        /// The type defining the desired method.
        /// </typeparam>
        /// <param name="expression">
        /// An expression identifying the desired method.
        /// </param>
        /// <returns>
        /// An object describing the method information.
        /// </returns>
        public static MethodInfo GetMethod<T>(Expression<Action<T>> expression)
        {
            return GetMethodInternal(expression);
        }

        /// <summary>
        /// Gets a <see cref="MethodInfo"/> describing the method associated 
        /// with the expression.
        /// </summary>
        /// <typeparam name="T">
        /// The type defining the desired method.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of object returned by the desired method.
        /// </typeparam>
        /// <param name="expression">
        /// An expression identifying the desired method.
        /// </param>
        /// <returns>
        /// An object describing the method information.
        /// </returns>
        public static MethodInfo GetMethod<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            return GetMethodInternal(expression);
        }

        private static MethodInfo GetMethodInternal(LambdaExpression expression)
        {
            var methodCallExpression = (MethodCallExpression)expression.Body;
            var result = methodCallExpression.Method;
            return result;
        }
    }
}
