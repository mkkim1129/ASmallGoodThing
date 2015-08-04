
namespace mkkim1129.ASmallGoodThing
{
    class AsTextBoxStreamWriter : System.IO.TextWriter
    {
        private delegate void UpdateTextCallback(char value);

        private ICSharpCode.AvalonEdit.TextEditor textBox_;

        public AsTextBoxStreamWriter(ICSharpCode.AvalonEdit.TextEditor textBox)
        {
            textBox_ = textBox;
        }

        public override void Write(char value)
        {
            base.Write(value);
            if (value == '\r')
            {
                return;
            }
            textBox_.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), value);
        }

        private void UpdateText(char value)
        {
            textBox_.AppendText(value.ToString());

            if (value == '\n')
            {
                textBox_.ScrollToEnd();
            }
        }

        public override System.Text.Encoding Encoding
        {
            get
            {
                return System.Text.Encoding.UTF8;
            }
        }
    }
}
