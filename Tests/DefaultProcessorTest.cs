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
                KeyboardEvent.FromChar('1'), KeyboardEvent.FromChar('a'),
                KeyboardEvent.FromKeys(Keys.Control | Keys.K), KeyboardEvent.FromKeys(Keys.Control | Keys.L),  
                KeyboardEvent.FromChar('p'), KeyboardEvent.FromChar('a'), KeyboardEvent.FromChar('s'), KeyboardEvent.FromChar('s'),
                KeyboardEvent.FromKeys(Keys.Control | Keys.K), KeyboardEvent.FromKeys(Keys.Control | Keys.L),  
                KeyboardEvent.FromChar('3'), KeyboardEvent.FromChar('7'),
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("1apass37", actual);
        }
    }
}
