using System.Windows.Forms;
using KeyPadawan.View;
using KeyPadawan.View.KeyProcessors;
using KeyPadawan.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using KeyPadawan;
using KeyPadawan.Model;

namespace KeyPadawanTests
{
    [TestClass()]
    public class ScenariosTest
    {
        [TestMethod()]
        public void Test1()
        {
            string text = string.Empty;

            var interceptor = new FakeKeyboardInterceptor();
            var converter = new NaturalTextConverter(new DefaultProcessorsBuilder());
            var viewModel = new KeyLogModel(interceptor);
            viewModel.PropertyChanged += (o,e)=> text = (string)converter.Convert(viewModel.Buffer, null, null, null);

            interceptor.Press("Right");
            interceptor.Press("Left");
            interceptor.Press("Up");
            interceptor.Press("Down");

            interceptor.Press("Escape");
            interceptor.Press("Enter");
            interceptor.Press("PageUp");
            interceptor.Press("PageDown");
            interceptor.Press("Tab");
            interceptor.Press("Insert");
            interceptor.Press("Delete");
            interceptor.Press("LControlKey");
            interceptor.Press("LControlKey");

            Assert.AreEqual("⇨⇦⇧⇩⎋↵PgUpPgDnTabInsDel", text);
        }
    }

    class FakeKeyboardInterceptor : IKeyboardInterceptor
    {
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;
        public event EventHandler<KeyPressEventArgs> KeyPress;

        internal void Press(int p)
        {
            KeyDown(this, new KeyEventArgs((Keys)p));
        }

        internal void Press(string s)
        {
            KeyDown(this, new KeyEventArgs((Keys)(new KeysConverter().ConvertFromString(s)) ));
        }
    }
}
