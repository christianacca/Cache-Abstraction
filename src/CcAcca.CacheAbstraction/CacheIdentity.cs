// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Linq;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Represents the identity of the cache made up of a <see cref="Name"/> and optional <see cref="InstanceName"/>
    /// </summary>
    /// <remarks>
    /// The identity is suitable to be used as a key for a dictionary
    /// </remarks>
    public class CacheIdentity : IEquatable<CacheIdentity>
    {
        public CacheIdentity(string name, string instanceName = null)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");

            Name = name.Trim();
            InstanceName = (instanceName ?? String.Empty).Trim();
        }

        public string Name { get; private set; }

        /// <remarks>
        /// There may be more than one instance that is sharing the same underlying cache, and hence a cache instance
        /// distriminates itself from other caches by it having a separate instance name.
        /// <para>
        /// The most common reason for this is wanting to have multiple cache instances with different policies, all
        /// sharing the same cache store
        /// </para>
        /// </remarks>
        public string InstanceName { get; private set; }

        public bool Equals(CacheIdentity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && string.Equals(InstanceName, other.InstanceName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CacheIdentity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode()*397) ^ InstanceName.GetHashCode();
            }
        }

        public static bool operator ==(CacheIdentity left, CacheIdentity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CacheIdentity left, CacheIdentity right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return InstanceName == String.Empty ? Name : string.Format("{0}.{1}", Name, InstanceName);
        }


        public static implicit operator CacheIdentity(string value)
        {
            return Parse(value);
        }

        public static CacheIdentity Parse(string value)
        {
            if (value == null) throw new ArgumentNullException("value");

            string[] parts = value.Split(new[] {"."}, StringSplitOptions.None);

            if (!(parts.Length <= 2))
            {
                throw new FormatException(string.Format(
                    "Cannot parse CacheIdentity from string supplied; received: {0}", value));
            }

            return new CacheIdentity(parts[0], parts.ElementAtOrDefault(1));
        }

        public static CacheIdentity TryParse(string value)
        {
            if (value == null) return null;

            try
            {
                return Parse(value);
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}