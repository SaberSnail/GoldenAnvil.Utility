﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;

namespace GoldenAnvil.Utility.Windows
{
	public abstract class NotifyPropertyChangedDispatcherBase : NotifyPropertyChangedBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NotifyPropertyChangedDispatcherBase"/> class.
		/// </summary>
		protected NotifyPropertyChangedDispatcherBase()
		{
			Dispatcher = Dispatcher.CurrentDispatcher;
		}

		/// <summary>
		/// The current Dispatcher at the time this object was created.
		/// </summary>
		protected Dispatcher Dispatcher { get; }

		/// <summary>
		/// Call this method to throw in DEBUG builds if called from a different dispatcher than the one used to create this object.
		/// </summary>
		protected override void VerifyAccess()
		{
			if (!Dispatcher.CheckAccess())
				throw new InvalidOperationException("This code must be called on the same dispatcher as the object was created.");
		}
	}
}
