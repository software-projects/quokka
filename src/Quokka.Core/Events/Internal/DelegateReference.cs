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