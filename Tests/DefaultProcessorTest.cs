using System.Windows.Forms;
using KeyPadawan.View;
using KeyPadawan.View.KeyProcessors;
using KeyPadawan.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeyPadawanTests
{
    [TestClass()]
    public class DefaultProcessorTest
    {
        [TestMethod()]
        public void ShowAllevenPasswordTest()
        {
            var builder = new FakeProcessorsBuilder(new[] { new CharKeyProcessor() });
            var converter = new NaturalTextConverter(builder);
            var events = new[] {
                Event.FromChar('1'), Event.FromChar('a'),
                Event.FromKeys(Keys.Control | Keys.K), Event.FromKeys(Keys.Control | Keys.L),  
                Event.FromChar('p'), Event.FromChar('a'), Event.FromChar('s'), Event.FromChar('s'),
                Event.FromKeys(Keys.Control | Keys.K), Event.FromKeys(Keys.Control | Keys.L),  
                Event.FromChar('3'), Event.FromChar('7'),
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("1apass37", actual);
        }
    }
}
