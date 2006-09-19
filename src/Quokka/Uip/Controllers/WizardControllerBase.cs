using System;
using System.Collections.Generic;
using System.Text;

namespace Quokka.Uip.Controllers
{
    public interface IWizardController
    {
        bool CanMoveNext { get; }
        bool CanMoveBack { get; }
        bool CanCancel { get; }
        bool ShowFinish { get; }
        void MoveNext();
        void MoveBack();
        void Cancel();
    }

    public abstract class WizardControllerBase : IWizardController
    {
        private bool canMoveNext;
        private bool canMoveBack;
        private bool canCancel;
        private bool showFinish;

        public bool CanMoveNext {
            get { return canMoveNext; }
            set { canMoveNext = value; }
        }

        public bool CanMoveBack {
            get { return canMoveBack; }
            set { canMoveBack = value; }
        }

        public bool CanCancel {
            get { return canCancel; }
            set { canCancel = value; }
        }

        public bool ShowFinish {
            get { return showFinish; }
            set { showFinish = value; }
        }

        public abstract void MoveNext();
        public abstract void MoveBack();
        public abstract void Cancel();
    }
}
