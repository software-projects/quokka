using System;
using System.Windows.Forms;
using Quokka.Config;
using Quokka.Config.Storage;

namespace Quokka.WinForms.Config
{
	public interface IConfigParameterEditor
	{
		string TextValue { get; }
		Control Control { get; }
		IConfigParameter Parameter { get; }

		void Initialize(IConfigParameter parameter);
	}
}