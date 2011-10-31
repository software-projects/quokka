using Quokka.WinForms.Commands;

namespace Quokka.WinForms.Interfaces
{
	/// <summary>
	/// Represents a main menu that can be grouped. Think of a list view with icons
	/// </summary>
	public interface IMainMenu
	{
		void Add(IImageCommand command, string group);
		void Remove(IImageCommand command);
	}
}