using System;
using System.Collections.Specialized;
using System.Diagnostics;

namespace MSRecordsEngine.Models.FusionModels
{
    public partial class SystemCustModel
    {
        public MSRecordsEngine.Entities.System SystemModel
        {
            get
            {
                return m_SystemModel;
            }
            set
            {
                m_SystemModel = value;
            }
        }
        private MSRecordsEngine.Entities.System m_SystemModel;

        public string get_CounterValue(string sCounterName)
        {
            try
            {
                return mcCounterValues[sCounterName].ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "0";
            }
        }

        public void set_CounterValue(string sCounterName, string value)
        {
            if (mcCounterValues.Contains(sCounterName))
            {
                mcCounterValues[sCounterName] = value.Trim(); 
            }
            else
            {
                mcCounterValues.Add(sCounterName, value.Trim());
            }
        }
        private string m_CounterValue;

        private ListDictionary mcCounterValues = new ListDictionary();
    }
}
