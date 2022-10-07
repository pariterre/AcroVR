using System.Globalization;

public class Utils
{
    static public float ToFloat(string text)
    {
        return float.Parse(text, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    static public bool ToBool(string text)
    {
        return bool.Parse(text);
    }
}
