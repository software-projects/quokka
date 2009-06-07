using System;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Quokka.Unity
{
	internal class ContainerExtension : UnityContainerExtension
	{
		/// <summary>
		/// Evaluates if a specified type was registered in the container.
		/// </summary>
		/// <param name="container">The container to check if the type was registered in.</param>
		/// <param name="type">The type to check.</param>
		/// <param name="name">The name associated with the type, or <see langword="null"/> for the default mapping for the type.</param>
		/// <returns><see langword="true" /> if the <paramref name="type"/> was registered with the container.</returns>
		/// <remarks>
		/// In order to use this extension, you must first call <see cref="IUnityContainer.AddNewExtension{TExtension}"/> 
		/// and specify <see cref="UnityContainerExtension"/> as the extension type.
		/// </remarks>
		public static bool IsTypeRegistered(IUnityContainer container, Type type, string name)
		{
			ContainerExtension extension = container.Configure<ContainerExtension>();
			if (extension == null)
			{
				//Extension was not added to the container.
				return false;
			}
			IBuildKeyMappingPolicy policy;

			if (name == null)
			{
				policy = extension.Context.Policies.Get<IBuildKeyMappingPolicy>(new NamedTypeBuildKey(type));
			}
			else
			{
				policy = extension.Context.Policies.Get<IBuildKeyMappingPolicy>(new NamedTypeBuildKey(type, name));
			}
			return policy != null;
		}

		protected override void Initialize()
		{
		}
	}
}