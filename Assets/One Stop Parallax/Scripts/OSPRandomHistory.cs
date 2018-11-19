using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace OSP
{
    /// <summary>
    /// Class OSPRandomHistory. This contains all variables and functions needed to generate,
    /// store, and retrieve position and offset histories. 
    /// </summary>
    public class OSPRandomHistory
    {
        /// <summary>
        /// Gets or sets the identifier for a given layer's objects. This allows us to only get history for our current object's layer.
        /// </summary>
        /// <value>The layer's object identifier.</value>
        public int ID { get; set; }
        /// <summary>
        /// Gets or sets the maximum size of the position/offset history.
        /// </summary>
        /// <value>The maximum history size.</value>
        public int MaxHistorySize { get; set; }

        /// <summary>
        /// List containing all of the X offsets for objects that tile horizontally.
        /// </summary>
        public List<float> xOffsets;
        /// <summary>
        /// List containing all of the Y offsets for objects that tile vertically.
        /// </summary>
        public List<float> yOffsets;

        /// <summary>
        /// List containing all of the X offsets for objects that tile vertically.
        /// </summary>
        public List<float> xVerticalOffsets;
        /// <summary>
        /// List containing all of the Y offsets for objects that tile horizontally.
        /// </summary>
        public List<float> yHorizontalOffsets;

        /// <summary>
        /// Determines whether the layer uses random X offsets for horizontal tiles.
        /// </summary>
        public bool UseRandomX = false;
        /// <summary>
        /// The minimum x offset for horizontal tiles.
        /// </summary>
        public float MinXOffset = 0;
        /// <summary>
        /// The maximum x offset for horizontal tiles.
        /// </summary>
        public float MaxXOffset = 0;

        /// <summary>
        /// Determines whether the layer uses random Y offsets for vertical tiles.
        /// </summary>
        public bool UseRandomY = false;
        /// <summary>
        /// The minimum y offset for vertical tiles.
        /// </summary>
        public float MinYOffset = 0;
        /// <summary>
        /// The maximum y offset for vertical tiles.
        /// </summary>
        public float MaxYOffset = 0;

        /// <summary>
        /// Determines whether the layer uses random X offsets for vertical tiles.
        /// </summary>
        public bool UseRandomVerticalX = false;
        /// <summary>
        /// The minimum x offset for vertical tiles.
        /// </summary>
        public float MinVerticalXOffset = 0;
        /// <summary>
        /// The maximum x offset for vertical tiles.
        /// </summary>
        public float MaxVerticalXOffset = 0;

        /// <summary>
        /// Determines whether the layer uses random Y offsets for horizontal tiles.
        /// </summary>
        public bool UseRandomHorizontalY = false;
        /// <summary>
        /// The minimum y offset for horizontal tiles.
        /// </summary>
        public float MinHorizontalYOffset = 0;
        /// <summary>
        /// The maximum y offset for horizontal tiles.
        /// </summary>
        public float MaxHorizontalYOffset = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="OSPRandomHistory"/> class. Initializes offset lists.
        /// </summary>
        public OSPRandomHistory()
        {
            xOffsets = new List<float>();
            yOffsets = new List<float>();
            xVerticalOffsets = new List<float>();
            yHorizontalOffsets = new List<float>();
        }

        /// <summary>
        /// Initializes the offset lists with random numbers between the provided minimum and maximum numbers.
        /// </summary>
        public void InitializeOffsetHistory()
        {
            for(int i = 0; i < MaxHistorySize; ++i)
            {
                if (UseRandomX)
                {
                    xOffsets.Add(Random.Range(MinXOffset, MaxXOffset));

                    if(UseRandomHorizontalY)
                        xVerticalOffsets.Add(Random.Range(MinHorizontalYOffset, MaxHorizontalYOffset));
                }

                if (UseRandomY)
                {
                    yOffsets.Add(Random.Range(MinYOffset, MaxYOffset));

                    if (UseRandomVerticalX)
                        yHorizontalOffsets.Add(Random.Range(MinVerticalXOffset, MaxVerticalXOffset));
                }
            }
        }

        /// <summary>
        /// Gets the x offset for a horizontally tiled object.
        /// </summary>
        /// <param name="col">The column of the object.</param>
        /// <param name="useRandomHistory">if set to <c>true</c> [use random history].</param>
        /// <returns>System.Single.</returns>
        public float GetXOffsetForColumn(int col, bool useRandomHistory)
        {
            // If we're not using random history, then just return a random number
            if (!useRandomHistory)
                return Random.Range(MinXOffset, MaxXOffset);

            int index = 0;

            // If our column is within the bounds of the array, grab the value at that location
            if (col >= 0 && col <= (MaxHistorySize - 1))
                return xOffsets[col];
            else if (col < 0)
            {
                // If our column is negative, traverse the array backwards that many times to find our random number
                int absVal = (int)Mathf.Abs(col);
                int divisible = absVal / MaxHistorySize;
                int subbed = absVal - (divisible * MaxHistorySize);
                index = MaxHistorySize - subbed - 1;

            }
            else if (col >= MaxHistorySize)
            {
                // If our column is greater than the array's size, traverse the array that many times to find our random number
                int divisible = col / MaxHistorySize;
                int subbed = col - (divisible * MaxHistorySize);
                index = subbed;
            }

            return xOffsets[index];
        }

        /// <summary>
        /// Gets the y offset for a horizontally tiled object.
        /// </summary>
        /// <param name="col">The column of the object.</param>
        /// <param name="useRandomHistory">if set to <c>true</c> [use random history].</param>
        /// <returns>System.Single.</returns>
        public float GetYOffsetForColumn(int col, bool useRandomHistory)
        {
            // If we're not using random history, then just return a random number
            if (!useRandomHistory)
                return Random.Range(MinHorizontalYOffset, MaxHorizontalYOffset);

            int index = 0;

            // If our column is within the bounds of the array, grab the value at that location
            if (col >= 0 && col <= (MaxHistorySize - 1))
                return xVerticalOffsets[col];
            else if (col < 0)
            {
                // If our column is negative, traverse the array backwards that many times to find our random number
                int absVal = (int)Mathf.Abs(col);
                int divisible = absVal / MaxHistorySize;
                int subbed = absVal - (divisible * MaxHistorySize);
                index = MaxHistorySize - subbed - 1;

            }
            else if (col >= MaxHistorySize)
            {
                // If our column is greater than the array's size, traverse the array that many times to find our random number
                int divisible = col / MaxHistorySize;
                int subbed = col - (divisible * MaxHistorySize);
                index = subbed;
            }

            return xVerticalOffsets[index];
        }

        /// <summary>
        /// Gets the y offset for a vertically tiled object.
        /// </summary>
        /// <param name="row">The row of the object.</param>
        /// <param name="useRandomHistory">if set to <c>true</c> [use random history].</param>
        /// <returns>System.Single.</returns>
        public float GetYOffsetForRow(int row, bool useRandomHistory)
        {
            // If we're not using random history, then just return a random number
            if (!useRandomHistory)
                return Random.Range(MinYOffset, MaxYOffset);

            int index = 0;

            // If our row is within the bounds of the array, grab the value at that location
            if (row >= 0 && row <= (MaxHistorySize - 1))
                return yOffsets[row];
            else if (row < 0)
            {
                // If our row is negative, traverse the array backwards that many times to find our random number
                int absVal = (int)Mathf.Abs(row);
                int divisible = absVal / MaxHistorySize;
                int subbed = absVal - (divisible * MaxHistorySize);
                index = MaxHistorySize - subbed - 1;

            }
            else if (row >= MaxHistorySize)
            {
                // If our row is greater than the array's size, traverse the array that many times to find our random number
                int divisible = row / MaxHistorySize;
                int subbed = row - (divisible * MaxHistorySize);
                index = subbed;
            }

            return yOffsets[index];
        }

        /// <summary>
        /// Gets the x offset for a vertically tiled object.
        /// </summary>
        /// <param name="row">The row of the object.</param>
        /// <param name="useRandomHistory">if set to <c>true</c> [use random history].</param>
        /// <returns>System.Single.</returns>
        public float GetXOffsetForRow(int row, bool useRandomHistory)
        {
            // If we're not using random history, then just return a random number
            if (!useRandomHistory)
                return Random.Range(MinVerticalXOffset, MaxVerticalXOffset);

            int index = 0;

            // If our row is within the bounds of the array, grab the value at that location
            if (row >= 0 && row <= (MaxHistorySize - 1))
                return yHorizontalOffsets[row];
            else if (row < 0)
            {
                // If our row is negative, traverse the array backwards that many times to find our random number
                int absVal = (int)Mathf.Abs(row);
                int divisible = absVal / MaxHistorySize;
                int subbed = absVal - (divisible * MaxHistorySize);
                index = MaxHistorySize - subbed - 1;

            }
            else if (row >= MaxHistorySize)
            {
                // If our row is greater than the array's size, traverse the array that many times to find our random number
                int divisible = row / MaxHistorySize;
                int subbed = row - (divisible * MaxHistorySize);
                index = subbed;
            }

            return yHorizontalOffsets[index];
        }
    }
}