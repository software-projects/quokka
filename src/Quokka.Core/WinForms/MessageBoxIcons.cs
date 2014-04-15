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

using System.Drawing;
using Quokka.UI.Messages;
using Quokka.Properties;

namespace Quokka.WinForms
{
	public class MessageBoxIcons
	{
		public static readonly Image Information = Resources.Information;
		public static readonly Image Success = Resources.Success;
		public static readonly Image Question = Resources.Question;
		public static readonly Image Warning = Resources.Warning;
		public static readonly Image Forbidden = Resources.Forbidden;
		public static readonly Image Unauthorized = Resources.NoEntry;
		public static readonly Image Failure = Resources.Failure;

		public static Image GetImage(UIMessageType messageType)
		{
			switch (messageType)
			{
				case UIMessageType.Information:
					return Information;
				case UIMessageType.Success:
					return Success;
				case UIMessageType.Question:
					return Question;
				case UIMessageType.Warning:
					return Warning;
				case UIMessageType.Forbidden:
					return Forbidden;
				case UIMessageType.Unauthorized:
					return Unauthorized;
				case UIMessageType.Failure:
					return Failure;

					// Should not happen
				default:
					return Information;
			}
		}
	}
}