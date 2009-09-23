using Quokka.Diagnostics;
using Quokka.ServiceLocation;

namespace Quokka.Uip
{
	public class UipTask<TState> : UipTask
		where TState : class
	{
		private readonly TState _state;

		#region Construction

		protected UipTask()
		{
			_serviceContainer.RegisterType<TState>(ServiceLifecycle.Singleton);
			_state = _serviceContainer.Locator.GetInstance<TState>();
		}

		protected UipTask(TState state)
		{
			Verify.ArgumentNotNull(state, "state", out _state);
			_serviceContainer.RegisterInstance(_state);
		}

		#endregion

		#region Public properties

		public TState State
		{
			get { return _state; }
		}

		#endregion

		#region Public methods

		public override object GetStateObject()
		{
			return _state;
		}

		#endregion
	}
}