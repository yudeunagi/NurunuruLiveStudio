using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unage
{
    public static class CommonUtils
    {
        /**
          * 全角to半角
          */
        static public string ZtoH(string message)
        {
            message = message.Replace("０", "0");
            message = message.Replace("１", "1");
            message = message.Replace("２", "2");
            message = message.Replace("３", "3");
            message = message.Replace("４", "4");
            message = message.Replace("５", "5");
            message = message.Replace("６", "6");
            message = message.Replace("７", "7");
            message = message.Replace("８", "8");
            message = message.Replace("９", "9");

            return message;
        }

    }
}