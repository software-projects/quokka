#region Copyright notice
//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using Quokka.Collections;
using Quokka.Events;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// 	Base class for the <see cref = "Presenter" /> and <see cref = "Presenter{T}" /> classes, providing
	/// 	common functionality.
	/// </summary>
	/// <remarks>
	///		Do not use this class as a base class for your presenters. Use <see cref="Presenter{T}"/> instead.
	///		In some cases you might use <see cref="Presenter"/>, but this will not be often.
	/// </remarks>
	public abstract class PresenterBase : IDisposable
	{
		public Type ViewType { get; protected set; }

		///<summary>
		///	A collection of objects that implement the <see cref = "IDisposable" /> interface.
		///	Derived classes can add objects to this collection with the knowledge that they
		///	will be disposed when the presenter is disposed.
		///</summary>
		///<example>
		///	This collection is very useful for event subscriptions. In the following example
		///	the presenter subscribes to the event <c>MyEvent</c>, and adds the 
		///	<see cref = "IEventSubscription" /> object to the <see cref = "Disposables" /> collection.
		///	Using this technique, the event subscription will always be disposed of when the presenter
		///	is disposed.
		///	<code>
		///		EventBroker.GetEvent&lt;MyEvent&gt;()
		///		.Subscribe(OnMyEvent)
		///		.AddTo(Disposables);		
		///	</code>
		///</example>
		protected readonly DisposableCollection Disposables = new DisposableCollection();

		~PresenterBase()
		{
			Dispose(false);
		}

		/// <summary>
		/// 	Implements the <see cref = "IDisposable" /> interface.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			Disposables.Dispose();
			GC.SuppressFinalize(this);
		}

		private bool _hasInitializePresenterBeenCalled;

		internal void PerformPresenterInitialization()
		{
			if (!_hasInitializePresenterBeenCalled)
			{
				_hasInitializePresenterBeenCalled = true;
				InitializePresenter();
			}
		}

		internal object ViewObject { get; set; }

		/// <summary>
		/// 	Override this method to dispose of objects in the presenter, but look at the <see cref = "Disposables" />
		/// 	collection first, to see if this will meet your needs.
		/// </summary>
		/// <param name = "disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// 	Override this method to perform initialization of the presenter. Itis called by the framework after 
		///		the presenter has been constructed, all of the properties have been assigned, and the view has been 
		///		created and assigned to the presenter.
		/// </summary>
		/// <remarks>
		/// 	Do not perform any initialization that concerns the view in the presenter's constructor, as it will 
		///		not have been assigned yet. Perform initialization of the view in this method instead.
		/// </remarks>
		public virtual void InitializePresenter()
		{
		}
	}
}