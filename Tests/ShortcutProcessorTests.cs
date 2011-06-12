using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeyPadawan.View;
using KeyPadawan.ViewModel;
using System.Windows.Forms;
using KeyPadawan.View.KeyProcessors;

namespace KeyPadawanTests
{
    [TestClass]
    public class ShortcutProcessorTests
    {
        [TestMethod]
        public void AllInOne()
        {
            var converter = new NaturalTextConverter(new DefaultProcessorsBuilder());
            var events = new[] {
                Event.FromChar('1'), Event.FromChar('a'),
                Event.FromKeys(Keys.Control | Keys.K), Event.FromKeys(Keys.Control | Keys.L),  
                Event.FromChar('p'), Event.FromChar('a'), Event.FromChar('s'), Event.FromChar('s'), Event.FromKeys(Keys.Enter),
                Event.FromKeys(Keys.Control | Keys.K), Event.FromKeys(Keys.Control | Keys.L),  
                Event.FromChar('3'), Event.FromChar('7'), Event.FromKeys(Keys.Escape), Event.FromKeys(Keys.Tab)
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("1aCtrl+KCtrl+L*****Ctrl+KCtrl+L37\u238bTab", actual);
        }

        [TestMethod]
        public void IgnoreCtrlandAlt()
        {
            var converter = new NaturalTextConverter(new FakeProcessorsBuilder(new [] { new ShortcutProcessor() } ) );
            var events = new[] {
                Event.FromKeys(Keys.Control),  
                Event.FromKeys(Keys.Alt),  
                Event.FromKeys(Keys.Control | Keys.I)
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("Ctrl+I", actual);
        }

        [TestMethod]
        public void RepeatCtrl()
        {
            var converter = new NaturalTextConverter(new FakeProcessorsBuilder(new[] { new ShortcutProcessor() }));
            var events = new[] {
                Event.FromKeys(Keys.LControlKey),  
                Event.FromKeys(Keys.LControlKey),  
                Event.FromKeys(Keys.LControlKey),  
                Event.FromKeys(Keys.Control),  
                Event.FromKeys(Keys.LControlKey)  
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("", actual);
        }
    }
}
