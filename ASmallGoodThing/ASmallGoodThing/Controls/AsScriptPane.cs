
namespace mkkim1129.ASmallGoodThing.Controls
{
    [System.Runtime.InteropServices.Guid(GuidList.guidAsScriptPaneString)]
    public class AsScriptPane : Microsoft.VisualStudio.Shell.ToolWindowPane
    {
        public AsScriptPane() : base(null)
        {
            this.Caption = "Script - A Small, Good Thing";
            this.Content = new AsScriptControl();
        }
    }
}
