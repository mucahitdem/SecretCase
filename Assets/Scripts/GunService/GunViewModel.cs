using UnityEngine;

namespace GunService
{
    public static class GunViewModel
    {
        public static void Attach(Transform owner)
        {
            if (owner.Find("GunViewModel") != null) return;

            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "GunViewModel";
            Object.Destroy(body.GetComponent<Collider>());

            var bodyTransform = body.transform;
            bodyTransform.SetParent(owner, false);
            bodyTransform.localPosition = new Vector3(0.3f, 1f, 0.5f);
            bodyTransform.localRotation = Quaternion.identity;
            bodyTransform.localScale = new Vector3(0.08f, 0.08f, 0.45f);

            var barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.name = "Barrel";
            Object.Destroy(barrel.GetComponent<Collider>());

            var barrelTransform = barrel.transform;
            barrelTransform.SetParent(bodyTransform, false);
            barrelTransform.localPosition = new Vector3(0f, 0f, 0.9f);
            barrelTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            barrelTransform.localScale = new Vector3(0.3f, 0.6f, 0.3f);

            var muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(barrelTransform, false);
            muzzle.transform.localPosition = new Vector3(0f, 1f, 0f);

            var color = new Color(0.15f, 0.15f, 0.17f);
            SetColor(body, color);
            SetColor(barrel, color);
        }

        public static Vector3 GetMuzzlePosition(Transform owner)
        {
            var muzzle = owner.Find("GunViewModel/Barrel/Muzzle");
            return muzzle != null ? muzzle.position : owner.position;
        }

        private static void SetColor(GameObject go, Color color)
        {
            var renderer = go.GetComponent<Renderer>();
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (material.shader == null || !material.shader.isSupported)
                material = new Material(Shader.Find("Standard"));
            material.color = color;
            renderer.material = material;
        }
    }
}
