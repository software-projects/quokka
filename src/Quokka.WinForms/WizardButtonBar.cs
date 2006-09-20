using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Quokka.WinForms
{
    public partial class WizardButtonBar : UserControl
    {
        private const string NextText = "Next";
        private const string FinishText = "Finish";

        private IController controller;

        // Because we might get passed a controller that throws
        // NotSupportedExceptions when we call any of the properties
        // or methods, keep a record of what properties throw exceptions
        // so that we do not keep calling them. Throwing an exception
        // expensive, and if the RefreshButtons method gets called frequently
        // then it would be a problem if the controller kept throwing
        // exceptions.
        private bool canCancelNotSupported;
        private bool canMoveNextNotSupported;
        private bool canMoveBackNotSupported;
        private bool showFinishNotSupported;

        /// <summary>
        /// Methods and properties expected in the controller.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The WizardButtonBar treats every one of these methods and
        /// properties as optional.
        /// </para>
        /// <para>
        /// The only requirement is that if <c>CanCancel</c> returns <c>true</c>,
        /// then the method <c>Cancel</c> should not throw an exception. Similarly
        /// for <c>CanMoveNext</c> and <c>CanMoveBack</c>.
        /// </para>
        /// </remarks>
        public interface IController
        {
            bool CanCancel { get; }
            bool CanMoveNext { get; }
            bool CanMoveBack { get; }
            bool ShowFinish { get; }
            void MoveNext();
            void MoveBack();
            void Cancel();
        }

        public event CancelEventHandler NextClicked;
        public event CancelEventHandler BackClicked;
        public event CancelEventHandler CancelClicked;

        public WizardButtonBar() {
            InitializeComponent();
        }

        public BorderStyle TopBorderStyle {
            get { return topBorder.BorderStyle; }
            set { topBorder.BorderStyle = value; }
        }

        public void SetController(IController controller) {
            this.controller = controller;
            canCancelNotSupported = false;
            canMoveNextNotSupported = false;
            canMoveBackNotSupported = false;
            showFinishNotSupported = false;
        }

        /// <summary>
        /// Call this function when the controller has changed state in order
        /// to redisplay the wizard bar buttons.
        /// </summary>
        public void RefreshButtons() {
            bool enableNextButton = false;
            bool enableBackButton = false;
            bool enableCancelButton = false;
            string nextButtonText = NextText;

            if (controller != null) {
                // Be wary of NotSupportedExceptions, in case the controller
                // does not support even the basic properties expected in
                // the interface.
                if (!canCancelNotSupported) {
                    try {
                        enableCancelButton = controller.CanCancel;
                    }
                    catch (NotSupportedException) {
                        canCancelNotSupported = true;
                    }
                }

                if (!canMoveBackNotSupported) {
                    try {
                        enableBackButton = controller.CanMoveBack;
                    }
                    catch (NotSupportedException) {
                        canMoveBackNotSupported = true;
                    }
                }

                if (!canMoveNextNotSupported) {

                    try {
                        enableNextButton = controller.CanMoveNext;
                    }
                    catch (NotSupportedException) {
                        canMoveNextNotSupported = true;
                    }
                }

                if (!showFinishNotSupported) {
                    try {
                        if (controller.ShowFinish) {
                            nextButtonText = FinishText;
                        }
                    }
                    catch (NotSupportedException) {
                        showFinishNotSupported = true;
                    }
                }
            }

            // Check before assigning to avoid repaint flicker.
            // It is important for the Text property, but does not
            // make much difference for Enabled. Best to play safe
            // and check for them all.
            if (nextButton.Text != nextButtonText) {
                nextButton.Text = nextButtonText;
            }
            if (cancelButton.Enabled != enableCancelButton) {
                cancelButton.Enabled = enableCancelButton;
            }
            if (backButton.Enabled != enableBackButton) {
                backButton.Enabled = enableBackButton;
            }
            if (nextButton.Enabled != enableNextButton) {
                nextButton.Enabled = enableNextButton;
            }
        }

        protected virtual void OnCancelClicked(CancelEventArgs e) {
            if (CancelClicked != null) {
                CancelClicked(this, e);
            }
        }

        protected virtual void OnNextClicked(CancelEventArgs e) {
            if (NextClicked != null) {
                NextClicked(this, e);
            }
        }

        protected virtual void OnBackClicked(CancelEventArgs e) {
            if (BackClicked != null) {
                BackClicked(this, e);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            CancelEventArgs ce = new CancelEventArgs();
            OnCancelClicked(ce);
            if (controller != null && !ce.Cancel) {
                controller.Cancel();
            }
        }

        private void nextButton_Click(object sender, EventArgs e) {
            CancelEventArgs ce = new CancelEventArgs();
            OnNextClicked(ce);
            if (controller != null && !ce.Cancel) {
                controller.MoveNext();
            }
        }

        private void backButton_Click(object sender, EventArgs e) {
            CancelEventArgs ce = new CancelEventArgs();
            OnBackClicked(ce);
            if (controller != null && !ce.Cancel) {
                controller.MoveBack();
            }
        }

        private void WizardButtonBar_Load(object sender, EventArgs e) {
            if (controller != null) {
                // Be wary of NotSupportedExceptions, in case the controller
                // does not support even the basic properties expected in
                // the interface.
                try {
                    nextButton.Enabled = controller.CanMoveNext;
                }
                catch (NotSupportedException) { }

                try {
                    backButton.Enabled = controller.CanMoveBack;
                }
                catch (NotSupportedException) { }

                try {
                    cancelButton.Enabled = controller.CanCancel;
                }
                catch (NotSupportedException) { }

                try {
                    if (controller.ShowFinish) {
                        nextButton.Text = "Finish";
                    }
                }
                catch (NotSupportedException) { }
            }
        }
    }
}
