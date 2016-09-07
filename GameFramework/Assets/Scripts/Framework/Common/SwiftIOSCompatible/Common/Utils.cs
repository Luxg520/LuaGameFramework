using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Swift
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Utils
    {
        // 当前本地时间（毫秒）
        public static long Now
        {
            get
            {
                return DateTime.Now.Ticks / 10000;
            }
        }

        // 当前本地日期
        public static DateTime NowDate
        {
            get
            {
                return DateTime.Now;
            }
        }

        // 当前本地时间（毫秒）
        public static long NowSecond
        {
            get
            {
                return DateTime.Now.Ticks / 10000000;
            }
        }

        // 判断2个日期间隔几天，同一天返回0
        // OnHours：以几点为界
        // 注意：可能返回负数，如果想得到正数，dt2要比dt1大！
        public static int DaysBetweenTwoDateTime(DateTime dt1, DateTime dt2, int OnHours)
        {
            dt1 = dt1.AddHours(24 - OnHours);
            dt2 = dt2.AddHours(24 - OnHours);

            DateTime DT1 = new DateTime(dt1.Year, dt1.Month, dt1.Day);
            DateTime DT2 = new DateTime(dt2.Year, dt2.Month, dt2.Day);

            TimeSpan span = DT2.Subtract(DT1);
            return span.Days;
        }

        public static bool IsDifferentDay_0Oclock(DateTime dt1, DateTime dt2)
        {
            return IsDifferentDay_OnClock(dt1, dt2, 0);
        }

        // 是否跨天，以早上5点为界
        public static bool IsDifferentDay_5OClock(DateTime dt1, DateTime dt2)
        {
            return IsDifferentDay_OnClock(dt1, dt2, 5);
        }

        // 是否跨周
        public static bool IsDifferentWeek(DateTime dt1, DateTime dt2, int OnHours)
        {
            int days = DaysBetweenTwoDateTime(dt1, dt2, OnHours);
            dt1.AddHours(-OnHours);
            for (int i = 0; i < days; i++)
			{
                if (dt1.DayOfWeek == DayOfWeek.Sunday)
                {
                    return true;
                }
                dt1 = dt1.AddDays(1);
			}

            return false;
        }

        // 是否跨天，以一天中的指定时间为准（24 小时制度）
        public static bool IsDifferentDay_OnClock(DateTime dt1, DateTime dt2, int OnHours)
        {
            DateTime DT1 = dt1.AddHours(24 - OnHours);
            DateTime DT2 = dt2.AddHours(24 - OnHours);

            return (DT1.Day != DT2.Day || DT1.Month != DT2.Month || DT1.Year != DT2.Year);
        }

        // 是否跨天，以一天中的指定时间为准（24 小时制度）
        public static bool IsDifferentDay_OnClock(DateTime dt1, DateTime dt2, int OnHours, int OnMin)
        {
            DateTime DT1 = dt1.AddMinutes(60 - OnMin);
            DateTime DT2 = dt1.AddMinutes(60 - OnMin);
            DT1 = dt1.AddHours(24 - OnHours - 1);
            DT2 = dt2.AddHours(24 - OnHours - 1);

            return (DT1.Day != DT2.Day || DT1.Month != DT2.Month || DT1.Year != DT2.Year);
        }

        // 判断两个日期是否在同一周
        public static bool IsInSameWeek(DateTime dtmS, DateTime dtmE)
        {
            TimeSpan ts = dtmE - dtmS;
            double dbl = ts.TotalDays;
            int intDow = Convert.ToInt32(dtmE.DayOfWeek);
            if (intDow == 0) intDow = 7;
            if (dbl >= 7 || dbl >= intDow) return false;
            else return true;
        } 

        // 计算一个 hash 字符串
        public static string MD5(string src)
        {
            byte[] data = Encoding.UTF8.GetBytes(src);
            byte[] code = p.ComputeHash(data);
            string r = "";
            foreach (byte b in code)
                r += b;

            return r;
        }
        // 计算一个 hash 字符串
        // 返回值是32位字符串，每个字符是一个16进制数
        public static string MD5Hex(string src)
        {
            byte[] data = Encoding.UTF8.GetBytes(src);
            byte[] code = p.ComputeHash(data);

            StringBuilder sb = new StringBuilder();
            foreach (var c in code)
                sb.Append(c.ToString("x2"));

            return sb.ToString();
        }

        // 获取一个不重复的随机字符串
        public static string GetRandomString(string prefix)
        {
            if (prefix != null)
                return prefix + r.Next(0, int.MaxValue) + seq++;
            else
                return r.Next(0, int.MaxValue).ToString() + seq++;
        }

        // 获取一个随机数，默认区间
        public static int RandomNext()
        {
            return r.Next();
        }

        // 获取一个随机数，区间为 [min, max)
        public static int RandomNext(int min, int max)
        {
            return r.Next(min, max);
        }
        public static int Random0Or1()
        {
            return RandomNext(0, 2);
        }

        // r 是[0,100]的一个数，表示一个概率
        // 随机一下看是否命中
        public static bool Hit100Rate(int r)
        {
            if (r == 0) return false;
            if (r == 100) return true;
            return RandomNext(1, 101) <= r;
        }

        // r 是[0, 10000]的一个数，表示一个概率
        // 随机一下看是否命中
        public static bool Hit10000Rate(int r)
        {
            if (r == 0) return false;
            if (r == 10000) return true;
            return RandomNext(1, 10001) <= r;
        }

        // 获取 n 个不重复的随机数，区间为 [min, max)
        public static int[] RandomNextN(int min, int max, int n)
        {
            List<int> lst = new List<int>();
            int cnt = max - min;

            if (cnt < n)
                throw new Exception("the range is not big enough");

            while (lst.Count < n && n <= cnt)
            {
                int num = r.Next(min, max);
                if (lst.IndexOf(num) < 0)
                    lst.Add(num);
                else
                {
                    // 如果遇到重复的，就取下一个数
                    for (int i = 0; i < cnt && lst.IndexOf(num) >= 0; i++)
                    {
                        num++;
                        if (num >= max)
                            num = min;
                    }

                    lst.Add(num);
                }
            }
            return lst.ToArray();
        }

		// 随机一个 [min, max) 的浮点数，最小粒度为 0.0001
        public static float RandomNext(float min, float max)
        {
            return r.Next((int)(min * 10000), (int)(max * 10000)) * 0.0001f;
        }

        // 计算两个集合是否有任何重复的元素
        public static bool AnyDuplicated<T>(IEnumerable<T> s1, IEnumerable<T> s2)
        {
            foreach (T e1 in s1)
            {
                foreach (T e2 in s2)
                {
                    if (e1.Equals(e2))
                        return true;
                }
            }

            return false;
        }
        
        // 在给定列表中随机选取一个满足条件的对象
        public static T RandomSelect<T>(IEnumerable<T> list, Func<T, bool> f)
        {
            List<T> candidates = new List<T>();
            foreach (T obj in list)
            {
                if (f == null || f(obj))
                    candidates.Add(obj);
            }

            int cnt = candidates.Count;
            if (cnt == 0)
                return default(T);
            else
                return candidates[r.Next(0, cnt)];
        }

        // 对指定概率做一次随机，判断是否击中概率，目标概率应该位于 [0, 1]
        public static bool HitChance(float ratio)
        {
            if (ratio <= 0)
                return false;
            else if (ratio >= 1)
                return true;
            else
                return r.NextDouble() <= ratio;
        }

        // RLE 压缩
        public static void RLECode<T>(List<T> buff, List<int> counter, T[] data) where T : struct
        {
            if (data.Length == 0)
                return;

            if (buff.Count == 0)
            {
                buff.Add(data[0]);
                counter.Add(0);
            }

            T last = buff[buff.Count - 1];

            foreach (T cur in data)
            {
                if (!last.Equals(cur))
                {
                    buff.Add(cur);
                    counter.Add(1);
                    last = cur;
                }
                else
                    counter[counter.Count - 1]++;
            }

            Debug.Assert(buff.Count == counter.Count);
        }

        // RLE 解压
        public static void RLEDecode<T>(T[] buff, int[] counter, List<T> data) where T : struct
        {
            if (buff.Length == 0)
                return;

            for (int i = 0; i < buff.Length; i++)
            {
                T d = buff[i];
                for (int j = 0; j < counter[i]; j++)
                    data.Add(d);
            }

            Debug.Assert(buff.Length == counter.Length);
        }

        // RLE 不解压访问
        public static T RLEVisit<T>(T[] buff, int[] counter, int index) where T : struct
        {
            if (index < 0)
                throw new Exception("index should be positive");

            int i = 0;
            while (counter[i] < index && counter.Count() < i)
            {
                index -= counter[i];
                i++;
            }

            if (i >= counter.Count())
                throw new Exception("index overflow");

            return buff[i];
        }

		// 选取给定数据的最大一个
		public static T Max<T>(params T[] arr)
		{
			T maxOne = arr[0];
			foreach (T d in arr)
			{
				if (Comparer<T>.Default.Compare(d, maxOne) > 0)
					maxOne = d;
			}

			return maxOne;
		}

		// 选取给定数据的最小一个
		public static T Min<T>(params T[] arr)
		{
			T maxOne = arr[0];
			foreach (T d in arr)
			{
				if (Comparer<T>.Default.Compare(d, maxOne) < 0)
					maxOne = d;
			}
			
			return maxOne;
		}

        // 计算名字长度
        // 中文算2个字符
        public static int NameLength(string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;

            int L = 0;
            foreach (var c in str)
            {
                if (c > 127)
                    L += 2;
                else
                    L += 1;
            }
            return L;
        }

        // 名字长度裁剪，只保留指定数量的字符
        // 中文算2个字符
        public static string NameLengthCut(string str, int length)
        {
            if (NameLength(str) <= length)
                return str;
            string s = "";
            int L = 0;
            foreach (var c in str)
            {
                if (c > 127)
                    L += 2;
                else
                    L += 1;

                if (L <= length)
                    s += c;
                else
                    break;
            }
            return s;
        }

        #region 保护部分

        // 递增序号
        static ulong seq = 0;

        // 随机种子
        public static Random r = new Random();

        // md5 加密器
        static MD5CryptoServiceProvider p = new MD5CryptoServiceProvider();

        #endregion
    }

    public static class SwiftSortClassExt
    {
        #region 冒泡排序

        public static T[] SwiftSort<T>(this T[] arr, Func<T, T, int> comp)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    int r = comp(arr[i], arr[j]);
                    if (r > 0)
                    {
                        T t = arr[i];
                        arr[i] = arr[j];
                        arr[j] = t;
                    }
                }
            }

            return arr;
        }

        public static T[] SwiftSort<T>(this T[] arr, Func<T, int> valueFun)
        {
            return arr.SwiftSort((T x, T y) =>
            {
                int vx = valueFun(x);
                int vy = valueFun(y);

                if (vx < vy)
                    return -1;
                else if (vx == vy)
                    return 0;
                else
                    return 1;
            });
        }

        public static List<T> SwiftSort<T>(this List<T> lst, Func<T, T, int> comp)
        {
            T[] arr = lst.ToArray();
            arr.SwiftSort(comp);
            lst.Clear();
            lst.AddRange(arr);
            return lst;
        }

        public static List<T> SwiftSort<T>(this List<T> lst, Func<T, int> valueFun)
        {
            T[] arr = lst.ToArray();
            arr.SwiftSort(valueFun);
            lst.Clear();
            lst.AddRange(arr);
            return lst;
        }

        public static int[] SwiftSort(this int[] arr)
        {
            return arr.SwiftSort((int x, int y) => { if (x < y) return -1; else if (x == y) return 0; else return 1; });
        }

        public static T[] SwiftRandomSort<T>(this T[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                int n = Utils.RandomNext(i, arr.Length);
                T tmp = arr[n];
                arr[n] = arr[i];
                arr[i] = tmp;
            }

            return arr;
        }

        public static List<T> SwiftRandomSort<T>(this List<T> lst)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                int n = Utils.RandomNext(i, lst.Count);
                T tmp = lst[n];
                lst[n] = lst[i];
                lst[i] = tmp;
            }

            return lst;
        }

        public static List<Key> GetSortedKeyList<Key, Value>(this Dictionary<Key, Value> dict, Func<Value, Value, int> sortFun)
        {
            List<Key> ks = new List<Key>();
            foreach (Key k in dict.Keys)
                ks.Add(k);

            ks.Sort((Key a, Key b) =>
            {
                Value va = dict[a];
                Value vb = dict[b];
                return sortFun(va, vb);
            });

            return ks;
        }

        #endregion
    }
}
