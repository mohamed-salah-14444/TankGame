namespace TankGame;

public partial class Form1:Form
{
    private readonly Image background;

Tank tank1 = null!;
Tank tank2 = null!;
    double firePower = Setting.MinCharge;

    bool leftheld = false;
    bool rightheld = false;
    bool upheld = false;
    bool downheld = false;
    bool aHeld = false;
    bool dHeld = false;
    bool wHeld = false;
    bool sHeld = false;
    bool spaceHeld = false;
    bool RHeld = false;
    private const int GroundLevel = 500;   // flat ground Y at the window edges
    private const int HillHeight = 150;    // how tall the hill is at the middle
    System.Windows.Forms.Timer gametimer = new System.Windows.Forms.Timer();
    public Form1()
    {
        InitializeComponent();
        DoubleBuffered = true;
        gametimer.Interval = 20;
        gametimer.Tick += GameTimer_Tick;
        gametimer.Start ();
        ClientSize = new Size(1000, 600);
        Ground.Initialize(ClientSize.Width, GroundLevel, HillHeight);
        tank1 = new Tank(100, 0, Setting.TankWidth, Setting.TankHeight, 0, Setting.StartHP, Color.Blue);
        tank2 = new Tank(900, 0, Setting.TankWidth, Setting.TankHeight, 180, Setting.StartHP, Color.Red);

        Paint += Form1_Paint;
        KeyDown += Form1_KeyDown;
        KeyUp += Form1_KeyUp;
        string path = Path.Combine(AppContext.BaseDirectory, "images", "background.jpg");
        background = Image.FromFile(path);
        
    }
     void GameTimer_Tick(object? sender, EventArgs e)
    {  
        int prevBox1X = tank1.x;
        int prevBox2X = tank2.x;
        if (leftheld) tank1.Move(-1, ClientSize.Width);
        if (rightheld) tank1.Move(1, ClientSize.Width);
        if (tank1.x < 0) tank1.x = 0;
        if (tank1.x > ClientSize.Width - Setting.TankWidth) tank1.x = ClientSize.Width - Setting.TankWidth;
        if (tank1.x < tank2.x + Setting.TankWidth && tank1.x + Setting.TankWidth > tank2.x)
        {
            if (prevBox1X < tank2.x)
                tank1.x = tank2.x - Setting.TankWidth; 
            else
                tank1.x = tank2.x + Setting.TankWidth; 
        }
        if (upheld) tank1.ChangeAngle(Setting.AngleSpeed);
        if (downheld) tank1.ChangeAngle(-Setting.AngleSpeed);
        if (tank1.angle < 0) tank1.angle = 0;
        if (tank1.angle > 180) tank1.angle = 180;


        if (aHeld) tank2.Move(-1, ClientSize.Width);
        if (dHeld) tank2.Move(1, ClientSize.Width);
        if (tank2.x < 0) tank2.x = 0;
        if (tank2.x > ClientSize.Width - Setting.TankWidth) tank2.x = ClientSize.Width - Setting.TankWidth;
        if (tank2.x < tank1.x + Setting.TankWidth && tank2.x + Setting.TankWidth > tank1.x)
        {
            if (prevBox2X < tank1.x)
                tank2.x = tank1.x - Setting.TankWidth;
            else
                tank2.x = tank1.x + Setting.TankWidth; 
        }

        if (sHeld) tank2.ChangeAngle(Setting.AngleSpeed);
        if (wHeld) tank2.ChangeAngle(-Setting.AngleSpeed);
        if (tank2.angle < 0) tank2.angle = 0;
        if (tank2.angle > 180) tank2.angle = 180;

        if (spaceHeld) firePower += Setting.ChargeRate;
        if (firePower > Setting.MaxCharge) firePower = Setting.MaxCharge;

        if (RHeld) firePower = Setting.MinCharge;


        tank1.UpdateY();
        tank2.UpdateY();

        Invalidate();
    }
    void Form1_Paint(object? sender, PaintEventArgs e)
    {
    e.Graphics.DrawImage(background, 0, 0, ClientSize.Width, ClientSize.Height);
    Ground.Draw(e.Graphics, ClientSize.Height);

    PointF tip1 = tank1.GetBarrelTip();
    List<PointF> path1 = Bullet.GetTrajectory(tip1.X, tip1.Y, tank1.angle, firePower, ClientSize.Width);
    if (path1.Count > 1)
    {
        using Pen tracePen1 = new Pen(Color.FromArgb(180, Color.Cyan), 2)
        {
            DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
        };
        e.Graphics.DrawLines(tracePen1, path1.ToArray());
    }
    
    PointF tip2 = tank2.GetBarrelTip();
    List<PointF> path2 = Bullet.GetTrajectory(tip2.X, tip2.Y, tank2.angle, firePower, ClientSize.Width);
    if (path2.Count > 1)
    {
        using Pen tracePen2 = new Pen(Color.FromArgb(180, Color.Orange), 2)
        {
            DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
        };
        e.Graphics.DrawLines(tracePen2, path2.ToArray());
    }

    tank1.Draw(e.Graphics);
    tank2.Draw(e.Graphics);
    void DrawFuelBar(Graphics g, Tank t)
    {
        float fraction = (float)(t.Fuel / Setting.Fuel);
        g.FillRectangle(Brushes.Gray, t.x, t.y - 12, t.width, 4);
        g.FillRectangle(Brushes.Gold, t.x, t.y - 12, t.width * fraction, 4);
    
    }
    DrawFuelBar(e.Graphics, tank1);
    DrawFuelBar(e.Graphics, tank2);

}
    void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left) leftheld = true;
        if (e.KeyCode == Keys.Right) rightheld = true;
        if (e.KeyCode == Keys.Up) upheld = true;
        if (e.KeyCode == Keys.Down) downheld = true;

        if (e.KeyCode == Keys.A) aHeld = true;
        if (e.KeyCode == Keys.D) dHeld = true;
        if (e.KeyCode == Keys.S) sHeld = true;
        if (e.KeyCode == Keys.W) wHeld = true;

        if (e.KeyCode == Keys.Space) spaceHeld = true;
        if (e.KeyCode == Keys.R) RHeld = true;


    }
    void Form1_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left) leftheld = false;
        if (e.KeyCode == Keys.Right) rightheld = false;
        if (e.KeyCode == Keys.Up) upheld = false;
        if (e.KeyCode == Keys.Down) downheld = false;

        if (e.KeyCode == Keys.A) aHeld = false;
        if (e.KeyCode == Keys.D) dHeld = false;
        if (e.KeyCode == Keys.S) sHeld = false;
        if (e.KeyCode == Keys.W) wHeld = false;

        if (e.KeyCode == Keys.Space) spaceHeld = false;
        if (e.KeyCode == Keys.R) RHeld = false;


    }
     

}