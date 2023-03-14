using System;
using System.Collections;
using UnityEngine;

namespace Depra.Assets.Tests.Common
{
    public class Wait
    {
        public static IEnumerator Until(Func<bool> condition, float timeout = 30f)
        {
            var timePassed = 0f;
            while (condition() == false && timePassed < timeout)
            {
                yield return null;
                timePassed += Time.deltaTime;
            }

            if (timePassed >= timeout)
            {
                throw new TimeoutException("Condition was not fulfilled for " + timeout + " seconds.");
            }
        }
    }
}