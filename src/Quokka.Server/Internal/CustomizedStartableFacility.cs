using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Castle.Facilities.Startable;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.SubSystems.Conversion;

namespace Quokka.Server.Internal
{
	/// <summary>
	/// Custom version of the Castle startable facility. Instead of starting
	/// components as soon as possible, this facility stores all startable
	/// components in the wait list, and starts them when the Start method is called.
	/// Also generates warnings for startable components that cannot be instantiated.
	/// </summary>
	/// <remarks>
	/// It's a pity, but the StartableFacility does not lend itself to subtyping,
	/// as everything we need is private, so we duplicate a bit of code here.
	/// </remarks>
	public class CustomizedStartableFacility : StartableFacility
	{
		private readonly List<IHandler> _waitList = new List<IHandler>();
		private bool _inStart;

		public ILogger Logger { get; set; }

		public CustomizedStartableFacility()
		{
			Logger = NullLogger.Instance;
		}

		protected override void Init()
		{
			var converter = Kernel.GetConversionManager();
			Kernel.ComponentModelBuilder.AddContributor(new StartableContributor(converter));
			Kernel.ComponentRegistered += HandleComponentRegistered;
		}

		private void HandleComponentRegistered(string key, IHandler handler)
		{
			if (IsStartable(handler))
			{
				_waitList.Add(handler);
			}
		}

		private bool IsStartable(IHandler handler)
		{
			var startable = handler.ComponentModel.ExtendedProperties["startable"];
			var isStartable = (bool?)startable;
			return isStartable.GetValueOrDefault();
		}

		public void StartAll()
		{
			var array = _waitList.ToArray();
			_waitList.Clear();

			var unresolvedHandlers = new List<IHandler>();

			foreach (var handler in array)
			{
				Logger.Debug("Starting {0}", handler.ComponentModel.Implementation.FullName);
				if (!TryStart(handler))
				{
					if (handler.ComponentModel.Name != handler.ComponentModel.Implementation.FullName)
					{
						Logger.WarnFormat("Cannot resolve type {0} (key={1})", handler.ComponentModel.Implementation.FullName,
										  handler.ComponentModel.Name);
					}
					else
					{
						Logger.WarnFormat("Cannot resolve type {0}", handler.ComponentModel.Implementation.FullName,
						                  handler.ComponentModel.Name);
						unresolvedHandlers.Add(handler);
					}
				}
			}

			foreach (var handler in unresolvedHandlers)
			{
				// TODO: should this be a fatal error or not.
			}
		}

		private bool TryStart(IHandler handler)
		{
			try
			{
				_inStart = true;
				return handler.TryResolve(CreationContext.Empty) != null;
			}
			finally
			{
				_inStart = false;
			}
		}

	}
}
