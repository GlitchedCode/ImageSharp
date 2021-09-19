// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif
{
    internal sealed class ExifLong8Array : ExifArrayValue<ulong>
    {
        public ExifLong8Array(ExifTag<ulong[]> tag)
            : base(tag)
        {
        }

        public ExifLong8Array(ExifTagValue tag)
            : base(tag)
        {
        }

        private ExifLong8Array(ExifLong8Array value)
            : base(value)
        {
        }

        public override ExifDataType DataType
        {
            get
            {
                if (this.Value is null)
                {
                    return ExifDataType.Long;
                }

                for (int i = 0; i < this.Value.Length; i++)
                {
                    if (this.Value[i] > uint.MaxValue)
                    {
                        return ExifDataType.Long8;
                    }
                }

                return ExifDataType.Long;
            }
        }

        public override bool TrySetValue(object value)
        {
            if (base.TrySetValue(value))
            {
                return true;
            }

            switch (value)
            {
                case int val:
                    return this.SetSingle((ulong)Numerics.Clamp(val, 0, int.MaxValue));

                case uint val:
                    return this.SetSingle((ulong)val);

                case short val:
                    return this.SetSingle((ulong)Numerics.Clamp(val, 0, short.MaxValue));

                case ushort val:
                    return this.SetSingle((ulong)val);

                case long[] array:
                {
                    if (value.GetType().Equals(typeof(ulong[])))
                    {
                        return this.SetArray((ulong[])value);
                    }

                    return this.SetArray(array);
                }

                case int[] array:
                {
                    if (value.GetType().Equals(typeof(uint[])))
                    {
                        return this.SetArray((uint[])value);
                    }

                    return this.SetArray(array);
                }

                case short[] array:
                {
                    if (value.GetType().Equals(typeof(ushort[])))
                    {
                        return this.SetArray((ushort[])value);
                    }

                    return this.SetArray(array);
                }
            }

            return false;
        }

        public override IExifValue DeepClone() => new ExifLong8Array(this);

        private bool SetSingle(ulong value)
        {
            this.Value = new[] { value };
            return true;
        }

        private bool SetArray(long[] values)
        {
            var numbers = new ulong[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                numbers[i] = (ulong)values[i];
            }

            this.Value = numbers;
            return true;
        }

        private bool SetArray(ulong[] values)
        {
            this.Value = values;
            return true;
        }

        private bool SetArray(int[] values)
        {
            var numbers = new ulong[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                numbers[i] = (ulong)values[i];
            }

            this.Value = numbers;
            return true;
        }

        private bool SetArray(uint[] values)
        {
            var numbers = new ulong[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                numbers[i] = (ulong)values[i];
            }

            this.Value = numbers;
            return true;
        }

        private bool SetArray(short[] values)
        {
            var numbers = new ulong[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                numbers[i] = (ulong)values[i];
            }

            this.Value = numbers;
            return true;
        }

        private bool SetArray(ushort[] values)
        {
            var numbers = new ulong[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                numbers[i] = (ulong)values[i];
            }

            this.Value = numbers;
            return true;
        }
    }
}
