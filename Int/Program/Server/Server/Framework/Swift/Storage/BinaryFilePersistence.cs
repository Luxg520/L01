using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Swift;

namespace Swift
{
    /// <summary>
    /// 二进制本地文件实现持久化
    /// </summary>
    public class BinaryFilePersistence<T, IDType> : CachedAsyncPersistence<T, IDType, byte[]> where T : DataItem<IDType>
    {
        Func<byte[], T> hb2d = null;

        // 构造器，需要指明数据映射工具
        public BinaryFilePersistence(string dirPath, Action<T, IWriteableBuffer> data2BufferHandler, Func<IReadableBuffer, T> buff2DataHandler)
            : base((T obj) =>
                {
                    WriteBuffer buff = new WriteBuffer();
                    data2BufferHandler(obj, buff);
                    return buff.Data;
                },
                (byte[] buff) =>
                {
                    RingBuffer data = new RingBuffer();
                    if (buff == null)
                        return buff2DataHandler(null);
                    else
                    {
                        data.Write(buff, 0, buff.Length);
                        return buff2DataHandler(data);
                    }
                }
                )
        {
            dir = dirPath;
            hb2d = (byte[] buff) =>
            {
                RingBuffer data = new RingBuffer();
                if (buff == null)
                    return buff2DataHandler(null);
                else
                {
                    data.Write(buff, 0, buff.Length);
                    return buff2DataHandler(data);
                }
            };
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        #region 需要继承实现的部分

        // 将缓冲同步落地保存
        protected override void AddImpl(IDType id, T d, byte[] buff)
        {
            string f = Path.Combine(dir, Utils1.MD5(id.ToString()));
            string tmpF = f + ".tmp";
            BinaryWriter w = new BinaryWriter(new FileStream(tmpF, FileMode.Create));
            w.Write(buff, 0, buff.Length);
            w.Close();

            if (File.Exists(f))
                File.Delete(f);

            File.Move(tmpF, f);
        }

        // 将缓冲同步落地保存
        protected override void UpdateImpl(IDType id, T d, byte[] buff)
        {
            string f = Path.Combine(dir, Utils1.MD5(id.ToString()));
            string tmpF = tmpF = f + ".tmp";
            BinaryWriter w = new BinaryWriter(new FileStream(tmpF, FileMode.Create));
            w.Write(buff, 0, buff.Length);
            w.Close();

            if (File.Exists(f))
                File.Delete(f);

            File.Move(tmpF, f);
        }

        // 同步删除指定 id 的数据
        protected override void DeleteImpl(IDType id)
        {
            string f = Path.Combine(dir, Utils1.MD5(id.ToString()));
            File.Delete(f);
        }

        // 同步加载指定 id 的数据
        protected override byte[] LoadImpl(IDType id)
        {
            string f = Path.Combine(dir, Utils1.MD5(id.ToString()));
            if (File.Exists(f))
            {
                BinaryReader r = new BinaryReader(new FileStream(f, FileMode.Open));
                byte[] data = r.ReadBytes((int)r.BaseStream.Length);
                r.Close();
                return data;
            }
            else
                return null;
        }

        // 同步加载所有数据
        protected override byte[][] LoadAllImpl(string whereClause)
        {
            Debug.Assert(whereClause == null);
            string[] files = Directory.GetFiles(dir);
            byte[][] dataArr = new byte[files.Length][];
            for (int i = 0; i < files.Length; i++)
            {
                string f = files[i];
                BinaryReader r = new BinaryReader(new FileStream(f, FileMode.Open));
                byte[] data = r.ReadBytes((int)r.BaseStream.Length);
                r.Close();
                dataArr[i] = data;
            }

            return dataArr;
        }

        // 同步加载所有数据
        protected override IDType[] LoadAllIDImpl(string whereClause)
        {
            Debug.Assert(whereClause == null);
            string[] files = Directory.GetFiles(dir);
            List<IDType> idList = new List<IDType>();
            
            for (int i = 0; i < files.Length; i++)
            {
                string f = files[i];
                BinaryReader r = new BinaryReader(new FileStream(f, FileMode.Open));
                byte[] bs = r.ReadBytes((int)r.BaseStream.Length);
                r.Close();

                T data = hb2d(bs);

                string[] tp = whereClause.Split('#');
                Type type = data.GetType();
                MethodInfo idm = type.GetMethod("ID");
                if (tp == null || tp.Length < 3)
                {
                    idList.Add((IDType)idm.Invoke(data, null));
                    continue;
                }
                bool ok = false;
                for (int j = 0; j < tp.Length / 3; j++)
                {
                    MethodInfo method = type.GetMethod(tp[j * 3 + 0]);
                    int n = (int)method.Invoke(data, null);
                    string op = tp[j * 3 + 1];
                    int param;
                    int.TryParse(tp[j * 3 + 2], out param);
                    switch (op)
                    {
                        case "<":
                            if (n < param)
                            {
                                ok = true;
                            }
                            else
                            {
                                ok = false;
                            }
                            break;
                        case "<=":
                             if (n < param)
                             {
                                 ok = true;
                             }
                             else
                             {
                                 ok = false;
                             }
                            break;
                        case ">":
                             if (n < param)
                             {
                                 ok = true;
                             }
                             else
                             {
                                 ok = false;
                             }
                            break;
                        case ">=":
                             if (n < param)
                             {
                                 ok = true;
                             }
                             else
                             {
                                 ok = false;
                             }
                            break;
                        case "==":
                             if (n < param)
                             {
                                 ok = true;
                             }
                             else
                             {
                                 ok = false;
                             }
                            break;
                        default:
                            break;
                    }
                }
                if (ok)
                {
                    idList.Add((IDType)idm.Invoke(data, null));
                }
            }

            return idList.ToArray();
        }

        #endregion

        #region 保护部分

        // 存储目录
        string dir = null;

        #endregion
    }
}
