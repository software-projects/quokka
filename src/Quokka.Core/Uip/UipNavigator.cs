using Quokka.Diagnostics;

namespace Quokka.Uip
{
	/// <summary>
	/// Navigator class used by the <see cref="UipTask"/>.
	/// </summary>
	internal class UipNavigator : IUipNavigator
	{
		private readonly UipTask _task;

		public UipNavigator(UipTask task)
		{
			_task = task;
		}

		public void Navigate(string navigateValue)
		{
			Verify.ArgumentNotNull(navigateValue, "navigateValue");
			_task.Navigate(navigateValue);
		}

		public bool CanNavigate(string navigateValue)
		{
			Verify.ArgumentNotNull(navigateValue, "navigateValue");
			UipNode nextNode;
			return _task.CurrentNode.GetNextNode(navigateValue, out nextNode);
		}
	}
}