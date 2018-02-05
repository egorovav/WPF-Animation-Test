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

		public PeriodicMotion(double aPeriod, double aAmplitude, Vector aDirection, double aPhase = 0)
		{
			this.FPeriod = aPeriod;
			this.FAmplitude = aAmplitude;
			this.FDirection = aDirection;
			this.FPhase = aPhase;
		}

		public override Vector GetVelocity()
		{
			return this.FDirection * this.FAmplitude * Math.Cos(this.FPhase + 2 * Math.PI * this.FTime / this.FPeriod) * 2 * Math.PI / this.FPeriod;
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
