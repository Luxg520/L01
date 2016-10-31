using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace NovaPacker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // n范围：[0, N]
        void updateProgress(string platform, int n, int N, string doWhat)
        {
            label2.Text = string.Format("{0}({1}/{2}): {3}", platform, n + 1 > N ? N : n + 1, N, doWhat);
            progressBar1.Maximum = 100;
            progressBar1.Value = (int)((float)n * 100f / (float)N);

            Thread.Sleep(1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Packer.Pack(updateProgress,
            Packer.PAndroid,
            Packer.Steps_Android_Full);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Packer.Pack(updateProgress,
            Packer.PWindows,
            Packer.Steps_Windows_Full);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //bool r = false;
            //if (Packer.ClearAndExportAllCsvText("Windows")
            //    && Packer.CopyAllCsvCs("Windows", "Android")
            //    && Packer.CopyAllCsvCs("Windows", "IOS"))
            //{
            //    MessageBox.Show("成功");
            //}
            //else
            //    MessageBox.Show("导表失败，请检查日志");
        }

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    OpenFileDialog d = new OpenFileDialog();
        //    d.FileName = Packer.UnityPath;
        //    if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        label1.Text = d.FileName;
        //        Packer.UnityPath = d.FileName;
        //        SavePath();
        //    }
        //}

        //private void button5_Click(object sender, EventArgs e)
        //{
        //    FolderBrowserDialog d = new FolderBrowserDialog();
        //    d.SelectedPath = Packer.ProgramPath;
        //    if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        label2.Text = d.SelectedPath;
        //        Packer.ProgramPath = d.SelectedPath;
        //        SavePath();
        //    }
        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPath();
        }

        void LoadPath()
        {
            PackerConfig cfg = Packer.Cfg = new PackerConfig();
            cfg.Load("Config.txt");

            Packer.UnityPath = cfg.Get("UnityPath");
            Packer.ProgramPath = cfg.ProgramPath;
            Packer.version = cfg.Get("Version");

            if (!File.Exists(Packer.UnityPath))
            {
                MessageBox.Show(Packer.UnityPath + "\n不存在！");
                Process.GetCurrentProcess().Kill();
            }

            if (!File.Exists(cfg.Get("JarSigner")))
            {
                MessageBox.Show(cfg.Get("JarSigner") + "\n不存在！");
                Process.GetCurrentProcess().Kill();
            }
            if (!File.Exists(cfg.Get("Java1_8")))
            {
                MessageBox.Show(cfg.Get("Java1_8") + "\n不存在！");
                Process.GetCurrentProcess().Kill();
            }
            if (!File.Exists(cfg.Get("Java1_7")))
            {
                MessageBox.Show(cfg.Get("Java1_7") + "\n不存在！");
                Process.GetCurrentProcess().Kill();
            }

            if (string.IsNullOrEmpty(Packer.version))
            {
                MessageBox.Show("版本号不对！");
                Process.GetCurrentProcess().Kill();
            }
        }

        void SavePath()
        {
            //System.IO.StreamWriter w = new System.IO.StreamWriter("path.txt", false);
            //w.WriteLine(Packer.UnityPath);
            //w.WriteLine(Packer.ProgramPath);
            //w.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Packer.CompileServer();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Packer.UpdateSVN();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Packer.Pack(updateProgress,
            Packer.PIOS,
            Packer.Steps_IOS_Full);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //if (Packer.ClearAndExportAllCsvText("Android"))
            //    MessageBox.Show("成功");
            //else
            //    MessageBox.Show("导表失败，请检查日志");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //if (Packer.ClearAndExportAllCsvText("IOS"))
            //    MessageBox.Show("成功");
            //else
            //    MessageBox.Show("导表失败，请检查日志");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //if (Packer.ClearAndExportAllCsvText("Windows"))
            //    MessageBox.Show("成功");
            //else
            //    MessageBox.Show("导表失败，请检查日志");
        }

        //private void textBox1_TextChanged(object sender, EventArgs e)
        //{
        //    Packer.version = textBox1.Text;
        //}

        private void button12_Click(object sender, EventArgs e)
        {
            if (!Packer.DoInfoPlistModifier())
                MessageBox.Show("执行 InfoPlistModifier 失败");
            else
                MessageBox.Show("执行 InfoPlistModifier 成功");
        }

        // 打包安卓更新包
        private void button13_Click(object sender, EventArgs e)
        {
            Packer.Pack(updateProgress,
            Packer.PAndroid,
            Packer.Steps_Android_Update);
        }

        // 打包IOS更新包
        private void button14_Click(object sender, EventArgs e)
        {
            Packer.Pack(updateProgress,
            Packer.PIOS,
            Packer.Steps_IOS_Update);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Packer.Pack(updateProgress,
                Packer.PAndroid,
                Packer.Steps_Android_Full_Simple);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Packer.Pack(updateProgress,
                Packer.PAndroid,
                Packer.Steps_Android_OnlyScript);
        }
    }
}
