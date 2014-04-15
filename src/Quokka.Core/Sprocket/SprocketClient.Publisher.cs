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
using System.Threading;
using Quokka.Diagnostics;

namespace Quokka.Sprocket
{
	public partial class SprocketClient
	{
		// TODO: This needs to be much smarter, so that it knows when to publish.
		// Currently it just thinks it has to publish all the time.
		private class Publisher<T> : IPublisher<T>
		{
			private readonly SprocketClient _client;

			public event EventHandler SubscribedChanged;

			public Publisher(SprocketClient client)
			{
				_client = Verify.ArgumentNotNull(client, "client");
				SynchronizationContext = _client.SynchronizationContext;
			}

			public void Dispose()
			{
				SubscribedChanged = null;
			}

			public ISprocket Sprocket
			{
				get { return _client; }
			}

			public SynchronizationContext SynchronizationContext { get; set; }

			public bool Subscribed
			{
				get { return true; }
			}

			public void Publish(T obj)
			{
				_client.Publish(obj);
			}
		}
	}
}
