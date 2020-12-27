using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GoldenAnvil.Utility
{
	/// <summary>
	/// Simplifies implementation of INotifyPropertyChanged.
	/// </summary>
	public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged, INotifyPropertyChanging
	{
		/// <summary>
		/// Raised before a property changes.
		/// </summary>
		public event PropertyChangingEventHandler PropertyChanging;

		/// <summary>
		/// Raised when a property changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises the property changing event and returns an object that, when disposed,
		/// will raise the property changed event.
		/// </summary>
		/// <param name="propertyName">Name of the property that will change.</param>
		/// <returns>An object that, when disposed, will raise the property changed event.</returns>
		protected IDisposable ScopedPropertyChange(string propertyName)
		{
			RaisePropertyChanging(propertyName);
			return Scope.Create(() => RaisePropertyChanged(propertyName));
		}

		/// <summary>
		/// Raises property changing events and returns an object that, when disposed,
		/// will raise property changed events.
		/// </summary>
		/// <param name="propertyNames">Names of the properties that will change.</param>
		/// <returns>An object that, when disposed, will raise property changed events.</returns>
		/// <remarks>Null or empty strings are ignored.</remarks>
		protected IDisposable ScopedPropertyChange(params string[] propertyNames)
		{
			RaisePropertyChanging(propertyNames);
			string[] reversedNames = propertyNames.Reverse().ToArray();
			return Scope.Create(() => RaisePropertyChanged(reversedNames));
		}

		/// <summary>
		/// Sets the property field and raises property changed when value changes.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="newValue">The new value of the field.</param>
		/// <param name="field">The field.</param>
		/// <returns><c>true</c>, if the field was updated; otherwise <c>false</c>.</returns>
		protected bool SetPropertyField<T>(string propertyName, T newValue, ref T field)
		{
			return SetPropertyField(propertyName, newValue, ref field, EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Sets the property field and raises property changed when value changes. This methods is intended
		/// for use only in the setter of the property being changed.
		/// </summary>
		/// <param name="newValue">The new value of the field.</param>
		/// <param name="field">The field.</param>
		/// <param name="propertyName">Name of the property. Do not explicitly specify this value.</param>
		/// <returns><c>true</c>, if the field was updated; otherwise <c>false</c>.</returns>
		protected bool SetPropertyField<T>(T newValue, ref T field, [CallerMemberName] string propertyName = "")
		{
			return SetPropertyField(propertyName, newValue, ref field, EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Sets the property field and raises property changed when value changes.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="newValue">The new value of the field.</param>
		/// <param name="field">The field.</param>
		/// <param name="comparer">The equality comparer.</param>
		/// <returns><c>true</c>, if the field was updated; otherwise <c>false</c>.</returns>
		protected bool SetPropertyField<T>(string propertyName, T newValue, ref T field, IEqualityComparer<T> comparer)
		{
			if (comparer.Equals(field, newValue))
				return false;

			using (ScopedPropertyChange(propertyName))
				field = newValue;
			return true;
		}

		/// <summary>
		/// Sets the property field and raises property changed when value changes. This methods is intended
		/// for use only in the setter of the property being changed.
		/// </summary>
		/// <param name="newValue">The new value of the field.</param>
		/// <param name="field">The field.</param>
		/// <param name="comparer">The equality comparer.</param>
		/// <param name="propertyName">Name of the property. Do not explicitly specify this value.</param>
		/// <returns><c>true</c>, if the field was updated; otherwise <c>false</c>.</returns>
		protected bool SetPropertyField<T>(T newValue, ref T field, IEqualityComparer<T> comparer, [CallerMemberName] string propertyName = "")
		{
			return SetPropertyField(propertyName, newValue, ref field, comparer);
		}

		/// <summary>
		/// Sets the property field and raises property changed when value changes.
		/// </summary>
		/// <param name="propertyNames">Names of the properties for which to raise the PropertyChanged event.</param>
		/// <param name="newValue">The new value of the field.</param>
		/// <param name="field">The field.</param>
		/// <returns><c>true</c>, if the field was updated; otherwise <c>false</c>.</returns>
		protected bool SetPropertyField<T>(string[] propertyNames, T newValue, ref T field)
		{
			return SetPropertyField(propertyNames, newValue, ref field, EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Sets the property field and raises property changed when value changes.
		/// </summary>
		/// <param name="propertyNames">Names of the properties for which to raise the PropertyChanged event.</param>
		/// <param name="newValue">The new value of the field.</param>
		/// <param name="field">The field.</param>
		/// <param name="comparer">The equality comparer.</param>
		/// <returns><c>true</c>, if the field was updated; otherwise <c>false</c>.</returns>
		protected bool SetPropertyField<T>(string[] propertyNames, T newValue, ref T field, IEqualityComparer<T> comparer)
		{
			if (comparer.Equals(field, newValue))
				return false;

			using (ScopedPropertyChange(propertyNames))
				field = newValue;
			return true;
		}

		/// <summary>
		/// Disposes the property field, sets it to null, and raises property change notifications.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="field">The field.</param>
		/// <returns><c>true</c>, if the field was disposed and set to null; otherwise <c>false</c>.</returns>
		/// <remarks>This method does nothing if the field is already set to null.</remarks>
		protected bool DisposePropertyField<T>(string propertyName, ref T field)
			where T : class, IDisposable
		{
			if (field == null)
				return false;

			using (ScopedPropertyChange(propertyName))
			{
				field.Dispose();
				field = null;
			}
			return true;
		}

		/// <summary>
		/// Disposes the property field, sets it to null, and raises property change notifications.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="propertyNames">Name of the property.</param>
		/// <param name="field">The field.</param>
		/// <returns><c>true</c>, if the field was disposed and set to null; otherwise <c>false</c>.</returns>
		/// <remarks>This method does nothing if the field is already set to null.</remarks>
		protected bool DisposePropertyField<T>(string[] propertyNames, ref T field)
			where T : class, IDisposable
		{
			if (field == null)
				return false;

			using (ScopedPropertyChange(propertyNames))
			{
				field.Dispose();
				field = null;
			}
			return true;
		}

		/// <summary>
		/// Called after a property has changed.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
		}

		/// <summary>
		/// Called before a property has changed.
		/// </summary>
		/// <param name="propertyName"></param>
		protected virtual void OnPropertyChanging(string propertyName)
		{
		}

		/// <summary>
		/// Call this method to verify in DEBUG builds that this class can be accessed in the current context.
		/// </summary>
		[Conditional("DEBUG")]
		protected virtual void VerifyAccess()
		{
		}

		/// <summary>
		/// Call this method to verify in DEBUG builds that this class can be accessed in the current context.
		/// </summary>
		/// <param name="value">The value to return.</param>
		/// <returns>The value passed as an argument.</returns>
		protected T VerifyAccess<T>(T value)
		{
			VerifyAccess();
			return value;
		}

		private void RaisePropertyChanging(string propertyName)
		{
			VerifyAccess();
			OnPropertyChanging(propertyName);
			PropertyChanging.Raise(this, propertyName);
		}

		private void RaisePropertyChanging(params string[] propertyNames)
		{
			VerifyAccess();
			foreach (string propertyName in propertyNames)
			{
				if (!string.IsNullOrEmpty(propertyName))
				{
					OnPropertyChanging(propertyName);
					PropertyChanging.Raise(this, propertyName);
				}
			}
		}

		private void RaisePropertyChanged(string propertyName)
		{
			VerifyAccess();
			OnPropertyChanged(propertyName);
			PropertyChanged.Raise(this, propertyName);
		}

		private void RaisePropertyChanged(params string[] propertyNames)
		{
			VerifyAccess();
			foreach (string propertyName in propertyNames)
			{
				if (!string.IsNullOrEmpty(propertyName))
				{
					OnPropertyChanged(propertyName);
					PropertyChanged.Raise(this, propertyName);
				}
			}
		}
	}
}
