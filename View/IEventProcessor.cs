namespace KeyPadawan.View
{
    using KeyPadawan.ViewModel;

    interface IEventProcessor
    {
        bool TryProcessEvent(Event evnt, out string result);
    }
}
