using KeyPadawan.View;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using KeyPadawan.ViewModel;
using System.Collections.Generic;
using System.Windows.Forms;
using KeyPadawan.View.KeyProcessors;

namespace KeyPadawanTests
{
    [TestClass()]
    public class SymbolicKeysTranslatorTest
    {
        [TestMethod()]
        public void SymbolicKeysTranslatorConstructorTest()
        {
            var builder = new FakeProcessorsBuilder(new[] { new SymbolicKeysTranslator() });
            var converter = new NaturalTextConverter(builder);
            var events = new [] { KeyboardEvent.FromKeys(Keys.Enter), KeyboardEvent.FromKeys(Keys.Escape) };

            var actual = converter.Convert(events, null, null, null);

            Assert.AreEqual("\u21b5\u238b", actual);
        }
    }

    class FakeProcessorsBuilder : IProcessorsBuilder
    {
        private IEnumerable<IEventProcessor> _processors;

        public FakeProcessorsBuilder(IEnumerable<IEventProcessor> processors)
        {
            _processors = processors;
        }

        public System.Collections.Generic.IEnumerable<IEventProcessor> Build()
        {
            return _processors;
        }


        public void Set(ShortcutProcessor[] shortcutProcessor)
        {
            throw new NotImplementedException();
        }
    }
}
