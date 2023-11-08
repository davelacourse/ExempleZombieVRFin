using UnityEngine;

public class BodyFollowHead : MonoBehaviour
{
    [SerializeField] private Transform _tete = default;
    [SerializeField] float _verticalOffset;

    private void Update()
    {
        // Déplace le body par rapport à la position de la tête
        transform.position = _tete.position + Vector3.up * _verticalOffset;

        //Effectue la rotation en Y du body dans la même direction que la tête
        transform.eulerAngles = new Vector3(0f, _tete.eulerAngles.y, 0f);
    }
}
