using Microsoft.Win32;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace gtfup
{
	public partial class MainWindow : Window
    {
		private const string appname = "Gaudy Timer For Uninterruptible People";
		private const int defaultTimer = 45;
		private const string helpmsg = "Use the buttons to control the timer\r\nClick the tray icon to hide/restore\r\nClick X to quit\r\n\r\nBrought to you by mznlab.net";
		private Dispatcher dispatcher;
		private Timer timer;
        private System.Windows.Forms.NotifyIcon trayIcon;

        public MainWindow()
        {
			dispatcher = Dispatcher.CurrentDispatcher;
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            InitializeComponent();
            trayIcon = new System.Windows.Forms.NotifyIcon();
            trayIcon.Icon = (System.Drawing.Icon)Properties.Resources.ResourceManager.GetObject("mznlab");
            trayIcon.BalloonTipText = minLeft.ToString();
            trayIcon.Visible = true;
            trayIcon.Click += trayIcon_Click;
            timer = new Timer();
            runTimer(timer, defaultTimer);
        }

        public int minLeft
        {
            get;
            set;
        }

		private void dispatch(object source, ElapsedEventArgs e)
		{
			Action dispatchAction = () => updateLabel();
			dispatcher.BeginInvoke(dispatchAction);
		}

		private void helpButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(helpmsg, appname, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void minusFiveButton_Click(object sender, RoutedEventArgs e)
		{
			if (minLeft > 5) { minLeft -= 5; }
			timerLbl.Content = minLeft;
			trayIcon.Text = minLeft.ToString();
		}

		private void resetButton_Click(object sender, RoutedEventArgs e)
		{
			timer.Stop();
			timer = new Timer();
			runTimer(timer, defaultTimer);
		}

		private void runTimer(Timer timer, int duration)
		{
			minLeft = duration;
			timerLbl.Content = minLeft;
			trayIcon.Text = minLeft.ToString();
			timer.Elapsed += new ElapsedEventHandler(dispatch);
			timer.AutoReset = true;
			timer.Interval = 60000;
			timer.Start();
		}

		private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                timer = new Timer();
                runTimer(timer, defaultTimer);
            }

            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                timer.Stop();
                timer = null;
            }
        }

		private void trayIcon_Click(object sender, EventArgs e)
		{
			if (!IsVisible) { Show(); }
			else { Hide(); }
		}

		private void updateLabel()
		{
            minLeft--;
            timerLbl.Content = minLeft;
            trayIcon.Text = minLeft.ToString();
            if (minLeft == 0)
            {
                bool hide = false;
                timer.Stop();
                timer = new Timer();
                if (!IsVisible) { Show(); hide = true; }
                if (MessageBoxResult.Cancel == MessageBox.Show(this,
					"GTFUP!\r\n\r\nOK to restart, Cancel to snooze for 10 minutes.",
					"GO!",
					MessageBoxButton.OKCancel,
					MessageBoxImage.Exclamation,
					MessageBoxResult.OK))
                { runTimer(timer, 10); }
                else
                { runTimer(timer, defaultTimer); }
                if (hide) { Hide(); }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            trayIcon.Dispose();
        }
	}
}
