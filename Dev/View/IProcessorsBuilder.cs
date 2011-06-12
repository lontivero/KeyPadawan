using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyPadawan.View
{
    interface IProcessorsBuilder
    {
        IEnumerable<IEventProcessor> Build();
    }
}
