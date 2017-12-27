using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SikuliSharp.TestApplication
{
	public partial class MainWindow
	{
		private DateTime _timerStart;
		private readonly DispatcherTimer _timer;
		private Action _timerTickAction;

		public MainWindow()
		{
			InitializeComponent();

			_timer = new DispatcherTimer();
			_timer.Tick += (o, e) => _timerTickAction();
		}

		private void TestButton_OnClick(object sender, RoutedEventArgs e)
		{
			TestButton.IsEnabled = false;

			StateLabel.Background = Brushes.SlateGray;
			StateLabel.Content = "5";

			_timerTickAction = () =>
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
			};

			_timerStart = DateTime.Now;
			_timer.Interval = TimeSpan.FromMilliseconds(50);
			_timer.Start();
		}

		private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.X)
			{
				TestButton.Foreground = Brushes.SlateGray;
				TestButton.Content = "5";

				_timerTickAction = () =>
				{
					var elapsed = DateTime.Now - _timerStart;

					if (elapsed.TotalSeconds >= 5)
					{
						_timer.Stop();
						Application.Current.Shutdown();
					}
					else
					{
						TestButton.Content = (5 - elapsed.TotalSeconds).ToString("0.00");
					}
				};

				_timerStart = DateTime.Now;
				_timer.Interval = TimeSpan.FromMilliseconds(50);
				_timer.Start();
			}
		}

		private void OffsetButton_OnClickButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (Equals(OffsetLabel.Background, Brushes.Black))
			{
				OffsetLabel.Background = Brushes.White;
				OffsetLabel.Foreground = Brushes.Black;
			}
			else
			{
				OffsetLabel.Background = Brushes.Black;
				OffsetLabel.Foreground = Brushes.White;
			}
		}
	}
}
