#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Reflection;
using Quokka.Diagnostics;

namespace Quokka.Events.Internal
{
	/// <summary>
	/// Reference a delegate, possibly via a weak reference.
	/// </summary>
	public class DelegateReference
	{
		private readonly Delegate _delegate;
		private readonly WeakReference _weakReference;
		private readonly MethodInfo _methodInfo;
		private readonly Type _delegateType;

		public DelegateReference(Delegate referencedDelegate, ReferenceOption referenceOption)
		{
			Verify.ArgumentNotNull(referencedDelegate, "referencedDelegate");
			if (referenceOption == ReferenceOption.StrongReference)
			{
				_delegate = referencedDelegate;
			}
			else
			{
				if (referencedDelegate.Target != null)
				{
					_weakReference = new WeakReference(referencedDelegate.Target);
				}
				_methodInfo = referencedDelegate.Method;
				_delegateType = referencedDelegate.GetType();
			}
		}

		/// <summary>
		/// Is this a strong reference or a weak reference
		/// </summary>
		public ReferenceOption ReferenceOption
		{
			get { return _delegate == null ? ReferenceOption.WeakReference : ReferenceOption.StrongReference; }
		}

		/// <summary>
		/// Obtain the delegate. Returns <c>null</c> if the target of the delegate has been garbage collected.
		/// </summary>
		public Delegate Delegate
		{
			get
			{
				if (_delegate != null)
				{
					return _delegate;
				}
				if (_methodInfo.IsStatic)
				{
					// static method
					return Delegate.CreateDelegate(_delegateType, null, _methodInfo);
				}

				// attempt to obtain the target of the weak reference
				object target = _weakReference.Target;
				if (target == null)
				{
					// the target has been garbage collected, so the delegate is now null
					return null;
				}

				// target is still alive, reconstitute the delegate
				return Delegate.CreateDelegate(_delegateType, target, _methodInfo);
			}
		}
	}
}