﻿//
//  ItemComponent.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2015 Fin Christensen
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using FreezingArcher.Math;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Item class describing an item for use in an inventory and placement in a map.
    /// </summary>
    public sealed class ItemComponent : EntityComponent
    {
        #region defaults

        /// <summary>
        /// The default location.
        /// </summary>
        public static readonly ItemLocation DefaultLocation = ItemLocation.Ground;

        /// <summary>
        /// The default attack classes.
        /// </summary>
        public static readonly AttackClass DefaultAttackClasses = AttackClass.Nothing;

        /// <summary>
        /// The default item usages.
        /// </summary>
        public static readonly ItemUsage DefaultItemUsages = ItemUsage.Placebo;

        /// <summary>
        /// The default protection.
        /// </summary>
        public static readonly Protection DefaultProtection = new Protection();

        /// <summary>
        /// The default size.
        /// </summary>
        public static readonly Vector2i DefaultSize = Vector2i.Zero;

        /// <summary>
        /// The default health delta.
        /// </summary>
        public static readonly float DefaultHealthDelta = 0;

        /// <summary>
        /// The default attack strength.
        /// </summary>
        public static readonly float DefaultAttackStrength = 0;

        /// <summary>
        /// The default throw power.
        /// </summary>
        public static readonly float DefaultThrowPower = 0;

        /// <summary>
        /// The default usage.
        /// </summary>
        public static readonly float DefaultUsage = 0;

        #endregion

        /// <summary>
        /// Gets or sets the abstract locations of this item such as ground, wall or inventory.
        /// </summary>
        /// <value>The location.</value>
        public ItemLocation Location { get; set; }

        /// <summary>
        /// Gets the attack classes of this item such as enemy or object.
        /// </summary>
        /// <value>The attack classes.</value>
        public AttackClass AttackClasses { get; set; }

        /// <summary>
        /// Gets the item usages of this item such as eatable, throwable or usable.
        /// </summary>
        /// <value>The item usages.</value>
        public ItemUsage ItemUsages { get; set; }

        /// <summary>
        /// Gets the protection this item applies to the entity.
        /// </summary>
        /// <value>The applied protection.</value>
        public Protection Protection { get; set; }

        /// <summary>
        /// Gets the size of this item for use in an inventory.
        /// </summary>
        /// <value>The size.</value>
        public Vector2i Size { get; set; }

        /// <summary>
        /// Gets the health delta applied when this item is used.
        /// </summary>
        /// <value>The health delta.</value>
        public float HealthDelta { get; set; }

        /// <summary>
        /// Gets the attack strength applied when this item is used.
        /// </summary>
        /// <value>The attack strength.</value>
        public float AttackStrength { get; set; }

        /// <summary>
        /// Gets the throw power applied when this item is thrown.
        /// </summary>
        /// <value>The throw power.</value>
        public float ThrowPower { get; set; }

        /// <summary>
        /// Gets or sets the usage describing how much of this item is already used. The value range is from 0 to 1.
        /// </summary>
        /// <value>The usage value.</value>
        public float Usage { get; set; }
    }
}