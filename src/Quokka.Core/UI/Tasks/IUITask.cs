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
using Quokka.ServiceLocation;
using Quokka.Uip;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// 	Represents a User Interface Task.
	/// </summary>
	/// <remarks>
	/// 	This interface contains common properties between the obsolete <see cref = "UipTask" /> and
	/// 	the newer <see cref = "UITask" />. It helps with providing some backwards compatibility.
	/// </remarks>
	public interface IUITask
	{
		event EventHandler TaskComplete;
		// TODO: could include TaskStarted event here for symmetry, but it is not needed at the moment

		bool IsRunning { get; }
		IServiceContainer ServiceContainer { get; }

		void Start(IViewDeck viewDeck);
		void EndTask();
	}
}