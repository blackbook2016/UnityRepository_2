// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Utils
{
    public static class Interpolation
    {
        /// <summary>
        /// cubic interpolation
        /// </summary>
        public static Vector3 Cubic(Vector3 y0, Vector3 y1, Vector3 y2, Vector3 y3, float t)
        {
            var mu2 = t * t;
            var a0 = y3 - y2 - y0 + y1;
            var a1 = y0 - y1 - a0;
            var a2 = y2 - y0;
            var a3 = y1;

            return (a0 * t * mu2 + a1 * mu2 + a2 * t + a3);
        }

        /// <summary>
        /// catmull-rom interpolation
        /// </summary>
        public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            var t2 = t * t;
            var t3 = t2 * t;

            return p0 * (-0.5f * t3 + t2 - 0.5f * t) +
                    p1 * (1.5f * t3 + -2.5f * t2 + 1.0f) +
                    p2 * (-1.5f * t3 + 2.0f * t2 + 0.5f * t) +
                    p3 * (0.5f * t3 - 0.5f * t2);
        }

        public static float EaseInOutCubic(float t, float b, float c, float d)
        {
	        t /= d/2;
	        if (t < 1) return c/2*t*t*t + b;
	        t -= 2;
	        return c/2*(t*t*t + 2) + b;
        }

        public static float InterpolateTowards(float pPrev, float pNext, float pSpeed, float pDt)
        {
	        var	diff = pNext - pPrev;
	        var	maxDiff	= pSpeed * pDt;
	        return pPrev + diff >= 0.0f ? Mathf.Min(diff, maxDiff) : Mathf.Max(diff, -maxDiff);
        }

        /// <summary>
        /// x		(basic linear)
        /// </summary>
        public static float Lerp(float a, float b, float t)
        {
            return a * (1 - t) + b * t;
        }

        /// <summary>
        /// 3x^2 - 2x^3		(S-function) 
        /// </summary>
        public static float LerpS(float a, float b, float t)
        {
            var t2 = t * t;
            var ts = 3.0f * t2 - 2.0f * t2 * t;
            return a * (1 - ts) + b * ts;
        }

        /// <summary>
        /// 3x^2 - 2x^3		(S-function) 
        /// </summary>
        public static Vector2 LerpS(Vector2 a, Vector2 b, float t)
        {
            var t2 = t * t;
            var ts = 3.0f * t2 - 2.0f * t2 * t;
            return a * (1 - ts) + b * ts;
        }

        /// <summary>
        /// 3x^2 - 2x^3		(S-function) 
        /// </summary>
        public static Vector3 LerpS(Vector3 a, Vector3 b, float t)
        {
            var t2 = t * t;
            var ts = 3.0f * t2 - 2.0f * t2 * t;
            return a * (1 - ts) + b * ts;
        }

        /// <summary>
        /// x + x^2 - x^3		(end-S function)
        /// </summary>
        public static float LerpS2(float a, float b, float t)
        {
            var t2 = t * t;
            var ts = t + t2 - t2 * t;
            return a * (1 - ts) + b * ts;
        }

        /// <summary>
        /// x + x^2 - x^3		(end-S function)
        /// </summary>
        public static Vector3 LerpS2(Vector3 a, Vector3 b, float t)
        {
            var t2 = t * t;
            var ts = t + t2 - t2 * t;
            return a * (1 - ts) + b * ts;
        }

        /// <summary>
        /// x + x^2 - x^3		(end-S function)
        /// </summary>
        public static Vector2 LerpS2(Vector2 a, Vector2 b, float t)
        {
            var t2 = t * t;
            var ts = t + t2 - t2 * t;
            return a * (1 - ts) + b * ts;
        }

        /// <summary>
        /// 1 - x - x^2 + x^3	(start-S function)
        /// </summary>
        public static float LerpS3(float a, float b, float t)
        {
            var t2 = t * t;
            var ts = 1.0f - t - t2 + t2 * t;
            return a * (1 - ts) + b * ts;
        }

        /// <summary>
        /// 1 - x - x^2 + x^3	(start-S function)
        /// </summary>
        public static Vector2 LerpS3(Vector2 a, Vector2 b, float t)
        {
            var t2 = t * t;
            var ts = 1.0f - t - t2 + t2 * t;
            return a * (1 - ts) + b * ts;
        }

        /// <summary>
        /// 1 - x - x^2 + x^3	(start-S function)
        /// </summary>
        public static Vector3 LerpS3(Vector3 a, Vector3 b, float t)
        {
            var t2 = t * t;
            var ts = 1.0f - t - t2 + t2 * t;
            return a * (1 - ts) + b * ts;
        }

        /// <summary>
        /// y = e^(n * ln(x)) ... y = x^n 
        /// </summary>
        public static float LerpExpN(float a, float b, float t, float n)
        {
            var ty = Mathf.Exp(n * Mathf.Log(t));
            return a * (1 - ty) + b * ty;
        }
    }
}
