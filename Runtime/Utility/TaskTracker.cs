using System.Linq;
using UnityEngine.LowLevel;

namespace Depra.Assets.Runtime.Utility
{
	internal static class TaskTracker
	{
		public static void AddUpdateFunction(PlayerLoopSystem.UpdateFunction function)
		{
			var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
			var newLoop = new PlayerLoopSystem { updateDelegate = function };

			for (var index = 0; index < playerLoop.subSystemList.Length; index++)
			{
				if (playerLoop.subSystemList[index].updateDelegate != null &&
				    playerLoop.subSystemList[index].updateDelegate.Target == function.Target)
				{
					return;
				}
			}

			var subSystemList = playerLoop.subSystemList.ToList();
			subSystemList.Add(newLoop);
			playerLoop.subSystemList = subSystemList.ToArray();
			PlayerLoop.SetPlayerLoop(playerLoop);
		}

		public static void RemoveUpdateFunction(PlayerLoopSystem.UpdateFunction function)
		{
			var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
			for (var index = 0; index < playerLoop.subSystemList.Length; index++)
			{
				if (playerLoop.subSystemList[index].updateDelegate == null ||
				    playerLoop.subSystemList[index].updateDelegate.Target != function.Target)
				{
					continue;
				}

				var subSystemList = playerLoop.subSystemList.ToList();
				subSystemList.RemoveAt(index);
				playerLoop.subSystemList = subSystemList.ToArray();
				PlayerLoop.SetPlayerLoop(playerLoop);

				return;
			}
		}
	}
}