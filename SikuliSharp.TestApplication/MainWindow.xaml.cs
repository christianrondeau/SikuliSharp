using System.Windows;
using System.Windows.Media;

namespace SikuliSharp.TestApplication
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void TestButton_OnClick(object sender, RoutedEventArgs e)
		{
			StateLabel.Background = Brushes.Green;
			StateLabel.Content = "Clicked!";
		}
	}
}
