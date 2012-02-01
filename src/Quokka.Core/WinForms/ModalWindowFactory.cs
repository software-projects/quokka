using System.Windows.Forms;
using Quokka.UI.Tasks;

namespace Quokka.WinForms
{
	public class ModalWindowFactory : IModalWindowFactory
	{
		public IModalWindow CreateModalWindow(IWin32Window owner)
		{
			return new ModalWindow {Owner = owner};
		}
	}
}