namespace ExpressionsAndIQueryable.Tests
{
    public class Foo
    {
        public int i = 5;
        float f = 5.0f;
        public string s = "str";
    }

    public class Bar
    {
        public int i = 3;
        public string s = "s";
        public double f = 3.0;

        public override string ToString() => $"{{i = {i}, s = {s}, f = {f}}}";
    }

    public class ReadOnlyBar
    {
        public readonly int i = 3;
        public string s = "s";
        public double f = 3.0;

        public override string ToString() => $"{{i = {i}, s = {s}, f = {f}}}";
    }

    public class PropertiesBar
    {
        public int i => 3;
        public string s = "s";
        public double f = 3.0;

        public override string ToString() => $"{{i = {i}, s = {s}, f = {f}}}";
    }

    public class PropertiesFoo
    {
        public int i { get; set; } = 5;
        private float f { get; set; } = 5.0f;
        public string s { get; set; } = "str";
    }
}
