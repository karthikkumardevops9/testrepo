using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MSRecordsEngine.RecordsManager
{
    internal class ViewFilterTranslation
    {
        internal static string BuildStringValue(string filterData, string fieldName, string filterOperator, bool isAMemoColumn, ref bool valid)
        {
            valid = true;
            string rtn = string.Empty;

            if (isAMemoColumn)
            {
                fieldName = string.Format("CONVERT(VARCHAR(8000), [{0}])", fieldName);
            }
            else
            {
                fieldName = string.Format("[{0}]", fieldName);
            }

            if (string.IsNullOrEmpty(filterData))
            {
                switch (filterOperator.ToUpper() ?? "")
                {
                    case ">":
                    case "<>":
                        {
                            rtn = string.Format("({0} >= '!')", fieldName);
                            break;
                        }
                    case ">=":
                        {
                            rtn = string.Format("(({0} IS NULL) OR ({0} >= '!'))", fieldName);
                            break;
                        }
                    case "=":
                    case "<=":
                        {
                            rtn = string.Format("(({0} IS NULL) OR ({0} < '!'))", fieldName);
                            break;
                        }
                    case "<":
                        {
                            valid = false;
                            break;
                        }

                    default:
                        {
                            valid = false;
                            break;
                        }
                }

                return rtn;
            }

            if (string.Compare(filterOperator, "LIST", true) == 0)
            {
                return string.Format("{0} IN ({1}) ", fieldName, filterData);
            }

            var convertLiterals = default(bool);
            string charEscape = string.Empty;
            bool containsWildcards = CheckForWildcards(ref filterData, ref charEscape, ref convertLiterals);

            if (containsWildcards | string.Compare(filterOperator, "BEG", true) == 0 | string.Compare(filterOperator, "IN", true) == 0)
            {
                switch (filterOperator.ToUpper() ?? "")
                {
                    case "BEG":
                    case "IN":
                        {
                            if (filterData.EndsWith("%"))
                                filterData = filterData.Substring(0, filterData.Length - 1);
                            rtn = string.Format("{0} LIKE ", fieldName);
                            break;
                        }
                    case "=":
                        {
                            rtn = string.Format("{0} LIKE ", fieldName);
                            break;
                        }
                    case "<>":
                        {
                            rtn = string.Format("{0} NOT LIKE ", fieldName);
                            break;
                        }

                    default:
                        {
                            valid = false;
                            break;
                        }
                }
            }
            else if (convertLiterals)
            {
                rtn = string.Format("{0} LIKE ", fieldName);
            }
            else
            {
                rtn = string.Format("{0} {1} ", fieldName, filterOperator);
            }

            if (convertLiterals)
            {
                charEscape = " ESCAPE '" + charEscape + "'";
            }
            else
            {
                charEscape = string.Empty;
            }

            switch (filterOperator.ToUpper() ?? "")
            {
                case "BEG":
                    {
                        return rtn + string.Format("'{0}%'{1}", filterData, charEscape);
                    }
                case "IN":
                    {
                        return rtn + string.Format("'%{0}%'{1}", filterData, charEscape);
                    }

                default:
                    {
                        return rtn + string.Format("'{0}'{1}", filterData, charEscape);
                    }
            }
        }

        internal static string BuildNumericValue(string filterData, string fieldName, string filterOperator, ref bool valid)
        {
            valid = true;
            string rtn = string.Empty;
            fieldName = string.Format("[{0}]", fieldName);

            if (string.IsNullOrEmpty(filterData))
            {
                switch (filterOperator.ToUpper() ?? "")
                {
                    case ">":
                    case "<>":
                        {
                            rtn = string.Format("({0} {1} 0)", fieldName, filterOperator);
                            break;
                        }
                    case ">=":
                        {
                            rtn = string.Format("(({0} IS NULL) OR ({0} >= 0))", fieldName);
                            break;
                        }
                    case "=":
                    case "<=":
                        {
                            rtn = string.Format("(({0} IS NULL) OR ({0} {1} 0))", fieldName, filterOperator);
                            break;
                        }
                    case "<":
                        {
                            rtn = string.Format("({0} {1} 0)", fieldName, filterOperator);
                            break;
                        }

                    default:
                        {
                            valid = false;
                            break;
                        }
                }

                return rtn;
            }

            if (string.Compare(filterOperator, "LIST", true) == 0)
            {
                return string.Format("{0} IN ({1}) ", fieldName, filterData);
            }

            var convertLiterals = default(bool);
            string charEscape = string.Empty;
            bool containsWildcards = CheckForWildcards(ref filterData, ref charEscape, ref convertLiterals);

            if (containsWildcards | string.Compare(filterOperator, "BEG", true) == 0 | string.Compare(filterOperator, "IN", true) == 0)
            {
                switch (filterOperator.ToUpper() ?? "")
                {
                    case "BEG":
                    case "IN":
                        {
                            if (filterData.EndsWith("%"))
                                filterData = filterData.Substring(0, filterData.Length - 1);
                            rtn = string.Format("{0} LIKE ", fieldName);
                            break;
                        }
                    case "=":
                        {
                            rtn = string.Format("{0} LIKE ", fieldName);
                            break;
                        }
                    case "<>":
                        {
                            rtn = string.Format("{0} NOT LIKE ", fieldName);
                            break;
                        }

                    default:
                        {
                            valid = false;
                            throw new Exception(string.Format("The wildcards \"*\" and \"?\" can only be used with the \"=\" or \"<>\" Operators in the \"{0}\" Field.", fieldName));
                        }
                }
            }
            else if (convertLiterals)
            {
                rtn = string.Format("{0} LIKE ", fieldName);
            }
            else
            {
                rtn = string.Format("{0} {1} ", fieldName, filterOperator);
            }

            if (!Information.IsNumeric(filterData.Trim()))
            {
                valid = false;

                for (int i = 0, loopTo = filterData.Length - 1; i <= loopTo; i++)
                {
                    if ("1234567890.".Contains(filterData.Substring(i, 1)))
                    {
                        valid = true;
                    }
                    else if ("*?".Contains(filterData.Substring(i, 1)))
                    {
                        valid = true;
                        containsWildcards = true;
                    }
                    else
                    {
                        valid = false;
                        containsWildcards = false;
                        break;
                    }
                }

                if (!valid)
                {
                    throw new Exception(string.Format("\"{0}\" is a Numeric Field. Must contain only numbers or wildcards(\"?\" or \"*\").", fieldName));
                }

                if (containsWildcards)
                {
                    filterData = filterData.Replace("?", "_");
                    filterData = "'" + filterData.Replace("*", "%") + "'";
                }
            }

            if (convertLiterals)
            {
                charEscape = " ESCAPE '" + charEscape + "'";
            }
            else
            {
                charEscape = string.Empty;
            }

            switch (filterOperator.ToUpper() ?? "")
            {
                case "BEG":
                    {
                        return rtn + string.Format("'{0}%'{1}", filterData, charEscape);
                    }
                case "IN":
                    {
                        return rtn + string.Format("'%{0}%'{1}", filterData, charEscape);
                    }

                default:
                    {
                        return rtn + string.Format("{0}", filterData);
                    }
            }
        }

        internal static string BuildCheckboxValue(string filterData, string fieldName, string filterOperator, ref bool valid)
        {
            valid = true;
            fieldName = string.Format("[{0}]", fieldName);

            if (string.IsNullOrEmpty(filterData) || string.Compare(filterData, "0") == 0)
            {
                return string.Format("(({0} IS NULL) OR ({0} = 0))", fieldName, filterOperator);
            }
            else
            {
                return string.Format("({0} <> 0)", fieldName);
            }
        }

        internal static string BuildDateValue(string filterData, string fieldName, string filterOperator, ref bool valid)
        {
            var wildQuestion = default(bool);
            var wildAsterisk = default(bool);
            int index;
            int position;
            int position1;
            int iTemp;
            string dateSeparator = "-";
            string sHour = string.Empty;
            string originalDate;
            string sTemp = string.Empty;
            string timeSeparator = ":";
            string sTime = string.Empty;
            string sYear = string.Empty;
            string rtn = string.Empty;

            var aiDate = new int[3, 2];
            var aiTime = new int[4, 2];

            valid = true;
            if (string.IsNullOrEmpty(filterData))
                filterData = string.Empty;

            originalDate = filterData;

            if (filterData.Length > 0)
            {
                wildAsterisk = filterData.IndexOf("*") > -1;
                wildQuestion = filterData.IndexOf("?") > -1;

                if (!wildAsterisk && !wildQuestion)
                {
                    try
                    {
                        valid = Information.IsDate(filterData);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        valid = false;
                    }

                    if (valid)
                    {
                        rtn = fieldName;

                        if (filterOperator == "=")
                        {
                            rtn = "CONVERT(VARCHAR(" + filterData.Length.ToString() + "), " + fieldName + ",120)";

                            if (Strings.Format(Conversions.ToDate(filterData), "yyyy-MM-dd HH:mm:ss").IndexOf("00:00:00") > -1)
                            {
                                rtn += " LIKE '" + Strings.Format(Conversions.ToDate(filterData), "yyyy-MM-dd") + "%'";
                            }
                            else
                            {
                                rtn += " LIKE '" + Strings.Format(Conversions.ToDate(filterData), "yyyy-MM-dd HH:mm:ss") + "%'";
                            }
                        }
                        else if (filterOperator == "BEG")
                        {
                            rtn = "CONVERT(VARCHAR(" + filterData.Length.ToString() + "), " + fieldName + ",120)";

                            if (Strings.Format(Conversions.ToDate(filterData), "yyyy-MM-dd HH:mm:ss").IndexOf("00:00:00") > -1)
                            {
                                rtn += " LIKE '" + Strings.Format(Conversions.ToDate(filterData), "yyyy-MM-dd") + "%'";
                            }
                            else
                            {
                                rtn += " LIKE '" + Strings.Format(Conversions.ToDate(filterData), "yyyy-MM-dd HH:mm:ss") + "%'";
                            }
                        }
                        else if (filterOperator == "<>")
                        {
                            rtn = "CONVERT(VARCHAR(" + filterData.Length.ToString() + "), " + fieldName + ",120)";

                            if (Strings.Format(Conversions.ToDate(filterData), "yyyy-MM-dd HH:mm:ss").IndexOf("00:00:00") > -1)
                            {
                                rtn += " Not LIKE '" + Strings.Format(Conversions.ToDate(filterData), "yyyy-MM-dd") + "%'";
                            }
                            else
                            {
                                rtn += " Not LIKE '" + Strings.Format(Conversions.ToDate(filterData), "yyyy-MM-dd HH:mm:ss") + "%'";
                            }
                        }
                        else
                        {
                            rtn += " " + filterOperator + " '" + filterData + "'";
                        }

                        return rtn;
                    }
                    else
                    {
                        valid = true;
                    }
                }

                if (wildAsterisk)
                    filterData = filterData.Substring(0, filterData.IndexOf("*"));

                if (wildQuestion)
                {
                    filterData = filterData.Replace("??", "01");
                    filterData = filterData.Replace("0?", "01");
                    filterData = filterData.Replace("?", "0");
                }

                position = filterData.IndexOf(" ");
                if (position > -1)
                    filterData = filterData.Substring(position).Trim();

                if (Information.IsNumeric(filterData))
                {
                    iTemp = Conversions.ToShort(filterData);

                    if (iTemp > 31)
                    {
                        aiDate[2, 0] = 1;
                        aiDate[2, 1] = filterData.Length;
                    }
                    else if (iTemp > 12)
                    {
                        aiDate[1, 0] = 1;
                        aiDate[1, 1] = filterData.Length;
                    }
                    else
                    {
                        aiDate[0, 0] = 1;
                        aiDate[0, 1] = filterData.Length;
                    }
                }
                else
                {
                    // at least one non-numeric character
                    position = 1;
                    position1 = 1;
                    sTemp = filterData;

                    while (position <= filterData.Length)
                    {
                        if (!Information.IsNumeric(Strings.Mid(filterData, position, 1)))
                        {
                            if (position1 != position)
                            {
                                try
                                {
                                    iTemp = Conversions.ToShort(Strings.Mid(filterData, position1, position - position1));
                                }
                                catch (InvalidCastException ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                    valid = false;
                                    throw new Exception(string.Format("The Filter Date for the \"{0}\" Field must be in a numeric format.  (i.e. 01/01/2001)", fieldName));
                                }

                                if (!LoadDateTimeArray(ref aiDate, iTemp, position1, position - position1, 31, 12, false))
                                {
                                    valid = false;
                                    return string.Empty;
                                }

                                position1 = position + 1;
                            }
                            else
                            {
                                position1 = position - 1;
                            }
                        }

                        position += 1;
                    }

                    if (position1 == 0)
                    {
                        valid = false;
                        throw new Exception(string.Format("The Filter Date for the \"{0}\" Field must be in a numeric format.  (i.e. 01/01/2001)", fieldName));
                    }

                    if (Strings.Len(Strings.Mid(filterData, position1)) > 0)
                    {
                        if (Information.IsNumeric(Strings.Mid(filterData, position1)))
                        {
                            try
                            {
                                iTemp = Conversions.ToShort(Strings.Mid(sTemp, position1));
                            }
                            catch (InvalidCastException ex)
                            {
                                Debug.WriteLine(ex.Message);
                                valid = false;
                                throw new Exception(string.Format("The Filter Date for the \"{0}\" Field must be in a numeric format.  (i.e. 01/01/2001)", fieldName));
                            }

                            if (!LoadDateTimeArray(ref aiDate, iTemp, position1, position - position1, 31, 12, false))
                            {
                                valid = false;
                                return string.Empty;
                            }
                        }
                    }

                    filterData = string.Empty;

                    for (index = 0; index <= 2; index++)
                    {
                        if (aiDate[index, 0] > 0)
                            filterData += Strings.Mid(sTemp, aiDate[index, 0], aiDate[index, 1]) + dateSeparator;
                    }

                    filterData = filterData.Substring(0, filterData.Length - 1);
                    valid = Information.IsDate(filterData);
                }

                if (!valid)
                    return rtn;

                sTime = string.Empty;
                position = originalDate.IndexOf(" ");

                if (position > -1)
                    sTime = originalDate.Substring(position);

                wildAsterisk = wildAsterisk | sTime.IndexOf("*") > -1;
                wildQuestion = wildQuestion | sTime.IndexOf("?") > -1;

                if (wildAsterisk & sTime.Length > 0)
                    sTime = sTime.Substring(0, sTime.IndexOf("*"));

                if (wildQuestion)
                {
                    sTime = sTime.Replace("??", "01");
                    sTime = sTime.Replace("?", "0");
                }

                if (Information.IsNumeric(sTime))
                {
                    iTemp = Conversions.ToShort(sTime);
                    if (iTemp > 59)
                    {
                        valid = false;
                        return string.Empty;
                    }

                    if (iTemp > 23)
                    {
                        aiTime[1, 0] = 1;
                        aiTime[1, 1] = sTime.Length;
                    }
                    else
                    {
                        aiTime[0, 0] = 1;
                        aiTime[0, 1] = sTime.Length;
                    }
                }
                else if (sTime.Length > 0)
                {
                    // at least one non-numeric character
                    position = 1;
                    position1 = 1;
                    sTemp = sTime;

                    while (position <= sTime.Length)
                    {
                        if (!Information.IsNumeric(Strings.Mid(sTime, position, 1)))
                        {
                            if (position1 != position)
                            {
                                try
                                {
                                    iTemp = Conversions.ToShort(Strings.Mid(sTime, position1, position - position1));

                                    if (!LoadDateTimeArray(ref aiTime, iTemp, position1, position - position1, 59, 23, true))
                                    {
                                        valid = false;
                                        return string.Empty;
                                    }

                                    position1 = position + 1;
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }
                            }
                            else
                            {
                                position1 = position - 1;
                            }
                        }

                        position += 1;
                    }

                    if (Strings.Len(Strings.Mid(sTime, position1)) > 0)
                    {
                        if (Information.IsNumeric(Strings.Mid(sTime, position1)))
                        {
                            iTemp = Conversions.ToShort(Strings.Mid(sTime, position1));
                            if (!LoadDateTimeArray(ref aiTime, iTemp, position1, position - position1, 59, 23, true))
                            {
                                valid = false;
                                return string.Empty;
                            }
                        }
                        else
                        {
                            aiTime[3, 0] = position1;
                            aiTime[3, 1] = position - position1;
                        }
                    }

                    sTime = string.Empty;
                    for (index = 0; index <= 3; index++)
                    {
                        if (aiTime[index, 0] > 0)
                        {
                            sTime += Strings.Mid(sTemp, aiTime[index, 0], aiTime[index, 1]);
                            if (index < 2)
                                sTime += timeSeparator;
                        }
                    }

                    if (string.Compare(Strings.Right(sTime, 1), timeSeparator) == 0)
                        sTime = sTime.Substring(0, sTime.Length - 1);
                    valid = Information.IsDate(sTime);
                }

                if (!valid)
                    return string.Empty;
            }

            filterData = originalDate;
            sTime = string.Empty;

            if (filterData.Length == 0)
            {
                rtn = fieldName;

                switch (filterOperator ?? "")
                {
                    case "=":
                    case "<":
                    case "<=":
                        {
                            rtn += " IS NULL";
                            break;
                        }
                    case "<>":
                    case ">":
                        {
                            rtn += " IS NOT NULL";
                            break;
                        }
                    case ">=":
                        {
                            valid = true;
                            return string.Empty;
                        }

                    default:
                        {
                            valid = false;
                            throw new Exception(string.Format("Filter Data for the \"{0}\" Field is required when using the \"IN\" or \"BEG\" Operators.", fieldName));
                        }
                }
            }
            else
            {
                if (wildAsterisk | wildQuestion)
                {
                    if (string.Compare(filterOperator, "=") != 0 & string.Compare(filterOperator, "<>") != 0)
                    {
                        valid = false;
                        throw new Exception(string.Format("The wildcards \"*\" and \"?\" can only be used with the \"=\" or \"<>\" Operators in the \"{0}\" Field.", fieldName));
                    }
                }

                position = filterData.IndexOf(" ");

                if (position == -1)
                {
                    filterData = string.Empty;
                    sYear = string.Empty;
                    sTemp = originalDate;

                    wildAsterisk = sTemp.IndexOf("*") > -1;
                    wildQuestion = sTemp.IndexOf("?") > -1;

                    if (wildAsterisk)
                        sTemp = sTemp.Substring(0, sTemp.IndexOf("*"));

                    for (index = 0; index <= 2; index++)
                    {
                        if (aiDate[index, 0] > 0)
                            filterData += Strings.Mid(sTemp, aiDate[index, 0], aiDate[index, 1]) + dateSeparator;
                    }

                    filterData = filterData.Substring(0, filterData.Length - 1);
                    sYear = string.Empty;

                    if (aiDate[2, 0] > 0 & aiDate[2, 1] != 4)
                    {
                        try
                        {
                            sYear = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(Conversions.ToDate(filterData)).ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }

                    sTemp = originalDate;

                    if (wildAsterisk)
                        sTemp = sTemp.Substring(0, sTemp.IndexOf("*"));
                    if (wildQuestion)
                        sTemp = sTemp.Replace("?", "_");

                    filterData = string.Empty;

                    if (sYear.Length > 0)
                    {
                        filterData = sYear + dateSeparator;
                    }
                    else if (aiDate[2, 0] > 0)
                    {
                        filterData = Strings.Left("0000", 4 - aiDate[2, 1]) + Strings.Mid(sTemp, aiDate[2, 0], aiDate[2, 1]) + dateSeparator;
                    }
                    else
                    {
                        filterData = "____" + dateSeparator;
                    }

                    for (index = 0; index <= 1; index++)
                    {
                        if (aiDate[index, 0] > 0)
                        {
                            filterData += Strings.Left("00", 2 - aiDate[index, 1]) + Strings.Mid(sTemp, aiDate[index, 0], aiDate[index, 1]) + dateSeparator;
                        }
                        else
                        {
                            filterData += "__" + dateSeparator;
                        }
                    }

                    filterData = filterData.Substring(0, filterData.Length - 1);
                }
                else
                {
                    filterData = string.Empty;
                    sYear = string.Empty;
                    sTemp = originalDate;

                    wildAsterisk = sTemp.IndexOf("*") > -1;
                    wildQuestion = sTemp.IndexOf("?") > -1;

                    if (wildAsterisk)
                        sTemp = sTemp.Substring(0, sTemp.IndexOf("*"));

                    for (index = 0; index <= 2; index++)
                    {
                        if (aiDate[index, 0] > 0)
                            filterData += Strings.Mid(sTemp, aiDate[index, 0], aiDate[index, 1]) + dateSeparator;
                    }

                    filterData = filterData.Substring(0, filterData.Length - 1);
                    sYear = string.Empty;

                    if (aiDate[2, 0] > 0 & aiDate[2, 1] != 4)
                    {
                        try
                        {
                            sYear = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetYear(Conversions.ToDate(filterData)).ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }

                    sTemp = originalDate;

                    if (wildAsterisk)
                        sTemp = sTemp.Substring(0, sTemp.IndexOf("*"));
                    if (wildQuestion)
                        sTemp = sTemp.Replace("?", "_");

                    filterData = string.Empty;

                    if (Strings.Len(sYear) > 0)
                    {
                        filterData = sYear + dateSeparator;
                    }
                    else if (aiDate[2, 0] > 0)
                    {
                        filterData = Strings.Left("0000", 4 - aiDate[2, 1]) + Strings.Mid(sTemp, aiDate[2, 0], aiDate[2, 1]) + dateSeparator;
                    }
                    else
                    {
                        filterData = "____" + dateSeparator;
                    }

                    for (index = 0; index <= 1; index++)
                    {
                        if (aiDate[index, 0] > 0)
                        {
                            filterData += Strings.Left("00", 2 - aiDate[index, 1]) + Strings.Mid(sTemp, aiDate[index, 0], aiDate[index, 1]) + dateSeparator;
                        }
                        else
                        {
                            filterData += "__" + dateSeparator;
                        }
                    }

                    filterData = filterData.Substring(0, filterData.Length - 1);

                    sTime = string.Empty;
                    sTemp = Strings.Mid(originalDate, position + 1);

                    wildAsterisk = sTemp.IndexOf("*") > -1;
                    wildQuestion = sTemp.IndexOf("?") > -1;

                    if (wildAsterisk)
                        sTemp = sTemp.Substring(0, sTemp.IndexOf("*"));
                    if (wildQuestion)
                        sTemp = sTemp.Replace("?", "_");

                    for (index = 0; index <= 3; index++)
                    {
                        if (aiTime[index, 0] > 0)
                        {
                            sTime = sTime + Strings.Mid(sTemp, aiTime[index, 0], aiTime[index, 1]);
                            if (index < 2)
                                sTime += timeSeparator;
                        }
                    }

                    if (string.Compare(Strings.Right(sTime, 1), timeSeparator) == 0)
                        sTime = sTime.Substring(0, sTime.Length - 1);

                    sHour = string.Empty;

                    if (Strings.InStr(sTime, timeSeparator) > 0)
                    {
                        try
                        {
                            sHour = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetHour(Conversions.ToDate(sTime)).ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }

                    if (Strings.Len(sHour) > 0)
                    {
                        sTime = Strings.Left("00", 2 - Strings.Len(sHour)) + sHour;
                    }
                    else
                    {
                        sTime = Strings.Left("00", 2 - aiTime[0, 1]) + Strings.Mid(sTemp, aiTime[0, 0], aiTime[0, 1]);
                    }

                    for (index = 1; index <= 2; index++)
                    {
                        if (aiTime[index, 0] > 0)
                            sTime += timeSeparator + Strings.Left("00", 2 - aiTime[index, 1]) + Strings.Mid(sTemp, aiTime[index, 0], aiTime[index, 1]);
                    }

                    if (Strings.Len(sTime) > 0)
                        filterData += " " + sTime;
                }

                rtn += "CONVERT(VARCHAR(" + filterData.Length.ToString() + "), " + fieldName + ",120)";

                if (filterOperator == "=")
                {
                    rtn += " LIKE ";
                }
                else if (filterOperator == "BEG")
                {
                    rtn += " LIKE ";
                }
                else if (filterOperator == "<>")
                {
                    rtn += " Not LIKE ";
                }
                else
                {
                    rtn += " " + filterOperator + " ";
                }

                rtn += "'" + filterData + "'";
            }

            return rtn;
        }

        private static bool LoadDateTimeArray(ref int[,] aiDateTime, int value, int position, int length, int invalid1, int invalid2, bool invalid1IsAlwaysInvalid)
        {
            try
            {
                if (value > invalid1)
                {
                    if (invalid1IsAlwaysInvalid)
                        return false;
                    // too many values larger than iInvalid1 to be a valid date/time
                    if (aiDateTime[2, 0] > 0)
                        return false;
                    aiDateTime[2, 0] = position;
                    aiDateTime[2, 1] = length;
                }
                else if (value > invalid2)
                {
                    if (aiDateTime[1, 0] > 0)
                    {
                        // too many values larger than iInvalid2 to be a valid date/time
                        if (aiDateTime[2, 0] > 0)
                            return false;
                        aiDateTime[2, 0] = position;
                        aiDateTime[2, 1] = length;
                    }
                    else
                    {
                        aiDateTime[1, 0] = position;
                        aiDateTime[1, 1] = length;
                    }
                }
                else if (aiDateTime[0, 0] > 0)
                {
                    if (aiDateTime[1, 0] > 0)
                    {
                        aiDateTime[2, 0] = position;
                        aiDateTime[2, 1] = length;
                    }
                    else
                    {
                        aiDateTime[1, 0] = position;
                        aiDateTime[1, 1] = length;
                    }
                }
                else
                {
                    aiDateTime[0, 0] = position;
                    aiDateTime[0, 1] = length;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private static bool CheckForWildcards(ref string filterData, ref string charEscape, ref bool convertLiterals)
        {
            bool rtn = false;
            string charBackslash = string.Empty;
            string charAsterisk = string.Empty;
            string charQuestionMark = string.Empty;

            charEscape = string.Empty;

            for (int i = 1; i <= 250; i++)
            {
                if (i != 92) // don't use backslash(92) for ESCAPE character
                {
                    if (!filterData.Contains(Strings.Chr(i).ToString()))
                    {
                        if (charEscape.Length == 0)
                        {
                            charEscape = Conversions.ToString(Strings.Chr(i));
                        }
                        else if (charBackslash.Length == 0)
                        {
                            charBackslash = Conversions.ToString(Strings.Chr(i));
                        }
                        else if (charAsterisk.Length == 0)
                        {
                            charAsterisk = Conversions.ToString(Strings.Chr(i));
                        }
                        else if (charQuestionMark.Length == 0)
                        {
                            charQuestionMark = Conversions.ToString(Strings.Chr(i));
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            if (charEscape.Length == 0)
                charEscape = Conversions.ToString(Strings.Chr(251));
            if (charBackslash.Length == 0)
                charBackslash = Conversions.ToString(Strings.Chr(252));
            if (charAsterisk.Length == 0)
                charAsterisk = Conversions.ToString(Strings.Chr(253));
            if (charQuestionMark.Length == 0)
                charQuestionMark = Conversions.ToString(Strings.Chr(254));

            filterData = filterData.Replace("_", charEscape + "_");
            filterData = filterData.Replace("%", charEscape + "%");

            convertLiterals = filterData.Contains("_") | filterData.Contains("%");

            filterData = filterData.Replace(@"\\", charBackslash);
            filterData = filterData.Replace(@"\*", charAsterisk);
            filterData = filterData.Replace(@"\?", charQuestionMark);

            rtn = filterData.Contains("*") | filterData.Contains("?");

            filterData = filterData.Replace("*", "%");
            filterData = filterData.Replace("?", "_");

            filterData = filterData.Replace(charAsterisk, "*");
            filterData = filterData.Replace(charBackslash, @"\");
            filterData = filterData.Replace(charQuestionMark, "?");
            return rtn;
        }
    }
}