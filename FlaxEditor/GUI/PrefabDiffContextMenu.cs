// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using System;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI
{
    /// <summary>
    /// The custom context menu that shows a tree of prefa diff items.
    /// </summary>
    /// <seealso cref="ContextMenuBase" />
    public class PrefabDiffContextMenu : ContextMenuBase
    {
        /// <summary>
        /// The tree control where you should add your nodes.
        /// </summary>
        public readonly Tree Tree;

        /// <summary>
        /// The event called to revert all the changes applied.
        /// </summary>
        public event Action RevertAll;

        /// <summary>
        /// The event called to apply all the changes.
        /// </summary>
        public event Action ApplyAll;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrefabDiffContextMenu"/> class.
        /// </summary>
        /// <param name="width">The control width.</param>
        /// <param name="height">The control height.</param>
        public PrefabDiffContextMenu(float width = 280, float height = 260)
        {
            // Context menu dimensions
            Size = new Vector2(width, height);

            // Buttons
            float buttonsWidth = (width - 6.0f) * 0.5f;
            float buttonsHeight = 20.0f;
            var revertAll = new Button(2.0f, 2.0f, buttonsWidth, buttonsHeight);
            revertAll.Text = "Revert All";
            revertAll.Parent = this;
            revertAll.Clicked += () => RevertAll?.Invoke();
            var applyAll = new Button(revertAll.Right + 2.0f, 2.0f, buttonsWidth, buttonsHeight);
            applyAll.Text = "Apply All";
            applyAll.Parent = this;
            applyAll.Clicked += () => ApplyAll?.Invoke();

            // Actual panel
            var panel1 = new Panel(ScrollBars.Vertical)
            {
                Bounds = new Rectangle(0, applyAll.Bottom + 2.0f, Width, Height - applyAll.Bottom - 2.0f),
                Parent = this
            };
            Tree = new Tree
            {
                DockStyle = DockStyle.Top,
                IsScrollable = true,
                Parent = panel1
            };
        }

        /// <inheritdoc />
        protected override void OnShow()
        {
            // Prepare
            Focus();

            base.OnShow();
        }

        /// <inheritdoc />
        public override void Hide()
        {
            if (!Visible)
                return;

            Focus(null);

            base.Hide();
        }

        /// <inheritdoc />
        public override bool OnKeyDown(Keys key)
        {
            if (key == Keys.Escape)
            {
                Hide();
                return true;
            }

            return base.OnKeyDown(key);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            RevertAll = null;
            ApplyAll = null;

            base.Dispose();
        }
    }
}
