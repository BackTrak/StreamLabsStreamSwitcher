using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreamLabsSceneSwitcher
{
	public enum KeyAction
	{
		KeyPress,
		KeyDown,
		KeyUp
	}

	public class KeyQueueItem
	{
		public readonly Keys Key = Keys.None;
		public readonly KeyAction KeyAction = KeyAction.KeyPress;

		public readonly int MillisecondsToPause = 0;

		public KeyQueueItem(Keys key, KeyAction keyAction, int millisecondsToPause)
		{
			this.Key = key;
			this.KeyAction = keyAction;
			this.MillisecondsToPause = millisecondsToPause;
		}
	}
}
