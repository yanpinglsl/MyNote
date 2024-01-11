using System;
using System.Collections.Generic;

namespace OfficeReportInterface.DefaultReportInterface.CommonluUsed
{
    class DayOfWeekManager
    {
        private static DayOfWeekManager m_instance;

        public static DayOfWeekManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new DayOfWeekManager();
                return m_instance;
            }
        }


        private Dictionary<string, string> m_dic;

        private DayOfWeekManager()
        {
            AddItems();
        }

        private void AddItems()
        {
            m_dic = new Dictionary<string, string>();
            AddOneItem("Sunday", LocalResourceManager.GetInstance().GetString("0565", "Sunday"));
            AddOneItem("Monday", LocalResourceManager.GetInstance().GetString("0566", "Monday"));
            AddOneItem("Tuesday", LocalResourceManager.GetInstance().GetString("0567", "Tuesday"));
            AddOneItem("Wednesday", LocalResourceManager.GetInstance().GetString("0568", "Wednesday"));
            AddOneItem("Thursday", LocalResourceManager.GetInstance().GetString("0569", "Thursday"));
            AddOneItem("Friday", LocalResourceManager.GetInstance().GetString("0570", "Friday"));
            AddOneItem("Saturday", LocalResourceManager.GetInstance().GetString("0571", "Saturday"));
        }

        private void AddOneItem(string key,string valueOne)
        {
            if (!m_dic.ContainsKey(key))
                m_dic.Add(key, valueOne);
        }
        /// <summary>
        /// 获取星期几的字符串
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string GetDayOfWeek(DateTime time)
        {
            string result;
            var day = time.DayOfWeek.ToString();
            if (!m_dic.TryGetValue(day, out result))
                return string.Empty;
            return result;
        }
    }
}
