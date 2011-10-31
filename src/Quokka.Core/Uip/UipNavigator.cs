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
using Quokka.Diagnostics;

namespace Quokka.Uip
{
	/// <summary>
	/// Navigator class used by the <see cref="UipTask"/>.
	/// </summary>
	//[Obsolete("This will be removed from Quokka in a future release")]
	internal class UipNavigator : IUipNavigator
	{
		private readonly UipTask _task;

		public UipNavigator(UipTask task)
		{
			_task = task;
		}

		public void Navigate(string navigateValue)
		{
			Verify.ArgumentNotNull(navigateValue, "navigateValue");
			_task.Navigate(navigateValue);
		}

		public bool CanNavigate(string navigateValue)
		{
			Verify.ArgumentNotNull(navigateValue, "navigateValue");
			UipNode nextNode;
			return _task.CurrentNode.GetNextNode(navigateValue, out nextNode);
		}
	}
}