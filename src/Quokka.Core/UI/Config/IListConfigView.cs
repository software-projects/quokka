using Quokka.Config;
using Quokka.UI.Commands;
using Quokka.WinForms;

namespace Quokka.UI.Config
{
	public interface IListConfigView
	{
		IUICommand EditCommand { get; }
		IUICommand RefreshCommand { get; }
		IVirtualDataSource<IConfigParameter> DataSource { set; }
		IConfigParameter Current { get; }
	}
}