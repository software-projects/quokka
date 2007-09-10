namespace Quokka.WinForms.Testing
{
	using System;
	using System.Text;
	using Quokka.Diagnostics;

	public class ViewTestNode :  IComparable<ViewTestNode>
	{
		public readonly Type ViewType;
		public readonly Type ControllerType;
		public readonly string Comment;
		public string _stringRepresentation;

		public ViewTestNode(Type viewType, Type controllerType, string comment)
		{
			Verify.ArgumentNotNull(viewType, "viewType");
			Verify.ArgumentNotNull(controllerType, "controllerType");
			ViewType = viewType;
			ControllerType = controllerType;
			Comment = comment ?? String.Empty;
		}

		public string ViewText
		{
			get { return ViewType.ToString(); }
		}

		public string ControllerText
		{
			get { return ControllerType.ToString(); }
		}

		public override bool Equals(object obj)
		{
			ViewTestNode other = obj as ViewTestNode;
			if (other == null)
				return false;

			return ViewType == other.ViewType
			       && ControllerType == other.ControllerType
			       && Comment == other.Comment;
		}

		public override int GetHashCode()
		{
			return ViewType.GetHashCode() ^ ControllerType.GetHashCode();
		}

		public override string ToString()
		{
			if (_stringRepresentation == null) {
				StringBuilder sb = new StringBuilder();
				sb.Append("View: ");
				sb.Append(ViewType.FullName);
				sb.Append(", Controller:");
				sb.Append(ControllerType.FullName);
				_stringRepresentation = sb.ToString();
			}
			return _stringRepresentation;
		}

		public int CompareTo(ViewTestNode other)
		{
			if (other == null)
				return 1;

			int ret = StringComparer.InvariantCulture.Compare(ViewType.ToString(), other.ViewType.ToString());
			if (ret != 0)
				return ret;

			ret = StringComparer.InvariantCultureIgnoreCase.Compare(ControllerType.ToString(), other.ControllerType.ToString());
			if (ret != 0)
				return ret;

			return StringComparer.InvariantCultureIgnoreCase.Compare(Comment, other.Comment);
		}
	}
}
