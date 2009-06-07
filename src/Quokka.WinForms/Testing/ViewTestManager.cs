using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Quokka.DynamicCodeGeneration;
using Quokka.ServiceLocation;
using Quokka.Uip;

namespace Quokka.WinForms.Testing
{
	public class ViewTestManager
	{
		private readonly Dictionary<Type, Type> _viewDictionary;
		private readonly IUipViewManager _uipViewManager;
		private readonly IServiceContainer _serviceContainer;
		private object _currentView;
		private ViewTestNode _currentNode;

		private readonly IList<ViewTestNode> _testNodes;

		public ViewTestManager(IUipViewManager uipViewManager, Assembly assembly)
		{
			_uipViewManager = uipViewManager;
			_serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
			_viewDictionary = new Dictionary<Type, Type>();
			_testNodes = new List<ViewTestNode>();

			if (assembly == null)
			{
				assembly = Assembly.GetEntryAssembly();
			}
			List<ViewTestNode> list = LoadViewsFromAttributes(assembly);

			_testNodes = list.AsReadOnly();
		}

		public ViewTestManager(IUipViewManager uipViewManager) : this(uipViewManager, null)
		{
		}

		public IEnumerable<Type> ViewTypes
		{
			get { return _viewDictionary.Keys; }
		}

		public IList<ViewTestNode> ViewTestNodes
		{
			get { return _testNodes; }
		}

		public ViewTestNode CurrentNode
		{
			get { return _currentNode; }
		}

		public IServiceContainer ServiceContainer
		{
			get { return _serviceContainer; }
		}

		public void AddView(Type viewType, Type controllerType)
		{
			_viewDictionary.Add(viewType, controllerType);
		}

		public void AddService(Type interfaceType, Type instanceType)
		{
			_serviceContainer.RegisterType(interfaceType, instanceType, null, ServiceLifecycle.Singleton);
		}

		public void ShowNode(ViewTestNode node)
		{
			IServiceContainer childContainer = _serviceContainer.CreateChildContainer();
			childContainer.RegisterType(node.ControllerType, null, null, ServiceLifecycle.Singleton);

			object controller = childContainer.Locator.GetService(node.ControllerType);

			Type nestedType = node.ViewType.GetNestedType("IController");
			if (nestedType != null && nestedType.IsInterface)
			{
				object proxyController = ProxyFactory.CreateDuckProxy(nestedType, controller);
				childContainer.RegisterInstance(nestedType, proxyController);
			}

			object view = childContainer.Locator.GetService(node.ViewType);
			UipUtil.SetController(view, controller, false);
			_uipViewManager.BeginTransition();
			if (_currentView != null)
			{
				_uipViewManager.RemoveView(_currentView);
				_currentView = null;
			}
			_uipViewManager.AddView(view, controller);
			_uipViewManager.ShowView(view);
			_currentView = view;
			_uipViewManager.EndTransition();
			_currentNode = node;
		}

		private static List<ViewTestNode> LoadViewsFromAttributes(Assembly assembly)
		{
			List<ViewTestNode> list = new List<ViewTestNode>();
			foreach (Type type in assembly.GetTypes())
			{
				foreach (TestsViewAttribute attribute in type.GetCustomAttributes(typeof (TestsViewAttribute), false))
				{
					ViewTestNode testNode = new ViewTestNode(attribute.ViewType, type, attribute.Comment);
					list.Add(testNode);
				}
			}

			list.Sort();
			return list;
		}

		public void Clear()
		{
			_uipViewManager.BeginTransition();
			if (_currentView != null)
			{
				_uipViewManager.RemoveView(_currentView);
				_currentView = null;
			}
			_uipViewManager.EndTransition();
		}
	}
}