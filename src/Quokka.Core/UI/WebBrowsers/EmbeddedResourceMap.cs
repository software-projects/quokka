using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Quokka.UI.WebBrowsers
{
	public class EmbeddedResourceMap
	{
		private readonly HashSet<Assembly> _assemblies = new HashSet<Assembly>();
		private Lazy<Dictionary<string, EmbeddedResource>> _lazy;


		public EmbeddedResourceMap()
		{
			ClearDict();
		}

		public void AddAssembly(Assembly assembly)
		{
			if (!_assemblies.Contains(assembly))
			{
				_assemblies.Add(assembly);
				ClearDict();
			}
		}

		public Stream GetStream(string name)
		{
			Uri uri = new Uri(name);

			var path = uri.AbsolutePath;
			var pieces = path.Split('/');
			var fileName = pieces[pieces.Length - 1];

			EmbeddedResource manifestInfo = null;
			if (_lazy.Value.TryGetValue(fileName, out manifestInfo))
			{
				return manifestInfo.Assembly.GetManifestResourceStream(manifestInfo.ResourceName);
			}
			return null;
		}

		private void ClearDict()
		{
			_lazy = new Lazy<Dictionary<string, EmbeddedResource>>(CreateDict, true);
		}

		private static readonly string[] Suffixes = new[]
		                                            	{
		                                            		".html",
		                                            		".htm",
		                                            		".js",
		                                            		".css",
		                                            		".png",
		                                            		".jpeg",
		                                            		".gif",
		                                            	};

		private static bool IsValidResource(string name)
		{
			return Suffixes.Any(name.EndsWith);
		}

		private Dictionary<string, EmbeddedResource> CreateDict()
		{
			var dict = new Dictionary<string, EmbeddedResource>(StringComparer.InvariantCultureIgnoreCase);

			foreach (var assembly in _assemblies)
			{
				foreach (var resourceName in assembly.GetManifestResourceNames())
				{
					if (IsValidResource(resourceName))
					{
						var embeddedResource = new EmbeddedResource(assembly, resourceName);

						// split name into bits separated by periods
						var nameParts = resourceName.Split('.');

						var fileName = nameParts[nameParts.Length - 2] + "." + nameParts[nameParts.Length - 1];

						dict[fileName] = embeddedResource;

						// TODO: add alternatives that include the rest of the path
					}
				}
			}
			return dict;
		}

		private class EmbeddedResource
		{
			public Assembly Assembly { get; private set; }
			public string ResourceName { get; private set; }

			public EmbeddedResource(Assembly assembly, string resourceName)
			{
				Assembly = assembly;
				ResourceName = resourceName;
			}
		}
	}
}