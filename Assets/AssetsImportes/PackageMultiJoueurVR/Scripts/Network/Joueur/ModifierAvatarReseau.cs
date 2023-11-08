using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ModifierAvatarReseau : NetworkBehaviour
{
    [SerializeField] private GameObject[] _optionsTete;  // Contient les différentes parties pour la tête
    public GameObject[] OptionsTete => _optionsTete;

    [SerializeField] private GameObject[] _optionsBody; // même chose pour le corps
    public GameObject[] OptionsBody => _optionsBody;
    
    [SerializeField] private Renderer[] _partiesCorps; // Parties du corps qui vont changer de couleur
    [SerializeField] private Gradient _gradientCouleurPeau; // Gradient utiliser pour changer la couleur
    [SerializeField] private TextMeshPro _nomJoueur;  // Objet TMPro qui contient le nom du joueur

    [SerializeField] private GameObject _tete;
    [SerializeField] private GameObject _nom;

    public NetworkVariable<int> _teteIndex = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> _bodyIndex = new NetworkVariable<int>(0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> _couleurAvatar = new NetworkVariable<float>(0.0f,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);

    // Le type string ne peut être utiliser pour les variables réseau
    public NetworkVariable<FixedString128Bytes> _nomAvatar = new NetworkVariable<FixedString128Bytes>("test",
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Au départ on creer un Avatar aléatoire et le place dans la variable
            CreerAvatarAleatoire();
            // Pour le propriétaire de l'avatar je déactive la tete et le nom
            // Car le morceau de tete nuisse à la vue et le nom fais face à la
            // caméra vers le bas pour le joueur propriétaire.
            _tete.layer = 7;  // Ici je change plutôt le layer afin qu'il soit visible dans le mirroir
            _nom.SetActive(false);

            UISelectionAvatar.Instance.Initialize(this);
        }

        // On change l'apparence de l'Avatar en lui transmettant la variable crée
        UpdateAvatarFromData(_teteIndex.Value, _bodyIndex.Value, _couleurAvatar.Value, _nomAvatar.Value);

        // Si je détecte un changement sur une des parties j'appelle la méthode Update
        _teteIndex.OnValueChanged += (x, y) => UpdateAvatarFromData(y, _bodyIndex.Value, _couleurAvatar.Value, _nomAvatar.Value);
        _bodyIndex.OnValueChanged += (x, y) => UpdateAvatarFromData(_teteIndex.Value, y, _couleurAvatar.Value, _nomAvatar.Value);
        _couleurAvatar.OnValueChanged += (x, y) => UpdateAvatarFromData(_teteIndex.Value, _bodyIndex.Value, y, _nomAvatar.Value);
        _nomAvatar.OnValueChanged += (x, y) => UpdateAvatarFromData(_teteIndex.Value, _bodyIndex.Value, _couleurAvatar.Value, y);
    }

    public override void OnNetworkDespawn()
    {
        _teteIndex.OnValueChanged -= (x, y) => UpdateAvatarFromData(y, _bodyIndex.Value, _couleurAvatar.Value, _nomAvatar.Value);
        _bodyIndex.OnValueChanged -= (x, y) => UpdateAvatarFromData(_teteIndex.Value, y, _couleurAvatar.Value, _nomAvatar.Value);
        _couleurAvatar.OnValueChanged -= (x, y) => UpdateAvatarFromData(_teteIndex.Value, _bodyIndex.Value, y, _nomAvatar.Value);
        _nomAvatar.OnValueChanged -= (x, y) => UpdateAvatarFromData(_teteIndex.Value, _bodyIndex.Value, _couleurAvatar.Value, y);
    }

    public void CreerAvatarAleatoire()
    {
        _teteIndex.Value = Random.Range(0, _optionsTete.Length);
        _bodyIndex.Value = Random.Range(0, _optionsBody.Length);
        _couleurAvatar.Value = Random.Range(0.0f, 1.0f);
        _nomAvatar.Value = "Joueur #" + NetworkManager.Singleton.LocalClientId.ToString();
    }

    // Méthode qui met à jour l'avatar avec les données reçu en paramètre
    public void UpdateAvatarFromData(int p_teteIndex, int p_bodyIndex, float p_couleurAvatar, FixedString128Bytes p_nomAvatar)
    {
        // Mise en place de l'élément pour la tête
        for (int i = 0; i < _optionsTete.Length; i++)
        {
            _optionsTete[i].SetActive(i == p_teteIndex);
        }

        // Mise en place de l'élément pour le corps
        for (int i = 0; i < _optionsBody.Length; i++)
        {
            _optionsBody[i].SetActive(i == p_bodyIndex);
        }

        // Pour chaque parties du corps on change sa couleur par celle du gradient
        foreach (var item in _partiesCorps)
        {
            item.material.color = _gradientCouleurPeau.Evaluate(p_couleurAvatar);
        }

        // Change le nom avec le nom reçu
        _nomJoueur.text = p_nomAvatar.ToString();
    }
}
