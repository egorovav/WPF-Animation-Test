using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using System.Linq;

namespace AnimationTest
{
    public class MainViewModel : BaseViewModel
    {
        //private List<Item> FItems = new List<Item>();
        private CancellationTokenSource FRunCancellationTokenSource;
        private int FWaitTime = 1;

        private double FEarthRadiusRel = 0.004;
        private double FEarthOrbit = 149598261;
        private double FEarthDiam = 12756;
        private double FDayDurationRel = 100;
        private double FPeriod = 365.256;
        private double FDelta = 0.1;
        private const double FEarthMass = 0.0000001;
        private double FSolarMass = FEarthMass * 332940;
        private double FSinPiPer6 = Math.Sqrt(3) / 2;

        private Dictionary<string, List<Item>> FItemsLists = new Dictionary<string, List<Item>>();

        public static object CanvasLocker = new object();

        public MainViewModel()
        {
            var _startPoint = new Point(20, 20);
            var _red = Color.FromRgb(255, 0, 0);
            var _blue = Color.FromRgb(0, 0, 255);
            var _violet = Color.FromRgb(255, 0, 255);

            var _list1 = new List<Item>();

            var _motion = new StraightMotion(new Vector(10, 0));
            _list1.Add(new Item(_blue, _startPoint, _motion));

            var _motion1 = new AcceleratedMotion(new Vector(0, 1));
            _list1.Add(new Item(_red, _startPoint, _motion1));

            var _motion2 = new MotionGroup();
            _motion2.AddMotion(_motion);
            _motion2.AddMotion(_motion1);
            _list1.Add(new Item(_violet, _startPoint, _motion2));

            var _list2 = new List<Item>();

            var _y = 200 * Math.Sqrt(3) + 50;
            var _v = 1.03;

            var _motionCenter = new GravityMotion(new Vector(-2 * _v, 0.1), _list2);
            var _center = new Item(Colors.Blue, new Point(300, 50), _motionCenter, 1700);
            _list2.Add(_center);

            var _motionSattelite = new GravityMotion(new Vector(_v, _v * Math.Sqrt(3)), _list2);
            var _sattelite = new Item(Colors.Gray, new Point(100, _y), _motionSattelite, 1700);
            _list2.Add(_sattelite);

            //var _motion3 = new GravityMotion(new Vector(1.1, -2.2), _list2);
            var _motion3 = new GravityMotion(new Vector(_v, -_v * Math.Sqrt(3)), _list2);
            var _sattelite2 = new Item(Colors.Green, new Point(500, _y), _motion3, 1700);
            _list2.Add(_sattelite2);

            var _list3 = new List<Item>();

            var _motionCenter1 = new GravityMotion(new Vector(0, 0), _list3);
            var _center1 = new Item(Colors.Blue, new Point(600, 400), _motionCenter1, 5860, 50);
            _list3.Add(_center1);

            var _motionSattelite1 = new GravityMotion(new Vector(2, 0), _list3);
            var _sattelite1 = new Item(Colors.Gray, new Point(600, 40), _motionSattelite1);
            _list3.Add(_sattelite1);

            for (int i = 0; i < 10; i++)
            {
                var _motionS = new GravityMotion(new Vector(2 + i * 0.5, 0), _list3);
                var _s = new Item(Colors.Gray, new Point(600, 40), _motionS);
                _list3.Add(_s);
            }

            var _list4 = new List<Item>();
            var _list5 = new List<Item>();

            var _motionEarth = new GravityMotion(new Vector(-0.048, 0), _list4);
            var _Earth = new Item(Colors.Blue, new Point(600, 400), _motionEarth, 5860, 50);
            _list4.Add(_Earth);

            var _motionEarth5 = new GravityMotion(new Vector(-0.048, 0), _list5);
            var _Earth5 = new Item(Colors.Blue, new Point(600, 400), _motionEarth5, 5860, 50);
            _list5.Add(_Earth5);

            var _motionMoon = new GravityMotion(new Vector(4, 0), _list4);
            var _Moon = new Item(Colors.SandyBrown, new Point(600, 40), _motionMoon, 70);
            _list4.Add(_Moon);

            var _motionMoon5 = new GravityMotion(new Vector(4, 0), _list5);
            var _Moon5 = new Item(Colors.SandyBrown, new Point(600, 40), _motionMoon5, 70);
            _list5.Add(_Moon5);

            var _motionSattelite4 = new GravityMotion(new Vector(12.60685198, -7.1), _list4);
            var _sattelite4 = new Item(Colors.Gray, new Point(600, 350), _motionSattelite4, 0, 3);
            _list4.Add(_sattelite4);

            //for (int i = 98; i < 99; i++)
            //{
            //	var _motionS = new GravityMotion(new Vector(12.606851 + i * 0.00000001, -7.1), _list4);
            //	var _s = new Item(Colors.Gray, new Point(600, 350), _motionS, 0, 3);
            //	_list4.Add(_s);
            //}

            //for (int i = 0; i < 100; i++)
            //{
            //    var _motionS = new GravityMotion(new Vector(7.29 + i * 0.000001, 12.3137), _list4);
            //    var _s = new Item(Colors.Gray, new Point(600, 350), _motionS, 0, 3);
            //    _list4.Add(_s);
            //}

            var _motionSattelite5 = new GravityMotion(new Vector(Math.Sqrt(7f / 2f), 0), _list5);
            var _sattelite5 = new Item(Colors.Gray, new Point(600, 20), _motionSattelite5, 0, 3);
            _list5.Add(_sattelite5);

            var _list6 = new List<Item>();

            var _motion6_1 = new GravityMotion(new Vector(1, -1), _list6);
            var _item6_1 = new Item(Colors.Blue, new Point(10, 10), _motion6_1, 1700);
            _list6.Add(_item6_1);

            var _motion6_2 = new GravityMotion(new Vector(1, 1), _list6);
            var _item6_2 = new Item(Colors.Gray, new Point(450, 150), _motion6_2, 1700);
            _list6.Add(_item6_2);

            var _motion6_3 = new GravityMotion(new Vector(-1, 1), _list6);
            var _item6_3 = new Item(Colors.Green, new Point(590, 590), _motion6_3, 1700);
            _list6.Add(_item6_3);

            var _motion6_4 = new GravityMotion(new Vector(-1, -1), _list6);
            var _item6_4 = new Item(Colors.Violet, new Point(150, 450), _motion6_4, 1700);
            _list6.Add(_item6_4);

            var _centerSolar = new Point(600, 400);

            var _list7 = new List<Item>();
            var _motionSolar1 = new GravityMotion(new Vector(0.17, 0), _list7);
            var _sol1 = new Item(Colors.Orange, _centerSolar, _motionSolar1, this.FSolarMass * 200);
            _list7.Add(_sol1);

            var _motionSolar2 = new GravityMotion(new Vector(-0.42, 0), _list7);
            var _sol2 = new Item(Colors.OrangeRed, new Point(_centerSolar.X, _centerSolar.Y - 30),
                _motionSolar2, this.FSolarMass * 100, 5);
            _list7.Add(_sol2);

            var _motion7_1 = new GravityMotion(new Vector(0.4, 0), _list7);
            var _plan1 = new Item(Colors.Blue, new Point(_centerSolar.X, _centerSolar.Y - 100),
                _motion7_1, this.FSolarMass * 20, 2);
            _list7.Add(_plan1);

            this.FItemsLists.Add(ItemsNames.GroupAccelerated, _list1);
            this.FItemsLists.Add(ItemsNames.GravityThreeObjects, _list2);
            this.FItemsLists.Add(ItemsNames.GravityEarthSattelite, _list3);
            this.FItemsLists.Add(ItemsNames.GravityToMoon, _list4);
            this.FItemsLists.Add(ItemsNames.GravityMoon, _list5);
            this.FItemsLists.Add(ItemsNames.GravityFourObjects, _list6);
            this.FItemsLists.Add(ItemsNames.Pulsar, _list7);

            var _earthOrbitRadius = this.FEarthRadiusRel * 2 * this.FEarthOrbit / this.FEarthDiam;
            var _planets = new List<Item>();

            double _earthEx = 0.017;
            var _earthVelocity = Math.Sqrt(this.FSolarMass * (1 + _earthEx) / _earthOrbitRadius * (1 - _earthEx));
            var _motionEarth1 = new GravityMotion(new Vector(_earthVelocity, 0), _planets);
            var _earth = new Item(Colors.Blue, new Point(_centerSolar.X, _centerSolar.Y - _earthOrbitRadius), _motionEarth1,
                FEarthMass * 1, 1); // this.FEarthRadiusRel);
            _planets.Add(_earth);

            double _mercEx = 0.2056;
            var _mercOrbitRadius = _earthOrbitRadius * 0.3075;
            var _mercVelocity = Math.Sqrt(this.FSolarMass * (1 + _mercEx) / _mercOrbitRadius * (1 - _mercEx));
            var _motionMerc = new GravityMotion(new Vector(_mercVelocity, 0), _planets);
            var _merc = new Item(Colors.Brown, new Point(_centerSolar.X, _centerSolar.Y - _mercOrbitRadius), _motionMerc,
                FEarthMass * 0.055, 1);// this.FEarthRadiusRel * 0.38);
            _planets.Add(_merc);

            double _venusEx = 0.0068;
            var _venusOrbitRadius = _earthOrbitRadius * 0.7233;
            var _venusVelocity = Math.Sqrt(this.FSolarMass * (1 + _venusEx) / _venusOrbitRadius * (1 - _venusEx));
            var _motionVenus = new GravityMotion(new Vector(_venusVelocity, 0), _planets);
            var _venus = new Item(Colors.Aqua, new Point(_centerSolar.X, _centerSolar.Y - _venusOrbitRadius), _motionVenus,
                FEarthMass * 0.815, 1); // this.FEarthRadiusRel * 0.95);
            _planets.Add(_venus);

            var _marsEx = 0.0934;
            var _marsOrbitRadius = _earthOrbitRadius * 1.381;
            var _marsVelocity = Math.Sqrt(this.FSolarMass * (1 + _marsEx) / _marsOrbitRadius * (1 - _marsEx));
            var _motionMars = new GravityMotion(new Vector(_marsVelocity, 0), _planets);
            var _mars = new Item(Colors.Red, new Point(_centerSolar.X, _centerSolar.Y - _marsOrbitRadius), _motionMars,
                FEarthMass * 0.107, 1); // this.FEarthRadiusRel * 0.532);
            _planets.Add(_mars);

            var _jupiEx = 0.0488;
            var _jupiOrbitRadius = _earthOrbitRadius * 4.9504;
            var _jupiVelocity = Math.Sqrt(this.FSolarMass * (1 + _jupiEx) / _jupiOrbitRadius * (1 - _jupiEx));
            var _motionJupi = new GravityMotion(new Vector(-_jupiVelocity, 0), _planets);
            var _jupi = new Item(Colors.DarkGray, new Point(_centerSolar.X, _centerSolar.Y + _jupiOrbitRadius), _motionJupi,
                FEarthMass * 317.8, 1); // this.FEarthRadiusRel * 11.2);
            _planets.Add(_jupi);

            var _saturnEx = 0.0557;
            var _saturnOrbitRadius = _earthOrbitRadius * 9.048;
            var _saturnVelocity = Math.Sqrt(this.FSolarMass * (1 + _saturnEx) / _saturnOrbitRadius * (1 - _saturnEx));
            var _motionSaturn = new GravityMotion(new Vector(_saturnVelocity, 0), _planets);
            var _saturn = new Item(Colors.GreenYellow, new Point(_centerSolar.X, _centerSolar.Y - _saturnOrbitRadius), _motionSaturn,
                FEarthMass * 95, 1); // this.FEarthRadiusRel * 9.41);
            _planets.Add(_saturn);

            var _uranEx = 0.0444;
            var _uranOrbitRadius = _earthOrbitRadius * 18.3755;
            var _uranVelocity = Math.Sqrt(this.FSolarMass * (1 + _uranEx) / _uranOrbitRadius * (1 - _uranEx));
            var _motionUran = new GravityMotion(new Vector(_uranVelocity, 0), _planets);
            var _uran = new Item(Colors.LightBlue, new Point(_centerSolar.X, _centerSolar.Y - _uranOrbitRadius), _motionUran,
                FEarthMass * 14.6, 1); // this.FEarthRadiusRel * 3.98);
            _planets.Add(_uran);

            var _neptunEx = 0.0112;
            var _neptunOrbitRadius = _earthOrbitRadius * 29.766;
            var _neptunVelocity = Math.Sqrt(this.FSolarMass * (1 + _neptunEx) / _neptunOrbitRadius * (1 - _neptunEx));
            var _motionNeptun = new GravityMotion(new Vector(_neptunVelocity, 0), _planets);
            var _neptun = new Item(Colors.Violet, new Point(_centerSolar.X, _centerSolar.Y - _neptunOrbitRadius), _motionNeptun,
                FEarthMass * 17.15, 1); // this.FEarthRadiusRel * 3.81);
            _planets.Add(_neptun);

            double _p = 0;
            double _mr = 0;
            double _ms = this.FSolarMass;
            for (int i = 0; i < _planets.Count; i++)
            {
                var _m = (GravityMotion)_planets[i].Motion;
                _p += _planets[i].Mass * _m.StartVelocity.X;
                _mr += _planets[i].Mass * _planets[i].Position.Y;
                _ms += _planets[i].Mass;
            }

            var _sy = (_ms * _centerSolar.Y - _mr) / this.FSolarMass;
            var _motionSolar = new GravityMotion(new Vector(-_p / this.FSolarMass, 0), _planets);
            var _sol = new Item(Colors.Orange, new Point(_centerSolar.X, _sy), _motionSolar,
                this.FSolarMass, 3); // 109 * this.FEarthRadiusRel);
            _planets.Add(_sol);

            this.FItemsLists.Add(ItemsNames.SolarSystem, _planets);

            var _listCombined1 = new List<Item>();

            var _periodicMotion = new PeriodicMotion(500, 100, new Vector(1, 0));
            _listCombined1.Add(new Item(_blue, new Point(200, 20), _periodicMotion));

            var _periodicMotion1 = new PeriodicMotion(500, 100, new Vector(0, -1), Math.PI / 2);
            _listCombined1.Add(new Item(_red, new Point(40, 100), _periodicMotion1));

            var _circleMotion = new MotionGroup();
            _circleMotion.AddMotion(_periodicMotion);
            _circleMotion.AddMotion(_periodicMotion1);
            _listCombined1.Add(new Item(_violet, new Point(200, 100), _circleMotion));

            this.FItemsLists.Add("Group:Circle", _listCombined1);

            var _listCombined2 = new List<Item>();

            var _periodicMotion2 = new PeriodicMotion(500, 200, new Vector(1, 0));
            _listCombined2.Add(new Item(_blue, new Point(200, 20), _periodicMotion2));

            var _periodicMotion4 = new PeriodicMotion(500, 100, new Vector(0, -1), Math.PI / 2);
            _listCombined2.Add(new Item(_red, new Point(40, 100), _periodicMotion4));

            var _ellipseMotion = new MotionGroup();
            _ellipseMotion.AddMotion(_periodicMotion2);
            _ellipseMotion.AddMotion(_periodicMotion4);
            _listCombined2.Add(new Item(_violet, new Point(200, 100), _ellipseMotion));

            this.FItemsLists.Add("Group:Ellipse", _listCombined2);

            var _listCombined3 = new List<Item>();

            var _periodicMotion5 = new PeriodicMotion(500, 200, new Vector(1, 0));
            _listCombined3.Add(new Item(_blue, new Point(200, 20), _periodicMotion5));

            var _periodicMotion3 = new PeriodicMotion(400, 200, new Vector(0, -1));
            _listCombined3.Add(new Item(_red, new Point(40, 200), _periodicMotion3));

            var _lissajousMotion = new MotionGroup();
            _lissajousMotion.AddMotion(_periodicMotion5);
            _lissajousMotion.AddMotion(_periodicMotion3);
            _listCombined3.Add(new Item(_violet, new Point(200, 200), _lissajousMotion));

            this.FItemsLists.Add("Group:Lissajous", _listCombined3);

            var _straightMotion1 = new StraightMotion(new Vector(1, 0));
            var _faddingMotion = new PeriodicMotion(200, 200, new Vector(0, -1), 0, 0.9999);
            var _elasticOscillation = new MotionGroup();
            _elasticOscillation.AddMotion(_faddingMotion);
            _elasticOscillation.AddMotion(_straightMotion1);
            var _listCombined5 = new List<Item>();
            _listCombined5.Add(new Item(Colors.Green, new Point(20, 250), _elasticOscillation));
            _listCombined5.Add(new Item(Colors.Aquamarine, new Point(20, 250), _faddingMotion));

            this.FItemsLists.Add("Group:Elastic", _listCombined5);

            var _listCombined6 = new List<Item>();
            var _periodicMotion3f = new PeriodicMotion(1000, 200, new Vector(1, 0), 0, 0.999995);
            var _periodicMotion5f = new PeriodicMotion(900, 200, new Vector(0, -1), 0, 0.999995);
            var _faddingLissajous = new MotionGroup();
            _faddingLissajous.AddMotion(_periodicMotion3f);
            _faddingLissajous.AddMotion(_periodicMotion5f);
            _listCombined6.Add(new Item(Colors.GreenYellow, new Point(300, 300), _faddingLissajous));

            this.FItemsLists.Add("Group:FaddingLissojus", _listCombined6);

            var _listCombined4 = new List<Item>();

            var _periodicMotion6 = new PeriodicMotion(500, 100, new Vector(1, 0));
            var _periodicMotion7 = new PeriodicMotion(500, 100, new Vector(0, -1), Math.PI / 2);
            var _straightMotion = new StraightMotion(_periodicMotion6.GetVelocity());
            var _cicloidMotion = new MotionGroup();
            _cicloidMotion.AddMotion(_periodicMotion6);
            _cicloidMotion.AddMotion(_periodicMotion7);
            _cicloidMotion.AddMotion(_straightMotion);

            var _circleMotion1 = new MotionGroup();
            _circleMotion1.AddMotion(_periodicMotion6);
            _circleMotion1.AddMotion(_periodicMotion7);

            _listCombined4.Add(new Item(_blue, new Point(100, 20), _circleMotion1));
            _listCombined4.Add(new Item(_red, new Point(100, 20), _straightMotion));
            _listCombined4.Add(new Item(_violet, new Point(100, 20), _cicloidMotion));
            this.FItemsLists.Add("Group:Cicloid", _listCombined4);

            var _listGaz1 = new List<Item>();

            var _random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 100; i++)
            {
                var _randomMotion = new StraightMotion(1, 10, _random);
                var _item = new Item(Colors.Gray, new Point(400, 200), _randomMotion, 1, 3);
                _listGaz1.Add(_item);
            }

            this.FItemsLists.Add("Random:Gaz", _listGaz1);

            var _listGaz2 = new List<Item>();

            _random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 100; i++)
            {
                var _randomMotion = new CollisionMotion(StraightMotion.GetRandomVelocity(1, 10, _random), _listGaz2);
                var _start = new Point(_random.Next(10, 1200), _random.Next(10, 800));
                var _item = new Item(Colors.Gray, _start, _randomMotion, 1, 5);
                _listGaz2.Add(_item);
            }

            this.FItemsLists.Add("Random:GazWithCollisions", _listGaz2);

            var _listGaz3 = new List<Item>();

            _random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 100; i++)
            {
                var _velocity = StraightMotion.GetRandomVelocity(1, 10, _random);
                var _randomMotion = new CollisionMotion(_velocity, new Vector(0, 0.1), _listGaz3);
                var _start = new Point(i % 20 * 40, (i / 20) * 40);
                var _item = new Item(Colors.Gray, _start, _randomMotion, 1, 5);
                _listGaz3.Add(_item);
            }

            this.FItemsLists.Add("Random:GazWithGravity", _listGaz3);

            //var _listGaz4 = new List<Item>();

            //var _velocity8 = new Vector(1, 0);
            //var _motion8 = new CollisionMotion(_velocity8, new Vector(0, 0.1), _listGaz4);
            //var _start8 = new Point(40, 40);
            //var _item8 = new Item(Colors.Gray, _start8, _motion8, 0, 5);
            //_listGaz4.Add(_item8);

            //var _velocity9 = new Vector(0, 0);
            //var _motion9 = new CollisionMotion(_velocity9, new Vector(0, 0.1), _listGaz4);
            //_start8 = new Point(70, 40);
            //var _item9 = new Item(Colors.Gray, _start8, _motion9, 0, 5);
            //_listGaz4.Add(_item9);

            //this.FItemsLists.Add("Random:GazWithGravityTest", _listGaz4);

            var _listGaz5 = new List<Item>();

            _random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 150; i++)
            {
                var _randomMotion = new CollisionMotion(StraightMotion.GetRandomVelocity(1, 10, _random), _listGaz5);
                var _start = new Point(_random.Next(10, 1200), _random.Next(10, 800));
                var _item = new Item(Colors.Transparent, _start, _randomMotion, 1, 5);
                _listGaz5.Add(_item);
            }
            var _motionBig = new CollisionMotion(new Vector(0, 0), _listGaz5);
            var _startBig = new Point(400, 400);
            var _itemBig = new Item(Colors.Gray, _startBig, _motionBig, 10, 20);
            _listGaz5.Add(_itemBig);

            this.FItemsLists.Add("Random:Brown", _listGaz5);

            var _collItems = new List<Item>();
            var _collMotion1 = new CollisionMotion(new Vector(5, 0), _collItems);
            var _collItem1 = new Item(Colors.Gray, new Point(20, 300), _collMotion1, 1);
            _collItems.Add(_collItem1);
            var _collMotion2 = new CollisionMotion(new Vector(-5, 0), _collItems);
            var _collItem2 = new Item(Colors.DarkGray, new Point(600, 315), _collMotion2, 1);
            _collItems.Add(_collItem2);
            this.FItemsLists.Add("Collision:SimpleTest", _collItems);

            var _collItemsDiff = new List<Item>();
            var _collMotionDiff1 = new CollisionMotion(new Vector(5, 0), _collItemsDiff);
            var _collItemDiff1 = new Item(Colors.Gray, new Point(20, 300), _collMotionDiff1, 1, 10);
            _collItemsDiff.Add(_collItemDiff1);
            var _collMotionDiff2 = new CollisionMotion(new Vector(-5, 0), _collItemsDiff);
            var _collItemDiff2 = new Item(Colors.DarkGray, new Point(800, 315), _collMotionDiff2, 2, 20);
            _collItemsDiff.Add(_collItemDiff2);
            this.FItemsLists.Add("Collision:SimpleTestDiffMass", _collItemsDiff);


            var _collItems1 = new List<Item>();
            var _collMotion11 = new CollisionMotion(new Vector(-5, 0), _collItems1);
            var _collItem11 = new Item(Colors.Gray, new Point(600, 300), _collMotion11, 1);
            _collItems1.Add(_collItem11);
            double _y0 = 300;
            double _x1 = 200;
            double _dy = _collItem11.Radius + 1;
            double _dx = 2 * _collItem11.Radius * this.FSinPiPer6 + 1;
            for (var i = 1; i < 5; i++)
            {
                double _y1 = _y0;

                for (var j = 0; j < i; j++)
                {
                    var _collMotion12 = new CollisionMotion(new Vector(0, 0), _collItems1);
                    var _collItem12 = new Item(Colors.DarkGray, new Point(_x1, _y1), _collMotion12, 1);
                    _collItems1.Add(_collItem12);

                    _y1 += 2 * _dy;
                }

                _y0 -= _dy;
                _x1 -= _dx;
            }
            this.FItemsLists.Add("Collision:Piramide", _collItems1);

            //this.FItems = this.FItemsLists[4];
        }


        public static string DeltaPropertyName = "Delta";
        public double Delta
        {
            get { return this.FDelta; }
            set
            {
                this.FDelta = value;
                NotifyPropertyChanged(DeltaPropertyName);
            }
        }

        public static string ItemsNamePropertyName = "ItemsName";
        private string FItemsName;
        public string ItemsName
        {
            get
            {
                return this.FItemsName;
            }
            set
            {
                this.FItemsName = value;
                NotifyPropertyChanged(ItemsNamePropertyName);
            }
        }

        public List<Item> Items
        {
            get
            {
                if (this.FItemsName == null)
                {
                    return null;
                }

                return this.FItemsLists[this.FItemsName];
            }
        }

        public Dictionary<string, List<Item>> ItemsLists
        {
            get { return this.FItemsLists; }
        }

        public static string IsRunPropertyName = "IsRun";
        private bool FIsRun = false;
        public bool IsRun
        {
            get { return this.FIsRun; }
            set
            {
                this.FIsRun = value;
                this.FStopCommand.CanExecute(this);
                this.FRunCommand.CanExecute(this);
                NotifyPropertyChanged(MainViewModel.IsRunPropertyName);
            }
        }

        public static string DrawTracksPropertyName = "DrawTracks";
        private bool FDrawTracks;
        public bool DrawTracks
        {
            get
            {
                return this.FDrawTracks;
            }
            set
            {
                this.FDrawTracks = value;
                NotifyPropertyChanged(DrawTracksPropertyName);
            }
        }

        public static string SimplifyTracksPropertyName = "SimplifyTracks";
        private bool FSimplifyTracks;
        public bool SimplifyTracks
        {
            get { return this.FSimplifyTracks; }
            set
            {
                this.FSimplifyTracks = value;
                NotifyPropertyChanged(SimplifyTracksPropertyName);
            }
        }

        private bool FFieldIsDrew = true;
        public bool CanvasIsDrew
        {
            get { return this.FFieldIsDrew; }
            set { this.FFieldIsDrew = value; }
        }

        private static string VelocityPropertyName = "Velocity";
        private Vector FVelocity;
        public Vector Velocity
        {
            get
            {
                return this.FVelocity;
            }
            set
            {
                this.FVelocity = value;

                var _list4 = this.ItemsLists[ItemsNames.GravityToMoon];

                var _motionSattelite4_1 = new GravityMotion(value, _list4);
                var _sattelite4_1 = new Item(Colors.Gray, new Point(600, 350), _motionSattelite4_1, 0, 3);
                _list4[2] = _sattelite4_1;

                NotifyPropertyChanged(VelocityPropertyName);
            }
        }

        private static string AvgVelocityPropertyName = "AvgVelocity";
        private double FAvgVelocity;
        public double AvgVelocity
        {
            get { return this.FAvgVelocity; }
            set
            {
                this.FAvgVelocity = value;
                NotifyPropertyChanged(AvgVelocityPropertyName);
            }
        }

        private static string MomentumPropertyName = "Momentum";
        private double FMomentum;
        public double Momentum
        {
            get { return this.FMomentum; }
            set
            {
                this.FMomentum = value;
                NotifyPropertyChanged(MomentumPropertyName);
            }
        }

        private static string EnergyPropertyName = "Energy";
        private double FEnergy;
        public double Energy
        {
            get { return this.FEnergy; }
            set
            {
                this.FEnergy = value;
                NotifyPropertyChanged(EnergyPropertyName);
            }
        }

        private Vector FChallengerVelocity;
        public Vector ChallengerVelocity
        {
            get { return this.FChallengerVelocity; }
            set
            {
                this.FChallengerVelocity = value;

                var _solarSystem = this.ItemsLists[ItemsNames.SolarSystem];
                var _motionChallenger = new GravityMotion(
                    new Vector(this.ChallengerVelocity.X / 100, this.ChallengerVelocity.Y / 100), _solarSystem);
                var _earth = _solarSystem[0];
                var _challenger = new Item(Colors.Gray, _earth.Position, _motionChallenger,
                    0.00000001 * FEarthMass);
                if (_solarSystem.Count < 10)
                    _solarSystem.Add(_challenger);
                else
                    _solarSystem[9] = _challenger;
            }
        }

        public async void Go()
        {
            if (this.IsRun)
                return;

            this.FRunCancellationTokenSource = new CancellationTokenSource();
            this.FRunCancellationTokenSource.Token.Register(() => this.IsRun = false);
            this.IsRun = true;
            this.IsRun = await Task.Run<bool>(() =>
            {
                try
                {
                    while (!this.FRunCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        //if(this.CanvasIsDrew)
                        {
                            if (this.Items == null || this.Items.Count == 0)
                                return false;

                            this.Step();
                            Thread.Sleep(this.FWaitTime);
                        }
                    }

                    return false;
                }
                catch (Exception exc)
                {
                    var _sb = new StringBuilder(exc.Message);
                    var _tempExc = exc;
                    while (_tempExc.InnerException != null)
                    {
                        _tempExc = _tempExc.InnerException;
                        _sb.AppendLine(_tempExc.Message);
                    }
                    _sb.AppendLine(exc.StackTrace);
                    this.ErrorString = _sb.ToString();
                    return false;
                }

            }, this.FRunCancellationTokenSource.Token);
        }

        public static string CanvasPropertyName = "Canvas";
        private void Step()
        {
            lock (CanvasLocker)
            {
                try
                {
                    foreach (var _item in this.Items)
                    {
                        _item.UpdatePosition(this.FDelta);
                    }
                }
                catch (OutOfMemoryException)
                {
                    foreach (var _item in this.Items)
                    {
                        _item.ClearTrack();
                    }
                }
                //this.CanvasIsDrew = false;
            }

            if (this.Items != null && this.Items.Count > 0)
            {
                var _motion = this.Items[0].Motion;
                //if(_motion is DetermineMotion)
                //    this.AvgVelocity = this.Items
                //        .Where(x => x.Motion is DetermineMotion)
                //        .Average(x => ((DetermineMotion)x.Motion).GetVelocity().Length);

                if (_motion is GravityMotion)
                    this.FAvgVelocity = this.Items.Average(x => ((GravityMotion)x.Motion).Velocity.Length);

                if (_motion is MotionGroup)
                    this.AvgVelocity = this.Items
                        .Where(x => x.Motion is MotionGroup)
                        .Average(x => ((MotionGroup)x.Motion).AvgVelocity);
                double _energy = 0;
                int _itemCount = 0;
                double _velocity = 0;
                foreach (var item in this.Items)
                {
                    var _itemMotion = item.Motion;
                    if (_itemMotion is DetermineMotion)
                    {
                        var _detMotion = (DetermineMotion)_itemMotion;
                        var _v = _detMotion.GetVelocity().Length;
                        _energy += _v * _v * item.Mass / 2;

                        _itemCount++;

                        _velocity += _v;
                    }
                    //this.Energy = this.Items
                    //    .Where(x => x.Motion is DetermineMotion)
                    //    .Sum(x => ((DetermineMotion)x.Motion).GetVelocity().Length * ((DetermineMotion)x.Motion).GetVelocity().Length * x.Mass);
                }
                this.Energy = _energy;
                this.AvgVelocity = _velocity / _itemCount;
            }

            NotifyPropertyChanged(MainViewModel.CanvasPropertyName);
        }

        private void Cancel()
        {
            if (this.FRunCancellationTokenSource != null)
            {
                this.FRunCancellationTokenSource.Cancel();
            }
        }

        #region = Commnands =

        private DelegateCommand FStepCommand = new DelegateCommand(ExecuteStep, CanExecuteStep);
        public DelegateCommand StepCommand
        {
            get { return this.FStepCommand; }
        }

        private static void ExecuteStep(object aCommandData)
        {
            var _model = (MainViewModel)aCommandData;
            _model.Step();
        }

        private static bool CanExecuteStep(object aCommandData)
        {
            return true;
        }

        private DelegateCommand FRunCommand = new DelegateCommand(ExecuteRun, CanExecuteRun);
        public DelegateCommand RunCommand
        {
            get { return this.FRunCommand; }
        }

        private static void ExecuteRun(object aCommandData)
        {
            var _model = (MainViewModel)aCommandData;
            _model.Go();
        }

        private static bool CanExecuteRun(object aCommandData)
        {
            if (aCommandData == null)
                return false;

            var _model = (MainViewModel)aCommandData;
            return !_model.IsRun;
        }

        private DelegateCommand FStopCommand = new DelegateCommand(ExecuteStop, CanExecuteStop);
        public DelegateCommand StopCommand
        {
            get { return this.FStopCommand; }
        }

        public static void ExecuteStop(object aCommandData)
        {
            var _model = (MainViewModel)aCommandData;
            _model.Cancel();
        }

        public static bool CanExecuteStop(object aCommandData)
        {
            if (aCommandData == null)
                return false;

            var _model = (MainViewModel)aCommandData;
            return _model.IsRun;
        }

        #endregion
    }
}
