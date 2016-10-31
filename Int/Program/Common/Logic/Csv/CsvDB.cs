using System.Collections;
using System.Collections.Generic;
/// <summary>
/// CSV数据容器
/// </summary>
public class CsvDB
{
    #region 解析文件名

    // csv表文件名
    public static string[] arrCsvName = 
    {
        "def.csv",
    };

    // txt文件名
    public static string[] arrTxtName =
    {
        "version.txt",
    };

    #endregion

    #region csv表数据

    public static Dictionary<int, object> Items { get; private set; }

    #endregion
}
