////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;

namespace FlaxEditor.Utilities
{
    /// <summary>
    ///     This structure represents the comparison of one member of an object to the corresponding member of another object.
    /// </summary>
    [Serializable]
    public struct MemberComparison
    {
        /// <summary>
        ///     Members path this Comparison compares.
        /// </summary>
        public MemberInfoPath MemberPath;

        /// <summary>
        ///     The value of first object respective member
        /// </summary>
        public readonly object Value1;

        /// <summary>
        ///     The value of second object respective member
        /// </summary>
        public readonly object Value2;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberComparison"/> struct.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        public MemberComparison(MemberInfo member, object value1, object value2)
        {
            MemberPath = new MemberInfoPath(member);
            Value1 = value1;
            Value2 = value2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberComparison"/> struct.
        /// </summary>
        /// <param name="memberPath">The member path.</param>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        public MemberComparison(MemberInfoPath memberPath, object value1, object value2)
        {
            MemberPath = memberPath;
            Value1 = value1;
            Value2 = value2;
        }

        /// <summary>
        /// Sets the member value. Handles field or property type.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        public void SetMemberValue(object instance, object value)
        {
            var finalMember = MemberPath.GetLastMember(ref instance);

            if (finalMember is FieldInfo field)
            {
                if (value != null)
                {
                    if (value is long && field.FieldType == typeof(int))
                    {
                        value = (int)(long)value;
                    }
                    else if (value is double && field.FieldType == typeof(float))
                    {
                        value = (float)(double)value;
                    }
                }

                field.SetValue(instance, value);
            }
            else
            {
                var property = (PropertyInfo)finalMember;

                if (value != null)
                {
                    if (value is long && property.PropertyType == typeof(int))
                    {
                        value = (int)(long)value;
                    }
                    else if (value is double && property.PropertyType == typeof(float))
                    {
                        value = (float)(double)value;
                    }
                }

                if (property.SetMethod != null)
                    property.SetValue(instance, value);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return MemberPath.Path + ": " + Value1 + (Value1.Equals(Value2) ? " == " : " != ") + Value2;
        }
    }
}
