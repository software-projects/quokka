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
using System.Collections.Generic;
using System.Windows.Forms;
using Quokka.UI.Tasks;
using Quokka.Uip;

namespace Quokka.WinForms
{
	/// <summary>
	/// 	Handles views inside a Windows Forms <see cref = "Control" />
	/// </summary>
	[Obsolete("Use class ViewDeck instead. This class will be removed in a future version of Quokka")]
	public class ViewManager : ViewDeck, IUipViewManager
	{
		private readonly List<Form> _modalForms = new List<Form>();

		public new event EventHandler<UipViewEventArgs> ViewClosed;


		public ViewManager(Control control) : base(control)
		{
		}

		public override void Clear()
		{
			base.Clear();
			_modalForms.Clear();
		}

		#region IUipViewManager Members

		public void BeginTask(UipTask task)
		{
			BeginTask((object) task);
		}

		public void EndTask(UipTask task)
		{
			EndTask((object) task);
		}

		public void AddView(object view, object controller)
		{
			var control = view as Control;

			if (control != null)
			{
				// This gives a chance for all of the controls within the view control
				// to get a look at the controller. To get the controller, the contained
				// control must implement a method called "SetController", which accepts
				// a compatible type. (Could be an embedded "IController" interface.
				if (controller != null)
				{
					WinFormsUipUtil.SetController(control, controller);
				}
			}

			AddView(view);
		}

		public override void RemoveView(object view)
		{
			Form form = view as Form;
			if (form != null && _modalForms.Contains(form))
			{
				// this is a modal form -- close it
				form.Close();
			}
			else
			{
				base.RemoveView(view);
			}
		}

		public void ShowModalView(object view)
		{
			ShowModalView(view, null);
		}

		public void ShowModalView(object view, object controller)
		{
			Form form = view as Form;
			if (form == null)
			{
				throw new ArgumentException("Modal views must inherit from System.Windows.Forms.Form");
			}

			if (controller != null)
			{
				WinFormsUipUtil.SetController(form, controller);
			}
			_modalForms.Add(form);
			form.Closed += ViewClosedHandler;
			form.ShowDialog(Control.TopLevelControl);
		}

		private delegate UipAnswer AskQuestionDelegate(UipQuestion question);

		public UipAnswer AskQuestion(UipQuestion question)
		{
			// This might be called from a different thread
			if (Control.InvokeRequired)
			{
				return (UipAnswer) Control.Invoke(new AskQuestionDelegate(AskQuestion), question);
			}

			MessageBoxForm questionForm = new MessageBoxForm {Question = question};
			questionForm.ShowDialog(Control.TopLevelControl);
			if (question.SelectedAnswer != null && question.SelectedAnswer.Callback != null)
			{
				question.SelectedAnswer.Callback();
			}
			return question.SelectedAnswer;
		}

		#endregion

		protected virtual void OnViewClosed(UipViewEventArgs e)
		{
			if (ViewClosed != null)
			{
				ViewClosed(this, e);
			}
		}

		protected override void OnViewClosed(ViewClosedEventArgs e)
		{
			Form form = e.View as Form;
			if (form != null)
			{
				_modalForms.Remove(form);
			}

			base.OnViewClosed(e);

			if (ViewClosed != null)
			{
				ViewClosed(this, new UipViewEventArgs(e.View));
			}
		}
	}
}