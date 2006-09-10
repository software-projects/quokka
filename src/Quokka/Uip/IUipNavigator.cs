using System;

namespace Quokka.Uip
{
    public interface IUipNavigator
    {
        bool CanNavigate(string navigateValue);
        void Navigate(string navigateValue);
    }
}
