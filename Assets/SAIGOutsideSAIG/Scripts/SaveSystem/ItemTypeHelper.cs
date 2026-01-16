
using System;
using System.Data.Common;

namespace SaveSystem
{
    public enum ItemType
    {
        Scene,
        Passcode
    }
    public static class ItemTypeHelper
    {
        /// <summary>
        /// Extracts the ItemType and ID from a composite item ID
        /// Example: "SCENE_9a479cc4-388d-4b57-8453-ab93792a9c0b" returns (ItemType.Scene, "9a479cc4-388d-4b57-8453-ab93792a9c0b")
        /// </summary>
        public static (ItemType, string) ExtractIdFromItemType(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentException("Item ID cannot be null or empty");

            var parts = itemId.Split('_', 2); // Split on first underscore only
            if (parts.Length != 2)
                throw new ArgumentException($"Invalid item ID format: {itemId}");

            var typeString = parts[0];
            var id = parts[1];

            if (!Enum.TryParse<ItemType>(typeString, ignoreCase: true, out var itemType))
                throw new ArgumentException($"Unknown item type: {typeString}");

            return (itemType, id);
        }

        /// <summary>
        /// Extracts just the ItemType from a composite item ID
        /// </summary>
        public static ItemType ExtractItemType(string itemId)
        {
            return ExtractIdFromItemType(itemId).Item1;
        }

        /// <summary>
        /// Extracts just the ID (without the type prefix) from a composite item ID
        /// </summary>
        public static string ExtractId(string itemId)
        {
            return ExtractIdFromItemType(itemId).Item2;
        }
    }
}
