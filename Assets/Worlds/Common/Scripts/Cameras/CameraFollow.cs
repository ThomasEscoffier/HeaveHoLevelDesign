using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public GameObject ObjToFollow;
	public float OffsetX;
	public float OffsetY;

    void Update () {
        if (ObjToFollow == null)
        {
            ObjToFollow = GameManager.Instance.GetPlayers()[0].GetCurrentCharacter().gameObject;
        }
        transform.position = new Vector3 (ObjToFollow.transform.position.x + OffsetX, ObjToFollow.transform.position.y + OffsetY, transform.position.z);
	}
}
