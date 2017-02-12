using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Rotorz.Extras.Collections;


public sealed class EditableTileTypesDictionary : EditableEntry<TileTypesDictionary>
{
}

[Serializable, EditableEntry(typeof(TileTypesDictionary))]
public sealed class TileTypesDictionary : OrderedDictionary<string, GameObject>
{
}
