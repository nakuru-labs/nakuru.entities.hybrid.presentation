using Unity.Entities;
using UnityEngine;

namespace View
{

	public class EntityRef : MonoBehaviour
	{
		public Entity Value { get; internal set; }
	}

}