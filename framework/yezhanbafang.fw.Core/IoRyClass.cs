using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Data.Common;
using System.Xml.Linq;
using System.Management;

namespace yezhanbafang.fw.Core
{
    /// <summary>
    /// Ԭ���� 2010-11-1�� �ܽ�֮ǰ��Ŀ����ȡ���õĶ�����
    /// Ԭ���� 2010-11-11 �޸������еķ�������path�ķ����Ͳ���path�ķ�������һ������pathҪ���¶�·����ȡ���ӷ�ʽ�ȣ����������ȡĬ��
    /// 2010-11-23 ����һ�¹�˾�ʼǱ���֮ǰ����Ŀ��Ψһһ���е��ô�������û�ӵ����ķ�������webcatch�ˡ�
    /// ִ�д洢����,��sqlparameter�ķ�ʽ,һ�㷵�����ݼ�  2012-4-17���
    /// 2013-3-27 ������Oracle���ݿ�
    /// 2013-4-17 �����XML����Ҳ�ҽ�ȥ,���Ǹо�ʵ��ûɶ���Լ��ɵ�.д��������
    /// linqtoxml�����ռ�System.Xml.Linq;��ȡ:XElement xe = XElement.Load("XMLFile1.xml");ɾ��Remove();���� xe.Save("XMLFile1.xml");
    /// ѡ��e.Elements("con").Select(x =>x.Attribute("a").Value).ToList();
    /// ����xe.Add(new XElement("con", new XAttribute("a", "4"),new XAttribute("b","ydh")));
    /// 2013-10-25�����˸��ֶ�Oracle��֧�֣�����Oracle�洢����ȡ�÷���ֵ
    /// 20190306 ����������sql���ӳ�ʱʱ���֧��
    /// 2019��11�������˴�������SQL���,���bit���͵�sql���,Ҳ��������sql��ע��
    /// 2020��2��30������oralce���ܷ���֧��provide
    /// 20200302 ����Excel��OlEDB��ʽ
    /// 20200318 ��Excel�������ɱȽ���ͨ�ķ�ʽ ע�� Excel�ķ�ʽ����ִ��Delete���
    /// 20200320 �޸Ĵ�log�ı�Ľṹ,��������xml����ʾ
    /// 20200323 ��������־��ı���IP�ͱ���UUID
    /// 20200401 �Ż���public�ṹ,������DbParameterS��getdatatale�ķ�ʽ�Լ�ִ������ʽ,�����˴�DbParameterS����־֧��
    /// 20200401 ��������,�ṹ���,������2.0�汾
    /// </summary>
    public class IoRyClass
    {
        #region ����

        /// <summary>
        /// ���ݿ�����
        /// </summary>
        private ConType _Contype = ConType.Null;

        /// <summary>
        /// ���ݿ�����
        /// </summary>
        ConType Contype
        {
            get
            {
                if (this._Contype == ConType.Null)
                {
                    this.ReadConString();
                }
                return this._Contype;
            }
            set { this._Contype = value; }
        }

        /// <summary>
        /// ���ݿⳬʱʱ��
        /// </summary>
        int timeout = -1;

        /// <summary>
        /// �����ַ���
        /// </summary>
        string ConString { get; set; }

        /// <summary>
        /// Ĭ��·��
        /// </summary>
        private string _Path = System.AppDomain.CurrentDomain.BaseDirectory + "constring.xml";

        /// <summary>
        /// ���û��߻�ȡĬ��·��
        /// </summary>
        string Path
        {
            get { return this._Path; }
            set { this._Path = value; }
        }

        static string _PCIP = null;
        /// <summary>
        /// ���ص���IP
        /// </summary>
        static public string PCIP
        {
            get
            {
                if (_PCIP == null)
                {
                    SelectQuery query = new SelectQuery("SELECT * FROM Win32_NetworkAdapterConfiguration");
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                    {
                        foreach (var item in searcher.Get())
                        {
                            if (Convert.ToBoolean(item["IPEnabled"]))
                            {
                                _PCIP = ((string[])item["IPAddress"])[0];
                            }
                        }
                    }
                }
                return _PCIP;
            }
        }

        static string _UUID = null;
        /// <summary>
        /// ���ص���UUID
        /// </summary>
        static public string UUID
        {
            get
            {
                if (_UUID == null)
                {
                    SelectQuery query = new SelectQuery("select * from Win32_ComputerSystemProduct");
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                    {
                        foreach (var item in searcher.Get())
                        {
                            _UUID = item["UUID"].ToString();
                        }
                    }
                }
                return _UUID;
            }
        }

        #endregion

        #region �������ݿ�

        /// <summary>
        /// Ĭ�Ϲ���
        /// </summary>
        public IoRyClass()
        {
        }

        /// <summary>
        /// ���ó�ʱʱ�� ����Ϊ0 ���޵ȴ�
        /// </summary>
        /// <param name="TimeOut">��ʱʱ�� ����Ϊ0 ���޵ȴ�</param>
        public IoRyClass(int TimeOut)
        {
            this.timeout = TimeOut;
        }

        /// <summary>
        /// �������Ĺ���
        /// </summary>
        /// <param name="Path"></param>
        public IoRyClass(string Path)
        {
            this.Path = Path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path">xml�ļ�·��</param>
        /// <param name="TimeOut">��ʱʱ�� ����Ϊ0 ���޵ȴ�</param>
        public IoRyClass(string Path, int TimeOut)
        {
            this.Path = Path;
            this.timeout = TimeOut;
        }

        /// <summary>
        /// ��ȡxml�Զ���������ַ���
        /// </summary>
        /// <returns></returns>
        void ReadConString()
        {
            this.ReadConString(this.Path);
        }

        /// <summary>
        /// ��ȡxml�Զ���������ַ���
        /// </summary>
        /// <param name="path">xml�ļ�������</param>
        /// <returns></returns>
        void ReadConString(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //�ж��Ƿ���ڴ�xml
            if (!File.Exists(path))
            {
                throw new Exception("�����ҵ�IoRyClass���XML�����ļ���");
            }
            xmlDoc.Load(path);
            //�ж������ַ�������
            XmlNode contype = xmlDoc.SelectSingleNode("constring/type");
            if (contype.InnerText.Trim() != "MSSQL" && contype.InnerText.Trim() != "ACCESS" && contype.InnerText.Trim() != "Oracle" && contype.InnerText.Trim() != "Excel")
            {
                throw new Exception("������������û��д,������д����,ֻ����дSQL;ACCESS;Oracle;Excel�������ִ�Сд!");
            }

            if (contype.InnerText.Trim() == "MSSQL")//sql
            {
                this._Contype = ConType.Sql;
                //�ж��Ƿ��ü򵥵�ֱ��д�ַ����ķ�ʽ
                XmlNode mynode = xmlDoc.SelectSingleNode("constring/sqlserver/simple");
                if (mynode.InnerText.Trim() != "")
                {
                    this.ConString = mynode.FirstChild.Value;
                }
                else
                {
                    mynode = xmlDoc.SelectSingleNode("constring/sqlserver/ip");
                    string ip = mynode.FirstChild.Value;
                    mynode = xmlDoc.SelectSingleNode("constring/sqlserver/databasename");
                    string databasename = mynode.FirstChild.Value;
                    mynode = xmlDoc.SelectSingleNode("constring/sqlserver/username");
                    string username = mynode.FirstChild.Value;
                    string password = null;
                    mynode = xmlDoc.SelectSingleNode("constring/sqlserver/passwordencryption");
                    //�ж����ݿ��ַ����Ƿ����
                    if (mynode.InnerText.Trim() != "")
                    {
                        XmlNode key = xmlDoc.SelectSingleNode("constring/sqlserver/encryptKey");
                        password = IoRyClass.DecryptDES(mynode.FirstChild.Value, key.FirstChild.Value);
                    }
                    else
                    {
                        mynode = xmlDoc.SelectSingleNode("constring/sqlserver/password");
                        password = mynode.FirstChild.Value;
                    }
                    string con = string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", ip, databasename, username, password);
                    this.ConString = con;
                }
            }
            else if (contype.InnerText.Trim() == "ACCESS") //access
            {
                this._Contype = ConType.Access;
                //�ж��Ƿ��ü򵥵�ֱ��д�ַ����ķ�ʽ
                XmlNode mynode = xmlDoc.SelectSingleNode("constring/access/simple");
                if (mynode.InnerText.Trim() != "")
                {
                    this.ConString = mynode.FirstChild.Value;
                }
                else
                {
                    mynode = xmlDoc.SelectSingleNode("constring/access/path");
                    string accpath = mynode.FirstChild.Value;
                    if (File.Exists(accpath))
                    {
                        string password = null;
                        mynode = xmlDoc.SelectSingleNode("constring/access/passwordencryption");
                        //�ж����ݿ��ַ����Ƿ����
                        if (mynode.InnerText.Trim() != "")
                        {
                            XmlNode key = xmlDoc.SelectSingleNode("constring/access/encryptKey");
                            password = IoRyClass.DecryptDES(mynode.FirstChild.Value, key.FirstChild.Value);
                        }
                        else
                        {
                            mynode = xmlDoc.SelectSingleNode("constring/access/password");
                            password = mynode.FirstChild.Value;
                        }
                        string con = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Database Password={1}", accpath, password);
                        this.ConString = con;
                    }
                    else
                    {
                        throw new Exception("�Ҳ����ļ�:" + accpath);
                    }
                }
            }
            else if (contype.InnerText.Trim() == "Oracle") //Oracle
            {
                this._Contype = ConType.Oracle;
                //�ж��Ƿ��ü򵥵�ֱ��д�ַ����ķ�ʽ
                XmlNode mynode = xmlDoc.SelectSingleNode("constring/Oracle/simple");
                if (mynode.InnerText.Trim() != "")
                {
                    this.ConString = mynode.FirstChild.Value;
                }
                else
                {
                    mynode = xmlDoc.SelectSingleNode("constring/Oracle/DataSource");
                    string DBServer = mynode.FirstChild.Value;
                    mynode = xmlDoc.SelectSingleNode("constring/Oracle/username");
                    string username = mynode.FirstChild.Value;
                    string password = null;
                    mynode = xmlDoc.SelectSingleNode("constring/Oracle/passwordencryption");
                    //�ж����ݿ��ַ����Ƿ����
                    if (mynode.InnerText.Trim() != "")
                    {
                        XmlNode key = xmlDoc.SelectSingleNode("constring/Oracle/encryptKey");
                        password = IoRyClass.DecryptDES(mynode.FirstChild.Value, key.FirstChild.Value);
                    }
                    else
                    {
                        mynode = xmlDoc.SelectSingleNode("constring/Oracle/password");
                        password = mynode.FirstChild.Value;
                    }
                    mynode = xmlDoc.SelectSingleNode("constring/Oracle/Provider");
                    string Provider = mynode.FirstChild.Value;
                    string con;
                    if (Provider == "OraOledb.Oracle")
                    {
                        con = string.Format("Provider=OraOledb.Oracle;Data Source={0};User ID={1};Password={2}", DBServer, username, password);
                    }
                    else
                    {
                        con = string.Format("Provider=MSDAORA;Data Source={0};User ID={1};Password={2}", DBServer, username, password);
                    }

                    this.ConString = con;
                }
            }
            else if (contype.InnerText.Trim() == "Excel")
            {
                this.Contype = ConType.Excel;
                XmlNode mynode = xmlDoc.SelectSingleNode("constring/Excel/path");
                string Excelpath = mynode.FirstChild.Value;
                if (File.Exists(Excelpath))
                {
                    this.ConString = this.GetExcelReadonlyConnStr(Excelpath);
                }
                else
                {
                    throw new Exception("�Ҳ����ļ�:" + Excelpath);
                }

            }
            else
            {
                this.ConString = null;
            }
        }

        /// <summary>
        /// ����д���Excel�����ַ���
        /// </summary>
        void ExcelWriteConString()
        {
            string path = this.Path;
            XmlDocument xmlDoc = new XmlDocument();
            //�ж��Ƿ���ڴ�xml
            if (!File.Exists(path))
            {
                throw new Exception("�����ҵ�IoRyClass���XML�����ļ���");
            }
            xmlDoc.Load(path);
            //�ж������ַ�������
            XmlNode contype = xmlDoc.SelectSingleNode("constring/type");
            if (contype.InnerText.Trim() != "SQL" && contype.InnerText.Trim() != "ACCESS" && contype.InnerText.Trim() != "Oracle" && contype.InnerText.Trim() != "Excel")
            {
                throw new Exception("������������û��д,������д����,ֻ����дSQL;ACCESS;Oracle;Excel�������ִ�Сд!");
            }

            if (contype.InnerText.Trim() == "Excel")
            {
                XmlNode mynode = xmlDoc.SelectSingleNode("constring/Excel/path");
                string Excelpath = mynode.FirstChild.Value;
                if (File.Exists(Excelpath))
                {
                    this.ConString = this.GetExcelConnStr(Excelpath);
                }
                else
                {
                    throw new Exception("�Ҳ����ļ�:" + Excelpath);
                }
            }
            else
            {
                this.ConString = null;
            }
        }

        /// <summary>
        /// ֻ��Excel���Ӵ�
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetExcelReadonlyConnStr(string filePath)
        {
            string connStr = string.Empty;

            if (filePath.Contains(".xlsx"))
            {
                connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1;'";
            }
            else if (filePath.Contains(".xls"))
            {
                connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1;'";
            }
            else
            {
                connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath.Remove(filePath.LastIndexOf("\\") + 1) + ";Extended Properties='Text;FMT=Delimited;HDR=YES;'";
            }

            return connStr;
        }

        /// <summary>
        /// Excel���Ӵ�
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetExcelConnStr(string filePath)
        {
            string connStr = string.Empty;

            if (filePath.Contains(".xlsx"))
            {
                connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=0;'";
            }
            else if (filePath.Contains(".xls"))
            {
                connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=0;'";
            }
            else
            {
                connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath.Remove(filePath.LastIndexOf("\\") + 1) + ";Extended Properties='Text;FMT=Delimited;HDR=YES;'";
            }

            return connStr;
        }

        /// <summary>
        /// ����Connection
        /// </summary>
        /// <returns></returns>
        IDbConnection IoRyCon()
        {
            switch (this.Contype)
            {
                case ConType.Null:
                    throw new Exception("�����ļ�����!û��ȷ�����ݿ������ַ�����");
                case ConType.Sql:
                    return new SqlConnection(this.ConString);
                case ConType.Access:
                case ConType.Oracle:
                case ConType.Excel:
                    return new OleDbConnection(this.ConString);

                default:
                    return null;
            }
        }

        /// <summary>
        /// ִ��sql���
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <returns>��Ӱ������</returns>
        public string ExecuteSql(string sql)
        {
            switch (this.Contype)
            {
                case ConType.Sql:
                    try
                    {
                        int result = 0;
                        using (SqlConnection Con = (SqlConnection)this.IoRyCon())
                        {
                            SqlCommand com = new SqlCommand(sql, Con);
                            if (this.timeout != -1)
                            {
                                com.CommandTimeout = this.timeout;
                            }
                            Con.Open();
                            result = com.ExecuteNonQuery();
                        }
                        return Convert.ToString(result);
                    }
                    catch (Exception me)
                    {
                        throw me;
                    }
                case ConType.Access:
                    try
                    {
                        int result = 0;
                        using (OleDbConnection Con = (OleDbConnection)this.IoRyCon())
                        {
                            OleDbCommand com = new OleDbCommand(sql, Con);
                            if (this.timeout != -1)
                            {
                                com.CommandTimeout = this.timeout;
                            }
                            Con.Open();
                            result = com.ExecuteNonQuery();
                        }
                        return Convert.ToString(result);
                    }
                    catch (Exception me)
                    {
                        throw me;
                    }
                case ConType.Oracle:
                    try
                    {
                        int result = 0;
                        using (OleDbConnection Con = (OleDbConnection)this.IoRyCon())
                        {
                            OleDbCommand com = new OleDbCommand(sql, Con);
                            if (this.timeout != -1)
                            {
                                com.CommandTimeout = this.timeout;
                            }
                            Con.Open();
                            result = com.ExecuteNonQuery();
                        }
                        return Convert.ToString(result);
                    }
                    catch (Exception me)
                    {
                        throw me;
                    }
                case ConType.Excel:
                    try
                    {
                        int result = 0;
                        //�������Ҫ�����ܸ��µ������ַ���
                        this.ExcelWriteConString();
                        using (OleDbConnection Con = (OleDbConnection)this.IoRyCon())
                        {
                            OleDbCommand com = new OleDbCommand(sql, Con);
                            if (this.timeout != -1)
                            {
                                com.CommandTimeout = this.timeout;
                            }
                            Con.Open();
                            result = com.ExecuteNonQuery();
                        }
                        return Convert.ToString(result);
                    }
                    catch (Exception me)
                    {
                        throw me;
                    }
                default:
                    return null;
            }
        }

        /// <summary>
        /// ִ��sql��� ��������
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <param name="DbParameterS">���</param>
        /// <returns>��Ӱ������</returns>
        public string ExecuteSql_DbParameter(string sql, List<DbParameter> DbParameterS)
        {
            switch (this.Contype)
            {
                case ConType.Sql:
                    try
                    {
                        int result = 0;
                        using (SqlConnection Con = (SqlConnection)this.IoRyCon())
                        {
                            SqlCommand com = new SqlCommand(sql, Con);
                            com.Parameters.AddRange(DbParameterS.ToArray());
                            if (this.timeout != -1)
                            {
                                com.CommandTimeout = this.timeout;
                            }
                            Con.Open();
                            result = com.ExecuteNonQuery();
                        }
                        return Convert.ToString(result);
                    }
                    catch (Exception me)
                    {
                        throw me;
                    }
                case ConType.Access:
                case ConType.Oracle:
                case ConType.Excel:
                    throw new Exception("Ŀǰ��֧��!");
                default:
                    return null;
            }
        }

        /// <summary>
        /// ����ִ��sql��ֻ֧��SqlServer������
        /// </summary>
        /// <param name="sql">sql�����</param>
        /// <returns>��Ӱ������</returns>
        public string ExecuteSqlTran(string sql)
        {
            switch (this.Contype)
            {
                case ConType.Sql:
                    SqlTransaction sqlTran = null;
                    int result = 0;
                    using (SqlConnection Con = (SqlConnection)this.IoRyCon())
                    {
                        try
                        {
                            Con.Open();
                            sqlTran = Con.BeginTransaction();
                            SqlCommand command = Con.CreateCommand();
                            if (this.timeout != -1)
                            {
                                command.CommandTimeout = this.timeout;
                            }
                            command.Transaction = sqlTran;
                            command.CommandText = sql;
                            result = command.ExecuteNonQuery();

                            sqlTran.Commit();
                        }
                        catch (Exception me)
                        {
                            if (sqlTran != null)
                            {
                                sqlTran.Rollback();
                            }
                            throw me;
                        }
                    }
                    return Convert.ToString(result);

                case ConType.Oracle:
                    throw new Exception("oracle��֧��һ��ִ�ж�����;��sql��䣡��ʹ��ExecuteSql�ķ���,ÿ��ִ��һ��sql���.");
                case ConType.Access:
                    throw new Exception("-.-#  Accessû�����񣬴��~��");
                case ConType.Excel:
                    throw new Exception("ExcelĿǰ��֧��");
                default:
                    return null;
            }
        }

        /// <summary>
        /// ����ִ��sql��ֻ֧��SqlServer������
        /// </summary>
        /// <param name="sql">sql�����</param>
        /// <param name="DbParameterS">���</param>
        /// <returns></returns>
        public string ExecuteSqlTran_DbParameter(string sql, List<DbParameter> DbParameterS)
        {
            switch (this.Contype)
            {
                case ConType.Sql:
                    SqlTransaction sqlTran = null;
                    int result = 0;
                    using (SqlConnection Con = (SqlConnection)this.IoRyCon())
                    {
                        try
                        {
                            Con.Open();
                            sqlTran = Con.BeginTransaction();
                            SqlCommand command = Con.CreateCommand();
                            command.Parameters.AddRange(DbParameterS.ToArray());
                            if (this.timeout != -1)
                            {
                                command.CommandTimeout = this.timeout;
                            }
                            command.Transaction = sqlTran;
                            command.CommandText = sql;
                            result = command.ExecuteNonQuery();

                            sqlTran.Commit();
                        }
                        catch (Exception me)
                        {
                            if (sqlTran != null)
                            {
                                sqlTran.Rollback();
                            }
                            throw me;
                        }
                    }
                    return Convert.ToString(result);

                case ConType.Oracle:
                    throw new Exception("oracle��֧��һ��ִ�ж�����;��sql��䣡��ʹ��ExecuteSql�ķ���,ÿ��ִ��һ��sql���.");
                case ConType.Access:
                    throw new Exception("-.-#  Accessû�����񣬴��~��");
                case ConType.Excel:
                    throw new Exception("ExcelĿǰ��֧��");
                default:
                    return null;
            }
        }

        /// <summary>
        /// ȡ��datateble,DataSet�ĵ�һ�ű�
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <returns></returns>
        public DataTable GetTable(string sql)
        {
            return this.GetDataSet(sql).Tables[0];
        }

        /// <summary>
        /// ȡ��datateble,DataSet�ĵ�һ�ű�
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <param name="DbParameterS">���</param>
        /// <returns></returns>
        public DataTable GetTable_DbParameter(string sql, List<DbParameter> DbParameterS)
        {
            return this.GetDataSet_DbParameter(sql, DbParameterS).Tables[0];
        }

        /// <summary>
        /// ȡ��DataSet
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql)
        {
            switch (this.Contype)
            {
                case ConType.Sql:
                    try
                    {
                        using (SqlConnection Con = (SqlConnection)this.IoRyCon())
                        {
                            SqlCommand com = new SqlCommand(sql, Con);
                            if (this.timeout != -1)
                            {
                                com.CommandTimeout = this.timeout;
                            }
                            SqlDataAdapter ada = new SqlDataAdapter(com);
                            DataSet myds = new DataSet();
                            ada.Fill(myds);
                            return myds;
                        }
                    }
                    catch (Exception me)
                    {
                        throw me;
                    }
                case ConType.Access:
                case ConType.Oracle:
                case ConType.Excel:
                    try
                    {
                        using (OleDbConnection Con = (OleDbConnection)this.IoRyCon())
                        {
                            OleDbCommand com = new OleDbCommand(sql, Con);
                            if (this.timeout != -1)
                            {
                                com.CommandTimeout = this.timeout;
                            }
                            OleDbDataAdapter ada = new OleDbDataAdapter(com);
                            DataSet myds = new DataSet();
                            ada.Fill(myds);
                            return myds;
                        }
                    }
                    catch (Exception me)
                    {
                        throw me;
                    }
                default:
                    return null;
            }
        }

        /// <summary>
        ///  ȡ��DataSet
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <param name="DbParameterS">���</param>
        /// <returns></returns>
        public DataSet GetDataSet_DbParameter(string sql, List<DbParameter> DbParameterS)
        {
            switch (this.Contype)
            {
                case ConType.Sql:
                    try
                    {
                        using (SqlConnection Con = (SqlConnection)this.IoRyCon())
                        {
                            SqlCommand com = new SqlCommand(sql, Con);
                            com.Parameters.AddRange(DbParameterS.ToArray());
                            if (this.timeout != -1)
                            {
                                com.CommandTimeout = this.timeout;
                            }
                            SqlDataAdapter ada = new SqlDataAdapter(com);
                            DataSet myds = new DataSet();
                            ada.Fill(myds);
                            return myds;
                        }
                    }
                    catch (Exception me)
                    {
                        throw me;
                    }
                case ConType.Access:
                case ConType.Oracle:
                case ConType.Excel:
                    throw new Exception("Ŀǰ��֧����Щ�����Ĵ�DbParameterS��ʽ!");
                default:
                    return null;
            }
        }

        /// <summary>
        /// ִ�д洢����,��sqlparameter�ķ�ʽ,һ�㷵�����ݼ� 2012-4-17���
        /// ע��out���͵���� Ҫ����dd.Direction = System.Data.ParameterDirection.Output; �洢�����и�out����Ҫ select @id=(select count(*) from log_data)
        /// </summary>
        /// <param name="SPname">SP����</param>
        /// <param name="DbParameterS">DbParameter�ļ���</param>
        /// <returns>һ�㷵�����ݼ�</returns>
        public DataSet ExecuteSP(string SPname, List<DbParameter> DbParameterS)
        {
            switch (this.Contype)
            {
                case ConType.Access:
                case ConType.Excel:
                    throw new Exception("-.-#  Access,Excel���������ַ���?");
                case ConType.Sql:
                    try
                    {
                        using (SqlConnection Con = (SqlConnection)this.IoRyCon())
                        {
                            SqlCommand sc = new SqlCommand();
                            if (this.timeout != -1)
                            {
                                sc.CommandTimeout = this.timeout;
                            }
                            sc.Connection = Con;
                            sc.CommandType = CommandType.StoredProcedure;
                            sc.CommandText = SPname;
                            sc.Parameters.AddRange(DbParameterS.ToArray());
                            DataSet ds = new DataSet();
                            SqlDataAdapter SDA = new SqlDataAdapter(sc);
                            SDA.Fill(ds);
                            return ds;
                        }
                    }
                    catch (Exception me)
                    {
                        throw me;
                    }
                case ConType.Oracle:
                    try
                    {
                        using (OleDbConnection Con = (OleDbConnection)this.IoRyCon())
                        {
                            OleDbCommand sc = new OleDbCommand();
                            if (this.timeout != -1)
                            {
                                sc.CommandTimeout = this.timeout;
                            }
                            sc.Connection = Con;
                            sc.CommandType = CommandType.StoredProcedure;
                            sc.CommandText = SPname;
                            sc.Parameters.AddRange(DbParameterS.ToArray());
                            DataSet ds = new DataSet();
                            OleDbDataAdapter SDA = new OleDbDataAdapter(sc);
                            SDA.Fill(ds);
                            return ds;
                        }
                    }
                    catch (Exception me)
                    {
                        throw me;
                    }
                case ConType.Null:
                default:
                    throw new Exception("ConType����");
            }
        }

        /// <summary>
        /// �ѹ�ʱ
        /// ���ֻ��ִ��һ�������ұ�����insert���
        /// ����GUID��Ӧ��,�˺��������ò�����
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <returns>��ǰ�����е�ֵ��������</returns>
        public string GetTheValueOfNewAdd(string sql)
        {
            switch (this.Contype)
            {
                case ConType.Sql:
                    SqlTransaction sqlTran = null;
                    string result = null;
                    using (SqlConnection Con = (SqlConnection)this.IoRyCon())
                    {
                        try
                        {
                            Con.Open();
                            sqlTran = Con.BeginTransaction();
                            SqlCommand command = Con.CreateCommand();
                            if (this.timeout != -1)
                            {
                                command.CommandTimeout = this.timeout;
                            }
                            command.Transaction = sqlTran;
                            //���裬����������ܵõ�
                            sql = sql + ";select scope_identity();";
                            command.CommandText = sql;
                            result = Convert.ToString(command.ExecuteScalar());
                            sqlTran.Commit();
                        }
                        catch (Exception me)
                        {
                            if (sqlTran != null)
                            {
                                sqlTran.Rollback();
                            }
                            throw me;
                        }
                    }
                    return result;

                case ConType.Access:
                    throw new Exception("-.-#  Accessû�����񣬴��~��");
                case ConType.Oracle:
                    throw new Exception("-.-#  ��DLL��֧��Oracle�Ĵ˲���");
                case ConType.Excel:
                    throw new Exception("-.-#  ��DLL��֧��Excel�Ĵ˲���");
                default:
                    throw new Exception("ConType����");
            }
        }

        #endregion

        #region ����

        #region ����������ļ��ܷ�ʽ����Ҫ��Կ �ԳƼ���

        //Ĭ����Կ����
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES�����ַ���
        /// </summary>
        /// <param name="encryptString">�����ܵ��ַ���</param>
        /// <param name="encryptKey">������Կ,Ҫ��Ϊ8λ</param>
        /// <returns>���ܳɹ����ؼ��ܺ���ַ�����ʧ�ܷ���Դ��</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// DES�����ַ���
        /// </summary>
        /// <param name="decryptString">�����ܵ��ַ���</param>
        /// <param name="decryptKey">������Կ,Ҫ��Ϊ8λ,�ͼ�����Կ��ͬ</param>
        /// <returns>���ܳɹ����ؽ��ܺ���ַ�����ʧ�ܷ�Դ��</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region �ǶԳƼ���

        //RSA rsa = RSA.Create();
        //rsa.ToXmlString(false)��Կ
        //rsa.ToXmlString(true)��Կ
        //string enData = EnRSA(���ܴ�, rsa.ToXmlString(false));
        //string bbb = DeRSA(���ܴ�, rsa.ToXmlString(true));

        /// <summary>
        /// �ǶԳƼ��� ���� Ĭ�ϵ��г�������
        /// </summary>
        /// <param name="data"></param>
        /// <param name="publickey">RSA rsa = RSA.Create();rsa.ToXmlString(false)</param>
        /// <returns></returns>
        public static string EnRSA(string data, string publickey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(data), false);
            return Convert.ToBase64String(cipherbytes);
        }

        /// <summary>
        /// �ǶԳƼ��� ���� �޳�������
        /// </summary>
        /// <param name="data"></param>
        /// <param name="publickey"></param>
        /// <returns></returns>
        public static String EncryptRSA_long(string data, string publickey)
        {
            using (RSACryptoServiceProvider RSACryptography = new RSACryptoServiceProvider())
            {
                Byte[] PlaintextData = Encoding.UTF8.GetBytes(data);
                RSACryptography.FromXmlString(publickey);
                int MaxBlockSize = RSACryptography.KeySize / 8 - 11;    //���ܿ���󳤶�����

                if (PlaintextData.Length <= MaxBlockSize)
                {
                    return Convert.ToBase64String(RSACryptography.Encrypt(PlaintextData, false));
                }

                using (MemoryStream PlaiStream = new MemoryStream(PlaintextData))
                {
                    using (MemoryStream CrypStream = new MemoryStream())
                    {
                        Byte[] Buffer = new Byte[MaxBlockSize];
                        int BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);

                        while (BlockSize > 0)
                        {
                            Byte[] ToEncrypt = new Byte[BlockSize];
                            Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);

                            Byte[] Cryptograph = RSACryptography.Encrypt(ToEncrypt, false);
                            CrypStream.Write(Cryptograph, 0, Cryptograph.Length);

                            BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                        }

                        return Convert.ToBase64String(CrypStream.ToArray(), Base64FormattingOptions.None);
                    }
                }
            }
        }

        /// <summary>
        /// �ǶԳƼ��� ���� Ĭ�ϵ��г�������
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privatekey">RSA rsa = RSA.Create();rsa.ToXmlString(true)</param>
        /// <returns></returns>
        public static string DeRSA(string data, string privatekey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privatekey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(data), false);
            return Encoding.UTF8.GetString(cipherbytes);
        }

        /// <summary>
        /// �ǶԳƼ��� ���� �޳�������
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privatekey"></param>
        /// <returns></returns>
        public static String DecryptRSA_long(string data, string privatekey)
        {
            using (RSACryptoServiceProvider RSACryptography = new RSACryptoServiceProvider())
            {
                byte[] CiphertextData = Convert.FromBase64String(data);
                RSACryptography.FromXmlString(privatekey);
                int MaxBlockSize = RSACryptography.KeySize / 8;    //���ܿ���󳤶�����

                if (CiphertextData.Length <= MaxBlockSize)
                {
                    return Encoding.UTF8.GetString(RSACryptography.Decrypt(CiphertextData, false));
                }

                using (MemoryStream CrypStream = new MemoryStream(CiphertextData))
                {
                    using (MemoryStream PlaiStream = new MemoryStream())
                    {
                        Byte[] Buffer = new Byte[MaxBlockSize];
                        int BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);

                        while (BlockSize > 0)
                        {
                            Byte[] ToDecrypt = new Byte[BlockSize];
                            Array.Copy(Buffer, 0, ToDecrypt, 0, BlockSize);

                            Byte[] Plaintext = RSACryptography.Decrypt(ToDecrypt, false);
                            PlaiStream.Write(Plaintext, 0, Plaintext.Length);

                            BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);
                        }

                        return Encoding.UTF8.GetString(PlaiStream.ToArray());
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// ����������������ݽ���md5���� ������������ 2016-3-9��ΪUFT8����
        /// </summary>
        /// <param name="strIn">�����ַ���</param>
        /// <returns>���</returns>
        public static string MD5(string strIn)
        {
            byte[] MainClass = System.Text.Encoding.UTF8.GetBytes(strIn);
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(MainClass);
            return BitConverter.ToString(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteIn"></param>
        /// <returns></returns>
        public static string MD5(byte[] byteIn)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(byteIn);
            return BitConverter.ToString(result);
        }

        #endregion

        #region �ֽ�ת����ѹ��


        /// <summary>
        /// datatable���л�string
        /// </summary>
        /// <param name="pDt"></param>
        /// <returns></returns>
        public static string SerializeDataTableXml(DataTable pDt)
        {
            //  ���л�DataTable
            pDt.TableName = pDt.TableName;
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            serializer.Serialize(writer, pDt);
            writer.Close();
            return sb.ToString();
        }
        ///   <summary>
        ///  �����л�DataTable string
        ///   </summary>
        ///   <param name="pXml"> ���л���DataTable </param>
        ///   <returns> DataTable </returns>
        public static DataTable DeserializeDataTable(string pXml)
        {
            StringReader strReader = new StringReader(pXml);
            XmlReader xmlReader = XmlReader.Create(strReader);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            DataTable dt = serializer.Deserialize(xmlReader) as DataTable;
            return dt;
        }

        /// <summary>
        /// �����ļ�ת����Base64
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string FileToBase64(string FilePath)
        {
            FileStream files = new FileStream(FilePath, FileMode.Open);
            byte[] bt = new byte[files.Length];
            files.Read(bt, 0, bt.Length);
            string base64Str = Convert.ToBase64String(bt);
            return base64Str;
        }

        /// <summary>
        /// ѹ������
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static byte[] CompressData(byte[] Data)
        {
            if (Data != null)
            {
                MemoryStream ms = new MemoryStream();
                DeflateStream deflateStream = new DeflateStream(ms, CompressionMode.Compress, false);
                deflateStream.Write(Data, 0, Data.Length);
                deflateStream.Close();
                return ms.ToArray();
            }
            return null;
        }

        /// <summary>
        /// ��ѹ������
        /// </summary>
        /// <param name="Data">��ѹ������</param>
        /// <param name="nMaxScale">�����޷���ȡʵ�ʽ�ѹ��Ĵ�С,����һ�������ԭʼ���ݴ�С�ı���ֵ</param>
        /// <returns></returns>
        public static byte[] DecompressData(byte[] Data, int nMaxScale)
        {
            MemoryStream ms = new MemoryStream(Data);

            #region -��ȡ��ѹ��Ĵ�С,Ҫ�Ľ�(̫����)

            //DeflateStream deflateStreamSize = new DeflateStream(ms, CompressionMode.Decompress,false);
            //int nLenght    = new int();
            //nLenght        = 0;
            //while (deflateStreamSize.ReadByte() != -1)
            //{
            //    nLenght = nLenght + 1;
            //}

            #endregion

            //byte[] dezipArray = new byte[nLenght];
            //ms.Position = 0;
            // �����޷���ȡʵ�ʽ�ѹ��Ĵ�С,��������һ������ֵ
            byte[] dezipArray = new byte[Data.Length * nMaxScale];
            DeflateStream deflateStream = new DeflateStream(ms, CompressionMode.Decompress, false);
            int nL = deflateStream.Read(dezipArray, 0, dezipArray.Length);
            byte[] resultData = new byte[nL];
            Array.Copy(dezipArray, resultData, nL);
            deflateStream.Close();
            return resultData;
        }

        /// <summary>
        /// ȡ��DataSet�Ķ�����XML����(������WebService���л�ʹ��,������ߴ����ٶ�Լ%40)
        /// </summary>
        /// <param name="dsOriginal">���л���DataSet</param>
        /// <returns>��������ʽ��XML</returns>
        public static byte[] GetBinaryFormatDataSet(DataSet dsOriginal)
        {
            if (dsOriginal != null)
            {
                byte[] binaryDataResult = null;
                using (MemoryStream memStream = new MemoryStream())        // ��Ҫһ���ڴ�������
                {
                    IFormatter brFormatter = new BinaryFormatter();        // ���������л�����
                    dsOriginal.RemotingFormat = SerializationFormat.Binary;// DataSet����ӵ�д�����

                    brFormatter.Serialize(memStream, dsOriginal);
                    binaryDataResult = memStream.ToArray();
                    memStream.Close();
                    return binaryDataResult;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// �������Ƶ�DataSetXML�����л�Ϊ��׼XML DataSet
        /// </summary>
        /// <param name="binaryData">��Ҫ�����л���DatSet</param>
        /// <returns>��׼XML��DataSet</returns>
        public static DataSet RetrieveDataSet(byte[] binaryData)
        {
            if (binaryData != null)
            {
                using (MemoryStream memStream = new MemoryStream(binaryData))
                {
                    IFormatter brFormatter = new BinaryFormatter();
                    DataSet ds = (DataSet)brFormatter.Deserialize(memStream);
                    memStream.Close();
                    return ds;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ���л�DataSet����
        /// </summary>
        /// <param name="dsOriginal"></param>
        /// <returns></returns>
        public static byte[] GetXmlFormatDataSet(DataSet dsOriginal)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                XmlSerializer xs = new XmlSerializer(typeof(DataSet));
                xs.Serialize(memStream, dsOriginal);
                return memStream.ToArray();
            }
        }

        /// <summary>
        /// �����л�DataSet����
        /// </summary>
        /// <param name="binaryData"></param>
        /// <returns></returns>
        public static DataSet RetrieveXmlDataSet(byte[] binaryData)
        {
            using (MemoryStream memStream = new MemoryStream(binaryData))
            {
                XmlSerializer xs = new XmlSerializer(typeof(DataSet));
                DataSet ds = xs.Deserialize(memStream) as DataSet;
                return ds;
            }
        }

        /// <summary>
        /// ��stringת����utf8��bytes
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StringToBytes(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// ��bytesת����string
        /// </summary>
        /// <param name="mybyte"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] mybyte)
        {
            return System.Text.Encoding.UTF8.GetString(mybyte);
        }

        /// <summary>
        /// ѹ���ַ���
        /// </summary>
        /// <param name="str">����</param>
        /// <returns>ѹ�����byte[]</returns>
        public static byte[] StringToZip(string str)
        {
            byte[] myby = IoRyClass.StringToBytes(str);
            return IoRyClass.CompressData(myby);
        }

        /// <summary>
        /// ��ѹ���ַ���
        /// </summary>
        /// <param name="mybytes"></param>
        /// <returns></returns>
        public static string ZipToString(byte[] mybytes)
        {
            byte[] myte = IoRyClass.DecompressData(mybytes, 10);
            return IoRyClass.BytesToString(myte);
        }

        /// <summary>
        /// ѹ��DataTable
        /// </summary>
        /// <param name="mydt">DataTable</param>
        /// <returns></returns>
        public static byte[] DataTableToZip(DataTable mydt)
        {
            DataSet myds = new DataSet();
            myds.Tables.Add(mydt.Copy());
            byte[] mybt = IoRyClass.GetBinaryFormatDataSet(myds);
            return IoRyClass.CompressData(mybt);
        }

        /// <summary>
        /// ��ѹDataTable
        /// </summary>
        /// <param name="mybytes"></param>
        /// <returns></returns>
        public static DataTable ZipToDateTable(byte[] mybytes)
        {
            byte[] myby = IoRyClass.DecompressData(mybytes, 50);
            DataSet myds = IoRyClass.RetrieveDataSet(myby);
            return myds.Tables[0];
        }

        /// <summary>
        /// ѹ��DataSet
        /// </summary>
        /// <param name="myds">DateSet</param>
        /// <returns></returns>
        public static byte[] DataSetToZip(DataSet myds)
        {
            byte[] mybys = IoRyClass.GetBinaryFormatDataSet(myds);
            return IoRyClass.CompressData(mybys);
        }

        /// <summary>
        /// ��ѹDataSet
        /// </summary>
        /// <param name="mybytes"></param>
        /// <returns></returns>
        public static DataSet ZipToDataSet(byte[] mybytes)
        {
            byte[] myby = IoRyClass.DecompressData(mybytes, 50);
            return IoRyClass.RetrieveDataSet(myby);
        }

        #endregion

        #region Log ��winformDemo��鿴log��һ��

        //20200320 �޸������ݿ�,���ҷŵ���tool��demo��,�����������
        //����Log��Ӧ�����ݿ���� 
        /*
CREATE TABLE [dbo].[Log_H](
	[int_index] [int] IDENTITY(1,1) NOT NULL,
	[str_opreater] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[str_Type] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
    [str_tablename] [nvarchar](50)  COLLATE Chinese_PRC_CI_AS NULL,
	[str_Sql] [nvarchar](4000) COLLATE Chinese_PRC_CI_AS NULL,
	[str_Old] [nvarchar](max) COLLATE Chinese_PRC_CI_AS NULL,
	[dat_time] [datetime] NULL,
 CONSTRAINT [PK_Log_H] PRIMARY KEY CLUSTERED 
(
	[int_index] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
        */

        /// <summary>
        /// ���ɴ�����IP��UUID��sql��־��� 20200323 ����Ĳ�������ip���ò����� 
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        string GetLogSql_IP(string username, string sql)
        {
            IoRyClass ic = new IoRyClass(this.Path);
            string tablename = "";
            if (sql.ToLower().Contains("insert"))
            {
                tablename = sql.ToLower().Replace("insert", "").Replace("into", "").Split('(')[0].Trim();
                return string.Format(@";insert into log_data (sopreater_str,type_str,SQL_str,olddata_str,createtime_dt,tablename_str,UUID_GUID_str,IP_str,log_data_GUID) 
values ('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}');", username, "����", sql.Replace("'", "''"), "", "getdate()", tablename, IoRyClass.UUID, IoRyClass.PCIP, Guid.NewGuid());
            }
            if (sql.ToLower().Contains("update"))
            {
                tablename = sql.ToLower().Split(new string[] { "set" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("update", "").Trim();
                string sqlold = "select * from " + tablename + " where " + sql.ToLower().Split(new string[] { "where" }, StringSplitOptions.None)[1];
                DataTable oldtable = ic.GetTable(sqlold);
                if (oldtable.Rows.Count == 0)
                {
                    return ";";
                }
                else
                {
                    //string old = "";
                    XElement xolddata = new XElement("OldData");
                    for (int j = 0; j < oldtable.Rows.Count; j++)
                    {
                        XElement row = new XElement("Row");
                        for (int i = 0; i < oldtable.Columns.Count; i++)
                        {
                            XElement col = new XElement("Column");
                            col.Add(new XAttribute("colName", oldtable.Columns[i].ColumnName));
                            col.Value = oldtable.Rows[j][i].ToString();
                            row.Add(col);
                            //old = old + " [ " + oldtable.Columns[i].ColumnName + " | " + oldtable.Rows[0][i].ToString() + " ]; ";
                        }
                        xolddata.Add(row);
                    }

                    return string.Format(@";insert into log_data (sopreater_str,type_str,SQL_str,olddata_str,createtime_dt,tablename_str,UUID_GUID_str,IP_str,log_data_GUID) 
values ('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}');", username, "�޸�", sql.Replace("'", "''"), xolddata.ToString().Replace("'", "''"), "getdate()", tablename, IoRyClass.UUID, IoRyClass.PCIP, Guid.NewGuid());
                }
            }
            if (sql.ToLower().Contains("delete"))
            {
                string sqlold = "";
                if (sql.ToLower().Contains("from"))
                {
                    if (sql.ToLower().Contains("where"))
                    {
                        tablename = sql.ToLower().Replace("delete", "").Replace("from", "").Split(new string[] { "where" }, StringSplitOptions.None)[0].Trim();
                    }
                    else
                    {
                        tablename = sql.ToLower().Replace("delete", "").Replace("from", "").Trim();
                    }
                    sqlold = sql.ToLower().Replace("delete", "select * ");
                }
                else
                {
                    if (sql.ToLower().Contains("where"))
                    {
                        tablename = sql.ToLower().Replace("delete", "").Split(new string[] { "where" }, StringSplitOptions.None)[0].Trim();
                    }
                    else
                    {
                        tablename = sql.ToLower().Replace("delete", "").Trim();
                    }
                    sqlold = sql.ToLower().Replace("delete", "select * from ");
                }
                DataTable oldtable = ic.GetTable(sqlold);
                if (oldtable.Rows.Count > 0)
                {
                    //string old = "";
                    //for (int i = 0; i < oldtable.Columns.Count; i++)
                    //{
                    //    old = old + " [ " + oldtable.Columns[i].ColumnName + " | " + oldtable.Rows[0][i].ToString() + " ]; ";
                    //}

                    XElement xolddata = new XElement("OldData");
                    for (int j = 0; j < oldtable.Rows.Count; j++)
                    {
                        XElement row = new XElement("Row");
                        for (int i = 0; i < oldtable.Columns.Count; i++)
                        {
                            XElement col = new XElement("Column");
                            col.Add(new XAttribute("colName", oldtable.Columns[i].ColumnName));
                            col.Value = oldtable.Rows[j][i].ToString();
                            row.Add(col);
                        }
                        xolddata.Add(row);
                    }


                    return string.Format(@";insert into log_data (sopreater_str,type_str,SQL_str,olddata_str,createtime_dt,tablename_str,UUID_GUID_str,IP_str,log_data_GUID)
values ('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}');", username, "ɾ��", sql.Replace("'", "''"), xolddata.ToString().Replace("'", "''"), "getdate()", tablename, IoRyClass.UUID, IoRyClass.PCIP, Guid.NewGuid());
                }
                else
                {
                    return "";
                }
            }
            if (sql.ToLower().Contains("select"))
            {
                if (sql.ToLower().Contains("where"))
                {
                    tablename = sql.ToLower().Split(new string[] { "where" }, StringSplitOptions.None)[0].Split(new string[] { "from" }, StringSplitOptions.None)[1].Trim();
                }
                else
                {
                    tablename = sql.ToLower().Split(new string[] { "from" }, StringSplitOptions.None)[1].Trim();
                }
                return string.Format(@";insert into log_data (sopreater_str,type_str,SQL_str,olddata_str,createtime_dt,tablename_str,UUID_GUID_str,IP_str,log_data_GUID)
values ('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}');", username, "��ѯ", sql.Replace("'", "''"), "", "getdate()", tablename, IoRyClass.UUID, IoRyClass.PCIP, Guid.NewGuid());
            }
            return "";
        }

        string GetLogSql_IP(string username, string sql, List<DbParameter> DbParameterS)
        {
            IoRyClass ic = new IoRyClass(this.Path);
            string ps = "<Parameters><SQL>" + sql + "</SQL>";
            foreach (var item in DbParameterS)
            {
                string cs = "<ParameterName>" + item.ParameterName + "</ParameterName><Value>" + item.Value.ToString() + "</Value>";
                ps += cs;
            }
            ps += "</Parameters>";
            string tablename = "";
            if (sql.ToLower().Contains("insert"))
            {
                tablename = sql.ToLower().Replace("insert", "").Replace("into", "").Split('(')[0].Trim();
                return string.Format(@";insert into log_data (sopreater_str,type_str,SQL_str,olddata_str,createtime_dt,tablename_str,UUID_GUID_str,IP_str,log_data_GUID) 
values ('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}');", username, "����", ps.Replace("'", "''"), "", "getdate()", tablename, IoRyClass.UUID, IoRyClass.PCIP, Guid.NewGuid());
            }
            if (sql.ToLower().Contains("update"))
            {
                tablename = sql.ToLower().Split(new string[] { "set" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("update", "").Trim();
                string sqlold = "select * from " + tablename + " where " + sql.ToLower().Split(new string[] { "where" }, StringSplitOptions.None)[1];
                DataTable oldtable = ic.GetTable_DbParameter(sqlold, DbParameterS);
                if (oldtable.Rows.Count == 0)
                {
                    return ";";
                }
                else
                {
                    //string old = "";
                    XElement xolddata = new XElement("OldData");
                    for (int j = 0; j < oldtable.Rows.Count; j++)
                    {
                        XElement row = new XElement("Row");
                        for (int i = 0; i < oldtable.Columns.Count; i++)
                        {
                            XElement col = new XElement("Column");
                            col.Add(new XAttribute("colName", oldtable.Columns[i].ColumnName));
                            col.Value = oldtable.Rows[j][i].ToString();
                            row.Add(col);
                            //old = old + " [ " + oldtable.Columns[i].ColumnName + " | " + oldtable.Rows[0][i].ToString() + " ]; ";
                        }
                        xolddata.Add(row);
                    }

                    return string.Format(@";insert into log_data (sopreater_str,type_str,SQL_str,olddata_str,createtime_dt,tablename_str,UUID_GUID_str,IP_str,log_data_GUID) 
values ('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}');", username, "�޸�", ps.Replace("'", "''"), xolddata.ToString().Replace("'", "''"), "getdate()", tablename, IoRyClass.UUID, IoRyClass.PCIP, Guid.NewGuid());
                }
            }
            if (sql.ToLower().Contains("delete"))
            {
                string sqlold = "";
                if (sql.ToLower().Contains("from"))
                {
                    if (sql.ToLower().Contains("where"))
                    {
                        tablename = sql.ToLower().Replace("delete", "").Replace("from", "").Split(new string[] { "where" }, StringSplitOptions.None)[0].Trim();
                    }
                    else
                    {
                        tablename = sql.ToLower().Replace("delete", "").Replace("from", "").Trim();
                    }
                    sqlold = sql.ToLower().Replace("delete", "select * ");
                }
                else
                {
                    if (sql.ToLower().Contains("where"))
                    {
                        tablename = sql.ToLower().Replace("delete", "").Split(new string[] { "where" }, StringSplitOptions.None)[0].Trim();
                    }
                    else
                    {
                        tablename = sql.ToLower().Replace("delete", "").Trim();
                    }
                    sqlold = sql.ToLower().Replace("delete", "select * from ");
                }
                DataTable oldtable = ic.GetTable_DbParameter(sqlold, DbParameterS);
                if (oldtable.Rows.Count > 0)
                {
                    XElement xolddata = new XElement("OldData");
                    for (int j = 0; j < oldtable.Rows.Count; j++)
                    {
                        XElement row = new XElement("Row");
                        for (int i = 0; i < oldtable.Columns.Count; i++)
                        {
                            XElement col = new XElement("Column");
                            col.Add(new XAttribute("colName", oldtable.Columns[i].ColumnName));
                            col.Value = oldtable.Rows[j][i].ToString();
                            row.Add(col);
                        }
                        xolddata.Add(row);
                    }


                    return string.Format(@";insert into log_data (sopreater_str,type_str,SQL_str,olddata_str,createtime_dt,tablename_str,UUID_GUID_str,IP_str,log_data_GUID)
values ('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}');", username, "ɾ��", ps.Replace("'", "''"), xolddata.ToString().Replace("'", "''"), "getdate()", tablename, IoRyClass.UUID, IoRyClass.PCIP, Guid.NewGuid());
                }
                else
                {
                    return "";
                }
            }
            if (sql.ToLower().Contains("select"))
            {
                if (sql.ToLower().Contains("where"))
                {
                    tablename = sql.ToLower().Split(new string[] { "where" }, StringSplitOptions.None)[0].Split(new string[] { "from" }, StringSplitOptions.None)[1].Trim();
                }
                else
                {
                    tablename = sql.ToLower().Split(new string[] { "from" }, StringSplitOptions.None)[1].Trim();
                }
                return string.Format(@";insert into log_data (sopreater_str,type_str,SQL_str,olddata_str,createtime_dt,tablename_str,UUID_GUID_str,IP_str,log_data_GUID)
values ('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}');", username, "��ѯ", ps.Replace("'", "''"), "", "getdate()", tablename, IoRyClass.UUID, IoRyClass.PCIP, Guid.NewGuid());
            }
            return "";
        }

        /// <summary>
        /// ���ɴ�����IP��UUID��ִ��sp��־���
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="SPName">�洢������</param>
        /// <param name="DbParameterS">���</param>
        /// <returns></returns>
        string GetLogSP_IP(string username, string SPName, List<DbParameter> DbParameterS)
        {
            IoRyClass ic = new IoRyClass(this.Path);
            string ps = "<Parameters><SPName>" + SPName + "</SPName>";
            foreach (var item in DbParameterS)
            {
                string cs = "<ParameterName>" + item.ParameterName + "</ParameterName><Value>" + item.Value.ToString() + "</Value>";
                ps += cs;
            }
            ps += "</Parameters>";
            string sql = string.Format(@";insert into log_data (sopreater_str,type_str,SQL_str,olddata_str,createtime_dt,tablename_str,UUID_GUID_str,IP_str,log_data_GUID) 
values ('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}');", username, "�洢����", ps.Replace("'", "''"), "", "getdate()", SPName, IoRyClass.UUID, IoRyClass.PCIP, Guid.NewGuid());
            return sql;
        }

        /// <summary>
        /// ����ִ��sql��������Log,ֻ֧��Sqlserver,oracle����,����
        /// </summary>
        /// <param name="sql">sql�����</param>
        /// <param name="username">ִ��sql�����û�</param>
        /// <returns>��Ӱ������</returns>
        public string Log_ExecuteSqlTran(string sql, string username)
        {
            switch (this.Contype)
            {
                case ConType.Sql:
                    string newsql = "";
                    if (sql.Contains(";"))
                    {
                        foreach (string sqls in sql.Split(';'))
                        {
                            newsql += this.GetLogSql_IP(username, sqls) + sqls;
                        }
                    }
                    else
                    {
                        newsql = this.GetLogSql_IP(username, sql) + sql;
                    }
                    return this.ExecuteSqlTran(newsql);
                default:
                    return null;
            }
        }

        /// <summary>
        /// ����ִ��sql��������Log,ֻ֧��Sqlserver,oracle����,����
        /// </summary>
        /// <param name="sql">sql�����</param>
        /// <param name="DbParameterS">����</param>
        /// <param name="username">ִ��sql�����û�</param>
        /// <returns></returns>
        public string Log_ExecuteSqlTran(string sql, List<DbParameter> DbParameterS, string username)
        {
            switch (this.Contype)
            {
                case ConType.Sql:
                    string newsql = "";
                    if (sql.Contains(";"))
                    {
                        foreach (string sqls in sql.Split(';'))
                        {
                            newsql += this.GetLogSql_IP(username, sqls, DbParameterS) + sqls;
                        }
                    }
                    else
                    {
                        newsql = this.GetLogSql_IP(username, sql, DbParameterS) + sql;
                    }
                    return this.ExecuteSqlTran_DbParameter(newsql, DbParameterS);
                default:
                    return null;
            }
        }

        /// <summary>
        /// ����־��ִ�д洢���̵ķ���
        /// </summary>
        /// <param name="SPname">�洢������</param>
        /// <param name="DbParameterS">����</param>
        /// <param name="username">ִ���û�</param>
        /// <returns></returns>
        public DataSet Log_ExecuteSP(string SPname, List<DbParameter> DbParameterS, string username)
        {
            switch (this.Contype)
            {
                case ConType.Sql:
                    string newsql = "";
                    newsql = this.GetLogSP_IP(username, SPname, DbParameterS);
                    this.ExecuteSql(newsql);
                    return this.ExecuteSP(SPname, DbParameterS);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Ŀǰֻ֧��sqlserver
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="username">ִ��sql�����û�</param>
        /// <returns></returns>
        public DataSet Log_GetDataSet(string sql, string username)
        {
            switch (this.Contype)
            {
                case ConType.Sql:
                    try
                    {
                        string newsql = "";
                        if (sql.Contains(";"))
                        {
                            foreach (string sqls in sql.Split(';'))
                            {
                                newsql += this.GetLogSql_IP(username, sqls) + sqls;
                            }
                        }
                        else
                        {
                            newsql = this.GetLogSql_IP(username, sql) + sql;
                        }
                        return this.GetDataSet(newsql);
                    }
                    catch (Exception me)
                    {
                        throw me;
                    }
                default:
                    return null;
            }
        }

        /// <summary>
        /// Ŀǰֻ֧��sqlserver
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="DbParameterS"></param>
        /// <param name="username">ִ��sql�����û�</param>
        /// <returns></returns>
        public DataSet Log_GetDataSet(string sql, List<DbParameter> DbParameterS, string username)
        {
            switch (this.Contype)
            {
                case ConType.Sql:
                    try
                    {
                        string newsql = "";
                        if (sql.Contains(";"))
                        {
                            foreach (string sqls in sql.Split(';'))
                            {
                                newsql += this.GetLogSql_IP(username, sqls, DbParameterS) + sqls;
                            }
                        }
                        else
                        {
                            newsql = this.GetLogSql_IP(username, sql, DbParameterS) + sql;
                        }
                        return this.GetDataSet_DbParameter(newsql, DbParameterS);
                    }
                    catch (Exception me)
                    {
                        throw me;
                    }
                default:
                    return null;
            }
        }

        #endregion

        #region ��ȡPCconfig ��winformDemo��PCconfig��һ��

        /// <summary>
        /// ��ȡȫ�ֱ���,��IP�޹�
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string PCconfigGlobalValue(string key)
        {
            string sql = string.Format("select value_str from PC_config where key_str='{0}'", key.Replace('\'', ' '));
            return PCconfigValue_Sql(sql);
        }

        /// <summary>
        /// ͨ��IP��UUID��ȡPCconfig ��winformDemo��PCconfig��һ��
        /// </summary>
        /// <param name="key">config��key</param>
        /// <returns></returns>
        public string PCconfigValue(string key)
        {
            string sql = string.Format("select value_str from PC_config where IP_str='{0}' and UUID_GUID='{1}' and key_str='{2}'", IoRyClass.PCIP, IoRyClass.UUID, key.Replace('\'', ' '));
            return PCconfigValue_Sql(sql);
        }

        /// <summary>
        /// ͨ������IP��ȡPCconfig ��winformDemo��PCconfig��һ��
        /// </summary>
        /// <param name="key">config��key</param>
        /// <returns></returns>
        public string PCconfigValueByIP(string key)
        {
            string sql = string.Format("select value_str from PC_config where IP_str='{0}' and key_str='{1}' ", IoRyClass.PCIP, key.Replace('\'', ' '));
            return PCconfigValue_Sql(sql);
        }

        /// <summary>
        /// ͨ��IP��ȡPCconfig ��winformDemo��PCconfig��һ��
        /// </summary>
        /// <param name="key">config��key</param>
        /// <param name="IP"></param>
        /// <returns></returns>
        public string PCconfigValueByIP(string key, string IP)
        {
            string sql = string.Format("select value_str from PC_config where IP_str='{0}' and key_str='{1}'", IP.Replace('\'', ' '), key.Replace('\'', ' '));
            return PCconfigValue_Sql(sql);
        }

        /// <summary>
        /// ��ȡPCconfig ��winformDemo��PCconfig��һ��
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>����Ҳ����Ļ�,�򷵻�null</returns>
        string PCconfigValue_Sql(string sql)
        {
            DataTable dt = this.GetTable(sql);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            if (dt.Rows.Count > 1)
            {
                throw new Exception("�ҵ����value,�봦�����ݿ�!");
            }
            else
            {
                return Convert.ToString(dt.Rows[0]["value_str"]);
            }
        }

        #endregion

        #region CSV�ļ�
        /// <summary>
        /// ע�������õ���gb2312,��������һ��Excel��CSV�ĸ�ʽ
        /// </summary>
        /// <param name="dt">����</param>
        /// <param name="path">·��</param>
        /// <returns></returns>
        public void CSV_Create(DataTable dt, string path)
        {
            try
            {
                System.IO.FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                System.Text.Encoding gb2312 = System.Text.Encoding.GetEncoding("gb2312");
                StreamWriter sw = new StreamWriter(fs, gb2312);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sw.Write(dt.Columns[i].ColumnName);
                    if (i == dt.Columns.Count - 1)
                    {
                        sw.WriteLine();
                    }
                    else
                    {
                        sw.Write(",");
                    }
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        sw.Write(Convert.ToString(dt.Rows[i][j]));
                        if (j < dt.Columns.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    if (i < dt.Rows.Count - 1)
                    {
                        sw.WriteLine();
                    }
                }
                sw.Flush();
                sw.Close();
            }
            catch (Exception me)
            {
                throw me;
            }
        }

        #endregion

        #region OLEDB��ȡExcel ע��Ҫ��װ https://www.microsoft.com/zh-CN/download/details.aspx?id=13255 ��x86�汾

        /// <summary>
        /// OLEDB��ȡExcel ע��Ҫ��װ https://www.microsoft.com/zh-CN/download/details.aspx?id=13255 ��x86�汾
        /// sqlʾ��:Select * From [Sheet1$]
        /// 20200318 ������Excel����������,���������ȡ,Ҳ�����������ʽ
        /// </summary>
        /// <param name="filePath">�ļ���ַ</param>
        /// <param name="sql">sqlʾ��:Select * From [Sheet1$]</param>
        /// <returns></returns>
        public DataSet Excel_Get(string filePath, string sql)
        {
            try
            {
                // �������
                string connStr = this.GetExcelReadonlyConnStr(filePath);

                if (File.Exists(filePath))
                {
                    OleDbDataAdapter myCommand = null;
                    DataSet ds = null;
                    using (OleDbConnection conn = new OleDbConnection(connStr))
                    {
                        conn.Open();
                        myCommand = new OleDbDataAdapter(sql, conn);
                        ds = new DataSet();
                        myCommand.Fill(ds, "Sheet");
                        return ds;
                    }
                }
                else
                {
                    throw new Exception("�Ҳ����ļ�:" + filePath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion
    }

    #region ö��
    /// <summary>
    /// ��������
    /// </summary>
    public enum ConType
    {
        /// <summary>
        /// ��
        /// </summary>
        Null,
        /// <summary>
        /// sql
        /// </summary>
        Sql,
        /// <summary>
        /// access
        /// </summary>
        Access,
        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,
        /// <summary>
        /// Excel
        /// </summary>
        Excel
    }

    #endregion
}
