﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

namespace ScpXInputBridge
{
    using DWORD = UInt32;

    public partial class XInputDll
    {
        #region Native structs

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_GAMEPAD
        {
            public ushort wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_STATE
        {
            public DWORD dwPacketNumber;
            public XINPUT_GAMEPAD Gamepad;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_VIBRATION
        {
            public DWORD wLeftMotorSpeed;
            public DWORD wRightMotorSpeed;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCP_EXTN
        {
            public  float SCP_UP;
            public  float SCP_RIGHT;
            public  float SCP_DOWN;
            public  float SCP_LEFT;
            
            public  float SCP_LX;
            public  float SCP_LY;
            
            public  float SCP_L1;
            public  float SCP_L2;
            public  float SCP_L3;
            
            public  float SCP_RX;
            public  float SCP_RY;
            
            public  float SCP_R1;
            public  float SCP_R2;
            public  float SCP_R3;
            
            public  float SCP_T;
            public  float SCP_C;
            public  float SCP_X;
            public  float SCP_S;
            
            public  float SCP_SELECT;
            public  float SCP_START;
            
            public  float SCP_PS;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_CAPABILITIES
        {
            public byte Type;
            public byte SubType;
            public ushort Flags;
            public XINPUT_GAMEPAD Gamepad;
            public XINPUT_VIBRATION Vibration;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_BATTERY_INFORMATION
        {
            public byte BatteryType;
            public byte BatteryLevel;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_KEYSTROKE
        {
            public ushort VirtualKey;
            public char Unicode;
            public ushort Flags;
            public byte UserIndex;
            public byte HidCode;
        }

        #endregion

        private static readonly IList<Delegate> _xInputFunctions = new List<Delegate>();
        private static IntPtr _dll = IntPtr.Zero;
        private static volatile bool _isInitialized;

        private static Delegate GetMethod<T>(IntPtr module, string methodName)
        {
            return Marshal.GetDelegateForFunctionPointer(Kernel32Natives.GetProcAddress(module, methodName), typeof(T));
        }

        #region Delegates for GetProcAddress

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void XInputEnableFunction(bool enable);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate DWORD XInputGetStateFunction(DWORD dwUserIndex, ref XINPUT_STATE pState);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate DWORD XInputSetStateFunction(DWORD dwUserIndex, ref XINPUT_VIBRATION pVibration);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate DWORD XInputGetCapabilitiesFunction(DWORD dwUserIndex, DWORD dwFlags,
            ref XINPUT_CAPABILITIES pCapabilities);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate DWORD XInputGetDSoundAudioDeviceGuidsFunction(DWORD dwUserIndex, ref Guid pDSoundRenderGuid,
            ref Guid pDSoundCaptureGuid);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate DWORD XInputGetBatteryInformationFunction(DWORD dwUserIndex, byte devType,
            ref XINPUT_BATTERY_INFORMATION pBatteryInformation);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate DWORD XInputGetKeystrokeFunction(
            DWORD dwUserIndex, DWORD dwReserved, ref XINPUT_KEYSTROKE pKeystroke);

        #endregion

        private static void Initialize()
        {
            if (_isInitialized)
                return;

            _dll = Kernel32Natives.LoadLibrary(@"C:\WINDOWS\system32\xinput1_3.dll");

            _xInputFunctions.Add(GetMethod<XInputEnableFunction>(_dll, "XInputEnable"));
            _xInputFunctions.Add(GetMethod<XInputGetStateFunction>(_dll, "XInputGetState"));
            _xInputFunctions.Add(GetMethod<XInputSetStateFunction>(_dll, "XInputSetState"));
            _xInputFunctions.Add(GetMethod<XInputGetCapabilitiesFunction>(_dll, "XInputGetCapabilities"));
            _xInputFunctions.Add(GetMethod<XInputGetDSoundAudioDeviceGuidsFunction>(_dll, "XInputGetDSoundAudioDeviceGuids"));
            _xInputFunctions.Add(GetMethod<XInputGetBatteryInformationFunction>(_dll, "XInputGetBatteryInformation"));
            _xInputFunctions.Add(GetMethod<XInputGetKeystrokeFunction>(_dll, "XInputGetKeystroke"));

            _isInitialized = true;
        }

        public void Dispose()
        {
            if (_dll == IntPtr.Zero) return;

            Kernel32Natives.FreeLibrary(_dll);
            _isInitialized = false;
        }

        #region SCP extension function

        [DllExport("XInputGetExtended", CallingConvention.StdCall)]
        public static DWORD XInputGetExtended(DWORD dwUserIndex, ref SCP_EXTN pPressure)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}