using UnityEngine;
using System.Collections;
using System;
using System.Globalization;

/// <summary>
/// 日期工具类
/// 使用前应该先调用下 Calibrate 校准下时间
/// </summary>
public class DateTimeUtil
{
    /// <summary>
    /// 服务器客户端时间差
    /// </summary>
    private static TimeSpan _deltaTime = new TimeSpan();

    /// <summary>
    /// 校准次数
    /// </summary>
    private static int _calibrationCount = 0;

    public static int calibrationCount
    {
        get { return _calibrationCount; }
        private set
        {
            if (value < int.MaxValue)
            {
                _calibrationCount = value;
            }
        }
    }

    /// <summary>
    /// 时间校准，将客户端时间校准到服务器端，计算个时间差。
    /// 根据当时设备的时区计算
    /// </summary>
    /// <param name="serverTimestamp">服务器时间戳(从1970-1-1 开始到现在(服务器时区的时间)的秒)</param>
    /// <returns>客户端和服务端时间差</returns>
    public static TimeSpan Calibrate(Int64 serverTimestamp)
    {
        //服务端(nodejs)发来的时间是从 "1970-1-1 00:00:00" 开始到现在的秒
        //客户端DateTime.Now 是从 "0001-1-1 00:00:00" 开始到现在的 100纳秒 
        //年份差了"1969"年, 单位差了 "1秒/100纳秒=10 000 000倍"
        //ps: 1秒=1000毫秒=1000*1000微秒=1000*1000*1000纳秒
        DateTime serverTime = new DateTime(serverTimestamp * 10000000).AddYears(1969).ToLocalTime();
        DateTime clientTime = DateTime.Now.ToLocalTime();
        _deltaTime = clientTime - serverTime;
        calibrationCount++;
        return _deltaTime;
    }

    /// <summary>
    /// 服务器现在时间（和时区有关）
    /// </summary>
    public static DateTime ServerNow
    {
        get
        {
            if (_calibrationCount == 0)
            {
                Debug.LogWarning("尚未与服务器时间校准");
            }

            return DateTime.Now.ToLocalTime().Subtract(_deltaTime);
        }
    }

    /// <summary>
    /// 客户端现在时间（和时区有关）
    /// </summary>
    public static DateTime ClientNow
    {
        get
        {
            return DateTime.Now.ToLocalTime();
        }
    }

    /// <summary>
    /// 通过服务器的时间戳 计算服务器时间
    /// </summary>
    /// <param name="timestamp">服务器时间戳</param>
    /// <returns></returns>
    public static DateTime GetServerTime(Int64 timestamp)
    {
        return new DateTime(timestamp * 10000000).AddYears(1969).ToLocalTime();
    }


    /// <summary>
    /// 通过字符串获取时间
    /// </summary>
    /// <param name="dateTimeString"></param>
    /// <returns></returns>
    public static DateTime Parse(string dateTimeString)
    {
        return DateTime.Parse(dateTimeString).ToLocalTime();
    }

    /// <summary>
    /// 日期格式
    /// </summary>
    private static DateTimeFormatInfo _provider = CultureInfo.GetCultureInfo("zh-cn").DateTimeFormat;

    static string _format24h = "yyyy-M-d H:m:s";

    /// <summary>
    /// 将字符串类型的中文日期格式转成DateTime
    /// </summary>
    /// <param name="dateStr">中文格式的日期字符串 "yyyy-M-d H:m:s" </param>
    /// <returns></returns>
    public static DateTime GetDateTime(string dateStr)
    {
        DateTime dt =  DateTime.ParseExact(dateStr, _format24h, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
        return dt;
    }
}