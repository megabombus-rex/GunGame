using Godot;
using System;
using System.Linq;

namespace GunGame.assets.scripts.misc
{
    public static class InputWrapper
    {
        public static bool IsActionPressed(string actionName, int deviceId)
        {
            return deviceId switch
            {
                0 => Input.IsActionPressed(actionName),
                1 => Input.IsJoyButtonPressed(deviceId, GetJoyButtonFromAction(actionName)),
                2 => Input.IsJoyButtonPressed(deviceId, GetJoyButtonFromAction(actionName)),
                _ => throw new NotImplementedException()
            };
        }

        // find out if it can be done better
        private static JoyButton GetJoyButtonFromAction(string actionName)
        {
            //var joy = ProjectSettings.GetSetting(actionName);
            var actionEvents = InputMap.Singleton.ActionGetEvents(actionName)
                .Select(ev => ev as InputEventJoypadButton)
                .ToArray();

            if (actionEvents.Length < 1)
            {
                return JoyButton.Invalid;
            }

            return actionEvents.FirstOrDefault().ButtonIndex;
        }
    }
}
