using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.HorizontallyAlarmAnalysis
{
    public static class VerticallyToHorizontally
    {
        public static DataTable VerticallyToHorizontallyTable(DataTable table,string columnName,string valueName)
        {
            DataTable result = new DataTable();
            //DataColumn col = new DataColumn("名称", typeof(string));
            //col.DefaultValue="报警次数";
            //result.Columns.Add(col);

            int count = table.Rows.Count;
            for(int i=0;i<count;i++)
            {
                DataRow row = table.Rows[i];
                string t_column=row[columnName].ToString().Trim();
                DataColumn column = new DataColumn(t_column, typeof(decimal));
                result.Columns.Add(column);
            }
            DataRow resultRow = result.NewRow();
            for (int i = 0; i < count; i++)
            {
                DataRow row = table.Rows[i];               
                resultRow[row[columnName].ToString().Trim()] = row[valueName];
            }
            result.Rows.Add(resultRow);
            return result;
        }
    }
}
