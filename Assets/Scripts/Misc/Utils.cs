using System;
using System.Linq;
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

    public class WrongArraySizeException: Exception {
        public WrongArraySizeException(string message): base(message) {}
    }

    static public T[,] To2D<T>(T[] _toExpand, int _nCols){
        if (_toExpand.Length % _nCols != 0) 
            throw new WrongArraySizeException("The length of array should be a multiple of number of columns");
        int _nRows = _toExpand.Length / _nCols;
        T[,] _out = new T[_nRows, _nCols];
        for (int i=0; i< _toExpand.Length; ++i){
            _out[i % _nRows, i / _nRows] = _toExpand[i];
        }
        return _out;
    }


    static public T[] Flat<T>(T[,] _toFlatten) => _toFlatten.Cast<T>().ToArray();
}
