using System.Linq;
using System.Collections.Generic;

namespace OSP
{
    /// <summary>
    /// Class OSPPool. Object pooling for our parallax layer objects.
    /// </summary>
    public class OSPPool
    {
        /// <summary>
        /// The object pool.
        /// </summary>
        public List<OSPObject> Pool;
        /// <summary>
        /// The temporary pool used to add new objects when the main pool is being traversed.
        /// The contents are moved to the main pool afterwards and cleared from this list.
        /// </summary>
        public List<OSPObject> TempPool;
        /// <summary>
        /// The parallax object being pooled. This gives us a reference to the object for when we need to clone it.
        /// </summary>
        public OSPObject PooledObject;
        /// <summary>
        /// The unique identifier for this layer's objects.
        /// </summary>
        public int ObjectID;
        /// <summary>
        /// The initial size of the object pool.
        /// </summary>
        public int InitialSize = 1;
        /// <summary>
        /// Determines whether this object pool can grow on its own if no inactive objects are found when one is requested.
        /// </summary>
        public bool CanGrow = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="OSPPool"/> class.
        /// This initializes any lists and adds an amount equal to the InitialSize variable to the pool.
        /// </summary>
        /// <param name="pooledObject">The pooled object.</param>
        public OSPPool(OSPObject pooledObject)
        {
            PooledObject = pooledObject;
            ObjectID = pooledObject.ObjectID;

            InitialSize = pooledObject.PoolAmount;

            if (InitialSize < 1)
                InitialSize = 1;

            TempPool = new List<OSPObject>();
            Pool = new List<OSPObject>();
            Pool.Add(PooledObject);

            for(int i = 0; i < (InitialSize - 1); ++i)
            {
                OSPObject newObject = PooledObject.CreateNewInstance(false);
                Pool.Add(newObject);
            }
        }

        /// <summary>
        /// Returns the first inactive object in the pool.
        /// If this pool is allowed to grow, it will create a new object if no inactive ones are found.
        /// </summary>
        /// <returns>OSPObject.</returns>
        public OSPObject GetPooledObject()
        {
            for(int i = 0; i < Pool.Count; ++i)
            {
                if (!Pool[i].gameObject.activeInHierarchy)
                    return Pool[i];
            }

            if(CanGrow)
            {
                OSPObject newObject = PooledObject.CreateNewInstance(true);
                TempPool.Add(newObject);
                return newObject;
            }

            return null;
        }

        /// <summary>
        /// Adds the temporary objects to the main pool.
        /// </summary>
        public void AddTemporaryObjectsToPool()
        {
            foreach(var obj in TempPool)
            {
                Pool.Add(obj);
            }

            TempPool.Clear();
        }

        /// <summary>
        /// Sets the left, right, top, and bottom tiles for this new tile if any are found.
        /// Doing this on creation saves us numerous checks in the main parallax loop when we check if a neighbor exists.
        /// </summary>
        /// <param name="tile">The tile.</param>
        public void SetSiblings(OSPObject tile)
        {
            #region Left and Right siblings

            if(tile.AutoTileX)
            {
                if (Pool.Any(o => o.row == tile.row && o.col == (tile.col + 1) && o.gameObject.activeInHierarchy))
                {
                    var rightTile = Pool.Single(o => o.row == tile.row && o.col == (tile.col + 1) && o.gameObject.activeInHierarchy);
                    rightTile.LeftTile = tile;
                    tile.RightTile = rightTile;
                }
                else if (TempPool.Any(o => o.row == tile.row && o.col == (tile.col + 1) && o.gameObject.activeInHierarchy))
                {
                    var rightTile = TempPool.Single(o => o.row == tile.row && o.col == (tile.col + 1) && o.gameObject.activeInHierarchy);
                    rightTile.LeftTile = tile;
                    tile.RightTile = rightTile;
                }
                else
                    tile.RightTile = null;

                if (Pool.Any(o => o.row == tile.row && o.col == (tile.col - 1) && o.gameObject.activeInHierarchy))
                {
                    var leftTile = Pool.Single(o => o.row == tile.row && o.col == (tile.col - 1) && o.gameObject.activeInHierarchy);
                    leftTile.RightTile = tile;
                    tile.LeftTile = leftTile;
                }
                else if (TempPool.Any(o => o.row == tile.row && o.col == (tile.col - 1) && o.gameObject.activeInHierarchy))
                {
                    var leftTile = TempPool.Single(o => o.row == tile.row && o.col == (tile.col - 1) && o.gameObject.activeInHierarchy);
                    leftTile.RightTile = tile;
                    tile.LeftTile = leftTile;
                }
                else
                    tile.LeftTile = null;
            }
            else
            {
                tile.RightTile = null;
                tile.LeftTile = null;
            }

            #endregion

            #region Top and Bottom siblings

            if(tile.AutoTileY)
            {
                if (Pool.Any(o => o.row == (tile.row + 1) && o.col == tile.col && o.gameObject.activeInHierarchy))
                {
                    var bottomTile = Pool.Single(o => o.row == (tile.row + 1) && o.col == tile.col && o.gameObject.activeInHierarchy);
                    bottomTile.TopTile = tile;
                    tile.BottomTile = bottomTile;
                }
                else if (TempPool.Any(o => o.row == (tile.row + 1) && o.col == tile.col && o.gameObject.activeInHierarchy))
                {
                    var bottomTile = TempPool.Single(o => o.row == (tile.row + 1) && o.col == tile.col && o.gameObject.activeInHierarchy);
                    bottomTile.TopTile = tile;
                    tile.BottomTile = bottomTile;
                }
                else
                    tile.BottomTile = null;

                if (Pool.Any(o => o.row == (tile.row - 1) && o.col == tile.col && o.gameObject.activeInHierarchy))
                {
                    var topTile = Pool.Single(o => o.row == (tile.row - 1) && o.col == tile.col && o.gameObject.activeInHierarchy);
                    topTile.BottomTile = tile;
                    tile.TopTile = topTile;
                }
                else if (TempPool.Any(o => o.row == (tile.row - 1) && o.col == tile.col && o.gameObject.activeInHierarchy))
                {
                    var topTile = TempPool.Single(o => o.row == (tile.row - 1) && o.col == tile.col && o.gameObject.activeInHierarchy);
                    topTile.BottomTile = tile;
                    tile.TopTile = topTile;
                }
                else
                    tile.TopTile = null;
            }
            else
            {
                tile.TopTile = null;
                tile.BottomTile = null;
            }

            #endregion
        }
    }
}
