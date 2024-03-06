using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;

namespace ClosetUI.Models.Models;

public static class Utils
{
    public static void CopyPropertiesTo(this object fromObject, object toObject)
    {
        PropertyInfo[] toObjectProperties = toObject.GetType().GetProperties();
        foreach (PropertyInfo propTo in toObjectProperties)
        {
            PropertyInfo propFrom = fromObject.GetType().GetProperty(propTo.Name);
            if (propFrom != null && propFrom.CanWrite)
                propTo.SetValue(toObject, propFrom.GetValue(fromObject, null), null);
        }
    }

    /// </summary>
    /// <param name="date"></param>
    /// <param name="pattern">YYYYMMDD or YYYYMMDDHHMM</param>
    /// <returns></returns>
    public static string Date2String(this DateTime date, string pattern = "yyyyMMdd")
    {
        if (pattern == "yyyyMMdd")
            return date.ToString("yyyyMMdd");
        else
            return date.ToString("yyyyMMddHHmm");
    }

    /// <summary>
    /// DateTime date
    /// </summary>
    /// <param name="date"></param>
    /// <returns>int YYYYMMDD</returns>
    public static int Date2Int(this DateTime date)
    {
        return Convert.ToInt32(Date2String(date));
    }

    /// <summary>
    /// DateTime date
    /// </summary>
    /// <param name="int_date"></param>
    /// <returns>DateTime</returns>
    public static DateTime Int2Date(this int int_date)
    {
        if (int_date == 0)
            int_date = 19730101;
        return DateTime.ParseExact(int_date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
    }


    /// <summary>
    /// DateTime date
    /// </summary>
    /// <param name="int_date"></param>
    /// <returns>DateTime</returns>
    public static string Int2DateString(this int int_date)
    {
        if (int_date == 0)
            int_date = 19730101;
        var d = DateTime.ParseExact(int_date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
        return d.ToString("dd/MM/yyyy");
    }

    /// <summary>
    /// DateTime date
    /// </summary>
    /// <param name="date"></param>
    /// <returns>long YYYYMMDDHHMM</returns>
    public static long Date2Long(this DateTime date)
    {
        return long.Parse(Date2String(date, "YYYYMMDDHHMM"));
    }

    /// <summary>
    /// DateTime date
    /// </summary>
    /// <param name="date"></param>
    /// <returns>DateTime</returns>
    public static DateTime Long2Date(this long long_date)
    {
        string long_s = long_date.ToString();   // YYYYMMDDHHMM

        int year = Convert.ToInt32(long_s.Substring(0, 4));
        int month = Convert.ToInt32(long_s.Substring(4, 2));
        int day = Convert.ToInt32(long_s.Substring(6, 2));
        int hour = Convert.ToInt32(long_s.Substring(8, 2));
        int minute = Convert.ToInt32(long_s.Substring(10, 2));
        return new DateTime(year, month, day, hour, minute, 0);
    }

    /// <summary>
    /// DateTime date
    /// </summary>
    /// <param name="date"></param>
    /// <returns>DateTime</returns>
    public static string Long2Hour(this long long_date)
    {
        string long_s = long_date.ToString();   // YYYYMMDDHHMM 
        return long_s.Substring(8, 2) + ":" + long_s.Substring(10, 2);
    }

    public static string Long2DateYYYYMMDD(this long long_date)
    {
        var d = DateTime.ParseExact(long_date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
        return d.ToString("dd/MM/yyyy");
    }

    /// <summary>
    /// convert string YYYYMMDDHHMM to date
    /// </summary>
    /// <param name="yyyyMMddhhmm"></param>
    /// <returns>DateTime</returns>
    public static DateTime LongString2Date(this string yyyyMMddhhmm)
    {
        string long_s = yyyyMMddhhmm.ToString();   // YYYYMMDDHHMM

        int year = Convert.ToInt32(long_s.Substring(0, 4));
        int month = Convert.ToInt32(long_s.Substring(4, 2));
        int day = Convert.ToInt32(long_s.Substring(6, 2));
        int hour = Convert.ToInt32(long_s.Substring(8, 2));
        int minute = Convert.ToInt32(long_s.Substring(10, 2));
        return new DateTime(year, month, day, hour, minute, 0);
        // return DateTime.ParseExact(yyyyMMddhhmm, "yyyyMMddhhmm", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// DateTime date
    /// </summary>
    /// <param name="date"></param>
    /// <returns>DateTime</returns>
    public static string Long2StringHour(this long long_date)
    {
        string long_s = long_date.ToString();   // YYYYMMDDHHMM
        int hour = Convert.ToInt32(long_s.Substring(8, 2));
        int minute = Convert.ToInt32(long_s.Substring(10, 2));
        return minute.ToString("00") + ":" + hour.ToString("00");
    }

    public static string GetDayOfWeek(this DateTime date_, Langu language)
    {
        string DOW = date_.DayOfWeek.ToString();
        switch (DOW)
        {
            case "Sunday":
            case "ראשון":
                DOW = language == Langu.EN ? "Sunday" : "ראשון";
                break;
            case "Monday":
            case "שני":
                DOW = language == Langu.EN ? "Monday" : "שני";
                break;
            case "Tuesday":
            case "שלישי":
                DOW = language == Langu.EN ? "Tuesday" : "שלישי";
                break;
            case "Wednesday":
            case "רביעי":
                DOW = language == Langu.EN ? "Wednesday" : "רביעי";
                break;
            case "Thursday":
            case "חמישי":
                DOW = language == Langu.EN ? "Thursday" : "חמישי";
                break;
            case "Friday":
            case "ששי":
                DOW = language == Langu.EN ? "Friday" : "ששי";
                break;
            case "Saturday":
            case "שבת":
                DOW = language == Langu.EN ? "Saturday" : "שבת";
                break;
        }
        return DOW;
    }

    public static string GetMonthName(this DateTime date_, Langu language)
    {
        string month_ = date_.Month.ToString();
        switch (month_)
        {
            case "1":
                month_ = language == Langu.EN ? "January" : "ינואר";
                break;
            case "2":
                month_ = language == Langu.EN ? "February" : "פברואר";
                break;
            case "3":
                month_ = language == Langu.EN ? "March" : "מרץ";
                break;
            case "4":
                month_ = language == Langu.EN ? "April" : "אפריל";
                break;
            case "5":
                month_ = language == Langu.EN ? "May" : "מאי";
                break;
            case "6":
                month_ = language == Langu.EN ? "June" : "יוני";
                break;
            case "7":
                month_ = language == Langu.EN ? "July" : "יולי";
                break;
            case "8":
                month_ = language == Langu.EN ? "August" : "אוגוסט";
                break;
            case "9":
                month_ = language == Langu.EN ? "September" : "ספטמבר";
                break;
            case "10":
                month_ = language == Langu.EN ? "October" : "אוקטובר";
                break;
            case "11":
                month_ = language == Langu.EN ? "November" : "נובמבר";
                break;
            case "12":
                month_ = language == Langu.EN ? "December" : "דצמבר";
                break;
        }
        return month_;
    }

    public static DateTime GetSundayBelow(this DateTime date)
    {
        while (date.GetDayOfWeek(Langu.EN) != "Sunday")
            date = date.AddDays(-1);
        return date;
    }

    public static DateTime GetSundayAbove(this DateTime date)
    {
        while (date.GetDayOfWeek(Langu.EN) != "Sunday")
            date = date.AddDays(1);
        return date;
    }

    public static DateTime GetSaturdayBelow(this DateTime date)
    {
        while (date.GetDayOfWeek(Langu.EN) != "Saturday")
            date = date.AddDays(-1);
        return date;
    }

    public static DateTime GetSaturdayAbove(this DateTime date)
    {
        while (date.GetDayOfWeek(Langu.EN) != "Saturday")
            date = date.AddDays(1);
        return date;
    }

    public static int GetNextDay(this int date)
    {
        return (date.Int2Date().AddDays(1)).Date2Int();
    }

    #region Convert DataTable to List using a Generic Method
    private static List<T> ConvertDataTable<T>(DataTable dt)
    {
        List<T> data = new List<T>();
        foreach (DataRow row in dt.Rows)
        {
            T item = GetItem<T>(row);
            data.Add(item);
        }
        return data;
    }
    private static T GetItem<T>(DataRow dr)
    {
        Type temp = typeof(T);
        T obj = Activator.CreateInstance<T>();

        foreach (DataColumn column in dr.Table.Columns)
        {
            foreach (PropertyInfo pro in temp.GetProperties())
            {
                if (pro.Name == column.ColumnName)
                    pro.SetValue(obj, dr[column.ColumnName], null);
                else
                    continue;
            }
        }
        return obj;
    }



    public static string DateToString()
    {
        DateTime now = DateTime.Now;
        int Year = now.Year;
        int Month = now.Month;
        int Day = now.Day;

        return Year.ToString() + (Month < 10 ? "0" + Month.ToString() : Month.ToString()) + (Day < 10 ? "0" + Day.ToString() : Day.ToString());
    }

    public static string DateToString_YYYYMMDDHHMM(this DateTime d)
    {
        int Year = d.Year;
        int Month = d.Month;
        int Day = d.Day;
        int Hour = d.Hour;
        int Minute = d.Minute;

        return Year.ToString() +
               (Month < 10 ? "0" + Month.ToString() : Month.ToString()) +
               (Day < 10 ? "0" + Day.ToString() : Day.ToString()) +
               (Hour < 10 ? "0" + Hour.ToString() : Hour.ToString()) +
               (Minute < 10 ? "0" + Minute.ToString() : Minute.ToString());
    }
    public static string DateToString_YYYYMMDDHHMMSS(this DateTime d)
    {
        int Year = d.Year;
        int Month = d.Month;
        int Day = d.Day;
        if (d.Hour == 0 && d.Minute == 0 && d.Second == 0)
        {
            d = d.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute).AddSeconds(DateTime.Now.Second);
        }
        int Hour = d.Hour;
        int Minute = d.Minute;
        int Second = d.Second;

        return (Year < 100 ? (2000 + Year).ToString() : Year.ToString() +
               (Month < 10 ? "0" + Month.ToString() : Month.ToString()) +
               (Day < 10 ? "0" + Day.ToString() : Day.ToString()) +
               (Hour < 10 ? "0" + Hour.ToString() : Hour.ToString()) +
               (Minute < 10 ? "0" + Minute.ToString() : Minute.ToString()) +
               (Second < 10 ? "0" + Second.ToString() : Second.ToString()));
    }

    public static long DateToLONG_YYYYMMDDHHMMSS(this DateTime d)
    {
        long l = 0;
        long.TryParse(DateToString_YYYYMMDDHHMMSS(d), out l);
        return l;
    }

    public static long DateToLONG_YYYYMMDDHHMMSS_1(this DateTime d)
    {
        long l = 0;
        int Year = d.Year;
        int Month = d.Month;
        int Day = d.Day;
        int Hour = d.Hour;
        int Minute = d.Minute;
        int Second = d.Second;

        string s =
               (Year < 100 ? (2000 + Year).ToString() : Year.ToString() +
               (Month < 10 ? "0" + Month.ToString() : Month.ToString()) +
               (Day < 10 ? "0" + Day.ToString() : Day.ToString()) +
               (Hour < 10 ? "0" + Hour.ToString() : Hour.ToString()) +
               (Minute < 10 ? "0" + Minute.ToString() : Minute.ToString()) +
               (Second < 10 ? "0" + Second.ToString() : Second.ToString()));
        long.TryParse(s, out l);
        return l;
    }

    public static int LONG2INT_YYYYMMDD(this long l_YYYYMMDDHHMMSS)
    {
        return int.Parse(l_YYYYMMDDHHMMSS.ToString().Substring(0, 8));
    }

    public static int DateTo_int_YYYYMMDD(this DateTime d)
    {
        int Year = d.Year;
        Year = Year < 100 ? 2000 + Year : Year;
        int Month = d.Month;
        int Day = d.Day;

        string int_date = Year.ToString() +
               (Month < 10 ? "0" + Month.ToString() : Month.ToString()) +
               (Day < 10 ? "0" + Day.ToString() : Day.ToString());

        int date1 = 0;
        int.TryParse(int_date, out date1);
        return date1;
    }

    public static string int_ToDate_YYYYMMDD(this int YYYYMMDD)
    {
        if (YYYYMMDD == 0)
            return "";
        string result = "{0}/{1}/{2}";
        string str_YYYYMMDD = YYYYMMDD.ToString();
        string Year = str_YYYYMMDD.Substring(0, 4);
        string Month = str_YYYYMMDD.Substring(4, 2);
        string Day = str_YYYYMMDD.Substring(6, 2);

        return string.Format(result, Day, Month, Year);
    }

    public static string Long_ToDate_YYYYMMDD(this long YYYYMMDDHHMMSS)
    {
        if (YYYYMMDDHHMMSS == 0)
            return "";
        string result = "{0}/{1}/{2}";
        string str_YYYYMMDD = YYYYMMDDHHMMSS.ToString();
        string Year = str_YYYYMMDD.Substring(0, 4);
        string Month = str_YYYYMMDD.Substring(4, 2);
        string Day = str_YYYYMMDD.Substring(6, 2);

        return string.Format(result, Day, Month, Year);
    }

    public static DateTime int_ToDateTime_YYYYMMDD(this int YYYYMMDD)
    {
        string str_YYYYMMDD = YYYYMMDD.ToString();
        int Year = int.Parse(str_YYYYMMDD.Substring(0, 4));
        int Month = int.Parse(str_YYYYMMDD.Substring(4, 2));
        int Day = int.Parse(str_YYYYMMDD.Substring(6, 2));

        return new DateTime(Year, Month, Day);
    }

    public static int Time2int_HHMMSS(this DateTime d)
    {
        int HH = d.Hour;
        int MM = d.Minute;
        int SS = d.Second;

        string int_time = HH.ToString() +
               (MM < 10 ? "0" + MM.ToString() : MM.ToString()) +
               (SS < 10 ? "0" + SS.ToString() : SS.ToString());

        int time1 = 0;
        int.TryParse(int_time, out time1);
        return time1;
    }

    public static int Date2int_YYYYMMDD(this DateTime d)
    {
        string Year = d.Year < 100 ? (2000 + d.Year).ToString() : d.Year.ToString();
        string Month = d.Month < 10 ? "0" + d.Month.ToString() : d.Month.ToString();
        string Day = d.Day < 10 ? "0" + d.Day.ToString() : d.Day.ToString();
        string date_ = Year + Month + Day;

        int date1 = 20231229;
        int.TryParse(date_, out date1);

        return date1;
    }

    public static string ToStringFromDateTimeYYYYMMDDHHMMSS(this DateTime date)
    {
        string Year = date.Year < 100 ? (2000 + date.Year).ToString() : date.Year.ToString();
        string Month = date.Month < 10 ? "0" + date.Month.ToString() : date.Month.ToString();
        string Day = date.Day < 10 ? "0" + date.Day.ToString() : date.Day.ToString();
        string Hour = date.Hour < 10 ? "0" + date.Hour.ToString() : date.Hour.ToString();
        string Minute = date.Minute < 10 ? "0" + date.Minute.ToString() : date.Minute.ToString();
        string Second = date.Second < 10 ? "0" + date.Second.ToString() : date.Second.ToString();
        return Year + Month + Day + Hour + Minute + Second;
    }

    public static string ToStringFromDateTimeYYYYMMDD(this DateTime date)
    {
        string Year = date.Year < 100 ? (2000 + date.Year).ToString() : date.Year.ToString();
        string Month = date.Month < 10 ? "0" + date.Month.ToString() : date.Month.ToString();
        string Day = date.Day < 10 ? "0" + date.Day.ToString() : date.Day.ToString();
        return Year + Month + Day;
    }

    /// <summary>
    /// return Month as MM string 2 digits
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string ToStringFromDateTimeYYYYMM(this DateTime date)
    {
        string Year = date.Year < 100 ? (2000 + date.Year).ToString() : date.Year.ToString();
        string Month = date.Month < 10 ? "0" + date.Month.ToString() : date.Month.ToString();
        return Year + Month;
    }

    public static string ToStringFromDateTimeYYYY(this DateTime date)
    {
        string Year = date.Year < 100 ? (2000 + date.Year).ToString() : date.Year.ToString();
        return Year;
    }

    public static string ToStringDateTimeFormatDateTimeYYYYMMDD(this DateTime date)
    {
        string result = "{0}/{1}/{2}";
        string Year = date.Year < 100 ? (2000 + date.Year).ToString() : date.Year.ToString();
        string Month = date.Month < 10 ? "0" + date.Month.ToString() : date.Month.ToString();
        string Day = date.Day < 10 ? "0" + date.Day.ToString() : date.Day.ToString();
        return string.Format(result, Day, Month, Year);

    }

    public static string ToStringDateTimeFormatStringYYYYMMDD(this string date)
    {
        if (date.Length >= 8)
        {
            string result = "{0}/{1}/{2}";
            string Year = date.Substring(0, 4);
            string Month = date.Substring(4, 2);
            string Day = date.Substring(6, 2);
            return string.Format(result, Day, Month, Year);
        }
        else
        {
            return "";
        }
    }

    public static string ToStringDateTimeFormatStringHHMMYYYYMMDD(this string date)
    {
        if (date.Length != 8)
        {
            string result = "{0}/{1}/{2} {3}:{4}";
            string Hour = date.Substring(8, 2);
            string Minute = date.Substring(10, 2);
            string Year = date.Substring(0, 4);
            string Month = date.Substring(4, 2);
            string Day = date.Substring(6, 2);
            return string.Format(result, Day, Month, Year, Hour, Minute);
        }
        else
        {
            string result = "{0}/{1}/{2}";
            string Year = date.Substring(0, 4);
            string Month = date.Substring(4, 2);
            string Day = date.Substring(6, 2);
            return string.Format(result, Day, Month, Year);
        }
    }

    public static string ToStringDateTimeFormatStringHHMM(this string date)
    {
        if (date == "19730207000000")
        {
            return "";
        }

        string result = "{0}:{1}";
        string Hour = date.Substring(8, 2);
        string Minute = date.Substring(10, 2);
        return string.Format(result, Hour, Minute);
    }

    public static string Old_Ver_Postfix(this DateTime date)
    {
        string Year = date.Year < 100 ? (2000 + date.Year).ToString() : date.Year.ToString();
        string Month = date.Month < 10 ? "0" + date.Month.ToString() : date.Month.ToString();
        string Day = date.Day < 10 ? "0" + date.Day.ToString() : date.Day.ToString();
        return "-OLD-" + Day + "." + Month + "." + Year;
    }

    public static DateTime ToDateTimeFROMStringYYYYMMDD(this string date_yyyyMMddHHmmss)
    {
        int Year = int.Parse(date_yyyyMMddHHmmss.Substring(0, 4));
        int Month = int.Parse(date_yyyyMMddHHmmss.Substring(4, 2));
        int Day = int.Parse(date_yyyyMMddHHmmss.Substring(6, 2));

        return new DateTime(Year, Month, Day);
    }

    public static DateTime ToDateTimeFROMStringYYYYMMDDHHMMSS(this string date_yyyyMMddHHmmss)
    {
        int Year = int.Parse(date_yyyyMMddHHmmss.Substring(0, 4));
        int Month = int.Parse(date_yyyyMMddHHmmss.Substring(4, 2));
        int Day = int.Parse(date_yyyyMMddHHmmss.Substring(6, 2));
        int Hour = int.Parse(date_yyyyMMddHHmmss.Substring(8, 2));
        int Minute = int.Parse(date_yyyyMMddHHmmss.Substring(10, 2));
        int Second = int.Parse(date_yyyyMMddHHmmss.Substring(12, 2));

        return new DateTime(Year, Month, Day, Hour, Minute, Second);
    }
    /// <summary>
    /// Convert to DateTime from string YYYYMMDDHHMMSS Or YYYYMMDD
    /// </summary>
    /// <param name="date_yyyyMMddHHmmss"></param>
    /// <returns></returns>
    public static DateTime ToDateTimeFROMLong(this long YYYYMMDDHHMMSS)
    {
        string s = YYYYMMDDHHMMSS.ToString();
        if (s.Length == 8)
        {
            s += "000000";
        }

        if (s.Length == 14)
        {
            int Year = int.Parse(s.Substring(0, 4));
            int Month = int.Parse(s.Substring(4, 2));
            int Day = int.Parse(s.Substring(6, 2));
            int Hour = int.Parse(s.Substring(8, 2));
            int Minute = int.Parse(s.Substring(10, 2));
            int Second = int.Parse(s.Substring(12, 2));
            return new DateTime(Year, Month, Day, Hour, Minute, Second);
        }
        else
        {
            return DateTime.Now;
        }
    }

    public static string Get_DOW_Heb(this DateTime date_)
    {
        string _DayOfWeek = date_.DayOfWeek.ToString();

        if (_DayOfWeek == "Sunday" || _DayOfWeek == "ראשון")
        {
            _DayOfWeek = "ראשון";
        }
        else if (_DayOfWeek == "Monday" || _DayOfWeek == "שני")
        {
            _DayOfWeek = "שני";
        }
        else if (_DayOfWeek == "Tuesday" || _DayOfWeek == "שלישי")
        {
            _DayOfWeek = "שלישי";
        }
        else if (_DayOfWeek == "Wednesday" || _DayOfWeek == "רביעי")
        {
            _DayOfWeek = "רביעי";
        }
        else if (_DayOfWeek == "Thursday" || _DayOfWeek == "חמישי")
        {
            _DayOfWeek = "חמישי";
        }
        else if (_DayOfWeek == "Friday" || _DayOfWeek == "ששי")
        {
            _DayOfWeek = "ששי";
        }
        else if (_DayOfWeek == "Saturday" || _DayOfWeek == "שבת")
        {
            _DayOfWeek = "שבת";
        }

        return _DayOfWeek;
    }

    public static string Get_DOW_Eng(this DateTime date_)
    {
        string _DayOfWeek = date_.DayOfWeek.ToString();

        if (_DayOfWeek == "Sunday" || _DayOfWeek == "ראשון")
        {
            _DayOfWeek = "Sunday";
        }
        else if (_DayOfWeek == "Monday" || _DayOfWeek == "שני")
        {
            _DayOfWeek = "Monday";
        }
        else if (_DayOfWeek == "Tuesday" || _DayOfWeek == "שלישי")
        {
            _DayOfWeek = "Tuesday";
        }
        else if (_DayOfWeek == "Wednesday" || _DayOfWeek == "רביעי")
        {
            _DayOfWeek = "Wednesday";
        }
        else if (_DayOfWeek == "Thursday" || _DayOfWeek == "חמישי")
        {
            _DayOfWeek = "Thursday";
        }
        else if (_DayOfWeek == "Friday" || _DayOfWeek == "ששי")
        {
            _DayOfWeek = "Friday";
        }
        else if (_DayOfWeek == "Saturday" || _DayOfWeek == "שבת")
        {
            _DayOfWeek = "Saturday";
        }

        return _DayOfWeek;
    }

    public static string PrepareOutput(this string s, int len)
    {
        if (s == null)
            return "";
        string result = "";
        result = s.ReverseString();
        result.Trim();
        //var arr = s.Split(' ');
        //for (int i = 0; i < arr.Length; i++)
        //{
        //    string txt = arr[i];
        //    if (!Regex.IsMatch(txt, "^[a-zA-Z0-9()./]*$"))
        //    {
        //        txt = txt.ReverseString();
        //    }
        //    arr[i] = txt;
        //}
        //for (int i = arr.Length; i > 0; i--)
        //{
        //    result += arr[i - 1];
        //    result += " ";
        //}
        //if (result.Length > len)
        //{
        //    result = result.Substring(0, len);
        //}

        while (result.Length < len)
        {
            result = result.Insert(0, " ");
        }

        return result;
    }

    public static string ReverseString_new(this string text)
    {
        string input = text;

        // Create a TextInfo for Hebrew language, which preserves RTL ordering
        TextInfo textInfo = new CultureInfo("he-IL", false).TextInfo;

        // Split the string into words
        string[] words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // Mirror each word and rebuild the string
        StringBuilder mirroredBuilder = new StringBuilder();
        foreach (string word in words)
        {
            // Reverse the characters of each word while preserving RTL order
            string mirroredWord = new string(word.Reverse().ToArray());

            // Append the mirrored word to the result
            mirroredBuilder.Append(mirroredWord);

            // Add a space to separate words (if not the last word)
            if (word != words[words.Length - 1])
            {
                mirroredBuilder.Append(' ');
            }
        }

        return textInfo.IsRightToLeft ? mirroredBuilder.ToString() : input;

    }

    public static string ReverseString(this string text)
    {
        if (text == null)
            return "";
        text = text.Replace("(", " ( ");
        text = text.Replace(")", " ) ");
        string HebPat = @"^[.""'אבגדהוזחטיכלמנסעפצקרשתךץףןם]+$";
        //string EngPat = @"^[a-zA-Z]+$";
        //string DigPat = @"^[0-9]+$";
        List<string> l1 = new List<string>();
        var l = text.Split(' ');
        foreach (var item in l)
        {
            Regex regex = new Regex(HebPat);
            if (regex.IsMatch(item))
                l1.Add(item.ReverseString1());
            else
                l1.Add(item);
        }

        l1 = l1.Where(tt => tt.Length > 0).ToList();
        string s = "";
        string s1 = "";
        foreach (var item in l1)
        {
            s = item;
            if (s.Length > 0)
                s += " ";
            s += s1;
            s1 = s;
        }

        string tmp = "";
        for (int i = 0; i < s1.Length; i++)
        {
            if (s1[i] == '(')
                tmp += ')';
            if (s1[i] == ')')
                tmp += '(';
            if (s1[i] != '(' && s1[i] != ')')
                tmp += s1[i];
        }
        tmp = tmp.Replace("( ", " (");
        tmp = tmp.Replace(" )", ") ");
        s1 = tmp;
        return s1;
    }

    private static string ReverseString1(this string text)
    {
        char[] cArray = text.ToCharArray();
        string reverse = string.Empty;
        for (int i = cArray.Length - 1; i > -1; i--)
        {
            reverse += cArray[i];
        }
        return reverse;
    }


    internal static string Trnslt2Heb(string name)
    {
        if (name == null)
            return "";

        switch (name)
        {
            case "":
                return "";

            case "Numer":
                return "מס'";

            case "ClntNum":
                return "מס' לקוח";

            case "FullName":
                return "שם לקוח";

            case "Tel1":
                return "שלפון";

            case "Tel2":
                return "שלפון";

            case "Cel1":
                return "שלפון";

            case "Cel2":
                return "שלפון";

            case "Status":
                return "סטטוס";

            case "City":
                return "עיר";

            case "LastBuyDate":
                return "ת' קנ' אחרון";

            case "LastPayDate":
                return "ת' תש' אחרון";

            case "MaxLimit":
                return "מסגרת";

            case "ClntBalance":
                return "יתרה";

            case "TotSales":
                return "סך מכירות";

            case "TotDepos":
                return "סך תשלומים";

            case "InCP":
                return "גבייה";


        }

        return "";
    }



    #endregion
    /*
        public static DateTime onIntToDate(int date)
        {
            if (date == 0)
                date = 19730101;
            return DateTime.ParseExact(date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static DateTime onLongToDate(long date)
        {
            return DateTime.ParseExact(date.ToString(), "yyyyMMddhhmm", CultureInfo.InvariantCulture);
        }

        public static int onDateToInt(DateTime date)
        {
            return int.Parse(date.ToString("yyyyMMdd"));
        }

        public static long onDateToLong(DateTime date)
        {
            return Int64.Parse(date.ToString("yyyyMMddhhmm"));
        }


                public static byte[] StringToByteArray(string str)
                {
                    byte[] array = Encoding.UTF8.GetBytes(str);
                    return array;
                }

                public static string ByteArrayToString(byte[] str)
                {
                  return  Encoding.Default.GetString(str);
                }*/
}
