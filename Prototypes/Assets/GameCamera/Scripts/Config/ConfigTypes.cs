// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Config
{
    public abstract partial class Config
    {
        /// <summary>
        /// parameter type
        /// </summary>
        public enum ConfigValue
        {
            Bool,
            Range,
            Vector3,
            Vector2,
            String,
            Selection,
        }

        public interface Param
        {
            ConfigValue Type { get; }

            object[] Serialize();
            void Deserialize(object[] data);
            void Interpolate(Param p0, Param p1, float t);
            void Set(Param p);
            Param Clone();
        }

        public class RangeParam : Param
        {
            public float value;
            public float min;
            public float max;

            public ConfigValue Type { get { return ConfigValue.Range; } }

            public object[] Serialize()
            {
                var array = new object[4];
                array[0] = ConfigValue.Range.ToString();
                array[1] = value;
                array[2] = min;
                array[3] = max;
                return array;
            }

            public void Deserialize(object[] data)
            {
                value = Convert.ToSingle(data[1]);
                min = Convert.ToSingle(data[2]);
                max = Convert.ToSingle(data[3]);
            }

            public void Interpolate(Param p0, Param p1, float t)
            {
                var range0 = (RangeParam) p0;
                var range1 = (RangeParam) p1;

                value = Interpolation.LerpS2(range0.value, range1.value, t);
                min = Interpolation.LerpS2(range0.min, range1.min, t);
                max = Interpolation.LerpS2(range0.max, range1.max, t);
            }

            public void Set(Param p)
            {
                var r = (RangeParam) p;
                value = r.value;
                min = r.min;
                max = r.max;
            }

            public Param Clone()
            {
                var r = new RangeParam();
                r.Set(this);
                return r;
            }
        }

        public struct Vector3Param : Param
        {
            public Vector3 value;

            public ConfigValue Type { get { return ConfigValue.Vector3; } }

            public object[] Serialize()
            {
                var array = new object[4];
                array[0] = ConfigValue.Vector3.ToString();
                array[1] = value.x;
                array[2] = value.y;
                array[3] = value.z;
                return array;
            }

            public void Deserialize(object[] data)
            {
                value.x = Convert.ToSingle(data[1]);
                value.y = Convert.ToSingle(data[2]);
                value.z = Convert.ToSingle(data[3]);
            }

            public void Interpolate(Param p0, Param p1, float t)
            {
                var range0 = (Vector3Param)p0;
                var range1 = (Vector3Param)p1;

                value = Interpolation.LerpS2(range0.value, range1.value, t);
            }

            public void Set(Param p)
            {
                var r = (Vector3Param) p;
                value = r.value;
            }

            public Param Clone()
            {
                var r = new Vector3Param();
                r.Set(this);
                return r;
            }
        }

        public struct Vector2Param : Param
        {
            public Vector2 value;

            public ConfigValue Type { get { return ConfigValue.Vector2; } }

            public object[] Serialize()
            {
                var array = new object[3];
                array[0] = ConfigValue.Vector2.ToString();
                array[1] = value.x;
                array[2] = value.y;
                return array;
            }

            public void Deserialize(object[] data)
            {
                value.x = Convert.ToSingle(data[1]);
                value.y = Convert.ToSingle(data[2]);
            }

            public void Interpolate(Param p0, Param p1, float t)
            {
                var range0 = (Vector2Param)p0;
                var range1 = (Vector2Param)p1;

                value = Interpolation.LerpS2(range0.value, range1.value, t);
            }

            public void Set(Param p)
            {
                var r = (Vector2Param)p;
                value = r.value;
            }

            public Param Clone()
            {
                var r = new Vector2Param();
                r.Set(this);
                return r;
            }
        }

        public struct StringParam : Param
        {
            public string value;

            public ConfigValue Type { get { return ConfigValue.String; } }

            public object[] Serialize()
            {
                var array = new object[2];
                array[0] = ConfigValue.String.ToString();
                array[1] = value;
                return array;
            }

            public void Deserialize(object[] data)
            {
                value = Convert.ToString(data[1]);
            }

            public void Interpolate(Param p0, Param p1, float t)
            {
                var range1 = (StringParam)p1;
                value = range1.value;
            }

            public void Set(Param p)
            {
                var r = (StringParam)p;
                value = r.value;
            }

            public Param Clone()
            {
                var r = new StringParam();
                r.Set(this);
                return r;
            }
        }

        public struct BoolParam : Param
        {
            public bool value;

            public ConfigValue Type { get { return ConfigValue.Bool; } }

            public object[] Serialize()
            {
                var array = new object[2];
                array[0] = ConfigValue.Bool.ToString();
                array[1] = value;
                return array;
            }

            public void Deserialize(object[] data)
            {
                value = Convert.ToBoolean(data[1]);
            }

            public void Interpolate(Param p0, Param p1, float t)
            {
                var range1 = (BoolParam)p1;
                value = range1.value;
            }

            public void Set(Param p)
            {
                var r = (BoolParam)p;
                value = r.value;
            }

            public Param Clone()
            {
                var r = new BoolParam();
                r.Set(this);
                return r;
            }
        }

        public struct SelectionParam : Param
        {
            public int index;
            public string[] value;

            public ConfigValue Type { get { return ConfigValue.Selection; } }

            public object[] Serialize()
            {
                var array = new object[value.Length + 2];
                array[0] = ConfigValue.Selection.ToString();
                array[1] = index;
                value.CopyTo(array, 2);
                return array;
            }

            public int Find(string val)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == val)
                    {
                        return i;
                    }
                }
                return -1;
            }

            public void Deserialize(object [] data)
            {
                index = Convert.ToInt32(data[1]);
                value = new string[data.Length-2];
                for (int i = 2; i < data.Length; i++)
                {
                    value[i - 2] = Convert.ToString(data[i]);
                }
            }

            public void Interpolate(Param p0, Param p1, float t)
            {
                var range1 = (SelectionParam)p1;
                index = range1.index;
            }

            public void Set(Param p)
            {
                var r = (SelectionParam)p;
                index = r.index;
            }

            public Param Clone()
            {
                var r = new SelectionParam();
                r.Set(this);
                return r;
            }
        }
    }
}
