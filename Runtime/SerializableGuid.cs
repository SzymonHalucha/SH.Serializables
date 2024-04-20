using System;
using UnityEngine;

namespace SH.Serializables
{
    [Serializable]
    public sealed class SerializableGuid : IComparable, IComparable<Guid>, IEquatable<Guid>, IComparable<SerializableGuid>, IEquatable<SerializableGuid>, ISerializationCallbackReceiver
    {
        public Guid Guid { get; private set; } = Guid.Empty;

        [SerializeField] private byte[] _guid = null;

        public SerializableGuid(Guid guid)
        {
            Guid = guid;
            _guid = Guid.ToByteArray();
        }

        public int CompareTo(Guid other)
        {
            return other.CompareTo(Guid);
        }

        public int CompareTo(SerializableGuid other)
        {
            return other.Guid.CompareTo(Guid);
        }

        public int CompareTo(object obj)
        {
            if (obj is Guid guid)
            {
                return guid.CompareTo(Guid);
            }
            return -1;
        }

        public bool Equals(Guid other)
        {
            return other.Equals(Guid);
        }

        public bool Equals(SerializableGuid other)
        {
            return Guid.Equals(other.Guid);
        }

        public override bool Equals(object obj)
        {
            return (obj is SerializableGuid serializable && Equals(serializable))
                || (obj is Guid guid && Equals(guid));
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public override string ToString()
        {
            return Guid.ToString();
        }

        public static implicit operator Guid(SerializableGuid guid)
        {
            return guid.Guid;
        }

        public static implicit operator SerializableGuid(Guid guid)
        {
            return new SerializableGuid(guid);
        }

        public void OnAfterDeserialize()
        {
            if (_guid == null || (sizeof(byte) * _guid.Length) != 16)
            {
                if (Guid == null || Guid == Guid.Empty)
                {
                    Guid = Guid.NewGuid();
                }
                _guid = Guid.ToByteArray();
            }
            Guid = new Guid(_guid);
        }

        public void OnBeforeSerialize()
        {
            if (Guid == null || Guid == Guid.Empty)
            {
                Guid = Guid.NewGuid();
            }
            _guid = Guid.ToByteArray();
        }
    }
}