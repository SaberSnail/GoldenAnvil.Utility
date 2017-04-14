using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GoldenAnvil.Utility
{
	public static class ReflectionUtility<T>
	{
		public static string GetMemberName<TValue>(Expression<Func<T, TValue>> selector)
		{
			Expression body = selector;
			if (body is LambdaExpression)
				body = ((LambdaExpression) body).Body;

			switch (body.NodeType)
			{
			case ExpressionType.MemberAccess:
				return ((MemberExpression) body).Member.Name;
			default:
				throw new InvalidOperationException();
			}
		}

		public static Type GetMemberType<TValue>(Expression<Func<T, TValue>> selector)
		{
			Expression body = selector;
			if (body is LambdaExpression)
				body = ((LambdaExpression) body).Body;

			if (body.NodeType != ExpressionType.MemberAccess)
				throw new InvalidOperationException();

			MemberInfo member = ((MemberExpression) body).Member;
			switch (member.MemberType)
			{
			case MemberTypes.Property:
				return ((PropertyInfo) member).PropertyType;
			case MemberTypes.Method:
				return ((MethodInfo) member).ReturnType;
			case MemberTypes.Field:
				return ((FieldInfo) member).FieldType;
			default:
				throw new InvalidOperationException();
			}
		}
	}
}
