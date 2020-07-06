using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using System.Windows.Forms;

namespace StreamLabsSceneSwitcher
{


	public static class KeySender
	{
		//imports mouse_event function from user32.dll
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		//imports keybd_event function from user32.dll
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int GetAsyncKeyState(byte vKey);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern Int16 GetKeyState(byte vKey);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern Boolean BlockInput(Boolean fBlockIt);

		//declare consts for mouse messages
		public const int MOUSEEVENTF_LEFTDOWN = 0x02;
		public const int MOUSEEVENTF_LEFTUP = 0x04;
		public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
		public const int MOUSEEVENTF_RIGHTUP = 0x10;

		//declare consts for key scan codes
		public const byte VK_TAB = 0x09;
		public const byte VK_MENU = 0x12; // VK_MENU is Microsoft talk for the ALT key

		public const byte VK_RCONTROL = 0xA3;

		public const int KEYEVENTF_EXTENDEDKEY = 0x01;
		public const int KEYEVENTF_KEYUP = 0x02;

		//              Make    Break   Make    Break
		// 64	R CTRL	E0 1D	E0 9D	E0 14	E0 F0 14

		/// <summary>
		/// Only the shift modifier is currently supported. 
		/// TODO: Add support for alt/ctrl/windows keys in the future.
		/// </summary>
		/// <param name="keyToPress"></param>
		public static void SendKeyPressToActiveApplication(Keys keyToPress)
		{
			if ((keyToPress & Keys.Shift) == Keys.Shift)
				KeySender.keybd_event((Byte)Keys.ShiftKey, 0, 0, 0);

			KeySender.keybd_event((Byte)keyToPress, 0, 0, 0);
			KeySender.keybd_event((Byte)keyToPress, 0, KeySender.KEYEVENTF_KEYUP, 0);

			if ((keyToPress & Keys.Shift) == Keys.Shift)
				KeySender.keybd_event((Byte)Keys.ShiftKey, 0, KeySender.KEYEVENTF_KEYUP, 0);
		}

		public static void SendKeyDownToActiveApplication(Keys keyToPress)
		{
			KeySender.keybd_event((Byte)keyToPress, 0, 0, 0);
		}

		public static void SendKeyUpToActiveApplication(Keys keyToPress)
		{
			KeySender.keybd_event((Byte)keyToPress, 0, KeySender.KEYEVENTF_KEYUP, 0);
		}


		public const int INPUT_MOUSE = 0;
		public const int INPUT_KEYBOARD = 1;
		public const int INPUT_HARDWARE = 2;

		public const int KEYEVENTF_SCANCODE = 0x0008;

		//public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
		//public const int KEYEVENTF_KEYUP = 0x0002;

		[StructLayout(LayoutKind.Sequential)]
		private struct KEYBOARD_INPUT
		{
			public uint type;
			public ushort vk;
			public ushort scanCode;
			public uint flags;
			public uint time;
			public uint extrainfo;
			public uint padding1;
			public uint padding2;
		}

		[DllImport("User32.dll")]
		private static extern uint SendInput(uint numberOfInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] KEYBOARD_INPUT[] input, int structSize);

		[DllImport("user32.dll")]
		static extern uint MapVirtualKey(uint uCode, uint uMapType);

		private const uint MapType_VirtualKeyToScanCode = 0;
		private const uint MapType_ScanCodeToVirtualKey = 1;
		private const uint MapType_VirtualKeyToUnshiftedScanCode = 2;
		private const uint MapType_ScanCodeToRightOrLeftHandVirtualKey = 3;




		//public static void SendKeyPressToActiveApplication(Keys key, bool isShifted)
		//{
		//    uint scanCode = MapVirtualKey((uint) key, MapType_VirtualKeyToScanCode);

		//    System.Diagnostics.Debug.WriteLine("scanCode: " + scanCode);

		//    Press((int) scanCode);
		//    Release((int) scanCode);
		//}

		//public static void Press(int scanCode)
		//{
		//    SendKey(scanCode, true);
		//}

		//public static void  Release(int scanCode)
		//{
		//    SendKey(scanCode, false);
		//}

		//private static void SendKey(int scanCode, bool press)
		//{
		//    KEYBOARD_INPUT[] input = new KEYBOARD_INPUT[1];
		//    input[0] = new KEYBOARD_INPUT();
		//    input[0].type = INPUT_KEYBOARD;
		//    input[0].flags = KEYEVENTF_SCANCODE;

		//    if ((scanCode & 0xFF00) == 0xE000)
		//    { // extended key? 
		//        input[0].flags |= KEYEVENTF_EXTENDEDKEY;
		//    }

		//    if (press)
		//    { // press? 
		//        input[0].scanCode = (ushort)(scanCode & 0xFF);
		//    }
		//    else
		//    { // release? 
		//        input[0].scanCode = (ushort) scanCode;
		//        input[0].flags |= KEYEVENTF_KEYUP;
		//    }

		//    uint result = SendInput(1, input, Marshal.SizeOf(input[0]));

		//    if (result != 1)
		//    {
		//        throw new Exception("Could not send key: " + scanCode);
		//    }
		//} 



	}
}
