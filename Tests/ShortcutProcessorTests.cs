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
            var converter = new NaturalTextConverter(new ProcessorsBuilder());
            var events = new[] {
                KeyboardEvent.FromChar('1'), KeyboardEvent.FromChar('a'),
                KeyboardEvent.FromKeys(Keys.Control | Keys.K), KeyboardEvent.FromKeys(Keys.Control | Keys.L),  
                KeyboardEvent.FromChar('p'), KeyboardEvent.FromChar('a'), KeyboardEvent.FromChar('s'), KeyboardEvent.FromChar('s'), KeyboardEvent.FromKeys(Keys.Enter),
                KeyboardEvent.FromKeys(Keys.Control | Keys.K), KeyboardEvent.FromKeys(Keys.Control | Keys.L),  
                KeyboardEvent.FromChar('3'), KeyboardEvent.FromChar('7'), KeyboardEvent.FromKeys(Keys.Escape), KeyboardEvent.FromKeys(Keys.Tab)
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("1aCtrl+KCtrl+L*****Ctrl+KCtrl+L37\u238bTab", actual);
        }

        [TestMethod]
        public void IgnoreCtrlandAlt()
        {
            var converter = new NaturalTextConverter(new FakeProcessorsBuilder(new [] { new ShortcutProcessor() } ) );
            var events = new[] {
                KeyboardEvent.FromKeys(Keys.Control | Keys.LControlKey),  
                KeyboardEvent.FromKeys(Keys.Alt ),  
                KeyboardEvent.FromKeys(Keys.Control | Keys.I)
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("Ctrl+I", actual);
        }

        [TestMethod]
        public void IgnoreCtrlandAlt2()
        {
            var converter = new NaturalTextConverter(new FakeProcessorsBuilder(new IEventProcessor[] { new ShortcutProcessor(), new RawProcessor() }));
            var events = new[] {
                KeyboardEvent.FromKeys(Keys.Control),  
                KeyboardEvent.FromKeys(Keys.Alt),  
                KeyboardEvent.FromKeys(Keys.Control | Keys.I)
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("CtrlAltCtrl+I", actual);
        }

        [TestMethod]
        public void IgnoreLMenu()
        {
            var converter = new NaturalTextConverter(new FakeProcessorsBuilder(new IEventProcessor[] { new ShortcutProcessor(), new RawProcessor() }));
            var events = new[] {
                KeyboardEvent.FromKeys(Keys.LMenu),  
                KeyboardEvent.FromKeys(Keys.LMenu),  
                KeyboardEvent.FromKeys(Keys.LMenu),  
                KeyboardEvent.FromKeys(Keys.LMenu)  
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("", actual);
        }

        [TestMethod]
        public void RepeatCtrl()
        {
            var converter = new NaturalTextConverter(new FakeProcessorsBuilder(new[] { new ShortcutProcessor() }));
            var events = new[] {
                KeyboardEvent.FromKeys(Keys.LControlKey),  
                KeyboardEvent.FromKeys(Keys.LControlKey),  
                KeyboardEvent.FromKeys(Keys.LControlKey),  
                KeyboardEvent.FromKeys(Keys.Control),  
                KeyboardEvent.FromKeys(Keys.LControlKey)  
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("", actual);
        }

        [TestMethod]
        public void IgnoreDownLeftRightUp()
        {
            var converter = new NaturalTextConverter(new FakeProcessorsBuilder(new[] { new ShortcutProcessor() }));
            var events = new[] {
                KeyboardEvent.FromKeys(Keys.Down),  
                KeyboardEvent.FromKeys(Keys.Up),  
                KeyboardEvent.FromKeys(Keys.Left),
                KeyboardEvent.FromKeys(Keys.Right),
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("", actual);
        }

        [TestMethod]
        public void IgnoreCtrlShiptOnly()
        {
            var converter = new NaturalTextConverter(new FakeProcessorsBuilder(new[] { new ShortcutProcessor() }));
            var events = new[] {
                KeyboardEvent.FromKeys(Keys.Control | Keys.LShiftKey ),  
            };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("", actual);
        }
    }
}
