using System.Windows.Forms;
using Quokka.UI.Tasks;

namespace Quokka.WinForms
{
	public interface IModalWindowFactory
	{
		IModalWindow CreateModalWindow(IWin32Window owner);
	}
}