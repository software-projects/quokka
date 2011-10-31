using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Quokka.WinForms;

namespace Sprocket.Manager.Views
{
    public static class DisplaySettingsExtensions
    {
        public static void SaveSplitterWidth(this DisplaySettings displaySettings, KryptonSplitContainer splitContainer)
        {
            string valueName = splitContainer.Name + "." + "SplitterDistance";
            splitContainer.FixedPanel = FixedPanel.Panel1;
            var splitterDistance = displaySettings.GetInt(valueName, -1);
            if (splitterDistance > 0)
            {
                splitContainer.SplitterDistance = splitterDistance;
            }
            splitContainer.SplitterMoved += delegate
            {
                displaySettings.SetInt(valueName, splitContainer.SplitterDistance);
            };
        }
    }
}
