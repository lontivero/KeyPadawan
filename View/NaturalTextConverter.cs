namespace KeyPadawan.View
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Data;
    using KeyPadawan.ViewModel;

    class NaturalTextConverter : IValueConverter
    {
        private readonly IProcessorsBuilder _processorsBuilder;

        public NaturalTextConverter()
            : this(new DefaultProcessorsBuilder())
        {
        }

        public NaturalTextConverter(IProcessorsBuilder builder)
        {
            _processorsBuilder = builder;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var _chainOfProcessors = _processorsBuilder.Build();

            var textToDisplay = string.Empty;
            var buffer = (IEnumerable<Event>)value;

            foreach (var evnt in buffer)
            {
                var processors = _chainOfProcessors.GetEnumerator();
                while (processors.MoveNext())
                {
                    string result = string.Empty;
                    var currentProcessor = processors.Current;
                    var done = currentProcessor.TryProcessEvent(evnt, out result);

                    if (result!=null) textToDisplay += result;
                    if (done) break;
                }
            }
            return textToDisplay;
        }

        private static string RemoveLatestChar(string buffer)
        {
            if (string.IsNullOrEmpty(buffer))
                return string.Empty;

            return buffer.Remove(buffer.Length - 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
