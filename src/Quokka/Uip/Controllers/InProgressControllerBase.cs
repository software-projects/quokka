using System;
using System.Collections.Generic;
using System.Text;

namespace Quokka.Uip.Controllers
{
    /// <summary>
    /// Default implementation of <see cref="IInProgressController"/>. Suitable for use as a base
    /// class for controllers which perform lengthy operations.
    /// </summary>
    /// <remarks>
    /// Using this class as a base class is simpler that implementing the <see cref="IInProgressController"/>
    /// from scratch. The base class has to implement <see cref="DoWork"/> and <see cref="FinishWork"/>, as they
    /// are defined as abstract methods in this class.
    /// </remarks>
    public abstract class InProgressControllerBase : IInProgressController
    {
        private string progressSummary;
        private string progressDetail;
        private int progressMinimum;
        private int progressMaximum;
        private int progressValue;
        private bool canCancel;
        private bool cancelRequested;

        public event EventHandler ProgressChanged;

        #region Public properties

        public string ProgressSummary {
            get { return progressSummary; }
            set {
                if (progressSummary != value) {
                    progressSummary = value;
                    OnProgressChanged();
                }
            }
        }

        public string ProgressDetail {
            get { return progressDetail; }
            set {
                if (progressDetail != value) {
                    progressDetail = value;
                    OnProgressChanged();
                }
            }
        }

        public int ProgressMinimum {
            get { return progressMinimum; }
            set {
                if (progressMinimum != value) {
                    progressMinimum = value;
                    if (progressMaximum < progressMinimum) {
                        progressMaximum = progressMinimum;
                    }
                    OnProgressChanged();
                }
            }
        }

        public int ProgressMaximum {
            get { return progressMaximum; }
            set {
                if (progressMaximum != value) {
                    progressMaximum = value;
                    if (progressMinimum > progressMaximum) {
                        progressMinimum = progressMaximum;
                    }
                    OnProgressChanged();
                }
            }
        }

        public int ProgressValue {
            get { return progressValue; }
            set {
                if (progressValue != value) {
                    progressValue = value;
                    OnProgressChanged();
                }
            }
        }

        public bool CanCancel {
            get { return canCancel; }
            set {
                if (canCancel != value) {
                    canCancel = value;
                    OnProgressChanged();
                }
            }
        }

        public bool CancelRequested {
            get { return cancelRequested; }
        }

        #endregion

        #region Public methods

        public abstract void DoWork();

        public abstract void FinishedWork();

        public void Cancel() {
            if (!canCancel) {
                throw new NotSupportedException("Cancel not supported");
            }
            if (!cancelRequested) {
                cancelRequested = true;
                OnProgressChanged();
                OnCancelRequested();
            }
        }

        public void ProgressStep(int increment) {
            if (increment != 0) {
                int newProgressValue = progressValue + increment;
                if (newProgressValue < progressMinimum) {
                    newProgressValue = progressMinimum;
                }
                if (newProgressValue > progressMaximum) {
                    newProgressValue = progressMaximum;
                }
                ProgressValue = newProgressValue;
            }
        }

        #endregion

        #region Protected methods

        protected virtual void OnProgressChanged() {
            if (ProgressChanged != null) {
                ProgressChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnCancelRequested() {
        }

        #endregion
    }
}
