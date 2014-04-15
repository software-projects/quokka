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

namespace Quokka.Sprocket
{
	/// <summary>
	/// Interface implemented by messages that indicate the current status
	/// of something.
	/// </summary>
	public interface IStatusMessage
	{
		/// <summary>
		/// Returns a string that identifies the object that this message
		/// is reporting.
		/// </summary>
		/// <returns>
		/// A string representation of the unique identifier for the object
		/// that this message is reporting on.
		/// </returns>
		string GetStatusId();
	}
}
