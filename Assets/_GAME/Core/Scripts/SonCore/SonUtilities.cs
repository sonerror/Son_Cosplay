using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace sonnv
{
    [Serializable]
    public struct PairVector3
    {
        public Vector3 first;
        public Vector3 second;
    }

    [Serializable]
    public struct PairInt
    {
        public int first;
        public int second;

        public PairInt(int first, int second)
        {
            this.first = first;
            this.second = second;
        }
    }

    [Serializable]
    public struct PairFloat
    {
        public float minValue;
        public float maxValue;
    }

    [Serializable]
    public struct AudioData
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    [Serializable]
    public struct FitScreenObject
    {
        public Transform transform;
        public Direction direction;
        public float padding;
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum CompleteP3DTextureState
    {
        Fill = 0,
        Erase = 1,
    }

    public static class SonUtilities
    {
        public static Rect GetWorldSpaceRect(this RectTransform rectTransform, Camera cam = null)
        {
            if (rectTransform == null)
                return new Rect();

            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            // Convert to world space if camera is provided (useful if Canvas is Screen Space - Camera)
            if (cam != null)
            {
                for (int i = 0; i < corners.Length; i++)
                    corners[i] = cam.ScreenToWorldPoint(corners[i]);
            }

            Vector3 bottomLeft = corners[0];
            Vector3 topRight = corners[2];

            return new Rect(
                bottomLeft.x,
                bottomLeft.y,
                topRight.x - bottomLeft.x,
                topRight.y - bottomLeft.y
            );
        }

        public static Task<Texture2D> TakeScreenshotAsync()
        {
            var tcs = new TaskCompletionSource<Texture2D>();

            Camera cam = Camera.main;
            if (!cam)
            {
                tcs.SetResult(null);
                return tcs.Task;
            }

            int width = cam.pixelWidth;
            int height = cam.pixelHeight;

            var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            cam.targetTexture = rt;
            cam.Render();
            cam.targetTexture = null;

            AsyncGPUReadback.Request(rt, 0, TextureFormat.RGBA32, request =>
            {
                UnityEngine.Object.Destroy(rt);

                if (request.hasError)
                {
                    tcs.SetResult(null);
                    return;
                }

                Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
                tex.LoadRawTextureData(request.GetData<byte>());
                tex.Apply();

                tcs.SetResult(tex);
            });

            return tcs.Task;
        }

        public static Task<Texture2D> TakeScreenshotRectAsync(Rect rect, bool rectYIsTopLeft = false, Camera cam = null)
        {
            var tcs = new TaskCompletionSource<Texture2D>();

            cam = cam != null ? cam : Camera.main;
            if (cam == null)
            {
                tcs.SetResult(null);
                return tcs.Task;
            }

            int rtW = cam.pixelWidth;
            int rtH = cam.pixelHeight;

            var rt = new RenderTexture(rtW, rtH, 24, RenderTextureFormat.ARGB32);

            // Render camera to RT
            cam.targetTexture = rt;
            cam.Render();
            cam.targetTexture = null;

            // Convert rect coordinate
            float x = rect.x;
            float y = rectYIsTopLeft ? (rtH - (rect.y + rect.height)) : rect.y;

            int ix = Mathf.FloorToInt(x);
            int iy = Mathf.FloorToInt(y);
            int iw = Mathf.CeilToInt(rect.width);
            int ih = Mathf.CeilToInt(rect.height);

            // Clamp the crop region inside render texture
            ix = Mathf.Clamp(ix, 0, rtW - 1);
            iy = Mathf.Clamp(iy, 0, rtH - 1);
            iw = Mathf.Clamp(iw, 1, rtW - ix);
            ih = Mathf.Clamp(ih, 1, rtH - iy);

            // Read GPU data async
            AsyncGPUReadback.Request(rt, 0, TextureFormat.RGBA32, request =>
            {
                UnityEngine.Object.Destroy(rt);

                if (request.hasError)
                {
                    tcs.SetResult(null);
                    return;
                }

                // Full frame bytes
                var fullData = request.GetData<byte>();

                // Create cropped texture
                Texture2D tex = new Texture2D(iw, ih, TextureFormat.RGBA32, false);

                // Copy only the crop region
                int stride = rtW * 4;     // each row has rtW pixels * 4 bytes
                byte[] cropped = new byte[iw * ih * 4];

                for (int row = 0; row < ih; row++)
                {
                    int fullIndex = ((iy + row) * rtW + ix) * 4;
                    int copyIndex = row * iw * 4;
                    Buffer.BlockCopy(fullData.ToArray(), fullIndex, cropped, copyIndex, iw * 4);
                }

                tex.LoadRawTextureData(cropped);
                tex.Apply();

                tcs.SetResult(tex);
            });

            return tcs.Task;
        }

        public static IEnumerator TakeScreenshotSync(Action<Texture2D> callback)
        {
            yield return new WaitForEndOfFrame();

            Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
            callback?.Invoke(tex);
        }

        public static IEnumerator TakeScreenshotSync(Rect rect, Action<Texture2D> callback)
        {
            yield return new WaitForEndOfFrame();

            Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
            callback?.Invoke(CropTexture(tex));
            UnityEngine.Object.Destroy(tex);
            yield break;

            Texture2D CropTexture(Texture2D src)
            {
                int x = Mathf.FloorToInt(rect.x);
                int y = Mathf.FloorToInt(rect.y);
                int w = Mathf.FloorToInt(rect.width);
                int h = Mathf.FloorToInt(rect.height);

                Color[] pixels = src.GetPixels(x, y, w, h);
                Texture2D dst = new Texture2D(w, h, src.format, false);
                dst.SetPixels(pixels);
                dst.Apply();

                return dst;
            }
        }

        private static RenderTexture _rt;

        public static Texture2D TakeScreenshot()
        {
            Camera cam = Camera.main;
            if (!cam) return null;

            int w = cam.pixelWidth;
            int h = cam.pixelHeight;

            if (_rt == null || _rt.width != w || _rt.height != h)
            {
                if (_rt != null) _rt.Release();
                _rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
                {
                    useMipMap = false,
                    autoGenerateMips = false
                };
                _rt.Create();
            }

            var prevCamRT = cam.targetTexture;
            var prevActive = RenderTexture.active;

            cam.targetTexture = _rt;
            cam.Render();
            RenderTexture.active = _rt;

            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false, true);
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply(false, false);

            cam.targetTexture = prevCamRT;
            RenderTexture.active = prevActive;

            return tex;
        }

        public static Texture2D TakeScreenshot(Rect rect, bool rectYIsTopLeft = false, Camera cam = null)
        {
            cam ??= Camera.main;
            if (cam == null) return null;

            int rtW = cam.pixelWidth;
            int rtH = cam.pixelHeight;

            if (_rt == null || _rt.width != rtW || _rt.height != rtH)
            {
                if (_rt != null) _rt.Release();
                _rt = new RenderTexture(rtW, rtH, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
                {
                    useMipMap = false,
                    autoGenerateMips = false
                };
                _rt.Create();
            }

            var prevCamRT = cam.targetTexture;
            var prevActive = RenderTexture.active;

            cam.targetTexture = _rt;
            cam.Render();
            RenderTexture.active = _rt;

            float x = rect.x;
            float y = rectYIsTopLeft ? (rtH - (rect.y + rect.height)) : rect.y;

            int ix = Mathf.FloorToInt(x);
            int iy = Mathf.FloorToInt(y);
            int iw = Mathf.CeilToInt(rect.width);
            int ih = Mathf.CeilToInt(rect.height);

            ix = Mathf.Clamp(ix, 0, rtW - 1);
            iy = Mathf.Clamp(iy, 0, rtH - 1);
            iw = Mathf.Clamp(iw, 1, rtW - ix);
            ih = Mathf.Clamp(ih, 1, rtH - iy);

            var tex = new Texture2D(iw, ih, TextureFormat.RGBA32, false, true);
            tex.ReadPixels(new Rect(ix, iy, iw, ih), 0, 0);
            tex.Apply(false, false);

            cam.targetTexture = prevCamRT;
            RenderTexture.active = prevActive;

            return tex;
        }

        public static void SetLocalPositionY(this Transform transform, float y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        }

        public static Color SetAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static float DistanceToInSqrVec2(Vector2 point1, Vector2 point2)
        {
            return (point2 - point1).sqrMagnitude;
        }

        public static void ModifyStartColorAlpha(this ParticleSystem particle, float alpha)
        {
            var main = particle.main;
            var color = main.startColor.color;
            color.a = alpha;
            main.startColor = color;
        }

        public static void ChangeColorOverLifeTime(this ParticleSystem particle, Color color)
        {
            if (particle == null) return;

            var colorOverLifetime = particle.colorOverLifetime;
            Gradient gradient = colorOverLifetime.color.gradient;

            GradientColorKey[] colorKeys = gradient.colorKeys;
            GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

            for (int i = 0; i < colorKeys.Length; i++)
            {
                colorKeys[i].color = color;
            }

            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i].alpha = color.a;
            }

            Gradient newGradient = new Gradient();
            newGradient.SetKeys(colorKeys, alphaKeys);
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(newGradient);
        }

        public static void FadeIn(this SkeletonAnimation skeleton, float time = 0.5f, Action callback = null)
        {
            // Set initial alpha to 0
            skeleton.skeleton.A = 0;

            // Use DOTween to fade to alpha 1
            DOTween.To(() => skeleton.skeleton.A,
                    x => skeleton.skeleton.A = x,
                    1,
                    1f)
                .OnUpdate(() => skeleton.skeleton.UpdateWorldTransform(Skeleton.Physics.None))
                .SetEase(Ease.Linear)
                .OnComplete(() => callback?.Invoke());
        }

        // public static void CompleteTexture(this P3dPaintableTexture texture, CompleteP3DTextureState state, Action callback = null, float duration = 1f,
        //     P3dPaintable customPaintable = null)
        // {
        //     #region Get Paintable
        //
        //     if (!customPaintable)
        //     {
        //         customPaintable = texture.GetComponent<P3dPaintable>();
        //     }
        //
        //     #endregion
        //
        //     if (!customPaintable) return;
        //     
        //     #region Get Mode
        //
        //     if (state == CompleteP3DTextureState.Fill)
        //     {
        //         texture.StartCoroutine(IECompleteTexture(P3dBlendMode.ReplaceOriginal(new Vector4(0, 0, 0, 1)),
        //             Color.white));
        //     }
        //     else
        //     {
        //         // Not use this because it's better to just fade out the Material color than submitting command every frame
        //         // texture.StartCoroutine(IECompleteTexture(P3dBlendMode.Replace(new Vector4(0, 0, 0, 1)), Color.clear));
        //         texture.StartCoroutine(IEEraseTextureByFade());
        //     }
        //
        //     #endregion
        //     
        //     return;
        //     
        //     IEnumerator IECompleteTexture(P3dBlendMode mode, Color color)
        //     {
        //         float timeStart = Time.time;
        //         while (Time.time < timeStart + duration)
        //         {
        //             float opacity = 1f / duration * 3f * Time.deltaTime; // 3f is a magic number?
        //             P3dCommandFill.Instance.SetState(false, 1);
        //             P3dCommandFill.Instance.SetMaterial(mode, null, color, opacity, 0);
        //             P3dPaintableManager.Submit(P3dCommandFill.Instance, customPaintable, texture);
        //             yield return null;
        //         }
        //         callback?.Invoke();
        //     }
        //
        //     // Another way using P3dPaintableManager is set mode to Replace with Color.clear
        //     // But I try to do this since it better optimize than submitting command every frame
        //     IEnumerator IEEraseTextureByFade()
        //     {
        //         Material material = texture.Material;
        //         float alpha = material.color.a;
        //         while (alpha > 0)
        //         {
        //             alpha -= Time.deltaTime / duration;
        //             material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
        //             yield return null;
        //         }
        //         callback?.Invoke();
        //     }
        // }

        public static bool IsPointerOverUIObject(this EventSystem eventSystem)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(eventSystem)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            List<RaycastResult> results = new List<RaycastResult>();
            eventSystem.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public static bool TryGetTouch(int fingerID, out Touch touch)
        {
            foreach (Touch t in Input.touches)
            {
                if (t.fingerId != fingerID) continue;
                touch = t;
                return true;
            }
            // No touch with given ID exists
            touch = default;
            return false;
        }

        public static void WaitFrame(int frame, Action action)
        {
            IEnumerator WaitFrameCoroutine()
            {
                yield return new WaitForSeconds(frame);
                action?.Invoke();
            }
        }

        public static void CenterObject(this SpriteRenderer sr, Camera cam, float marginHorizontal, float marginVertical, float paddingBottom, float zPos, Transform[] constantSizeChild = null)
        {
            Vector3 srScale = sr.transform.localScale;
            // make the sprite fit all the screen
            float width = sr.sprite.bounds.size.x;
            float height = sr.sprite.bounds.size.y;
            float worldScreenHeight = cam.orthographicSize * 2;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
            sr.transform.localScale = new Vector3(worldScreenWidth / width - marginHorizontal, worldScreenHeight / height - marginVertical, 1);
            // center the sprite
            Vector3 newPos = cam.transform.position;
            newPos.z = zPos;
            newPos.y += paddingBottom;
            sr.transform.position = newPos;

            float xScale = sr.transform.localScale.x / srScale.x;
            float yScale = sr.transform.localScale.y / srScale.y;
            if (constantSizeChild == null) return;
            for (int index = 0; index < constantSizeChild.Length; index++)
            {
                Transform child = constantSizeChild[index];
                child.localScale = new Vector3(child.localScale.x * xScale, child.localScale.y * yScale, 1);
            }
        }

        public static void CenterSpriteFitScreen(this SpriteRenderer sr, Camera camera, float paddingBottom, float zPosition = 0f)
        {
            // make the sprite fit all the screen
            float width = sr.sprite.bounds.size.x;
            float height = sr.sprite.bounds.size.y;
            float worldScreenHeight = camera.orthographicSize * 2;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
            sr.transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
            // center the sprite
            Vector3 newPos = camera.transform.position;
            newPos.z = zPosition;
            sr.transform.position = newPos;
        }

        public static void FitScreen(this Transform tf, Camera camera, float padding, Direction direction)
        {
            Vector3 newPosition = tf.position;
            switch (direction)
            {
                case Direction.Left:
                    newPosition.x = camera.ViewportToWorldPoint(new Vector3(0f + padding, 0f, 0f)).x;
                    break;
                case Direction.Right:
                    newPosition.x = camera.ViewportToWorldPoint(new Vector3(1f - padding, 0f, 0f)).x;
                    break;
                case Direction.Up:
                    newPosition.y = camera.ViewportToWorldPoint(new Vector3(0f, 1f - padding, 0f)).y;
                    break;
                case Direction.Down:
                    newPosition.y = camera.ViewportToWorldPoint(new Vector3(0f, 0f + padding, 0f)).y;
                    break;
            }
            tf.position = newPosition;
        }


        public static Dictionary<Direction, BoxCollider2D> QuickScreenBoundCollider(this Transform parent, float bottomOffset = 1.5f)
        {
            return CreateBoundCollider(parent, 10f, 0f, new Vector2(0, 0), Camera.main, 20f, bottomOffset);
        }

        private static Dictionary<Direction, BoxCollider2D> CreateBoundCollider(Transform parent, float colThickness, float zPosition,
            Vector2 screenSize, Camera cam, float xyScale = 20f, float offsetYDown = 1.5f)
        {
            Dictionary<Direction, BoxCollider2D> colliderDic = new();
            //Create a Dictionary to contain all our Objects/Transforms
            Dictionary<Direction, Transform> colliders = new()
            {
                //Create our GameObjects and add their Transform components to the Dictionary we created above
                { Direction.Up, new GameObject().transform },
                { Direction.Down, new GameObject().transform },
                { Direction.Right, new GameObject().transform },
                { Direction.Left, new GameObject().transform }
            };
            //Generate world space point information for position and scale calculations
            Vector3 cameraPos = cam.transform.position;
            screenSize.x = Vector2.Distance(cam.ScreenToWorldPoint(new Vector2(0, 0)),
                               cam.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f;
            screenSize.y = Vector2.Distance(cam.ScreenToWorldPoint(new Vector2(0, 0)),
                cam.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f;
            //For each Transform/Object in our Dictionary
            foreach (KeyValuePair<Direction, Transform> valPair in colliders)
            {
                valPair.Value.gameObject
                    .AddComponent<
                        BoxCollider2D>(); //Add our colliders. Remove the "2D" if you would like 3D colliders.
                valPair.Value.name =
                    valPair.Key +
                    "Collider"; //Set the object's name to it's "Key" name, and take on "Collider".  I.e: TopCollider
                valPair.Value.parent =
                    parent; //Make the object a child of whatever object this script is on (preferably the camera)

                valPair.Value.localScale = valPair.Key is Direction.Left or Direction.Right
                    ? new Vector3(colThickness, xyScale, colThickness)
                    : new Vector3(xyScale, colThickness, colThickness); //Scale the object to the width and height of the screen, using the world-space values calculated earlier

                colliderDic.Add(valPair.Key, valPair.Value.GetComponent<BoxCollider2D>());
            }

            colliders[Direction.Right].position =
                new Vector3(cameraPos.x + screenSize.x + colliders[Direction.Right].localScale.x * 0.5f, cameraPos.y,
                    zPosition);
            colliders[Direction.Left].position = new Vector3(cameraPos.x - screenSize.x - colliders[Direction.Left].localScale.x * 0.5f,
                cameraPos.y, zPosition);
            colliders[Direction.Up].position = new Vector3(cameraPos.x,
                cameraPos.y + screenSize.y + colliders[Direction.Up].localScale.y * 0.5f, zPosition);
            colliders[Direction.Down].position = new Vector3(cameraPos.x,
                cameraPos.y - screenSize.y - colliders[Direction.Down].localScale.y * 0.5f + offsetYDown, zPosition);

            return colliderDic;
        }
    }
}
