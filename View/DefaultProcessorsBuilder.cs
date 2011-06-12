using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyPadawan.View.KeyProcessors;


namespace KeyPadawan.View
{
    class DefaultProcessorsBuilder : IProcessorsBuilder
    {
        public IEnumerable<IEventProcessor> Build()
        {
            var processors = new List<IEventProcessor>();
            processors.Add(new PasswordProcessor());
            processors.Add(new SymbolicKeysTranslator());
            processors.Add(new ShortcutProcessor());
            processors.Add(new CharKeyProcessor());
            processors.Add(new RawProcessor());
            return processors;
        }
    }
}
