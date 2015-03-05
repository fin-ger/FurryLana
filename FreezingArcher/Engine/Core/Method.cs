﻿//
//  Method.cs
//
//  Author:
//       martin <>
//
//  Copyright (c) 2015 martin
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
using System;

namespace FreezingArcher.Core.Interfaces
{
    public class Method
    {
        public string Name { get; private set;}
        public Delegate Implementation { get; private set;}
        public Attribute[] Attributes {get;private set;}
        public Method (string name, Delegate implementation, params Attribute[] attributes)
        {
            Name = name;
            Implementation = implementation;
            Attributes = attributes;
        }
    }
}
