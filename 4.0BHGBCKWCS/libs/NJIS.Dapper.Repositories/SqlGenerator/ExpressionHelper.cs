﻿// ************************************************************************************
//  解决方案：NJIS.FPZWS.LineControl.Drilling
//  项目名称：NJIS.Dapper.Repositories
//  文 件 名：ExpressionHelper.cs
//  创建时间：2019-08-30 9:47
//  作    者：
//  说    明：
//  修改时间：2018-11-21 11:51
//  修 改 人：
// Copyright © 2017 广州宁基智能系统有限公司. 版权所有
// *************************************************************************************

#region

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NJIS.Dapper.Repositories.Extensions;

#endregion

namespace NJIS.Dapper.Repositories.SqlGenerator
{
    internal static class ExpressionHelper
    {
        public static string GetPropertyName<TSource, TField>(Expression<Func<TSource, TField>> field)
        {
            if (Equals(field, null))
                throw new NullReferenceException("Field is required");

            MemberExpression expr;

            var body = field.Body as MemberExpression;
            if (body != null)
            {
                expr = body;
            }
            else
            {
                var expression = field.Body as UnaryExpression;
                if (expression != null)
                    expr = (MemberExpression) expression.Operand;
                else
                    throw new ArgumentException("Expression" + field + " is not supported.", nameof(field));
            }

            return expr.Member.Name;
        }

        public static string GetSqlLikeValue(string methodName, object value)
        {
            if (value == null)
                value = string.Empty;

            switch (methodName)
            {
                case "StartsWith":
                    return string.Format("{0}%", value);

                case "EndsWith":
                    return string.Format("%{0}", value);

                case "StringContains":
                    return string.Format("%{0}%", value);

                default:
                    throw new NotImplementedException();
            }
        }

        public static object GetValuesFromStringMethod(MethodCallExpression callExpr)
        {
            var expr = callExpr.Method.IsStatic ? callExpr.Arguments[1] : callExpr.Arguments[0];

            return GetValue(expr);
        }

        public static object GetValue(Expression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        public static string GetSqlOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                case ExpressionType.Not:
                case ExpressionType.MemberAccess:
                    return "=";

                case ExpressionType.NotEqual:
                    return "!=";

                case ExpressionType.LessThan:
                    return "<";

                case ExpressionType.LessThanOrEqual:
                    return "<=";

                case ExpressionType.GreaterThan:
                    return ">";

                case ExpressionType.GreaterThanOrEqual:
                    return ">=";

                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    return "AND";

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return "OR";

                case ExpressionType.Default:
                    return string.Empty;

                default:
                    throw new NotImplementedException();
            }
        }


        public static string GetMethodCallSqlOperator(string methodName, bool isNotUnary = false)
        {
            switch (methodName)
            {
                case "StartsWith":
                case "EndsWith":
                case "StringContains":
                    return isNotUnary ? "NOT LIKE" : "LIKE";

                case "Contains":
                    return isNotUnary ? "NOT IN" : "IN";

                case "Any":
                case "All":
                    return methodName.ToUpperInvariant();

                default:
                    throw new NotSupportedException(methodName + " isn't supported");
            }
        }


        public static BinaryExpression GetBinaryExpression(Expression expression)
        {
            var binaryExpression = expression as BinaryExpression;
            var body = binaryExpression ??
                       Expression.MakeBinary(ExpressionType.Equal, expression,
                           expression.NodeType == ExpressionType.Not
                               ? Expression.Constant(false)
                               : Expression.Constant(true));
            return body;
        }

        public static Func<PropertyInfo, bool> GetPrimitivePropertiesPredicate()
        {
            return
                p =>
                    p.CanWrite &&
                    (p.PropertyType.IsValueType() || p.PropertyType == typeof(string) ||
                     p.PropertyType == typeof(byte[]));
        }

        public static object GetValuesFromCollection(MethodCallExpression callExpr)
        {
            var expr = callExpr.Object as MemberExpression;

            if (!(expr?.Expression is ConstantExpression))
                throw new NotImplementedException($"{callExpr.Method.Name} is not implemented");

            var constExpr = (ConstantExpression) expr.Expression;

            var constExprType = constExpr.Value.GetType();
            return constExprType.GetField(expr.Member.Name).GetValue(constExpr.Value);
        }


        public static MemberExpression GetMemberExpression(Expression expression)
        {
            var expr = expression as MethodCallExpression;
            if (expr != null)
                return (MemberExpression) expr.Arguments[0];

            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
                return memberExpression;

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
                return (MemberExpression) unaryExpression.Operand;

            var binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
            {
                var binaryExpr = binaryExpression;

                var left = binaryExpr.Left as UnaryExpression;
                if (left != null)
                    return (MemberExpression) left.Operand;

                //should we take care if right operation is memberaccess, not left ?
                try
                {
                    return (MemberExpression) binaryExpr.Left;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            var expression1 = expression as LambdaExpression;
            if (expression1 != null)
            {
                var lambdaExpression = expression1;

                var body = lambdaExpression.Body as MemberExpression;
                if (body != null)
                    return body;

                var expressionBody = lambdaExpression.Body as UnaryExpression;
                if (expressionBody != null)
                    return (MemberExpression) expressionBody.Operand;
            }

            return null;
        }

        /// <summary>
        ///     Gets the name of the property.
        /// </summary>
        /// <param name="expr">The Expression.</param>
        /// <param name="nested">Out. Is nested property.</param>
        /// <returns>The property name for the property expression.</returns>
        public static string GetPropertyNamePath(Expression expr, out bool nested)
        {
            var path = new StringBuilder();
            var memberExpression = GetMemberExpression(expr);
            nested = false;
            if (memberExpression == null || memberExpression.Member == null) return "";
            var count = 0;
            do
            {
                count++;
                if (path.Length > 0)
                    path.Insert(0, "");
                path.Insert(0, memberExpression.Member.Name);
                memberExpression = GetMemberExpression(memberExpression.Expression);
            } while (memberExpression != null);

            if (count > 2)
                throw new ArgumentException("Only one degree of nesting is supported");

            nested = count == 2;

            return path.ToString();
        }
    }
}