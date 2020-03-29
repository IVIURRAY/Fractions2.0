using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
	[SerializeField]
	private bool isAqurired = false;

	public bool IsAqurired { get => isAqurired; set => isAqurired = value; }
}
