using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Quokka.UI.Tasks;
using Quokka.WinForms;

namespace Quokka.Krypton
{
	public class KryptonModalWindowFactory : IModalWindowFactory
	{
		public IModalWindow CreateModalWindow(IWin32Window owner)
		{
			return new ModalWindow<KryptonForm, KryptonPanel> {Owner = owner};
		}
	}
}