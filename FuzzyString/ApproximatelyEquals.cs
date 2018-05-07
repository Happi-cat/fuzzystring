using System;
using System.Collections.Generic;
using System.Linq;

namespace FuzzyString
{
    public static partial class ComparisonMetrics
    {
        public static ApproximateComparisonResult ApproximatelyEquals(this string source, string target,
            FuzzyStringComparisonOptions options)
        {
            if (!options.HasFlag(FuzzyStringComparisonOptions.CaseSensitive))
            {
                source = source.ToLower();
                target = target.ToLower();
            }

            var results = GetComparisonResults().ToArray();

            return results.Length == 0
                ? new ApproximateComparisonResult(source, target, 1)
                : new ApproximateComparisonResult(source, target, results.Average());

            IEnumerable<float> GetComparisonResults()
            {
                // Min: 0    Max: source.Length = target.Length
                if (options.HasFlag(FuzzyStringComparisonOptions.UseHammingDistance) && source.Length == target.Length)
                {
                    yield return source.HammingDistance(target) / target.Length;
                }

                // Min: 0    Max: 1
                if (options.HasFlag(FuzzyStringComparisonOptions.UseJaccardDistance))
                {
                    yield return source.JaccardDistance(target);
                }

                // Min: 0    Max: 1
                if (options.HasFlag(FuzzyStringComparisonOptions.UseJaroDistance))
                {
                    yield return source.JaroDistance(target);
                }

                // Min: 0    Max: 1
                if (options.HasFlag(FuzzyStringComparisonOptions.UseJaroWinklerDistance))
                {
                    yield return source.JaroWinklerDistance(target);
                }

                // Min: 0    Max: LevenshteinDistanceUpperBounds - LevenshteinDistanceLowerBounds
                // Min: LevenshteinDistanceLowerBounds    Max: LevenshteinDistanceUpperBounds
                if (options.HasFlag(FuzzyStringComparisonOptions.UseNormalizedLevenshteinDistance))
                {
                    yield return Convert.ToSingle(source.NormalizedLevenshteinDistance(target)) /
                        Convert.ToSingle(Math.Max(source.Length, target.Length) -
                            source.LevenshteinDistanceLowerBounds(target));
                }
                else if (options.HasFlag(FuzzyStringComparisonOptions.UseLevenshteinDistance))
                {
                    yield return Convert.ToSingle(source.LevenshteinDistance(target)) /
                        Convert.ToSingle(source.LevenshteinDistanceUpperBounds(target));
                }

                if (options.HasFlag(FuzzyStringComparisonOptions.UseLongestCommonSubsequence))
                {
                    yield return 1 - Convert.ToSingle(source.LongestCommonSubsequence(target).Length /
                        Convert.ToDouble(Math.Min(source.Length, target.Length)));
                }

                if (options.HasFlag(FuzzyStringComparisonOptions.UseLongestCommonSubstring))
                {
                    yield return 1 - Convert.ToSingle(source.LongestCommonSubstring(target).Length /
                        Convert.ToSingle(Math.Min(source.Length, target.Length)));
                }

                // Min: 0    Max: 1
                if (options.HasFlag(FuzzyStringComparisonOptions.UseSorensenDiceDistance))
                {
                    yield return source.SorensenDiceDistance(target);
                }

                // Min: 0    Max: 1
                if (options.HasFlag(FuzzyStringComparisonOptions.UseOverlapCoefficient))
                {
                    yield return 1 - source.OverlapCoefficient(target);
                }

                // Min: 0    Max: 1
                if (options.HasFlag(FuzzyStringComparisonOptions.UseRatcliffObershelpSimilarity))
                {
                    yield return 1 - source.RatcliffObershelpSimilarity(target);
                }
            }
        }

        public static bool ApproximatelyEquals(this string source, string target,
            FuzzyStringComparisonTolerance tolerance,
            FuzzyStringComparisonOptions options)
        {
            var result = source.ApproximatelyEquals(target, options);

            switch (tolerance)
            {
                case FuzzyStringComparisonTolerance.Strong:
                    return result.Tolerance < 0.25;
                case FuzzyStringComparisonTolerance.Normal:
                    return result.Tolerance < 0.5;
                case FuzzyStringComparisonTolerance.Weak:
                    return result.Tolerance < 0.75;
                case FuzzyStringComparisonTolerance.Manual:
                    return result.Tolerance > 0.6;
                default:
                    return false;
            }
        }
    }
}