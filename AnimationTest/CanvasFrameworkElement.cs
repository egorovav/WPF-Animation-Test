using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AnimationTest
{
	public class CanvasFrameworkElement : FrameworkElement
	{
		protected DrawingVisual FItemsVisual;
		//protected DrawingVisual FTracksVisual;

		private Dictionary<Color, Brush> FItemBrushes = new Dictionary<Color, Brush>();
		private Dictionary<Color, Brush> FTrackBrushes = new Dictionary<Color, Brush>();
		private Dictionary<Color, Pen> FTrackPens = new Dictionary<Color, Pen>();
		private List<Item> FItems = new List<Item>();
		//private List<PathGeometry> FItemsPaths = new List<PathGeometry>();

		public CanvasFrameworkElement() : base()
		{
			this.FItemsVisual = new DrawingVisual();
			//this.FTracksVisual = new DrawingVisual();

			base.AddVisualChild(this.FItemsVisual);
			//base.AddVisualChild(this.FTracksVisual);
		}

		public CanvasFrameworkElement(List<Item> aItems) : this()
		{
			this.FItems = aItems;
		}

		public void AddItem(Item aItem)
		{
			this.FItems.Add(aItem);
			var _path = new PathGeometry();
			var _figure = new PathFigure();
			_figure.IsClosed = false;
			_figure.StartPoint = aItem.Position;
			_path.Figures.Add(_figure);
			//this.FItemsPaths.Add(_path);
			addBrush(aItem.Color);
		}

		public void Clear()
		{
			this.FItems.Clear();
			//this.FItemsPaths.Clear();

			//this.FTracksContext.Close();

			base.RemoveVisualChild(this.FItemsVisual);
			//base.RemoveVisualChild(this.FTracksVisual);

			this.FItemsVisual = new DrawingVisual();
			//this.FTracksVisual = new DrawingVisual();

			base.AddVisualChild(this.FItemsVisual);
			//base.AddVisualChild(this.FTracksVisual);
		}

		private void addBrush(Color aColor)
		{
			var _itemBrush = new RadialGradientBrush(Colors.White, aColor);
			//_itemBrush.Center = new Point(0.2, 0.2);
			_itemBrush.GradientOrigin = new Point(0.3, 0.3);
			this.FItemBrushes[aColor] = _itemBrush;

			var _trackBrush = new SolidColorBrush(aColor);
			this.FTrackBrushes[aColor] = _trackBrush;
			this.FTrackPens[aColor] = new Pen(_trackBrush, 1);
		}

		public void DrawItem(Item aItem, DrawingContext aContext, double aZoom, Point aCenter, Point aDrawPoint)
		{
			var _brush = this.FItemBrushes[aItem.Color];
			var _r = aItem.Radius * aZoom;
			var _p = (aDrawPoint - aCenter) * aZoom + aCenter;
			aContext.DrawEllipse(_brush, null, _p, _r, _r);
		}

		public void DrawItem(Item aItem, DrawingContext aContext, double aZoom, Point aCenter)
		{
			DrawItem(aItem, aContext, aZoom, aCenter, aItem.DrawPoint);
		}


		private long FDrawingCount = 0;

		public void DrawItems(double aZoom, Point aCenter)
		{
			this.FDrawingCount++;
			Item _mainItem = this.FItems.Find(x => x.IsMainItem);

			using (var _context = this.FItemsVisual.RenderOpen())
			{

				for (int i = 0; i < this.FItems.Count; i++)
				{
					var _item = this.FItems[i];
					if (_mainItem == null)
					{
						_item.DrawPoint = _item.Position;
					}
					else
					{
						aCenter = _mainItem.StartPosition;
						if(_item.IsMainItem)
						{
							_item.DrawPoint = _item.StartPosition;
						}
						else
						{
							_item.DrawPoint = _item.Position - (_mainItem.Position - _mainItem.StartPosition);
						}
					}

					DrawItem(_item, _context, aZoom, aCenter);
				}
			}
		}

		public void DrawItems()
		{
			this.DrawItems(1, new Point(0, 0));
		}

		protected override int VisualChildrenCount
		{
			get
			{
				//return base.VisualChildrenCount;
				return 2;
			}
		}

		protected override Visual GetVisualChild(int index)
		{
			//return base.GetVisualChild(index);

			switch (index)
			{
				case 0: { return this.FItemsVisual; }
				//case 1: { return this.FTracksVisual; }
				default:
					return null;
			}
		}
    }
}
