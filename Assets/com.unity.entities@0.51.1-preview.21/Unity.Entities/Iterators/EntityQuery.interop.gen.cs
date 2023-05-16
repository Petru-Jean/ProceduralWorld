
//------------------------------------------------------------------------------
// <auto-generated>
//     This file was automatically generated by Unity.Entities.Editor.BurstInteropCodeGenerator
//     Any changes you make here will be overwritten
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
//
//     To update this file, use the "DOTS -> Regenerate Burst Interop" menu option.
//
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using Unity.Burst;
using Unity.Collections;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities
{
     unsafe partial struct EntityQueryImpl
    {

#if !UNITY_IOS

        [BurstDiscard]
        private static void CheckDelegate(ref bool useDelegate)
        {
            //@TODO: This should use BurstCompiler.IsEnabled once that is available as an efficient API.
            useDelegate = true;
        }

        private static bool UseDelegate()
        {
            bool result = false;
            CheckDelegate(ref result);
            return result;
        }

        static class Managed
        {
            public static bool _initialized = false;

            public delegate void _dlg_ResetFilter(IntPtr self);
            public static object _gcDefeat_ResetFilter;
            public delegate void _dlg_FreeCachedState(IntPtr self);
            public static object _gcDefeat_FreeCachedState;
        }

        struct TagType_ResetFilter {};
        public static readonly SharedStatic<IntPtr> _bfp_ResetFilter = SharedStatic<IntPtr>.GetOrCreate<TagType_ResetFilter>();
        struct TagType_FreeCachedState {};
        public static readonly SharedStatic<IntPtr> _bfp_FreeCachedState = SharedStatic<IntPtr>.GetOrCreate<TagType_FreeCachedState>();

#endif

        [NotBurstCompatible]
        internal static void Initialize()
        {
#if !UNITY_IOS
            if (Managed._initialized)
                return;
            Managed._initialized = true;
        {
            Managed._dlg_ResetFilter delegateObject = _wrapper_ResetFilter;
            Managed._gcDefeat_ResetFilter = delegateObject;
            _bfp_ResetFilter.Data = Marshal.GetFunctionPointerForDelegate(delegateObject);
        }
        {
            Managed._dlg_FreeCachedState delegateObject = _wrapper_FreeCachedState;
            Managed._gcDefeat_FreeCachedState = delegateObject;
            _bfp_FreeCachedState.Data = Marshal.GetFunctionPointerForDelegate(delegateObject);
        }

#endif
        }


        internal static void ResetFilter (EntityQueryImpl* self)
        {
#if !UNITY_IOS
            if (!UseDelegate())
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if( _bfp_ResetFilter.Data == IntPtr.Zero)
                    throw new InvalidOperationException("Burst Interop Classes must be initialized manually");
#endif

                var fp = new FunctionPointer<Managed._dlg_ResetFilter>(_bfp_ResetFilter.Data);
                fp.Invoke((IntPtr) self);
                return;
            }
#endif
            _ResetFilter(self);
        }

#if !UNITY_IOS
        [MonoPInvokeCallback(typeof(Managed._dlg_ResetFilter))]
#endif
        private static void _wrapper_ResetFilter (IntPtr self)
        {
            _ResetFilter((EntityQueryImpl*)self);
        }
        internal static void FreeCachedState (EntityQueryImpl* self)
        {
#if !UNITY_IOS
            if (!UseDelegate())
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if( _bfp_FreeCachedState.Data == IntPtr.Zero)
                    throw new InvalidOperationException("Burst Interop Classes must be initialized manually");
#endif

                var fp = new FunctionPointer<Managed._dlg_FreeCachedState>(_bfp_FreeCachedState.Data);
                fp.Invoke((IntPtr) self);
                return;
            }
#endif
            _FreeCachedState(self);
        }

#if !UNITY_IOS
        [MonoPInvokeCallback(typeof(Managed._dlg_FreeCachedState))]
#endif
        private static void _wrapper_FreeCachedState (IntPtr self)
        {
            _FreeCachedState((EntityQueryImpl*)self);
        }


    }
}
