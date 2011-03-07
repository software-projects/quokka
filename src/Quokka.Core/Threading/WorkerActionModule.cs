using System;

namespace Quokka.Threading
{
	/// <summary>
	/// 	Simple base class for implementing <see cref = "IWorkerActionModule" />
	/// </summary>
	public class WorkerActionModule : IWorkerActionModule
	{
		public virtual void Before()
		{
		}

		public virtual void After()
		{
		}

		public virtual void Error(Exception ex)
		{
		}

		public virtual void Finished()
		{
		}
	}
}