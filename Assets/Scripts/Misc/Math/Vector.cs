using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcroVrVector : AcroVrMatrix
{

    public AcroVrVector(AcroVrVector _other)
        : base(_other.Length, 1)
    {
        for (int i = 0; i < _other.Length; i++)
        {
            Value[i, 0] = _other.Value[i, 0];
        }
    }

    public AcroVrVector(double[] _other)
        : base(_other.Length, 1)
    {
        for (int i = 0; i < _other.Length; i++)
        {
            Value[i, 0] = _other[i];
        }
    }

    public virtual double Get(int row)
    {
        return Value[row, 0];
    }
    public new AcroVrVector Get(int _row, int _nbRows)
    {
        AcroVrVector _result = new AcroVrVector(_nbRows);
        for (int i = 0; i < _nbRows; i++)
        {
            _result.Value[i, 0] = Value[_row + i, 0];
        }
        return _result;
    }
    public virtual void Set(int row, double val)
    {
        Value[row, 0] = val;
    }
    public new void Set(int _row, int _nbRows, double _val)
    {
        for (int i = _row; i < _row + _nbRows; i++)
        {
            Value[i, 0] = _val;
        }
    }
    public void Set(int _row, int _nbRows, AcroVrVector _other)
    {
        for (int i = _row; i < _row + _nbRows; i++)
        {
            Value[i, 0] = _other.Value[i - _row, 0];
        }
    }

    static public AcroVrVector Zero(int _nbRows)
    {
        return new AcroVrVector(_nbRows);
    }

    public int Length { get { return NbRows; } }

    public AcroVrVector(int _nbElements) : base(_nbElements, 1)
    {

    }
    public new double[] ToDouble()
    {
        double[] _result = new double[NbRows];
        for (int i = 0; i < NbRows; i++)
        {
            _result[i] = Value[i, 0];
        }
        return _result;
    }
    public new float[] ToFloat()
    {
        float[] _result = new float[NbRows];
        for (int i = 0; i < NbRows; i++)
        {
            _result[i] = (float)Value[i, 0];
        }
        return _result;
    }

    static public AcroVrVector operator +(AcroVrVector _first, AcroVrVector _second)
    {
        AcroVrMatrix _result = new AcroVrVector(_first.NbRows);
        Add(_first, _second, ref _result);
        return (AcroVrVector)_result;
    }

    static public AcroVrVector operator *(AcroVrMatrix _first, AcroVrVector _second)
    {
        AcroVrMatrix _result = new AcroVrVector(_first.NbRows);
        Multiply(_first, _second, ref _result);
        return (AcroVrVector)_result;
    }
    static public AcroVrVector operator *(double _scalar, AcroVrVector _old)
    {
        AcroVrMatrix _result = new AcroVrVector(_old.NbRows);
        _old.Multiply(_scalar, ref _result);
        return (AcroVrVector)_result;
    }
    static public AcroVrVector operator *(AcroVrVector _old, double _scalar)
    {
        return _scalar * _old;
    }
}
