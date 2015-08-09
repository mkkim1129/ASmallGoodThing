using ICSharpCode.AvalonEdit.Highlighting;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using WM = System.Windows.Media;

namespace mkkim1129.ASmallGoodThing.Controls
{
    /// <summary>
    /// Interaction logic for AsScriptControl.xaml
    /// </summary>
    public partial class AsScriptControl : UserControl
    {
        private string extensionPath_;
        private ScriptEngine engine_ = null;
        private ScriptScope scope_ = null;
        private List<string> commandHistory_ = new List<string>();
        private int commandHistoryCursor_ = -1;
        private IHighlightingDefinition darkCustomHighlighting_ = null;
        private IHighlightingDefinition lightCustomHighlighting_ = null;
        private IHighlightingDefinition selectedHighlighting_ = null;
        private Dictionary<string, Action> builtInFunctionTable_ = new Dictionary<string, Action>();

        public AsScriptControl()
        {
            string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            extensionPath_ = System.IO.Path.GetDirectoryName(assemblyName).Replace("file:\\", "").Replace("\\", "/");

            InitializeComponent();
            InitializeScriptEngine();

            textboxOutput.Document.UndoStack.SizeLimit = 0;
            textboxOutput.WordWrap = true;

            VSColorTheme.ThemeChanged += VSColorThemeChanged;

            darkCustomHighlighting_ = LoadHighlighting("mkkim1129.ASmallGoodThing.Resources.PythonDark.xshd");
            lightCustomHighlighting_ = LoadHighlighting("mkkim1129.ASmallGoodThing.Resources.PythonLight.xshd");
            UpdateHeghlighting();

            AddScriptTabFile(null);

            builtInFunctionTable_.Add(".cls", () => textboxOutput.Text = "" );
            builtInFunctionTable_.Add(".restart", () => InitializeScriptEngine() );
            builtInFunctionTable_.Add(".run", () => RunScript() );
            builtInFunctionTable_.Add(".sel", () => RunSelectedText() );
        }

        private void InitializeScriptEngine()
        {
            engine_ = Python.CreateEngine();
            scope_ = engine_.CreateScope();

            System.IO.Stream stream = new System.IO.MemoryStream();
            System.IO.TextWriter textWriter = new AsTextBoxStreamWriter(textboxOutput);
            engine_.Runtime.IO.SetOutput(stream, textWriter);
            engine_.Runtime.IO.SetErrorOutput(stream, textWriter);
            string initializeCode = string.Format(@"
import sys
import clr

sys.path.append('{0}')
sys.path.append('{0}/py')
clr.AddReference('AsDebuggerExtension.dll')
from AsDebuggerExtension import *
print(sys.version)
", extensionPath_);
            engine_.Execute(initializeCode, scope_);
        }

        private void VSColorThemeChanged(ThemeChangedEventArgs e)
        {
            UpdateHeghlighting();
        }

        private void UpdateHeghlighting()
        {
            Color colorBackground = VSColorTheme.GetThemedColor(EnvironmentColors.CommandBarOptionsBackgroundColorKey);

            double toWhite = CalculateColorDifference(colorBackground, Color.White);
            double toBlack = CalculateColorDifference(colorBackground, Color.Black);

            if (toBlack < toWhite)
            {
                selectedHighlighting_ = darkCustomHighlighting_;
            }
            else
            {
                selectedHighlighting_ = lightCustomHighlighting_;
            }

            foreach (TabItem tabItem in tabScript.Items)
            {
                var textEditor = tabItem.Content as ICSharpCode.AvalonEdit.TextEditor;
                textEditor.SyntaxHighlighting = selectedHighlighting_;
            }
        }

        private void AddScriptTabFile(string filename) 
        {
            var textEditor = new ICSharpCode.AvalonEdit.TextEditor();
            textEditor.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            textEditor.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textEditor.FontFamily = new WM.FontFamily("Consolas");
            textEditor.ShowLineNumbers = true;
            textEditor.SyntaxHighlighting = selectedHighlighting_;

            string headerText = "NewFile";
            if (String.IsNullOrEmpty(filename) == false)
            {
                textEditor.Document.FileName = filename;
                textEditor.Load(filename);
                headerText = System.IO.Path.GetFileNameWithoutExtension(filename);
            }

            AsClosableTabItem tabItem = new AsClosableTabItem();
            tabItem.SetHeader(headerText);
            tabItem.Content = textEditor;

            int tabIndex = tabScript.Items.Add(tabItem);
            tabScript.SelectedIndex = tabIndex;
        }

        private void RunScript()
        {
            try
            {
                AsClosableTabItem tabItem = tabScript.SelectedItem as AsClosableTabItem;
                string message = "[" + DateTime.Now.ToString("tt HH:mm:ss") + "] Run Script(" + tabItem.HeaderText + ")" + Environment.NewLine;
                textboxOutput.AppendText(message);
                textboxOutput.ScrollToEnd();

                var textEditor = tabItem.Content as ICSharpCode.AvalonEdit.TextEditor;
                engine_.Execute(textEditor.Document.Text, scope_);
            }
            catch (Exception exception)
            {
                textboxOutput.AppendText(exception.Message);
                textboxOutput.AppendText(Environment.NewLine);
                textboxOutput.ScrollToEnd();
            }
        }

        private void RunSelectedText()
        {
            try
            {
                // TODO : 탭이 없는 것 정도는 별도로 처리해주자
                AsClosableTabItem tabItem = tabScript.SelectedItem as AsClosableTabItem;
                var textEditor = tabItem.Content as ICSharpCode.AvalonEdit.TextEditor;
                string selectedText = textEditor.SelectedText;
                textboxOutput.AppendText(">>> " + selectedText + Environment.NewLine);
                textboxOutput.ScrollToEnd();

                engine_.Execute(selectedText, scope_);
            }
            catch (Exception exception)
            {
                textboxOutput.AppendText(exception.Message);
                textboxOutput.AppendText(Environment.NewLine);
                textboxOutput.ScrollToEnd();
            }
        }
        
        private void textboxCommand_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                try
                {
                    string command = textboxCommand.Text;
                    textboxCommand.Text = "";
                    if (string.IsNullOrEmpty(command) == false)
                    {
                        if ((commandHistory_.Count == 0) || (commandHistory_[commandHistory_.Count - 1] != command))
                        {
                            commandHistory_.Add(command);
                        }
                        commandHistoryCursor_ = commandHistory_.Count;
                    }

                    if (builtInFunctionTable_.ContainsKey(command) == true)
                    {
                        builtInFunctionTable_[command]();
                    }
                    else
                    {
                        textboxOutput.AppendText(">>> " + command + Environment.NewLine);
                        textboxOutput.ScrollToEnd();
                        engine_.Execute(command, scope_);
                    }
                }
                catch (Exception exception)
                {
                    textboxOutput.AppendText(exception.Message);
                    textboxOutput.AppendText(Environment.NewLine);
                    textboxOutput.ScrollToEnd();
                }
            }
        }

        private void textboxCommand_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (commandHistory_.Count == 0)
                {
                    return;
                }

                if (commandHistoryCursor_ == -1)
                {
                    commandHistoryCursor_ = commandHistory_.Count - 1;
                }
                else
                {
                    if (commandHistoryCursor_ > 0)
                    {
                        --commandHistoryCursor_;
                    }
                }

                textboxCommand.Text = commandHistory_[commandHistoryCursor_];
                textboxCommand.CaretIndex = textboxCommand.Text.Length;
            }
            else if (e.Key == Key.Down)
            {
                if (commandHistory_.Count == 0)
                {
                    return;
                }

                if (commandHistoryCursor_ == -1)
                {
                    commandHistoryCursor_ = commandHistory_.Count - 1;
                }
                else
                {
                    ++commandHistoryCursor_;
                    commandHistoryCursor_ = Math.Min(commandHistoryCursor_, commandHistory_.Count - 1);
                }

                textboxCommand.Text = commandHistory_[commandHistoryCursor_];
                textboxCommand.CaretIndex = textboxCommand.Text.Length;
            }
        }

        private void newFileClick(object sender, System.Windows.RoutedEventArgs e)
        {
            AddScriptTabFile(null);
        }

        private void openFileClick(object sender, System.Windows.RoutedEventArgs e)
        {
            List<string> opened = new List<string>();
            foreach (TabItem tabItem in tabScript.Items)
            {
                var textEditor = tabItem.Content as ICSharpCode.AvalonEdit.TextEditor;
                if (string.IsNullOrEmpty(textEditor.Document.FileName) == false)
                {
                    opened.Add(textEditor.Document.FileName);
                }
            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.Multiselect = true;
            if (dlg.ShowDialog() ?? false)
            {
                foreach (string filename in dlg.FileNames)
                {
                    if (opened.Contains(filename) == false)
                    {
                        AddScriptTabFile(filename);
                    }
                    else
                    {
                        string message = "[" + DateTime.Now.ToString("tt HH:mm:ss") + "] File already opened(" + filename + ")" + Environment.NewLine;
                        textboxOutput.AppendText(message);
                        textboxOutput.ScrollToEnd();
                    }
                }
            }
        }

        private void saveFileClick(object sender, System.Windows.RoutedEventArgs e)
        {
            AsClosableTabItem tabItem = tabScript.SelectedItem as AsClosableTabItem;
            SaveFile(tabItem);
        }

        private void saveFileAllClick(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TabItem tabItem in tabScript.Items)
            {
                SaveFile(tabItem as AsClosableTabItem);
            }
        }

        private void SaveFile(AsClosableTabItem tabItem)
        {
            var textEditor = tabItem.Content as ICSharpCode.AvalonEdit.TextEditor;
            string filename = textEditor.Document.FileName;
            if (String.IsNullOrEmpty(filename) == true)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".py";
                if (dlg.ShowDialog() ?? false)
                {
                    filename = dlg.FileName;
                    textEditor.Document.FileName = dlg.FileName;

                    string headerText = System.IO.Path.GetFileNameWithoutExtension(filename);
                    tabItem.SetHeader(headerText);
                }
                else
                {
                    return;
                }
            }

            textEditor.Save(filename);
            string message = "[" + DateTime.Now.ToString("tt HH:mm:ss") + "] Save succeeded(" + filename + ")" + Environment.NewLine;
            textboxOutput.AppendText(message);
            textboxOutput.ScrollToEnd();
        }

        private void closeAllClick(object sender, System.Windows.RoutedEventArgs e)
        {
            tabScript.Items.Clear();
        }

        private void clearClick(object sender, System.Windows.RoutedEventArgs e)
        {
            textboxOutput.Text = "";
        }

        private void restartClick(object sender, System.Windows.RoutedEventArgs e)
        {
            InitializeScriptEngine();
        }

        private void runClick(object sender, System.Windows.RoutedEventArgs e)
        {
            RunScript();
        }

        private void runSelectedClick(object sender, System.Windows.RoutedEventArgs e)
        {
            RunSelectedText();
        }        

        static private WM.Brush ToMediaSolidBrush(Color color)
        {
            return (new WM.SolidColorBrush(WM.Color.FromArgb(color.A, color.R, color.G, color.B)));
        }

        static private double CalculateColorDifference(Color lhs, Color rhs)
        {
            double hue = lhs.GetHue() - rhs.GetHue();
            double brightness = lhs.GetBrightness() - rhs.GetBrightness();
            double saturation = lhs.GetSaturation() - rhs.GetSaturation();

            return Math.Sqrt(hue * hue + brightness * brightness + saturation * saturation);
        }

        static private IHighlightingDefinition LoadHighlighting(string resourceName)
        {
            IHighlightingDefinition highlightingDefinition;
            using (System.IO.Stream s = typeof(AsScriptControl).Assembly.GetManifestResourceStream(resourceName))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not find embedded resource");
                }
                using (XmlReader reader = new XmlTextReader(s))
                {
                    highlightingDefinition = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            return highlightingDefinition;
        }
    }
}
