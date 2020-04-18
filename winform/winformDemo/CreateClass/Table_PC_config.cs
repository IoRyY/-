﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

/************************************************************************************
 * 作者 袁东辉 时间：2016-1 最后修改时间：2020-3
 * Email windy_23762872@126.com 253625488@qq.com
 * 博客 2016BigProject http://blog.csdn.net/yuandonghuia/article/details/50514985
 * 作用 代码生成器生成的Table类
 * VS版本 2010 2013 2015 2019
 ***********************************************************************************/

namespace CreateDataTableTool
{
    /// <summary>
    /// 自定义前缀+表名称为类名
    /// </summary>
    public class PC_config : IoRyRow
    {
        Guid? _PC_config_GUID;
        /// <summary>
        /// 数据库PC_config_GUID字段
        /// </summary>
        [IoRyDisPlay(DisplayName ="")]
        public Guid? PC_config_GUID
        {
            get
            {
                return _PC_config_GUID;
            }
            set
            {
                _PC_config_GUID = value;
                if (value == null)
                {
                    LIC.Where(x => x.ioryName == "PC_config_GUID").First().ioryValueNull = true;
                }
                else
                {
                    LIC.Where(x => x.ioryName == "PC_config_GUID").First().ioryValueNull = false;
                }
                LIC.Where(x => x.ioryName == "PC_config_GUID").First().ioryValueChange = true;
                LIC.Where(x => x.ioryName == "PC_config_GUID").First().ioryValue = Convert.ToString(value);
            }
        }

        Guid? _UUID_GUID;
        /// <summary>
        /// 数据库UUID_GUID字段
        /// </summary>
        [IoRyDisPlay(DisplayName ="")]
        public Guid? UUID_GUID
        {
            get
            {
                return _UUID_GUID;
            }
            set
            {
                _UUID_GUID = value;
                if (value == null)
                {
                    LIC.Where(x => x.ioryName == "UUID_GUID").First().ioryValueNull = true;
                }
                else
                {
                    LIC.Where(x => x.ioryName == "UUID_GUID").First().ioryValueNull = false;
                }
                LIC.Where(x => x.ioryName == "UUID_GUID").First().ioryValueChange = true;
                LIC.Where(x => x.ioryName == "UUID_GUID").First().ioryValue = Convert.ToString(value);
            }
        }

        string _IP_str;
        /// <summary>
        /// 数据库IP_str字段
        /// </summary>
        [IoRyDisPlay(DisplayName ="")]
        public string IP_str
        {
            get
            {
                return _IP_str;
            }
            set
            {
                _IP_str = value;
                if (value == null)
                {
                    LIC.Where(x => x.ioryName == "IP_str").First().ioryValueNull = true;
                }
                else
                {
                    LIC.Where(x => x.ioryName == "IP_str").First().ioryValueNull = false;
                }
                LIC.Where(x => x.ioryName == "IP_str").First().ioryValueChange = true;
                LIC.Where(x => x.ioryName == "IP_str").First().ioryValue = Convert.ToString(value);
            }
        }

        string _key_str;
        /// <summary>
        /// 数据库key_str字段
        /// </summary>
        [IoRyDisPlay(DisplayName ="")]
        public string key_str
        {
            get
            {
                return _key_str;
            }
            set
            {
                _key_str = value;
                if (value == null)
                {
                    LIC.Where(x => x.ioryName == "key_str").First().ioryValueNull = true;
                }
                else
                {
                    LIC.Where(x => x.ioryName == "key_str").First().ioryValueNull = false;
                }
                LIC.Where(x => x.ioryName == "key_str").First().ioryValueChange = true;
                LIC.Where(x => x.ioryName == "key_str").First().ioryValue = Convert.ToString(value);
            }
        }

        string _value_str;
        /// <summary>
        /// 数据库value_str字段
        /// </summary>
        [IoRyDisPlay(DisplayName ="")]
        public string value_str
        {
            get
            {
                return _value_str;
            }
            set
            {
                _value_str = value;
                if (value == null)
                {
                    LIC.Where(x => x.ioryName == "value_str").First().ioryValueNull = true;
                }
                else
                {
                    LIC.Where(x => x.ioryName == "value_str").First().ioryValueNull = false;
                }
                LIC.Where(x => x.ioryName == "value_str").First().ioryValueChange = true;
                LIC.Where(x => x.ioryName == "value_str").First().ioryValue = Convert.ToString(value);
            }
        }

        DateTime? _createtime_dt;
        /// <summary>
        /// 数据库createtime_dt字段
        /// </summary>
        [IoRyDisPlay(DisplayName ="")]
        public DateTime? createtime_dt
        {
            get
            {
                return _createtime_dt;
            }
            set
            {
                _createtime_dt = value;
                if (value == null)
                {
                    LIC.Where(x => x.ioryName == "createtime_dt").First().ioryValueNull = true;
                }
                else
                {
                    LIC.Where(x => x.ioryName == "createtime_dt").First().ioryValueNull = false;
                }
                LIC.Where(x => x.ioryName == "createtime_dt").First().ioryValueChange = true;
                LIC.Where(x => x.ioryName == "createtime_dt").First().ioryValue = Convert.ToString(value);
            }
        }

        DateTime? _changetime_dt;
        /// <summary>
        /// 数据库changetime_dt字段
        /// </summary>
        [IoRyDisPlay(DisplayName ="")]
        public DateTime? changetime_dt
        {
            get
            {
                return _changetime_dt;
            }
            set
            {
                _changetime_dt = value;
                if (value == null)
                {
                    LIC.Where(x => x.ioryName == "changetime_dt").First().ioryValueNull = true;
                }
                else
                {
                    LIC.Where(x => x.ioryName == "changetime_dt").First().ioryValueNull = false;
                }
                LIC.Where(x => x.ioryName == "changetime_dt").First().ioryValueChange = true;
                LIC.Where(x => x.ioryName == "changetime_dt").First().ioryValue = Convert.ToString(value);
            }
        }

        /// <summary>
        /// 实现IoRyTable的接口
        /// </summary>
        public void SetData(DataRow dr)
        {
            PC_config_GUID = dr.Field<Guid?>("PC_config_GUID");
            UUID_GUID = dr.Field<Guid?>("UUID_GUID");
            IP_str = dr.Field<string>("IP_str");
            key_str = dr.Field<string>("key_str");
            value_str = dr.Field<string>("value_str");
            createtime_dt = dr.Field<DateTime?>("createtime_dt");
            changetime_dt = dr.Field<DateTime?>("changetime_dt");
            foreach (var item in LIC)
            {
                item.ioryValueChange = false;
            }
        }
        /// <summary>
        /// 初始化函数
        /// </summary>
        public PC_config()
        {
            LIC.Add(new IoRyCol
            {
                ioryName = "PC_config_GUID",
                ioryType = "Guid?",
                IsIdentity = false,
                IsKey = true,
                IsNull = false,
                ioryValueNull = true,
                ioryValueChange = false
            });
            LIC.Add(new IoRyCol
            {
                ioryName = "UUID_GUID",
                ioryType = "Guid?",
                IsIdentity = false,
                IsKey = false,
                IsNull = true,
                ioryValueNull = true,
                ioryValueChange = false
            });
            LIC.Add(new IoRyCol
            {
                ioryName = "IP_str",
                ioryType = "string",
                IsIdentity = false,
                IsKey = false,
                IsNull = true,
                ioryValueNull = true,
                ioryValueChange = false
            });
            LIC.Add(new IoRyCol
            {
                ioryName = "key_str",
                ioryType = "string",
                IsIdentity = false,
                IsKey = false,
                IsNull = true,
                ioryValueNull = true,
                ioryValueChange = false
            });
            LIC.Add(new IoRyCol
            {
                ioryName = "value_str",
                ioryType = "string",
                IsIdentity = false,
                IsKey = false,
                IsNull = true,
                ioryValueNull = true,
                ioryValueChange = false
            });
            LIC.Add(new IoRyCol
            {
                ioryName = "createtime_dt",
                ioryType = "DateTime?",
                IsIdentity = false,
                IsKey = false,
                IsNull = true,
                ioryValueNull = true,
                ioryValueChange = false
            });
            LIC.Add(new IoRyCol
            {
                ioryName = "changetime_dt",
                ioryType = "DateTime?",
                IsIdentity = false,
                IsKey = false,
                IsNull = true,
                ioryValueNull = true,
                ioryValueChange = false
            });
        }

        string tablename = "PC_config";﻿
        /// <summary>
        /// LIC是列集合
        /// i前缀+列名为字段名
        /// </summary>
        List<IoRyCol> LIC = new List<IoRyCol>();

        /// <summary>
        /// 普通新增
        /// </summary>
        /// <returns></returns>
        public void IoRyAdd()
        {
            string sqlp = " insert into " + tablename + " ({0}) values ({1})";
            List<string> lscname = new List<string>();
            List<string> lscvalue = new List<string>();
            foreach (IoRyCol item in this.LIC)
            {
                if (item.ioryValueNull == false && item.IsIdentity == false)
                {
                    lscname.Add(item.ioryName);
                    lscvalue.Add("'" + item.ioryValue.Replace("'", "''") + "'");
                }
            }
            if (lscname.Count == 0)
            {
                throw new Exception("新增的类必须有值!");
            }
            string sql = string.Format(sqlp, string.Join(",", lscname), string.Join(",", lscvalue));
            IoRyFunction.CallIoRyClass(sql);
        }

        /// <summary>
        /// 带Log新增
        /// </summary>
        /// <returns></returns>
        public void IoRyAdd(string cuser)
        {
            string sqlp = " insert into " + tablename + " ({0}) values ({1})";
            List<string> lscname = new List<string>();
            List<string> lscvalue = new List<string>();
            foreach (IoRyCol item in this.LIC)
            {
                if (item.ioryValueNull == false && item.IsIdentity == false)
                {
                    lscname.Add(item.ioryName);
                    lscvalue.Add("'" + item.ioryValue.Replace("'", "''") + "'");
                }
            }
            if (lscname.Count == 0)
            {
                throw new Exception("新增的类必须有值!");
            }
            string sql = string.Format(sqlp, string.Join(",", lscname), string.Join(",", lscvalue));
            IoRyFunction.CallIoRyClass(sql, cuser);
        }

        /// <summary>
        /// 自定义where 修改
        /// </summary>
        /// <param name="keys"></param>
        public void IoRyUpdate(List<string> keys)
        {
            string sqlp = "update " + tablename + " set {0} where {1}";
            List<string> lsset = new List<string>();
            List<string> lswhere = new List<string>();
            if (LIC.Any(x => x.ioryValueChange == true))
            {
                foreach (var item in LIC)
                {
                    if (item.ioryValueChange == true)
                    {
                        if (!keys.Contains(item.ioryName))
                        {
                            if (item.ioryValueNull)
                            {
                                lsset.Add(item.ioryName + " = null ");
                            }
                            else
                            {
                                lsset.Add(item.ioryName + "='" + item.ioryValue.Replace("'", "''") + "'");
                            }
                        }
                    }
                }
                foreach (var item in keys)
                {
                    string mv = LIC.Where(x => x.ioryName == item).First().ioryValue;
                    lswhere.Add(item + "='" + mv + "'");
                }
                string sql = string.Format(sqlp, string.Join(",", lsset), string.Join(" and ", lswhere));
                IoRyFunction.CallIoRyClass(sql);
            }
            else
            {
                throw new Exception("此数据没有修改!");
            }
        }

        /// <summary>
        /// 自定义where 带Log 修改
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="cuser"></param>
        public void IoRyUpdate(List<string> keys, string cuser)
        {
            string sqlp = "update " + tablename + " set {0} where {1}";
            List<string> lsset = new List<string>();
            List<string> lswhere = new List<string>();
            if (LIC.Any(x => x.ioryValueChange == true))
            {
                foreach (var item in LIC)
                {
                    if (item.ioryValueChange == true)
                    {
                        if (!keys.Contains(item.ioryName))
                        {
                            if (item.ioryValueNull)
                            {
                                lsset.Add(item.ioryName + " = null ");
                            }
                            else
                            {
                                lsset.Add(item.ioryName + "='" + item.ioryValue.Replace("'", "''") + "'");
                            }
                        }
                    }
                }
                foreach (var item in keys)
                {
                    string mv = LIC.Where(x => x.ioryName == item).First().ioryValue;
                    lswhere.Add(item + "='" + mv + "'");
                }
                string sql = string.Format(sqlp, string.Join(",", lsset), string.Join(" and ", lswhere));
                IoRyFunction.CallIoRyClass(sql, cuser);
            }
            else
            {
                throw new Exception("此数据没有修改!");
            }
        }

        /// <summary>
        /// 普通修改 以keys为where
        /// </summary>
        public void IoRyUpdate()
        {
            List<string> ls = LIC.Where(x => x.IsKey == true).Select(x => x.ioryName).ToList();
            this.IoRyUpdate(ls);
        }

        /// <summary>
        /// 带Log修改 以keys为where
        /// </summary>
        /// <param name="cuser"></param>
        public void IoRyUpdate(string cuser)
        {
            List<string> ls = LIC.Where(x => x.IsKey == true).Select(x => x.ioryName).ToList();
            this.IoRyUpdate(ls, cuser);
        }

        /// <summary>
        /// 普通删除 自定义where
        /// </summary>
        /// <param name="keys"></param>
        public void IoRyDelete(List<string> keys)
        {
            string sqlp = "delete " + tablename + " where {0}";
            List<string> lswhere = new List<string>();
            foreach (var item in keys)
            {
                string mv = LIC.Where(x => x.ioryName == item).First().ioryValue;
                lswhere.Add(item + "='" + mv + "'");
            }
            string sql = string.Format(sqlp, string.Join(" and ", lswhere));
            IoRyFunction.CallIoRyClass(sql);
        }

        /// <summary>
        /// 带Log删除 自定义where
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="cuser"></param>
        public void IoRyDelete(List<string> keys, string cuser)
        {
            string sqlp = "delete " + tablename + " where {0}";
            List<string> lswhere = new List<string>();
            foreach (var item in keys)
            {
                string mv = LIC.Where(x => x.ioryName == item).First().ioryValue;
                lswhere.Add(item + "='" + mv + "'");
            }
            string sql = string.Format(sqlp, string.Join(" and ", lswhere));
            IoRyFunction.CallIoRyClass(sql, cuser);
        }

        /// <summary>
        /// 普通删除 以keys为where 
        /// </summary>
        public void IoRyDelete()
        {
            List<string> ls = LIC.Where(x => x.IsKey == true).Select(x => x.ioryName).ToList();
            this.IoRyDelete(ls);
        }

        /// <summary>
        /// 带Log删除 以keys为where
        /// </summary>
        /// <param name="cuser"></param>
        public void IoRyDelete(string cuser)
        {
            List<string> ls = LIC.Where(x => x.IsKey == true).Select(x => x.ioryName).ToList();
            this.IoRyDelete(ls, cuser);
        }
    }
}