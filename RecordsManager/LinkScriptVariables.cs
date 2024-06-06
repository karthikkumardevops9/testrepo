using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace MSRecordsEngine.RecordsManager
{
    /// <summary>
    /// Stores information regarding a variabled defined in a linkscript.
    /// </summary>
    /// <remarks></remarks>
    public class LinkScriptVariable
    {
        /// <summary>
        /// Supported Variable Types for parsing linkscripts.
        /// </summary>
        /// <remarks></remarks>
        public enum VariableTypes
        {
            vtString,
            vtInteger,
            vtSingle,
            vtDouble,
            vtLong,
            vtDate,
            vtDataTable,
            vtProperty
        }
        /// <summary>
        /// Variables Scope for parsing linkscripts.
        /// </summary>
        /// <remarks></remarks>
        public enum VarScope
        {
            vsPublic,
            vsPrivate
        }
        /// <summary>
        /// Math Types for parsing linkscripts.
        /// </summary>
        /// <remarks></remarks>
        public enum MathTypes
        {
            mtAdd,
            mtSubtract,
            mtMultiply,
            mtDivide
        }
        /// <summary>
        /// String Trim Types for parsing linkscripts.
        /// </summary>
        /// <remarks></remarks>
        public enum TrimTypes
        {
            ttTrim,
            ttLTrim,
            ttRTrim
        }
        /// <summary>
        /// Char Types for parsing linkscripts.
        /// </summary>
        /// <remarks></remarks>
        public enum CharTypes
        {
            ctLeft,
            ctRight,
            ctMid
        }

        private object _value;
        private VariableTypes _variableType;
        private string _tableName;
        private List<string> _fieldNames;
        private List<string> _beforeValues;
        private List<string> _afterValues;

        internal LinkScriptVariable()
        {
            InitiateClass();
        }
        // ------------------------------------------------------------------------------------------
        // Property: FieldNames (Collection)
        // ------------------------------------------------------------------------------------------
        public List<string> FieldNames
        {
            get
            {
                if (_fieldNames is null)
                    _fieldNames = new List<string>();
                return _fieldNames;
            }
            set
            {
                _fieldNames = value;
            }
        }

        public List<string> FieldBeforeValues
        {
            get
            {
                if (_beforeValues is null)
                    _beforeValues = new List<string>();
                return _beforeValues;
            }
            set
            {
                _beforeValues = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: FieldAfterValues (Collection)
        // ------------------------------------------------------------------------------------------
        public List<string> FieldAfterValues
        {
            get
            {
                if (_afterValues is null)
                    _afterValues = new List<string>();
                return _afterValues;
            }
            set
            {
                _afterValues = value;
            }
        }

        public object Value
        {
            get
            {
                if (_value is null & _variableType == VariableTypes.vtString)
                {
                    _value = " ";  // Reggie told me to put this space here, but we're not sure why
                }
                return _value;
            }

            set
            {
                if (!(value is ValueType))
                {
                    _value = value;
                }
                else
                {
                    if (value is DBNull | string.IsNullOrEmpty(Strings.Trim(value.ToString())))
                        value = " ";
                    _value = value;
                }
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: VariableType (enum)
        // ------------------------------------------------------------------------------------------
        public VariableTypes VariableType
        {
            get
            {
                return _variableType;
            }
            set
            {
                _variableType = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: TableName (String)
        // ------------------------------------------------------------------------------------------
        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = Strings.Trim(value);
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: BeforeData (String)
        // ------------------------------------------------------------------------------------------
        public string BeforeData
        {
            get
            {
                int lCnt;
                var rtn = new System.Text.StringBuilder();

                if (_beforeValues is not null)
                {
                    var loopTo = _fieldNames.Count - 1;
                    for (lCnt = 0; lCnt <= loopTo; lCnt++)
                        rtn.Append(_fieldNames[lCnt] + ": " + _beforeValues[lCnt] + Constants.vbCrLf);
                    if (rtn.Length > 2)
                        return rtn.ToString().Substring(0, rtn.Length - 2);
                }

                return rtn.ToString();
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: AfterData (String)
        // ------------------------------------------------------------------------------------------
        public string AfterData
        {
            get
            {
                int lCnt;
                var sReturn = new System.Text.StringBuilder();

                if (_afterValues is not null)
                {
                    var loopTo = _fieldNames.Count - 1;
                    for (lCnt = 0; lCnt <= loopTo; lCnt++)
                        sReturn.Append(_fieldNames[lCnt] + ": " + _afterValues[lCnt] + Constants.vbCrLf);
                    if (sReturn.Length > 2)
                        return sReturn.ToString().Substring(0, sReturn.Length - 2);
                }

                return sReturn.ToString();
            }
        }

        public void ClearCollections()
        {
            _fieldNames = null;
            _beforeValues = null;
            _afterValues = null;
        }

        private void InitiateClass()
        {
            ClearCollections();
        }

        private void TerminateClass()
        {
            ClearCollections();
        }

        ~LinkScriptVariable()
        {
            TerminateClass();
        }
    }

    internal class BaseCollection : System.Collections.Specialized.NameObjectCollectionBase
    {

        public override IEnumerator GetEnumerator()
        {
            return BaseGetAllValues().GetEnumerator();
        }

        public virtual object BaseItem(int iIndex)
        {
            return BaseGet(iIndex);
        }
        public virtual object BaseItem(string sName)
        {
            return BaseGet(sName);
        }

        public virtual void Add(string sKey, object oValue)
        {
            try
            {
                BaseAdd(sKey, oValue);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }

    internal class LinkScriptVariables : BaseCollection
    {

        internal LinkScriptVariables() : base()
        {
        }

        public void Clear()
        {
            BaseClear();
        }

        public new IEnumerator GetEnumerator()
        {
            return BaseGetAllValues().GetEnumerator();
        }

        public LinkScriptVariable Item(string sKey)
        {
            try
            {
                return (LinkScriptVariable)BaseGet(sKey);
            }
            catch
            {
                return null;
            }
        }

        public LinkScriptVariable Item(int iKey)
        {
            try
            {
                return (LinkScriptVariable)BaseGet(iKey);
            }
            catch
            {
                return null;
            }
        }

        public void Remove(string sKey)
        {
            BaseRemove(sKey);
        }
    }

    public class LinkScriptHeaders : object
    {

        private string _name;
        private int _pageAction;
        private string _directoryName;
        private string _outputSettingName;
        private ScriptEngine.CallerTypes _callingType;
        private bool _deleteAfterCopy;
        private int _viewGroup;
        private ScriptEngine.UITypes _UIType;

        internal LinkScriptHeaders() : base()
        {
        }

        public string Key
        {
            get
            {
                return Name;
            }
        }

        [Obsolete("Name property should be used for LinkScriptHeaders.", true)]
        [ComVisible(false)]
        public int Id
        {
            // Do not delete this method because it provides a warning when a consumer attempts to use this property.
            get
            {
                throw new InvalidOperationException("Use Name property instead.");
            }
            set
            {
                throw new InvalidOperationException("Use Name property instead.");
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = Strings.Trim(value);
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: FormPageAction (Integer)
        // ------------------------------------------------------------------------------------------
        public int FormPageAction
        {
            get
            {
                return _pageAction;
            }
            set
            {
                _pageAction = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: msDirectoryName (String * 120)
        // ------------------------------------------------------------------------------------------
        public string DirectoryName
        {
            get
            {
                return _directoryName;
            }
            set
            {
                _directoryName = Strings.Trim(value);
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: msOutputSettingsId (String * 20)
        // ------------------------------------------------------------------------------------------
        public string OutputSettingsId
        {
            get
            {
                return _outputSettingName;
            }
            set
            {
                _outputSettingName = Strings.Trim(value);
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: CallingType (Long)
        // ------------------------------------------------------------------------------------------
        public ScriptEngine.CallerTypes CallingType
        {
            get
            {
                return _callingType;
            }
            set
            {
                _callingType = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: PCFilesDeleteAfterCopy (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool PCFilesDeleteAfterCopy
        {
            get
            {
                return _deleteAfterCopy;
            }
            set
            {
                _deleteAfterCopy = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: ViewGroup (Long)
        // ------------------------------------------------------------------------------------------
        public int ViewGroup
        {
            get
            {
                return _viewGroup;
            }
            set
            {
                _viewGroup = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: UIType (geLinkScriptHeaderUITypes)
        // ------------------------------------------------------------------------------------------
        public ScriptEngine.UITypes UIType
        {
            get
            {
                switch (_UIType)
                {
                    case ScriptEngine.UITypes.NoUI:
                    case ScriptEngine.UITypes.SimpleUI:
                    case ScriptEngine.UITypes.ComplexUI:
                    case ScriptEngine.UITypes.CompleteUI:
                        {
                            return _UIType;
                        }

                    default:
                        {
                            return ScriptEngine.UITypes.NoUI;
                        }
                }
            }

            set
            {
                switch (value)
                {
                    case ScriptEngine.UITypes.NoUI:
                    case ScriptEngine.UITypes.SimpleUI:
                    case ScriptEngine.UITypes.ComplexUI:
                    case ScriptEngine.UITypes.CompleteUI:
                        {
                            break;
                        }

                    default:
                        {
                            value = ScriptEngine.UITypes.NoUI;
                            break;
                        }
                }

                _UIType = value;
            }
        }
    }
}