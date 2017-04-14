using System;
using System.Linq.Expressions;
using System.Windows;

namespace GoldenAnvil.Utility.Views
{
	public static class DependencyPropertyUtility<T>
	{
		public static DependencyProperty Register<TValue>(Expression<Func<T, TValue>> selector)
		{
			return DependencyProperty.Register(ReflectionUtility<T>.GetMemberName(selector), ReflectionUtility<T>.GetMemberType(selector), typeof(T));
		}

		public static DependencyProperty Register<TValue>(Expression<Func<T, TValue>> selector, PropertyChangedCallback changedCallback)
		{
			return DependencyProperty.Register(ReflectionUtility<T>.GetMemberName(selector), ReflectionUtility<T>.GetMemberType(selector), typeof(T), new PropertyMetadata(changedCallback));
		}

		public static DependencyProperty Register<TValue>(Expression<Func<T, TValue>> selector, PropertyChangedCallback changedCallback, FrameworkPropertyMetadataOptions flags)
		{
			return DependencyProperty.Register(ReflectionUtility<T>.GetMemberName(selector), ReflectionUtility<T>.GetMemberType(selector), typeof(T), new FrameworkPropertyMetadata(default(T), flags, changedCallback));
		}

		public static DependencyPropertyKey RegisterReadOnly<TValue>(Expression<Func<T, TValue>> selector)
		{
			return DependencyProperty.RegisterReadOnly(ReflectionUtility<T>.GetMemberName(selector), ReflectionUtility<T>.GetMemberType(selector), typeof(T), null);
		}

		public static DependencyPropertyKey RegisterReadOnly<TValue>(Expression<Func<T, TValue>> selector, PropertyChangedCallback changedCallback)
		{
			return DependencyProperty.RegisterReadOnly(ReflectionUtility<T>.GetMemberName(selector), ReflectionUtility<T>.GetMemberType(selector), typeof(T), new PropertyMetadata(changedCallback));
		}
	}
}
