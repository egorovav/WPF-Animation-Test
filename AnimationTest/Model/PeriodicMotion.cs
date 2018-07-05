using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnimationTest
{
	public class PeriodicMotion : DetermineMotion
	{
		private double FPeriod;
		private double FAmplitude;
		private Vector FDirection;
		private double FPhase;
		private double FFadding;

		public PeriodicMotion(double aPeriod, double aAmplitude, Vector aDirection, double aPhase = 0, double aFadding = 1)
		{
			this.FPeriod = aPeriod;
			this.FAmplitude = aAmplitude;
			this.FDirection = aDirection;
			this.FPhase = aPhase;
			this.FFadding = aFadding;
		}

		public override Vector GetVelocity()
		{
            var _amplitude = this.FAmplitude;
            //if (this.FIsFadding)
            //    _amplitude = Math.Max(this.FAmplitude - 0.1 * this.FTime, 0);

            if (this.FFadding != 1)
            {
                this.FAmplitude = this.FAmplitude * this.FFadding;
                this.FPeriod = this.FPeriod * this.FFadding;
            }
            var _velocity = 
				this.FDirection * _amplitude * Math.Cos(this.FPhase + 2 * Math.PI * this.FTime / this.FPeriod) * 2 * Math.PI / this.FPeriod;
			//if (this.FIsFadding)
			//	_velocity *= Math.Pow(0.999, this.FTime);
			return _velocity;
		}

		protected override void ReverseX()
		{
			this.FDirection = new Vector(-this.FDirection.X, this.FDirection.Y);
		}

		protected override void ReverseY()
		{
			this.FDirection = new Vector(this.FDirection.X, -this.FDirection.Y);
		}
	}
}
