using Unity.Netcode;
using UnityEngine;

namespace Core
{
    public static class NetworkAttachUtility
    {
        public static void AttachToAnchor(NetworkObject target, Transform networkedRoot, Transform anchor)
        {
            target.TrySetParent(networkedRoot, false);
            target.transform.localPosition = networkedRoot.InverseTransformPoint(anchor.position);
            target.transform.localRotation = Quaternion.Inverse(networkedRoot.rotation) * anchor.rotation;
        }
    }
}
