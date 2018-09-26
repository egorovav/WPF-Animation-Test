using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnimationTest
{
    public class CollisionMotion : AcceleratedMotion // StraightMotion // MotionBase
    {
        private List<Item> FItems = new List<Item>();

        public CollisionMotion(Vector aVelocity, List<Item> aItems)
            : base(new Vector(0, 0), aVelocity)
        {
            this.FItems = aItems;
        }

        public CollisionMotion(Vector aVelocity, Vector aAcceleration, List<Item> aItems)
            : base(aAcceleration, aVelocity)
        {
            this.FItems = aItems;
        }

        public override Vector GetShift(Point aPoint, double aTime)
        {
            foreach(var item in this.FItems)
            {
                var dx = aPoint.X - item.Position.X;
                var dy = aPoint.Y - item.Position.Y;

                var dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist < Double.Epsilon)
                    continue;

                if(dist < 2 * item.Radius)
                {
                    CollisionMotion _itemMot = null;
                    if (item.Motion is CollisionMotion)
                    {
                        _itemMot = (CollisionMotion)item.Motion;
                    }

                    if (_itemMot == null)
                        continue;

                    var _itemVel = _itemMot.Velocity;

                    var _collDir = Math.Atan2(dy, dx);
                    var _moveDir = Math.Atan2(this.FVelocity.Y, this.FVelocity.X);
                    var _dDir = _moveDir - _collDir;

                    var _proj = this.FVelocity.Length * Math.Cos(_dDir);
                    var _velX = this.FVelocity.X - _proj * Math.Cos(_collDir);
                    var _velY = this.FVelocity.Y - _proj * Math.Sin(_collDir);

                    var _itemVelX = _itemVel.X + _proj * Math.Cos(_collDir);
                    var _itemVelY = _itemVel.Y + _proj * Math.Sin(_collDir);

                    _moveDir = Math.Atan2(_itemVel.Y, _itemVel.X);
                    _dDir = _moveDir - _collDir;
                    _proj = _itemVel.Length * Math.Cos(_dDir);

                    _velX += _proj * Math.Cos(_collDir);
                    _velY += _proj * Math.Sin(_collDir);

                    _itemVelX -= _proj * Math.Cos(_collDir);
                    _itemVelY -= _proj * Math.Sin(_collDir);

                    this.FVelocity = new Vector(_velX, _velY);
                    _itemMot.Velocity = new Vector(_itemVelX, _itemVelY);

                    return this.GetVelocity() * aTime;
                }
            }

            return base.GetShift(aPoint, aTime);
        }

        public override void Reset()
        {
            this.FVelocity = this.FStartVelocity;
        }

        public Vector Velocity
        {
            get { return this.FVelocity; }
            set { this.FVelocity = value; }
        }

    }
}
