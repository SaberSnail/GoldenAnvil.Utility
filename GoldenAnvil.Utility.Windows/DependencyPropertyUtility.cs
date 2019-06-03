using System;
using System.Linq.Expressions;
using System.Windows;

namespace GoldenAnvil.Utility.Windows
{
	public static class DependencyPropertyUtility<T>
	{
		public static DependencyProperty Register<TValue>(Expression<Func<T, TValue>> selector, TValue defaultValue = default(TValue))
		{
			return DependencyProperty.Register(ReflectionUtility<T>.GetMemberName(selector), typeof(TValue), typeof(T), new PropertyMetadata(defaultValue));
		}

		public static DependencyProperty Register<TValue>(Expression<Func<T, TValue>> selector, PropertyChangedCallback changedCallback, TValue defaultValue = default(TValue))
		{
			return DependencyProperty.Register(ReflectionUtility<T>.GetMemberName(selector), typeof(TValue), typeof(T), new PropertyMetadata(defaultValue, changedCallback));
		}

		public static DependencyProperty Register<TValue>(Expression<Func<T, TValue>> selector, PropertyChangedCallback changedCallback, FrameworkPropertyMetadataOptions flags, TValue defaultValue = default(TValue))
		{
			return DependencyProperty.Register(ReflectionUtility<T>.GetMemberName(selector), typeof(TValue), typeof(T), new FrameworkPropertyMetadata(defaultValue, flags, changedCallback));
		}

		public static DependencyPropertyKey RegisterReadOnly<TValue>(Expression<Func<T, TValue>> selector)
		{
			return DependencyProperty.RegisterReadOnly(ReflectionUtility<T>.GetMemberName(selector), typeof(TValue), typeof(T), null);
		}

		public static DependencyPropertyKey RegisterReadOnly<TValue>(Expression<Func<T, TValue>> selector, PropertyChangedCallback changedCallback)
		{
			return DependencyProperty.RegisterReadOnly(ReflectionUtility<T>.GetMemberName(selector), typeof(TValue), typeof(T), new PropertyMetadata(changedCallback));
		}

		public static DependencyProperty RegisterAttached<TValue>(string name)
		{
			return DependencyProperty.RegisterAttached(name, typeof(TValue), typeof(T));
		}

		public static DependencyProperty RegisterAttached<TValue>(string name, PropertyChangedCallback changedCallback)
		{
			return DependencyProperty.RegisterAttached(name, typeof(TValue), typeof(T), new PropertyMetadata(changedCallback));
		}

		public static DependencyProperty RegisterAttached<TValue>(string name, PropertyChangedCallback changedCallback, FrameworkPropertyMetadataOptions flags)
		{
			return DependencyProperty.RegisterAttached(name, typeof(TValue), typeof(T), new FrameworkPropertyMetadata(default(TValue) , flags, changedCallback));
		}
	}
}
