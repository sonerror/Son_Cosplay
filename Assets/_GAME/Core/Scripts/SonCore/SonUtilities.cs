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

            var col = particle.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();

            gradient.SetKeys(
                new GradientColorKey[]
                {
            new GradientColorKey(color, 0f),
            new GradientColorKey(color, 1f)
                },
                new GradientAlphaKey[]
                {
            new GradientAlphaKey(color.a, 0f),
            new GradientAlphaKey(color.a, 1f)
                }
            );

            col.color = new ParticleSystem.MinMaxGradient(gradient);
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
            Dictionary<Direction, BoxCollider2D> colliderDic = new Dictionary<Direction, BoxCollider2D>();
            //Create a Dictionary to contain all our Objects/Transforms
            Dictionary<Direction, Transform> colliders = new Dictionary<Direction, Transform>()
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

                valPair.Value.localScale =
     (valPair.Key == Direction.Left || valPair.Key == Direction.Right)
         ? new Vector3(colThickness, xyScale, colThickness)
         : new Vector3(xyScale, colThickness, colThickness);
                //Scale the object to the width and height of the screen, using the world-space values calculated earlier

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
