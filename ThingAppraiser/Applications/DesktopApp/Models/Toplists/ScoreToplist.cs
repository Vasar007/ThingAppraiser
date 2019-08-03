﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal class ScoreToplist : ToplistBase
    {
        private Dictionary<int, ToplistBlock> _blocks = new Dictionary<int, ToplistBlock>();


        public ScoreToplist(string name, ToplistFormat format)
            : base(name, ToplistType.Score, format)
        {
        }

        #region ToplistBase Overriden Methods

        public override bool AddBlock(ToplistBlock block)
        {
            block.ThrowIfNull(nameof(block));

            Blocks.Insert(block.Number - 1, block);
            _blocks.Add(block.Number, block);

            return true;
        }

        public override bool RemoveBlock(ToplistBlock block)
        {
            block.ThrowIfNull(nameof(block));

            bool internalRemove = _blocks.Remove(block.Number);
            bool baseRemove = Blocks.Remove(block);

            if (internalRemove == baseRemove) return internalRemove;

            throw new InvalidOperationException("Removal operation in this and base class has " +
                                                "different results.");
        }

        public override void UpdateBlocks(IEnumerable<ToplistBlock> blocks)
        {
            blocks.ThrowIfNull(nameof(blocks));

            base.UpdateBlocks(blocks);

            _blocks = blocks.ToDictionary(block => block.Number, block => block);
        }

        #endregion
    }
}