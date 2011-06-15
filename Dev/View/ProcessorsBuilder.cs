using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyPadawan.View.KeyProcessors;
using KeyPadawan.Properties;


namespace KeyPadawan.View
{
    class ProcessorsBuilder : IProcessorsBuilder
    {
        public IEnumerable<IEventProcessor> Build()
        {
            var settings = Settings.Default;
            if (settings.ShortcutsMode) return new[] { new ShortcutProcessor() };
            if (settings.RawMode) return new[] { new RawProcessor() };
            if (settings.NormalMode) 
            return new IEventProcessor[] {
                new PasswordProcessor(),
                new SymbolicKeysTranslator(),
                new ShortcutProcessor(),
                new CharKeyProcessor(),
                new RawProcessor() };

            return Enumerable.Empty<IEventProcessor>();
        }
    }
}
