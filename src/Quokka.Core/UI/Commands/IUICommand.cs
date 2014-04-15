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
using System.ComponentModel;

namespace Quokka.UI.Commands
{
	/// <summary>
	///		Represents a command that can be invoked from a UI control.
	/// </summary>
	/// <remarks>
	///		Acknowledgement to Component Factory Krypton Toolkit for the 
	///		basic idea (http://componentfactory.com/). Implementation differs slightly.
	/// </remarks>
	public interface IUICommand
	{
		/// <summary>
		///		Raised when the command is executed, via the <see cref="PerformExecute"/> method.
		/// </summary>
		event EventHandler Execute;

		/// <summary>
		///		Raised when the value of a property changes.
		/// </summary>
		event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///		Is the user interface control checked. 
		/// </summary>
		/// <remarks>
		///		Not all controls support being checked. If a control does not support being
		///		checked, setting this property has not effect.
		/// </remarks>
		bool Checked { get; set; }

		/// <summary>
		///		Gets or sets whether the user interface control is enabled.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		///		Gets or sets the text associated with the user interface control.
		/// </summary>
		string Text { get; set; }

		/// <summary>
		///		Raises the <see cref="Execute"/> event.
		/// </summary>
		void PerformExecute();
	}
}