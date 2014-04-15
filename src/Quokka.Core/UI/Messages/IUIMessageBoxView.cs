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

namespace Quokka.UI.Messages
{
	/// <summary>
	/// 	Interface implemented by a view that can display a modal message box.
	/// </summary>
	public interface IUIMessageBoxView
	{
		/// <summary>
		/// 	Raised when the user selects one of the possible answers.
		/// </summary>
		/// <remarks>
		/// 	The answer selected is stored in the <see cref = "UIMessage.SelectedAnswer" /> property 
		/// 	of the message.
		/// </remarks>
		event EventHandler Answered;

		/// <summary>
		/// 	Contains the message to be asked of the user.
		/// </summary>
		UIMessage Message { get; set; }
	}
}