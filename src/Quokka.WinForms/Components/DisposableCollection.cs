using System;

namespace Quokka.WinForms.Components
{
	/// <summary>
	/// Non-generic form of <see cref="DisposableCollection{T}"/>, which contains <see cref="IDisposable"/> objects.
	/// </summary>
	public class DisposableCollection : DisposableCollection<IDisposable>
	{
	}
}