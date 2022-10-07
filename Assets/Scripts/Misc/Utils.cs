using System.Globalization;

public class Utils
{
    static public int ToInt(string text)
    {
        return int.Parse(text);
    }

    static public float ToFloat(string text)
    {
        return float.Parse(text, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    static public bool ToBool(string text)
    {
        return bool.Parse(text);
    }
}
