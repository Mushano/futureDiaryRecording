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

namespace diary
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string pathForFuture;
        string tempPath;
        int y, m, d, h;
        List<string> fut = new List<string>();
        int fut_n = 0;
        
        string path =Settings.Default.path;
        //*************************************
        //            Function函数
        //*************************************

        /// <summary>
        /// 在指定路径下创建文件夹
        /// </summary>
        /// <param name="path"></param>
        void createDirectory(string path2)
        {
            if (!Directory.Exists(path2))
            {
                Directory.CreateDirectory(path2);
            }
        }
        /// <summary>
        /// 根据指定路径创建写入数据并且保存文件
        /// </summary>
        /// <param name="path2"></param>
        void create_write_File(string path2,string text)
        {
            using (FileStream fs = new FileStream(path2, FileMode.Append, FileAccess.Write, FileShare.Write))
            {
                StreamWriter wr = new StreamWriter(fs, Encoding.UTF8);
                wr.WriteLine(text);
                wr.Flush();
                wr.Close();
            }
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="text"></param>
        /// <param name="a"></param>
        void WriteLog(string text,string a)
        {
            string keyword = textBox1.Text;
            string path2 = path + "/"+a+"/";
            createDirectory(path2);
            path2 += DateTime.Now.ToString("yyyy-MM-dd") +keyword+ ".doc";
            create_write_File(path2, text);
        }
        /// <summary>
        /// 保存分类的判断
        /// </summary>
        /// <param name="a"></param>
        /// <param name="text"></param>
        void saveJudge(out string  a,string text)
        {
            if (radioButton1.Checked == true)
            {
                a = "学习";
                WriteLog(text, a);
                MessageBox.Show("保存成功！");
            }
            else if (radioButton2.Checked == true)
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
        }
        /// <summary>
        /// 获取文件的名字（无扩展名）
        /// </summary>
        string getFileName(string a , string b)
        {
            a = a.Replace(b, "");
            return a;
        }
        /// <summary>
        /// 对未来日记文件夹进行遍历
        /// </summary>
        void traverseFutureDictory()
        {
            //先遍历文件夹，找到对应的文件夹
            //在对应的文件夹里面遍历文件
            DirectoryInfo folder = new DirectoryInfo(path);
            foreach (var i in folder.GetDirectories())
            {
                if (i.Name == "未来日记")
                {
                    int n = 0;
                    tempPath = path + "/未来日记";
                    DirectoryInfo f = new DirectoryInfo(tempPath);
                    double time = double.Parse(DateTime.Now.ToString("yyyyMMdd"));
                    string temp1 = tempPath + "/" + DateTime.Now.ToString("yyyyMMdd");
                    double create_time = time+1;
                    //在文件夹里面对文件进行遍历
                    foreach (var j in f.GetFiles())
                    {
                        string filename = j.Name;
                        filename=getFileName(filename, j.Extension);
                        //判断是否阅读过
                        try
                        {
                            create_time = double.Parse(filename);
                            fut.Add(j.Name);
                            n++;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    if (time >= create_time)
                    {
                        label9.Text = "你有" + n + "封未来邮件！请接收！";
                        pathForFuture = tempPath + "/" + fut[0];
                    }
                }
            }
        }
        //年月日的判断
        void monthJudgement(int M)
        {
            if (m == 1 || m == 3 || m == 5 || m == 7 || m == 8 || m == 10 || m == 12)
            {
                if (d > 31)
                {
                    d = d-31;
                    if (M > 12)
                    {
                        m = 1;
                    }
                    y = y + 1;
                }
            }
            else
            {
                if (d > 30)
                {
                    d = d-30;
                    if (M > 12)
                    {
                        m = 1;
                    }
                    y = y + 1;
                }
            }
            
        }

        //*******************************
        //    MainOperation主要操作
        //*******************************

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            string a;
            string text = richTextBox1.Text;
            saveJudge(out a, text);
            if (checkBoxOpen.Checked == true)
            {
                string path3 = path + "/未来日记/";
                //完善了代码
                createDirectory(path3);
                path3 += textBoxYear.Text + textBoxMonth.Text + textBoxDay.Text + ".doc";
                create_write_File(path3, text);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                path = fd.SelectedPath;
                MessageBox.Show("保存路径成功！");
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("ch-zh")); //显示标签为中文
            label3.Text = DateTime.Now.ToString("yyyy.MM.dd dddd");
            richTextBox1.AcceptsTab = true;        //接收制表符
            traverseFutureDictory();//遍历未来日记文件夹
            
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

        //*******************************
        //            Paint
        //*******************************
        private void button3_Paint(object sender, PaintEventArgs e)
        {
            DrawControls.Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(210, 210, 210), Color.FromArgb(242, 242, 242));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("路径设置", new Font("微软雅黑", 9, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(10, 10));
        }

        private void button4_Paint(object sender, PaintEventArgs e)
        {
            DrawControls.Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(210, 210, 210), Color.FromArgb(242, 242, 242));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("查看日记", new Font("微软雅黑", 9, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(10, 10));
        }

        private void button5_Paint(object sender, PaintEventArgs e)
        {
            DrawControls.Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(210, 210, 210), Color.FromArgb(242, 242, 242));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("打开日记本", new Font("微软雅黑", 9, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(10, 10));
        }

        private void btnSave_Paint(object sender, PaintEventArgs e)
        {
            DrawControls.Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(210, 210, 210), Color.FromArgb(242, 242, 242));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("保存日记(Alt+s)", new Font("微软雅黑", 9, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(10, 10));
        }

        private void button2_Paint(object sender, PaintEventArgs e)
        {
            DrawControls.Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(210, 210, 210), Color.FromArgb(242, 242, 242));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("清空", new Font("微软雅黑", 9, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(10, 10));
        }

        private void button6_Paint(object sender, PaintEventArgs e)
        {
            DrawControls.Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(113, 113, 113), Color.FromArgb(0, 0, 0));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("-", new Font("微软雅黑", 13, FontStyle.Regular), new SolidBrush(Color.White), new PointF(10, 10));
        }

        private void button7_Paint(object sender, PaintEventArgs e)
        {
            DrawControls.Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(113, 113, 113), Color.FromArgb(0, 0, 0));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("×", new Font("微软雅黑", 13, FontStyle.Regular), new SolidBrush(Color.White), new PointF(10, 10));
        }

        //*******************************
        //           Keydown
        //*******************************

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D3&&e.Alt)
            {
                textBox1.Focus();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode ==Keys.Enter)
            {
                richTextBox1.Focus();
            }
        }

        //*******************************
        //         CheckedChanged
        //*******************************
        private void checkBoxOpen_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOpen.Checked ==true)
            {
                y = int.Parse(DateTime.Now.ToString("yyyy"));
                m = int.Parse(DateTime.Now.ToString("MM"));
                d = int.Parse(DateTime.Now.ToString("dd"));
                h = int.Parse(DateTime.Now.ToString("HH"));
                try
                {
                    var s = listBox1.SelectedItem.ToString();
                    if (s == "一天")
                    {
                        d = d + 1;

                    }
                    if (s == "一星期")
                    {
                        d = d + 7;
                        monthJudgement(m);
                    }
                    if (s == "一个月")
                    {
                        m = m + 1;
                        monthJudgement(m);
                    }
                    if (s == "一年")
                    {
                        y = y + 1;
                    }
                }
                catch
                {

                }
                
                textBoxDay.Text = d.ToString();
                textBoxMonth.Text = m.ToString();
                textBoxYear.Text = y.ToString();
                textBoxHour.Text = h.ToString();
            }
        }
        //*******************************
        //             Click
        //*******************************
        private void label9_Click(object sender, EventArgs e)     //未来日记
        {
            try
            {
                StreamReader sr = new StreamReader(pathForFuture, Encoding.Default);
                string title = getFileName(pathForFuture, ".doc");
                while (!sr.EndOfStream)
                {
                    richTextBox1.Text = sr.ReadToEnd();
                    textBox1.Text = title;
                }
                sr.Close();
                File.Move(pathForFuture, title + "(已阅)" + ".doc");
                if (fut_n < fut.Count - 1)
                {
                    fut_n++;
                    pathForFuture = fut[fut_n];
                }
            }
            catch
            {
                label9.Text = "你没有邮件可以接收哦~ ";
            }
           
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            textBox1.Text = "";
        }
        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        //
        //others
        //

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
    }
}
