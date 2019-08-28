using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;

namespace AnimationTest
{
	public interface IMotion
	{
		Point GetPosition(Item aPoint);

		Point GetPosition(Item aPoint, double aTime);

		void Reset();
	}

	public abstract class MotionBase : IMotion
	{
		protected double FTime = 0;

		public Point GetPosition(Item aItem)
		{
			return GetPosition(aItem, 1);
		}

		public virtual Point GetPosition(Item aItem, double aTime)
		{
			var _s = this.GetShift(aItem, aTime);
			return aItem.Position + _s;
		}

		protected Vector GetShift(Item aItem)
		{
			return GetShift(aItem, 1);
		}

		public abstract Vector GetShift(Item aItem, double aTime);

		public abstract void Reset();
	}

	public abstract class DetermineMotion : MotionBase
	{
		protected int FRight = 1200;
		protected int FLeft = 10;
		protected int FTop = 10;
		protected int FBottom = 800;

		protected bool IsWallUsed = true;

		public abstract Vector GetVelocity();
		protected abstract void ReverseX();
		protected abstract void ReverseY();

		public override Vector GetShift(Item aItem, double aTime)
		{
			this.FTime += aTime;
			var _v = GetVelocity();
			var _s = _v * aTime;

			if (IsWallUsed)
            {
                _s = WallProcessing(aItem.Position, _s);
            }

            return _s;
		}

        protected Vector WallProcessing(Point aPoint, Vector _s)
        {
            if (aPoint.X + _s.X < this.FLeft)
            {
                _s = new Vector(this.FLeft - aPoint.X, _s.Y);
                this.ReverseX();
            }

            if (aPoint.X + _s.X > this.FRight)
            {
                _s = new Vector(this.FRight - aPoint.X, _s.Y);
                this.ReverseX();
            }

            if (aPoint.Y + _s.Y > this.FBottom)
            {
                _s = new Vector(_s.X, this.FBottom - aPoint.Y);
                this.ReverseY();
            }

            if (aPoint.Y + _s.Y < this.FTop)
            {
                _s = new Vector(_s.X, this.FTop - aPoint.Y);
                this.ReverseY();
            }

            return _s;
        }

        public override void Reset()
		{
			this.FTime = 0;
		}
	}

	public class StraightMotion : DetermineMotion
	{
		protected Vector FVelocity;
		protected Vector FStartVelocity;

		public StraightMotion(Vector aVelocity)
		{
			this.FVelocity = aVelocity;
			this.FStartVelocity = aVelocity;
		}

        public static Vector GetRandomVelocity(double aMinVelocity, double aMaxVelocity, Random aRandom)
        {
            var _velocityRate = aRandom.NextDouble();
            var _velocityValue = aMinVelocity * (1 - _velocityRate) + _velocityRate * aMaxVelocity;

            var _velocityDirection = aRandom.NextDouble() * 2 - 1;

            var _vx = Math.Sqrt(_velocityValue * _velocityValue / (1 + _velocityDirection * _velocityDirection));
            var _reverse = aRandom.NextDouble() - 0.5;
            if (_reverse < 0)
                _vx = -_vx;

            var _vy = _velocityDirection * _vx;

            var _swap = aRandom.NextDouble() - 0.5;
            if (_swap < 0)
                return new Vector(_vx, _vy);
            else
                return new Vector(_vy, _vx);
        }

		public StraightMotion(double aMinVelocity, double aMaxVelocity, Random aRandom)
		{
            this.FVelocity = StraightMotion.GetRandomVelocity(aMinVelocity, aMaxVelocity, aRandom);
			this.FStartVelocity = this.FVelocity;
		}

		public override Vector GetVelocity()
		{
			return this.FVelocity;
		}

		protected override void ReverseX()
		{
			this.FVelocity = new Vector(-this.FVelocity.X, this.FVelocity.Y);
		}

		protected override void ReverseY()
		{
			this.FVelocity = new Vector(this.FVelocity.X, -this.FVelocity.Y);
		}

		public override void Reset()
		{
			base.Reset();
			this.FVelocity = this.FStartVelocity;
		}
	}

	public class AcceleratedMotion : DetermineMotion
	{
		protected Vector FAcceleration;
        protected Vector FStartVelocity;
		protected Vector FVelocity = new Vector(0, 0);

		public AcceleratedMotion(Vector aAcceleration)
		{
			this.FAcceleration = aAcceleration;
		}

        public AcceleratedMotion(Vector aAcceleration, Vector aVelocity)
            : this(aAcceleration)
        {
            this.FVelocity = aVelocity;
            this.FStartVelocity = aVelocity;
        }

		public override Vector GetVelocity()
		{
			return this.FVelocity + this.FTime * this.FAcceleration;
		}

		protected override void ReverseX()
		{
            var _v = GetVelocity();
            //var _v = this.FVelocity;
			this.FVelocity = new Vector(-_v.X, _v.Y);
			this.FTime = 0;
		}

		protected override void ReverseY()
		{
            var _v = GetVelocity();
            //var _v = this.FVelocity;
			this.FVelocity = new Vector(_v.X, -_v.Y);
			this.FTime = 0;
		}

		public override void Reset()
		{
			base.Reset();
            //this.FVelocity = new Vector(0, 0);
            this.FVelocity = this.FStartVelocity;
		}
	}

	public class MotionGroup : MotionBase
	{
		protected List<MotionBase> FMotions = new List<MotionBase>();


		public void AddMotion(MotionBase aMotion)
		{
			this.FMotions.Add(aMotion);
		}

		public override Vector GetShift(Item aItem, double aTime)
		{
			var _ts = new Vector(0, 0);
			foreach (var _motion in this.FMotions)
			{				
				_ts += _motion.GetShift(aItem, aTime);
			}
			return _ts;
		}

		public override void Reset()
		{
			foreach (var _motion in this.FMotions)
				_motion.Reset();
		}

        public double AvgVelocity
        {
            get
            {
                    return this.FMotions
                        .Where(x => x is DetermineMotion)
                        .Average(x => ((DetermineMotion)x).GetVelocity().Length);
            }
        }
	}
}
