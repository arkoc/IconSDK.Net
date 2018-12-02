﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IconSDK.Types
{
    public class Bytes
    {
        #region ** static **

        static Dictionary<Type, int> _sizes = new Dictionary<Type, int>
        {
            [typeof(Bytes)] = 0,
            [typeof(ExternalAddress)] = 20,
            [typeof(ContractAddress)] = 20,
            [typeof(Hash32)] = 32,
            [typeof(PrivateKey)] = 32,
            [typeof(PublicKey)] = 65,
            [typeof(Signature)] = 65,
        };

        static Dictionary<Type, string> _prefixes = new Dictionary<Type, string>
        {
            [typeof(Bytes)] = string.Empty,
            [typeof(ExternalAddress)] = "hx",
            [typeof(ContractAddress)] = "cx",
            [typeof(Hash32)] = "0x",
            [typeof(PrivateKey)] = string.Empty,
            [typeof(PublicKey)] = string.Empty,
            [typeof(Signature)] = string.Empty
        };

        #endregion

        public readonly ImmutableArray<byte> Binary;

        public int Size => _sizes[this.GetType()];
        public string Prefix => _prefixes[this.GetType()];

        public Bytes(IEnumerable<byte> bytes)
        {
            if (Size > 0 && Size != bytes.Count())
                throw new Exception($"{Size} != {bytes.Count()}");

            Binary = bytes.ToImmutableArray();
        }

        public Bytes(string hex)
        {
            hex = hex.Replace(Prefix, string.Empty);

            var builder = ImmutableArray.CreateBuilder<byte>(hex.Length / 2);
            for (int i = 0; i < builder.Capacity; ++i)
            {
                byte b = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                builder.Add(b);
            }

            if (Size > 0 && Size != builder.Count)
                throw new Exception();

            Binary = builder.ToImmutable();
        }

        public Bytes(BigInteger value)
        {
            var bytes = value.ToByteArray();

            if (Size > 0 && Size != bytes.Count())
                throw new Exception();

            Binary = bytes.ToImmutableArray();
        }

        public string ToHex()
        {
            StringBuilder hex = new StringBuilder(Binary.Length * 2);
            foreach (byte b in Binary)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public override string ToString()
        {
            return Prefix + ToHex();
        }

        public override int GetHashCode()
        {
            return Binary.Sum(item => item);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            return this == (obj as Bytes);
        }

        public static implicit operator byte[](Bytes bytes)
        {
            return bytes.Binary.ToArray();
        }

        public static implicit operator Bytes(string hex)
        {
            return new Bytes(hex);
        }

        public static bool operator ==(Bytes x, Bytes y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            if (x.Binary.Length != y.Binary.Length)
                return false;

            return Enumerable.SequenceEqual(x.Binary, y.Binary);
        }

        public static bool operator !=(Bytes x, Bytes y)
        {
            return !(x == y);
        }
    }
}
