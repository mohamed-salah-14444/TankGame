namespace TankGame;

static class Program
{
    
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }    

}
static class Setting
{
    public const int StartHP = 100;
    public const double AngleSpeed = 1;
    public const int TankWidth = 40;
    public const int TankHeight = 20;
    public const int Speed = 3;
    public const int BarrelLength = 40;
    public const double ChargeRate = 0.25;
    public const int MaxCharge = 25;
    public const int MinCharge = 4;
    public const int HitDamage = 34;
    public const double Fuel = 30;
    public const double FuelPerTick = 0.3;
    public const double Gravity = 0.5;
}
 class Tank
{
    /*
    int fuel = 30;
    */
    public int x { get; set; }
    public int y { get; private set; }
    public int width { get; }
    public int height { get; }
    public double angle { get; set; }
    public int health { get; private set; }
    public Color bodyColor { get;  }
    public double Fuel { get; private set; }
    

    public Tank(int x, int y, int width, int height, double startangle, int health, Color bodyColor)
    {

        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.angle = startangle;
        this.health = health;
        this.bodyColor = bodyColor;
        this.Fuel = Setting.Fuel;
        UpdateY();
    }

    public void UpdateY()
    {
        y = Ground.GetHeight(x + width / 2) - height;
    }
    
    public PointF GetBarrelTip()
    {

        PointF pivot = GetPivot();
        double rad = angle * Math.PI / 180;
        return new PointF(
            pivot.X + (float)(Setting.BarrelLength * Math.Cos(rad)),
            pivot.Y - (float)(Setting.BarrelLength * Math.Sin(rad)));
    }   
        public PointF GetPivot()
    {
           return new PointF(x + width / 2f, y);
           
    }

        public void ChangeAngle(double delta)
    {
        angle = Math.Clamp(angle + delta, 0, 180);
    }

    public void Draw(Graphics g)
    {
        using var brush = new SolidBrush(bodyColor);
        g.FillRectangle(brush, x, y, width, height);

        PointF pivot = GetPivot();
        PointF tip = GetBarrelTip();
        using var pen = new Pen(Color.DarkGreen, 4);
        g.DrawLine(pen, pivot, tip);
    }

    public bool TakeDamage(int amount)
    {
        health = Math.Max(0, health - amount);
        return health == 0;
    }

    public void Move(int direction, int windowWidth)
    {
    if (Fuel <= 0) return;

    x = Math.Clamp(x + direction * Setting.Speed, 0, windowWidth - width);
    Fuel = Math.Max(0, Fuel - Setting.FuelPerTick);

    }

    public void Refuel()
    {
        Fuel = Setting.Fuel;
    }

        
}
 public static class Bullet
{
    /*
    int ammo1 = 5;
    int ammo2 = 5;
    double bulletdamage = 33.4;
    bool bullet1fired = false;
    bool bullet2fired = false;
    bool doubleBullet1 = false;
    bool doubleBullet2 = false;
    */
   public static List<PointF> GetTrajectory(float startX, float starty, double angleDeg, double power, int windowWidth)
    {
    var points = new List<PointF>();
    double angleRad = angleDeg * Math.PI / 180;
    double vx = power * Math.Cos(angleRad);
    double vy = -power * Math.Sin(angleRad);
    double x = startX;
    double y = starty;
    const double gravity = Setting.Gravity;
    points.Add(new PointF((float)x, (float)y));

    while (x >= 0 && x < windowWidth && y < Ground.GetHeight((int)x))
        {
            x += vx;
            y += vy;
            vy += gravity; // Pull downward

            points.Add(new PointF((float)x, (float)y));

            // Stop if ground level is reached or breached
            if (x >= 0 && x < windowWidth && y >= Ground.GetHeight((int)x))
                break;
        }
    return points;
    }
}
 class Barrel
{
    /*
    angle1 = 0
    angle2 = 0
    angle1 = angle1 + angleSpeed;
    angle2 = angle2 + angleSpeed;
    */
    
}
 class Ground
{
    // Each value is the screen Y of the ground surface in that column.
    // Y grows downward, so a smaller value means higher ground.
    public static int[] Heights { get; private set; } = Array.Empty<int>();

    public static void Initialize(int width, int baseLevel, int hillHeight)
    {
        Heights = new int[Math.Max(0, width)];

        for (int x = 0; x < Heights.Length; x++)
        {
            // t goes from 0 to PI across the window, so Sin(t) goes 0 -> 1 -> 0
            double t = (double)x / Math.Max(1, Heights.Length - 1) * Math.PI;
            Heights[x] = baseLevel - (int)(hillHeight * Math.Sin(t));
        }
    }

    public static int GetHeight(int x)
    {
        if (Heights.Length == 0)
            return 0;

        x = Math.Clamp(x, 0, Heights.Length - 1);
        return Heights[x];
    }

    public static void Draw(Graphics g, int windowHeight)
    {
        for (int x = 0; x < Heights.Length; x++)
        {
            g.FillRectangle(Brushes.SaddleBrown, x, Heights[x], 1, windowHeight - Heights[x]);
        }
    }

}
