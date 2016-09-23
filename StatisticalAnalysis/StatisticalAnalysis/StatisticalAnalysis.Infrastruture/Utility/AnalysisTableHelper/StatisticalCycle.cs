using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Infrastruture.Utility.AnalysisTableHelper
{
    /// <summary>
    /// 统计区间
    /// </summary>
    public enum StatisticalCycle
    {
        /// <summary>
        /// 年区间（按月统计）
        /// </summary>
        Yearly,
        /// <summary>
        /// 月区间（按日统计）
        /// </summary>
        Monthly,
        /// <summary>
        /// 日区间（按小时统计）
        /// </summary>
        Daily,
        /// <summary>
        /// 自定义统计区间（按日统计）
        /// </summary>
        CustomDaily
    }
}
