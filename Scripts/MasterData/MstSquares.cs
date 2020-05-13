using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class MstSquares : ScriptableObject
{
	//public List<EntityType> Entities; // Replace 'EntityType' to an actual type that is serializable.
	public List<MstSquaresDef> entities;
}
