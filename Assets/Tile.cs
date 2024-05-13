using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{

    [SerializeField] public GameObject _x;
    [SerializeField] public GameObject _o;
    public int row, column;
    public static UnityEvent<Tile> OnTileClicked = new UnityEvent<Tile>();

    private void OnMouseDown()
    {
        OnTileClicked.Invoke(this);
    }
}
