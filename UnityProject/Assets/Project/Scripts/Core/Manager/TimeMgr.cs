using System;
using UnityEngine;

public static class TimeMgr
{
    public static float ObjTimeScale = 1;
    public static float ObjDeltaTime
    {
        get
        {
            return Time.deltaTime * ObjTimeScale;
        }
        set
        {
            if (value <= 0)
            {
                ObjTimeScale = 0f;
                DebugUtil.Log("Can't set time to lower than 0");
            }
            ObjTimeScale = value;
            DebugUtil.Log($"Obj DeltaTime Set ::: {ObjTimeScale}");
        }
    }

    public static float ObjFixedTimeScale = 1;
    public static float ObjFixedDeltaTime
    {
        get
        {
            return Time.fixedDeltaTime * ObjFixedTimeScale;
        }
        set
        {
            if (value <= 0)
            {
                ObjFixedTimeScale = 0f;
                DebugUtil.Log("Can't set time to lower than 0");
            }
            ObjFixedTimeScale = value;
            DebugUtil.Log($"Obj DeltaTime Set ::: {ObjFixedTimeScale}");
        }
    }

    private static float s_uiTimeScale = 1;
    public static float UIDeltaTime
    {
        get
        {
            return Time.deltaTime * s_uiTimeScale;
        }
        set
        {
            if (value <= 0)
            {
                s_uiTimeScale = 0f;
                DebugUtil.Log("Can't set time to lower than 0");
            }

            s_uiTimeScale = value;
            DebugUtil.Log($"UIDeltaTime Set ::: {s_uiTimeScale}");
        }
    }

    private static double _lastGetTime = double.MinValue;

    public static double GetUtcTimeSeconds()
    {
        double now = (DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
        //if (now < _lastGetTime)
        //{
        //    return _lastGetTime; // 역행 방지
        //}

        //_lastGetTime = now; // 갱신
        return now;
    }


    public static DateTime GetUtcDateTime()
    {
        DateTime now = DateTime.UtcNow;
        return now;
    }

    /// <summary>
    /// date 만큼의 다음날의 00시를 반환
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime GetNextDay(int date)
    {
        DateTime now = GetUtcDateTime();
        DateTime today = now.Date; // 오늘 00시 00분 00초

        // day일 후의 00시를 계산
        DateTime targetDate = today.AddDays(date);

        return targetDate;
    }

    /// <summary>
    /// DateTime의 최대값 (9999년 12월 31일 23:59:59)
    /// </summary>
    /// <returns></returns>
    public static DateTime GetMaxDateTime()
    {
        return DateTime.MaxValue;
    }

    /// <summary>
    /// DateTime의 최솟값
    /// </summary>
    /// <returns></returns>
    public static DateTime GetMinDateTime()
    {
        return DateTime.MinValue;
    }

    /// <summary>
    /// 특정 날짜가 오늘 자정 기준으로 이전인지 확인
    /// </summary>
    /// <param name="targetDate"></param>
    /// <returns></returns>
    public static bool IsPastDay(DateTime targetDate)
    {
        DateTime todayMidnight = GetUtcDateTime().Date;
        return targetDate.Date < todayMidnight;
    }

    /// <summary>
    /// 특정 날짜가 오늘 자정 기준으로 이후인지 확인
    /// </summary>
    /// <param name="targetDate"></param>
    /// <returns></returns>
    public static bool IsFutureDay(DateTime targetDate)
    {
        DateTime todayMidnight = GetUtcDateTime().Date;
        return targetDate.Date > todayMidnight;
    }
}
