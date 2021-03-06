// Copyright (c) 2012-2020 Wojciech Figat. All rights reserved.

using System;
using System.IO;
using System.Text;

namespace FlaxEditor.Content
{
    /// <summary>
    /// Asset item object.
    /// </summary>
    /// <seealso cref="FlaxEditor.Content.ContentItem" />
    public abstract class AssetItem : ContentItem
    {
        /// <summary>
        /// Gets the asset unique identifier.
        /// </summary>
        public Guid ID { get; protected set; }

        /// <summary>
        /// Gets the asset type identifier.
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetItem"/> class.
        /// </summary>
        /// <param name="path">The asset path.</param>
        /// <param name="typeName">The asset type name.</param>
        /// <param name="id">The asset identifier.</param>
        protected AssetItem(string path, string typeName, ref Guid id)
        : base(path)
        {
            TypeName = typeName;
            ID = id;
        }

        /// <inheritdoc />
        protected override void UpdateTooltipText()
        {
            var sb = new StringBuilder();
            sb.Append("Type: ").Append(TypeName).AppendLine();
            sb.Append("Size: ").Append(Utilities.Utils.FormatBytesCount((int)new FileInfo(Path).Length)).AppendLine();
            sb.Append("Path: ").Append(Path).AppendLine();
            TooltipText = sb.ToString();
        }

        /// <inheritdoc />
        public override ContentItemType ItemType => ContentItemType.Asset;

        /// <summary>
        /// Determines whether asset is of the specified type (included inheritance checks).
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <returns><c>true</c> if asset is of the specified type (including inherited types); otherwise, <c>false</c>.</returns>
        public bool IsOfType<T>()
        {
            return IsOfType(typeof(T));
        }

        /// <summary>
        /// Determines whether asset is of the specified type (included inheritance checks).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if asset is of the specified type (including inherited types); otherwise, <c>false</c>.</returns>
        public virtual bool IsOfType(Type type)
        {
            return false;
        }

        /// <inheritdoc />
        protected override bool DrawShadow => true;

        /// <inheritdoc />
        public override ContentItem Find(Guid id)
        {
            return id == ID ? this : null;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Path + ":" + ID;
        }
    }
}
