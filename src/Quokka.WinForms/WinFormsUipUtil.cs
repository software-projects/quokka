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

using System.Windows.Forms;

namespace Quokka.WinForms
{
	internal static class WinFormsUipUtil
	{
		/// <summary>
		/// Attempt to set the controller for a WinForms control, and all its contained controls
		/// </summary>
		/// <param name="control">The control (may have any level of contained controls)</param>
		/// <param name="controller">The controller to assign.</param>
		internal static void SetController(Control control, object controller)
		{
			if (control == null)
				return;

			// Do not set the controller for the control itself, as this has already been done
			// by the task processing itself.
			//UipUtil.SetController(control, controller, false);
			foreach (Control containedControl in control.Controls)
			{
				SetController(containedControl, controller);
			}
		}
	}
}