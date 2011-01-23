using System;
using Quokka.Collections;
using Quokka.Events;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// Presenter class, where the view type has not been specified. This is a good base class
	/// for presenters that do not have an associated view, which is a reasonably rare use-case.
	/// Most presenters should derive from the <see cref="Presenter{T}"/> class.
	/// </summary>
	public abstract class Presenter : PresenterBase
	{
		public object View
		{
			get { return ViewObject; }
			set { ViewObject = value; }
		}
	}

	/// <summary>
	/// Base class for a presenter. The generic argument specifies the view interface or class.
	/// </summary>
	/// <typeparam name="TView">
	/// The type of view that this presenter interacts with. It is recommended that this should be an
	/// interface to permit mocked unit-testing, but it is possible to specify a concrete class.
	/// </typeparam>
	public abstract class Presenter<TView> : PresenterBase where TView : class
	{
		protected Presenter()
		{
			ViewType = typeof(TView);
		}

		/// <summary>
		/// The view object associated with this presenter. The <see cref="UITask"/> will assign the
		/// view to the presenter immediately after constructing the presenter.
		/// </summary>
		public TView View
		{
			get { return (TView)ViewObject; }
			set { ViewObject = value; }
		}
	}
}