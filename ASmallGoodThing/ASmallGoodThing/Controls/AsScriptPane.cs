
namespace mkkim1129.ASmallGoodThing.Controls
{
    [System.Runtime.InteropServices.Guid(GuidList.guidAsScriptPaneString)]
    public class AsScriptPane : Microsoft.VisualStudio.Shell.ToolWindowPane
    {
        public AsScriptPane() : base(null)
        {
            this.Caption = "AsScript";
            //this.BitmapResourceID = 301;
            //this.BitmapIndex = 1;
            this.Content = new AsScriptControl();
        }
    }
}
