using System.Collections;
using UnityEngine;

namespace GunService
{
    public static class GunFireEffects
    {
        private static bool _initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            if (_initialized) return;
            _initialized = true;
            GunActionManager.onFireEffect += Play;
        }

        private static void Play(Vector3 origin, Vector3 endPoint, bool hitSomething)
        {
            var runner = GunEffectRunner.Instance;
            runner.StartCoroutine(runner.PlayTracer(origin, endPoint));
            if (hitSomething)
                runner.StartCoroutine(runner.PlayImpact(endPoint));
        }
    }

    internal class GunEffectRunner : MonoBehaviour
    {
        private static GunEffectRunner _instance;

        public static GunEffectRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("GunEffectRunner");
                    Object.DontDestroyOnLoad(go);
                    _instance = go.AddComponent<GunEffectRunner>();
                }
                return _instance;
            }
        }

        public IEnumerator PlayTracer(Vector3 origin, Vector3 endPoint)
        {
            var tracer = new GameObject("Tracer");
            var line = tracer.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = new Color(1f, 0.9f, 0.4f, 1f);
            line.endColor = new Color(1f, 0.9f, 0.4f, 0f);
            line.startWidth = 0.03f;
            line.endWidth = 0.01f;
            line.positionCount = 2;
            line.SetPosition(0, origin);
            line.SetPosition(1, endPoint);

            var flashObject = new GameObject("MuzzleFlash");
            flashObject.transform.position = origin;
            var flash = flashObject.AddComponent<Light>();
            flash.color = new Color(1f, 0.8f, 0.4f);
            flash.range = 4f;
            flash.intensity = 6f;

            yield return new WaitForSeconds(0.05f);

            Object.Destroy(flashObject);
            Object.Destroy(tracer);
        }

        public IEnumerator PlayImpact(Vector3 point)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Impact";
            Object.Destroy(sphere.GetComponent<Collider>());
            sphere.transform.position = point;

            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (material.shader == null || !material.shader.isSupported)
                material = new Material(Shader.Find("Standard"));
            material.color = Color.yellow;
            sphere.GetComponent<Renderer>().material = material;

            const float duration = 0.15f;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                sphere.transform.localScale = Vector3.one * Mathf.Lerp(0.15f, 0f, elapsed / duration);
                yield return null;
            }

            Object.Destroy(sphere);
        }
    }
}
