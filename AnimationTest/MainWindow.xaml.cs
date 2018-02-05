using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AnimationTest
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.MainViewModel.PropertyChanged += MainViewModel_PropertyChanged;

			//this.CanvasElement.Items = this.MainViewModel.Items;	

			foreach(var _list in this.MainViewModel.ItemsLists)
			{
				var _menuItem = new MenuItem();
				_menuItem.Header = _list.Key;
				_menuItem.Click += _menuItem_Click;

				if (_list.Key.StartsWith("Group:"))
					this.mGroup.Items.Add(_menuItem);
				else if (_list.Key.StartsWith("Gravity:"))
					this.mGravity.Items.Add(_menuItem);
				else if (_list.Key.StartsWith("Random:"))
					this.mRandom.Items.Add(_menuItem);
				else
					this.mMotion.Items.Add(_menuItem);
			}

			this.FTracksImage = new WriteableBitmap(
				(int)iCanvas.Width, (int)iCanvas.Height, 96, 96, PixelFormats.Bgra32, null);
			this.FStright = this.FTracksImage.PixelWidth * this.FTracksImage.Format.BitsPerPixel / 8;
			this.iCanvas.Source = this.FTracksImage;

			//this.iCanvas.RenderTransform = this.FTransform;
			this.CanvasElement.RenderTransform = this.FTransform;
		}

		WriteableBitmap FTracksImage;
		int FStright;
		TransformGroup FTransform = new TransformGroup();

		private void _menuItem_Click(object sender, RoutedEventArgs e)
		{
			this.MainViewModel.ItemsName = (string)((MenuItem)sender).Header;
		}

		private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == MainViewModel.CanvasPropertyName)
			{
				this.CanvasElement.Dispatcher.Invoke(() =>
				{
					lock (MainViewModel.CanvasLocker)
					{
						this.CanvasElement.DrawItems(1, new Point(0, 0));
						this.MainViewModel.CanvasIsDrew = true;
					}
				});

				this.Dispatcher.Invoke(() =>
				{
					if (this.MainViewModel.DrawTracks)
					{
						foreach (var _item in this.MainViewModel.Items)
						{
							DrawTrackPoint(this.FTransform.Transform(_item.Position), _item.Color, 1, new Point(0, 0));
						}
					}
				});
			}

			if(e.PropertyName == MainViewModel.ErrorStringPropertyName)
			{
				this.Dispatcher.Invoke(() =>
				{
					MessageBox.Show(this.MainViewModel.ErrorString);
				});				
			}

			if(e.PropertyName == MainViewModel.ItemsNamePropertyName)
			{
				if (this.MainViewModel.Items == null)
					return;

				this.CanvasElement.Clear();
				foreach (var _item in this.MainViewModel.Items)
				{
					_item.Reset();
					this.CanvasElement.AddItem(_item);
				}

				this.CanvasElement.DrawItems(1, new Point(0, 0));
				this.ClearTracks();
			}

			if(e.PropertyName == MainViewModel.DrawTracksPropertyName)
			{
				if (this.MainViewModel.DrawTracks)
					this.DrawTracks();
				else
					this.ClearTracks();
			}
		}

		private void ClearTracks()
		{
			var _pixels = new byte[this.FTracksImage.PixelHeight * this.FStright];
			var _rect = new Int32Rect(0, 0, this.FTracksImage.PixelWidth, this.FTracksImage.PixelHeight);
			this.FTracksImage.WritePixels(_rect, _pixels, this.FStright, 0);
		}

		private void DrawTracks()
		{
			var _items = this.MainViewModel.Items;
			if (!this.MainViewModel.DrawTracks || _items == null)
				return;

			var _w = this.FTracksImage.PixelWidth;
			var _h = this.FTracksImage.PixelHeight;
			var _bpp = this.FTracksImage.Format.BitsPerPixel / 8;
			var _rect = new Int32Rect(0, 0, _w, _h);
			var _pixels = new byte[_w * _h * _bpp];
			var _m = this.FTransform.Value;

			lock (MainViewModel.CanvasLocker)
			{
				foreach (var _item in _items)
				{
					foreach (var _p in _item.Track)
					{
						var _x = (int)(_p.X * _m.M11 + _m.OffsetX);
						if (_x < 0 || _x >= this.FTracksImage.PixelWidth)
							continue;

						var _y = (int)(_p.Y * _m.M11 + _m.OffsetY);
						if (_y < 0 || _y >= this.FTracksImage.PixelHeight)
							continue;

						var _index = (_y * this.FTracksImage.PixelWidth + _x) * _bpp;
						_pixels[_index] = _item.Color.B;
						_pixels[_index + 1] = _item.Color.G;
						_pixels[_index + 2] = _item.Color.R;
						_pixels[_index + 3] = _item.Color.A;
					}
				}
			}

			this.FTracksImage.WritePixels(_rect, _pixels, this.FStright, 0);
		}

		private void DrawTrackPoint(Point aPosition, Color aColor, double aZoom, Point aCenter)
		{
			var _p = aPosition;

			var _x = (int)_p.X;
			if (_x < 0 || _x >= this.FTracksImage.PixelWidth)
				return;

			var _y = (int)_p.Y;
			if (_y < 0 || _y >= this.FTracksImage.PixelHeight)
				return;

			var _rect = new Int32Rect(_x, _y, 1, 1);
			var _pixels = new byte[] { aColor.B, aColor.G, aColor.R, aColor.A };
			this.FTracksImage.WritePixels(_rect, _pixels, this.FStright, 0);
		}

		protected MainViewModel MainViewModel
		{
			get { return (MainViewModel)gMain.DataContext; }
		}

		private void iCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			var _p = e.GetPosition(this.iCanvas);

			var _rate = 1.1;
			if (e.Delta < 0)
				_rate = 0.9;
			this.FTransform.Children.Add(new ScaleTransform(_rate, _rate, _p.X, _p.Y));

			this.ClearTracks();
			this.DrawTracks();
		}

		Point FDragStart;
		private void iCanvas_MouseDown(object sender, MouseEventArgs e)
		{
			this.FDragStart = e.GetPosition(this.iCanvas);
		}

		private void iCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && this.FDragStart.X != 0)
			{
				var _dragEnd = e.GetPosition(this.iCanvas);
				var _shift = _dragEnd - this.FDragStart;
				this.FDragStart = _dragEnd;

				this.FTransform.Children.Add(new TranslateTransform(_shift.X, _shift.Y));
				this.ClearTracks();
				this.DrawTracks();
			}
			else
				this.FDragStart = new Point(0, 0);
		}

		private void btnAccelerate_Click(object sender, RoutedEventArgs e)
		{
			this.MainViewModel.Delta *= 1.1;
		}

		private void btnDeaccelerate_Click(object sender, RoutedEventArgs e)
		{
			this.MainViewModel.Delta /= 1.1;
		}

		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
			foreach(var _item in this.MainViewModel.Items)
			{
				_item.ClearTrack();
				this.ClearTracks();
			}
		}
	}
}
