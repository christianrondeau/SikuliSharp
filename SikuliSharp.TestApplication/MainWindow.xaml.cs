using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SikuliSharp.TestApplication
{
	public partial class MainWindow
	{
		private DateTime _timerStart;
		private readonly DispatcherTimer _timer;

		public MainWindow()
		{
			InitializeComponent();

			_timer = new DispatcherTimer();
			_timer.Tick += _timer_Tick;
		}

		private void TestButton_OnClick(object sender, RoutedEventArgs e)
		{
			TestButton.IsEnabled = false;

			StateLabel.Background = Brushes.SlateGray;
			StateLabel.Content = "5";

			_timerStart = DateTime.Now;
			_timer.Interval = TimeSpan.FromMilliseconds(50);
			_timer.Start();
		}

		private void _timer_Tick(object sender, EventArgs eventArgs)
		{
			var elapsed = DateTime.Now - _timerStart;

			if (elapsed.TotalSeconds >= 5)
			{
				_timer.Stop();

				StateLabel.Background = Brushes.Green;
				StateLabel.Content = "Clicked!";
			}
			else
			{
				StateLabel.Content = (5 - elapsed.TotalSeconds).ToString("0.00");
			}
		}
	}
}
