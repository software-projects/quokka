using System;
using System.Collections.Generic;
using System.Text;

namespace Quokka.Uip
{
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
