using Quokka.Config.Storage;
using Quokka.UI.Commands;
using Quokka.WinForms;

namespace Quokka.UI.Config
{
	public interface IListConfigView
	{
		IUICommand EditCommand { get; }
		IVirtualDataSource<IConfigParameter> DataSource { set; }
		IConfigParameter Current { get; }
	}
}