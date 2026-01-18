using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleTest
{
    public class StringSimilarity
    {

        /// <summary>
        /// 计算Levenshtein编辑距离
        /// </summary>
        private static int LevenshteinDistance(string str1, string str2)
        {
            int n = str1.Length;
            int m = str2.Length;
            int[,] dp = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++)
                dp[i, 0] = i;
            for (int j = 0; j <= m; j++)
                dp[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = str1[i - 1] == str2[j - 1] ? 0 : 1;
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost
                    );
                }
            }

            return dp[n, m];
        }
        /// <summary>
        /// 计算两个字符串之间的相似度百分比（基于Levenshtein距离）
        /// </summary>
        /// <param name="str1">第一个字符串</param>
        /// <param name="str2">第二个字符串</param>
        /// <returns>相似度百分比 (0-100)</returns>
        public static double LevenshteinSimilarity(string str1, string str2)
        {
            // 处理空值情况
            if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2))
                return 100.0;
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
                return 0.0;

            // 使用Levenshtein距离算法
            int distance = LevenshteinDistance(str1, str2);
            int maxLength = Math.Max(str1.Length, str2.Length);

            // 计算相似度百分比
            return (1.0 - (double)distance / maxLength) * 100.0;
        }

        /// <summary>
        /// 判断两个字符串是否相似（基于阈值）
        /// </summary>
        /// <param name="str1">第一个字符串</param>
        /// <param name="str2">第二个字符串</param>
        /// <param name="threshold">相似度阈值（0-100）</param>
        /// <returns>是否相似</returns>
        public static bool AreStringsSimilar(string str1, string str2, double threshold = 80.0)
        {
            double similarity = LevenshteinSimilarity(str1, str2);
            return similarity >= threshold;
        }
        /// <summary>
        /// 修复OCR多次识别结果不一致的问题
        /// </summary>
        /// <param name="ocrResults">多次OCR识别结果列表</param>
        /// <returns>修复后的稳定结果</returns>
        public static string FixInconsistentResults(List<string> ocrResults)
        {
            if (ocrResults == null || ocrResults.Count == 0)
                return string.Empty;

            if (ocrResults.Count == 1)
                return ocrResults[0];

            // 1. 基于字符频率分析的稳定结果
            var stableResult = GetStableResultByFrequency(ocrResults);

            // 2. 基于相似度的优化
            var optimizedResult = OptimizeBySimilarity(ocrResults, stableResult);

            return optimizedResult;
        }

        /// <summary>
        /// 通过字符频率分析获取稳定结果
        /// </summary>
        private static string GetStableResultByFrequency(List<string> ocrResults)
        {
            if (ocrResults == null || ocrResults.Count == 0)
                return string.Empty;

            // 统计每个位置上各字符出现的频率
            var maxLen = ocrResults.Max(r => r.Length);
            var charFrequency = new List<Dictionary<char, int>>();

            // 初始化字符频率统计
            for (int i = 0; i < maxLen; i++)
            {
                charFrequency.Add(new Dictionary<char, int>());
            }

            // 统计每个位置的字符频率
            foreach (var result in ocrResults)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    if (i < charFrequency.Count)
                    {
                        var ch = result[i];
                        if (charFrequency[i].ContainsKey(ch))
                            charFrequency[i][ch]++;
                        else
                            charFrequency[i][ch] = 1;
                    }
                }
            }

            // 选择每个位置出现频率最高的字符
            var sb = new StringBuilder();
            for (int i = 0; i < charFrequency.Count; i++)
            {
                if (charFrequency[i].Count > 0)
                {
                    var maxChar = charFrequency[i].OrderByDescending(kvp => kvp.Value).First().Key;
                    sb.Append(maxChar);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 基于相似度优化结果
        /// </summary>
        private static string OptimizeBySimilarity(List<string> ocrResults, string baseResult)
        {
            if (ocrResults == null || ocrResults.Count == 0 || string.IsNullOrEmpty(baseResult))
                return baseResult;

            // 计算每个结果与基础结果的相似度
            var similarities = new List<double>();
            foreach (var result in ocrResults)
            {
                var similarity = LevenshteinSimilarity(baseResult, result);
                similarities.Add(similarity);
            }

            // 如果相似度都很高，返回基础结果
            if (similarities.All(s => s > 0.9))
            {
                return baseResult;
            }

            // 否则返回最相似的结果
            var maxSimilarity = similarities.Max();
            var bestIndex = similarities.IndexOf(maxSimilarity);
            return ocrResults[bestIndex];
        }

        /// <summary>
        /// 预处理OCR结果，去除常见错误
        /// </summary>
        public static string PreprocessOCRResult(string ocrResult)
        {
            if (string.IsNullOrEmpty(ocrResult))
                return ocrResult;

            // 去除常见的OCR错误字符
            var processed = ocrResult
                .Replace(" ", "")  // 去除空格
                .Replace("。", ".") // 中文句号转英文
                .Replace("，", ",") // 中文逗号转英文
                .Replace("！", "!") // 中文感叹号转英文
                .Replace("？", "?"); // 中文问号转英文

            return processed;
        }
    }
}
