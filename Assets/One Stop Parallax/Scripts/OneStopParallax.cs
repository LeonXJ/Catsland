using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace OSP
{
    /// <summary>
    /// An enum so we don't have to use magic strings to check for directions.
    /// </summary>
    public enum TileDirection
    {
        UP, DOWN, LEFT, RIGHT, NONE
    }

    /// <summary>
    /// Class OneStopParallax.
    /// Contains all functionality for implementing parallax movement on layers, tiling layers, and reading history offsets.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class OneStopParallax : MonoBehaviour
    {
        /// <summary>
        /// The public-facing list of parallax objects. 
        /// During runtime, this list will only contain parallax objects that don't tile.
        /// </summary>
        public List<OSPObject> ParallaxObjects;
        /// <summary>
        /// Contains the object pools for all parallax layers that tile on any axis.
        /// </summary>
        public List<OSPPool> ParallaxPools;

        /// <summary>
        /// The amount of smoothing for parallax movement on the X axis. This needs to be between or equal to 0 and 1.
        /// </summary>
        public float ParallaxSmoothingX = 1;
        /// <summary>
        /// The amount of smoothing for parallax movement on the Y axis. This needs to be between or equal to 0 and 1.
        /// </summary>
        public float ParallaxSmoothingY = 1;

        /// <summary>
        /// Reference to the main camera.
        /// </summary>
        public Camera MainCamera;

        /// <summary>
        /// Stores the position of the camera from the previous frame.
        /// </summary>
        private Vector3 prevCameraPosition;

        /// <summary>
        /// Contains a list of tiles that need to be deactivated because they are too far from the camera.
        /// </summary>
        private List<OSPObject> TilesToRemove;
        /// <summary>
        /// The bounds of the camera's viewport.
        /// </summary>
        public Bounds CamBounds;
        /// <summary>
        /// List of all offset history for each layer than implements randomization.
        /// </summary>
        private List<OSPRandomHistory> PlacementHistory;
        /// <summary>
        /// The Z value of the furthest parallax element.
        /// </summary>
        private float maxZ = float.MinValue;

        /// <summary>
        /// Determines if a seed is used for randomization to allow for the same sequence of random offsets between sessions.
        /// </summary>
        public bool UseSeedForRandomization = false;
        /// <summary>
        /// The seed used for randomization.
        /// </summary>
        public int RandomSeed = 0;

        /// <summary>
        /// Initializes everything.
        /// </summary>
        void Start()
        {
            prevCameraPosition = MainCamera.transform.position;
            TilesToRemove = new List<OSPObject>();
            PlacementHistory = new List<OSPRandomHistory>();
            ParallaxPools = new List<OSPPool>();

            // Grab the camera's bounds
            CamBounds = MainCamera.OrthographicBounds();

            // Loops through the parallax layers and assigns an ID to each one, finds the maximum Z distance from the camera,
            // and sets the object pool amount.
            // This loop is mainly to determine the maximum Z distance because our parallax speed doe each layer depends on it.
            for (int i = 0; i < ParallaxObjects.Count; ++i)
            {
                var obj = ParallaxObjects[i];
                obj.ObjectID = i;

                if (GetDistance(obj.transform.position.z, MainCamera.transform.position.z) > maxZ)
                {
                    maxZ = obj.transform.position.z - MainCamera.transform.position.z;
                }

                obj.DeterminePoolAmount(CamBounds);
            }

            // Store the previous random state so we can go back to it after setting our seed in case your game has another seeded random state.
            Random.State oldState = Random.state;
            if (UseSeedForRandomization)
                Random.InitState(RandomSeed);

            // Loop through the parallax objects again
            foreach (var obj in ParallaxObjects)
            {
                // Set the parallax speed for the layer
                obj.ParallaxScalar = GetParallaxScalar(obj);

                // If we're randomizing anything, we create a new history object and initialize the offset values
                if (obj.RandomizeHorizontalDistance || obj.RandomizeVerticalDistance)
                {
                    var newHist = new OSPRandomHistory
                    {
                        ID = obj.ObjectID,
                        MaxHistorySize = obj.MaxPlacementHistorySize,
                        UseRandomX = obj.RandomizeHorizontalDistance,
                        UseRandomY = obj.RandomizeVerticalDistance,
                        UseRandomHorizontalY = obj.RandomizeHorizontalYAxis,
                        UseRandomVerticalX = obj.RandomizeVerticalXAxis,
                        MinXOffset = obj.HorizontalDistanceMin,
                        MaxXOffset = obj.HorizontalDistanceMax,
                        MinYOffset = obj.VerticalDistanceMin,
                        MaxYOffset = obj.VerticalDistanceMax,
                        MinVerticalXOffset = obj.VerticalXMin,
                        MaxVerticalXOffset = obj.VerticalXMax,
                        MinHorizontalYOffset = obj.HorizontalYMin,
                        MaxHorizontalYOffset = obj.HorizontalYMax
                    };

                    // Only initialize the offset lists if we're actually storing history
                    if(obj.UseRandomHistory)
                        newHist.InitializeOffsetHistory();

                    PlacementHistory.Add(newHist);
                }

                // Create object pools for each layer and add them to the pool list
                if (obj.AutoTileX || obj.AutoTileY)
                {
                    var pool = new OSPPool(obj);
                    pool.CanGrow = false;
                    ParallaxPools.Add(pool);
                }
            }

            // Restore the original random state
            Random.state = oldState;

            // Remove the pooled objects from the parallax list since we don't need to perform the same functionality on tiled and non-tiled objects
            foreach (var pool in ParallaxPools)
            {
                ParallaxObjects.Remove(pool.PooledObject);
            }
        }

        /// <summary>
        /// Updates all of the parallax layers and objects.
        /// </summary>
        void LateUpdate()
        {
            // Set the camera bounds every frame so we know where the min and max are
            CamBounds = MainCamera.OrthographicBounds();

            // Apply parallax movement to each layer not being tiled            
            foreach (var obj in ParallaxObjects)
            {
                ApplyParallax(obj);
            }

            // Apply parallax movement to each pooled object
            foreach (var pool in ParallaxPools)
            {
                foreach (var obj in pool.Pool)
                {
                    if (!obj.gameObject.activeInHierarchy)
                        continue;

                    ApplyParallax(obj);
                }
            }

            TilesToRemove.Clear();

            // Loop over each tiled object and check if a tile needs to be added or removed
            foreach (var pool in ParallaxPools)
            {
                foreach (var obj in pool.Pool)
                {
                    // If it's an inactive object, don't waste any more time here
                    if (!obj.gameObject.activeInHierarchy)
                        continue;

                    // Get the random offset history for this object's layer if it exists
                    OSPRandomHistory historyList = null;
                    if (obj.RandomizeHorizontalDistance || obj.RandomizeVerticalDistance)
                    {
                        if (PlacementHistory.Any(h => h.ID == obj.ObjectID))
                        {
                            historyList = PlacementHistory.Single(h => h.ID == obj.ObjectID);
                        }
                    }

                    #region AutoTileX
                    if (obj.AutoTileX)
                    {
                        var sb = obj.SpriteBounds;

                        // Don't do anything if we already have a left or right tile set
                        if (!obj.hasLeftTile || !obj.hasRightTile)
                        {
                            // Add a tile to the right if it doesn't already have one and either it's visible by the camera or could be seen by the camera soon
                            if ((sb.min.x < CamBounds.max.x && !obj.hasRightTile) || (GetDistance(sb.max.x, CamBounds.max.x) < sb.size.x && sb.min.x <= CamBounds.max.x && !obj.hasRightTile))
                            {
                                int rowCheck = obj.row;
                                int colCheck = obj.col + 1;

                                // Check to see if an object already exists at this column/row combination before trying to add a new one
                                if (!pool.Pool.Any(o => o.row == rowCheck && o.col == colCheck && o.gameObject.activeInHierarchy) && !pool.TempPool.Any(o => o.row == rowCheck && o.col == colCheck && o.gameObject.activeInHierarchy))
                                {
                                    CreateNewTile(obj, TileDirection.RIGHT, historyList);
                                }
                            }

                            // Add a tile to the left if it doesn't already have one and either it's visible by the camera or could be seen by the camera soon
                            if ((sb.max.x > CamBounds.min.x && !obj.hasLeftTile) || (GetDistance(sb.min.x, CamBounds.min.x) < sb.size.x && sb.max.x >= CamBounds.min.x && !obj.hasLeftTile))
                            {
                                int rowCheck = obj.row;
                                int colCheck = obj.col - 1;

                                // Check to see if an object already exists at this column/row combination before trying to add a new one
                                if (!pool.Pool.Any(o => o.row == rowCheck && o.col == colCheck && o.gameObject.activeInHierarchy) && !pool.TempPool.Any(o => o.row == rowCheck && o.col == colCheck && o.gameObject.activeInHierarchy))
                                {
                                    CreateNewTile(obj, TileDirection.LEFT, historyList);
                                }
                            }
                        }

                        // Removes items too far from camera
                        if (obj.gameObject.activeInHierarchy)
                        {
                            bool removeLeft = obj.hasRightTile ? (sb.min.x < CamBounds.min.x && sb.max.x < CamBounds.min.x)
                                && (obj.RightTile.SpriteBounds.min.x < CamBounds.min.x && obj.RightTile.SpriteBounds.max.x < CamBounds.min.x) : false;

                            bool removeRight = obj.hasLeftTile ? (sb.min.x > CamBounds.max.x && sb.max.x > CamBounds.max.x)
                                && (obj.LeftTile.SpriteBounds.min.x > CamBounds.max.x && obj.LeftTile.SpriteBounds.max.x > CamBounds.max.x) : false;

                            bool removeUnlinked = !obj.hasLeftTile && !obj.hasRightTile && !obj.hasTopTile && !obj.hasBottomTile;

                            if (removeLeft || removeRight || removeUnlinked)
                            {
                                if (!TilesToRemove.Contains(obj))
                                    TilesToRemove.Add(obj);
                            }
                        }

                    }
                    #endregion

                    #region AutoTileY
                    if (obj.AutoTileY)
                    {
                        var sb = obj.SpriteBounds;

                        // Don't do anything if we already have a top or bottom tile set
                        if (!obj.hasTopTile || !obj.hasBottomTile)
                        {
                            // Add a tile to the top if it doesn't already have one and either it's visible by the camera or could be seen by the camera soon
                            if ((sb.min.y < CamBounds.max.y && !obj.hasTopTile) || (GetDistance(sb.max.y, CamBounds.max.y) < sb.size.y && sb.min.y <= CamBounds.max.y && !obj.hasTopTile))
                            {
                                int rowCheck = obj.row - 1;
                                int colCheck = obj.col;

                                // Check to see if an object already exists at this column/row combination before trying to add a new one
                                if (!pool.Pool.Any(o => o.row == rowCheck && o.col == colCheck && o.gameObject.activeInHierarchy) && !pool.TempPool.Any(o => o.row == rowCheck && o.col == colCheck && o.gameObject.activeInHierarchy))
                                {
                                    CreateNewTile(obj, TileDirection.UP, historyList);
                                }
                            }

                            // Add a tile to the bottom if it doesn't already have one and either it's visible by the camera or could be seen by the camera soon
                            if ((sb.max.y > CamBounds.min.y && !obj.hasBottomTile) || (GetDistance(sb.min.y, CamBounds.min.y) < sb.size.y && sb.max.y >= CamBounds.min.y && !obj.hasBottomTile))
                            {
                                int rowCheck = obj.row + 1;
                                int colCheck = obj.col;

                                // Check to see if an object already exists at this column/row combination before trying to add a new one
                                if (!pool.Pool.Any(o => o.row == rowCheck && o.col == colCheck && o.gameObject.activeInHierarchy) && !pool.TempPool.Any(o => o.row == rowCheck && o.col == colCheck && o.gameObject.activeInHierarchy))
                                {
                                    CreateNewTile(obj, TileDirection.DOWN, historyList);
                                }
                            }
                        }

                        // Removes items too far from camera
                        if (obj.gameObject.activeInHierarchy)
                        {
                            bool removeBottom = obj.hasTopTile ? (sb.min.y < CamBounds.min.y && sb.max.y < CamBounds.min.y)
                                && (obj.TopTile.SpriteBounds.min.y < CamBounds.min.y && obj.TopTile.SpriteBounds.max.y < CamBounds.min.y) : false;

                            bool removeTop = obj.hasBottomTile ? (sb.min.y > CamBounds.max.y && sb.max.y > CamBounds.max.y)
                                && (obj.BottomTile.SpriteBounds.min.y > CamBounds.max.y && obj.BottomTile.SpriteBounds.max.y > CamBounds.max.y) : false;

                            bool removeUnlinked = !obj.hasLeftTile && !obj.hasRightTile && !obj.hasTopTile && !obj.hasBottomTile;

                            if (removeBottom || removeTop || removeUnlinked)
                            {
                                if (!TilesToRemove.Contains(obj))
                                    TilesToRemove.Add(obj);
                            }
                        }

                    }
                    #endregion
                }
            }

            foreach (var pool in ParallaxPools)
            {
                pool.AddTemporaryObjectsToPool();
            }

            foreach (var tile in TilesToRemove)
            {
                //remove tiles and update their connected tiles 
                if (tile.hasLeftTile)
                    tile.LeftTile.RightTile = null;

                if (tile.hasRightTile)
                    tile.RightTile.LeftTile = null;

                if (tile.hasTopTile)
                    tile.TopTile.BottomTile = null;

                if (tile.hasBottomTile)
                    tile.BottomTile.TopTile = null;

                tile.LeftTile = null;
                tile.RightTile = null;
                tile.TopTile = null;
                tile.BottomTile = null;
                tile.gameObject.SetActive(false);
            }

            prevCameraPosition = MainCamera.transform.position;
        }

        /// <summary>
        /// Applies parallax movement to a layer.
        /// </summary>
        /// <param name="obj">The parallax object.</param>
        void ApplyParallax(OSPObject obj)
        {
            // Find out how much the camera has moved since the last frame
            Vector3 cameraDelta = MainCamera.transform.position - prevCameraPosition;

            // Calculate how much this layer should move on the X and Y axis and add it to the variables holding the new position offsets
            float parallaxX = (cameraDelta.x - (cameraDelta.x * obj.ParallaxScalar)) * obj.ParallaxSmoothingX * ParallaxSmoothingX;
            float parallaxY = (cameraDelta.y - (cameraDelta.y * obj.ParallaxScalar)) * obj.ParallaxSmoothingY * ParallaxSmoothingY;

            float addedX = 0;
            if (obj.UseParallaxOnXAxis)
                addedX = parallaxX;

            float addedY = 0;
            if (obj.UseParallaxOnYAxis)
                addedY = parallaxY;

            // If the object moves, add that to the offset as well
            // Dividing by ten normalizes the movement so you don't have to have numbers like 0.01 since 1 would be very fast otherwise
            if (obj.AutoMoveX)
                addedX += obj.HorizontalSpeed / 10;

            if (obj.AutoMoveY)
                addedY += obj.VerticalSpeed / 10;

            Vector3 addedPos = new Vector3(addedX, addedY, 0);

            obj.transform.position += addedPos;
        }

        /// <summary>
        /// Creates a new tile in the given direction.
        /// </summary>
        /// <param name="tile">The parallax tile object.</param>
        /// <param name="direction">The direction we're creating a new tile in.</param>
        /// <param name="historyList">The history list for that layer.</param>
        void CreateNewTile(OSPObject tile, TileDirection direction, OSPRandomHistory historyList)
        {
            // Grab the object pool for this layer
            OSPPool pool = ParallaxPools.SingleOrDefault(p => p.ObjectID == tile.ObjectID);

            // Sometimes, a large image will have a slight gap that shows up on camera movement. This isn't the best fix, but it works.
            float gapOffset = 0.05f;

            // Figure out the scale of the sprite so we can figure out how far to move it in order to tile
            float spriteScaleX = tile.transform.localScale.x;
            if (tile.transform.parent != null)
                spriteScaleX *= tile.transform.parent.localScale.x;

            float spriteScaleY = tile.transform.localScale.y;
            if (tile.transform.parent != null)
                spriteScaleY *= tile.transform.parent.localScale.y;

            if (direction == TileDirection.LEFT || direction == TileDirection.RIGHT)
            {
                #region Left/Right Tiles

                float posOffset = (tile.SpriteWidth * (direction == TileDirection.LEFT ? -1 : 1) * spriteScaleX);
                gapOffset *= direction == TileDirection.LEFT ? 1 : -1;

                Vector3 newPosition = new Vector3(
                    tile.transform.position.x + posOffset + gapOffset,
                    tile.transform.position.y,
                    tile.transform.position.z);

                if (direction == TileDirection.RIGHT)
                {
                    // If we're randomizing the horizontal distance, get the offset and add it to the position
                    if (tile.RandomizeHorizontalDistance)
                    {
                        newPosition.x += historyList.GetXOffsetForColumn(tile.col, tile.UseRandomHistory);

                        // If we're randomizing the Y offset, get the offset and add it to the position
                        if (tile.RandomizeHorizontalYAxis)
                            newPosition.y += historyList.GetYOffsetForColumn(tile.col, tile.UseRandomHistory);
                    }
                    else
                        newPosition.x += tile.HorizontalDistance;
                }
                else if (direction == TileDirection.LEFT)
                {
                    // If we're randomizing the horizontal distance, get the offset and add it to the position
                    if (tile.RandomizeHorizontalDistance)
                    {
                        newPosition.x -= historyList.GetXOffsetForColumn(tile.col - 1, tile.UseRandomHistory);

                        // If we're randomizing the Y offset, get the offset and add it to the position
                        if (tile.RandomizeHorizontalYAxis)
                            newPosition.y += historyList.GetYOffsetForColumn(tile.col - 1, tile.UseRandomHistory);
                    }
                    else
                        newPosition.x -= tile.HorizontalDistance;
                }

                // Get the new tile from the object pool. Abandon ship if it couldn't get a tile.
                var newTile = pool.GetPooledObject();
                if (newTile == null)
                    return;

                newTile.transform.position = newPosition;
                newTile.transform.rotation = tile.transform.rotation;

                // Set the new column for this tile
                if (direction == TileDirection.RIGHT)
                {
                    newTile.row = tile.row;
                    newTile.col = tile.col + 1;
                }
                else
                {
                    newTile.row = tile.row;
                    newTile.col = tile.col - 1;
                }
                
                // Set the left, right, top, and bottom tiles that connect to this one
                pool.SetSiblings(newTile);

                // Now that we're in position, set this tile active
                newTile.gameObject.SetActive(true);

                #endregion
            }
            else
            {
                #region Up/Down Tiles

                float posOffset = (tile.SpriteHeight * (direction == TileDirection.DOWN ? -1 : 1) * spriteScaleY);
                gapOffset *= direction == TileDirection.DOWN ? 1 : -1;

                Vector3 newPosition = new Vector3(
                    tile.transform.position.x,
                    tile.transform.position.y + posOffset + gapOffset,
                    tile.transform.position.z);

                if (direction == TileDirection.UP)
                {
                    // If we're randomizing the vertical distance, get the offset and add it to the position
                    if (tile.RandomizeVerticalDistance)
                    {
                        newPosition.y += historyList.GetYOffsetForRow(tile.row, tile.UseRandomHistory);

                        // If we're randomizing the X offset, get the offset and add it to the position
                        if (tile.RandomizeVerticalXAxis)
                            newPosition.x += historyList.GetXOffsetForRow(tile.row, tile.UseRandomHistory);
                    }
                    else
                        newPosition.y += tile.VerticalDistance;
                }
                else if (direction == TileDirection.DOWN)
                {
                    // If we're randomizing the vertical distance, get the offset and add it to the position
                    if (tile.RandomizeVerticalDistance)
                    {
                        newPosition.y -= historyList.GetYOffsetForRow(tile.row - 1, tile.UseRandomHistory);

                        // If we're randomizing the X offset, get the offset and add it to the position
                        if (tile.RandomizeVerticalXAxis)
                            newPosition.x += historyList.GetXOffsetForRow(tile.row - 1, tile.UseRandomHistory);
                    }
                    else
                        newPosition.y -= tile.VerticalDistance;
                }

                // Get the new tile from the object pool. Abandon ship if it couldn't get a tile.
                var newTile = pool.GetPooledObject();
                if (newTile == null)
                    return;

                newTile.transform.position = newPosition;
                newTile.transform.rotation = tile.transform.rotation;

                // Set the new row for this tile
                if (direction == TileDirection.DOWN)
                {
                    newTile.row = tile.row + 1;
                    newTile.col = tile.col;
                }
                else
                {
                    newTile.row = tile.row - 1;
                    newTile.col = tile.col;
                }

                // Set the left, right, top, and bottom tiles that connect to this one
                pool.SetSiblings(newTile);

                // Now that we're in position, set this tile active
                newTile.gameObject.SetActive(true);

                #endregion
            }
        }

        /// <summary>
        /// Gets the normalized parallax scalar for the layer using the distance from the camera to the furthest object's Z position
        /// to keep the movements all related to one another. This way, no matter where you are on the Z axis with your camera, we'll get
        /// consistent movement of the layers.
        /// </summary>
        /// <param name="obj">The object layer.</param>
        /// <returns>System.Single.</returns>
        float GetParallaxScalar(OSPObject obj)
        {
            // Find the Z distance between the camera and the parallax layer
            float distanceToCamera = Mathf.Abs(obj.transform.position.z - MainCamera.transform.position.z);
            float parallaxScalar;

            // If the camera is behind the camera somehow, and you're somehow rendering that, set the ratio to be > 1
            // Otherwise, set the ratio for movement to be under one
            if (MainCamera.transform.position.z > obj.transform.position.z)
                parallaxScalar = ((maxZ + distanceToCamera) / maxZ);
            else
                parallaxScalar = Mathf.Abs((maxZ - distanceToCamera) / maxZ);

            return parallaxScalar;
        }

        /// <summary>
        /// Gets the distance between the axis value of two points in a function using the DRY principle that I always forget to use.
        /// </summary>
        /// <param name="num1">The first point's number.</param>
        /// <param name="num2">The second point's number.</param>
        /// <returns>System.Single.</returns>
        float GetDistance(float num1, float num2)
        {
            // Gutted distance formula
            return Mathf.Sqrt(Mathf.Pow(num1 - num2, 2));
        }
    }
}