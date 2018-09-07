// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI.Drag
{
    public abstract class DragHelper
    {
        public abstract bool HasValidDrag { get; }
        public abstract DragDropEffect Effect { get; }
        public abstract bool OnDragEnter(DragData data);
        public abstract void OnDragLeave();
        public abstract void OnDragDrop();
    }

    /// <summary>
    /// Base class for drag and drop operation helpers.
    /// </summary>
    /// <typeparam name="T">Type of the objects to collect from drag data.</typeparam>
    /// <typeparam name="U">Type of the drag-drop event.</typeparam>
    public abstract class DragHelper<T, U> : DragHelper where U : DragEventArgs
    {
        /// <summary>
        /// The objects gathered.
        /// </summary>
        public readonly List<T> Objects = new List<T>();

        /// <summary>
        /// Gets a value indicating whether this instance has valid drag data.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has valid drag data; otherwise, <c>false</c>.
        /// </value>
        public sealed override bool HasValidDrag => Objects.Count > 0;

        /// <summary>
        /// Gets the current drag effect.
        /// </summary>
        /// <value>
        /// The effect.
        /// </value>
        public override DragDropEffect Effect => HasValidDrag ? DragDropEffect.Move : DragDropEffect.None;

        /// <summary>
        /// The validation function
        /// </summary>
        protected Func<T, bool> ValidateFunction { get; set; }

        /// <summary>
        /// Creates a new DragHelper
        /// </summary>
        /// <param name="validateFunction">The validation function</param>
        protected DragHelper(Func<T, bool> validateFunction)
        {
            ValidateFunction = validateFunction;
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="item">An item.</param>
        /// <returns>The data.</returns>
        public abstract DragData ToDragData(T item);

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The data.</returns>
        public abstract DragData ToDragData(IEnumerable<T> items);

        /// <summary>
        /// Tries to parse the drag data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Gathered objects or empty IEnumerable if cannot get any valid.</returns>
        public abstract IEnumerable<T> FromDragData(DragData data);

        public abstract void DragDrop(U dragEventArgs, IEnumerable<T> item);

        /// <summary>
        /// Invalids the drag data.
        /// </summary>
        public void InvalidDrag()
        {
            Objects.Clear();
        }

        /// <summary>
        /// Called when drag enters.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>True if drag event is valid and can be performed, otherwise false.</returns>
        public sealed override bool OnDragEnter(DragData data)
        {
            if (data == null || ValidateFunction == null)
                throw new ArgumentNullException();

            Objects.Clear();

            var items = FromDragData(data);
            foreach (var item in items)
            {
                if (ValidateFunction(item))
                    Objects.Add(item);
            }

            return HasValidDrag;
        }

        /// <summary>
        /// Called when drag leaves.
        /// </summary>
        public override void OnDragLeave()
        {
            Objects.Clear();
        }

        /// <summary>
        /// Called when drag drops.
        /// </summary>
        public sealed override void OnDragDrop()
        {
            if (HasValidDrag) DragDrop(null, Objects);
            Objects.Clear();
        }

        public void OnDragDrop(U dragEventArgs)
        {
            if (HasValidDrag) DragDrop(dragEventArgs, Objects);
            Objects.Clear();
        }
    }
}
