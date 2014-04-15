#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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
