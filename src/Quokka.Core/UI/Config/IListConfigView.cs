using Quokka.Config;
using Quokka.UI.Commands;
using Quokka.WinForms;

namespace Quokka.UI.Config
{
	public interface IListConfigView
	{
		IUICommand EditCommand { get; }
		IVirtualDataSource<ConfigParameter> DataSource { set; }
		ConfigParameter Current { get; }
	}
}