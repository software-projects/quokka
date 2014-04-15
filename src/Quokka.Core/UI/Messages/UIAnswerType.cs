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

namespace Quokka.UI.Messages
{
	/// <summary>
	/// 	Identifies a set of common answers that are provided in response to UI messages.
	/// </summary>
	/// <remarks>
	/// 	The integer values assigned to these enumerations match the values in the
	/// 	System.Windows.Forms.DialogResult enumeration.
	/// </remarks>
	public enum UIAnswerType
	{
		/// <summary>
		/// 	Custom answer, which indicates the answer is not one of the common
		/// 	answer types.
		/// </summary>
		Custom = 0,
		OK = 1,
		Cancel = 2,
		Abort = 3,
		Retry = 4,
		Ignore = 5,
		Yes = 6,
		No = 7,
	}
}