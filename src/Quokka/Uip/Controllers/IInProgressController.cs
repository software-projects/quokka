using System;
using System.Collections.Generic;
using System.Text;

namespace Quokka.Uip
{
    /// <summary>
    /// Controller interface used for operations which take a long
    /// time and need to provide feedback to the user.
    /// </summary>
    public interface IInProgressController
    {
        /// <summary>
        /// This event fires whenever any of the properties in this interface change.
        /// </summary>
        /// <remarks>
        /// Whenever this event fires, the user interface should refresh as one or more
        /// properties have changed.
        /// </remarks>
        event EventHandler ProgressChanged;

        /// <summary>
        /// Short summary of the operation being performed (eg 'Moving Files')
        /// </summary>
        string ProgressSummary { get; }

        /// <summary>
        /// More information on what is being done (eg name of file currently being moved)
        /// </summary>
        string ProgressDetail { get; }

        /// <summary>
        /// Lower bound for progress, almost always set to zero
        /// </summary>
        int ProgressMinimum { get; }

        /// <summary>
        /// Upper bound for progress.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Set this to any convenient value. For example if you are moving lots of files,
        /// set this value to the number of files that have to be moved.
        /// </para>
        /// <para>
        /// If <c>ProgressMinimum == ProgressMaximum</c>, this indicates to the user interface
        /// that there is feedback available on progress. The user interface should indicate
        /// that the operation is in progress, but without showing percent complete.
        /// </para>
        /// </remarks>
        int ProgressMaximum { get; }

        /// <summary>
        /// A value in the range [<see cref="ProgressMinimum"/>, <see cref="ProgressMaximum"/>] that
        /// indicates how much progress has been made.
        /// </summary>
        int ProgressValue { get; }

        /// <summary>
        /// True if the user can cancel the operation in progress.
        /// </summary>
        bool CanCancel { get; }

        /// <summary>
        /// Indicates that the user has requested that the operation be cancelled.
        /// </summary>
        bool CancelRequested { get; }

        /// <summary>
        /// Called from the view on a separate thread to perform the work.
        /// </summary>
        void DoWork();

        /// <summary>
        /// Called from the view on the main thread when the work is complete.
        /// </summary>
        /// <remarks>
        /// This is the method where the controller usually navigates to the next view.
        /// </remarks>
        void FinishedWork();

        /// <summary>
        /// Called from the view on any thread to indicate that the user has requested
        /// that the operation cancel.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown when <see cref="CanCancel"/> returns false. In multi-threaded situations
        /// where some parts of the operation support cancel and others don't, there is the
        /// possibility of a race condition. The view should be prepared to handle this exception
        /// in these circumstances.
        /// </exception>
        void Cancel();
    }
}
