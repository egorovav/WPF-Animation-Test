using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnimationTest
{
	public class GravityMotion : MotionBase
	{
		private List<Item> FItems = new List<Item>();
		private Vector FVelocity;
		private Vector FStartVelocity;

		public GravityMotion(Vector aVelocity, List<Item> aItems)
		{
			this.FVelocity = aVelocity;
			this.FItems = aItems;
			this.FStartVelocity = aVelocity;
		}

		public double Time
		{
			get { return this.FTime; }
		}

		public override Vector GetShift(Item aItem, double aTime)
		{
			var _a = new Vector(0, 0);
			
			foreach(var _item in this.FItems)
			{
				if (_item.Mass == 0)
					continue;

				var _position = _item.Position;
				var _motion = (GravityMotion)_item.Motion;
				if (_motion.Time > this.FTime)
					_position = _item.PrevPosition;
				var _r = _position - aItem.Position;
				var _l = _r.Length;
				if (_l == 0)
					continue;

				_a += _r * _item.Mass / (_l * _l * _l);
			}

			var _v = this.FVelocity + _a * aTime;
			var _s = _v * aTime;

			this.FVelocity = _v;
			this.FTime += aTime;

			return _s;
		}

		public override void Reset()
		{
			this.FVelocity = this.FStartVelocity;
		}

		public Vector StartVelocity
		{
			get { return this.FStartVelocity; }
		}

        public Vector Velocity
        {
            get { return this.FVelocity; }
        }
	}
}
