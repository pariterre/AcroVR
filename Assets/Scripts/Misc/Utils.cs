using System.Globalization;

public class Utils
{
    static public int ToInt(string _text)
    {
        return int.Parse(_text);
    }

    static public float ToFloat(string _text)
    {
        return float.Parse(_text, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    static public bool ToBool(string _text)
    {
        return bool.Parse(_text);
    }

    static public string ToString(float _value){
        return _value.ToString("0.0", CultureInfo.InvariantCulture);
    }
}
