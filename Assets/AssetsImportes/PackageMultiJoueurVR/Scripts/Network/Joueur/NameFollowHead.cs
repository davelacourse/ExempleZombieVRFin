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
        // D�place le nom par rapport � la position de la t�te
        transform.position = _tete.position + Vector3.up * _verticalOffset;

        //Effectue la rotation en Y du nom pour toujours �tre face � la cam�ra
        transform.LookAt(_playerCamera, Vector3.up);
    }
}
