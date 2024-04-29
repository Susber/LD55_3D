using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PrecomputedCB", menuName = "ScriptableObjects/PrecomputedCBPieces", order = 1)]
public class PrecomputedCBPieces : ScriptableObject
{ 
	public int numCutVariations = 10;
	public int numPieces = 5;

	public List<List<Mesh>> pieces;
	public List<List<Mesh>> sides;

	private void OnEnable()
	{
		if (pieces == null) { pieces = new List<List<Mesh>>(); };
		if (sides == null) { sides = new List<List<Mesh>>(); };
	}

	public bool piecesAvailable()
	{
		return pieces.Count == numCutVariations && sides.Count == numCutVariations;
	}
}
