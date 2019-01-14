// Copyright (c) 2012-2019 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using FlaxEditor.Surface.Elements;
using FlaxEditor.Utilities;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.Surface.ContextMenu
{
    /// <summary>
    /// Single item used for <see cref="VisjectCM"/>. Represents type of the <see cref="NodeArchetype"/> that can be spawned.
    /// </summary>
    /// <seealso cref="FlaxEngine.GUI.Control" />
    public sealed class VisjectCMItem : Control
    {
        private bool _isMouseDown;
        private List<Rectangle> _highlights;
        private GroupArchetype _groupArchetype;
        private NodeArchetype _archetype;

        /// <summary>
        /// Gets the item's group
        /// </summary>
        /// <value>
        /// The group of the item
        /// </value>
        public VisjectCMGroup Group { get; }

        /// <summary>
        /// Gets the group archetype.
        /// </summary>
        /// <value>
        /// The group archetype.
        /// </value>
        public GroupArchetype GroupArchetype => _groupArchetype;

        /// <summary>
        /// Gets the node archetype.
        /// </summary>
        /// <value>
        /// The node archetype.
        /// </value>
        public NodeArchetype NodeArchetype => _archetype;

        /// <summary>
        /// Gets and sets the data of the node.
        /// </summary>
        /// <value>
        /// The data of the node.
        /// </value>
        public object[] Data { get; set; }

        /// <summary>
        /// A computed score for the context menu order
        /// </summary>
        public float SortScore { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisjectCMItem"/> class.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="groupArchetype">The group archetype.</param>
        /// <param name="archetype">The archetype.</param>
        public VisjectCMItem(VisjectCMGroup group, GroupArchetype groupArchetype, NodeArchetype archetype)
        : base(0, 0, 120, 12)
        {
            Group = group;
            _groupArchetype = groupArchetype;
            _archetype = archetype;
        }

        /// <summary>
        /// Updates the <see cref="SortScore"/>
        /// </summary>
        /// <param name="selectedBox">The currently user-selected box</param>
        public void UpdateScore(Box selectedBox)
        {
            // Start off by resetting the score!
            SortScore = 0;

            if (selectedBox == null) return;
            if (!(_highlights?.Count > 0)) return;
            if (!Visible) return;

            if (CanConnectTo(selectedBox, NodeArchetype))
            {
                SortScore += 1;
            }

            if (Data != null)
            {
                SortScore += 1;
            }
        }

        private bool CanConnectTo(Box startBox, NodeArchetype nodeArchetype)
        {
            if (startBox == null) return false;
            if (!startBox.IsOutput) return false; // For now, I'm only handing the output box case

            for (int i = 0; i < nodeArchetype.Elements.Length; i++)
            {
                if (nodeArchetype.Elements[i].Type == NodeElementType.Input &&
                    startBox.CanUseType(nodeArchetype.Elements[i].ConnectionsType))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Updates the filter.
        /// </summary>
        /// <param name="filterText">The filter text.</param>
        public void UpdateFilter(string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
            {
                // Clear filter
                _highlights?.Clear();
                Visible = true;
            }
            else
            {
                object[] data;

                QueryFilterHelper.Range[] ranges;
                if (QueryFilterHelper.Match(filterText, _archetype.Title, out ranges))
                {
                    // Update highlights
                    if (_highlights == null)
                        _highlights = new List<Rectangle>(ranges.Length);
                    else
                        _highlights.Clear();
                    var style = Style.Current;
                    var font = style.FontSmall;
                    for (int i = 0; i < ranges.Length; i++)
                    {
                        var start = font.GetCharPosition(_archetype.Title, ranges[i].StartIndex);
                        var end = font.GetCharPosition(_archetype.Title, ranges[i].EndIndex);
                        _highlights.Add(new Rectangle(start.X + 2, 0, end.X - start.X, Height));
                    }
                    Visible = true;
                }
                else if (_archetype.AlternativeTitles?.Any(filterText.Equals) == true)
                {
                    // Update highlights
                    if (_highlights == null)
                        _highlights = new List<Rectangle>(1);
                    else
                        _highlights.Clear();
                    var style = Style.Current;
                    var font = style.FontSmall;
                    var start = font.GetCharPosition(_archetype.Title, 0);
                    var end = font.GetCharPosition(_archetype.Title, _archetype.Title.Length - 1);
                    _highlights.Add(new Rectangle(start.X + 2, 0, end.X - start.X, Height));
                    Visible = true;
                }
                else if (NodeArchetype.TryParseText != null && NodeArchetype.TryParseText(filterText, out data))
                {
                    // Update highlights
                    if (_highlights == null)
                        _highlights = new List<Rectangle>(1);
                    else
                        _highlights.Clear();
                    var style = Style.Current;
                    var font = style.FontSmall;
                    var start = font.GetCharPosition(_archetype.Title, 0);
                    var end = font.GetCharPosition(_archetype.Title, _archetype.Title.Length - 1);
                    _highlights.Add(new Rectangle(start.X + 2, 0, end.X - start.X, Height));
                    Visible = true;

                    Data = data;
                }
                else
                {
                    // Hide
                    _highlights?.Clear();
                    Visible = false;
                }
            }
        }

        /// <inheritdoc />
        public override void Draw()
        {
            var style = Style.Current;
            var rect = new Rectangle(Vector2.Zero, Size);

            // Overlay
            if (IsMouseOver)
                Render2D.FillRectangle(rect, style.BackgroundHighlighted);

            if (Group.ContextMenu.SelectedItem == this)
                Render2D.FillRectangle(rect, style.BackgroundSelected);

            // Draw all highlights
            if (_highlights != null)
            {
                var color = style.ProgressNormal * 0.6f;
                for (int i = 0; i < _highlights.Count; i++)
                    Render2D.FillRectangle(_highlights[i], color);
            }

            // Draw name
            Render2D.DrawText(style.FontSmall, (SortScore > 0.1f ? "> " : "") + _archetype.Title, new Rectangle(2, 0, rect.Width - 4, rect.Height), Enabled ? style.Foreground : style.ForegroundDisabled, TextAlignment.Near, TextAlignment.Center);
        }

        /// <inheritdoc />
        public override bool OnMouseDown(Vector2 location, MouseButton buttons)
        {
            if (buttons == MouseButton.Left)
            {
                _isMouseDown = true;
            }

            return base.OnMouseDown(location, buttons);
        }

        /// <inheritdoc />
        public override bool OnMouseUp(Vector2 location, MouseButton buttons)
        {
            if (buttons == MouseButton.Left && _isMouseDown)
            {
                _isMouseDown = false;
                Group.ContextMenu.OnClickItem(this);
            }

            return base.OnMouseUp(location, buttons);
        }

        /// <inheritdoc />
        public override void OnMouseLeave()
        {
            _isMouseDown = false;

            base.OnMouseLeave();
        }

        /// <inheritdoc />
        public override int Compare(Control other)
        {
            if (other is VisjectCMItem otherItem)
            {
                int order = -1 * SortScore.CompareTo(otherItem.SortScore);
                if (order == 0)
                {
                    order = string.Compare(_archetype.Title, otherItem._archetype.Title, StringComparison.Ordinal);
                }
                return order;
            }
            return base.Compare(other);
        }
    }
}
