using System;

public class AcroVrMatrixRotation : AcroVrMatrix
{
    public delegate AcroVrMatrixRotation FromEuler(double _x, double _y, double _z);
    public delegate AcroVrVector3 ToEuler(AcroVrMatrixRotation _rot);

    public AcroVrMatrixRotation() : base(AcroVrMatrixRotation.Identity())
    {

    }

    public AcroVrMatrixRotation(
        double r0c0, double r0c1, double r0c2,
        double r1c0, double r1c1, double r1c2,
        double r2c0, double r2c1, double r2c2
    ) : base(3, 3)
    {
        Value[0, 0] = r0c0;
        Value[1, 0] = r1c0;
        Value[2, 0] = r2c0;
        Value[0, 1] = r0c1;
        Value[1, 1] = r1c1;
        Value[2, 1] = r2c1;
        Value[0, 2] = r0c2;
        Value[1, 2] = r1c2;
        Value[2, 2] = r2c2;
    }

    public AcroVrMatrixRotation(AcroVrMatrixRotation _other) : base(_other)
    {

    }

    static public AcroVrMatrixRotation Identity()
    {
        return new AcroVrMatrixRotation(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1
        );
    }

    static protected AcroVrMatrixRotation BaseRotationX(double x)
    {
        return new AcroVrMatrixRotation(
            1, 0, 0,
            0, Math.Cos(x), -Math.Sin(x),
            0, Math.Sin(x), Math.Cos(x)
        );
    }
    static protected AcroVrMatrixRotation BaseRotationY(double y)
    {
        return new AcroVrMatrixRotation(
            Math.Cos(y), 0, Math.Sin(y),
            0, 1, 0,
            -Math.Sin(y), 0, Math.Cos(y)
        );
    }
    static protected AcroVrMatrixRotation BaseRotationZ(double z)
    {
        return new AcroVrMatrixRotation(
            Math.Cos(z), -Math.Sin(z), 0,
            Math.Sin(z), Math.Cos(z), 0,
            0, 0, 1
        );
    }
    static public AcroVrMatrixRotation operator *(AcroVrMatrixRotation _first, AcroVrMatrixRotation _second)
    {
        return _first.Multiply(_second);
    }

    public AcroVrMatrixRotation Multiply(AcroVrMatrixRotation _other)
    {
        AcroVrMatrix _result = new AcroVrMatrixRotation();
        Multiply(this, _other, ref _result);
        return (AcroVrMatrixRotation)_result;
    }
    static public AcroVrMatrixRotation operator *(AcroVrMatrixRotation _matrix, double _scalar)
    {
        AcroVrMatrix _result = new AcroVrMatrixRotation();
        _matrix.Multiply(_scalar, ref _result);
        return (AcroVrMatrixRotation)_result;
    }
    static public AcroVrMatrixRotation operator *(double _scalar, AcroVrMatrixRotation _matrix)
    {
        AcroVrMatrix _result = new AcroVrMatrixRotation();
        _matrix.Multiply(_scalar, ref _result);
        return (AcroVrMatrixRotation)_result;
    }
    static public AcroVrMatrixRotation operator *(AcroVrMatrixRotation _matrix, int _scalar)
    {
        AcroVrMatrix _result = new AcroVrMatrixRotation();
        _matrix.Multiply(_scalar, ref _result);
        return (AcroVrMatrixRotation)_result;
    }
    static public AcroVrMatrixRotation operator *(int _scalar, AcroVrMatrixRotation _matrix)
    {
        AcroVrMatrix _result = new AcroVrMatrixRotation();
        _matrix.Multiply(_scalar, ref _result);
        return (AcroVrMatrixRotation)_result;
    }
    static public AcroVrVector3 operator *(AcroVrMatrixRotation _matrix, AcroVrVector3 _vector)
    {
        AcroVrMatrix _result = new AcroVrVector3();
        Multiply(_matrix, _vector, ref _result);
        return (AcroVrVector3)_result;
    }

    public new AcroVrMatrixRotation Transpose()
    {
        AcroVrMatrix _result = new AcroVrMatrixRotation();
        Transpose(ref _result);
        return (AcroVrMatrixRotation)_result;
    }

    static public AcroVrVector3 ChangeSequence(double[] _xyz, FromEuler _from, ToEuler _to)
    {
        return ChangeSequence(_xyz[0], _xyz[1], _xyz[2], _from, _to);
    }
    static public AcroVrVector3 ChangeSequence(AcroVrVector3 _xyz, FromEuler _from, ToEuler _to)
    {
        return ChangeSequence(_xyz.Get(0), _xyz.Get(1), _xyz.Get(2), _from, _to);
    }
    static public AcroVrVector3 ChangeSequence(double _x, double _y, double _z, FromEuler _from, ToEuler _to)
    {
        var _asVector = _from(_x, _y, _z);
        return _to(_asVector);
    }

    static public AcroVrMatrixRotation FromEulerXYZ(double[] _xyz)
    {
        return FromEulerXYZ(_xyz[0], _xyz[1], _xyz[2]);
    }
    static public AcroVrMatrixRotation FromEulerXYZ(AcroVrVector3 _xyz)
    {
        return FromEulerXYZ(_xyz.Get(0), _xyz.Get(1), _xyz.Get(2));
    }
    static public AcroVrMatrixRotation FromEulerXYZ(double x, double y, double z)
    {

        AcroVrMatrixRotation BaseX = BaseRotationX(x);
        AcroVrMatrixRotation BaseY = BaseRotationY(y);
        AcroVrMatrixRotation BaseZ = BaseRotationZ(z);
        return BaseX * BaseY * BaseZ;
    }

    static public AcroVrMatrixRotation FromEulerXZY(double[] _xyz)
    {
        return FromEulerXYZ(_xyz[0], _xyz[1], _xyz[2]);
    }
    static public AcroVrMatrixRotation FromEulerXZY(AcroVrVector3 _xyz)
    {
        return FromEulerXYZ(_xyz.Get(0), _xyz.Get(1), _xyz.Get(2));
    }
    static public AcroVrMatrixRotation FromEulerXZY(double x, double y, double z)
    {

        AcroVrMatrixRotation BaseX = BaseRotationX(x);
        AcroVrMatrixRotation BaseY = BaseRotationY(y);
        AcroVrMatrixRotation BaseZ = BaseRotationZ(z);
        return BaseX * BaseZ * BaseY;
    }

    static public AcroVrMatrixRotation FromEulerYXZ(double[] _xyz)
    {
        return FromEulerXYZ(_xyz[0], _xyz[1], _xyz[2]);
    }
    static public AcroVrMatrixRotation FromEulerYXZ(AcroVrVector3 _xyz)
    {
        return FromEulerXYZ(_xyz.Get(0), _xyz.Get(1), _xyz.Get(2));
    }
    static public AcroVrMatrixRotation FromEulerYXZ(double x, double y, double z)
    {

        AcroVrMatrixRotation BaseX = BaseRotationX(x);
        AcroVrMatrixRotation BaseY = BaseRotationY(y);
        AcroVrMatrixRotation BaseZ = BaseRotationZ(z);
        return BaseY * BaseX * BaseZ;
    }

    static public AcroVrMatrixRotation FromEulerYZX(double[] _xyz)
    {
        return FromEulerXYZ(_xyz[0], _xyz[1], _xyz[2]);
    }
    static public AcroVrMatrixRotation FromEulerYZX(AcroVrVector3 _xyz)
    {
        return FromEulerXYZ(_xyz.Get(0), _xyz.Get(1), _xyz.Get(2));
    }
    static public AcroVrMatrixRotation FromEulerYZX(double x, double y, double z)
    {

        AcroVrMatrixRotation BaseX = BaseRotationX(x);
        AcroVrMatrixRotation BaseY = BaseRotationY(y);
        AcroVrMatrixRotation BaseZ = BaseRotationZ(z);
        return BaseY * BaseZ * BaseX;
    }

    static public AcroVrMatrixRotation FromEulerZXY(double[] _xyz)
    {
        return FromEulerXYZ(_xyz[0], _xyz[1], _xyz[2]);
    }
    static public AcroVrMatrixRotation FromEulerZXY(AcroVrVector3 _xyz)
    {
        return FromEulerXYZ(_xyz.Get(0), _xyz.Get(1), _xyz.Get(2));
    }
    static public AcroVrMatrixRotation FromEulerZXY(double x, double y, double z)
    {

        AcroVrMatrixRotation BaseX = BaseRotationX(x);
        AcroVrMatrixRotation BaseY = BaseRotationY(y);
        AcroVrMatrixRotation BaseZ = BaseRotationZ(z);
        return BaseZ * BaseX * BaseY;
    }

    static public AcroVrMatrixRotation FromEulerZYX(double[] _xyz)
    {
        return FromEulerXYZ(_xyz[0], _xyz[1], _xyz[2]);
    }
    static public AcroVrMatrixRotation FromEulerZYX(AcroVrVector3 _xyz)
    {
        return FromEulerXYZ(_xyz.Get(0), _xyz.Get(1), _xyz.Get(2));
    }
    static public AcroVrMatrixRotation FromEulerZYX(double x, double y, double z)
    {

        AcroVrMatrixRotation BaseX = BaseRotationX(x);
        AcroVrMatrixRotation BaseY = BaseRotationY(y);
        AcroVrMatrixRotation BaseZ = BaseRotationZ(z);
        return BaseZ * BaseY * BaseX;
    }

    static public AcroVrVector3 ToEulerXYZ(AcroVrMatrixRotation _mat)
    {
        return new AcroVrVector3(
            Math.Atan2(-_mat.Value[1, 2], _mat.Value[2, 2]),
            Math.Asin(_mat.Value[0, 2]),
            Math.Atan2(-_mat.Value[0, 1], _mat.Value[0, 0])
        );
    }
    public AcroVrVector3 ToEulerXYZ()
    {
        return ToEulerXYZ(this);
    }
    static public AcroVrVector3 ToEulerXZY(AcroVrMatrixRotation _mat)
    {
        return new AcroVrVector3(
            Math.Atan2(_mat.Value[2, 1], _mat.Value[1, 1]),
            Math.Atan2(_mat.Value[0, 2], _mat.Value[0, 0]),
            Math.Asin(-_mat.Value[0, 1])
        );
    }
    public AcroVrVector3 ToEulerXZY()
    {
        return ToEulerXZY(this);
    }
    static public AcroVrVector3 ToEulerYXZ(AcroVrMatrixRotation _mat)
    {
        return new AcroVrVector3(
            Math.Asin(-_mat.Value[1, 2]),
            Math.Atan2(_mat.Value[0, 2], _mat.Value[2, 2]),
            Math.Atan2(_mat.Value[1, 0], _mat.Value[1, 1])
        );
    }
    public AcroVrVector3 ToEulerYXZ()
    {
        return ToEulerYXZ(this);
    }
    static public AcroVrVector3 ToEulerYZX(AcroVrMatrixRotation _mat)
    {
        return new AcroVrVector3(
            Math.Atan2(-_mat.Value[1, 2], _mat.Value[1, 1]),
            Math.Atan2(-_mat.Value[2, 0], _mat.Value[0, 0]),
            Math.Asin(_mat.Value[1, 0])
        );
    }
    public AcroVrVector3 ToEulerYZX()
    {
        return ToEulerYZX(this);
    }
    static public AcroVrVector3 ToEulerZXY(AcroVrMatrixRotation _mat)
    {
        return new AcroVrVector3(
            Math.Asin(_mat.Value[2, 1]),
            Math.Atan2(-_mat.Value[2, 0], _mat.Value[2, 2]),
            Math.Atan2(-_mat.Value[0, 1], _mat.Value[1, 1])
        );
    }
    public AcroVrVector3 ToEulerZXY()
    {
        return ToEulerZXY(this);
    }
    static public AcroVrVector3 ToEulerZYX(AcroVrMatrixRotation _mat)
    {
        return new AcroVrVector3(
            Math.Atan2(_mat.Value[2, 1], _mat.Value[2, 2]),
            Math.Asin(-_mat.Value[2, 0]),
            Math.Atan2(_mat.Value[1, 0], _mat.Value[0, 0])
        );
    }
    public AcroVrVector3 ToEulerZYX()
    {
        return ToEulerZYX(this);
    }
}
