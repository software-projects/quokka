namespace Quokka.WinForms.Testing
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Design;
	using System.Reflection;
	using Quokka.DynamicCodeGeneration;
	using Quokka.Reflection;
	using Quokka.Uip;

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
			_serviceContainer = new ServiceContainer();
			_viewDictionary = new Dictionary<Type, Type>();
			_testNodes = new List<ViewTestNode>();

			if (assembly == null) {
				assembly = Assembly.GetEntryAssembly();
			}
			List<ViewTestNode> list = LoadViewsFromAttributes(assembly);

			_testNodes = list.AsReadOnly();
		}

		public ViewTestManager(IUipViewManager uipViewManager) : this(uipViewManager, null) {}

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
			ServiceContainerUtil.AddService(_serviceContainer, interfaceType, instanceType);
		}

		public void ShowNode(ViewTestNode node)
		{
			object controller = Activator.CreateInstance(node.ControllerType);
			object proxyController = null;
			Type nestedType = node.ViewType.GetNestedType("IController");
			if (nestedType != null && nestedType.IsInterface)
			{
				proxyController = ProxyFactory.CreateDuckProxy(nestedType, controller);
			}

			object view = ObjectFactory.Create(node.ViewType, _serviceContainer, controller, proxyController);
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
			foreach (Type type in assembly.GetTypes()) {
				foreach (TestsViewAttribute attribute in type.GetCustomAttributes(typeof(TestsViewAttribute), false)) {
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
			if (_currentView != null) {
				_uipViewManager.RemoveView(_currentView);
				_currentView = null;
			}
			_uipViewManager.EndTransition();
		}
	}
}
