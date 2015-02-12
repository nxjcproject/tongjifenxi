using StatisticalAnalysis.Infrastruture.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Infrastruture.Utility
{
    public static class FSTAnalysisTableHelper
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
        /// 将甲乙丙表转换为以时间为字段的横表
        /// </summary>
        /// <param name="source">原表，应有以下字段,[]为可选：（timeStamp,FirstB,SecondB,ThirdB）</param>
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
        /// 将峰谷平表转换为以时间为字段的横表（按年统计）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private static DataTable VerticalToHorizontalYearly(DataTable source, int year)
        {
            DataTable destination = new DataTable();

            for (int i = 1; i <= 12; i++)
            {
                DataColumn dc = new DataColumn(i.ToString("00"), typeof(decimal));
                dc.DefaultValue = 0;
                destination.Columns.Add(dc);
            }

            DataRow drPeak = destination.NewRow();
            DataRow drValley = destination.NewRow();
            DataRow drFlat = destination.NewRow();

            for (int i = 0; i < source.Rows.Count; i++)
            {
                string columnName = ((DateTime)source.Rows[i]["TimeStamp"]).ToString("MM");

                drPeak[columnName] = (decimal)source.Rows[i]["FirstB"];
                drValley[columnName] = (decimal)source.Rows[i]["SecondB"];
                drFlat[columnName] = (decimal)source.Rows[i]["ThirdB"];
            }

            destination.Rows.Add(drPeak);
            destination.Rows.Add(drValley);
            destination.Rows.Add(drFlat);

            return destination;
        }


        /// <summary>
        /// 将峰谷平表转换为以时间为字段的横表（按月统计）
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
                DataColumn dc = new DataColumn(dayLooper.ToString("MM-dd"), typeof(decimal));
                dc.DefaultValue = 0;
                destination.Columns.Add(dc);
            }

            DataRow drPeak = destination.NewRow();
            DataRow drValley = destination.NewRow();
            DataRow drFlat = destination.NewRow();

            for (int i = 0; i < source.Rows.Count; i++)
            {
                string columnName = DateTime.Parse(source.Rows[i]["TimeStamp"].ToString()).ToString("MM-dd");

                drPeak[columnName] = (decimal)source.Rows[i]["FirstB"];
                drValley[columnName] = (decimal)source.Rows[i]["SecondB"];
                drFlat[columnName] = (decimal)source.Rows[i]["ThirdB"];
            }

            destination.Rows.Add(drPeak);
            destination.Rows.Add(drValley);
            destination.Rows.Add(drFlat);

            return destination;
        }

        /// <summary>
        /// 将峰谷平表转换为以时间为字段的横表（自定义时间段统计）
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

            DataRow drPeak = destination.NewRow();
            DataRow drValley = destination.NewRow();
            DataRow drFlat = destination.NewRow();

            for (int i = 0; i < source.Rows.Count; i++)
            {
                string columnName = DateTime.Parse(source.Rows[i]["TimeStamp"].ToString()).ToString("yyyy-MM-dd");

                drPeak[columnName] = (decimal)source.Rows[i]["FirstB"];
                drValley[columnName] = (decimal)source.Rows[i]["SecondB"];
                drFlat[columnName] = (decimal)source.Rows[i]["ThirdB"];
            }

            destination.Rows.Add(drPeak);
            destination.Rows.Add(drValley);
            destination.Rows.Add(drFlat);

            return destination;
        }
    }
}
