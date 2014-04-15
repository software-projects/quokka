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
	/// Base class for all events.
	/// </summary>
	/// <remarks>
	/// This class contains all functionality that does not rely on the
	/// generic type parameter (<c>TPayload</c>) in the derived <see cref="Event"/> class.
	/// </remarks>
	public class EventBase
	{
		public IEventBroker EventBroker { get; internal set; }
	}
}