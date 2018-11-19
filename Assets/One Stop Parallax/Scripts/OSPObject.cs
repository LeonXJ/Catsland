using UnityEngine;
using System;

namespace OSP
{
    /// <summary>
    /// Class OSPObject. This class holds all of the information needed for a parallax layer. This must be added as a component
    /// to any GameObject that will be used by the One Stop Parallax system. It will automatically be added to any GameObject
    /// dragged into the list on the OneStopParallax script's object.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [RequireComponent(typeof(SpriteRenderer))]
    [Serializable]
    public class OSPObject : MonoBehaviour
    {
        /// <summary>
        /// The SpriteRenderer for this layer. Used to determine the bounds of the sprite.
        /// </summary>
        public SpriteRenderer sRenderer;

        /// <summary>
        /// Gets the sprite bounds. Used for calculations needing the size of the sprite.
        /// </summary>
        /// <value>The sprite bounds.</value>
        public Bounds SpriteBounds { get { return sRenderer.bounds; } }

        /// <summary>
        /// Determines whether the layer should use parallax on the X axis.
        /// </summary>
        public bool UseParallaxOnXAxis = true;
        /// <summary>
        /// Determines whether the layer should use parallax on the Y axis.
        /// </summary>
        public bool UseParallaxOnYAxis = false;

        /// <summary>
        /// Parallax smoothing amount on the X axis.
        /// </summary>
        public float ParallaxSmoothingX = 1;
        /// <summary>
        /// Parallax smoothing amount on the Y axis.
        /// </summary>
        public float ParallaxSmoothingY = 1;

        /// <summary>
        /// The scale at which this layer moves in relation to the camera's movement speed.
        /// This is determined by the Z position of this layer and its relation to the furthest Z element.
        /// </summary>
        public float ParallaxScalar = 0;

        /// <summary>
        /// Determines whether this layer should infinitely tile horizontally.
        /// </summary>
        public bool AutoTileX = false;
        /// <summary>
        /// Determines whether this layer should infinitely tile vertically.
        /// </summary>
        public bool AutoTileY = false;

        /// <summary>
        /// Determines whether this layer should move on its own along the X axis.
        /// </summary>
        public bool AutoMoveX = false;
        /// <summary>
        /// Determines whether this layer should move on its own along the Y axis.
        /// </summary>
        public bool AutoMoveY = false;

        /// <summary>
        /// The speed that this layer moves along the X axis.
        /// </summary>
        public float HorizontalSpeed = 0f;
        /// <summary>
        /// The speed that this layer moves along the Y axis.
        /// </summary>
        public float VerticalSpeed = 0f;

        /// <summary>
        /// The Y distance between objects that tile vertically. If the distance is set to be random, this won't be used.
        /// </summary>
        public float VerticalDistance = 0f;
        /// <summary>
        /// The X distance between objects that tile horizontally. If the distance is set to be random, this won't be used.
        /// </summary>
        public float HorizontalDistance = 0f;

        /// <summary>
        /// Determines whether this layer will randomize the Y distance between objects that tile vertically.
        /// </summary>
        public bool RandomizeVerticalDistance = false;
        /// <summary>
        /// Determines whether this layer will randomize the X distance between objects that tile horizontally.
        /// </summary>
        public bool RandomizeHorizontalDistance = false;

        /// <summary>
        /// Determines whether this layer will randomize the Y offset of objects that tile horizontally.
        /// </summary>
        public bool RandomizeHorizontalYAxis = false;
        /// <summary>
        /// Determines whether this layer will randomize the X offset of objects that tile vertically.
        /// </summary>
        public bool RandomizeVerticalXAxis = false;

        /// <summary>
        /// Determines if this layer will store the history of its random positions or not.
        /// </summary>
        public bool UseRandomHistory = true;
        /// <summary>
        /// The maximum number of random positions that this layer will store.
        /// </summary>
        public int MaxPlacementHistorySize = 100;
        /// <summary>
        /// The number of pooled objects to use for this layer. 
        /// This is determined by the size of the sprite and the size of the camera.
        /// </summary>
        public int PoolAmount = 1;

        /// <summary>
        /// The minimum random distance between vertically tiled objects on this layer.
        /// </summary>
        public float VerticalDistanceMin = 0f;
        /// <summary>
        /// The maximum random distance between vertically tiled objects on this layer.
        /// </summary>
        public float VerticalDistanceMax = 0f;
        /// <summary>
        /// The minimum random distance between horizontally tiled objects on this layer.
        /// </summary>
        public float HorizontalDistanceMin = 0f;
        /// <summary>
        /// The maximum random distance between horizontally tiled objects on this layer.
        /// </summary>
        public float HorizontalDistanceMax = 0f;

        /// <summary>
        /// The minimum random X offset for vertically tiled objects on this layer.
        /// </summary>
        public float VerticalXMin = 0f;
        /// <summary>
        /// The maximum random X offset for vertically tiled objects on this layer.
        /// </summary>
        public float VerticalXMax = 0f;
        /// <summary>
        /// The minimum random Y offset for horizontally tiled objects on this layer.
        /// </summary>
        public float HorizontalYMin = 0f;
        /// <summary>
        /// The minimum random Y offset for horizontally tiled objects on this layer.
        /// </summary>
        public float HorizontalYMax = 0f;

        /// <summary>
        /// Gets a value indicating whether this instance has right tile.
        /// </summary>
        /// <value><c>true</c> if this instance has right tile; otherwise, <c>false</c>.</value>
        public bool hasRightTile { get { return RightTile != null; } }
        /// <summary>
        /// Gets a value indicating whether this instance has left tile.
        /// </summary>
        /// <value><c>true</c> if this instance has left tile; otherwise, <c>false</c>.</value>
        public bool hasLeftTile { get { return LeftTile != null; } }
        /// <summary>
        /// Gets a value indicating whether this instance has top tile.
        /// </summary>
        /// <value><c>true</c> if this instance has top tile; otherwise, <c>false</c>.</value>
        public bool hasTopTile { get { return TopTile != null; } }
        /// <summary>
        /// Gets a value indicating whether this instance has bottom tile.
        /// </summary>
        /// <value><c>true</c> if this instance has bottom tile; otherwise, <c>false</c>.</value>
        public bool hasBottomTile { get { return BottomTile != null; } }

        /// <summary>
        /// The right tile
        /// </summary>
        public OSPObject RightTile;
        /// <summary>
        /// The left tile
        /// </summary>
        public OSPObject LeftTile;
        /// <summary>
        /// The top tile
        /// </summary>
        public OSPObject TopTile;
        /// <summary>
        /// The bottom tile
        /// </summary>
        public OSPObject BottomTile;

        /// <summary>
        /// Gets the width of the sprite.
        /// </summary>
        /// <value>The width of the sprite.</value>
        public float SpriteWidth { get { return sRenderer.sprite.bounds.size.x; } }
        /// <summary>
        /// Gets the height of the sprite.
        /// </summary>
        /// <value>The height of the sprite.</value>
        public float SpriteHeight { get { return sRenderer.sprite.bounds.size.y; } }

        /// <summary>
        /// This object's row value
        /// </summary>
        public int row = 0;
        /// <summary>
        /// This object's column value
        /// </summary>
        public int col = 0;

        /// <summary>
        /// The unique ID for this layer's objects
        /// </summary>
        public int ObjectID;

        /// <summary>
        /// Initializes any variables before the Start function
        /// </summary>
        void Awake()
        {
            sRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Performs any error checks to let the user know if any values are invalid.
        /// </summary>
        void Start()
        {
            #region Error Checks

            if (HorizontalDistanceMin < 0)
                Debug.LogError(name + " - Minimum X Distance can't be less than zero.");

            if (HorizontalDistanceMax < 0)
                Debug.LogError(name + " - Maximum X Distance can't be less than zero.");

            if (HorizontalDistanceMin > HorizontalDistanceMax)
                Debug.LogError(name + " - Maximum X Distance can't be less than the Minimum X Distance.");

            if (HorizontalYMin > HorizontalYMax)
                Debug.LogError(name + " - Maximum Y Offset can't be less than the Minimum Y Offset.");

            if (VerticalDistanceMin < 0)
                Debug.LogError(name + " - Minimum Y Distance can't be less than zero.");

            if (VerticalDistanceMax < 0)
                Debug.LogError(name + " - Maximum Y Distance can't be less than zero.");

            if (VerticalDistanceMin > VerticalDistanceMax)
                Debug.LogError(name + " - Maximum Y Distance can't be less than the Minimum Y Distance.");

            if (VerticalXMin > VerticalXMax)
                Debug.LogError(name + " - Maximum X Offset can't be less than the Minimum X Offset.");

            #endregion
        }

        /// <summary>
        /// Creates a new instance of this object for the object pool.
        /// </summary>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <returns>OSPObject.</returns>
        public OSPObject CreateNewInstance(bool active)
        {
            OSPObject newObject = Instantiate(this);
            newObject.ObjectID = ObjectID;
            newObject.transform.parent = transform.parent;
            newObject.transform.localScale = transform.localScale;
            newObject.ParallaxScalar = ParallaxScalar;
            newObject.LeftTile = null;
            newObject.RightTile = null;
            newObject.TopTile = null;
            newObject.BottomTile = null;
            newObject.gameObject.SetActive(active);
            return newObject;
        }

        /// <summary>
        /// Determines the pool amount for this layer's objects. This is determined by taking the sprite's size and
        /// calculating how many can fit inside the camera horizontally and vertically. This is rounded up and a couple of
        /// extra objects are added for padding to make sure we have enough in any given situation.
        /// </summary>
        /// <param name="camBounds">The camera's bounds.</param>
        public void DeterminePoolAmount(Bounds camBounds)
        {
            int poolTotal = 0;

            float spriteX = SpriteBounds.size.x;
            float spriteY = SpriteBounds.size.y;

            if (AutoTileX && RandomizeHorizontalDistance)
            {
                spriteX += HorizontalDistanceMin;

                if (RandomizeHorizontalYAxis)
                    spriteY += Mathf.Abs(HorizontalYMin - HorizontalYMax);
            }

            if (AutoTileY && RandomizeVerticalDistance)
            {
                spriteY += VerticalDistanceMin;

                if (RandomizeVerticalXAxis)
                    spriteX += Mathf.Abs(VerticalXMin - VerticalXMax);
            }

            var horizontalPool = (int)Mathf.Ceil(camBounds.size.x / spriteX);
            var verticalPool = (int)Mathf.Ceil(camBounds.size.y / spriteY);

            if (horizontalPool <= 2)
                horizontalPool = 3;
            else
                horizontalPool += 2;

            if (verticalPool <= 2)
                verticalPool = 3;
            else
                verticalPool += 2;

            if (AutoTileX && AutoTileY)
                poolTotal = horizontalPool * verticalPool;
            else if (AutoTileX)
                poolTotal += horizontalPool;
            else if (AutoTileY)
                poolTotal += verticalPool;

            // Insurance for cases where we have tiling on both the X and Y axes for large tiles. 
            // Because of how removal works, we might have gaps while it's waiting to remove one as we move diagonally.
            // It's better to have a few too many that aren't being used than to not have enough.
            if (AutoTileX && AutoTileY && poolTotal < 12)
                poolTotal = 12;

            PoolAmount = poolTotal;
        }
    }
}