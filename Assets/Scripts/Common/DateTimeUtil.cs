using System;
using UnityEngine;
/// <summary>
/// ʱ�乤����
/// </summary>
public static class DateTimeUtil
{
    /// <summary>
    /// ʱ�����ʱ��ʼʱ��
    /// </summary>
    private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// DateTimeת��Ϊ10λʱ�������λ���룩
    /// </summary>
    /// <param name="dateTime"> DateTime</param>
    /// <returns>10λʱ�������λ���룩</returns>
    public static long DateTimeToTimeStamp(DateTime dateTime)
    {
        return (long)(dateTime.ToUniversalTime() - timeStampStartTime).TotalSeconds;
    }

    /// <summary>
    /// DateTimeת��Ϊ13λʱ�������λ�����룩
    /// </summary>
    /// <param name="dateTime"> DateTime</param>
    /// <returns>13λʱ�������λ�����룩</returns>
    public static long DateTimeToLongTimeStamp(DateTime dateTime)
    {
        return (long)(dateTime.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
    }

    /// <summary>
    /// 10λʱ�������λ���룩ת��ΪDateTime
    /// </summary>
    /// <param name="timeStamp">10λʱ�������λ���룩</param>
    /// <returns>DateTime</returns>
    public static DateTime TimeStampToDateTime(long timeStamp)
    {
        return timeStampStartTime.AddSeconds(timeStamp).ToLocalTime();
    }

    /// <summary>
    /// 13λʱ�������λ�����룩ת��ΪDateTime
    /// </summary>
    /// <param name="longTimeStamp">13λʱ�������λ�����룩</param>
    /// <returns>DateTime</returns>
    public static DateTime LongTimeStampToDateTime(long longTimeStamp)
    {
        return timeStampStartTime.AddMilliseconds(longTimeStamp).ToLocalTime();
    }
    /// <summary>
    /// Sat, 05 Nov 2005 14:06:25 GMT 
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static string ToEnglishDateTimeString(DateTime dt)
    {
        if (dt != null)
        {
            string str = dt.GetDateTimeFormats('r')[0].ToString();//Sat, 05 Nov 2005 14:06:25 GMT 
            return str.Substring(4, str.Length - 11);
        }
        else
        {
            return null;
        }
    }
    public static string ToEnglishDateTimeString(long ldt)
    {
        var dt = TimeStampToDateTime(ldt);
        if (dt != null)
        {
            return dt.ToString("d");
        }
        else
        {
            return null;
        }
    }


    /**
     * @Description: ������ʱ���� ��ʽ��days:HI24:mm:ss СʱΪ24Сʱ�� 00:23:15:00
     */
    public static String secondToTime(long seconds)
    {
        long days = seconds / 86400;//ת������
        seconds = seconds % 86400;//ʣ������
        long hours = seconds / 3600;//ת��Сʱ��
        seconds = seconds % 3600;//ʣ������
        long minutes = seconds / 60;//ת������
        seconds = seconds % 60;//ʣ������
        string day = string.Format("{0:D2}", days);
        string hour = string.Format("{0:D2}", hours);
        string minute = string.Format("{0:D2}", minutes);
        string second = string.Format("{0:D2}", seconds);
        string ddHHmmss = day + ":" + hour + ":" + minute + ":" + second;
        return ddHHmmss;
    }

    /**
      * @Description: ���ط��� ��ʽ��hh:mm:ss СʱΪ24Сʱ�� 00:15:00
      */
    public static String secondToHHMMSS(long seconds)
    {
        long hours = seconds / 3600;//ת��Сʱ��
        seconds = seconds % 3600;//ʣ������
        long minutes = seconds / 60;//ת������
        seconds = seconds % 60;//ʣ������
 
        string hour = string.Format("{0:D2}", hours);
        string minute = string.Format("{0:D2}", minutes);
        string second = string.Format("{0:D2}", seconds);
        string ddHHmmss =  hour + ":" + minute + ":" + second;
        return ddHHmmss;
    }
    /**
      * @Description: ���ط��� ��ʽ��mm:ss СʱΪ24Сʱ�� 15:00
      */
    public static String secondToMMSS(long seconds)
    {
        long minutes = seconds / 60;//ת������
        seconds = seconds % 60;//ʣ������
        string minute = string.Format("{0:D2}", minutes);
        string second = string.Format("{0:D2}", seconds);
        string ddHHmmss =  minute + ":" + second;
        return ddHHmmss;
    }
}

