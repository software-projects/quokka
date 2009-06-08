using System.Collections.Generic;
using Quokka.Uip;

namespace Quokka.UI
{
	/// <summary>
	/// A 'strawman' interface if all interactions were via UITasks
	/// </summary>
	public interface IWorkspace
	{
		IEnumerable<UipTask> Tasks { get; }

		void StartTask(UipTask task);
	}
}