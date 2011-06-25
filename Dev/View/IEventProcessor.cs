namespace KeyPadawan.View
{
    using KeyPadawan.ViewModel;

    interface IEventProcessor
    {
        bool TryProcessEvent(KeyboardEvent evnt, out string result);
    }
}
