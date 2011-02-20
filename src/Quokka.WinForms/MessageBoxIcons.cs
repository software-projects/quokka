using System.Drawing;
using Quokka.UI.Messages;
using Quokka.WinForms.Properties;

namespace Quokka.WinForms
{
	public class MessageBoxIcons
	{
		public static readonly Image Information = Resources.Information;
		public static readonly Image Success = Resources.Success;
		public static readonly Image Question = Resources.Question;
		public static readonly Image Warning = Resources.Warning;
		public static readonly Image Forbidden = Resources.Forbidden;
		public static readonly Image Unauthorized = Resources.NoEntry;
		public static readonly Image Failure = Resources.Failure;

		public static Image GetImage(UIMessageType messageType)
		{
			switch (messageType)
			{
				case UIMessageType.Information:
					return Information;
				case UIMessageType.Success:
					return Success;
				case UIMessageType.Question:
					return Question;
				case UIMessageType.Warning:
					return Warning;
				case UIMessageType.Forbidden:
					return Forbidden;
				case UIMessageType.Unauthorized:
					return Unauthorized;
				case UIMessageType.Failure:
					return Failure;

					// Should not happen
				default:
					return Information;
			}
		}
	}
}