namespace Quokka.Uip
{
	using System;
	using System.Collections.Generic;
	using Quokka.DynamicCodeGeneration;
	using Quokka.Reflection;

	public class UipTask<TState> : UipTask
	{
		private readonly TState _state;

		#region Construction

		protected UipTask()
		{
			_state = (TState)ObjectFactory.Create(typeof(TState), ServiceProvider, ServiceProvider, this);
			AddNestedStateInterfaces();
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

		#region Private methods

		/// <summary>
		/// Create proxies to the state object for nested interfaces called 'IState'
		/// </summary>
		/// <remarks>
		/// <para>
		/// Iterate through each controller and view class looking for nested interfaces called 'IState'.
		/// If a class contains a nested interface called 'IState', assume that this is
		/// a 'duck proxy' reference to the real state object, and create an entry in the service
		/// container that will return a duck proxy to the state object when a service with the nested
		/// interface is requested.
		/// </para>
		/// </remarks>
		private void AddNestedStateInterfaces()
		{
			// Get the distinct controller types, as a controller type may be present in more than
			// one node.
			Dictionary<Type, Type> typeDict = new Dictionary<Type, Type>();
			foreach (UipNode node in Nodes)
			{
				Type controllerType = node.ControllerType;
				if (controllerType != null)
				{
					typeDict[controllerType] = controllerType;
				}
				Type viewType = node.ViewType;
				if (viewType != null)
				{
					typeDict[viewType] = viewType;
				}
			}

			// Look for nested interface types called "IState" and assume that they are 
			// duck typing references to the state object.
			foreach (Type controllerType in typeDict.Keys)
			{
				Type nestedType = controllerType.GetNestedType("IState");
				if (nestedType != null && nestedType.IsInterface)
				{
					// create a duck proxy for the state object and add it to the service provider
					object stateProxy = ProxyFactory.CreateDuckProxy(nestedType, _state);
					_serviceContainer.AddService(nestedType, stateProxy);
				}
			}
		}

		#endregion
	}	
}