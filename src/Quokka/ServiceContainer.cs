
namespace Quokka
{
    using System;
    using SCMD = System.ComponentModel.Design;

    public class ServiceContainer : SCMD.ServiceContainer
    {
        private ServiceFactory factory = new ServiceFactory();

        public ServiceContainer() { }
        public ServiceContainer(IServiceProvider parentProvider) : base(parentProvider) { }

        public void AddService(Type serviceType, Type serviceInstanceType) {
            AddService(serviceType, serviceInstanceType, false);
        }

        public virtual void AddService(Type serviceType, Type serviceInstanceType, bool promote) {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (serviceInstanceType == null)
                throw new ArgumentNullException("serviceInstanceType");

            SCMD.ServiceCreatorCallback callback = factory.AddService(serviceType, serviceInstanceType);
            base.AddService(serviceType, callback, promote);
        }

        public override void RemoveService(Type serviceType, bool promote) {
            base.RemoveService(serviceType, promote);
            factory.RemoveService(serviceType);
        }
    }
}
