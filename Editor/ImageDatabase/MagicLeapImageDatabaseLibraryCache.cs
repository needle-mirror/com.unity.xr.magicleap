using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    /// <summary>
    /// Class representing a single Image Database Entry.
    /// It essentially is a pair of an Asset Guid (stored as a string) and a Timestamp
    /// </summary>
    [Serializable]
    public class ImageDatabaseEntry
    {
        /// <summary>
        /// Asset GUID
        /// </summary>
        public string assetGuid;
        [SerializeField]
        private long _timestamp;

        /// <summary>
        /// Getter/Setter to map the 'DateTime' we store for the entry vs what we can serialize
        ///  - note that the JSONUtility class cannot serialize a DateTime
        /// </summary>
        public DateTime timeStamp
        {
            get
            {
                return DateTime.FromFileTimeUtc(_timestamp);
            }
            set
            {
                _timestamp = value.ToFileTimeUtc();
            }
        }
    }

    /// <summary>
    /// A scriptable object that is used to cache image library binary blob
    /// generation data to prevent rebuilds everytime the user presses play in
    /// the editor.
    /// </summary>
    [Serializable]
    /// <summary>
    /// Class representing a list of `ImageDatabaseEntry`
    /// </summary>

    public class MagicLeapImageDatabaseLibraryCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MagicLeapImageDatabaseLibraryCache()
        {
            m_LibraryCache = new List<ImageDatabaseEntry>(25);
        }

        /// <summary>
        /// A simple list of ImageDatabaseEntry objects.
        /// </summary>
        public List<ImageDatabaseEntry> m_LibraryCache;
    }
}
