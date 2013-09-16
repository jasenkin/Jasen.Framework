using System; 
using System.Text;

namespace Jasen.Framework
{
    /// <summary>
    /// 字符串扩展类。
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 截取长度为length的字符串。（其中length是按字节计算的）
        /// </summary>
        /// <param name="inputValue">输入字符串。</param>
        /// <param name="length">截取长度(按字节计算，英文为一个字节，中文为2个字节)。</param>
        /// <returns>返回截取后的字符串。</returns>
        public static string Truncate(this string inputValue, int length)
        {
            if (string.IsNullOrWhiteSpace(inputValue) || length <= 0)
            {
                return string.Empty;
            }

            inputValue = inputValue.Trim();
            int totalLength = Encoding.Default.GetBytes(inputValue).Length;


            if (length >= totalLength)
            {
                return inputValue;
            }

            // 计算长度
            int index = 0;

            while (index < length)
            {
                //每遇到一个中文，则将目标长度减一。
                if (inputValue[index] > 128)
                {
                    length--;

                    if (index == length)
                    {
                        break;
                    }
                }

                index++;
            }

            return inputValue.Substring(0, index) + "...";
        }

        public static decimal? AsNullableDecimal(this string inputValue)
        {
            if(string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsDecimal();
        }

        public static decimal AsDecimal(this string inputValue)
        {
            decimal result;
            if (!decimal.TryParse(inputValue, out result))
            {
                result = 0;
            }
            return result;
        }

        public static DateTime? AsNullableDateTime(this string inputValue)
        {
            if(string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsDateTime();
        }

        public static DateTime AsDateTime(this string inputValue)
        {
            DateTime result;

            if (!DateTime.TryParse(inputValue, out result))
            {
                result = DateTime.MinValue;
            }
            return result;
        }

        /// <summary>
        /// 转换成字符串类型。如果转换失败返回空，否则返回指定格式的值。
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string AsDateTimeString(this string inputValue, string format)
        {
            DateTime result;

            if (!DateTime.TryParse(inputValue, out result))
            {
                return string.Empty;
            }

            return result.ToString(format);
        }

        public static float? AsNullableFloat(this string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsFloat();
        }

        public static float AsFloat(this string inputValue)
        {
            float result;
            if (!float.TryParse(inputValue, out result))
            {
                result = 0;
            }
            return result;
        }

        public static double? AsNullableDouble(this string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsDouble();
        }

        public static double AsDouble(this object inputValue)
        {
            string inputValueString = (inputValue ?? string.Empty).ToString();

            double result;
            if (!double.TryParse(inputValueString, out result))
            {
                result = 0;
            }
            return result;
        }

    
        public static double AsDouble(this string inputValue)
        {
            double result;
            if (!double.TryParse(inputValue, out result))
            {
                result = 0;
            }
            return result;
        }

        public static int? AsNullableInt(this string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsInt();
        }

        public static int AsInt(this object inputValue)
        {
            string inputValueString = (inputValue ?? string.Empty).ToString();

            int result;
            if (!int.TryParse(inputValueString, out result))
            {
                result = 0;
            }
            return result;
        }

        public static int AsInt(this string inputValue)
        {
            int result;
            if (!int.TryParse(inputValue, out result))
            {
                result = 0;
            }
            return result;
        }

        public static byte? AsNullableByte(this string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsByte();
        }

        public static byte AsByte(this string inputValue)
        {
            byte result;
            if (!byte.TryParse(inputValue, out result))
            {
                result = 0;
            }
            return result;
        }

        public static sbyte? AsNullableSbyte(this string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsSbyte();
        }

        public static sbyte AsSbyte(this string inputValue)
        {
            sbyte result;
            if (!sbyte.TryParse(inputValue, out result))
            {
                result = 0;
            }
            return result;
        }

        public static short? AsNullableShort(this string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsShort();
        }

        public static short AsShort(this string inputValue)
        {
            short result;
            if (!short.TryParse(inputValue, out result))
            {
                result = 0;
            }
            return result;
        }

        public static ushort? AsNullableUshort(this string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsUshort();
        }

        public static ushort AsUshort(this string inputValue)
        {
            ushort result;
            if (!ushort.TryParse(inputValue, out result))
            {
                result = 0;
            }
            return result;
        }

        public static uint? AsNullableUint(this string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsUint();
        }

        public static uint AsUint(this string inputValue)
        {
            uint result;
            if (!uint.TryParse(inputValue, out result))
            {
                result = 0;
            }
            return result;
        }

        public static long? AsNullableLong(this string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsLong();
        }

        public static long AsLong(this string inputValue)
        {
            long result;
            if (!long.TryParse(inputValue, out result))
            {
                result = 0;
            }
            return result;
        }

        public static ulong? AsNullableUlong(this string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsUlong();
        }

        public static ulong AsUlong(this string inputValue)
        {
            ulong result;
            if (!ulong.TryParse(inputValue, out result))
            {
                result = 0;
            }
            return result;
        }

        public static char? AsNullableChar(this string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsChar();
        }

        public static char AsChar(this string inputValue)
        {
            char result;
            if (!char.TryParse(inputValue, out result))
            {
                result = '\0';
            }
            return result;
        }

        public static bool? AsNullableBool(this string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return null;
            }

            return inputValue.AsBool();
        }

        public static bool AsBool(this string inputValue)
        {
            bool result;
            if (!bool.TryParse(inputValue, out result))
            {
                result = false;
            }
            return result;
        }

        public static string AsString(this object inputValue)
        {
            if (inputValue == null)
            {
                return string.Empty;
            }

            return inputValue.ToString().Trim();
        }
 
        public static bool AsBool(this object inputValue)
        {
            if(inputValue==null)
            {
                return false;
            }

            bool result;
            if (!bool.TryParse(inputValue.ToString(), out result))
            {
                result = false;
            }
            return result;
        }
    }
}
