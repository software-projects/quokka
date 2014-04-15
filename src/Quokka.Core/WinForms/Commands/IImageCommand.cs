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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Quokka.WinForms.Commands
{
	/// <summary>
	/// Represents a command that can be invoked from a UI control.
	/// </summary>
	/// <remarks>
	/// Acknowledgement to Component Factory Krypton Toolkit for the 
	/// basic idea. Implementation differs slightly.
	/// </remarks>
	public interface IImageCommand : INotifyPropertyChanged
	{
		bool Checked { get; set; }
		CheckState CheckState { get; set; }
		bool Enabled { get; set; }
		string Text { get; set; }
		string ExtraText { get; set; }
		Image ImageLarge { get; set; }
		Image ImageSmall { get; set; }
		Color ImageTransparentColor { get; set; }

		void Execute();
	}
}