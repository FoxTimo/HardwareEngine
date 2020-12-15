using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using System.IO.Ports;

namespace ValorantEngine
{

class Logic
    {

        //SIZE
        const int xSize = 1920;
        const int ySize = 1080;

        //FOV in pixels, smaller fov will result in faster update time
        const int maxX = 400;
        const int maxY = 100;

        // GAME
        const int msBetweenShots = 200;
        const int closeSize = 10;
        const bool canShoot = true;

        // COLOR
        const int color = 0xc8c722; //0xb41515 = Red; 0xaf2eaf = purple 0xc8c722 = yellow
        const int colorVariation = 20;

        const double size = 60;  // DONT CHANGE
        const int maxCount = 5;

        static void Main(string[] args)
        {
            Update();
        }

        static void Update()
        {
            System.DateTime lastshot = System.DateTime.Now;
          
            while (true)
            {
                Task.Delay(1); // ANTI CRASH
                var l = PixelSearch(new Rectangle((xSize - maxX) / 2, (ySize - maxY) / 2, maxX, maxY), Color.FromArgb(color), colorVariation);
                if (l.Length > 0)
                { // IF NOT ERROR
                    var q = l.OrderBy(t => t.Y).ToArray();

                    List<Vector2> forbidden = new List<Vector2>();

                    for (int i = 0; i < q.Length; i++)
                    {
                        Vector2 current = new Vector2(q[i].X, q[i].Y);
                        if (forbidden.Where(t => (t - current).Length() < size || Math.Abs(t.X - current.X) < size).Count() < 1)
                        { // TO NOT PLACE POINTS AT THE BODY
                            forbidden.Add(current);
                            if (forbidden.Count > maxCount)
                            {
                                break;
                            }
                        }
                    }
                    // SHOOTING
                    var closes = forbidden.Select(t => (t - new Vector2(xSize / 2, ySize / 2))).OrderBy(t => t.Length()).ElementAt(0) + new Vector2(1, 1);

                    MouseHandle.Initialize();

                    if (MouseHandle.IsMouseButtonDown(MOUSE_BUTTONS.VK_LBUTTON))
                    {
                        ArduinoHandle ard = new ArduinoHandle("COM3");
                        ard.MoveMouse((int)(closes.X * (ValorantEngine.Properties.Settings.Default.speedX + Maths.getSpeed())), (int)(closes.Y * (ValorantEngine.Properties.Settings.Default.speedY + Maths.getSpeed())));
                        ard.ClosePort();
                    }
                }
            }
        }

        public static Point[] PixelSearch(Rectangle rect, Color Pixel_Color, int Shade_Variation) // REZ is for debugging
        {
            ArrayList points = new ArrayList();
            Bitmap RegionIn_Bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            using (Graphics GFX = Graphics.FromImage(RegionIn_Bitmap))
            {
                GFX.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }
            BitmapData RegionIn_BitmapData = RegionIn_Bitmap.LockBits(new Rectangle(0, 0, RegionIn_Bitmap.Width, RegionIn_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] Formatted_Color = new int[3] { Pixel_Color.B, Pixel_Color.G, Pixel_Color.R }; //bgr

            unsafe
            {
                for (int y = 0; y < RegionIn_BitmapData.Height; y++)
                {
                    byte* row = (byte*)RegionIn_BitmapData.Scan0 + (y * RegionIn_BitmapData.Stride);
                    for (int x = 0; x < RegionIn_BitmapData.Width; x++)
                    {
                        if (row[x * 3] >= (Formatted_Color[0] - Shade_Variation) & row[x * 3] <= (Formatted_Color[0] + Shade_Variation)) //blue
                            if (row[(x * 3) + 1] >= (Formatted_Color[1] - Shade_Variation) & row[(x * 3) + 1] <= (Formatted_Color[1] + Shade_Variation)) //green
                                if (row[(x * 3) + 2] >= (Formatted_Color[2] - Shade_Variation) & row[(x * 3) + 2] <= (Formatted_Color[2] + Shade_Variation)) //red
                                    points.Add(new Point(x + rect.X, y + rect.Y));
                    }
                }
            }
            RegionIn_Bitmap.Dispose();
            return (Point[])points.ToArray(typeof(Point));
        }
    }
}