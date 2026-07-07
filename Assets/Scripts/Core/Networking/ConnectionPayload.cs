using System;
using System.Text;
using UnityEngine;

namespace Game.Core.Networking
{
    [Serializable]
    public struct ConnectionPayload
    {
        public string PlayerName;

        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(JsonUtility.ToJson(this));
        }

        public static ConnectionPayload FromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return new ConnectionPayload { PlayerName = "Player" };
            return JsonUtility.FromJson<ConnectionPayload>(Encoding.UTF8.GetString(bytes));
        }
    }
}
