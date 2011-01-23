using System;
using Quokka.Collections;
using Quokka.Events;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// 	Base class for the <see cref = "Presenter" /> and <see cref = "Presenter{T}" /> classes, providing
	/// 	common functionality.
	/// </summary>
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

		internal void PresenterInitialized()
		{
			OnViewCreated();
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
		/// 	This method is called after the presenter has been constructed, and all of the properties have been assigned.
		/// </summary>
		/// <remarks>
		/// 	Do not perform initialization on the view in the presenter's constructor, as it will not have been assigned yet.
		/// 	Perform initialization of the view in this method instead.
		/// </remarks>
		protected virtual void OnViewCreated()
		{
		}
	}
}