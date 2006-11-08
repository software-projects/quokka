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
