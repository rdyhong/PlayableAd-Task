using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = System.Random;

public static class GameUtil
{
    /// <summary>
    /// Get random Success in 0 to 1
    /// </summary>
    /// <param name="percentage"></param>
    /// <returns></returns>
    public static bool RandomSuccess(float percentage)
    {
        float randomPoint = UnityEngine.Random.value * 100;
        return randomPoint < percentage;
    }
    public static int Random(float[] param)
    {
        float total = 0;

        foreach (float f in param)
        {
            total += f;
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < param.Length; i++)
        {
            if (randomPoint < param[i])
            {
                return i;
            }
            else
            {
                randomPoint -= param[i];
            }
        }
        return param.Length - 1;
    }

    public static bool CalculatePercentage(float val)
    {
        var result = false;

        val = Mathf.Max(0, val);

        val = (float)Math.Round((decimal)val, 1);
        var normalizeVal = Mathf.Clamp01(val);

        result = normalizeVal >= UnityEngine.Random.value;

        return result;
    }

    public static double Clamp(double targetValue, double min, double max)
    {
        double ret = targetValue;

        if (ret > max)
        {
            ret = max;
        }
        if (ret < min)
        {
            ret = min;
        }
        return ret;
    }

    public static double Lerp(double startValue, double endValue, double time)
    {
        return (startValue + ((double)(endValue - startValue) * time));
    }

    public static Color Color256(float r, float g, float b)
    {
        return new Color(r / 255f, g / 255f, b / 255f, 1);
    }

    public static Color Color256(float r, float g, float b, float a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static Color Color1(float r, float g, float b)
    {
        return new Color(r * 255f, g * 255f, b * 255f, 1);
    }

    public static Color Color1(float r, float g, float b, float a)
    {
        return new Color(r * 255f, g * 255f, b * 255f, a * 255f);
    }

    public static Color Fade(float alpha)
    {
        return new Color(255f, 255f, 255f, alpha * 255f);
    }

    public static float calculateAngle(Vector3 startDir, Vector3 targetDir, int direction)
    {
        float angle = 0;
        Vector2 dist = targetDir - startDir;

        Vector2 v2 = (targetDir - startDir);
        v2.x *= direction;
        angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

        return angle;
    }

    public static float calculateAngle(Vector2 direction, ref float prevTargetAngle, ref float addtionalAngle)
    {
        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 360) % 360;
        if (angle != 0)
        {
            if (prevTargetAngle - angle > 180)
                addtionalAngle += 360;
            else if (prevTargetAngle - angle < -180)
                addtionalAngle -= 360;

            prevTargetAngle = angle;

            return addtionalAngle + angle - 90;
        }
        else
        {
            return 0;
        }
    }

    public static void changeSpriteColor(SpriteRenderer targetSpriteRenderer, Color targetColor)
    {
        targetSpriteRenderer.color = targetColor;
    }

    public static void changeSpritesColor(SpriteRenderer[] targetSpriteRenderers, Color targetColor)
    {
        for (int i = 0; i < targetSpriteRenderers.Length; i++)
        {
            if (targetSpriteRenderers[i] != null)
            {
                targetSpriteRenderers[i].color = targetColor;
            }
        }
    }

    public static Transform getNearTransform(Vector2 basePosition, params Transform[] targets)
    {
        Transform nearTf = null;
        float dis = float.MaxValue;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                float tempDis = Vector2.Distance(basePosition, targets[i].position);
                if (tempDis <= dis)
                {
                    dis = tempDis;
                    nearTf = targets[i];
                }
            }
        }

        return nearTf;
    }

    public static bool CheckInternetConnected()
    {
        bool _isConnect = false;

        if (Application.internetReachability == NetworkReachability.NotReachable) 
        {
            DebugUtil.LogErr("Internet Disconnected");
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            _isConnect = true;
        }
        else
        {
            _isConnect = true;
        }

        return _isConnect;
    }

    public static void waitFrame(int frame, Action endAction)
    {
        GameMgr.Inst.StartCoroutine(waitUpdate(frame, endAction));
    }

    private static IEnumerator waitUpdate(int frame, Action endAction)
    {
        for (int i = 0; i < frame; i++)
            yield return null;

        if (endAction != null)
            endAction();
    }

    public static void WaitForSeconds(float time, Action endAction)
    {
        GameMgr.Inst.StartCoroutine(WaitForSecondsCo(time, endAction));
    }
    public static Coroutine WaitForSeconds_cor(float time, Action endAction)
    {
        return GameMgr.Inst.StartCoroutine(WaitForSecondsCo(time, endAction));
    }
    private static IEnumerator WaitForSecondsCo(float time, Action endAction)
    {
        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        if (endAction != null)
            endAction();
    }

    public static void WaitUntil(Func<bool> isUntil, Action endAction)
    {
        GameMgr.Inst.StartCoroutine(waitUntilUpdate(isUntil, endAction));
    }

    private static IEnumerator waitUntilUpdate(Func<bool> isUntil, Action endAction)
    {
        yield return new WaitUntil(isUntil);

        if (endAction != null)
            endAction();
    }

    public static void waitWhile(Func<bool> isWhile, Action endAction)
    {
        GameMgr.Inst.StartCoroutine(waitWhileUpdate(isWhile, endAction));
    }

    private static IEnumerator waitWhileUpdate(Func<bool> isWhile, Action endAction)
    {
        yield return new WaitWhile(isWhile);

        if (endAction != null)
            endAction();
    }

    public static bool ProbabilityCalculate(int probability, int min = 0, int max = 100)
    {
        if (probability == 0)
            return false;

        float offset = 100;
        if (probability > (int)(UnityEngine.Random.Range(min, max * offset) / offset))
            return true;
        else
            return false;
    }

    public static bool CheckFolder(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);

        return di.Exists;
    }

    public static T StringToEnum<T>(string target)
    {
        try
        {
            return (T)Enum.Parse(typeof(T), target);
        }
        catch
        {
            return default(T);
        }
    }

    public static void WidthMatchToParent(RectTransform target, RectTransform refTarget)
    {
        target.sizeDelta = new Vector2(refTarget.rect.width, target.sizeDelta.y);
    }
}
