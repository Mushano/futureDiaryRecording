using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using diary.Properties;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace diary
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string pathForFuture;
        string temp1;
        private void Draw(Rectangle rectangle, Graphics g, int _radius, bool cusp, Color begin_color, Color end_color)
        {
            int span = 2;
            //抗锯齿
            g.SmoothingMode = SmoothingMode.AntiAlias;
            //渐变填充
            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush(rectangle, begin_color, end_color, LinearGradientMode.Vertical);
            //画尖角
            if (cusp)
            {
                span = 10;
                PointF p1 = new PointF(rectangle.Width - 12, rectangle.Y + 10);
                PointF p2 = new PointF(rectangle.Width - 12, rectangle.Y + 30);
                PointF p3 = new PointF(rectangle.Width, rectangle.Y + 20);
                PointF[] ptsArray = { p1, p2, p3 };
                g.FillPolygon(myLinearGradientBrush, ptsArray);
            }
            //填充
            g.FillPath(myLinearGradientBrush, DrawRoundRect(rectangle.X, rectangle.Y, rectangle.Width - span, rectangle.Height - 1, _radius));
        }
        public static GraphicsPath DrawRoundRect(int x, int y, int width, int height, int radius)
        {
            //四边圆角
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(x, y, radius, radius, 180, 90);
            gp.AddArc(width - radius, y, radius, radius, 270, 90);
            gp.AddArc(width - radius, height - radius, radius, radius, 0, 90);
            gp.AddArc(x, height - radius, radius, radius, 90, 90);
            gp.CloseAllFigures();
            return gp;
        }
        string path =Settings.Default.path;
        void WriteLog(string text,string a)
        {
            string keyword = textBox1.Text;
            string path2 = path + "/"+a+"/";
            if (!Directory.Exists(path2))
            {
                Directory.CreateDirectory(path2);
            }
            path2 += DateTime.Now.ToString("yyyy-MM-dd") +keyword+ ".doc";
            using (FileStream fs = new FileStream(path2, FileMode.Append, FileAccess.Write, FileShare.Write))
            {
                StreamWriter wr = new StreamWriter(fs, Encoding.UTF8);
                wr.WriteLine(text);
                wr.Flush();
                wr.Close();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string a;
            string text = richTextBox1.Text;
            if (radioButton1.Checked == true)
            {
                a = "学习";
                WriteLog(text, a);
                MessageBox.Show("保存成功！");
            }
            else if (radioButton2.Checked ==true)
            {
                a = "心情";
                WriteLog(text, a);
                MessageBox.Show("保存成功！");
            }
            else
            {
                a = "";
                MessageBox.Show("请选择一个按钮！");  
            }
            if (checkBoxOpen.Checked ==true)
            {
                string path3 = path + "/未来日记/";
                //其实下面这个地方完全可以用一个函数进行调用。但是后面可以再想想优化，先实现
                if (!Directory.Exists(path3))
                {
                    Directory.CreateDirectory(path3);
                }
                path3 += textBoxYear.Text+textBoxMonth.Text+textBoxDay.Text + ".doc";
                using (FileStream fs = new FileStream(path3, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    StreamWriter wr = new StreamWriter(fs, Encoding.UTF8);
                    wr.WriteLine(text);
                    wr.Flush();
                    wr.Close();
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() ==DialogResult.OK)
            {
                path = fd.SelectedPath;
                MessageBox.Show("保存路径成功！");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("ch-zh"));
            label3.Text = DateTime.Now.ToString("yyyy.MM.dd dddd");
            richTextBox1.AcceptsTab = true;
            DirectoryInfo folder = new DirectoryInfo(path);
            foreach(var i in folder.GetDirectories())
            {
                if (i.Name =="未来日记")
                {
                    string tempPath = path + "/未来日记";
                    DirectoryInfo f = new DirectoryInfo(tempPath);
                    string time = DateTime.Now.ToString("yyyyMMdd")+".doc";
                    temp1 = tempPath + "/" + DateTime.Now.ToString("yyyyMMdd");
                    foreach (var j in f.GetFiles())
                    {
                        if (time ==j.Name)
                        {
                            label9.Text = "你有一封未来邮件！请接收！";
                            pathForFuture = tempPath + "/" + j.Name;
                        }
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.path = path;
            Settings.Default.Save();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog sd = new OpenFileDialog();
            sd.InitialDirectory = path;
            if (sd.ShowDialog() ==DialogResult.OK)
            {
                string s = sd.FileName;
                StreamReader sr = new StreamReader(s, Encoding.Default);
                while (!sr.EndOfStream)
                {
                    richTextBox1.Text = sr.ReadToEnd();
                }
            }
            textBox1.Text = sd.SafeFileName;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(210, 210, 210), Color.FromArgb(242, 242, 242));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("路径设置", new Font("微软雅黑", 9, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(10, 10));
        }

        private void button4_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(210, 210, 210), Color.FromArgb(242, 242, 242));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("查看日记", new Font("微软雅黑", 9, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(10, 10));
        }

        private void button5_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(210, 210, 210), Color.FromArgb(242, 242, 242));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("打开日记本", new Font("微软雅黑", 9, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(10, 10));
        }

        private void button1_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(210, 210, 210), Color.FromArgb(242, 242, 242));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("保存日记(Alt+s)", new Font("微软雅黑", 9, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(10, 10));
        }

        private void button2_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(210, 210, 210), Color.FromArgb(242, 242, 242));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("清空", new Font("微软雅黑", 9, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(10, 10));
        }

        private void button6_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(113, 113, 113), Color.FromArgb(0, 0, 0));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("-", new Font("微软雅黑", 13, FontStyle.Regular), new SolidBrush(Color.White), new PointF(10, 10));
        }

        private void button7_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(113, 113, 113), Color.FromArgb(0, 0, 0));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("×", new Font("微软雅黑", 13, FontStyle.Regular), new SolidBrush(Color.White), new PointF(10, 10));
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode ==Keys.Enter)
            {
                richTextBox1.Focus();
            }
        }

        private void checkBoxOpen_CheckedChanged(object sender, EventArgs e)
        {
                textBoxYear.Text = DateTime.Now.ToString("yyyy");
                textBoxMonth.Text = DateTime.Now.ToString("MM");
                textBoxDay.Text = DateTime.Now.ToString("dd");
        }

        private void label8_Click(object sender, EventArgs e)
        {
            
        }

        private void label9_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(pathForFuture, Encoding.Default);
            while (!sr.EndOfStream)
            {
                richTextBox1.Text = sr.ReadToEnd();
            }
            sr.Close();
            File.Move(pathForFuture, temp1+"(已阅)"+".doc");
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
