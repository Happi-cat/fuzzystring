namespace FuzzyString
{
    public struct ApproximateComparisonResult
    {
        public ApproximateComparisonResult(string source, string target, float tolerance)
        {
            Source = source;
            Target = target;
            Tolerance = tolerance;
        }

        public string Source { get; private set; }
        public string Target { get; private set; }
        public float Tolerance { get; private set; }
    }
}