using System;
using System.Collections.Generic;
using Swift;

namespace Server
{
    // 服务器对象
    public class Server : Core
    {
        // 服务器逻辑真间隔（毫秒）
        public int Interval = 50;

        public Server(string[] args)
        {
            CsScriptShell<ScriptObject> css = new CsScriptShell<ScriptObject>();
            {
                DynamicScriptProvider<ScriptObject> dsp = new DynamicScriptProvider<ScriptObject>();
                css.DSP = dsp;
            }
            Add("CsScriptShell", css);
            css.DSP.AddNamespace("Server");
            css.DSP.AddAssembly(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // 执行启动脚本
            foreach (string cfg in args)
                css.RunScript(cfg, "init", this);
        }

        // 启动服务器
        public void Start()
        {
            Console.WriteLine("server starting ...");
            Console.ReadLine();
        }

        // 停止服务器
        public void Stop()
        { 
        
        }
    }
}
