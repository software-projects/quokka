namespace Quokka.WinForms.Regions
{
	/// <summary>
	/// A region that only allows one active view at a time.
	/// </summary>
	public abstract class SingleActiveRegion : Region
	{
		public override void Activate(object view)
		{
			foreach (object activeView in ActiveViews)
			{
				if (activeView != view && Views.Contains(activeView))
				{
					base.Deactivate(activeView);
				}
			}
			base.Activate(view);
		}
	}
}