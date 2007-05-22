#region Copyright notice
//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006 John Jeffery. All rights reserved.
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

namespace Quokka.Uip
{
	using System;

	public class UipViewEventArgs : EventArgs {
        private object view;

        public UipViewEventArgs(object view) {
            if (view == null)
                throw new ArgumentNullException("view");
            this.view = view;
        }

        public object View {
            get { return view; }
        }
    }

    public interface IUipViewManager
    {
        event EventHandler<UipViewEventArgs> ViewClosed;

        void BeginTask(UipTask task);
        void EndTask(UipTask task);
        void BeginTransition();
        void EndTransition();
        void AddView(object view, object controller);
        void RemoveView(object view);
        void ShowView(object view);
        void HideView(object view);
    	void ShowModalView(object view, object controller);
    }

    /// <summary>
    /// I don't know if this is a good idea.
    /// </summary>
    public interface IUipInteraction {
        /// <summary>
        ///     Display a modal information message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="caption">Caption, or message summary</param>
        void InfoMessage(string message, string caption);

        /// <summary>
        ///     Display a modal error message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="caption">Caption, or message summary</param>
        void ErrorMessage(string message, string caption);

        /// <summary>
        ///     Display a modal question message, whose answer is 'yes' or 'no'
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="defaultAnswer"></param>
        /// <returns>Returns <c>true</c> for 'yes', <c>false</c> for 'no'.</returns>
        bool AskYesNoQuestion(string message, string caption, bool defaultAnswer);
    }
}
