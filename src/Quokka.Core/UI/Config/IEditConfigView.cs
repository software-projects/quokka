using Quokka.Config;
using Quokka.UI.Commands;

namespace Quokka.UI.Config
{
	public interface IEditConfigView
	{
		IUICommand SaveCommand { get; }
		IUICommand CancelCommand { get; }
		IConfigParameter Parameter { get; set; }
		string Value { get; }
	}
}