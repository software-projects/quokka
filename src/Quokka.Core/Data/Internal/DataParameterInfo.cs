using System.Reflection;
using Quokka.Diagnostics;

namespace Quokka.Data.Internal
{
	internal class DataParameterInfo
	{
		public ParameterAttribute Attribute { get; private set; }
		public PropertyInfo Property { get; private set; }

		public DataParameterInfo(ParameterAttribute attribute, PropertyInfo property)
		{
			Attribute = Verify.ArgumentNotNull(attribute, "attribute");
			Property = Verify.ArgumentNotNull(property, "property");
		}
	}
}