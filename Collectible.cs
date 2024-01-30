using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string objectName;
    public string objectDescription;
    public Enums.ItemType type;
    public int value;

    [Tooltip("Preencher apenas se a vari�vel [type] for populada como [Ammu]")]
    public string weaponName;
}