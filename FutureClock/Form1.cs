using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FutureClock
{
    public partial class Form1 : Form
    {
        private DateTime currentTime;
        private PointF center;
        private float radius;
        private float colorLerpProgress = 0.0f;
        Color startColor = Color.Black;
        Color endColor = Color.White;

        public Form1()
        {
            InitializeComponent();
            currentTime = DateTime.Now;
            timer1.Interval = 1000;

            Text = "Future clock";

            timer1.Start();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            using (LinearGradientBrush backgroundBrush = new LinearGradientBrush(
                ClientRectangle,
                InterpolateColors(startColor, endColor, colorLerpProgress),
                InterpolateColors(endColor, startColor, colorLerpProgress), 
                45f))

            {
                g.FillRectangle(backgroundBrush, ClientRectangle);
            }

            center = new PointF(ClientSize.Width / 2, ClientSize.Height / 2);
            radius = Math.Min(center.X, center.Y) - 10;

            using (LinearGradientBrush clockGradientBrush = new LinearGradientBrush(
                new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2),
                Color.DarkSlateGray, Color.LightSkyBlue, 45f))
            {
                using (Pen clockPen = new Pen(clockGradientBrush, 14))
                {
                    g.DrawEllipse(clockPen, center.X - radius, center.Y - radius, radius * 2, radius * 2);
                }
                using (Pen clockInnerPen = new Pen(clockGradientBrush, 8)) 
                {
                    g.DrawEllipse(clockInnerPen, center.X - (radius * 0.9f), center.Y - (radius * 0.9f), radius * 1.8f, radius * 1.8f);
                    g.DrawEllipse(clockInnerPen, center.X - (radius * 0.8f), center.Y - (radius * 0.8f), radius * 1.6f, radius * 1.6f);
                }
            }


            using (Pen minuteMarkerPen = new Pen(Color.Black, 2.5f))  
            {
                for (int i = 0; i < 60; i++)
                {
                    float angle = (float)(Math.PI / 2 - 2 * Math.PI * i / 60);
                    float innerRadius = i % 5 == 0 ? radius - 20 : radius - 15;
                    float x1 = center.X + radius * (float)Math.Cos(angle);
                    float y1 = center.Y - radius * (float)Math.Sin(angle);
                    float x2 = center.X + innerRadius * (float)Math.Cos(angle);
                    float y2 = center.Y - innerRadius * (float)Math.Sin(angle);
                    g.DrawLine(minuteMarkerPen, x1, y1, x2, y2);
                }
            }

            Font font = new Font("Georgia", 18, FontStyle.Bold);
            string[] romanNumbers = { "XII", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI" };
            for (int i = 0; i < 12; i++)
            {
                float angle = (float)(Math.PI / 2 - 2 * Math.PI * i / 12);
                float textRadius = radius - 40; 
                float x = center.X + textRadius * (float)Math.Cos(angle);
                float y = center.Y - textRadius * (float)Math.Sin(angle);
                string number = romanNumbers[i];
                SizeF textSize = g.MeasureString(number, font);
                g.DrawString(number, font, Brushes.Black, x - textSize.Width / 2, y - textSize.Height / 2);
            }

            
            Color startColor1 = Color.Black; 
            Color endColor1 = Color.CornflowerBlue;     

            DrawHand(g, (currentTime.Hour % 12) / 12f, radius * 0.5f, startColor1, endColor1, 6);
            DrawHand(g, currentTime.Minute / 60f, radius * 0.7f, startColor1, endColor1, 4);
            DrawHand(g, currentTime.Second / 60f, radius * 0.9f, startColor1, endColor1, 2);
           
            float dotSize = 10f; 
            g.FillEllipse(Brushes.Black, center.X - dotSize / 2, center.Y - dotSize / 2, dotSize, dotSize);

        }

        private void DrawHand(Graphics g, float fraction, float length, Color startColor, Color endColor, float width)
        {
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(center, new PointF(center.X + (float)(length * Math.Cos(Math.PI / 2 - 2 * Math.PI * fraction)), center.Y - (float)(length * Math.Sin(Math.PI / 2 - 2 * Math.PI * fraction))), startColor, endColor))
            {
                using (Pen gradientPen = new Pen(gradientBrush, width))
                {
                    double angle = Math.PI / 2 - 2 * Math.PI * fraction;
                    float endX = center.X + (float)(length * Math.Cos(angle));
                    float endY = center.Y - (float)(length * Math.Sin(angle));
                    g.DrawLine(gradientPen, center, new PointF(endX, endY));
                }
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            currentTime = DateTime.Now;

            colorLerpProgress += 0.09f; 
            if (colorLerpProgress >= 1.0f)
            {
                colorLerpProgress = 0.0f;
                Color temp = startColor;
                startColor = endColor;
                endColor = temp;
            }

            Invalidate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
        }
        private Color InterpolateColors(Color color1, Color color2, float ratio)
        {
            int r = (int)(color1.R * (1 - ratio) + color2.R * ratio);
            int g = (int)(color1.G * (1 - ratio) + color2.G * ratio);
            int b = (int)(color1.B * (1 - ratio) + color2.B * ratio);
            return Color.FromArgb(r, g, b);
        }

    }
}
