﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/************************************************************************************
 * 作者 袁东辉 时间：2017-10
 * Email windy_23762872@126.com 253625488@qq.com
 * 作用 用来控制每一列的各种属性,以便后续的增,删,改等处理
 * VS版本 2010 2013 2015
 * 通过此属性来控制显示层的文字描述
 ***********************************************************************************/

namespace CreateDataTableTool
{
    [AttributeUsage(AttributeTargets.All)]
    public class IoRyDisPlayAttribute : Attribute
    {
        public string DisplayName { get; set; }
    }
}
