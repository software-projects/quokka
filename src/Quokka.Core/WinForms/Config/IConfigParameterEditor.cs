using System;
using System.Windows.Forms;
using Quokka.Config;

namespace Quokka.WinForms.Config
{
	public interface IConfigParameterEditor
	{
		string TextValue { get; }
		Control Control { get; }
		ConfigParameter Parameter { get; }

		void Initialize(ConfigParameter parameter);
	}
}