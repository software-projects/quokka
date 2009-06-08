using System.Drawing;

namespace Quokka.WinForms.Workspaces
{
	public interface IWorkspaceInfo
	{
		string Text { get; set; }
		Image Image { get; set; }
		bool CanClose { get; set; }
		void Activate();
	}
}