using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class IniReader
{
    Dictionary<string, string> d;

    public void Load(string path)
    {
        d = new Dictionary<string, string>();

        string text = File.ReadAllText(path);
        string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                continue;

            string[] kv = line.Split('=');
            if (kv.Length != 2)
                continue;

            string k = kv[0];
            string v = kv[1];
            if (!d.ContainsKey(k))
                d.Add(k, v);
        }
    }

    public string Get(string k)
    {
        if (d.ContainsKey(k))
            return d[k];
        else
            return null;
    }
}

public class PackerConfig
{
    string currDir;
    public PackerConfig()
    {
        currDir = Directory.GetCurrentDirectory();
    }

    IniReader ini;
    public void Load(string path)
    {
        ini = new IniReader();
        ini.Load(path);
    }

    public string Get(string k)
    {
        return ini.Get(k);
    }

    public string PAndroid = "Android";
    public string PIOS = "IOS";
    public string PWindows = "Windows";
    public string ProgramPath
    {
        get
        {
            return currDir + "\\..";
        }
    }
    public string FunParamFullPath
    {
        get
        {
            return currDir + "\\Build\\fun_param.txt";
        }
    }
    public string Java1_8ParamFullPath
    {
        get
        {
            return currDir + "\\Build\\Java1_8.txt";
        }
    }
    public string Java1_7ParamFullPath
    {
        get
        {
            return currDir + "\\Build\\Java1_7.txt";
        }
    }
}