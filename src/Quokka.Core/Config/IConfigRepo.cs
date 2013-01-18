using System;
using System.Collections.Generic;

namespace Quokka.Config
{
	/// <summary>
	/// Interface for finding all configuration items
	/// </summary>
	public interface IConfigRepo
	{
		IList<ConfigParameter> FindAll();
		ConfigParameter FindByName(string name);
	}
}