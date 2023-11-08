using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ModifierAvatarReseau : NetworkBehaviour
{
    [SerializeField] private GameObject[] _optionsTete;  // Contient les diff�rentes parties pour la t�te
    public GameObject[] OptionsTete => _optionsTete;

    [SerializeField] private GameObject[] _optionsBody; // m�me chose pour le corps
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

    // Le type string ne peut �tre utiliser pour les variables r�seau
    public NetworkVariable<FixedString128Bytes> _nomAvatar = new NetworkVariable<FixedString128Bytes>("test",
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Au d�part on creer un Avatar al�atoire et le place dans la variable
            CreerAvatarAleatoire();
            // Pour le propri�taire de l'avatar je d�active la tete et le nom
            // Car le morceau de tete nuisse � la vue et le nom fais face � la
            // cam�ra vers le bas pour le joueur propri�taire.
            _tete.layer = 7;  // Ici je change plut�t le layer afin qu'il soit visible dans le mirroir
            _nom.SetActive(false);

            UISelectionAvatar.Instance.Initialize(this);
        }

        // On change l'apparence de l'Avatar en lui transmettant la variable cr�e
        UpdateAvatarFromData(_teteIndex.Value, _bodyIndex.Value, _couleurAvatar.Value, _nomAvatar.Value);

        // Si je d�tecte un changement sur une des parties j'appelle la m�thode Update
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

    // M�thode qui met � jour l'avatar avec les donn�es re�u en param�tre
    public void UpdateAvatarFromData(int p_teteIndex, int p_bodyIndex, float p_couleurAvatar, FixedString128Bytes p_nomAvatar)
    {
        // Mise en place de l'�l�ment pour la t�te
        for (int i = 0; i < _optionsTete.Length; i++)
        {
            _optionsTete[i].SetActive(i == p_teteIndex);
        }

        // Mise en place de l'�l�ment pour le corps
        for (int i = 0; i < _optionsBody.Length; i++)
        {
            _optionsBody[i].SetActive(i == p_bodyIndex);
        }

        // Pour chaque parties du corps on change sa couleur par celle du gradient
        foreach (var item in _partiesCorps)
        {
            item.material.color = _gradientCouleurPeau.Evaluate(p_couleurAvatar);
        }

        // Change le nom avec le nom re�u
        _nomJoueur.text = p_nomAvatar.ToString();
    }
}
