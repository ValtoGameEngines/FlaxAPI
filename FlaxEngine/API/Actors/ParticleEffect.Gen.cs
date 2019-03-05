// Copyright (c) 2012-2019 Wojciech Figat. All rights reserved.
// This code was generated by a tool. Changes to this file may cause
// incorrect behavior and will be lost if the code is regenerated.

using System;
using System.Runtime.CompilerServices;

namespace FlaxEngine
{
    /// <summary>
    /// The particle system instance that plays the particles simulation in the game.
    /// </summary>
    [Serializable]
    public sealed partial class ParticleEffect : Actor
    {
        /// <summary>
        /// Creates new <see cref="ParticleEffect"/> object.
        /// </summary>
        private ParticleEffect() : base()
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="ParticleEffect"/> object.
        /// </summary>
        /// <returns>Created object.</returns>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static ParticleEffect New()
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            return Internal_Create(typeof(ParticleEffect)) as ParticleEffect;
#endif
        }

        /// <summary>
        /// Gets or sets the particle system to play.
        /// </summary>
        [UnmanagedCall]
        [EditorDisplay("Particle Effect"), EditorOrder(0), Tooltip("The particle system to play.")]
        public ParticleSystem ParticleSystem
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetParticleSystem(unmanagedPtr); }
            set { Internal_SetParticleSystem(unmanagedPtr, value); }
#endif
        }

        #region Internal Calls

#if !UNIT_TEST_COMPILANT
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern ParticleSystem Internal_GetParticleSystem(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_SetParticleSystem(IntPtr obj, ParticleSystem val);
#endif

        #endregion
    }
}