using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

namespace KeyPadawan.Windows.Controls
{
    public class AutoHideWindow : TransparentWindow
    {
        public static readonly DependencyProperty IsHidingProperty  =
            DependencyProperty.RegisterAttached("IsHiding", typeof(bool),
            typeof(DependencyObject), new PropertyMetadata(false));

        private DispatcherTimer _timer;
        private int _ticks;

        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);
            _timer = new DispatcherTimer(
                        interval: TimeSpan.FromSeconds(1),
                        priority: DispatcherPriority.Normal,
                        callback: new EventHandler(OnTick),
                        dispatcher: Dispatcher);
            _ticks = 4000;
            _timer.Start();
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            IsHiding = false;
        }

        private void OnTick(object sender, EventArgs args)
        {
            _ticks--;
            if (_ticks == 0)
                IsHiding = true;
        }

        public bool IsHiding
        {
            get { return (bool)GetValue(IsHidingProperty); }
            set 
            {
                if (value == false)
                {
                    _ticks = 4000;
                }
                SetValue(IsHidingProperty, value);      
            }
        }
    }
}
