using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Infrastruture.Utility
{
    public static class TempAnalysisTableHelper
    {
        /// <summary>
        /// 将统计表转换为以时间为字段的横表（按小时统计）（自定义时间段统计）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable VerticalToHorizontalHourly(DataTable source, DateTime startTime, DateTime endTime)
        {
            DataTable destination = new DataTable();

            for (DateTime hourLooper = startTime; hourLooper <= endTime; hourLooper = hourLooper.AddHours(1))
            {
                DataColumn dc = new DataColumn(hourLooper.ToString("yyyy-MM-dd-HH"), typeof(decimal));
                dc.DefaultValue = 0;
                destination.Columns.Add(dc);
            }

            if (source.Columns.Contains("OrganizationID") && source.Columns.Contains("VariableId"))
            {
                DataRow dr = destination.NewRow();

                for (int i = 0; i < source.Rows.Count; i++)
                {
                    if (i > 0 && (source.Rows[i]["OrganizationID"].ToString() != source.Rows[i - 1]["OrganizationID"].ToString() || source.Rows[i]["VariableId"].ToString() != source.Rows[i - 1]["VariableId"].ToString()))
                    {
                        destination.Rows.Add(dr);
                        dr = destination.NewRow();
                    }
                    string columnName = source.Rows[i]["Year"].ToString() + "-" + ((int)source.Rows[i]["Month"]).ToString("00") + "-" + ((int)source.Rows[i]["Day"]).ToString("00") + "-" + ((int)source.Rows[i]["Hour"]).ToString("00");
                    dr[columnName] = (decimal)source.Rows[i]["Sum"];
                }

                destination.Rows.Add(dr);
            }
            else
            {
                DataRow dr = destination.NewRow();

                for (int i = 0; i < source.Rows.Count; i++)
                {
                    string columnName = source.Rows[i]["Year"].ToString() + "-" + ((int)source.Rows[i]["Month"]).ToString("00") + "-" + ((int)source.Rows[i]["Day"]).ToString("00") + "-" + ((int)source.Rows[i]["Hour"]).ToString("00");
                    dr[columnName] = (decimal)source.Rows[i]["Sum"];
                }

                destination.Rows.Add(dr);
            }

            return destination;
        }
        public static DataTable VerticalToHorizontalByInterval(DataTable source, DateTime startTime, DateTime endTime, string myTimeInterval)
        {
            DataTable destination = new DataTable();
            if (myTimeInterval == "10")    //10分钟
            {
                string m_StartTime = startTime.ToString("yyyy-MM-dd HH") + ":" + (startTime.Minute / 10).ToString() + "0";
                for (DateTime hourLooper = DateTime.Parse(m_StartTime); hourLooper <= endTime; hourLooper = hourLooper.AddMinutes(10))
                {
                    DataColumn dc = new DataColumn(hourLooper.ToString("MM月dd日HH时mm分"), typeof(decimal));
                    dc.DefaultValue = 0;
                    destination.Columns.Add(dc);
                }

                if (source.Columns.Contains("OrganizationId") && source.Columns.Contains("VariableId"))
                {
                    DataRow dr = destination.NewRow();

                    for (int i = 0; i < source.Rows.Count; i++)
                    {
                        if (i > 0 && (source.Rows[i]["OrganizationId"].ToString() != source.Rows[i - 1]["OrganizationId"].ToString() || source.Rows[i]["VariableId"].ToString() != source.Rows[i - 1]["VariableId"].ToString()))
                        {
                            destination.Rows.Add(dr);
                            dr = destination.NewRow();
                        }
                        string columnName = ((int)source.Rows[i]["Month"]).ToString("00") + "月" + ((int)source.Rows[i]["Day"]).ToString("00") + "日" + ((int)source.Rows[i]["Hour"]).ToString("00") + "时" + ((int)(source.Rows[i]["Minute"])).ToString("00").Substring(0, 1) + "0分";
                        dr[columnName] = (decimal)source.Rows[i]["Sum"];
                    }

                    destination.Rows.Add(dr);
                }
                else
                {
                    DataRow dr = destination.NewRow();

                    for (int i = 0; i < source.Rows.Count; i++)
                    {
                        string columnName = ((int)source.Rows[i]["Month"]).ToString("00") + "月" + ((int)source.Rows[i]["Day"]).ToString("00") + "日" + ((int)source.Rows[i]["Hour"]).ToString("00") + "时" + ((int)(source.Rows[i]["Minute"])).ToString("00").Substring(0, 1) + "0分";
                        dr[columnName] = (decimal)source.Rows[i]["Sum"];
                    }

                    destination.Rows.Add(dr);
                }
            }
            else if (myTimeInterval == "20")    //1小时
            {
                for (DateTime hourLooper = startTime; hourLooper <= endTime; hourLooper = hourLooper.AddHours(1))
                {
                    DataColumn dc = new DataColumn(hourLooper.ToString("MM月dd日HH时"), typeof(decimal));
                    dc.DefaultValue = 0;
                    destination.Columns.Add(dc);
                }

                if (source.Columns.Contains("OrganizationId") && source.Columns.Contains("VariableId"))
                {
                    DataRow dr = destination.NewRow();

                    for (int i = 0; i < source.Rows.Count; i++)
                    {
                        if (i > 0 && (source.Rows[i]["OrganizationId"].ToString() != source.Rows[i - 1]["OrganizationId"].ToString() || source.Rows[i]["VariableId"].ToString() != source.Rows[i - 1]["VariableId"].ToString()))
                        {
                            destination.Rows.Add(dr);
                            dr = destination.NewRow();
                        }
                        string columnName = ((int)source.Rows[i]["Month"]).ToString("00") + "月" + ((int)source.Rows[i]["Day"]).ToString("00") + "日" + ((int)source.Rows[i]["Hour"]).ToString("00") + "时";
                        dr[columnName] = (decimal)source.Rows[i]["Sum"];
                    }

                    destination.Rows.Add(dr);
                }
                else
                {
                    DataRow dr = destination.NewRow();

                    for (int i = 0; i < source.Rows.Count; i++)
                    {
                        string columnName = ((int)source.Rows[i]["Month"]).ToString("00") + "月" + ((int)source.Rows[i]["Day"]).ToString("00") + "日" + ((int)source.Rows[i]["Hour"]).ToString("00") + "时";
                        dr[columnName] = (decimal)source.Rows[i]["Sum"];
                    }

                    destination.Rows.Add(dr);
                }
            }
            else if (myTimeInterval == "30")    //1天
            {
                for (DateTime hourLooper = startTime; hourLooper <= endTime; hourLooper = hourLooper.AddDays(1))
                {
                    DataColumn dc = new DataColumn(hourLooper.ToString("yyyy年MM月dd日"), typeof(decimal));
                    dc.DefaultValue = 0;
                    destination.Columns.Add(dc);
                }

                if (source.Columns.Contains("OrganizationId") && source.Columns.Contains("VariableId"))
                {
                    DataRow dr = destination.NewRow();

                    for (int i = 0; i < source.Rows.Count; i++)
                    {
                        if (i > 0 && (source.Rows[i]["OrganizationId"].ToString() != source.Rows[i - 1]["OrganizationId"].ToString() || source.Rows[i]["VariableId"].ToString() != source.Rows[i - 1]["VariableId"].ToString()))
                        {
                            destination.Rows.Add(dr);
                            dr = destination.NewRow();
                        }
                        string columnName = source.Rows[i]["Year"].ToString() + "年" + ((int)source.Rows[i]["Month"]).ToString("00") + "月" + ((int)source.Rows[i]["Day"]).ToString("00") + "日";
                        dr[columnName] = (decimal)source.Rows[i]["Sum"];
                    }

                    destination.Rows.Add(dr);
                }
                else
                {
                    DataRow dr = destination.NewRow();

                    for (int i = 0; i < source.Rows.Count; i++)
                    {
                        string columnName = source.Rows[i]["Year"].ToString() + "年" + ((int)source.Rows[i]["Month"]).ToString("00") + "月" + ((int)source.Rows[i]["Day"]).ToString("00") + "日";
                        dr[columnName] = (decimal)source.Rows[i]["Sum"];
                    }

                    destination.Rows.Add(dr);
                }
            }
            return destination;
        }
    }
}
