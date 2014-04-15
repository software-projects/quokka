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

namespace Quokka.Stomp.Internal
{
	public struct HeartBeatValues
	{
		public readonly int Outgoing;
		public readonly int Incoming;
		private static readonly char[] SeparatorChars = new[] {','};

		public override string ToString()
		{
			return Outgoing + "," + Incoming;
		}

		public HeartBeatValues(int outgoing, int incoming)
		{
			Outgoing = outgoing;
			Incoming = incoming;
		}

		public HeartBeatValues CombineWith(HeartBeatValues other)
		{
			int outgoing;
			int incoming;

			if (Outgoing == 0 || other.Incoming == 0)
			{
				outgoing = 0;
			}
			else
			{
				outgoing = Math.Max(Outgoing, other.Incoming);
			}

			if (Incoming == 0 || other.Outgoing == 0)
			{
				incoming = 0;
			}
			else
			{
				incoming = Math.Max(Incoming, other.Outgoing);
			}

			return new HeartBeatValues(outgoing, incoming);
		}

		public HeartBeatValues(string text)
		{
			Outgoing = 0;
			Incoming = 0;
			if (!string.IsNullOrEmpty(text))
			{
				var values = text.Split(SeparatorChars, 2);
				if (values.Length > 0)
				{
					int.TryParse(values[0], out Outgoing);
				}
				if (values.Length > 1)
				{
					int.TryParse(values[1], out Incoming);
				}
			}
		}
	}
}
