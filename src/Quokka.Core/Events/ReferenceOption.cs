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

namespace Quokka.Events
{
	/// <summary>
	/// Provides options for how the event subscription will refer to
	/// the action delegate.
	/// </summary>
	/// <remarks>
	/// This could have been a boolean value, because it is very unlikely
	/// to ever have more than two values. (Is there a CLI enumeration that
	/// does the same job that we could use?)
	/// </remarks>
	public enum ReferenceOption
	{
		/// <summary>
		/// The event subscription will use a weak reference to the
		/// action delegate. The subscription will lapse if the action
		/// delegate is garbage collected.
		/// </summary>
		WeakReference,

		/// <summary>
		/// The event subscription will use a normal (ie strong) reference
		/// to the action delegate. This will prevent the action delegate
		/// from being garbage collected until the event subscription is
		/// unsubscribed.
		/// </summary>
		StrongReference
	}
}