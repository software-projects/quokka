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
			_state = (TState) _serviceContainer.Locator.GetInstance(typeof (TState));
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