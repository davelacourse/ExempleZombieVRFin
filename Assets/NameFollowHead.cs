using UnityEngine;

public class NameFollowHead : MonoBehaviour
{
    [SerializeField] private Transform _tete = default;
    [SerializeField] float _verticalOffset;

    private Transform _playerCamera;

    private void Start()
    {
        _playerCamera = Camera.main.transform;
    }

    private void Update()
    {
        // Déplace le nom par rapport à la position de la tête
        transform.position = _tete.position + Vector3.up * _verticalOffset;

        //Effectue la rotation en Y du nom pour toujours être face à la caméra
        transform.LookAt(_playerCamera, Vector3.up);
    }
}
