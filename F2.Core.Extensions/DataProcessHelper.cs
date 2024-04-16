using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace F2.Core.Extensions
{
    /// <summary>
    /// 数据转换
    /// </summary>
    public class DataProcessHelper
    {
        /// <summary>
        /// 将DataSet格式化成字节数组byte[]
        /// </summary>
        /// <param name="dsOriginal">DataSet对象</param>
        /// <returns>字节数组</returns>
        public static byte[] GetBinaryFormatData(DataSet dsOriginal)
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();
            dsOriginal.RemotingFormat = SerializationFormat.Binary;
            brFormatter.Serialize(memStream, dsOriginal);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return binaryDataResult;
        }

        /// <summary>
        /// 将DataSet格式化成字节数组byte[]，并且已经经过压缩
        /// </summary>
        /// <param name="dsOriginal">DataSet对象</param>
        /// <returns>字节数组</returns>
        public static byte[] GetBinaryFormatDataCompress(DataSet dsOriginal)
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();
            dsOriginal.RemotingFormat = SerializationFormat.Binary;
            brFormatter.Serialize(memStream, dsOriginal);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return Compress(binaryDataResult);
        }

        /// <summary>
        /// 解压数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            byte[] bData;
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Position = 0;
            GZipStream stream = new GZipStream(ms, CompressionMode.Decompress, true);
            byte[] buffer = new byte[1024];
            MemoryStream temp = new MemoryStream();
            int read = stream.Read(buffer, 0, buffer.Length);
            while (read > 0)
            {
                temp.Write(buffer, 0, read);
                read = stream.Read(buffer, 0, buffer.Length);
            }
            //必须把stream流关闭才能返回ms流数据,不然数据会不完整
            stream.Close();
            stream.Dispose();
            ms.Close();
            ms.Dispose();
            bData = temp.ToArray();
            temp.Close();
            temp.Dispose();
            return bData;
        }

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] data)
        {
            byte[] bData;
            MemoryStream ms = new MemoryStream();
            GZipStream stream = new GZipStream(ms, CompressionMode.Compress, true);
            stream.Write(data, 0, data.Length);
            stream.Close();
            stream.Dispose();
            //必须把stream流关闭才能返回ms流数据,不然数据会不完整
            //并且解压缩方法stream.Read(buffer, 0, buffer.Length)时会返回0
            bData = ms.ToArray();
            ms.Close();
            ms.Dispose();
            return bData;
        }

        /// <summary>
        /// 将字节数组反序列化成DataSet对象
        /// </summary>
        /// <param name="binaryData">字节数组</param>
        /// <returns>DataSet对象</returns>
        public static DataSet RetrieveDataSet(byte[] binaryData)
        {
            DataSet ds = null;
            MemoryStream memStream = new MemoryStream(binaryData, true);
            //byte[] bs = memStream.GetBuffer();

            // memStream.Write(bs, 0, bs.Length);
            //memStream.Seek(0, SeekOrigin.Begin);

            IFormatter brFormatter = new BinaryFormatter();
            ds = (DataSet)brFormatter.Deserialize(memStream);
            return ds;
        }

        /// <summary>
        /// 将字节数组反解压后序列化成DataSet对象
        /// </summary>
        /// <param name="binaryData">字节数组</param>
        /// <returns>DataSet对象</returns>
        public static DataSet RetrieveDataSetDecompress(byte[] binaryData)
        {
            DataSet dsOriginal = null;
            MemoryStream memStream = new MemoryStream(Decompress(binaryData));
            IFormatter brFormatter = new BinaryFormatter();
            Object obj = brFormatter.Deserialize(memStream);
            dsOriginal = (DataSet)obj;
            return dsOriginal;
        }

        /// <summary>
        /// 将object格式化成字节数组byte[]
        /// </summary>
        /// <param name="dsOriginal">object对象</param>
        /// <returns>字节数组</returns>
        public static byte[] GetBinaryFormatData(object dsOriginal)
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();
            brFormatter.Serialize(memStream, dsOriginal);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return binaryDataResult;
        }

        /// <summary>
        /// 将objec格式化成字节数组byte[]，并压缩
        /// </summary>
        /// <param name="dsOriginal">object对象</param>
        /// <returns>字节数组</returns>
        public static byte[] GetBinaryFormatDataCompress(object dsOriginal)
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();
            brFormatter.Serialize(memStream, dsOriginal);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return Compress(binaryDataResult);
        }

        /// <summary>
        /// 将字节数组反序列化成object对象
        /// </summary>
        /// <param name="binaryData">字节数组</param>
        /// <returns>object对象</returns>
        public static object RetrieveObject(byte[] binaryData)
        {
            MemoryStream memStream = new MemoryStream(binaryData);
            IFormatter brFormatter = new BinaryFormatter();
            Object obj = brFormatter.Deserialize(memStream);
            return obj;
        }

        /// <summary>
        /// 将字节数组解压后反序列化成object对象
        /// </summary>
        /// <param name="binaryData">字节数组</param>
        /// <returns>object对象</returns>
        public static object RetrieveObjectDecompress(byte[] binaryData)
        {
            MemoryStream memStream = new MemoryStream(Decompress(binaryData));
            IFormatter brFormatter = new BinaryFormatter();
            Object obj = brFormatter.Deserialize(memStream);
            return obj;
        }

        ///// <summary>
        ///// 解密配置文件并读入到xmldoc中
        ///// </summary>
        //public static XmlNode DecryptConfigFile(string filePath)
        //{
        //    FileStream fs = new FileStream(filePath, FileMode.Open);
        //    XmlDocument m_XmlDoc = new XmlDocument();
        //    BinaryFormatter formatter = null;
        //    try
        //    {
        //        formatter = new BinaryFormatter();
        //        //   Deserialize   the   hashtable   from   the   file   and
        //        //   assign   the   reference   to   the   local   variable.
        //        m_XmlDoc.LoadXml(KCEncrypt.Decrypt((string)formatter.Deserialize(fs)));
        //        return m_XmlDoc.DocumentElement;
        //    }
        //    catch (SerializationException e)
        //    {
        //        Console.WriteLine("Failed   to   deserialize.   Reason:   " + e.Message);
        //        throw;
        //    }
        //    finally
        //    {
        //        fs.Close();
        //        fs = null;
        //    }
        //}

        ///// <summary>
        ///// 加密密钥后再对文件字符进行加密
        ///// </summary>
        //public static void EncryptConfigFile(string filePath, string str)
        //{
        //    FileStream fs = new FileStream(filePath, FileMode.Create);
        //    BinaryFormatter formatter = new BinaryFormatter();

        //    try
        //    {
        //        formatter.Serialize(fs, KCEncrypt.Encrypt(str));
        //    }
        //    catch (SerializationException e)
        //    {
        //        Console.WriteLine("Failed   to   serialize.   Reason:   " + e.Message);
        //        throw;
        //    }
        //    finally
        //    {
        //        fs.Close();
        //        fs = null;
        //    }
        //}

        /// <summary>
        /// 设置数据行值为Blank：bool：false,string:string.empty,dateTime:DateTime.MinValue,int/decimal:0
        /// </summary>
        /// <param name="dtTable"></param>
        /// <param name="dr"></param>
        public static void SetDataRowValueToBlank(DataTable dtTable, DataRow dr)
        {
            try
            {
                for (int i = 0; i < dtTable.Columns.Count; i++)
                {
                    if (dtTable.Columns[i].AllowDBNull)
                    {
                        System.Type mType = dtTable.Columns[i].DataType;

                        if (mType == typeof(int))
                            dr[i] = 0;
                        else if (mType == typeof(string))
                            dr[i] = string.Empty;
                        else if (mType == typeof(bool))
                            dr[i] = false;
                        else if (mType == typeof(DateTime))
                            dr[i] = DateTime.Parse("1900/01/01");
                        else if (mType == typeof(decimal) || mType == typeof(float) || mType == typeof(double))
                            dr[i] = 0;
                    }
                }

            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 按指定条件获取过滤的数据表（模糊搜索）
        /// </summary>
        /// <param name="dtSrc"></param>
        /// <param name="condition">指定的条件如：CustCode like '%test%' or CustName like '%test%'</param>
        /// <returns></returns>
        public static DataTable GetDataTableSearch(DataTable dtSrc, string condition)
        {
            DataTable dtFind = dtSrc.Clone();

            if (condition != "")
            {
                DataRow[] foundRows = dtSrc.Select(condition);
                if (foundRows.Length > 0)
                {
                    foreach (DataRow dr in foundRows)
                    {
                        dtFind.ImportRow(dr);
                    }

                }
            }
            return dtFind;
        }


        /// <summary>
        /// 按指定条件获取过滤的数据表（模糊搜索）
        /// </summary>
        /// <param name="dtSrc"></param>
        /// <param name="searchText">Search Text</param>
        /// <param name="searchColumnNames">字段名列表，如：CustCode,CustName</param>
        /// <returns></returns>
        public static DataTable GetDataTableSearch(DataTable dtSrc, string searchText, string searchColumnNames)
        {
            string condition = "";

            string[] columnNames = searchColumnNames.Split(new char[] { ',' });
            foreach (string c in columnNames)
            {
                if (c != "")
                    condition += " OR " + c + " Like '%" + searchText + "'";

            }

            if (condition != "")
                condition = condition.Substring(4);

            return GetDataTableSearch(dtSrc, condition);
        }


        /// <summary>
        /// 按指定条件获取过滤的数据表（模糊搜索）
        /// </summary>
        /// <param name="dtSrc"></param>
        /// <param name="searchID"></param>
        /// <param name="searchColumnName"></param>
        /// <returns></returns>
        public static DataTable GetDataTableSearch(DataTable dtSrc, int searchID, string searchColumnName)
        {
            DataTable dtFind = dtSrc.Clone();

            DataRow[] foundRows = dtSrc.Select(searchColumnName + "=" + searchID);
            if (foundRows.Length > 0)
            {
                foreach (DataRow dr in foundRows)
                {
                    dtFind.ImportRow(dr);
                }
            }

            return dtFind;
        }

        /// <summary>
        /// 按指定列精确查找指定的值返回符合条件的行，找不到返回NULL（精确查找）
        /// </summary>
        /// <param name="dtSrc"></param>
        /// <param name="searchText"></param>
        /// <param name="searchColumnName"></param>
        /// <returns></returns>
        public static DataRow GetDataRowByCode(DataTable dtSrc, string searchText, string searchColumnName)
        {
            DataRow[] foundRows;

            if (dtSrc == null)
                return null;
            else
            {
                foundRows = dtSrc.Select(searchColumnName + "='" + searchText + "'");
                if (foundRows.Length > 0)
                    return foundRows[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// 按指定列精确查找指定的值返回符合条件的行，找不到返回NULL（精确查找）
        /// </summary>
        /// <param name="dtSrc"></param>
        /// <param name="searchID"></param>
        /// <param name="searchColumnName"></param>
        /// <returns></returns>
        public static DataRow GetDataRowByID(DataTable dtSrc, int searchID, string searchColumnName)
        {
            DataRow[] foundRows;

            if (dtSrc == null)
                return null;
            else
            {
                foundRows = dtSrc.Select(searchColumnName + "=" + searchID);
                if (foundRows.Length > 0)
                    return foundRows[0];
                else
                    return null;
            }
        }


        /// <summary>
        /// 将带有别名的字段名转换成物理表的字段名（即去掉别名）
        /// </summary>
        /// <param name="aliasFieldName"></param>
        /// <returns></returns>
        public static string TransToColumnName(string aliasFieldName)
        {
            int pos = aliasFieldName.IndexOf(".");
            if (pos >= 0)
                return aliasFieldName.Substring(pos + 1);
            else
                return aliasFieldName;
        }


        /// <summary>
        /// 根据父子节点递归删除DataTable表中的数据
        /// </summary>
        /// <param name="dt">需要删除的表</param>
        /// <param name="keyParentField">父节点的字段名</param>
        /// <param name="keyParentValue">要删除父节点的值</param>
        /// <param name="keyIDField">需要删除表的主键</param>
        public static void RecursiveDataTableDelete(DataTable dt, string keyParentField, string keyParentValue, string keyIDField)
        {
            string keyID = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][keyParentField].ToString() == keyParentValue)
                {
                    keyID = dt.Rows[i][keyIDField].ToString();
                    dt.Rows.RemoveAt(i);
                    i = i - 1;
                    //dt.Rows[i].AcceptChanges();
                    RecursiveDataTableDelete(dt, keyParentField, keyID, keyIDField);
                }
            }
        }

        /// <summary>
        /// DataTable转换成实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> GetEntityFromTable<T>(DataTable dt)
        {
            List<T> listObject = new List<T>();
            T entity = default(T);

            foreach (DataRow dr in dt.Rows)
            {
                entity = Activator.CreateInstance<T>();
                foreach (DataColumn dc in dr.Table.Columns)
                {
                    PropertyInfo pi = entity.GetType().GetProperty(dc.ColumnName);
                    if (pi != null)
                    {
                        if (dr[dc.ColumnName] != DBNull.Value)
                            pi.SetValue(entity, dr[dc.ColumnName], null);
                        else
                            pi.SetValue(entity, null, null);
                    }
                }
                listObject.Add(entity);
            }

            return listObject;
        }
    }
}
