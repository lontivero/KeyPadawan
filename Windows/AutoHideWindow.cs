using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace KeyPadawan.Windows.Controls
{
    public class AutoHideWindow : TransparentWindow
    {
        private Storyboard _fadeOutEffect;

        private DispatcherTimer _timer;
        private int _ticks;

        public EventHandler<EventArgs> AfterFadeOut;

        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);
            _fadeOutEffect = BuildFadeOutStoryBoard();

            _timer = new DispatcherTimer(
                        interval: TimeSpan.FromMilliseconds(250),
                        priority: DispatcherPriority.Normal,
                        callback: new EventHandler(OnTick),
                        dispatcher: Dispatcher);
            _ticks = 6;
            _timer.Start();
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            StopFadingOut();
        }

        public void StopFadingOut()
        {
           _fadeOutEffect.Stop(this);
           _ticks = 6;
        }

        private void StartFadingOut()
        {
            if (!IsMouseOver)
            {
                _fadeOutEffect.Begin(this, true);
            }
        }

        private Storyboard BuildFadeOutStoryBoard()
        {
            var duration = TimeSpan.FromMilliseconds(750);
            var fadeOutAnimation = new DoubleAnimation(0.0, duration);

            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(Window.OpacityProperty));

            var winFadeStoryBoard = new Storyboard();
            winFadeStoryBoard.Children.Add(fadeOutAnimation);
            winFadeStoryBoard.Completed += new EventHandler(winFadeStoryBoard_Completed);
            return winFadeStoryBoard;
        }

        private void winFadeStoryBoard_Completed(object sender, EventArgs e)
        {
            var afterFadeOutEventHandler = AfterFadeOut;
            if (afterFadeOutEventHandler != null)
            {
                afterFadeOutEventHandler(this, EventArgs.Empty);
            }
        }

        private void OnTick(object sender, EventArgs args)
        {
            if (_ticks-- == 0)
            {
                StartFadingOut();
            }
        }
    }
}
