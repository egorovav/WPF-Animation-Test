using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AnimationTest
{
	public class Item
	{
		private Color FColor;
		private Point FPosition;
		private Point FPrevPosition = new Point(-1, -1);
		private IMotion FMotion;
		private List<Point> FTrack = new List<Point>();
		private double FRadius = 10;
		private double FMass;
		private Point FStartPosition;
		//private Point FLastTrackPoint = new Point(-1, -1);

		public Item(Color aColor, Point aPosition, IMotion aMotion)
		{
			this.FColor = aColor;
			this.FPosition = aPosition;
			this.FMotion = aMotion;
			this.FStartPosition = aPosition;
		}

		public Item(Color aColor, Point aPoint, IMotion aMotion, double aMass, double aRadius = 10)
			:this(aColor, aPoint, aMotion)
		{
			this.FMass = aMass;
			this.FRadius = aRadius;
		}

		public Point Position
		{
			get
			{
				return this.FPosition;
			}
		}

		public Color Color
		{
			get { return this.FColor; }
		}

		public List<Point> Track
		{
			get { return this.FTrack; }
		}

		public double Mass
		{
			get { return this.FMass; }
		}

		public Point PrevPosition
		{
			get { return this.FPrevPosition; }
		}

		public double Radius
		{
			get { return this.FRadius; }
		}

		public IMotion Motion
		{
			get { return this.FMotion; }
		}

		public Point UpdatePosition(double aTime)
		{
			//if (Math.Truncate(this.Position.X) != Math.Truncate(this.PrevPosition.X) ||
			//	Math.Truncate(this.Position.Y) != Math.Truncate(this.PrevPosition.Y))
			{
				this.FTrack.Add(this.Position);
			}

			this.FPrevPosition = this.FPosition;
			this.FPosition = this.FMotion.GetPosition(this.FPosition, aTime);

			return this.Position;
		}

		public Point UpdatePosition()
		{
			this.FPosition = this.UpdatePosition(1);
			return this.Position;
		}

		public void ClearTrack()
		{
			this.FTrack = null;
			this.FTrack = new List<Point>();
		}

		public void Reset()
		{
			this.FPosition = this.FStartPosition;
			this.FMotion.Reset();
			this.FTrack.Clear();
		}
	}
}
