// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;
using FlaxEngine;
using Object = FlaxEngine.Object;

namespace FlaxEditor.Actions
{
    /// <summary>
    /// Implementation of <see cref="IUndoAction"/> used to break/restore <see cref="Prefab"/> connection for the collection of <see cref="Actor"/> and <see cref="Script"/> objects.
    /// </summary>
    /// <seealso cref="IUndoAction" />
    public sealed class BreakPrefabLinkAction : IUndoAction
    {
        private readonly bool _isBreak;
        private Guid _actorId;
        private Guid _prefabId;
        private Dictionary<Guid, Guid> _prefabObjectIds;

        internal BreakPrefabLinkAction(bool isBreak, Guid actorId, Guid prefabId)
        {
            _isBreak = isBreak;
            _actorId = actorId;
            _prefabId = prefabId;
        }

        /// <summary>
        /// Creates a new undo action that in state for breaking prefab connection.
        /// </summary>
        /// <param name="actor">The target actor.</param>
        /// <returns>The action.</returns>
        public static BreakPrefabLinkAction Break(Actor actor)
        {
            if (actor == null)
                throw new ArgumentNullException(nameof(actor));
            return new BreakPrefabLinkAction(true, actor.id, Guid.Empty);
        }

        /// <summary>
        /// Creates a new undo action that in state for breaking prefab connection.
        /// </summary>
        /// <param name="actor">The target actor.</param>
        /// <returns>The action.</returns>
        public static BreakPrefabLinkAction Link(Actor actor)
        {
            if (actor == null)
                throw new ArgumentNullException(nameof(actor));
            return new BreakPrefabLinkAction(false, actor.id, actor.PrefabID);
        }

        /// <inheritdoc />
        public string ActionString => _isBreak ? "Break prefab link" : "Link prefab";

        /// <inheritdoc />
        public void Do()
        {
            if (_isBreak)
                DoBreak();
            else
                DoLink();
        }

        /// <inheritdoc />
        public void Undo()
        {
            if (_isBreak)
                DoLink();
            else
                DoBreak();
        }

        private void DoLink()
        {
            if (_prefabObjectIds == null)
                throw new FlaxException("Cannot link prefab. Missing objects Ids mapping.");

            var actor = Object.Find<Actor>(ref _actorId);
            if (actor == null)
                throw new FlaxException("Cannot link prefab. Missing actor.");

            // Restore cached links
            foreach (var e in _prefabObjectIds)
            {
                var objId = e.Key;
                var prefabObjId = e.Value;

                var obj = Object.Find<Object>(ref objId);
                if (obj is Actor)
                {
                    Actor.Internal_LinkPrefab(obj.unmanagedPtr, ref _prefabId, ref prefabObjId);
                }
                else if (obj is Script)
                {
                    Script.Internal_LinkPrefab(obj.unmanagedPtr, ref prefabObjId);
                }
            }

            Editor.Instance.Scene.MarkSceneEdited(actor.Scene);
        }

        private void CollectIds(Actor actor)
        {
            _prefabObjectIds.Add(actor.ID, actor.PrefabObjectID);

            for (int i = 0; i < actor.ChildrenCount; i++)
            {
                CollectIds(actor.GetChild(i));
            }

            for (int i = 0; i < actor.ScriptsCount; i++)
            {
                var script = actor.GetScript(i);
                _prefabObjectIds.Add(script.ID, script.PrefabObjectID);
            }
        }

        private void DoBreak()
        {
            var actor = Object.Find<Actor>(ref _actorId);
            if (actor == null)
                throw new FlaxException("Cannot break prefab link. Missing actor.");
            if (!actor.HasPrefabLink)
                throw new FlaxException("Cannot break missing prefab link.");

            // Note: this code assumes that all objects are using the same prefab asset

            // Cache prefab objects ids to restore them on undo
            if (_prefabObjectIds == null)
                _prefabObjectIds = new Dictionary<Guid, Guid>(1024);
            else
                _prefabObjectIds.Clear();
            CollectIds(actor);

            _prefabId = actor.PrefabID;
            actor.BreakPrefabLink();

            Editor.Instance.Scene.MarkSceneEdited(actor.Scene);
        }
    }
}
