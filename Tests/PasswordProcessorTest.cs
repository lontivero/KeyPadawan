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
                Event.FromChar('1'), Event.FromChar('a'),
                Event.FromKeys(Keys.Control | Keys.K), Event.FromKeys(Keys.Control | Keys.L),  // password mode shortcut (E)
                Event.FromChar('p'), Event.FromChar('a'), Event.FromChar('s'), Event.FromChar('s'),
                Event.FromKeys(Keys.Control | Keys.K), Event.FromKeys(Keys.Control | Keys.L),  // password mode shortcut (D)
                Event.FromChar('3'), Event.FromChar('7'),
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("****", actual);
        }
    }
}
