using System;

namespace Quokka.Collections
{
	public class DisposableCollection : DisposableCollection<IDisposable>, IDisposableCollection
	{
	}
}