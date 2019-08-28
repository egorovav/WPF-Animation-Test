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

        public double AverageVelocity
        {
            get
            {
                return this.FItems.Average(x => ((DetermineMotion)x.Motion).GetVelocity().Length);
            }
        }

        public override Vector GetShift(Item aItem, double aTime)
        {
             var _res = base.GetShift(aItem, aTime);

            foreach (var item1 in this.FItems)
            {
                var dx = aItem.Position.X - item1.Position.X;
                var dy = aItem.Position.Y - item1.Position.Y;

                var _dist = Math.Sqrt(dx * dx + dy * dy);
                if (_dist < Double.Epsilon)
                    continue;

                if(_dist <= aItem.Radius + item1.Radius)
                {
                    CollisionMotion _itemMot = null;
                    if (item1.Motion is CollisionMotion)
                    {
                        _itemMot = (CollisionMotion)item1.Motion;
                    }

                    if (_itemMot == null)
                        continue;

                    if (Math.Abs(item1.Mass - aItem.Mass) < Double.Epsilon)
                    {
                        var _itemVel = _itemMot.GetVelocity();
                        var _vel = this.GetVelocity();

                        double _collDir = Math.Atan2(dy, dx);

                        double _moveDir = Math.Atan2(_vel.Y, _vel.X);
                    
                        var _dDir = _moveDir - _collDir;

                        var _proj = _vel.Length * Math.Cos(_dDir);
                        var _velX = _vel.X - _proj * Math.Cos(_collDir);
                        var _velY = _vel.Y - _proj * Math.Sin(_collDir);

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
                        this.FTime = 0;
                        _itemMot.FVelocity = new Vector(_itemVelX, _itemVelY);
                        _itemMot.FTime = 0;

                        var _s = this.GetVelocity() * aTime;

                        var _collDist = 2 * item1.Radius - _dist + 1;

                        _s += new Vector(_collDist * Math.Cos(_collDir), _collDist * Math.Sin(_collDir));

                        //if (IsWallUsed)
                        //{
                        //    _s = WallProcessing(aPoint, _s);
                        //}

                        //return _s;

                        _res += _s;
                    }
                    else
                    {
                        var _v2 = _itemMot.GetVelocity();
                        var _v1 = this.GetVelocity();

                        var _phi = Math.Atan2(dy, dx);

                        var _theta1 = Math.Atan2(_v1.Y, _v1.X);
                        var _dir1 = _theta1 - _phi;
                        var _proj1 = _v1.Length * Math.Cos(_dir1);

                        var _theta2 = Math.Atan2(_v2.Y, _v2.X);
                        var _dir2 = _theta2 - _phi;
                        var _proj2 = _v2.Length * Math.Cos(_dir2);

                        var _temp = (_proj1 * (aItem.Mass - item1.Mass) + 2 * item1.Mass * _proj2) / (aItem.Mass + item1.Mass);

                        double _velX = Math.Cos(_phi) * _temp + _v1.Length * Math.Sin(_dir1) * Math.Cos(_phi + Math.PI / 2);
                        double _velY = Math.Sin(_phi) * _temp + _v1.Length * Math.Sin(_dir1) * Math.Sin(_phi + Math.PI / 2);

                        _temp = (_proj2 * (item1.Mass - aItem.Mass) + 2 * aItem.Mass * _proj1) / (item1.Mass + aItem.Mass);

                        double _itemVelX = Math.Cos(_phi) * _temp + _v2.Length * Math.Sin(_dir2) * Math.Cos(_phi + Math.PI / 2);
                        double _itemVelY = Math.Sin(_phi) * _temp + _v2.Length * Math.Sin(_dir2) * Math.Sin(_phi + Math.PI / 2);

                        this.FVelocity = new Vector(_velX, _velY);
                        this.FTime = 0;
                        _itemMot.FVelocity = new Vector(_itemVelX, _itemVelY);
                        _itemMot.FTime = 0;

                        var _s = this.GetVelocity() * aTime;

                        var _collDist = aItem.Radius + item1.Radius - _dist + 1;

                        _s += new Vector(_collDist * Math.Cos(_phi), _collDist * Math.Sin(_phi));

                        //if (IsWallUsed)
                        //{
                        //    _s = WallProcessing(aPoint, _s);
                        //}

                        //return _s;

                        _res += _s;
                    }
                }
            }

            //return base.GetShift(aPoint, aTime);
            return _res;
        }

        public override void Reset()
        {
            this.FVelocity = this.FStartVelocity;
        }
    }
}
