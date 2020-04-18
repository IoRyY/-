using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections;

namespace yezhanbafang.fw.MSoffice
{
    /// <summary>
    /// Excel��ӡ��ί��
    /// </summary>
    /// <returns></returns>
    public delegate object myPrint(object o);//(Microsoft.Office.Interop.Excel.Application myexcel);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lls"></param>
    /// <returns></returns>
    public delegate List<List<string>> mySave();


    //�������ھ��崰���У�Ӧ�ô˷���������
    //private void button3_Click(object sender, EventArgs e)
    //{
    //    IoRyWinFormClass.IoRyYExcel myex = new IoRyWinFormClass.IoRyYExcel();
    //    myex._Print += new IoRyWinFormClass.myPrint(myex__Print);
    //    myex.Print("aaa.xls");
    //}

    //object myex__Print(object o)
    //{
    //    //myExcel.Cells[3, 1] = "sdfsdfs!";
    //    _Worksheet myex = (_Worksheet)o;
    //    myex.Cells[1, 1] = "niux";
    //    return myex;
    //}


    /// <summary>
    /// ����excel����
    /// </summary>
    public class IoRyYExcel
    {
        #region ��ӡ�¼�

        /// <summary>
        /// ��ӡ�¼�
        /// </summary>
        public event myPrint _Print;

        public event mySave SaveEvent;


        /// <summary>
        /// ��ӡ�Ĺ���
        /// </summary>
        /// <param name="ExcelPath">Excel�ļ�������</param>
        public void Print(string ExcelPath)
        {
            //����Excel Applicatione
            _Application myExcel = null;
            //���û�퓲�e 
            _Workbook myBook = null;
            //���ù�����e
            _Worksheet mySheet = null;
            try
            {
                //�_��һ���µđ��ó�ʽ
                myExcel = new Microsoft.Office.Interop.Excel.Application();
                myExcel.Workbooks.Open(ExcelPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                //ͣ�þ���ӍϢ
                myExcel.DisplayAlerts = false;
                //׌Excel�ļ���Ҋ
                myExcel.Visible = false;
                //���õ�һ����퓲�
                myBook = myExcel.Workbooks[1];
                //�O����퓲����c
                myBook.Activate();
                //���õ�һ��������
                mySheet = (_Worksheet)myBook.Worksheets[1];
                //�O�������c
                mySheet.Activate();

                //������¼���Excel��ĸ�ֵ���������£�
                //mySheet.Cells[3, 1] = "sdfsdfs!";
                if (_Print != null)
                {
                    Microsoft.Office.Interop.Excel._Worksheet excel = (Microsoft.Office.Interop.Excel._Worksheet)_Print(mySheet);

                    //myBook.Save();
                    //ֱ����Ĭ�ϴ�ӡ��.
                    myBook.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    //�P�]��퓲�
                    //myBook.Close(false, Type.Missing, Type.Missing);
                    //�P�]Excel
                    //excel.Quit();
                }
                //MessageBox.Show("��ӡ�ɹ���");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //ጷ�Excel�YԴ
                myBook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myBook);
                myExcel.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myExcel);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        #endregion

        public void Save(string ExcelPath)
        {
            //����Excel Applicatione
            _Application myExcel = null;
            //���û�퓲�e 
            _Workbook myBook = null;
            //���ù�����e
            _Worksheet mySheet = null;
            try
            {
                //�_��һ���µđ��ó�ʽ
                myExcel = new Microsoft.Office.Interop.Excel.Application();
                myExcel.Workbooks.Open(ExcelPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                //ͣ�þ���ӍϢ
                myExcel.DisplayAlerts = false;
                //׌Excel�ļ���Ҋ
                myExcel.Visible = false;
                //���õ�һ����퓲�
                myBook = myExcel.Workbooks[1];
                //�O����퓲����c
                myBook.Activate();
                //���õ�һ��������
                mySheet = (_Worksheet)myBook.Worksheets[1];
                //�O�������c
                mySheet.Activate();

                //������¼���Excel��ĸ�ֵ���������£�
                //mySheet.Cells[3, 1] = "sdfsdfs!";
                if (SaveEvent != null)
                {
                    //Microsoft.Office.Interop.Excel._Worksheet excel = (Microsoft.Office.Interop.Excel._Worksheet)SaveEvent(mySheet);
                    List<List<string>> fuzhi = SaveEvent();
                    for (int i = 0; i < fuzhi.Count; i++)
                    {
                        for (int j = 0; j < fuzhi[i].Count; j++)
                        {
                            if (fuzhi[i][j] != null)
                            {
                                mySheet.Cells[i + 1, j + 1] = fuzhi[i][j];
                            }
                        }
                    }
                    //����
                    myBook.Save();
                    //�P�]��퓲�
                    //myBook.Close(false, Type.Missing, Type.Missing);
                    //�P�]Excel
                    //excel.Quit();
                }
                //MessageBox.Show("��ӡ�ɹ���");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //ጷ�Excel�YԴ
                myBook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myBook);
                myExcel.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myExcel);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// ��ȡExcel������Xml.��һ�н���Ϊ�����������Ժ��� ע��ֻ�ܶ�ȡ��һҳ
        /// ע��,Excel�е�ʱ���ʽ,��ȡ�����Ժ�,���������·�ʽת������
        /// DateTime.FromOADate(double.Parse(item.Element("Column" + i.ToString()).Value)))
        /// </summary>
        /// <param name="ReadExcelPath">��ȡExcel��·��</param>
        /// <returns></returns>
        static public string ReadExcelReturnXml(string ReadExcelPath)
        {
            //����Excel Applicatione
            _Application myExcel = null;
            //���û�퓲�e 
            _Workbook myBook = null;
            //���ù�����e
            _Worksheet mySheet = null;
            try
            {
                //�_��һ���µđ��ó�ʽ
                myExcel = new Microsoft.Office.Interop.Excel.Application();
                //myExcel.Workbooks.Open(AppDomain.CurrentDomain.BaseDirectory + ReadExcelPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                myExcel.Workbooks.Open(ReadExcelPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                //ͣ�þ���ӍϢ
                myExcel.DisplayAlerts = false;
                //׌Excel�ļ���Ҋ
                myExcel.Visible = false;
                //���õ�һ����퓲�
                myBook = myExcel.Workbooks[1];
                //�O����퓲����c
                myBook.Activate();
                //���õ�һ��������
                mySheet = (_Worksheet)myBook.Worksheets[1];
                //�O�������c
                mySheet.Activate();

                Array myvalues = (Array)mySheet.UsedRange.Cells.Value2;
                int lie = mySheet.UsedRange.Columns.Count;
                int hang = mySheet.UsedRange.Rows.Count;

                XElement xmlTree = new XElement("ExcelContent");

                for (int mhang = 1; mhang <= hang; mhang++)
                {
                    xmlTree.Add(new XElement("Row"));
                    for (int mlie = 1; mlie <= lie; mlie++)
                    {
                        string mzhi = Convert.ToString(myvalues.GetValue(mhang, mlie));
                        xmlTree.Elements("Row").Last().Add(new XElement("Column" + mlie.ToString(), mzhi));
                    }
                }
                return xmlTree.ToString();
            }
            catch (Exception me)
            {
                throw me;
            }
            finally
            {
                //ጷ�Excel�YԴ
                myBook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myBook);
                myExcel.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myExcel);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// ��ָ����DataTable�������������ģ��Excel���������ָ��λ��
        /// </summary>
        /// <param name="InputDGV">�����DataGridView</param>
        /// <param name="TemplatesExcelPath">ģ��Excelλ��</param>
        /// <param name="OutputExcelPath">�����Excelλ��</param>
        /// <param name="StartRow">Excel�е�һ�г��ֵ�λ��,��������ͨ��ģ��������ͷ</param>
        /// <returns></returns>
        static public bool OutputExcelFromDataTable(System.Data.DataTable InputDt, string TemplatesExcelPath, string OutputExcelPath, int StartRow)
        {
            //����Excel Applicatione
            _Application myExcel = null;
            //���û�퓲�e 
            _Workbook myBook = null;
            //���ù�����e
            _Worksheet mySheet = null;
            try
            {
                //�_��һ���µđ��ó�ʽ
                myExcel = new Microsoft.Office.Interop.Excel.Application();
                myExcel.Workbooks.Open(AppDomain.CurrentDomain.BaseDirectory + TemplatesExcelPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                //ͣ�þ���ӍϢ
                myExcel.DisplayAlerts = false;
                //׌Excel�ļ���Ҋ
                myExcel.Visible = false;
                //���õ�һ����퓲�
                myBook = myExcel.Workbooks[1];
                //�O����퓲����c
                myBook.Activate();
                //���õ�һ��������
                mySheet = (_Worksheet)myBook.Worksheets[1];
                //�O�������c
                mySheet.Activate();

                if (InputDt.Rows.Count != 0)
                {
                    for (int i = 0; i < InputDt.Rows.Count; i++)
                    {
                        for (int j = 0; j < InputDt.Columns.Count; j++)
                        {
                            mySheet.Cells[i + StartRow + 1, j + 1] = Convert.ToString(InputDt.Rows[i][j]);
                        }
                    }
                }


                myBook.SaveAs(OutputExcelPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                //MessageBox.Show("����ɹ���");
                return true;
            }
            catch (Exception me)
            {
                //MessageBox.Show("��ȷ���Ƿ���ͬ��Excel�ļ�  " + me.Message);
                throw me;
                //return false;
            }
            finally
            {
                //ጷ�Excel�YԴ
                myBook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myBook);
                myExcel.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myExcel);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        /// <summary>
        /// ��ָ����DataTable�������������ģ��Excel���������ָ��λ�� ����·����Ҫ����ƴ�ӳɾ���·��
        /// </summary>
        /// <param name="InputDGV">�����DataGridView</param>
        /// <param name="TemplatesExcelPath">ģ��Excelλ��</param>
        /// <param name="OutputExcelPath">�����Excelλ��</param>
        /// <param name="StartRow">Excel�е�һ�г��ֵ�λ��,��������ͨ��ģ��������ͷ</param>
        /// <returns></returns>
        static public bool OutputExcelFromDataTable_path(System.Data.DataTable InputDt, string TemplatesExcelPath, string OutputExcelPath, int StartRow)
        {
            //����Excel Applicatione
            _Application myExcel = null;
            //���û�퓲�e 
            _Workbook myBook = null;
            //���ù�����e
            _Worksheet mySheet = null;
            try
            {
                //�_��һ���µđ��ó�ʽ
                myExcel = new Microsoft.Office.Interop.Excel.Application();
                myExcel.Workbooks.Open(TemplatesExcelPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                //ͣ�þ���ӍϢ
                myExcel.DisplayAlerts = false;
                //׌Excel�ļ���Ҋ
                myExcel.Visible = false;
                //���õ�һ����퓲�
                myBook = myExcel.Workbooks[1];
                //�O����퓲����c
                myBook.Activate();
                //���õ�һ��������
                mySheet = (_Worksheet)myBook.Worksheets[1];
                //�O�������c
                mySheet.Activate();

                if (InputDt.Rows.Count != 0)
                {
                    for (int i = 0; i < InputDt.Rows.Count; i++)
                    {
                        for (int j = 0; j < InputDt.Columns.Count; j++)
                        {
                            mySheet.Cells[i + StartRow + 1, j + 1] = Convert.ToString(InputDt.Rows[i][j]);
                        }
                    }
                }


                myBook.SaveAs(OutputExcelPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                //MessageBox.Show("����ɹ���");
                return true;
            }
            catch (Exception me)
            {
                //MessageBox.Show("��ȷ���Ƿ���ͬ��Excel�ļ�  " + me.Message);
                throw me;
                //return false;
            }
            finally
            {
                //ጷ�Excel�YԴ
                myBook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myBook);
                myExcel.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myExcel);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        /// <summary>
        /// ����Html��ʽ��Excel����������Web��ʾ
        /// </summary>
        /// <param name="ExcelPath"></param>
        /// <returns></returns>
        static public bool CreatHtml(string ExcelPath, string OutPath)
        {
            //����Excel Applicatione
            _Application myExcel = null;
            //���û�퓲�e 
            _Workbook myBook = null;
            //���ù�����e
            _Worksheet mySheet = null;
            try
            {
                //�_��һ���µđ��ó�ʽ
                myExcel = new Microsoft.Office.Interop.Excel.Application();
                myExcel.Workbooks.Open(AppDomain.CurrentDomain.BaseDirectory + ExcelPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                //ͣ�þ���ӍϢ
                myExcel.DisplayAlerts = false;
                //׌Excel�ļ���Ҋ
                myExcel.Visible = false;
                //���õ�һ����퓲�
                myBook = myExcel.Workbooks[1];
                //�O����퓲����c
                myBook.Activate();
                //���õ�һ��������
                mySheet = (_Worksheet)myBook.Worksheets[1];
                //�O�������c
                mySheet.Activate();
                //����Html��Excelģ��
                object format = Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml;

                IEnumerator wsEnumerator =
                myExcel.ActiveWorkbook.Worksheets.GetEnumerator();
                int i = 1;
                //
                while (wsEnumerator.MoveNext())
                {
                    Microsoft.Office.Interop.Excel.Worksheet wsCurrent =
                    (Microsoft.Office.Interop.Excel.Worksheet)wsEnumerator.Current;
                    String outputFile = AppDomain.CurrentDomain.BaseDirectory + OutPath + "." + i.ToString() + ".html";
                    wsCurrent.SaveAs(outputFile, format, Type.Missing, Type.Missing, Type.Missing,
                     Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    ++i;
                }

                return true;
            }
            catch (Exception me)
            {
                throw me;
            }
            finally
            {
                myBook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myBook);
                myExcel.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myExcel);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
