using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    private static CharacterMoveController character;

    private void Start()
    {
        character = GameObject.Find("Character").GetComponent<CharacterMoveController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log("Hit");
        character.GameOver();

    }
}
