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
    public class Packer
    {
        public static string UnityPath = null;
        public static string ProgramPath = null;
        public static string version = "";

        public const string PAndroid = "Android";
        public const string PIOS = "IOS";
        public const string PWindows = "Windows";

        public static PackerConfig Cfg;

        public static string[] Steps_Android_Update = new string[]
        { 
            "deleteExportedCsvScript",          // 删除 CsvAllDataAllText.cs
            "switchPlatform",                   // 切换平台
            "setVersion_exportCsv",                        // 设置版本号、导表

            // 安卓特殊步骤
            "android_buildPlayer_copyDll",      // build player temp

            //"changeFont",                       // 修正字体
            "buildAbs",
            "compress1",
        };

        public static string[] Steps_Android_Full_Simple = new string[]
        {
            "deleteExportedCsvScript",          // 删除 CsvAllDataAllText.cs
            "switchPlatform",                   // 切换平台

            "defineSymbolsForSimpleApk",
            "setVersion_exportCsv",             // 设置版本号、导表
            
            //"changeFont",                       // 修正字体
            "buildAbs_compress1_compress2",     // 
            "buildPlayer",                      // build player

            "deleteSymbolsForSimpleApk",
        };

        public static string[] Steps_Android_Full = new string[]
        {
            "setJavaPath",

            "deleteExportedCsvScript",          // 删除 CsvAllDataAllText.cs
            "switchPlatform",                   // 切换平台
            "setVersion_exportCsv",             // 设置版本号、导表
            
            // 安卓特殊步骤
            "android_buildPlayer_copyDll",      // build player temp

            //"changeFont",                       // 修正字体
            "buildAbs_compress1_compress2",     // 

            "setGameNameXXBL",
            "setSDKWanDouJia",
            "buildPlayer",                      // build player
            "android_decodeApk_replaceMono_zeroHash_encodeApk_signApk",

            //"setGameNameMsjnh",
            //"setSDKWanDouJia",
            //"buildPlayer",                      // build player
            //"android_decodeApk_replaceMono_zeroHash_encodeApk_signApk",

            //"setGameNameYxjnh",
            //"setSDKWanDouJia",
            //"buildPlayer",                      // build player
            //"android_decodeApk_replaceMono_zeroHash_encodeApk_signApk",

            // 华为
            // 在这几步之前需要先处理 Plugins 下的文件
            //"setGameNameYxjnh",
            //"setSDKYiJie",
            //"buildPlayer",                      // build player
            //"android_decodeApk_replaceMono_zeroHash_encodeApk_signApk",
        };
        public static string[] Steps_Android_OnlyScript = new string[]
        {
            // 安卓特殊步骤
            "android_buildPlayer_copyDll",      // build player temp
            "android_buildScript",
        };

        public static string[] Steps_IOS_Update = new string[]
        { 
            "deleteExportedCsvScript",          // 删除 CsvAllDataAllText.cs
            "switchPlatform",                   // 切换平台
            "setVersion_exportCsv",                        // 设置版本号、导表

            //"changeFont",                       // 修正字体

            "buildAbs",
            "compress1",
        };
        public static string[] Steps_IOS_Full = new string[]
        { 
            "deleteExportedCsvScript",          // 删除 CsvAllDataAllText.cs
            "switchPlatform",                   // 切换平台
            "setVersion_exportCsv",                        // 设置版本号、导表

            //"changeFont",                       // 修正字体

            "buildAbs_compress1_compress2",     // 

            // 越狱用这2项
            "setGameNameMsjnh",
            "setSDKYiJie", 

            // iOS官方用这2项
            //"setGameNameYxjnh",
            //"setSDKIOS",

            "buildPlayer",                      // build player

            // iOS特有步骤
            "modifyInfoPlist"
        };

        public static string[] Steps_Windows_Full = new string[]
        { 
            "deleteExportedCsvScript",          // 删除 CsvAllDataAllText.cs
            "switchPlatform",                   // 切换平台
            "setVersion_exportCsv",                        // 设置版本号、导表

            //"changeFont",                       // 修正字体

            "buildAbs_compress1_compress2",     // 

            "setSDKNone",
            "buildPlayer",                      // build player
        };

        static void MakeSureFilePathExist(string path)
        {
            int i = path.LastIndexOf('\\');            
            int j = path.LastIndexOf('/');

            int x = (i >= j ? i : j);
            if (x >= 0)
                Directory.CreateDirectory(path.Substring(0, x));
            else
                Directory.CreateDirectory(path);
        }

        // 安全写入文件（保证目录一定存在）
        public static void WriteAllTextSafe(string path, string content)
        {
            MakeSureFilePathExist(path);
            File.WriteAllText(path, content);
        }

        // 设置要传递给 Unity 函数的参数
        static void SetFunParam(string platform, string param)
        {
            //string dir = string.Format("{0}\\Client{1}\\PackerTemp", ProgramPath, platform);
            //Directory.CreateDirectory(dir);            

            //File.WriteAllText(dir + "\\fun_param.txt", param);

            WriteAllTextSafe(Cfg.FunParamFullPath, param);
        }

        public static void Pack(Action<string, int, int, string> updateProgress, string platform, string[] steps)
        {
            if (string.IsNullOrEmpty(version))
            {
                MessageBox.Show("请填写版本号");
                return;
            }

            for (int i = 0; i < steps.Length; i++)
            {
                string step = steps[i];

                updateProgress(platform, i, steps.Length, step);

                //if (step == MakeTargetFolder)
                //{
                //    DoMakeTargetFolder(platform);
                //}
                //else 

                if (step == "setJavaPath")
                {
                    File.WriteAllText(Cfg.Java1_8ParamFullPath, Cfg.Get("Java1_8"));
                    File.WriteAllText(Cfg.Java1_7ParamFullPath, Cfg.Get("Java1_7"));
                }
                else if (step == "deleteExportedCsvScript")
                {
                    deleteExportedCsvScript(platform);
                }
                else if (step == "modifyInfoPlist")
                {
                    // IOS有一个特殊的步骤
                    if (!DoInfoPlistModifier())
                        MessageBox.Show("执行 InfoPlistModifier 失败");
                }
                else
                {
                    // 有些步骤需要传递参数
                    if (step == "setVersion_exportCsv")
                    {
                        SetFunParam(platform, version);
                    }
                    else if (step == "android_decodeApk_replaceMono_zeroHash_encodeApk_signApk" ||
                        step == "signApk")
                    {
                        SetFunParam(platform, Cfg.Get("JarSigner"));
                    }

                    DoAStep(platform, i, step);
                }

                updateProgress(platform, i + 1, steps.Length, "done."/*step*/);
            }
        }

        //public static void PackAndroid(Action<int> p, bool tip)
        //{
        //    PrepareClientFolders("Android");
        //    p(0);

        //    if (File.Exists(Packer.ProjectPath + ".\\Build\\Nova.apk"))
        //        File.Delete(Packer.ProjectPath + ".\\Build\\Nova.apk");

        //    SetClientVersion("Android", version);
        //    DeleteCsvData("Android");

        //    p(1);
        //    int stepCount = 5;
        //    for (int i = 0; i < stepCount; i++)
        //    {
        //        PackClient("Android", i);
        //        p((i + 1) * 100 / stepCount);
        //    }

        //    p(100);
        //}

        public static bool DoInfoPlistModifier()
        {
            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = "InfoPlistModifier.exe";
            ps.Arguments = "Build\\Client\\IOS\\xcodeproject\\Info.plist " + version;
            ps.RedirectStandardOutput = true;
            ps.RedirectStandardError = true;
            ps.UseShellExecute = false;
            Process p = new Process();
            p.StartInfo = ps;
            return p.Start();
        }

        //public static void PackIOS(Action<int> p, bool tip)
        //{
        //    PrepareClientFolders("IOS");
        //    p(0);

        //    if (Directory.Exists(Packer.ProjectPath + ".\\Build\\xcodeproject"))
        //        Directory.Delete(Packer.ProjectPath + ".\\Build\\xcodeproject", true);

        //    SetClientVersion("IOS", version);
        //    DeleteCsvData("IOS");

        //    p(1);
        //    int stepCount = 5;
        //    for (int i = 0; i < stepCount; i++)
        //    {
        //        PackClient("IOS", i);
        //        p((i + 1) * 100 / stepCount);
        //    }

            
        //    if (!DoInfoPlistModifier())
        //        MessageBox.Show("执行 InfoPlistModifier 失败");

        //    p(100);
        //}

        //public static void PackWindows(Action<int> p, bool tip)
        //{
        //    PrepareClientFolders("Windows");
        //    p(0);

        //    if (File.Exists(Packer.ProjectPath + ".\\Build\\Nova.exe"))
        //        File.Delete(Packer.ProjectPath + ".\\Build\\Nova.exe");

        //    if (Directory.Exists(Packer.ProjectPath + ".\\Build\\Nova_Data"))
        //        Directory.Delete(Packer.ProjectPath + ".\\Build\\Nova_Data", true);

        //    SetClientVersion("Windows", version);
        //    DeleteCsvData("Windows");

        //    p(1);
        //    int stepCount = 5;
        //    for (int i = 0; i < stepCount; i++)
        //    {
        //        PackClient("Windows", i);
        //        p((i + 1) * 100 / stepCount);
        //    }

        //    p(100);
        //}

        public static bool UpdateSVN()
        {
            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = "svn.exe";
            ps.WorkingDirectory = Packer.ProgramPath;
            ps.Arguments = "update";
            ps.RedirectStandardOutput = true;
            ps.RedirectStandardError = true;
            ps.UseShellExecute = false;
            Process p = new Process();
            p.StartInfo = ps;
            if (!p.Start())
                return false;

            p.WaitForExit();

            string str = p.StandardOutput.ReadLine();
            while (str != null)
            {
                if (str.IndexOf("Error") >= 0)
                    return false;
                else if (str.IndexOf("At revision") >= 0)
                    return false;
                else if (str.IndexOf("Update to revision") >= 0)
                    return true;

                str = p.StandardOutput.ReadLine();
            }

            return false;
        }

        //public static bool CopyAllCsvCs(string from, string to)
        //{
        //    string fromPath = "" + ProjectPath + "\\Client" + from + "\\Assets\\Scripts\\Logic\\Csv\\CsvAllDataAllText.cs";
        //    string toPath = "" + ProjectPath + "\\Client" + to + "\\Assets\\Scripts\\Logic\\Csv\\CsvAllDataAllText.cs";
        //    if (File.Exists(toPath))
        //        File.Delete(toPath);

        //    File.Copy(fromPath, toPath);
        //    return File.Exists(toPath);
        //}

        //public static bool ClearAndExportAllCsvText(string platform)
        //{
        //    ProcessStartInfo ps = new ProcessStartInfo();
        //    ps.FileName = UnityPath;
        //    ps.Arguments = "-batchmode -nographics -quit -logFile .\\packlog_all.txt -projectPath \"" + ProjectPath + "\\Client" + platform + "\" -executeMethod AutoBuild.ClearAndExportAllCsvText";

        //    Process p = new Process();
        //    p.StartInfo = ps;

        //    if (!p.Start())
        //        return false;

        //    p.WaitForExit();

        //    return File.Exists(ProjectPath + "\\config\\all.csv");
        //}

        //public static void SetClientVersion(string platform, string ver)
        //{
        //    // 跟 Unity 里的代码约定好
        //    string dir = string.Format("{0}\\Client{1}\\PackerTemp", ProjectPath, platform);
        //    Directory.CreateDirectory(dir);
        //    File.WriteAllText(dir + "\\client_version.csv", ver);
        //}
        public static void deleteExportedCsvScript(string platform)
        {
            string p = string.Format("..\\Client{0}\\Assets\\Scripts\\Logic\\Csv\\CsvAllDataAllText.cs", platform);
            if (File.Exists(p))
                File.Delete(p);
        }

        //public static bool PackClient(string platform, int step)
        //{
        //    if (!File.Exists(ProjectPath + "\\config\\all.csv"))
        //    {
        //        MessageBox.Show("找不到 all.csv，请先成功执行导表操作");
        //        return false;
        //    }

        //    ProcessStartInfo ps = new ProcessStartInfo();
        //    ps.FileName = UnityPath;
        //    ps.Arguments = "-batchmode -nographics -quit -logFile .\\packlog_" + platform + "_" + step + ".txt -projectPath \"" + ProjectPath + "\\Client" + platform + "\" -executeMethod AutoBuild.Build" + platform + "_" + step;

        //    Process p = new Process();
        //    p.StartInfo = ps;

        //    if (!p.Start())
        //        return false;

        //    p.WaitForExit();
        //    return true;
        //}

        public static bool DoAStep(string platform, int index, string step)
        {
            

            Directory.CreateDirectory("Build\\Log");

            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = UnityPath;
            //ps.Arguments = "-batchmode -nographics -quit -logFile .\\packlog_" + platform + "_" + step + ".txt -projectPath \"" + ProjectPath + "\\Client" + platform + "\" -executeMethod AutoBuild.Step_" + step;
            ps.Arguments = string.Format("-batchmode -nographics -quit -logFile Build\\Log\\{3}_{0}_{1}.txt -projectPath \"{2}\\Client{0}\" -executeMethod AutoBuild2.Step_{1}",
                platform, step, ProgramPath, index + 1);

            Process p = new Process();
            p.StartInfo = ps;

            if (!p.Start())
                return false;

            p.WaitForExit();
            if (0 != p.ExitCode)
            {
                MessageBox.Show("出错了！platform = " + platform + "\n step = " + step);
                Process.GetCurrentProcess().Kill();
            }
            return true;
        }

        public static void PrepareServerFolders()
        {
            if (!Directory.Exists("..\\Build\\Server"))
                Directory.CreateDirectory("..\\Build\\Server");

            if (Directory.Exists("..\\Build\\Server\\Config"))
                Directory.Delete("..\\Build\\Server\\Config", true);

            Directory.CreateDirectory("..\\Build\\Server\\Config");

            if (File.Exists("..\\Build\\Server\\Server.exe"))
                File.Delete("..\\Build\\Server\\Server.exe");

            if (File.Exists("..\\Build\\Server\\mysql.data.dll"))
                File.Delete("..\\Build\\Server\\mysql.data.dll");
        }

        //public static void DoMakeTargetFolder(string platform)
        //{
        //    Directory.CreateDirectory("Build\\Client");
        //    Directory.CreateDirectory("Build\\Client\\" + platform);
        //}

        public static void CompileServer()
        {
            // todo

            CopyServerFile();
        }

        static void CopyServerFile()
        {
            // Server
            PrepareServerFolders();
            File.Copy("..\\Server\\Server\\bin\\Debug\\Server.exe", "..\\Build\\Server\\Server.exe");
            File.Copy("..\\Server\\Server\\bin\\Debug\\mysql.data.dll", "..\\Build\\Server\\mysql.data.dll");
            
            string[] fs1 = Directory.GetFiles("..\\Config", "*.csv");
            string[] fs2 = Directory.GetFiles("..\\Config", "*.sc");
            string[] fs3 = Directory.GetFiles("..\\Config", "*.txt");
            IEnumerable<string> fs = fs1.Concat(fs2).Concat(fs3);
            foreach (string f in fs)
                File.Copy(f, "..\\Build\\Server\\Config\\" + f.Substring(f.LastIndexOf("\\") + 1));
        }
    }
}
