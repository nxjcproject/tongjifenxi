using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Infrastruture.Utility
{
    /// <summary>
    /// 辅助类：将按时间统计的纵表转换为以时间为列的横表
    /// </summary>
    public static class CountAnalysisTableHelper
    {
        /// <summary>
        /// 数据源表的类型
        /// </summary>
        public enum SourceType
        {
            Yearly,
            Monthly,
            Daily
        }

        /// <summary>
        /// 将统计表转换为以时间为字段的横表
        /// </summary>
        /// <param name="source">原表，应有以下字段,[]为可选：（year,month[,day],count）</param>
        /// <param name="sourceType">数据源表的类型</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">终止时间</param>
        /// <returns>以时间为字段的横表</returns>
        public static DataTable VerticalToHorizontal(DataTable source, SourceType sourceType, DateTime startTime, DateTime endTime = new DateTime())
        {
            switch (sourceType)
            {
                case SourceType.Yearly:
                    return VerticalToHorizontalYearly(source, startTime.Year);
                case SourceType.Monthly:
                    return VerticalToHorizontalMonthly(source, startTime.Year, startTime.Month);
                case SourceType.Daily:
                    return VerticalToHorizontalDaily(source, startTime, endTime);
                default:
                    throw new ArgumentException("无法识别数据源的类型。");
            }
        }

        /// <summary>
        /// 将统计表转换为以时间为字段的横表（按年统计）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private static DataTable VerticalToHorizontalYearly(DataTable source, int year)
        {
            DataTable destination = new DataTable();

            for (int i = 1; i <= 12; i++)
            {
                DataColumn dc = new DataColumn(i.ToString("00"), typeof(int));
                dc.DefaultValue = 0;
                destination.Columns.Add(dc);
            }

            DataRow dr = destination.NewRow();

            for (int i = 0; i < source.Rows.Count; i++)
            {
                string columnName = ((int)source.Rows[i]["Month"]).ToString("00");
                dr[columnName] = (int)source.Rows[i]["Count"];
            }

            destination.Rows.Add(dr);

            return destination;
        }

        /// <summary>
        /// 将统计表转换为以时间为字段的横表（按月统计）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        private static DataTable VerticalToHorizontalMonthly(DataTable source, int year, int month)
        {
            DataTable destination = new DataTable();

            DateTime startTime = new DateTime(year, month, 1);
            DateTime endTime = new DateTime(year, month, startTime.AddMonths(1).AddDays(-1).Day);

            for (DateTime dayLooper = startTime; dayLooper <= endTime; dayLooper = dayLooper.AddDays(1))
            {
                DataColumn dc = new DataColumn(dayLooper.ToString("MM-dd"), typeof(int));
                dc.DefaultValue = 0;
                destination.Columns.Add(dc);
            }

            DataRow dr = destination.NewRow();

            for (int i = 0; i < source.Rows.Count; i++)
            {
                string columnName = ((int)source.Rows[i]["Month"]).ToString("00") + "-" + ((int)source.Rows[i]["Day"]).ToString("00");
                dr[columnName] = (int)source.Rows[i]["Count"];
            }

            destination.Rows.Add(dr);

            return destination;
        }

        /// <summary>
        /// 将统计表转换为以时间为字段的横表（自定义时间段统计）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private static DataTable VerticalToHorizontalDaily(DataTable source, DateTime startTime, DateTime endTime)
        {
            DataTable destination = new DataTable();

            for (DateTime dayLooper = startTime; dayLooper <= endTime; dayLooper = dayLooper.AddDays(1))
            {
                DataColumn dc = new DataColumn(dayLooper.ToString("yyyy-MM-dd"), typeof(int));
                dc.DefaultValue = 0;
                destination.Columns.Add(dc);
            }

            DataRow dr = destination.NewRow();

            for (int i = 0; i < source.Rows.Count; i++)
            {
                string columnName = source.Rows[i]["Year"].ToString() + "-" + ((int)source.Rows[i]["Month"]).ToString("00") + "-" + ((int)source.Rows[i]["Day"]).ToString("00");
                dr[columnName] = (int)source.Rows[i]["Count"];
            }

            destination.Rows.Add(dr);

            return destination;
        }
    }
}
