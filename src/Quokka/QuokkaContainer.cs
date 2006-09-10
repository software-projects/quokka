
namespace Quokka
{
    using System;

    /// <summary>
    /// Slightly simpler to use that <see cref="ServiceContainerUtil"/>
    /// </summary>
    /// <remarks>
    /// Extension methods, which become availble in C# version 3.0 will
    /// make this class obsolete. This is because the <c>ServiceContainerUtil</c> 
    /// static methods can be defined like:
    /// <code>
    /// public static void AddService(this ServiceContainer container, Type serviceType, Type serviceIntanceType) { ... }
    /// </code>
    /// </remarks>
    public class QuokkaContainer : System.ComponentModel.Design.ServiceContainer
    {
        public QuokkaContainer() { }
        public QuokkaContainer(IServiceProvider parentProvider) : base(parentProvider) { }

        public void AddService(Type serviceType, Type serviceInstanceType) {
            ServiceContainerUtil.AddService(this, serviceType, serviceInstanceType);
        }

        public virtual void AddService(Type serviceType, Type serviceInstanceType, bool promote) {
            ServiceContainerUtil.AddService(this, serviceType, serviceInstanceType, promote);
        }
    }
}
