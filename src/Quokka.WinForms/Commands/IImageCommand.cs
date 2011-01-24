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
		Image ImageLarge { get; set; }
		Image ImageSmall { get; set; }
		Color ImageTransparentColor { get; set; }

		void Execute();
	}
}