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
	/// Specifies the thread on which an <see cref="Event{TPayload}"/> subscriber will be called.
	/// </summary>
	public enum ThreadOption
	{
		/// <summary>
		/// The call is performed on the same thread on which the <see cref="Event{TPayload}"/> was published.
		/// </summary>
		PublisherThread,

		/// <summary>
		/// The call is performed synchronously on the UI thread.
		/// </summary>
		UIThread,

		/// <summary>
		/// The call call is perfomed asynchronously on the UI thread.
		/// </summary>
		UIThreadPost,

		/// <summary>
		/// The call is performed asynchronously on a background thread.
		/// </summary>
		BackgroundThread
	}
}