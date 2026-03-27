using System.Numerics;
namespace cardgames.core
{
    public readonly struct Money
                : IComparable<Money>, IEquatable<Money>, IFormattable
    {
        private readonly decimal _value;

        // Use primary constructor to fix IDE0290 and correct constructor name to match struct name for CS1520
        public Money(decimal value)
        {
            _value = value;
        }

        public decimal Value => _value;

        public static Money operator +(Money left, Money right) => new(left._value + right._value);
        public static Money operator -(Money left, Money right) => new(left._value - right._value);

        public static Money operator *(Money left, decimal right) => new(left._value * right);
        public static Money operator *(Money left, Money right) => new(left._value * right._value);
        public static Money operator *(decimal left, Money right) => new(left * right._value);

        public static Money operator /(Money left, decimal right)
        {
            if (right == 0) throw new DivideByZeroException();
            return new(left._value / right);
        }

        public static decimal operator /(Money numerator, Money denominator)
        {
            if (denominator._value == 0) throw new DivideByZeroException();
            return numerator._value / denominator._value;
        }

        public static bool operator ==(Money a, Money b) => a._value == b._value;
        public static bool operator !=(Money a, Money b) => a._value != b._value;

        public static implicit operator Money(int value) => new Money(value);
        public static implicit operator Money(decimal value) => new Money(value);

        public static explicit operator decimal(Money money) => money._value;
        public static explicit operator int(Money money) => (int)money._value;
        public static explicit operator float(Money money) => (float)money._value;
        public static explicit operator double(Money money) => (double)money._value;

        public static bool operator >(Money a, Money b) => a._value > b._value;
        public static bool operator <(Money a, Money b) => a._value < b._value;
        public static bool operator >=(Money a, Money b) => a._value >= b._value;
        public static bool operator <=(Money a, Money b) => a._value <= b._value;


        public int CompareTo(Money other) => _value.CompareTo(other._value);
        public bool Equals(Money other) => _value == other._value;
        public override bool Equals(object obj) => obj is Money other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => decimal.Round(_value, decimals:2).ToString("F2");
        public string ToString(string format, IFormatProvider formatProvider) => decimal.Round(_value, decimals:2).ToString(format, formatProvider);
    }
}
