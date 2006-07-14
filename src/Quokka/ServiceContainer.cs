
namespace Quokka
{
    using System;
    using SCMD = System.ComponentModel.Design;

    public class QuokkaServiceContainer : SCMD.ServiceContainer
    {
        private ServiceFactory factory = new ServiceFactory();

        public QuokkaServiceContainer() { }
        public QuokkaServiceContainer(IServiceProvider parentProvider) : base(parentProvider) { }

        public void AddService(Type serviceType, Type serviceInstanceType) {
            ServiceContainerUtil.AddServiceTypeInfo(this, serviceType, serviceInstanceType);
        }

        public virtual void AddService(Type serviceType, Type serviceInstanceType, bool promote) {
            ServiceContainerUtil.AddServiceTypeInfo(this, serviceType, serviceInstanceType, promote);
        }
    }
}
