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
        private IController controller;

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
                nextButton.Enabled = controller.CanMoveNext;
                backButton.Enabled = controller.CanMoveBack;
                cancelButton.Enabled = controller.CanCancel;

                if (controller.ShowFinish) {
                    nextButton.Text = "Finish";
                }
            }
        }
    }
}
