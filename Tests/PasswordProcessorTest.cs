using System.Windows.Forms;
using KeyPadawan.View;
using KeyPadawan.View.KeyProcessors;
using KeyPadawan.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeyPadawanTests
{
    [TestClass()]
    public class PasswordProcessorTest
    {
        [TestMethod()]
        public void HidePasswordTest()
        {
            var builder = new FakeProcessorsBuilder(new[] { new PasswordProcessor() });
            var converter = new NaturalTextConverter(builder);
            var events = new[] {
                KeyboardEvent.FromChar('1'), KeyboardEvent.FromChar('a'),
                KeyboardEvent.FromKeys(Keys.Control | Keys.K), KeyboardEvent.FromKeys(Keys.Control | Keys.L),  // password mode shortcut (E)
                KeyboardEvent.FromChar('p'), KeyboardEvent.FromChar('a'), KeyboardEvent.FromChar('s'), KeyboardEvent.FromChar('s'),
                KeyboardEvent.FromKeys(Keys.Control | Keys.K), KeyboardEvent.FromKeys(Keys.Control | Keys.L),  // password mode shortcut (D)
                KeyboardEvent.FromChar('3'), KeyboardEvent.FromChar('7'),
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("****", actual);
        }
    }
}
