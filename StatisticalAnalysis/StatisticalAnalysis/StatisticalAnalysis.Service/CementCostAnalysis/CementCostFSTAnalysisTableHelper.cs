using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.CementCostAnalysis
{
    
    public static class CementCostFSTAnalysisTableHelper
    {
        //static CementCostFSTAnalysisTableHelper()
        //{
        //    ISqlServerDataFactory dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
        //    string sql = "";
        //}
        /// <summary>
        /// 数据源表的类型
        /// </summary>
        public enum SourceType
        {
            Yearly,
            Monthly,
            Daily
        }
        //峰谷平电价
        private static decimal _peakElcPrice;
        private static decimal _valleyElcPrice;
        private static decimal _flatElcPrice;
        /// <summary>
        /// 设置电价
        /// </summary>
        /// <param name="organizationId">组织机构ID</param>
        public static void SetElcPrice(string organizationId)
        {
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
            string sql = "SELECT * FROM [dbo].[system_PeakValleyFlatElectrovalence] WHERE OrganizationID=@OrganizationID";
            SqlParameter paramater = new SqlParameter("OrganizationID", organizationId);
            DataTable table= dataFactory.Query(sql, paramater);
            _peakElcPrice = (decimal)table.Rows[0]["PeakElectrovalence"];
            _valleyElcPrice = (decimal)table.Rows[0]["ValleyElectrovalence"];
            _flatElcPrice = (decimal)table.Rows[0]["FlatElectrovalence"];
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
            DataRow drAverage = destination.NewRow();
            //DataRow[] electricityConsumption = source.Select("ValueType='ElectricityConsumption'");//取出属于电耗的行
            var electricityConsumptionQuery = from dr in source.AsEnumerable()
                                              where dr["ValueType"].ToString() == "ElectricityConsumption"
                                              orderby dr["TimeStamp"] ascending
                                              select dr;
            DataRow[] electricityConsumptions = electricityConsumptionQuery.ToArray();

            for (int i = 0; i < electricityConsumptions.Count(); i++)
            {
                string columnName = DateTime.Parse(electricityConsumptions[i]["TimeStamp"].ToString()).ToString("MM");

                drPeak[columnName] = (decimal)electricityConsumptions[i]["PeakB"] * _peakElcPrice;
                drValley[columnName] = (decimal)electricityConsumptions[i]["ValleyB"] * _valleyElcPrice;
                drFlat[columnName] = (decimal)electricityConsumptions[i]["FlatB"] * _flatElcPrice;
                //drAverage[columnName]=((decimal)source.Rows[i]["PeakB"]+(decimal)source.Rows[i]["FlatB"]+(decimal)source.Rows[i]["FlatB"])/
            }

            //DataRow[] electricityQuantity = source.Select("ValueType='ElectricityQuantity'");//取出电量的行
            //DataRow[] materialWeight = source.Select("ValueType='MaterialWeight'");//取出产量的行
            var electricityQuantityQuery = from dr in source.AsEnumerable()
                                           where dr["ValueType"].ToString() == "ElectricityQuantity"
                                           orderby dr["TimeStamp"] ascending
                                           select dr;
            var materialWeightQuery = from dr in source.AsEnumerable()
                                      where dr["ValueType"].ToString() == "MaterialWeight"
                                      orderby dr["TimeStamp"] ascending
                                      select dr;
            DataRow[] electricityQuantitys = electricityQuantityQuery.ToArray();
            DataRow[] materialWeights = materialWeightQuery.ToArray();
            if (electricityQuantitys.Count() == materialWeights.Count())
            {
                for (int i = 0; i < electricityQuantitys.Count(); i++)
                {
                    string columnName = DateTime.Parse(electricityQuantitys[i]["TimeStamp"].ToString()).ToString("MM-dd");
                    if (columnName == DateTime.Parse(materialWeights[i]["TimeStamp"].ToString()).ToString("MM-dd"))
                    {
                        drAverage[columnName] =(decimal)materialWeights[i]["TotalPeakValleyFlatB"]==0?0:((decimal)electricityQuantitys[i]["PeakB"]*_peakElcPrice + (decimal)electricityQuantitys[i]["ValleyB"]*_valleyElcPrice +
                                                (decimal)electricityQuantitys[i]["FlatB"]*_flatElcPrice) / (decimal)materialWeights[i]["TotalPeakValleyFlatB"];
                    }
                }
            }

            destination.Rows.Add(drPeak);
            destination.Rows.Add(drValley);
            destination.Rows.Add(drFlat);
            destination.Rows.Add(drAverage);

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
            DataRow drAverage = destination.NewRow();
            //DataRow[] electricityConsumption = source.Select("ValueType='ElectricityConsumption'");//取出属于电耗的行
            var electricityConsumptionQuery = from dr in source.AsEnumerable()
                                           where dr["ValueType"].ToString() == "ElectricityConsumption"
                                           orderby dr["TimeStamp"] ascending
                                           select dr;
            DataRow[] electricityConsumptions = electricityConsumptionQuery.ToArray();
            for (int i = 0; i < electricityConsumptions.Count(); i++)
            {
                string columnName = DateTime.Parse(electricityConsumptions[i]["TimeStamp"].ToString()).ToString("MM-dd");

                drPeak[columnName] = (decimal)electricityConsumptions[i]["PeakB"]*_peakElcPrice;
                drValley[columnName] = (decimal)electricityConsumptions[i]["ValleyB"]*_valleyElcPrice;
                drFlat[columnName] = (decimal)electricityConsumptions[i]["FlatB"]*_flatElcPrice;
            }
            //DataRow[] electricityQuantity = source.Select("ValueType='ElectricityQuantity'");//取出电量的行
            //DataRow[] materialWeight = source.Select("ValueType='MaterialWeight'");//取出产量的行
            var electricityQuantityQuery = from dr in source.AsEnumerable()
                                           where dr["ValueType"].ToString() == "ElectricityQuantity"
                                           orderby dr["TimeStamp"] ascending
                                           select dr;
            var materialWeightQuery = from dr in source.AsEnumerable()
                                          where dr["ValueType"].ToString() == "MaterialWeight"
                                          orderby dr["TimeStamp"] ascending
                                          select dr;
            DataRow[] electricityQuantitys = electricityQuantityQuery.ToArray();
            DataRow[] materialWeights = materialWeightQuery.ToArray();

            
            if (electricityQuantitys.Count() == materialWeights.Count())
            {
                for (int i = 0; i < electricityQuantityQuery.ToArray().Count(); i++)
                {
                    string columnName = DateTime.Parse(electricityQuantitys[i]["TimeStamp"].ToString()).ToString("MM-dd");
                    if (columnName == DateTime.Parse(materialWeights[i]["TimeStamp"].ToString()).ToString("MM-dd"))
                    {
                        drAverage[columnName] = (decimal)materialWeights[i]["TotalPeakValleyFlatB"]==0?0:((decimal)electricityQuantitys[i]["PeakB"] * _peakElcPrice + (decimal)electricityQuantitys[i]["ValleyB"] * _valleyElcPrice +
                                                (decimal)electricityQuantitys[i]["FlatB"]*_flatElcPrice) / (decimal)materialWeights[i]["TotalPeakValleyFlatB"];
                    }
                }
            }
            destination.Rows.Add(drPeak);
            destination.Rows.Add(drValley);
            destination.Rows.Add(drFlat);
            destination.Rows.Add(drAverage);

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
            DataRow drAverage = destination.NewRow();
            //DataRow[] electricityConsumption = source.Select("ValueType='ElectricityConsumption'");//取出属于电耗的行
            var electricityConsumptionQuery = from dr in source.AsEnumerable()
                                              where dr["ValueType"].ToString() == "ElectricityConsumption"
                                              orderby dr["TimeStamp"] ascending
                                              select dr;
            DataRow[] electricityConsumptions = electricityConsumptionQuery.ToArray();
            for (int i = 0; i < electricityConsumptions.Count(); i++)
            {
                string columnName = DateTime.Parse(source.Rows[i]["TimeStamp"].ToString()).ToString("yyyy-MM-dd");

                drPeak[columnName] = (decimal)electricityConsumptions[i]["PeakB"] * _peakElcPrice;
                drValley[columnName] = (decimal)electricityConsumptions[i]["ValleyB"] * _valleyElcPrice;
                drFlat[columnName] = (decimal)electricityConsumptions[i]["FlatB"] * _flatElcPrice;
            }
            //DataRow[] electricityQuantity = source.Select("ValueType='ElectricityQuantity'");//取出电量的行
            //DataRow[] materialWeight = source.Select("ValueType='MaterialWeight'");//取出产量的行
            var electricityQuantityQuery = from dr in source.AsEnumerable()
                                           where dr["ValueType"].ToString() == "ElectricityQuantity"
                                           orderby dr["TimeStamp"] ascending
                                           select dr;
            var materialWeightQuery = from dr in source.AsEnumerable()
                                      where dr["ValueType"].ToString() == "MaterialWeight"
                                      orderby dr["TimeStamp"] ascending
                                      select dr;
            DataRow[] electricityQuantitys = electricityQuantityQuery.ToArray();
            DataRow[] materialWeights = materialWeightQuery.ToArray();
            if (electricityQuantitys.Count() == materialWeights.Count())
            {
                for (int i = 0; i < electricityQuantitys.Count(); i++)
                {
                    string columnName = DateTime.Parse(electricityQuantitys[i]["TimeStamp"].ToString()).ToString("MM-dd");
                    if (columnName == DateTime.Parse(materialWeights[i]["TimeStamp"].ToString()).ToString("MM-dd"))
                    {
                        drAverage[columnName] = (decimal)materialWeights[i]["TotalPeakValleyFlatB"]==0?0:
                                                ((decimal)electricityQuantitys[i]["PeakB"] * _peakElcPrice + (decimal)electricityQuantitys[i]["ValleyB"] * _valleyElcPrice +
                                                (decimal)electricityQuantitys[i]["FlatB"])*_flatElcPrice / (decimal)materialWeights[i]["TotalPeakValleyFlatB"];
                    }
                }
            }

            destination.Rows.Add(drPeak);
            destination.Rows.Add(drValley);
            destination.Rows.Add(drFlat);
            destination.Rows.Add(drAverage);

            return destination;
        }
    }
}
