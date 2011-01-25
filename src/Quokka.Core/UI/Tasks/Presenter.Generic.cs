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

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// 	Base class for a presenter. The generic argument specifies the view interface or class.
	/// </summary>
	/// <typeparam name = "TView">
	/// 	The type of view that this presenter interacts with. It is recommended that this should be an
	/// 	interface to permit mocked unit-testing, but it is possible to specify a concrete class.
	/// </typeparam>
	public abstract class Presenter<TView> : PresenterBase where TView : class
	{
		protected Presenter()
		{
			ViewType = typeof(TView);
		}

		/// <summary>
		/// 	The view object associated with this presenter. The <see cref = "UITask" /> will assign the
		/// 	view to the presenter immediately after constructing the presenter.
		/// </summary>
		public TView View
		{
			get { return (TView)ViewObject; }
			set { ViewObject = value; }
		}
	}
}