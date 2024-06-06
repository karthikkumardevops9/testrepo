using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Microsoft.VisualBasic.CompilerServices;

namespace MSRecordsEngine.RecordsManager
{

    public class ScriptControls
    {
        public enum TextboxTypes
        {
            ttAlpha,
            ttAlphaNumeric,
            ttNumeric,
            ttInteger,
            ttDate
        }
        // Control Types Supported...
        public enum ControlTypes
        {
            ctTextBox = 0,
            ctLabel = 1,
            ctComboBox = 2,
            ctOption = 3,
            ctCheck = 4,
            ctListBox = 5,
            ctButton = 6,
            ctMemoBox = 7
        }
        // Exposed Control Properties...
        public enum ControlProperties
        {
            cpCaption,
            cpValue,
            cpEnabled,
            cpListindex,
            cpText,
            cpVisible,
            cpDefault,
            cpCancel,
            cpItemData,
            cpMaxLength,
            cpTooltip,
            cpPassword,
            cpTextboxType,
            cpRangeMin,
            cpRangeMax,
            cpValidate,
            cpAllowNullDate,
            cpRequired,
            cpWidth,
            cpHeight,
            cpStartFocusHere,
            cpBackColor,
            cpForeColor,
            cpFontName,
            cpFontSize,
            cpFontBold,
            cpFontItalic,
            cpFontUnderline,
            cpLocked
        }

        private bool mbAllowNullDate;
        private bool mbRequired;
        private bool mbSetWidth;
        private bool mbStartFocusHere;
        private bool mbValidate;
        private bool mbVisible;
        private int miControlIndex;
        private int miHeight;
        private int miWidth;
        private string msCaption;
        private string msTag;
        private string msText;
        private List<string> mcItem;
        private List<string> mcItemData;
        private ControlTypes meControlType;
        private TextboxTypes meTextboxType;
        private object mvRangeMin;
        private object mvRangeMax;
        private object mvValue;
        private Dictionary<ControlProperties, object> dPropertyValues;

        internal ScriptControls() : base()
        {
            dPropertyValues = new Dictionary<ControlProperties, object>();
        }
        // ------------------------------------------------------------------------------------------
        // Property: AllowNullDate (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool AllowNullDate
        {
            get
            {
                return mbAllowNullDate;
            }
            set
            {
                mbAllowNullDate = value;
            }
        }

        public object GetProperty(ControlProperties controlProperty)
        {
            if (controlProperty == ControlProperties.cpItemData)
            {
                if (dPropertyValues.ContainsKey(ControlProperties.cpListindex))
                {
                    try
                    {
                        return mcItemData[Conversions.ToInteger(dPropertyValues[ControlProperties.cpListindex])];
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }

            object rtn;

            if (dPropertyValues.ContainsKey(controlProperty))
            {
                rtn = dPropertyValues[controlProperty];
            }
            else
            {
                switch (controlProperty)
                {
                    case ControlProperties.cpEnabled:
                    case ControlProperties.cpVisible:
                    case ControlProperties.cpValidate:
                        {
                            rtn = true;
                            break;
                        }
                    case ControlProperties.cpAllowNullDate:
                    case ControlProperties.cpCancel:
                    case ControlProperties.cpDefault:
                    case ControlProperties.cpRequired:
                        {
                            rtn = false;
                            break;
                        }
                    case ControlProperties.cpLocked:
                    case ControlProperties.cpStartFocusHere:
                        {
                            rtn = false;
                            break;
                        }
                    case ControlProperties.cpFontBold:
                    case ControlProperties.cpFontItalic:
                    case ControlProperties.cpFontUnderline:
                        {
                            rtn = false;
                            break;
                        }
                    case ControlProperties.cpForeColor:
                        {
                            rtn = Color.Black.ToArgb();
                            break;
                        }
                    case ControlProperties.cpBackColor:
                        {
                            rtn = Color.White.ToArgb();
                            break;
                        }
                    case ControlProperties.cpMaxLength:
                    case ControlProperties.cpHeight:
                    case ControlProperties.cpWidth:
                        {
                            rtn = 0;
                            break;
                        }
                    case ControlProperties.cpRangeMax:
                    case ControlProperties.cpRangeMin:
                    case ControlProperties.cpFontSize:
                        {
                            rtn = 0;
                            break;
                        }

                    default:
                        {
                            rtn = string.Empty;
                            break;
                        }
                }

                if (controlProperty == ControlProperties.cpCaption && ControlType == ControlTypes.ctLabel)
                    rtn = "lblObject";
                if (controlProperty == ControlProperties.cpListindex && (ControlType == ControlTypes.ctListBox || ControlType == ControlTypes.ctComboBox))
                    rtn = -1;
            }

            return rtn;
        }

        public void SetProperty(ControlProperties controlProperty, object value)
        {
            if (dPropertyValues.ContainsKey(controlProperty))
            {
                dPropertyValues[controlProperty] = value;
            }
            else
            {
                dPropertyValues.Add(controlProperty, value);
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Caption (String)
        // ------------------------------------------------------------------------------------------
        public string Caption
        {
            get
            {
                return msCaption;
            }
            set
            {
                msCaption = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: ControlIndex (Integer)
        // ------------------------------------------------------------------------------------------
        public int ControlIndex
        {
            get
            {
                return miControlIndex;
            }
            set
            {
                miControlIndex = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: ControlType (geControlTypes)
        // ------------------------------------------------------------------------------------------
        public ControlTypes ControlType
        {
            get
            {
                return meControlType;
            }
            set
            {
                meControlType = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: ControlHeight (Integer)
        // ------------------------------------------------------------------------------------------
        public int ControlHeight
        {
            get
            {
                return miHeight;
            }
            set
            {
                if (value > 0)
                    miHeight = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: ControlWidth (Integer)
        // ------------------------------------------------------------------------------------------
        public int ControlWidth
        {
            get
            {
                return miWidth;
            }
            set
            {
                if (value > 0)
                    miWidth = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Item (String)
        // ------------------------------------------------------------------------------------------
        public string get_Item(int iKey)
        {
            try
            {
                return mcItem[iKey];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public void set_Item(int iKey, string value)
        {
            if (mcItem is null)
                mcItem = new List<string>();
            if (iKey < mcItem.Count)
            {
                mcItem[iKey] = value;
            }
            else
            {
                mcItem.Add(value);
            }
        }

        public List<string> ItemList
        {
            get
            {
                if (mcItem is null)
                    mcItem = new List<string>();
                return mcItem;
            }
        }

        public void AddItem(string item)
        {
            if (mcItem is null)
                mcItem = new List<string>();
            mcItem.Add(item);
        }

        public void AddItemData(string itemdata)
        {
            if (mcItemData is null)
                mcItemData = new List<string>();
            mcItemData.Add(itemdata);
        }
        // ------------------------------------------------------------------------------------------
        // Property: ItemData (String)
        // ------------------------------------------------------------------------------------------
        public string get_ItemData(int iKey)
        {
            try
            {
                return mcItemData[iKey];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public void set_ItemData(int iKey, string value)
        {
            if (mcItemData is null)
                mcItemData = new List<string>();
            if (iKey < mcItemData.Count)
            {
                mcItemData[iKey] = value;
            }
            else
            {
                mcItemData.Add(value);
            }
        }

        public List<string> ItemDataList
        {
            get
            {
                if (mcItemData is null)
                    mcItemData = new List<string>();
                return mcItemData;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: RangeMin (Object)
        // ------------------------------------------------------------------------------------------
        public object RangeMin
        {
            get
            {
                return mvRangeMin;
            }
            set
            {
                mvRangeMin = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: RangeMax (Object)
        // ------------------------------------------------------------------------------------------
        public object RangeMax
        {
            get
            {
                return mvRangeMax;
            }
            set
            {
                mvRangeMax = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Required (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool Required
        {
            get
            {
                return mbRequired;
            }
            set
            {
                mbRequired = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: SetWidth (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool SetWidth
        {
            get
            {
                return mbSetWidth;
            }
            set
            {
                mbSetWidth = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: StartFocusHere (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool StartFocusHere
        {
            get
            {
                return mbStartFocusHere;
            }
            set
            {
                mbStartFocusHere = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Tag (String)
        // ------------------------------------------------------------------------------------------
        public string Tag
        {
            get
            {
                return msTag;
            }
            set
            {
                msTag = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Text (String)
        // ------------------------------------------------------------------------------------------
        public string Text
        {
            get
            {
                return msText;
            }
            set
            {
                msText = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: TextboxType (geTextboxType)
        // ------------------------------------------------------------------------------------------
        public TextboxTypes TextboxType
        {
            get
            {
                return meTextboxType;
            }
            set
            {
                meTextboxType = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Validate (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool Validate
        {
            get
            {
                return mbValidate;
            }
            set
            {
                mbValidate = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Visible (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool Visible
        {
            get
            {
                return mbVisible;
            }
            set
            {
                mbVisible = value;
            }
        }

        public object Value
        {
            get
            {
                return mvValue;
            }
            set
            {
                mvValue = value;
            }
        }
    }

    public class ScriptControlRules : object
    {

        private bool mbAllowNullDate;
        private bool mbRequired;
        private bool mbSetWidth;
        private bool mbStartFocusHere;
        private bool mbValidate;
        private int miControlIndex;
        private int miHeight;
        private int miWidth;
        private string msCaption;
        private string msTag;
        private string msText;
        private List<string> mcItem;
        private List<string> mcItemData;
        private object mvRangeMin;
        private object mvRangeMax;
        private object mvValue;

        public ScriptControlRules() : base()
        {
        }
        // ------------------------------------------------------------------------------------------
        // Property: AllowNullDate (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool AllowNullDate
        {
            get
            {
                return mbAllowNullDate;
            }
            set
            {
                mbAllowNullDate = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Caption (String)
        // ------------------------------------------------------------------------------------------
        public string Caption
        {
            get
            {
                return msCaption;
            }
            set
            {
                msCaption = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: ControlIndex (Integer)
        // ------------------------------------------------------------------------------------------
        public int ControlIndex
        {
            get
            {
                return miControlIndex;
            }
            set
            {
                miControlIndex = value;
            }
        }

        public int ControlHeight
        {
            get
            {
                return miHeight;
            }
            set
            {
                if (value > 0)
                {
                    miHeight = value;
                }
            }
        }

        public int ControlWidth
        {
            get
            {
                return miWidth;
            }
            set
            {
                if (value > 0)
                {
                    miWidth = value;
                }
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Item (String)
        // ------------------------------------------------------------------------------------------
        public string get_Item(int iKey)
        {
            try
            {
                return mcItem[iKey];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public void set_Item(int iKey, string value)
        {
            if (mcItem is null)
            {
                mcItem = new List<string>();
            }

            if (iKey < mcItem.Count)
            {
                mcItem[iKey] = value;
            }
            else
            {
                mcItem.Add(value);
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: ItemData (String)
        // ------------------------------------------------------------------------------------------
        public string get_ItemData(int iKey)
        {
            try
            {
                return mcItemData[iKey];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public void set_ItemData(int iKey, string value)
        {
            if (mcItemData is null)
                mcItemData = new List<string>();
            if (iKey < mcItemData.Count)
            {
                mcItemData[iKey] = value;
            }
            else
            {
                mcItemData.Add(value);
            }
        }

        public object RangeMin
        {
            get
            {
                return mvRangeMin;
            }
            set
            {
                mvRangeMin = value;
            }
        }

        public object RangeMax
        {
            get
            {
                return mvRangeMax;
            }
            set
            {
                mvRangeMax = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Required (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool Required
        {
            get
            {
                return mbRequired;
            }
            set
            {
                mbRequired = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: SetWidth (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool SetWidth
        {
            get
            {
                return mbSetWidth;
            }
            set
            {
                mbSetWidth = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: StartFocusHere (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool StartFocusHere
        {
            get
            {
                return mbStartFocusHere;
            }
            set
            {
                mbStartFocusHere = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Tag (String)
        // ------------------------------------------------------------------------------------------
        public string Tag
        {
            get
            {
                return msTag;
            }
            set
            {
                msTag = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Caption (String)
        // ------------------------------------------------------------------------------------------
        public string Text
        {
            get
            {
                return msText;
            }
            set
            {
                msText = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Validate (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool Validate
        {
            get
            {
                return mbValidate;
            }
            set
            {
                mbValidate = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Value (Object)
        // ------------------------------------------------------------------------------------------
        public object Value
        {
            get
            {
                return mvValue;
            }
            set
            {
                mvValue = value;
            }
        }
    }
}